namespace TroublesOfJord.Core
{
	public interface IUsableAbility
	{
		string Name { get; }
		bool CanAuto { get; }

		void Use();
	}
}
