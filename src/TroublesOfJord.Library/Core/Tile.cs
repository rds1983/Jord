﻿using Microsoft.Xna.Framework;
using System;

namespace TroublesOfJord.Core
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

		public Point Position;
		public Creature Creature
		{
			get;
			internal set;
		}

		public bool Highlighted;

		public Exit Exit;

		public bool Visible;

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
