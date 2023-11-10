using Jord.Core;
using Jord.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Jord.Generation
{
	public class CityGenerator : BaseGenerator
	{
		private const int PiersCount = 4;
		private const int PiersWidth = 2;
		private const int RoadHeight = 3;

		public int BuildingsCount { get; }

		public CityGenerator(int width, int height, int buildingsCount) : base(width, height)
		{
			BuildingsCount = buildingsCount;
		}

		public override Map Generate()
		{
			var result = new Map(Width, Height);


			// Fill with grass
			var grass = TJ.Database.TileInfos["Grass"];
			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					result[x, y].Info = grass;
				}
			}

			Step();

			// Water
			var topWaterPost = Height - 1;
			var water = TJ.Database.TileInfos["Water"];
			var n = MathUtils.Random.NextDouble();
			for (var x = 0; x < Width; ++x)
			{
				var nw = (int)(Math.Sin(n) * 5.0) + 12 + MathUtils.Random.Next(1, 5);
				for (var y = 0; y < nw; ++y)
				{
					var y2 = Height - y - 1;
					result[x, y2].Info = water;

					if (y2 < topWaterPost)
					{
						topWaterPost = y2;
					}
				}

				n += 0.1;
			}

			--topWaterPost;

			Step();

			// Piers
			var woodFloor = TJ.Database.TileInfos["WoodFloor"];
			var freeSpace = Width - (PiersCount * PiersWidth);

			var freeSpaceBetweenPiers = freeSpace / (PiersCount + 1);

			var startX = freeSpaceBetweenPiers;
			for (var i = 0; i < PiersCount; ++i)
			{
				for (var y = Height - 5; ; --y)
				{
					var nonWaterCount = 0;
					for (var x = startX; x < startX + PiersWidth; ++x)
					{
						var tile = result[x, y];
						if (tile.Info != water)
						{
							++nonWaterCount;
						}

						tile.Info = woodFloor;
					}

					if (nonWaterCount == PiersWidth)
					{
						break;
					}
				}

				startX += PiersWidth;
				startX += freeSpaceBetweenPiers;
			}

			Step();

			// City floor
			var floor = TJ.Database.TileInfos["Floor"];
			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < topWaterPost; ++y)
				{
					result[x, y].Info = floor;
				}
			}

			Step();

			// Walls
			var wall = TJ.Database.TileInfos["Wall"];
			for (var x = 0; x < Width; ++x)
			{
				result[x, 0].Info = wall;
				result[x, topWaterPost - 1].Info = wall;
			}

			for (var y = 0; y < topWaterPost; ++y)
			{
				result[0, y].Info = wall;
				result[Width - 1, y].Info = wall;
			}

			Step();

			// Main road
			var road = TJ.Database.TileInfos["Road"];
			var roadTiles = new List<Point>();
			for (var x = 0; x < Width; ++x)
			{
				var startY = (topWaterPost - RoadHeight) / 2;
				for (var y = startY; y < startY + RoadHeight; ++y)
				{
					result[x, y].Info = road;
					roadTiles.Add(new Point(x, y));
				}
			}

			for (var y = 0; y < topWaterPost; ++y)
			{
				startX = (Width - RoadHeight) / 2;
				for (var x = startX; x < startX + RoadHeight; ++x)
				{
					result[x, y].Info = road;
					roadTiles.Add(new Point(x, y));
				}
			}

			Step();

			// Exit down
			result[Width / 2 - 1, topWaterPost / 2].Info = TJ.Database.TileInfos["ExitDown"];
			Step();


			// Buildings
			var openDoor = TJ.Database.TileInfos["OpenDoor"];
			for (var i = 0; i < BuildingsCount; ++i)
			{
				var found = false;
				int x = 0, y = 0, w = 0, h = 0;
				for (var tries = 0; tries < 10000; ++tries)
				{
					x = MathUtils.Random.Next(1, Width - 8);
					y = MathUtils.Random.Next(1, topWaterPost - 8);
					w = MathUtils.Random.Next(1, 8) + 4;
					h = MathUtils.Random.Next(1, 8) + 4;

					if (w % 2 == 0)
					{
						++w;
					}

					if (h % 2 == 0)
					{
						++h;
					}

					if (x + w >= Width - 1 ||
						y + h >= topWaterPost - 1)
					{
						continue;
					}

					// Check whether place is free
					found = true;
					for (var xx = x - 2; xx <= x + w + 2; ++xx)
					{
						for (var yy = y - 2; yy <= y + h + 2; ++yy)
						{
							if (xx < 0 || xx >= Width ||
								yy < 0 || yy >= topWaterPost)
							{
								continue;
							}

							if (result[xx, yy].Info != floor)
							{
								found = false;
								goto finish;
							}
						}
					}
				finish:;
					if (found)
					{
						break;
					}
				}

				if (!found)
				{
					continue;
				}

				for (var xx = x; xx < x + w; ++xx)
				{
					result[xx, y].Info = wall;
					result[xx, y + h - 1].Info = wall;
				}

				for (var yy = y; yy < y + h; ++yy)
				{
					result[x, yy].Info = wall;
					result[x + w - 1, yy].Info = wall;
				}

				var possibleExits = new List<Point>
				{
					new Point(x + w / 2, y),
					new Point(x + w / 2, y + h - 1),
					new Point(x, y + h / 2),
					new Point(x + w - 1, y + h / 2)
				};

				var exit = possibleExits[0];
				var roadConnection = roadTiles[0];
				float? dist = null;

				// Find closest way to the city road
				for (var j = 0; j < possibleExits.Count; ++j)
				{
					for (var k = 0; k < roadTiles.Count; ++k)
					{
						var d = Vector2.Distance(possibleExits[j].ToVector2(), roadTiles[k].ToVector2()); 
						if (dist == null || dist.Value > d)
						{
							dist = d;
							exit = possibleExits[j];
							roadConnection = roadTiles[k];
						}
					}
				}

				result[exit].Info = openDoor;
				var path = result.PathFinder.ShortestPath(exit.ToCoord(), roadConnection.ToCoord());
				foreach(var step in path.Steps)
				{
					result[step.X, step.Y].Info = road;
				}

			}

			Step();

			// Set spawn point at center
			result.SpawnSpot = new Point(Width / 2 - 1, topWaterPost / 2);

			return result;
		}
	}
}
