using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TroublesOfJord.Core;
using TroublesOfJord.Core.Items;

namespace TroublesOfJord.Storage
{
	public class PlayerData
	{
		public string Name { get; set; }
		public string ClassId { get; set; }
		public int Gold { get; set; }
		public string StartingMapId { get; set; } = "BalHarbor";

		public Dictionary<string, int> Inventory { get; } = new Dictionary<string, int>();

		public Dictionary<EquipType, string> Equipment { get; } = new Dictionary<EquipType, string>();
		public string[] LearnedAbilities;

		public PlayerData()
		{
		}

		public PlayerData(Player player): this()
		{
			Name = player.Name;
			ClassId = player.Class.Id;
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
				Class = TJ.Module.Classes[ClassId],
				Name = Name,
				Gold = Gold
			};

			foreach (var pair in Inventory)
			{
				result.Inventory.Add(new Item(TJ.Module.ItemInfos[pair.Key]), pair.Value);
			}

			foreach(var pair in Equipment)
			{
				result.Equipment.Equip(new Item(TJ.Module.ItemInfos[pair.Value]));
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