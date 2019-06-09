using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Wanderers.Generator
{
	public class HeightMapGenerator
	{
		private const int MinimumIslandSize = 1000;
		private const int MinimumLakeSize = 300;

		private static readonly Point[] _deltas = new Point[]{
			new Point(0, -1),
			new Point(-1, 0),
			new Point(1, 0),
			new Point(0, 1),
			new Point(-1, -1),
			new Point(1, -1),
			new Point(-1, 1),
			new Point(1, 1),
		};

		private static readonly Point[] _deltas2 = new Point[]{
			new Point(0, -1),
			new Point(-1, 0),
			new Point(-1, -1),
			new Point(1, -1),
		};

		private static readonly float[,] _smoothMatrix = new float[,]{
			{0.1f, 0.1f, 0.1f},
			{0.1f, 0.2f, 0.1f},
			{0.1f, 0.1f, 0.1f}
		};

		private readonly Random _random = new Random();
		private bool[,] _isSet;
		private float[,] _data;
		private bool _firstDisplace = true;

		private bool[,] _islandMask;

		private GenerationResult _result;
		private float _landMinimum;
		private float _mountainMinimum;

		public int Size
		{
			get
			{
				return Config.Instance.WorldSize;
			}
		}

		private List<Point> Build(int x, int y, Func<Point, bool> addCondition)
		{
			// Clear mask
			List<Point> result = new List<Point>();

			Stack<Point> toProcess = new Stack<Point>();

			toProcess.Push(new Point(x, y));

			while (toProcess.Count > 0)
			{
				Point top = toProcess.Pop();

				if (top.X < 0 ||
						top.X >= Size ||
						top.Y < 0 ||
						top.Y >= Size ||
						_islandMask[top.Y, top.X] ||
						!addCondition(top))
				{
					continue;
				}

				result.Add(top);
				_islandMask[top.Y, top.X] = true;

				// Add adjancement tiles
				toProcess.Push(new Point(top.X - 1, top.Y));
				toProcess.Push(new Point(top.X, top.Y - 1));
				toProcess.Push(new Point(top.X + 1, top.Y));
				toProcess.Push(new Point(top.X, top.Y + 1));
			}

			return result;
		}

		private void ClearMask()
		{
			_islandMask.Fill(false);
		}

		private float Displace(float average, float d)
		{
			if (Config.Instance.SurroundedByWater && _firstDisplace)
			{
				_firstDisplace = false;
				return 1.0f;
			}

			float p = (float)_random.NextDouble() - 0.5f;
			float result = (average + d * p);

			return result;
		}

		private float GetData(int x, int y)
		{
			return _data[y, x];
		}

		private void SetDataIfNotSet(int x, int y, float value)
		{
			if (_isSet[y, x])
			{
				return;
			}

			_data[y, x] = value;

			_isSet[y, x] = true;
		}

		private void MiddlePointDisplacement(int left, int top, int right, int bottom, float d)
		{
			int localWidth = right - left + 1;
			int localHeight = bottom - top + 1;

			if (localWidth <= 2 && localHeight <= 2)
			{
				return;
			}

			// Retrieve corner heights
			float heightTopLeft = GetData(left, top);
			float heightTopRight = GetData(right, top);
			float heightBottomLeft = GetData(left, bottom);
			float heightBottomRight = GetData(right, bottom);
			float average = (heightTopLeft + heightTopRight + heightBottomLeft + heightBottomRight) / 4;

			// Calculate center
			int centerX = left + localWidth / 2;
			int centerY = top + localHeight / 2;

			// Square step
			float centerHeight = Displace(average, d);
			SetDataIfNotSet(centerX, centerY, centerHeight);

			// Diamond step
			SetDataIfNotSet(left, centerY, (heightTopLeft + heightBottomLeft + centerHeight) / 3);
			SetDataIfNotSet(centerX, top, (heightTopLeft + heightTopRight + centerHeight) / 3);
			SetDataIfNotSet(right, centerY, (heightTopRight + heightBottomRight + centerHeight) / 3);
			SetDataIfNotSet(centerX, bottom, (heightBottomLeft + heightBottomRight + centerHeight) / 3);

			// Sub-recursion
			float div = 1.0f + (10.0f - Config.Instance.HeightMapVariability) / 10.0f;

			d /= div;

			MiddlePointDisplacement(left, top, centerX, centerY, d);
			MiddlePointDisplacement(centerX, top, right, centerY, d);
			MiddlePointDisplacement(left, centerY, centerX, bottom, d);
			MiddlePointDisplacement(centerX, centerY, right, bottom, d);
		}

		public void GenerateHeightMap()
		{
			// Set initial values
			if (!Config.Instance.SurroundedByWater)
			{
				SetDataIfNotSet(0, 0, (float)_random.NextDouble());
				SetDataIfNotSet(Size - 1, 0, (float)_random.NextDouble());
				SetDataIfNotSet(0, Size - 1, (float)_random.NextDouble());
				SetDataIfNotSet(Size - 1, Size - 1, (float)_random.NextDouble());
			}
			else
			{
				SetDataIfNotSet(0, 0, 0.0f);
				SetDataIfNotSet(Size - 1, 0, 0.0f);
				SetDataIfNotSet(0, Size - 1, 0.0f);
				SetDataIfNotSet(Size - 1, Size - 1, 0.0f);
			}

			// Plasma
			MiddlePointDisplacement(0, 0, Size - 1, Size - 1, 1.0f);

			// Determine min & max
			float? min = null, max = null;
			for (int y = 0; y < Size; ++y)
			{
				for (int x = 0; x < Size; ++x)
				{
					float v = GetData(x, y);

					if (min == null || v < min)
					{
						min = v;
					}

					if (max == null || v > max)
					{
						max = v;
					}
				}
			}

			// Normalize
			float delta = max.Value - min.Value;
			for (int y = 0; y < Size; ++y)
			{
				for (int x = 0; x < Size; ++x)
				{
					float v = GetData(x, y);

					v -= min.Value;

					if (delta > 1.0f)
					{
						v /= delta;
					}

					_data[y, x] = v;
				}
			}
		}

		private bool IsLand(Point p)
		{
			if (p.X < 0 || p.X >= Size || p.Y < 0 || p.Y >= Size)
			{
				return true;
			}

			return _data[p.Y, p.X] >= _landMinimum;
		}

		private bool IsMountain(Point p)
		{
			if (p.X < 0 || p.X >= Size || p.Y < 0 || p.Y >= Size)
			{
				return true;
			}

			return _data[p.Y, p.X] >= _mountainMinimum;
		}

		private void Smooth()
		{
			if (!Config.Instance.Smooth)
			{
				return;
			}

			var oldHeightMap = new float[Size, Size];
			for (int y = 0; y < Size; ++y)
			{
				for (int x = 0; x < Size; ++x)
				{
					oldHeightMap[y, x] = _data[y, x];
				}
			}

			for (int y = 0; y < Size; ++y)
			{
				for (int x = 0; x < Size; ++x)
				{
					float newValue = 0;

					for (int k = 0; k < _deltas.Length; ++k)
					{
						int dx = x + _deltas[k].X;
						int dy = y + _deltas[k].Y;

						if (dx < 0 || dx >= Size ||
							dy < 0 || dy >= Size)
						{
							continue;
						}

						float value = _smoothMatrix[_deltas[k].Y + 1, _deltas[k].X + 1] * oldHeightMap[dy, dx];
						newValue += value;
					}

					newValue += _smoothMatrix[1, 1] * oldHeightMap[y, x];
					_data[y, x] = newValue;
				}
			}
		}

		private float CalculateMinimum(float part)
		{
			float result = 0.99f;

			while (result >= 0.0f)
			{
				int c = 0;
				for (int y = 0; y < Size; ++y)
				{
					for (int x = 0; x < Size; ++x)
					{
						float n = GetData(x, y);

						if (n >= result)
						{
							++c;
						}
					}
				}

				float prop = (float)c / (Size * Size);
				if (prop >= part)
				{
					break;
				}

				result -= 0.01f;
			}

			return result;
		}

		private void CalculateMinimums()
		{
			_landMinimum = CalculateMinimum(Config.Instance.LandPart);
			_mountainMinimum = CalculateMinimum(Config.Instance.MountainPart);

			TJ.LogInfo("Land minimum: {0:0.##}", _landMinimum);
			TJ.LogInfo("Mountain minimum: {0:0.##}", _mountainMinimum);
		}

		private void RemoveTiles(string name, WorldMapTileType tileType, WorldMapTileType newValue)
		{
			TJ.LogInfo("Removing {0}...", name);

			var tilesReplaced = 0;

			ClearMask();

			// Next run remove small islands
			for (int y = 0; y < Size; ++y)
			{
				for (int x = 0; x < Size; ++x)
				{
					if (!_islandMask[y, x] && _result.GetWorldMapTileType(x, y) == tileType)
					{
						List<Point> island = Build(x, y, p => _result.GetWorldMapTileType(p) == tileType);

						if (island.Count < MinimumIslandSize)
						{
							// Remove small island
							foreach (var p in island)
							{
								_result.SetWorldMapTileType(p, newValue);
								++tilesReplaced;
							}
						}
					}
				}
			}

			TJ.LogInfo("Tiles replaced: {0}", tilesReplaced);
		}

		private void RemoveNoise(string name, WorldMapTileType tileType)
		{
			TJ.LogInfo("Removing {0}...", name);
			var iterations = 0;
			var tilesReplaced = 0;
			while (true)
			{
				++iterations;
				var changed = false;
				for (int y = 0; y < Size; ++y)
				{
					for (int x = 0; x < Size; ++x)
					{
						if (_result.GetWorldMapTileType(x, y) == tileType)
						{
							continue;
						}

						for (var i = 0; i < _deltas2.Length; ++i)
						{
							var d = _deltas2[i];
							var p = new Point(x + d.X, y + d.Y);
							var p2 = new Point(x - d.X, y - d.Y);

							if (_result.GetWorldMapTileType(p, tileType) == tileType &&
								_result.GetWorldMapTileType(p2, tileType) == tileType)
							{
								// Turn into new value
								_result.SetWorldMapTileType(x, y, tileType);
								changed = true;
								++tilesReplaced;
								break;
							}
						}
					}
				}

				if (!changed)
				{
					break;
				}
			}

			TJ.LogInfo("Removal iterations: {0}", iterations);
			TJ.LogInfo("Tiles replaced: {0}", tilesReplaced);
		}

		private void GenerateForests()
		{
			TJ.LogInfo("Generating forests...");

			var c = 0;

			var tilesCount = Size * Size;
			while (((float)c / tilesCount) < Config.Instance.ForestPart)
			{
				var locations = new Queue<Point>();
				for (var i = 0; i < 10; ++i)
				{
					// Find starting spot
					var tries = 100;
					var p = Point.Zero;
					while (tries > 0)
					{
						--tries;
						p.X = _random.Next(0, _result.Width);
						p.Y = _random.Next(0, _result.Height);

						if (_result.IsLand(p))
						{
							locations.Enqueue(p);
							break;
						}
					}
				}

				// Grow forest from this spot
				while (locations.Count > 0 && ((float)c / tilesCount) < Config.Instance.ForestPart)
				{
					var p = locations.Dequeue();

					if (!_result.IsLand(p) || 
						_result.IsNear(p, WorldMapTileType.Water) || 
						_result.IsNear(p, WorldMapTileType.Mountain))
					{
						continue;
					}

					_result.SetWorldMapTileType(p, WorldMapTileType.Forest);
					++c;

					for (var i = 0; i < 30; ++i)
					{
						var dist = _random.Next(0, 25);
						var angle = _random.Next(0, 360);

						var radAngle = Math.PI * angle / 180;

						var d = new Point(p.X + (int)(Math.Cos(radAngle) * dist),
							p.Y + (int)(Math.Sin(radAngle) * dist));

						if (_result.IsLand(d))
						{
							locations.Enqueue(d);
						}
					}
				}
			}
		}

		public GenerationResult Generate()
		{
			_islandMask = new bool[Size, Size];

			_data = new float[Size, Size];
			_isSet = new bool[Size, Size];
			_data.Fill(0.0f);
			_isSet.Fill(false);

			_firstDisplace = true;

			TJ.LogInfo("Generating height map...");
			GenerateHeightMap();

			TJ.LogInfo("Postprocessing height map...");
			Smooth();
			CalculateMinimums();

			var tiles = new WorldMapTileType[Size, Size];

			for (int y = 0; y < Size; ++y)
			{
				for (int x = 0; x < Size; ++x)
				{
					var p = new Point(x, y);

					var tileType = WorldMapTileType.Water;
					if (IsLand(p))
					{
						var h = GetData(x, y);

						if (!IsMountain(p))
						{
							tileType = WorldMapTileType.Land;
						}
						else
						{
							tileType = WorldMapTileType.Mountain;
						}
					}

					tiles[y, x] = tileType;
				}
			}

			_result = new GenerationResult(tiles);

			if (Config.Instance.RemoveSmallIslands)
			{
				RemoveTiles("small islands", WorldMapTileType.Land, WorldMapTileType.Water);
			}

			if (Config.Instance.RemoveSmallLakes)
			{
				RemoveTiles("small lakes", WorldMapTileType.Water, WorldMapTileType.Land);
				RemoveNoise("water noise", WorldMapTileType.Land);
			}

			RemoveTiles("small mountains", WorldMapTileType.Mountain, WorldMapTileType.Land);
			RemoveNoise("mountain noise", WorldMapTileType.Mountain);

			GenerateForests();

			RemoveTiles("small forests", WorldMapTileType.Forest, WorldMapTileType.Land);
			RemoveNoise("forest noise", WorldMapTileType.Forest);

			// Calculate amount of different tiles
			int w = 0, l = 0, m = 0, f = 0;
			for (int y = 0; y < Size; ++y)
			{
				for (int x = 0; x < Size; ++x)
				{
					var tileType = _result.GetWorldMapTileType(x, y);
					switch (tileType)
					{
						case WorldMapTileType.Water:
							++w;
							break;
						case WorldMapTileType.Land:
							++l;
							break;
						case WorldMapTileType.Forest:
							++f;
							break;
						case WorldMapTileType.Mountain:
							++m;
							break;
					}
				}
			}

			int tilesCount = Size * Size;

			TJ.LogInfo("{0}% water, {1}% land, {2}% mountains, {3}% forests",
					w * 100 / tilesCount,
					l * 100 / tilesCount,
					m * 100 / tilesCount,
					f * 100 / tilesCount);

			return new GenerationResult(tiles);
		}
	}
}