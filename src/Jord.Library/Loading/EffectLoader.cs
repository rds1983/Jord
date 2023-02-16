using Jord.Core;
using Jord.Core.Abilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Jord.Loading
{
	class EffectLoader : BaseObjectLoader<Effect>
	{
		public static readonly EffectLoader Instance = new EffectLoader();

		protected override Effect CreateObject(string source, JObject data, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			var result = new Effect
			{
				Name = data.EnsureString("Name"),
				Type = data.EnsureEnum<AbilityType>("Type"),
				Description = data.EnsureString("Description")
			};

			var bonusesObject = data.OptionalJObject("Bonuses");
			if (bonusesObject != null)
			{
				foreach (var bonusObject in bonusesObject)
				{
					var bonusType = bonusObject.Key.ToEnum<BonusType>();
					var bonusValue = bonusObject.Value.ToString().ToInt();

					result.Bonuses[bonusType] = bonusValue;
				}
			}

			secondRunAction = null;

			return result;
		}
	}
}
