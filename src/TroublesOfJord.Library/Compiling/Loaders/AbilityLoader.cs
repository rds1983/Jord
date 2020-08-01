using System;
using System.Collections.Generic;
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
				Name = EnsureString(data, "Name")
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

			var bonusesObject = OptionalJObject(data.Data, "Bonuses");
			if (bonusesObject != null)
			{
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
