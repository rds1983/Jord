using System;
using System.Collections.Generic;
using TroublesOfJord.Core.Items;
using TroublesOfJord.Generation;

namespace TroublesOfJord.Core
{
	public class Module
	{
		public Dictionary<string, TileInfo> TileInfos { get; } = new Dictionary<string, TileInfo>();
		public Dictionary<string, Class> Classes { get; } = new Dictionary<string, Class>();
		public Dictionary<string, CreatureInfo> CreatureInfos { get; } = new Dictionary<string, CreatureInfo>();
		public Dictionary<string, BaseItemInfo> ItemInfos { get; } = new Dictionary<string, BaseItemInfo>();
		public Dictionary<string, BaseGenerator> Generators { get; } = new Dictionary<string, BaseGenerator>();
		public Dictionary<string, Map> Maps { get; } = new Dictionary<string, Map>();
		public Dictionary<string, MapTemplate> MapTemplates { get; } = new Dictionary<string, MapTemplate>();

		private static T Ensure<T>(Dictionary<string, T> data, string id)
		{
			T result;
			if (!data.TryGetValue(id, out result))
			{
				throw new Exception(string.Format("Could not find {0} '{1}'.", typeof(T).Name, id));
			}

			return result;
		}

		public TileInfo EnsureTileInfo(string id)
		{
			return Ensure(TileInfos, id);
		}

		public Class EnsureClass(string id)
		{
			return Ensure(Classes, id);
		}

		public CreatureInfo EnsureCreatureInfo(string id)
		{
			return Ensure(CreatureInfos, id);
		}

		public BaseItemInfo EnsureItemInfo(string id)
		{
			return Ensure(ItemInfos, id);
		}

		public BaseGenerator EnsureGenerator(string id)
		{
			return Ensure(Generators, id);
		}

		public Map EnsureMap(string id)
		{
			return Ensure(Maps, id);
		}

		public MapTemplate EnsureMapTemplate(string id)
		{
			return Ensure(MapTemplates, id);
		}
	}
}