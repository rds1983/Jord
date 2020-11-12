using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TroublesOfJord.Core;
using TroublesOfJord.Core.Abilities;

namespace TroublesOfJord.Compiling.Loaders
{
	internal class AbilityLoader : Loader<AbilityInfo>
	{
		public AbilityLoader() : base("Abilities")
		{
		}

		public override AbilityInfo LoadItem(Module module, string id, ObjectData data)
		{
			var result = new AbilityInfo
			{
				Name = EnsureString(data, "Name"),
				Mana = OptionalInt(data, "Mana", 0),
				Type = EnsureEnum<AbilityType>(data, "Type"),
				Description = EnsureString(data, "Description")
			};

			var requirementsObject = EnsureJObject(data, "Requirements");
			var requirements = new List<AbilityRequirement>();
			foreach (var pair in requirementsObject)
			{
				var cls = module.EnsureClass(pair.Key);
				var level = StringToInt(pair.Value.ToString(), data.Source);

				var requirement = new AbilityRequirement
				{
					Class = cls,
					Level = level
				};

				requirements.Add(requirement);
			}

			result.Requirements = requirements.ToArray();

			var effectsObject = OptionalJObject(data.Data, "Effects");
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
					
					var obj = (JObject)pair.Value;
					switch (pair.Key)
					{
						case "HealSelf":
							var healSelf = new HealSelf
							{
								Minimum = EnsureInt(obj, data.Source, "Minimum"),
								Maximum = EnsureInt(obj, data.Source, "Maximum"),
								MessageActivated = EnsureString(obj, data.Source, "Message"),
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

			var bonusesObject = OptionalJObject(data.Data, "Bonuses");
			if (bonusesObject != null)
			{
				if (result.Type != AbilityType.Permanent)
				{
					RaiseError("Non-permanent ability can't have bonuses");
				}

				foreach(var pair in bonusesObject)
				{
					var bonusType = (BonusType)Enum.Parse(typeof(BonusType), pair.Key);
					var value = StringToInt(pair.Value.ToString(), data.Source);

					result.Bonuses[bonusType] = value;
				}
			}

			return result;
		}
	}
}
