﻿using Jord.Core.Abilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jord.Core.Items
{
	public enum ItemType
	{
		Custom,
		Equipment,
		Component,
		Trash
	}

	public class BaseItemInfo : BaseMapObject
	{
		private static readonly Dictionary<Type, string> _typeDisplayNames = new Dictionary<Type, string>();

		public string Name { get; set; }

		public int Price { get; set; }

		public Inventory Tanning { get; set; }

		public Inventory Crafting { get; set; }

		public ItemType Type { get; set; }

		public Dictionary<BonusType, int> Bonuses { get; } = new Dictionary<BonusType, int>();

		public string TypeDisplayName
		{
			get
			{
				return GetTypeDisplayName(GetType());
			}
		}

		public virtual string BuildDescription()
		{
			var sb = new StringBuilder();
			sb.Append(TypeDisplayName);

			foreach (var pair in Bonuses)
			{
				sb.Append($", {pair.Key} = {pair.Value}");
			}

			return sb.ToString();
		}

		private static string GetTypeDisplayName(Type type)
		{
			string result;
			if (_typeDisplayNames.TryGetValue(type, out result))
			{
				return result;
			}

			var sb = new StringBuilder();
			var input = type.Name.Replace("Info", string.Empty);

			for (var i = 0; i < input.Length; ++i)
			{
				var c = input[i];

				if (i > 0 && char.IsUpper(c))
				{
					sb.Append(" ");
				}

				sb.Append(char.ToLower(c));
			}

			_typeDisplayNames[type] = sb.ToString();

			return sb.ToString();
		}
	}
}
