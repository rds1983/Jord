using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Jord.Core
{
	public class SpawnableWithCoords
	{
		public ISpawnable Spawnable { get; }
		public Point Position { get; }

		public SpawnableWithCoords(ISpawnable spawnable, Point position)
		{
			Spawnable = spawnable ?? throw new ArgumentNullException(nameof(spawnable));
			Position = position;
		}
	}

	public class MapInfo : BaseObject
	{
		public Map Map { get; }

		public List<SpawnableWithCoords> Spawns = new List<SpawnableWithCoords>();

		public MapInfo(Map map)
		{
			if (map == null)
			{
				throw new ArgumentNullException(nameof(map));
			}

			Map = map;
		}

		public void SpawnAll()
		{
			foreach (var spawn in Spawns)
			{
				spawn.Spawnable.Spawn(spawn.Position);
			}
		}
	}
}
