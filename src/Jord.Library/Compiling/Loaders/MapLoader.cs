﻿using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Jord.Core;

namespace Jord.Compiling.Loaders
{
	public class MapLoader: Loader<Map>
	{
		private class SpawnSpot
		{
		}

		private const string LegendName = "Legend";
		private const string TileInfoIdName = "TileInfoId";
		private const string TileObjectIdName = "TileObjectId";
		private const string CreatureInfoIdName = "CreatureInfoId";
		private const string DataName = "Data";
		private const string ForbiddenLegendItems = "{}[]";

		public MapLoader() : base("Maps")
		{
		}

		public override Map LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;

			if (module.Dungeons.ContainsKey(id))
			{
				RaiseError("There's already MapTemplate with id '{0}'", id);
			}

			var map = new Map(dataObj.EnsurePoint("Size"))
			{
				Local = dataObj.EnsureBool("Local")
			};

			map.Name = dataObj.EnsureString("Name");
			map.Explored = dataObj.OptionalBool("Explored", false);
			map.Light = dataObj.OptionalBool("Light", false);

			if (map.Explored)
			{

				foreach(var tile in map.GetAllCells())
				{
					tile.IsExplored = true;
				}
			}

			var legend = new Dictionary<char, object>();
			var legendObject = dataObj.EnsureJObject(LegendName);
			foreach (var pair in legendObject)
			{
				if (string.IsNullOrEmpty(pair.Key))
				{
					RaiseError("Map legend item id could not be empty.");
				}

				if (pair.Key.Length != 1)
				{
					RaiseError("Map legend item id {0} could consist only from single symbol.'", pair.Key);
				}

				var symbol = pair.Key[0];
				if (ForbiddenLegendItems.IndexOf(symbol) != -1)
				{
					RaiseError("Symbol '{0}' can't be used in legend.", symbol);
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

					do
					{

						var tileInfoId = obj.OptionalString(TileInfoIdName);
						if (!string.IsNullOrEmpty(tileInfoId))
						{
							item = module.TileInfos.Ensure(tileInfoId);
							break;
						}

						var tileObjectId = obj.OptionalString(TileObjectIdName);
						if (!string.IsNullOrEmpty(tileObjectId))
						{
							item = module.TileObjects.Ensure(tileObjectId);
							break;
						}

						var creatureInfoId = obj.OptionalString(CreatureInfoIdName);
						if (!string.IsNullOrEmpty(creatureInfoId))
						{
							item = module.CreatureInfos.Ensure(creatureInfoId);
							break;
						}
					}
					while (false);
				}

				legend[symbol] = item;
			}

			var dataObject = dataObj.EnsureJArray(DataName);

			var pos = Point.Zero;
			var entities = new List<Tuple<object, Point>>();
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
							RaiseError("Exit sequence lacks closing curly bracket.");
						}

						if (lastTile == null)
						{
							RaiseError("Last tile is null.");
						}

						lastTile.Exit = Exit.FromString(sb.ToString());

						continue;
					}

					if (symbol == '[')
					{
						// Exit
						var sb = new StringBuilder();
						++j;

						var hasEnd = false;
						for (; j < line.Length; ++j)
						{
							symbol = line[j];
							if (symbol == ']')
							{
								hasEnd = true;
								break;
							}

							sb.Append(symbol);
						}

						if (!hasEnd)
						{
							RaiseError("Sign sequence lacks closing square bracket.");
						}

						if (lastTile == null)
						{
							RaiseError("Last tile is null.");
						}

						lastTile.Sign = sb.ToString();

						continue;
					}

					if (!legend.TryGetValue(symbol, out item))
					{
						RaiseError("Unknown symbol '{0}'.", symbol);
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
						entities.Add(new Tuple<object, Point>(npc, lastTile.Position));
						continue;
					}

					var asObjectInfo = item as TileObject;
					if (asObjectInfo != null)
					{
						entities.Add(new Tuple<object, Point>(asObjectInfo, lastTile.Position));
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
				var asObjectInfo = entity.Item1 as TileObject;
				if (asObjectInfo != null)
				{
					map[entity.Item2].Object = asObjectInfo;
					continue;
				}

				var asCreature = entity.Item1 as Creature;
				if (asCreature != null)
				{
					asCreature.Place(map, entity.Item2);
					continue;
				}
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
			var tileObjects = new Dictionary<string, char>();
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

					if (tile.Object != null)
					{
						if (!tileObjects.ContainsKey(tile.Object.Id))
						{
							tileObjects[tile.Object.Id] = AddToLegend(tile.Object.Symbol, tile.Object, legend);
						}
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
				[Compiler.NameName] = map.Name,
				["Size"] = new JObject
				{ 
					["X"] = map.Width,
					["Y"] = map.Height,
				},
				["Explored"] = map.Explored,
				["Light"] = map.Light,
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

				var asTileObject = pair.Value as TileObject;
				if (asTileObject != null)
				{
					legendItemNode = new JObject
					{
						[TileObjectIdName] = asTileObject.Id
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

					if (tile.Object != null)
					{
						sb.Append(tileObjects[tile.Object.Id]);
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