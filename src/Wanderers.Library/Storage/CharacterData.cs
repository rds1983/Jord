using System.Collections.Generic;
using Wanderers.Core;

namespace Wanderers.Storage
{
	public class CharacterData
	{
		public string Name
		{
			get; set;
		}

		public string ClassId
		{
			get; set;
		}

		public int Gold
		{
			get; set;
		}

		public Dictionary<string, int> Inventory
		{
			get; set;
		}

		public CharacterData()
		{
			Inventory = new Dictionary<string, int>();
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
				result.Player.Inventory.Add(new Core.Items.Item(TJ.Module.ItemInfos[pair.Key]), pair.Value);
			}

			return result;
		}
	}
}