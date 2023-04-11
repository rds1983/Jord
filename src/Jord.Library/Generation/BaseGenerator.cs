using Jord.Core;

namespace Jord.Generation
{
	public abstract class BaseGenerator: BaseObject
	{
		public int Width { get; }
		public int Height { get; }

		protected BaseGenerator(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public abstract Map Generate();

		public void Step()
		{
		}
	}
}
