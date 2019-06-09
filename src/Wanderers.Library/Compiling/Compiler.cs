using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Wanderers.Core;
using Wanderers.Utils;
using Microsoft.Xna.Framework;
using Myra.Utility;
using Newtonsoft.Json.Linq;
using Module = Wanderers.Core.Module;
using System.Text;
using Wanderers.Core.Items;

namespace Wanderers.Compiling
{
	public class Compiler
	{
		private const string ColorsName = "colors";
		private const string TileInfosName = "tileInfos";
		private const string CreatureInfosName = "creatureInfos";
		private const string ItemInfosName = "itemInfos";
		private const string ClassesName = "classes";
		private const string MapName = "map";
		private const string MapsName = "maps";
		private const string IdName = "id";
		private const string LegendName = "legend";
		private const string TileInfoIdName = "tileInfoId";
		private const string CreatureInfoIdName = "creatureInfoId";
		private const string DataName = "data";

		private class SpawnSpot
		{
		}

		private class BaseData
		{
			public string Source { get; set; }
		}

		private class ColorData : BaseData
		{
			public Color Color { get; set; }
		}

		private class ObjectData : BaseData
		{
			public JObject Object { get; set; }
		}

		private readonly Dictionary<string, ColorData> _colors = new Dictionary<string, ColorData>();
		private readonly Dictionary<string, Dictionary<string, ObjectData>> _sourceData = new Dictionary<string, Dictionary<string, ObjectData>>();
		private readonly Dictionary<Type, List<PropertyInfo>> _propsCache = new Dictionary<Type, List<PropertyInfo>>();
		private Module _result;

		public CompilerParams Params;

		private void FirstRun(IEnumerable<string> sources)
		{
			if (Params.Verbose)
			{
				TJ.LogInfo("{0} source files found", sources.Count());
			}

			// First run - parse json and build object maps
			foreach (var s in sources)
			{
				if (Params.Verbose)
				{
					TJ.LogInfo("Processing {0}", s);
				}

				JObject json;

				try
				{
					json = JObject.Parse(File.ReadAllText(s));
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("JSON parsing error. Source '{0}'. Error '{1}'",
						s, ex.ToString()));
				}

				foreach (var pair in json)
				{
					var key = pair.Key;
					if (key == ColorsName)
					{
						// Special case
						foreach (var pair2 in (JObject)pair.Value)
						{
							ColorData cd;
							if (_colors.TryGetValue(pair2.Key, out cd))
							{
								throw new Exception(string.Format("Two colors with same id '{0}' had been declared. Conflicting source: '{1}' and '{2}'",
									pair2.Key, s, cd.Source));
							}

							var c = ParseColor(pair2.Value);

							cd = new ColorData
							{
								Source = s,
								Color = c
							};

							_colors[pair2.Key] = cd;

							if (Params.Verbose)
							{
								TJ.LogInfo("Parsed color '{0}': '{1}'", pair2.Key, cd.Color.ToString());
							}
						}

						continue;
					}
					else if (key == MapName)
					{
						// Another special case
						if (json.Count > 1)
						{
							throw new Exception(string.Format("Map file can have only one map entry. Source: '{0}'", s));
						}

						var obj = (JObject)pair.Value;
						JToken idToken;
						if (!obj.TryGetValue(IdName, out idToken))
						{
							throw new Exception(string.Format("Map object lacks id. Source: '{0}'", s));
						}

						var id = idToken.ToString();

						SafelyAddObject(MapsName, id, s, (JObject)pair.Value);

						continue;
					}
					else if (key == TileInfosName || key == ClassesName || key == CreatureInfosName || key == ItemInfosName)
					{
					}
					else
					{
						throw new Exception(string.Format("Unknown object type '{0}', source: '{1}", key, s));
					}

					foreach (var pair2 in (JObject)pair.Value)
					{
						SafelyAddObject(key, pair2.Key, s, (JObject)pair2.Value);
					}
				}
			}
		}

		private static List<PropertyInfo> BuildProperties(Type type)
		{
			var props = new List<PropertyInfo>();
			var properties = from p in type.GetProperties() select p;
			foreach (var property in properties)
			{
				if (property.GetGetMethod() == null ||
					!property.GetGetMethod().IsPublic ||
					property.GetGetMethod().IsStatic ||
					property.GetSetMethod() == null ||
					!property.GetSetMethod().IsPublic)
				{
					continue;
				}

				var ignoreFieldAttr = property.FindAttribute<IgnoreFieldAttribute>();
				if (ignoreFieldAttr != null)
				{
					continue;
				}

				props.Add(property);
			}

			return props;
		}

		private static List<PropertyInfo> BuildProperties<T>()
		{
			return BuildProperties(typeof(T));
		}

		private List<PropertyInfo> GetProperties(Type type)
		{
			List<PropertyInfo> result;
			if (_propsCache.TryGetValue(type, out result))
			{
				return result;
			}

			result = BuildProperties(type);
			_propsCache[type] = result;

			return result;
		}

		private List<PropertyInfo> GetProperties<T>()
		{
			return GetProperties(typeof(T));
		}

		private static ItemWithId LoadObject(Type type, Compiler compiler, List<PropertyInfo> props, string id, ObjectData od, Action<ObjectData, ItemWithId> customProcess)
		{
			var item = (ItemWithId)Activator.CreateInstance(type);

			item.Id = id;
			foreach (var p in props)
			{
				if (item is WeaponInfo && p.Name == "SubType")
				{
					// Special case
					continue;
				}

				if (p.PropertyType == typeof(Appearance))
				{
					// Special case
					var symbol = od.Object["symbol"].ToString()[0];
					var color = GetColor(compiler._colors, od.Source, od.Object["color"].ToString());

					var appearance = new Appearance(symbol, color);

					p.SetValue(item, appearance);

					continue;
				}

				var name = p.Name.LowercaseFirstLetter();

				var optionalFieldAttr = p.FindAttribute<OptionalFieldAttribute>();

				JToken token;
				if (!od.Object.TryGetValue(name, out token) && optionalFieldAttr == null)
				{
					throw new Exception(string.Format(
						"Could not find mandatory field {0} for {1}, id: '{2}', source: '{3}'",
						name, type.Name, id, od.Source));
				}

				if (p.PropertyType == typeof(string))
				{
					p.SetValue(item, token.ToString());
				}
				else if (p.PropertyType == typeof(Color))
				{
					var c = GetColor(compiler._colors, od.Source, token.ToString());
					p.SetValue(item, c);
				}
				else if (p.PropertyType.IsPrimitive)
				{
					var val = Convert.ChangeType(token.ToString(), p.PropertyType);
					p.SetValue(item, val);
				}
				else if (p.PropertyType.IsEnum)
				{
					var enumValue = Enum.Parse(p.PropertyType, token.ToString().UppercaseFirstLetter());
					p.SetValue(item, enumValue);
				}
			}

			if (customProcess != null)
			{
				customProcess(od, item);
			}

			return item;
		}

		private static T LoadObject<T>(Compiler compiler, List<PropertyInfo> props, string id, ObjectData od, Action<ObjectData, T> customProcess)
			where T : ItemWithId, new()
		{
			return (T)LoadObject(typeof(T), compiler, props, id, od, (o, t) =>
			{
				customProcess?.Invoke(o, (T)t);
			});
		}

		private void FillData<T>(string dictName, Dictionary<string, T> output, Action<ObjectData, T> customProcess = null)
			where T : ItemWithId, new()
		{
			if (!_sourceData.ContainsKey(dictName))
			{
				return;
			}

			var dict = _sourceData[dictName];
			var props = GetProperties<T>();

			foreach (var pair in dict)
			{
				var item = LoadObject(this, props, pair.Key, pair.Value, customProcess);
				output[item.Id] = item;

				if (Params.Verbose)
				{
					TJ.LogInfo("Added to {0}, id: '{1}', value: '{2}'", dictName, item.Id, item.ToString());
				}
			}
		}

		private void FillItems(Dictionary<string, BaseItemInfo> output)
		{
			var dictName = ItemInfosName;
			if (!_sourceData.ContainsKey(dictName))
			{
				return;
			}

			var dict = _sourceData[dictName];

			var assembly = GetType().Assembly;
			foreach (var pair in dict)
			{
				var typeName = pair.Value.Object["type"].ToString();

				var fullTypeName = "Wanderers.Core.Items." + typeName.UppercaseFirstLetter() + "Info";
				var type = assembly.GetType(fullTypeName);

				if (type == null)
				{
					throw new Exception(string.Format("Could not resolve item type '{0}'", typeName));
				}

				var props = GetProperties(type);
				var item = (BaseItemInfo)LoadObject(type, this, props, pair.Key, pair.Value, null);
				output[item.Id] = item;

				if (Params.Verbose)
				{
					TJ.LogInfo("Added to {0}, id: '{1}', value: '{2}'", dictName, item.Id, item.ToString());
				}
			}
		}

		private static Color GetColor(Dictionary<string, ColorData> colors, string source, string name)
		{
			Color result;
			if (name.StartsWith("#"))
			{
				result = ParseColor(name);
			}
			else
			{
				ColorData cd;
				if (!colors.TryGetValue(name, out cd))
				{
					throw new Exception(string.Format("Could not find color with id '{0}', source = '{1}'", name, source));
				}

				result = cd.Color;
			}

			return result;
		}


		private Color GetColor(string source, string name)
		{
			return GetColor(_colors, source, name);
		}

		private static JObject EnsureObject(ObjectData root, string name, string dictName)
		{
			JToken token;
			if (!root.Object.TryGetValue(name, out token))
			{
				throw new Exception(string.Format("Could not find mandatory node {0} for {1}, id '{2}', source = '{3}'",
					name, dictName, root.Object[IdName], root.Source));
			}

			return (JObject)token;
		}

		private static  JArray EnsureArray(ObjectData root, string name, string dictName)
		{
			JToken token;
			if (!root.Object.TryGetValue(name, out token))
			{
				throw new Exception(string.Format("Could not find mandatory node {0} for {1}, id '{2}', source = '{3}'",
					name, dictName, root.Object[IdName], root.Source));
			}

			return (JArray)token;
		}

		private static void ProcessMap(Module module, ObjectData data, Map map)
		{
			var legend = new Dictionary<char, object>();
			var legendObject = EnsureObject(data, LegendName, MapsName);
			foreach (var pair in legendObject)
			{
				if (string.IsNullOrEmpty(pair.Key))
				{
					throw new Exception(string.Format("Map legend item id could not be empty, source = '{0}'",
						data.Source));
				}

				if (pair.Key.Length != 1)
				{
					throw new Exception(string.Format("Map legend item id {0} could consist only from single symbol, source = '{1}'",
						pair.Key,
						data.Source));
				}

				object item = null;

				var asValue = pair.Value as JValue;
				if (asValue != null)
				{
					// Spawn
					item = new SpawnSpot();
				}
				else
				{
					var obj = (JObject)pair.Value;

					JToken token;
					if (obj.TryGetValue(TileInfoIdName, out token))
					{
						TileInfo info;
						if (!module.TileInfos.TryGetValue(token.ToString(), out info))
						{
							throw new Exception(string.Format("Could not find tileInfo with id '{0}', source = '{1}'",
								token,
								data.Source));
						}

						item = info;
					}

					if (obj.TryGetValue(CreatureInfoIdName, out token))
					{
						CreatureInfo info;
						if (!module.CreatureInfos.TryGetValue(token.ToString(), out info))
						{
							throw new Exception(string.Format("Could not find creatureInfo with id '{0}', source = '{1}'",
								token,
								data.Source));
						}

						item = info;
					}
				}

				legend[pair.Key[0]] = item;
			}

			var dataObject = EnsureArray(data, DataName, MapsName);
			var lines = new List<List<Tile>>();

			int? width = null;
			var pos = Point.Zero;

			var entities = new List<Tuple<Creature, Vector2>>();
			for (var i = 0; i < dataObject.Count; ++i)
			{
				var lineToken = dataObject[i];
				var lineData = new List<Tile>();
				var line = lineToken.ToString();

				pos.X = 0;
				Tile tile = null;
				foreach (var symbol in line)
				{
					object item;
					if (!legend.TryGetValue(symbol, out item))
					{
						throw new Exception(string.Format("Unknown symbol '{0}', source = '{1}'",
							symbol,
							data.Source));
					}

					if (item is SpawnSpot)
					{
						map.SpawnSpot = tile.Position;
						continue;
					}

					var asCreatureInfo = item as CreatureInfo;
					if (asCreatureInfo != null)
					{
						var npc = new NonPlayer(asCreatureInfo);
						entities.Add(new Tuple<Creature, Vector2>(npc, tile.Position.ToVector2()));
						continue;
					}

					// New tile
					tile = new Tile
					{
						Position = pos
					};

					var asTileInfo = item as TileInfo;
					if (asTileInfo != null)
					{
						tile.Info = asTileInfo;
					}

					lineData.Add(tile);

					++pos.X;
				}

				if (width == null)
				{
					width = lineData.Count;
				}
				else if (width.Value != lineData.Count)
				{
					throw new Exception(string.Format("All map lines should have same width. Map width '{0}', this line('{1}') width '{2}', source = '{3}'",
						width.Value,
						i,
						lineData.Count,
						data.Source));
				}

				lines.Add(lineData);

				++pos.Y;
			}

			map.Size = new Point(width.Value, lines.Count);

			for (var i = 0; i < lines.Count; ++i)
			{
				var line = lines[i];
				for (var j = 0; j < line.Count; ++j)
				{
					map.SetTileAt(new Point(j, i), line[j]);
				}
			}

			// Place entities
			foreach (var entity in entities)
			{
				entity.Item1.Place(map, entity.Item2);
			}
		}

		private void ProcessMap(ObjectData data, Map map)
		{
			ProcessMap(_result, data, map);
		}

		public static MapData LoadMapData(Compiler compiler, string path)
		{
			var folder = Path.GetDirectoryName(path);
			var name = Path.GetFileNameWithoutExtension(path);
			var files = Directory.EnumerateFiles(folder, name + ".*.json", SearchOption.TopDirectoryOnly);

			var compiler2 = new Compiler();
			compiler2.FirstRun(files);

			foreach (var pair in compiler._colors)
			{
				compiler2._colors[pair.Key] = pair.Value;
			}

			// Second run - build data
			var result = new MapData();

			// Tile Infos
			compiler2.FillData(TileInfosName, result.TileInfos);

			// Creature Infos
			compiler2.FillData(CreatureInfosName, result.CreatureInfos);

			return result;
		}

		public static Map LoadMapFromJson(Compiler compiler, Module module, string json)
		{
			var obj = (JObject)JObject.Parse(json)[MapName];
			var od = new ObjectData
			{
				Source = json,
				Object = obj
			};

			var id = obj[IdName].ToString();

			var props = BuildProperties<Map>();
			var result = LoadObject<Map>(compiler, props, id,
				od,	(o, m) => ProcessMap(module, o, m));

			return result;
		}

		private static char GenerateCode(Dictionary<char, object> legend, char code)
		{
			while (legend.ContainsKey(code))
			{
				++code;
			}

			return code;
		}

		private static char AddToLegend(char code, object item, Dictionary<char, object> legend)
		{
			code = GenerateCode(legend, code);
			legend[code] = item;

			return code;
		}

		public static string SaveMapToString(Map map)
		{
			// First run - build legend
			var legend = new Dictionary<char, object>();
			var tileInfos = new Dictionary<string, char>();
			var creatureInfos = new Dictionary<string, char>();
			for (var y = 0; y < map.Size.Y; ++y)
			{
				for (var x = 0; x < map.Size.X; ++x)
				{
					var tile = map.GetTileAt(x, y);

					if (!tileInfos.ContainsKey(tile.Info.Id))
					{
						tileInfos[tile.Info.Id] = AddToLegend(tile.Info.Image.Symbol, tile.Info, legend);
					}

					if (tile.Creature != null)
					{
						var npc = (NonPlayer)tile.Creature;
						if (!creatureInfos.ContainsKey(npc.Info.Id))
						{
							creatureInfos[npc.Info.Id] = AddToLegend(npc.Info.Image.Symbol, npc.Info, legend);
						}
					}
				}
			}

			var spawnSpotCode = '*';

			if (map.SpawnSpot != null)
			{
				spawnSpotCode = AddToLegend(spawnSpotCode, new SpawnSpot(), legend);
			}

			// Build json object
			var root = new JObject();
			var mapObject = new JObject();

			root[MapName] = mapObject;

			mapObject[IdName] = map.Id;
			mapObject["local"] = map.Local;

			var legendNode = new JObject();
			foreach (var pair in legend)
			{
				JToken legendItemNode = null;

				var asTileInfo = pair.Value as TileInfo;
				if (asTileInfo != null)
				{
					legendItemNode = new JObject
					{
						[TileInfoIdName] = asTileInfo.Id
					};
				}

				var asCreatureInfo = pair.Value as CreatureInfo;
				if (asCreatureInfo != null)
				{
					legendItemNode = new JObject
					{
						[CreatureInfoIdName] = asCreatureInfo.Id
					};
				}

				var asSpawnSpot = pair.Value as SpawnSpot;
				if (asSpawnSpot != null)
				{
					legendItemNode = new JValue("spawn");
				}

				legendNode[pair.Key.ToString()] = legendItemNode;
			}

			mapObject[LegendName] = legendNode;

			// Data
			var dataArray = new JArray();
			var sb = new StringBuilder();
			for (var y = 0; y < map.Size.Y; ++y)
			{
				sb.Clear();
				for (var x = 0; x < map.Size.X; ++x)
				{
					var tile = map.GetTileAt(x, y);
					sb.Append(tileInfos[tile.Info.Id]);

					if (tile.Creature != null)
					{
						var npc = (NonPlayer)tile.Creature;
						sb.Append(creatureInfos[npc.Info.Id]);
					}

					if (map.SpawnSpot == new Point(x, y))
					{
						sb.Append(spawnSpotCode);
					}
				}

				dataArray.Add(sb.ToString());
			}

			mapObject[DataName] = dataArray;

			return root.ToString();
		}

		public void FindSources(string path, bool isTop, List<string> result, bool skipMaps)
		{
			var files = Directory.EnumerateFiles(path, "*.json", SearchOption.TopDirectoryOnly);
			result.AddRange(files);

			var subFolders = Directory.EnumerateDirectories(path);

			foreach (var sf in subFolders)
			{
				if (isTop && sf == "modules")
				{
					continue;
				}

				if (skipMaps && sf.EndsWith("maps"))
				{
					continue;
				}

				FindSources(sf, false, result, skipMaps);
			}
		}

		private void ProcessCreature(ObjectData data, CreatureInfo creature)
		{
			JToken t;
			if (data.Object.TryGetValue("inventory", out t))
			{
				JObject obj = (JObject)t;

				var inventory = new Inventory();

				foreach (var pair in obj)
				{
					inventory.Items.Add(new ItemPile
					{
						Item = new Item(_result.ItemInfos[pair.Key]),
						Quantity = (int)pair.Value
					});
				}

				creature.Inventory = inventory;
			}
		}


		public Module Process(string path, bool skipMaps = false)
		{
			// First run - parse json and build object maps
			var sources = new List<string>();
			FindSources(path, true, sources, skipMaps);

			FirstRun(sources);

			// Second run - build module
			_result = new Module();

			// Tile Infos
			FillData(TileInfosName, _result.TileInfos);

			// Item Infos
			FillItems(_result.ItemInfos);

			// Creature Infos
			FillData(CreatureInfosName, _result.CreatureInfos, ProcessCreature);

			// Classes
			FillData(ClassesName, _result.Classes);

			if (!skipMaps)
			{
				// Maps
				FillData(MapsName, _result.Maps, ProcessMap);
			}

			return _result;
		}

		private Dictionary<string, ObjectData> GetObjectDict(string name)
		{

			Dictionary<string, ObjectData> dict;
			if (!_sourceData.TryGetValue(name, out dict))
			{
				dict = new Dictionary<string, ObjectData>();
				_sourceData[name] = dict;
			}

			return dict;
		}

		private void SafelyAddObject(string dictName, string id, string source, JObject obj)
		{
			var objectDict = GetObjectDict(dictName);

			ObjectData od;
			if (objectDict.TryGetValue(id, out od))
			{
				throw new Exception(string.Format(
					"Two {0} with same id '{1}' had been declared. Conflicting source: '{2}' and '{3}'",
					dictName, id, source, od.Source));
			}

			od = new ObjectData
			{
				Source = source,
				Object = obj
			};

			objectDict[id] = od;
		}

		private static Color ParseColor(JToken source)
		{
			var s = source.ToString();
			var c = s.FromName();
			if (c == null)
			{
				throw new Exception(string.Format("Could not parse color '{0}'", s));
			}

			return c.Value;
		}
	}
}