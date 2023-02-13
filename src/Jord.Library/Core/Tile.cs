using Microsoft.Xna.Framework;
using System;

namespace Jord.Core
{
	public class Tile
	{
		private TileInfo _info;

		public TileInfo Info
		{
			get
			{
				return _info;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_info = value;
			}
		}

		public TileObject Object { get; set; }


		public Creature Creature
		{
			get;
			internal set;
		}

		public int X { get; set; }
		public int Y { get; set; }

		public Point Position
		{
			get
			{
				return new Point(X, Y);
			}

			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		public bool IsInFov { get; set; }
		public bool IsExplored { get; set; }

		public bool Highlighted { get; set; }

		public Exit Exit { get; set; }

		public string Sign { get; set; }

		public readonly Inventory Inventory = new Inventory();

		public Appearance Appearance { get; set; }

		public Tile()
		{
		}

		public Tile(TileInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			Info = info;
		}
	}
}
