using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Jord.Core;
using Jord.Core.Abilities;

namespace Jord.Loading
{
	internal class AbilityLoader : BaseObjectLoader<AbilityInfo>
	{
		public static readonly AbilityLoader Instance = new AbilityLoader();

		protected override AbilityInfo CreateObject(string source, JObject data, out Action<Database> secondRunAction)
		{
			var result = new AbilityInfo
			{
				Name = data.EnsureString("Name"),
				Mana = data.OptionalInt("Mana", 0),
				Type = data.EnsureEnum<AbilityType>("Type"),
				Description = data.EnsureString("Description")
			};

			var effectsObject = data.OptionalJObject("Effects");
			var effects = new List<AbilityEffect>();
			if (effectsObject != null)
			{
				if (result.Type == AbilityType.Permanent)
				{
					RaiseError("Permanent ability can't have effects.");
				}

				foreach (var pair in effectsObject)
				{
					AbilityEffect effect = null;

					var effectObj = (JObject)pair.Value;
					switch (pair.Key)
					{
						case "HealSelf":
							var healSelf = new HealSelf
							{
								Minimum = effectObj.EnsureInt("Minimum"),
								Maximum = effectObj.EnsureInt("Maximum"),
								MessageActivated = effectObj.EnsureString("Message"),
							};

							effect = healSelf;
							break;
						default:
							RaiseError($"Unknown ability effect {pair.Key}");
							break;
					}

					effects.Add(effect);
				}
			}

			result.Effects = effects.ToArray();

			var bonusesObject = data.OptionalJObject("Bonuses");
			if (bonusesObject != null)
			{
				if (result.Type != AbilityType.Permanent)
				{
					RaiseError("Non-permanent ability can't have bonuses");
				}

				foreach (var pair in bonusesObject)
				{
					var bonusType = (BonusType)Enum.Parse(typeof(BonusType), pair.Key);
					var value = pair.Value.ToString().ToInt();

					result.Bonuses[bonusType] = value;
				}
			}

			secondRunAction = db => SecondRun(result, data, db);

			return result;
		}

		private void SecondRun(AbilityInfo result, JObject data, Database database)
		{
			var requirementsObject = data.EnsureJObject("Requirements");
			var requirements = new List<AbilityRequirement>();
			foreach (var pair in requirementsObject)
			{
				var cls = database.Classes.Ensure(pair.Key);
				var level = pair.Value.ToString().ToInt();

				var requirement = new AbilityRequirement
				{
					Class = cls,
					Level = level
				};

				requirements.Add(requirement);
			}

			result.Requirements = requirements.ToArray();
		}
	}
}