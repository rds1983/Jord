using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wanderers.Core;
using Microsoft.Xna.Framework;
using Myra.Utility;
using Newtonsoft.Json.Linq;
using Module = Wanderers.Core.Module;
using Wanderers.Core.Items;
using Wanderers.Compiling.Loaders;
using Wanderers.Generation;

namespace Wanderers.Compiling
{
	public class Compiler
	{
		private const string ColorsName = "Colors";

		private readonly CompilerContext _context = new CompilerContext();
		private readonly Dictionary<Type, BaseLoader> _loaders = new Dictionary<Type, BaseLoader>();

		private MapLoader MapLoader
		{
			get
			{
				return (MapLoader)_loaders[typeof(Map)];
			}
		}

		public Compiler()
		{
			_loaders[typeof(Map)] = new MapLoader();
			_loaders[typeof(TileInfo)] = new Loader<TileInfo>("TileInfos");
			_loaders[typeof(CreatureInfo)] = new CreatureLoader();
			_loaders[typeof(BaseItemInfo)] = new ItemLoader();
			_loaders[typeof(Class)] = new Loader<Class>("Classes");
			_loaders[typeof(BaseGenerator)] = new GeneratorLoader();
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

		private void FirstRun(IEnumerable<string> sources)
		{
			if (CompilerParams.Verbose)
			{
				TJ.LogInfo("{0} source files found", sources.Count());
			}

			_context.Colors.Clear();

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
					if (key == ColorsName)
					{
						// Special case
						foreach (var pair2 in (JObject)pair.Value)
						{
							var c = ParseColor(pair2.Value);
							_context.Colors[pair2.Key] = c;

							if (CompilerParams.Verbose)
							{
								TJ.LogInfo("Parsed color '{0}': '{1}'", pair2.Key, c.ToString());
							}
						}

						continue;
					}
					else if (key == CompilerUtils.MapName)
					{
						// Another special case
						if (json.Count > 1)
						{
							throw new Exception(string.Format("Map file can have only one map entry. Source: '{0}'", s));
						}

						var obj = (JObject)pair.Value;
						JToken idToken;
						if (!obj.TryGetValue(CompilerUtils.IdName, out idToken))
						{
							throw new Exception(string.Format("Map object lacks id. Source: '{0}'", s));
						}

						var id = idToken.ToString();

						_loaders[typeof(Map)].SafelyAddObject(id, s, (JObject)pair.Value);

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

					foreach (var pair2 in (JObject)pair.Value)
					{
						loader.SafelyAddObject(pair2.Key, s, (JObject)pair2.Value);
					}
				}
			}
		}

		private void FillData<T>(Dictionary<string, T> output) where T : ItemWithId
		{
			((Loader<T>)_loaders[typeof(T)]).FillData(_context, output);
		}

		public Map LoadMapFromJson(string json)
		{
			var obj = (JObject)JObject.Parse(json)[CompilerUtils.MapName];
			var od = new ObjectData
			{
				Source = json,
				Data = obj
			};

			var id = obj[CompilerUtils.IdName].ToString();

			return (Map)MapLoader.LoadItem(_context, id, od);
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

		public Module Process(string path, bool skipMaps = false)
		{
			// First run - parse json and build object maps
			var sources = new List<string>();
			FindSources(path, true, sources, skipMaps);

			FirstRun(sources);

			// Second run - build module
			// Tile Infos
			FillData(_context.Module.TileInfos);

			// Item Infos
			FillData(_context.Module.ItemInfos);

			// Creature Infos
			FillData(_context.Module.CreatureInfos);

			// Classes
			FillData(_context.Module.Classes);

			// Generators
			FillData(_context.Module.GeneratorConfigs);

			if (!skipMaps)
			{
				// Maps
				FillData(_context.Module.Maps);
			}

			return _context.Module;
		}
	}
}