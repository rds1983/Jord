using System.ComponentModel;
using Wanderers.Compiling;
using Microsoft.Xna.Framework;

namespace Wanderers.Core
{
	public class Map : ItemWithId
	{
		private Tile[,] _tiles;

		[IgnoreField]
		public Point Size
		{
			get
			{
				var result = Point.Zero;
				if (_tiles != null)
				{
					result.X = _tiles.GetLength(0);
					result.Y = _tiles.GetLength(1);
				}
				return result;
			}

			set
			{
				_tiles = new Tile[value.X, value.Y];
			}
		}

		[IgnoreField]
		public Point? SpawnSpot
		{
			get; set;
		}

		public bool Local { get; set; }

		[Browsable(false)]
		[IgnoreField]
		public Tile[,] Tiles
		{
			get { return _tiles; }
		}

		public Map()
		{
			Local = true;
		}

		public Tile GetTileAt(int x, int y)
		{
			return _tiles[x, y];
		}

		public Tile GetTileAt(Point pos)
		{
			return _tiles[pos.X, pos.Y];
		}

		public Tile GetTileAt(Vector2 pos)
		{
			return _tiles[(int)pos.X, (int)pos.Y];
		}

		public static Tile GetTileAt(Tile[,] tiles, int x, int y)
		{
			return tiles[x, y];
		}

		public void SetTileAt(Point pos, Tile tile)
		{
			_tiles[pos.X, pos.Y] = tile;
		}

		public void SetTileAt(int x, int y, Tile tile)
		{
			_tiles[x, y] = tile;
		}
	}
}
