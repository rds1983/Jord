using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TroublesOfJord.Compiling;
using TroublesOfJord.Compiling.Loaders;
using TroublesOfJord.Core;
using TroublesOfJord.Core.Items;

namespace TroublesOfJord.Storage
{
	public class CharacterData
	{
		public string Name { get; set; }
		public string ClassId { get; set; }
		public int Gold { get; set; }
		public string StartingMapId { get; set; } = "BalHarbor";

		[OptionalField]
		public Dictionary<string, int> Inventory { get; } = new Dictionary<string, int>();

		[OptionalField]
		public Dictionary<EquipType, string> Equipment { get; } = new Dictionary<EquipType, string>();

		public CharacterData()
		{
		}

		public CharacterData(Character character): this()
		{
			Name = character.Player.Name;
			ClassId = character.Class.Id;
			Gold = character.Player.Gold;

			foreach (var item in character.Player.Inventory.Items)
			{
				Inventory[item.Item.Info.Id] = item.Quantity;
			}

			foreach (var item in character.Player.Equipment.Items)
			{
				if (item == null || item.Item == null)
				{
					continue;
				}

				Equipment[item.Slot] = item.Item.Info.Id;
			}
		}

		public Character CreateCharacter()
		{
			var result = new Character
			{
				Class = TJ.Module.Classes[ClassId]
			};

			result.Player = new Player
			{
				Name = Name,
				Gold = Gold
			};

			foreach (var pair in Inventory)
			{
				result.Player.Inventory.Add(new Item(TJ.Module.ItemInfos[pair.Key]), pair.Value);
			}

			foreach(var pair in Equipment)
			{
				result.Player.Equipment.Equip(new Item(TJ.Module.ItemInfos[pair.Value]));
			}

			return result;
		}

		public string ToJson()
		{
			var obj = BaseLoader.SaveObject(this);

			return obj.ToString();
		}

		public static CharacterData FromJson(string data)
		{
			var obj = JObject.Parse(data);
			return (CharacterData)BaseLoader.LoadData(new Module(),
				typeof(CharacterData),
				string.Empty,
				obj,
				string.Empty);
		}
	}
}