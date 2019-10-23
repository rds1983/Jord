using Wanderers.Core;

namespace Wanderers.Generation
{
	public abstract class BaseGenerator: ItemWithId
	{
		public int Width = 64;
		public int Height = 64;

		public abstract Map Generate();
	}
}
