using Microsoft.Xna.Framework;
using RogueSharp;
using System;

namespace TroublesOfJord.Core
{
	public class Tile: ICell
	{
		private TileInfo _info;
		private bool _isTransparent, _isWalkable;

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
				_isTransparent = _info.Passable;
				_isWalkable = _info.Passable;
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

		public bool IsTransparent { get => _isTransparent; set => _isTransparent = value; }
		public bool IsWalkable { get => _isWalkable; set => _isWalkable = value; }
		public bool IsInFov { get; set; }
		public bool IsExplored { get; set; }

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
			return X == other.X && Y == other.Y && IsTransparent == other.IsTransparent && IsWalkable == other.IsWalkable && IsInFov == other.IsInFov && IsExplored == other.IsExplored;
		}
	}
}
