namespace Wanderers.Core.Skills
{
	public class Kick : ISkill, IUsableAbility
	{
		public string Name => "Kick";

		public AbilityUsage Usage => AbilityUsage.OnlyInFight;

		public int UsageDelayInMs => Player.PlayerRoundInMs;

		bool IUsableAbility.CanAuto => true;

		public void Use()
		{
			throw new System.NotImplementedException();
		}
	}
}
