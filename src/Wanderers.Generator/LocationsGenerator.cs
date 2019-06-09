using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Wanderers.Core;
using Wanderers.Utils;

namespace Wanderers.Generator
{
	public class LocationsGenerator
	{
		private const int CityWidth = 65;
		private const int CityHeight = 65;
		private const int LocationPadding = 10;

		private readonly GenerationResult _result;
		private readonly Random _random = new Random();
		private readonly HashSet<int> _roadTiles = new HashSet<int>();

		public LocationsGenerator(GenerationResult result)
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}

			_result = result;
		}

		private void AddToRoadTiles(Point p)
		{
			var h = p.X + (p.Y * _result.Width);

			_roadTiles.Add(h);
		}

		private void Connect(int sourceIndex, int destIndex)
		{
			if (sourceIndex == destIndex)
			{
				return;
			}

			var source = _result.Locations[sourceIndex];
			var dest = _result.Locations[destIndex];

			TJ.LogInfo("Building road beetween '{0}' and '{1}'...",
				source.Config.Name,
				dest.Config.Name);

			// Add dest entrances to road tiles
			foreach (var s in dest.EntranceLocations)
			{
				AddToRoadTiles(s);
			}

			// Find closest location
			float? closestD = null;
			var destPos = Point.Zero;
			var startPos = Point.Zero;

			foreach (var s in source.EntranceLocations)
			{
				foreach (var h in _roadTiles)
				{
					var p = new Point(h % _result.Width, h / _result.Height);
					var d = Vector2.Distance(p.ToVector2(), s.ToVector2());

					if (closestD == null || closestD.Value > d)
					{
						closestD = d;
						startPos = s;
						destPos = p;
					}
				}
			}

			var pathFinder = new PathFinder(startPos,
				destPos,
				new Point(_result.Width, _result.Height),
				p => _result.IsRoadPlaceable(p),
				p => p == destPos);

			var steps = pathFinder.Process();

			if (steps == null || steps.Length == 0)
			{
				return;
			}

			foreach (var step in steps)
			{
				_result.SetWorldMapTileType(step.Position, WorldMapTileType.Road);
				AddToRoadTiles(step.Position);
			}
		}

		public void Generate()
		{
			if (Config.Instance.Locations.Count == 0)
			{
				return;
			}

			var pathSet = new bool[_result.Height, _result.Width];
			var areas = new List<Rectangle>();

			// Draw cities
			for (var i = 0; i < Config.Instance.Locations.Count; ++i)
			{
				var locationConfig = Config.Instance.Locations[i];

				TJ.LogInfo("Generating location {0}...", locationConfig.Name);

				// Generate city size
				int width = CityWidth;
				int height = CityHeight;

				int totalWidth = width + 2 * LocationPadding;
				int totalHeight = height + 2 * LocationPadding;

				// Generate city location
				var newArea = Rectangle.Empty;
				int left, top;
				int tries = 100;
				while (tries > 0)
				{
					regenerate:
					tries--;

					var rnd = _random.Next(0, _result.Width - totalWidth);
					left = rnd;

					rnd = _random.Next(0, _result.Height - totalHeight);
					top = rnd;

					// Check height
					for (int y = top; y < top + height; y++)
					{
						for (int x = left; x < left + width; x++)
						{
							if (!_result.IsLand(x, y))
							{
								goto regenerate;
							}
						}
					}

					// And doesn't intersects with already generated cities
					newArea.X = left;
					newArea.Y = top;
					newArea.Width = totalWidth;
					newArea.Height = totalHeight;

					foreach (var r in areas)
					{
						if (r.Intersects(newArea))
						{
							goto regenerate;
						}
					}

					break;
				}

				if (tries == 0) return;

				// Save area
				areas.Add(newArea);

				// Remove padding from area
				newArea.X += LocationPadding;
				newArea.Y += LocationPadding;
				newArea.Width -= 2 * LocationPadding;
				newArea.Height -= 2 * LocationPadding;

				var location = new LocationInfo(locationConfig);

				// Fill with land
				Point p;
				for (var y = 0; y < newArea.Height; ++y)
				{
					for (var x = 0; x < newArea.Width; ++x)
					{
						p = new Point(x + newArea.Left,
							y + newArea.Top);

						_result.SetWorldMapTileType(p, WorldMapTileType.Road);
					}
				}

				// Draw walls
				for (var x = 0; x < newArea.Width; ++x)
				{
					p = new Point(x + newArea.Left, newArea.Top);
					_result.SetWorldMapTileType(p, WorldMapTileType.Wall);

					p = new Point(x + newArea.Left, newArea.Bottom - 1);
					_result.SetWorldMapTileType(p, WorldMapTileType.Wall);
				}

				for (var y = 0; y < newArea.Height; ++y)
				{
					p = new Point(newArea.Left, newArea.Top + y);
					_result.SetWorldMapTileType(p, WorldMapTileType.Wall);

					p = new Point(newArea.Right - 1, newArea.Top + y);
					_result.SetWorldMapTileType(p, WorldMapTileType.Wall);
				}

				// Add entrances
				p = new Point(newArea.X + newArea.Width / 2,
					newArea.Top);
				_result.SetWorldMapTileType(p, WorldMapTileType.Road);
				location.EntranceLocations.Add(p);

				p = new Point(newArea.X,
					newArea.Y + newArea.Height / 2);
				_result.SetWorldMapTileType(p, WorldMapTileType.Road);
				location.EntranceLocations.Add(p);

				p = new Point(newArea.X + newArea.Width / 2,
					newArea.Bottom - 1);
				_result.SetWorldMapTileType(p, WorldMapTileType.Road);
				location.EntranceLocations.Add(p);

				p = new Point(newArea.Right - 1,
					newArea.Top + newArea.Height / 2);
				_result.SetWorldMapTileType(p, WorldMapTileType.Road);
				location.EntranceLocations.Add(p);

				_result.Locations.Add(location);
			}

			_roadTiles.Clear();
			for (var i = 0; i < _result.Locations.Count - 1; ++i)
			{
				Connect(i, i + 1);
			}
		}
	}
}