using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Wanderers.Core;

namespace Wanderers.Generator
{
	public class GenerationResult
	{
		private readonly WorldMapTileType[,] _data;
		private readonly List<LocationInfo> _locations = new List<LocationInfo>();

		public WorldMapTileType[,] Data
		{
			get
			{
				return _data;
			}
		}

		public int Width
		{
			get
			{
				return _data.GetLength(1);
			}
		}

		public int Height
		{
			get
			{
				return _data.GetLength(0);
			}
		}

		public List<LocationInfo> Locations
		{
			get
			{
				return _locations;
			}
		}

		public GenerationResult(WorldMapTileType[,] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			_data = data;
		}

		public WorldMapTileType GetWorldMapTileType(int x, int y, WorldMapTileType def = WorldMapTileType.Water)
		{
			if (x < 0 || x >= _data.GetLength(1) ||
				y < 0 || y >= _data.GetLength(0))
			{
				return def;
			}

			return _data[y, x];
		}

		public WorldMapTileType GetWorldMapTileType(Point p, WorldMapTileType def = WorldMapTileType.Water)
		{
			return GetWorldMapTileType(p.X, p.Y, def);
		}

		public void SetWorldMapTileType(int x, int y, WorldMapTileType type)
		{
			_data[y, x] = type;
		}

		public void SetWorldMapTileType(Point p, WorldMapTileType type)
		{
			SetWorldMapTileType(p.X, p.Y, type);
		}

		public bool IsWater(int x, int y)
		{
			return GetWorldMapTileType(x, y) == WorldMapTileType.Water;
		}

		public bool IsWater(Point p)
		{
			return IsWater(p.X, p.Y);
		}

		public bool IsMountain(int x, int y)
		{
			return GetWorldMapTileType(x, y) == WorldMapTileType.Mountain;
		}

		public bool IsForest(int x, int y)
		{
			return GetWorldMapTileType(x, y) == WorldMapTileType.Forest;
		}

		public bool IsRoad(int x, int y)
		{
			return GetWorldMapTileType(x, y) == WorldMapTileType.Road;
		}

		public bool IsLand(int x, int y)
		{
			return GetWorldMapTileType(x, y) == WorldMapTileType.Land;
		}

		public bool IsLand(Point p)
		{
			return IsLand(p.X, p.Y);
		}

		public bool IsNear(Point p, WorldMapTileType tileType)
		{
			foreach (var d in PathFinder.AllDirections)
			{
				if (GetWorldMapTileType(p + d) == tileType)
				{
					return true;
				}
			}

			return false;
		}

		public bool IsRoadPlaceable(Point p)
		{
			var tileType = GetWorldMapTileType(p);
			return (tileType == WorldMapTileType.Land ||
					tileType == WorldMapTileType.Road ||
					tileType == WorldMapTileType.Forest) && 
					!IsNear(p, WorldMapTileType.Mountain);
		}
	}
}