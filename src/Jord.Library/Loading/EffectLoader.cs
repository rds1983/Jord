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
				Description = data.EnsureString("Description")
			};

			var conditionsObject = data.OptionalJArray("Conditions");
			if (conditionsObject != null)
			{
				foreach(JArray conditionObject in conditionsObject)
				{
					if (conditionObject.Count < 3)
					{
						LoaderExtensions.RaiseError($"Conditions record has less than 3 values.");
					}

					var variable = conditionObject[0].ToString().ToEnum<EffectConditionVariable>();
					var op = conditionObject[1].ToString().ToEnum<EffectConditionOperator>();
					var value = bool.Parse(conditionObject[2].ToString());

					var condition = new EffectCondition(variable, op, value);
					result.Conditions.Add(condition);
				}
			}

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
