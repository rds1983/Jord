using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TroublesOfJord.Core;

namespace TroublesOfJord.Compiling.Loaders
{
	public class MapLoader: Loader<Map>
	{
		private class SpawnSpot
		{
		}

		private const string LegendName = "Legend";
		private const string TileInfoIdName = "TileInfoId";
		private const string CreatureInfoIdName = "CreatureInfoId";
		private const string DataName = "Data";
		private const string ForbiddenLegendItems = "{}";

		public MapLoader() : base("Maps")
		{
		}

		private JObject EnsureObject(ObjectData root, string name)
		{
			JToken token;
			if (!root.Data.TryGetValue(name, out token))
			{
				RaiseError("Could not find mandatory node {0} for {1}, id '{2}', source = '{3}'",
					name, JsonArrayName, root.Data[Compiler.IdName], root.Source);
			}

			return (JObject)token;
		}

		private JArray EnsureArray(ObjectData root, string name)
		{
			JToken token;
			if (!root.Data.TryGetValue(name, out token))
			{
				RaiseError("Could not find mandatory node {0} for {1}, id '{2}', source = '{3}'",
					name, JsonArrayName, root.Data[Compiler.IdName], root.Source);
			}

			return (JArray)token;
		}

		public override Map LoadItem(Module module, string id, ObjectData data)
		{
			if (module.MapTemplates.ContainsKey(id))
			{
				RaiseError("There's already MapTemplate with id '{0}'", id);
			}

			var map = new Map(EnsurePoint(data.Data, data.Source, "Size"))
			{
				Local = EnsureBool(data, "Local")
			};

			map.Explored = OptionalBool(data, "Explored", false);

			if (map.Explored)
			{
				foreach(var tile in map.GetAllCells())
				{
					tile.IsExplored = true;
				}
			}

			var legend = new Dictionary<char, object>();
			var legendObject = EnsureObject(data, LegendName);
			foreach (var pair in legendObject)
			{
				if (string.IsNullOrEmpty(pair.Key))
				{
					RaiseError("Map legend item id could not be empty, source = '{0}'",
						data.Source);
				}

				if (pair.Key.Length != 1)
				{
					RaiseError("Map legend item id {0} could consist only from single symbol, source = '{1}'",
						pair.Key,
						data.Source);
				}

				var symbol = pair.Key[0];
				if (ForbiddenLegendItems.IndexOf(symbol) != -1)
				{
					RaiseError("Symbol '{0}' can't be used in legend. Source = '{1}'", symbol, data.Source);
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
							RaiseError("Could not find tileInfo with id '{0}', source = '{1}'",
								token,
								data.Source);
						}

						item = info;
					}

					if (obj.TryGetValue(CreatureInfoIdName, out token))
					{
						CreatureInfo info;
						if (!module.CreatureInfos.TryGetValue(token.ToString(), out info))
						{
							RaiseError("Could not find creatureInfo with id '{0}', source = '{1}'",
								token,
								data.Source);
						}

						item = info;
					}
				}

				legend[symbol] = item;
			}

			var dataObject = EnsureArray(data, DataName);

			var pos = Point.Zero;
			var entities = new List<Tuple<Creature, Point>>();
			for (var i = 0; i < dataObject.Count; ++i)
			{
				var lineToken = dataObject[i];
				var line = lineToken.ToString();

				pos.X = 0;
				for(var j = 0; j < line.Length; ++j)
				{
					Tile lastTile = null;
					if (pos.X > 0)
					{
						lastTile = map[pos.X - 1, pos.Y];
					}

					var symbol = line[j];
					object item;

					if (symbol == '{')
					{
						// Exit
						var sb = new StringBuilder();
						++j;

						var hasEnd = false;
						for(; j < line.Length; ++j)
						{
							symbol = line[j];
							if (symbol == '}')
							{
								hasEnd = true;
								break;
							}

							sb.Append(symbol);
						}

						if (!hasEnd)
						{
							RaiseError("Exit sequence lacks closing figure bracket. Source: '{0}'", data.Source);
						}

						if (lastTile == null)
						{
							RaiseError("Last tile is null. Source: '{0}'", data.Source);
						}

						lastTile.Exit = Exit.FromString(sb.ToString());

						continue;
					}

					if (!legend.TryGetValue(symbol, out item))
					{
						RaiseError("Unknown symbol '{0}', source = '{1}'",
							symbol,
							data.Source);
					}

					if (item is SpawnSpot)
					{
						map.SpawnSpot = lastTile.Position;
						continue;
					}

					var asCreatureInfo = item as CreatureInfo;
					if (asCreatureInfo != null)
					{
						var npc = new NonPlayer(asCreatureInfo);
						entities.Add(new Tuple<Creature, Point>(npc, lastTile.Position));
						continue;
					}

					var asTileInfo = item as TileInfo;
					if (asTileInfo != null)
					{
						map[pos].Info = asTileInfo;
					}

					++pos.X;
				}

				++pos.Y;
			}

			// Place entities
			foreach (var entity in entities)
			{
				entity.Item1.Place(map, entity.Item2);
			}

			return map;
		}

		private static char GenerateCode(Dictionary<char, object> legend, char code)
		{
			while (legend.ContainsKey(code) || ForbiddenLegendItems.IndexOf(code) != -1)
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
			for (var y = 0; y < map.Height; ++y)
			{
				for (var x = 0; x < map.Width; ++x)
				{
					var tile = map[x, y];

					if (!tileInfos.ContainsKey(tile.Info.Id))
					{
						tileInfos[tile.Info.Id] = AddToLegend(tile.Info.Symbol, tile.Info, legend);
					}

					if (tile.Creature != null)
					{
						var npc = (NonPlayer)tile.Creature;
						if (!creatureInfos.ContainsKey(npc.Info.Id))
						{
							creatureInfos[npc.Info.Id] = AddToLegend(npc.Info.Symbol, npc.Info, legend);
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

			var mapObject = new JObject
			{
				[Compiler.IdName] = map.Id,
				["Size"] = new JObject
				{ 
					["X"] = map.Width,
					["Y"] = map.Height,
				},
				["Explored"] = map.Explored,
				["Local"] = map.Local
			};

			root[Compiler.MapName] = mapObject;

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
					legendItemNode = new JValue("Spawn");
				}

				legendNode[pair.Key.ToString()] = legendItemNode;
			}

			mapObject[LegendName] = legendNode;

			// Data
			var dataArray = new JArray();
			var sb = new StringBuilder();
			for (var y = 0; y < map.Height; ++y)
			{
				sb.Clear();
				for (var x = 0; x < map.Width; ++x)
				{
					var tile = map[x, y];
					sb.Append(tileInfos[tile.Info.Id]);

					if (tile.Exit != null)
					{
						sb.Append('{');
						sb.Append(tile.Exit.ToString());
						sb.Append('}');
					}

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
	}
}