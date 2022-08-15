using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jord.Core;
using Newtonsoft.Json.Linq;
using Module = Jord.Core.Module;
using Jord.Core.Items;
using Jord.Compiling.Loaders;
using Jord.Generation;
using Jord.Core.Abilities;
using FontStashSharp;

namespace Jord.Compiling
{
	public class Compiler
	{
		public const string IdName = "Id";
		public const string NameName = "Name";
		public const string MapName = "Map";
		public const string TileSetName = "TileSet";
		public const string ModuleInfoName = "ModuleInfo";
		public const string LevelsName = "Levels";

		private const string PropertiesName = "Properties";

		private readonly Module _module = new Module();
		private readonly Dictionary<Type, BaseLoader> _loaders = new Dictionary<Type, BaseLoader>();
		private string _path;
		private ObjectData _moduleInfo;

		public Compiler()
		{
			_loaders[typeof(Map)] = new MapLoader();
			_loaders[typeof(Dungeon)] = new DungeonLoader();
			_loaders[typeof(TileInfo)] = new TileInfoLoader();
			_loaders[typeof(TileObject)] = new TileObjectLoader();
			_loaders[typeof(CreatureInfo)] = new CreatureLoader();
			_loaders[typeof(BaseItemInfo)] = new ItemLoader();
			_loaders[typeof(Class)] = new ClassLoader();
			_loaders[typeof(BaseGenerator)] = new GeneratorLoader();
			_loaders[typeof(AbilityInfo)] = new AbilityLoader();
		}

		private void FirstRun(IEnumerable<string> sources)
		{
			if (CompilerParams.Verbose)
			{
				TJ.LogInfo("{0} source files found", sources.Count());
			}

			// First run - parse json and build object maps
			foreach (var s in sources)
			{
				if (CompilerParams.Verbose)
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

					if (key == MapName)
					{
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

						_loaders[typeof(Map)].SafelyAddObject(id, s, (JObject)pair.Value);

						continue;
					}
					else if (key == ModuleInfoName)
					{
						var obj = (JObject)pair.Value;
						_moduleInfo = new ObjectData
						{
							Source = s,
							Data = obj
						};

						continue;
					}
					else if (key == LevelsName)
					{
						var arr = (JArray)pair.Value;
						foreach(JObject levelObject in arr)
						{
							var levelCost = new LevelCost(
								levelObject.EnsureInt("Level"),
								levelObject.EnsureInt("Experience"),
								levelObject.EnsureInt("Gold")
							);

							_module.LevelCosts[levelCost.Level] = levelCost;
						}

						continue;
					}

					BaseLoader loader = null;
					foreach (var pair2 in _loaders)
					{
						if (pair2.Value.JsonArrayName == key)
						{
							loader = pair2.Value;
							break;
						}
					}

					if (loader == null)
					{
						throw new Exception(string.Format("Unknown object type '{0}', source: '{1}", key, s));
					}

					var properties = new Dictionary<string, string>();

					JToken token;
					if (((JObject)pair.Value).TryGetValue(PropertiesName, out token))
					{
						foreach (var pair2 in (JObject)token)
						{
							properties[pair2.Key] = pair2.Value.ToString();
						}
					}

					foreach (var pair2 in (JObject)pair.Value)
					{
						if (pair2.Key == PropertiesName)
						{

							continue;
						}

						loader.SafelyAddObject(pair2.Key, s, (JObject)pair2.Value, properties);
					}
				}
			}
		}

		private void FillData<T>(Dictionary<string, T> output) where T : IBaseObject
		{
			((Loader<T>)_loaders[typeof(T)]).FillData(_module, output);
		}

		public void FindSources(string path, bool isTop, List<string> result)
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

				FindSources(sf, false, result);
			}
		}

		private string BuildFullPath(string relativePath) => Path.Combine(_path, relativePath);

		private ModuleInfo LoadModuleInfo(Module module, ObjectData od)
		{
			var fontSystem = new FontSystem();
			var path = BuildFullPath(od.Data.EnsureString("Font"));
			fontSystem.AddFont(File.ReadAllBytes(path));
			var font = fontSystem.GetFont(od.Data.EnsureInt("FontSize"));

			var symbolStr = od.Data.EnsureString("PlayerSymbol");
			if (symbolStr.Length != 1)
			{
				throw new Exception($"Unable to read '{symbolStr}' as symbol.");
			}

			var color = od.Data.EnsureColor("PlayerColor");
			var playerAppearance = new Appearance(symbolStr, color, null);

			return new ModuleInfo
			{
				Id = od.Data.EnsureId(),
				PlayerAppearance = playerAppearance,
				Font = font,
				Source = od.Source
			};
		}

		public Module Process(string path)
		{
			_path = path;

			// First run - parse json and build object maps
			var sources = new List<string>();
			FindSources(path, true, sources);

			FirstRun(sources);

			// Second run - build module

			// Module Info
			if (_moduleInfo == null)
			{
				throw new Exception("Couldn't find mandatory 'ModuleInfo' node");
			}

			_module.ModuleInfo = LoadModuleInfo(_module, _moduleInfo);

			// Tile Infos
			FillData(_module.TileInfos);

			// Tile Objects
			FillData(_module.TileObjects);

			// Item Infos
			FillData(_module.ItemInfos);

			// Creature Infos
			FillData(_module.CreatureInfos);

			// Classes
			FillData(_module.Classes);

			// Generators
			FillData(_module.Generators);

			// Maps
			FillData(_module.Maps);

			// Map templates
			FillData(_module.Dungeons);

			// Abilities
			FillData(_module.Abilities);

			return _module;
		}
	}
}