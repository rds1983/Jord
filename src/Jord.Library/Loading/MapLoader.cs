using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Jord.Core;

namespace Jord.Loading
{
	public class MapLoader: BaseObjectLoader<Map>
	{
		public static readonly MapLoader Instance = new MapLoader();

		private class SpawnSpot
		{
		}

		private const string LegendName = "Legend";
		private const string TileInfoIdName = "TileInfoId";
		private const string TileObjectIdName = "TileObjectId";
		private const string CreatureInfoIdName = "CreatureInfoId";
		private const string DataName = "Data";
		private const string ForbiddenLegendItems = "{}[]";

		protected override Map CreateObject(string source, JObject data, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			var map = new Map(data.EnsurePoint("Size"))
			{
				Id = data.EnsureId(),
				Name = data.EnsureString("Name"),
				Explored = data.OptionalBool("Explored", false),
				Light = data.OptionalBool("Light", false)
			};

			if (map.Explored)
			{
				foreach(var tile in map.GetAllCells())
				{
					tile.IsExplored = true;
				}
			}

			secondRunAction = db => SecondRun(map, data, db);

			return map;
		}

		private void SecondRun(Map result, JObject data, Database database)
		{
			if (database.Dungeons.ContainsKey(result.Id))
			{
				RaiseError($"There's already MapTemplate with id '{result.Id}'");
			}

			var legend = new Dictionary<char, object>();
			var legendObject = data.EnsureJObject(LegendName);
			foreach (var pair in legendObject)
			{
				if (string.IsNullOrEmpty(pair.Key))
				{
					RaiseError("Map legend item id could not be empty.");
				}

				if (pair.Key.Length != 1)
				{
					RaiseError($"Map legend item id {pair.Key} could consist only from single symbol.'");
				}

				var symbol = pair.Key[0];
				if (ForbiddenLegendItems.IndexOf(symbol) != -1)
				{
					RaiseError($"Symbol '{symbol}' can't be used in legend.");
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
							item = database.TileInfos.Ensure(tileInfoId);
							break;
						}

						var tileObjectId = obj.OptionalString(TileObjectIdName);
						if (!string.IsNullOrEmpty(tileObjectId))
						{
							item = database.TileObjects.Ensure(tileObjectId);
							break;
						}

						var creatureInfoId = obj.OptionalString(CreatureInfoIdName);
						if (!string.IsNullOrEmpty(creatureInfoId))
						{
							item = database.CreatureInfos.Ensure(creatureInfoId);
							break;
						}
					}
					while (false);
				}

				legend[symbol] = item;
			}

			var dataObject = data.EnsureJArray(DataName);

			var pos = Point.Zero;
			var entities = new List<Tuple<object, Point>>();
			for (var i = 0; i < dataObject.Count; ++i)
			{
				var lineToken = dataObject[i];
				var line = lineToken.ToString();

				pos.X = 0;
				for (var j = 0; j < line.Length; ++j)
				{
					Tile lastTile = null;
					if (pos.X > 0)
					{
						lastTile = result[pos.X - 1, pos.Y];
					}

					var symbol = line[j];
					object item;

					if (symbol == '{')
					{
						// Exit
						var sb = new StringBuilder();
						++j;

						var hasEnd = false;
						for (; j < line.Length; ++j)
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
						RaiseError($"Unknown symbol '{symbol}'.");
					}

					if (item is SpawnSpot)
					{
						result.SpawnSpot = lastTile.Position;
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
						result[pos].Info = asTileInfo;
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
					result[entity.Item2].Object = asObjectInfo;
					continue;
				}

				var asCreature = entity.Item1 as Creature;
				if (asCreature != null)
				{
					asCreature.Place(result, entity.Item2);
					continue;
				}
			}
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
				["Id"] = map.Id,
				["Name"] = map.Name,
				["Size"] = new JObject
				{ 
					["X"] = map.Width,
					["Y"] = map.Height,
				},
				["Explored"] = map.Explored,
				["Light"] = map.Light,
			};

			root["Map"] = mapObject;

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