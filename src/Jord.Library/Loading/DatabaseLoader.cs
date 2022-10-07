using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jord.Core;
using Newtonsoft.Json.Linq;

namespace Jord.Loading
{
	public class DatabaseLoader
	{
		private class SecondRunItem
		{
			public readonly BaseObject Item;
			public readonly JObject Data;

			public SecondRunItem(BaseObject item, JObject data)
			{
				Item = item;
				Data = data;
			}
		}

		private Database _database;
		private readonly List<Action<Database>> _secondRunActions = new List<Action<Database>>();

		protected static void RaiseError(string message) => LoaderExtensions.RaiseError(message);

		private static void LoadProperties(JObject propertiesDict, Dictionary<string, string> properties)
		{
			foreach (var pair in propertiesDict)
			{
				properties[pair.Key] = pair.Value.ToString();
			}
		}

		private T FirstRunLoadObject<T>(string source, JObject value, Dictionary<string, string> properties, BaseObjectLoader<T> loader) where T : BaseObject
		{
			Action<Database> secondRunAction;
			var item = loader.LoadObject(source, value, properties, out secondRunAction);

			if (secondRunAction != null)
			{
				_secondRunActions.Add(secondRunAction);
			}

			return item;
		}

		private void FirstRunDictionary<T>(string source, JObject dataDict, BaseObjectLoader<T> loader, Dictionary<string, T> dbDict) where T : BaseObject
		{
			var properties = new Dictionary<string, string>();
			foreach (var pair in dataDict)
			{
				var id = pair.Key;
				var value = (JObject)pair.Value;

				if (id == "Properties")
				{
					LoadProperties(value, properties);
					continue;
				}

				var item = FirstRunLoadObject(source, value, properties, loader);

				item.Id = id;
				dbDict[id] = item;
			}
		}

		private void LoadLevels(JArray arr)
		{
			foreach (JObject levelObject in arr)
			{
				var levelCost = new LevelCost(
					levelObject.EnsureInt("Level"),
					levelObject.EnsureInt("Experience"),
					levelObject.EnsureInt("Gold")
				);

				_database.LevelCosts[levelCost.Level] = levelCost;
			}
		}

		private void FirstRun(IEnumerable<string> sources)
		{
			if (LoadSettings.Verbose)
			{
				TJ.LogInfo($"{sources.Count()} source files found");
			}

			// First run - parse json and build object maps
			foreach (var s in sources)
			{
				if (LoadSettings.Verbose)
				{
					TJ.LogInfo($"Processing {s}");
				}

				JObject json = null;

				try
				{
					json = JObject.Parse(File.ReadAllText(s));
				}
				catch (Exception ex)
				{
					RaiseError($"JSON parsing error. Source '{s}'. Error '{ex}'");
				}

				foreach (var pair in json)
				{
					var key = pair.Key;

					switch (key)
					{
						case "Abilities":
							FirstRunDictionary(s, (JObject)pair.Value, AbilityLoader.Instance, _database.Abilities);
							break;
						case "Classes":
							FirstRunDictionary(s, (JObject)pair.Value, ClassLoader.Instance, _database.Classes);
							break;
						case "GeneratorConfigs":
							FirstRunDictionary(s, (JObject)pair.Value, GeneratorLoader.Instance, _database.Generators);
							break;
						case "ItemInfos":
							FirstRunDictionary(s, (JObject)pair.Value, ItemLoader.Instance, _database.ItemInfos);
							break;
						case "CreatureInfos":
							FirstRunDictionary(s, (JObject)pair.Value, CreatureInfoLoader.Instance, _database.CreatureInfos);
							break;
						case "TileInfos":
							FirstRunDictionary(s, (JObject)pair.Value, TileInfoLoader.Instance, _database.TileInfos);
							break;
						case "TileObjects":
							FirstRunDictionary(s, (JObject)pair.Value, TileObjectLoader.Instance, _database.TileObjects);
							break;
						case "Dungeons":
							FirstRunDictionary(s, (JObject)pair.Value, DungeonLoader.Instance, _database.Dungeons);
							break;
						case "Levels":
							LoadLevels((JArray)pair.Value);
							break;
						case "Settings":
							_database.ModuleInfo = FirstRunLoadObject(s, (JObject)pair.Value, null, SettingsLoader.Instance);
							break;
						case "Map":
							if (json.Count > 1)
							{
								RaiseError($"Map file can have only one map entry. Source: '{s}'");
							}

							var map = FirstRunLoadObject(s, (JObject)pair.Value, null, MapLoader.Instance);
							_database.Maps[map.Id] = map;
							break;
						default:
							RaiseError($"Unknown object type '{key}', source '{s}'.");
							break;
					}
				}
			}
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

		public Database Process(string path)
		{
			_database = new Database();
			_secondRunActions.Clear();

			// First run - parse json and build object maps
			var sources = new List<string>();
			FindSources(path, true, sources);

			// First run - parse basic data
			FirstRun(sources);

			// Second run - resolve references
			foreach(var action in _secondRunActions)
			{
				action(_database);
			}

			return _database;
		}
	}
}