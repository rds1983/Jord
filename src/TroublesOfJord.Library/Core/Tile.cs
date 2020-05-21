using Microsoft.Xna.Framework;
using RogueSharp;
using System;

namespace TroublesOfJord.Core
{
	public class Tile: ICell
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
		ICellInfo ICell.Info { get => Info; set => Info = (TileInfo)value; }

		public bool Highlighted;

		public Exit Exit;

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

		/// <summary>
		/// Determines whether two Cell instances are equal
		/// </summary>
		/// <param name="other">The Cell to compare this instance to</param>
		/// <returns>True if the instances are equal; False otherwise</returns>
		public bool Equals(ICell other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return X == other.X && Y == other.Y && Info == other.Info && IsInFov == other.IsInFov && IsExplored == other.IsExplored;
		}

		/// <summary>
		/// Provides a simple visual representation of the Cell using the following symbols:
		/// - `%`: `Cell` is not in field-of-view
		/// - `.`: `Cell` is transparent, walkable, and in field-of-view
		/// - `s`: `Cell` is walkable and in field-of-view (but not transparent)
		/// - `o`: `Cell` is transparent and in field-of-view (but not walkable)
		/// - `#`: `Cell` is in field-of-view (but not transparent or walkable)
		/// </summary>
		/// <returns>A string representation of the Cell using special symbols to denote Cell properties</returns>
		public override string ToString()
		{
			if (Info.IsWalkable)
			{
				if (Info.IsTransparent)
				{
					return ".";
				}
				else
				{
					return "s";
				}
			}
			else
			{
				if (Info.IsTransparent)
				{
					return "o";
				}
				else
				{
					return "#";
				}
			}
		}
	}
}
