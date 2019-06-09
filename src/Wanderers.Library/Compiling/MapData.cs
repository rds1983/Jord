using System.Collections.Generic;
using Wanderers.Core;

namespace Wanderers.Compiling
{
	public class MapData
	{
		private readonly Dictionary<string, TileInfo> _tileInfos = new Dictionary<string, TileInfo>();
		private readonly Dictionary<string, CreatureInfo> _creatureInfos = new Dictionary<string, CreatureInfo>();

		public Dictionary<string, TileInfo> TileInfos
		{
			get
			{
				return _tileInfos;
			}
		}

		public Dictionary<string, CreatureInfo> CreatureInfos
		{
			get
			{
				return _creatureInfos;
			}
		}
	}
}
