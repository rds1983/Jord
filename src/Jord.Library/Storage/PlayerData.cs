using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Jord.Core;
using Jord.Core.Items;
using Jord.Core.Abilities;

namespace Jord.Storage
{
	public class PlayerData
	{
		public string Name { get; set; }
		public string ClassId { get; set; }
		public int Level { get; set; }

		public List<Perk> Perks { get; set; }

		public int Experience { get; set; }
		public int Gold { get; set; }
		public string StartingMapId { get; set; } = "BalHarbor";

		public Dictionary<string, int> Inventory { get; } = new Dictionary<string, int>();

		public Dictionary<EquipType, string> Equipment { get; } = new Dictionary<EquipType, string>();

		public string[] LearnedAbilities;

		public PlayerData()
		{
		}

		public PlayerData(Player player) : this()
		{
			Name = player.Name;
			ClassId = player.Class.Id;
			Level = player.Level;
			Perks = player.Perks;
			Experience = player.Experience;
			Gold = player.Gold;

			foreach (var item in player.Inventory.Items)
			{
				Inventory[item.Item.Info.Id] = item.Quantity;
			}

			foreach (var item in player.Equipment.Items)
			{
				if (item == null || item.Item == null)
				{
					continue;
				}

				Equipment[item.Slot] = item.Item.Info.Id;
			}

			var abilities = player.BuildLearnedAbilities();
			LearnedAbilities = (from a in abilities select a.Name).ToArray();
		}

		public Player CreateCharacter()
		{
			var result = new Player
			{
				Class = TJ.Database.Classes[ClassId],
				Name = Name,
				Level = Level,
				Experience = Experience,
				Gold = Gold
			};

			if (Perks != null)
			{
				result.Perks.AddRange(Perks);
			}

			foreach (var pair in Inventory)
			{
				result.Inventory.Add(new Item(TJ.Database.ItemInfos[pair.Key]), pair.Value);
			}

			foreach (var pair in Equipment)
			{
				result.Equipment.Equip(new Item(TJ.Database.ItemInfos[pair.Value]));
			}

			result.Abilities = result.BuildFreeAbilities();

			return result;
		}

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		public static PlayerData FromJson(string data)
		{
			return JsonConvert.DeserializeObject<PlayerData>(data);
		}
	}
}