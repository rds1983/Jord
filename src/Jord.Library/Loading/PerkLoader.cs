using Jord.Core;
using Jord.Core.Abilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Jord.Loading
{
	class PerkLoader : BaseObjectLoader<Perk>
	{
		public static readonly PerkLoader Instance = new PerkLoader();

		protected override Perk CreateObject(string source, JObject data, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			var result = new Perk
			{
				Name = data.EnsureString("Name"),
				Description = data.EnsureString("Description"),
				Tier = data.EnsureInt("Tier")
			};

			string category;
			if (properties != null && properties.TryGetValue("Category", out category))
			{
				result.Category = category;
			}

			var addsEffectsIds = new List<string>();
			var addsEffectsObject = data.Optional("AddsEffects");
			if (addsEffectsObject!= null)
			{
				addsEffectsIds.Add(addsEffectsObject.ToString());
			}

			var requiresPerksIds = new List<string>();
			var requiresPerksObject = data.Optional("RequiresPerks");
			if (requiresPerksObject != null)
			{
				requiresPerksIds.Add(requiresPerksObject.ToString());
			}

			secondRunAction = db =>
			{
				var addsEffects = new List<Effect>();
				foreach(var effectId in addsEffectsIds)
				{
					addsEffects.Add(db.Effects.Ensure(effectId));
				}

				var requiresPerks = new List<Perk>();
				foreach(var perkId in requiresPerksIds)
				{
					requiresPerks.Add(db.Perks.Ensure(perkId));
				}

				result.AddsEffects = addsEffects.ToArray();
				result.RequiresPerks= requiresPerks.ToArray();
			};

			return result;
		}
	}
}
