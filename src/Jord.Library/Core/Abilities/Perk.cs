namespace Jord.Core.Abilities
{
	public class Perk : BaseObject
	{
		public string Name { get; set; }
		public string Description { get; set; }

		public Effect[] AddsEffects { get; set; }
		public Perk[] RequiresPerks { get; set; }
	}
}
