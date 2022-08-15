using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Jord.Core;
using Jord.Core.Abilities;

namespace Jord.Serialization.Loaders
{
	internal class AbilityLoader : Loader<AbilityInfo>
	{
		public AbilityLoader() : base("Abilities")
		{
		}

		public override AbilityInfo LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;
			var result = new AbilityInfo
			{
				Name = dataObj.EnsureString("Name"),
				Mana = dataObj.OptionalInt("Mana", 0),
				Type = dataObj.EnsureEnum<AbilityType>("Type"),
				Description = dataObj.EnsureString("Description")
			};

			var requirementsObject = dataObj.EnsureJObject("Requirements");
			var requirements = new List<AbilityRequirement>();
			foreach (var pair in requirementsObject)
			{
				var cls = module.Classes.Ensure(pair.Key);
				var level = pair.Value.ToString().ToInt();

				var requirement = new AbilityRequirement
				{
					Class = cls,
					Level = level
				};

				requirements.Add(requirement);
			}

			result.Requirements = requirements.ToArray();

			var effectsObject = dataObj.OptionalJObject("Effects");
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
							RaiseError("Unknown ability effect {0}", pair.Key);
							break;
					}
					
					effects.Add(effect);
				}
			}

			result.Effects = effects.ToArray();

			var bonusesObject = dataObj.OptionalJObject("Bonuses");
			if (bonusesObject != null)
			{
				if (result.Type != AbilityType.Permanent)
				{
					RaiseError("Non-permanent ability can't have bonuses");
				}

				foreach(var pair in bonusesObject)
				{
					var bonusType = (BonusType)Enum.Parse(typeof(BonusType), pair.Key);
					var value = pair.Value.ToString().ToInt();

					result.Bonuses[bonusType] = value;
				}
			}

			return result;
		}
	}
}
