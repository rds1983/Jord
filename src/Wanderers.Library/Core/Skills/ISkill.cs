namespace Wanderers.Core.Skills
{
	public interface ISkill
	{
		string Name { get; }
		AbilityUsage Usage { get; }
		int UsageDelayInMs { get; }

		void Use();
	}
}
