using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Wanderers.UI
{
	public class EndlessContainer<T>: SingleItemContainer<T> where T : Widget
	{
		public override void Arrange()
		{
			base.Arrange();

			var bounds = ActualBounds;
			var availableSize = bounds.Size();
			var measureSize = InternalChild.Measure(availableSize);

			bounds.Width = measureSize.X;
			bounds.Height = measureSize.Y;
			InternalChild.Layout(bounds);
		}
	}
}
