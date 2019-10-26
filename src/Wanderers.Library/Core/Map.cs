using System.ComponentModel;
using Wanderers.Compiling;
using Microsoft.Xna.Framework;

namespace Wanderers.Core
{
	public class Map : BaseObject
	{
		private Tile[,] _tiles;

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

				for(var x = 0; x < value.X; ++x)
				{
					for(var y = 0; y < value.Y; ++y)
					{
						this[x, y] = new Tile
						{
							Position = new Point(x, y)
						};
					}
				}
			}
		}

		[IgnoreField]
		public Point? SpawnSpot
		{
			get; set;
		}

		public bool Local { get; set; }

		[IgnoreField]
		public Tile this[int x, int y]
		{
			get
			{
				return _tiles[x, y];
			}

			private set
			{
				_tiles[x, y] = value;
			}
		}

		[IgnoreField]
		public Tile this[Point p]
		{
			get
			{
				return this[p.X, p.Y];
			}
		}

		public Map()
		{
			Local = true;
		}
	}
}
