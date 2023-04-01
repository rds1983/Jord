using Jord.Core.Items;
using System.Collections.Generic;

namespace Jord.Components
{
	public class PlayerInfo
	{
		public string Name { get; set; }
		public string ClassId { get; set; }
		public int Level { get; set; }

		public List<string> Perks { get; } = new List<string>();

		public int Experience { get; set; }
		public int Gold { get; set; }

		public Dictionary<EquipType, string> Equipment { get; } = new Dictionary<EquipType, string>();
	}
}
