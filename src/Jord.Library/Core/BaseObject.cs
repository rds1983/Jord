namespace Jord.Core
{
	public class BaseObject
	{
		public string Id { get; set; }

		public string Source { get; set; }

		public override string ToString()
		{
			return Id;
		}
	}
}