﻿using System;
using System.Collections.Generic;
using System.Linq;
using Jord.Core.Abilities;
using Jord.Core.Items;
using Jord.Generation;

namespace Jord.Core
{
	public class Database
	{
		private Tileset _tileset;

		public Dictionary<string, TileInfo> TileInfos { get; } = new Dictionary<string, TileInfo>();
		public Dictionary<string, TileObject> TileObjects { get; } = new Dictionary<string, TileObject>();
		public Dictionary<string, Class> Classes { get; } = new Dictionary<string, Class>();
		public Dictionary<string, CreatureInfo> CreatureInfos { get; } = new Dictionary<string, CreatureInfo>();
		public Dictionary<string, BaseItemInfo> ItemInfos { get; } = new Dictionary<string, BaseItemInfo>();
		public Dictionary<string, BaseGenerator> Generators { get; } = new Dictionary<string, BaseGenerator>();
		public Dictionary<string, MapInfo> Maps { get; } = new Dictionary<string, MapInfo>();
		public Dictionary<string, Dungeon> Dungeons { get; } = new Dictionary<string, Dungeon>();
		public Dictionary<string, AbilityInfo> Abilities { get; } = new Dictionary<string, AbilityInfo>();
		public Dictionary<int, LevelCost> LevelCosts { get; } = new Dictionary<int, LevelCost>();
		public Dictionary<string, Tileset> Tilesets { get; } = new Dictionary<string, Tileset>();
		public Dictionary<string, Effect> Effects { get; } = new Dictionary<string, Effect>();
		public Dictionary<string, Perk> Perks { get; } = new Dictionary<string, Perk>();
		public Settings Settings { get; set; }

		public Tileset Tileset
		{
			get => _tileset;
			set
			{
				_tileset = value;

				foreach (var pair in TileInfos)
				{
					pair.Value.UpdateAppearance(value);
				}
			}
		}

		public int MaximumLevel
		{
			get
			{
				return (from v in LevelCosts.Values select v.Level).Max();
			}
		}
	}

	public static class DatabaseExtensions
	{
		public static T Ensure<T, T2>(this Dictionary<T2, T> data, T2 id)
		{
			T result;
			if (!data.TryGetValue(id, out result))
			{
				throw new Exception(string.Format("Could not find {0} '{1}'.", typeof(T).Name, id));
			}

			return result;
		}
	}
}