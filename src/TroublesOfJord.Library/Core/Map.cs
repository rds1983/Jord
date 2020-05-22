using Microsoft.Xna.Framework;
using RogueSharp;
using System;

namespace TroublesOfJord.Core
{
	public class Map : Map<Tile>, IBaseObject
	{
		public string Id { get; set; }

		public string Source { get; set; }

		public Point? SpawnSpot
		{
			get; set;
		}

		public bool Explored { get; set; }

		public bool Local { get; set; }

		public Tile this[int x, int y]
		{
			get
			{
				return GetCell(x, y);
			}
		}

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

		/// <summary>
		/// Constructor creates a new Map and immediately initializes it
		/// </summary>
		/// <param name="width">How many Cells wide the Map will be</param>
		/// <param name="height">How many Cells tall the Map will be</param>
		public Map(int width, int height) : base(width, height)
		{
			Local = true;
		}

		public Map(Point size): this(size.X, size.Y)
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
	}
}
