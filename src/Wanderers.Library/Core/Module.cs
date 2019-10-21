using System;
using System.Collections.Generic;
using Wanderers.Core.Items;

namespace Wanderers.Core
{
	public class Module
	{
		private readonly Dictionary<string, TileInfo> _tileInfos = new Dictionary<string, TileInfo>();
		private readonly Dictionary<string, Class> _classes = new Dictionary<string, Class>();
		private readonly Dictionary<string, CreatureInfo> _creatureInfos = new Dictionary<string, CreatureInfo>();
		private readonly Dictionary<string, BaseItemInfo> _itemInfos = new Dictionary<string, BaseItemInfo>();
		private readonly Dictionary<string, Map> _maps = new Dictionary<string, Map>();

		public Dictionary<string, TileInfo> TileInfos
		{
			get
			{
				return _tileInfos;
			}
		}

		public Dictionary<string, Class> Classes
		{
			get
			{
				return _classes;
			}
		}

		public Dictionary<string, CreatureInfo> CreatureInfos
		{
			get
			{
				return _creatureInfos;
			}
		}

		public Dictionary<string, BaseItemInfo> ItemInfos
		{
			get
			{
				return _itemInfos;
			}
		}

		public Dictionary<string, Map> Maps
		{
			get
			{
				return _maps;
			}
		}

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
			return Ensure(_tileInfos, id);
		}

		public Class EnsureClass(string id)
		{
			return Ensure(_classes, id);
		}

		public CreatureInfo EnsureCreatureInfo(string id)
		{
			return Ensure(_creatureInfos, id);
		}

		public BaseItemInfo EnsureItemInfo(string id)
		{
			return Ensure(_itemInfos, id);
		}

		public Map EnsureMap(string id)
		{
			return Ensure(_maps, id);
		}
	}
}