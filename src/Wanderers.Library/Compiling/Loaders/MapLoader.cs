﻿using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Wanderers.Core;

namespace Wanderers.Compiling.Loaders
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

		public MapLoader() : base("Maps")
		{
		}

		private JObject EnsureObject(ObjectData root, string name)
		{
			JToken token;
			if (!root.Object.TryGetValue(name, out token))
			{
				throw new Exception(string.Format("Could not find mandatory node {0} for {1}, id '{2}', source = '{3}'",
					name, JsonArrayName, root.Object[CompilerUtils.IdName], root.Source));
			}

			return (JObject)token;
		}

		private JArray EnsureArray(ObjectData root, string name)
		{
			JToken token;
			if (!root.Object.TryGetValue(name, out token))
			{
				throw new Exception(string.Format("Could not find mandatory node {0} for {1}, id '{2}', source = '{3}'",
					name, JsonArrayName, root.Object[CompilerUtils.IdName], root.Source));
			}

			return (JArray)token;
		}

		public override ItemWithId LoadItem(CompilerContext context, string id, ObjectData data)
		{
			var map = (Map)base.LoadItem(context, id, data);

			var legend = new Dictionary<char, object>();
			var legendObject = EnsureObject(data, LegendName);
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
						if (!context.Module.TileInfos.TryGetValue(token.ToString(), out info))
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
						if (!context.Module.CreatureInfos.TryGetValue(token.ToString(), out info))
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

			var dataObject = EnsureArray(data, DataName);
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

			return map;
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

			root[CompilerUtils.MapName] = mapObject;

			mapObject[CompilerUtils.IdName] = map.Id;
			mapObject["Local"] = map.Local;

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
	}
}