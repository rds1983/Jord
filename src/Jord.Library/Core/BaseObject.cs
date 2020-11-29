namespace Jord.Core
{
	public interface IBaseObject
	{
		string Id { get; set; }
		string Source { get; set; }
	}

	public class BaseObject : IBaseObject
	{
		public string Id { get; set; }

		public string Source { get; set; }

		public override string ToString()
		{
			return Id;
		}
	}
}