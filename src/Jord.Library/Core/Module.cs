using System;
using System.Collections.Generic;
using System.Linq;
using Jord.Core.Abilities;
using Jord.Core.Items;
using Jord.Generation;

namespace Jord.Core
{
	public class Module
	{
		public Dictionary<string, TileInfo> TileInfos { get; } = new Dictionary<string, TileInfo>();
		public Dictionary<string, TileObject> TileObjects { get; } = new Dictionary<string, TileObject>();
		public Dictionary<string, Class> Classes { get; } = new Dictionary<string, Class>();
		public Dictionary<string, CreatureInfo> CreatureInfos { get; } = new Dictionary<string, CreatureInfo>();
		public Dictionary<string, BaseItemInfo> ItemInfos { get; } = new Dictionary<string, BaseItemInfo>();
		public Dictionary<string, BaseGenerator> Generators { get; } = new Dictionary<string, BaseGenerator>();
		public Dictionary<string, Map> Maps { get; } = new Dictionary<string, Map>();
		public Dictionary<string, Dungeon> Dungeons { get; } = new Dictionary<string, Dungeon>();
		public Dictionary<string, AbilityInfo> Abilities { get; } = new Dictionary<string, AbilityInfo>();
		public Dictionary<int, LevelCost> LevelCosts { get; } = new Dictionary<int, LevelCost>();

		public ModuleInfo ModuleInfo;

		public int MaximumLevel
		{
			get
			{
				return (from v in LevelCosts.Values select v.Level).Max();
			}
		}
	}

	public static class ModuleExtensions
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