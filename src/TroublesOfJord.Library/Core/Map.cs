using GoRogue;
using GoRogue.MapViews;
using GoRogue.Pathing;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace TroublesOfJord.Core
{
	public class Map : IBaseObject
	{
		private class MapFOVView : IMapView<bool>
		{
			private readonly Map _map;

			public bool this[Coord pos] => _map[pos].Info.IsTransparent;

			public bool this[int index1D] => _map[index1D].Info.IsTransparent;

			public bool this[int x, int y] => _map[x, y].Info.IsTransparent;

			public int Height => _map.Height;

			public int Width => _map.Width;

			public MapFOVView(Map map)
			{
				_map = map;
			}
		}

		private readonly ArrayMap2D<Tile> _tiles;

		public FOV FieldOfView { get; }

		public AStar PathFinder { get; }

		public string Id { get; set; }

		public string Source { get; set; }

		public Point? SpawnSpot
		{
			get; set;
		}

		public bool Explored { get; set; }

		public bool Local { get; set; }

		public List<Creature> Creatures { get; } = new List<Creature>();

		public Tile this[int x, int y]
		{
			get => _tiles[x, y];
			set => _tiles[x, y] = value;
		}

		public Tile this[Coord pos]
		{
			get => _tiles[pos.X, pos.Y];
			set => _tiles[pos.X, pos.Y] = value;
		}

		public Tile this[Point pos]
		{
			get => _tiles[pos.X, pos.Y];
			set => _tiles[pos.X, pos.Y] = value;
		}

		public Tile this[int index1D]
		{
			get => _tiles[index1D];
			set => _tiles[index1D] = value;
		}

		public int Width => _tiles.Width;

		public int Height => _tiles.Height;

		/// <summary>
		/// Constructor creates a new Map and immediately initializes it
		/// </summary>
		/// <param name="width">How many Cells wide the Map will be</param>
		/// <param name="height">How many Cells tall the Map will be</param>
		public Map(int width, int height)
		{
			_tiles = new ArrayMap2D<Tile>(width, height);
			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					this[x, y] = new Tile
					{
						X = x,
						Y = y
					};
				}
			}

			var mapView = new MapFOVView(this);
			FieldOfView = new FOV(mapView);
			PathFinder = new AStar(mapView, Distance.EUCLIDEAN);
			Local = true;
		}

		public Map(Point size) : this(size.X, size.Y)
		{
		}

		public Tile EnsureExitTileById(string id)
		{
			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					var tile = this[x, y];
					if (tile.Exit != null && tile.Exit.MapId == id)
					{
						return tile;
					}
				}
			}

			throw new Exception(string.Format("Could not find exit with id '{0}'.", id));
		}

		public IEnumerable<Tile> GetAllCells()
		{
			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					yield return this[x, y];
				}
			}
		}
	}
}
