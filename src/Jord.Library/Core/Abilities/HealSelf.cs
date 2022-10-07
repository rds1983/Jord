namespace Jord.Core.Abilities
{
	public class HealSelf: AbilityEffect
	{
		public int Minimum { get; set; }
		public int Maximum { get; set; }
		public string MessageActivated { get; set; }
	}
}