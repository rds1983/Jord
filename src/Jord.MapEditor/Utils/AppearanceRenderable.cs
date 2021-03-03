using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using System;
using Jord.Core;

namespace Jord.Utils
{
	public class AppearanceRenderable : IImage
	{
		private readonly Appearance _appearance;

		public Point Size
		{
			get
			{
				return _appearance.TextureRegion.Size;
			}
		}

		public AppearanceRenderable(Appearance appearance)
		{
			if (appearance == null)
			{
				throw new ArgumentNullException(nameof(appearance));
			}

			_appearance = appearance;
		}

		public void Draw(RenderContext context, Rectangle dest, Color color)
		{
			_appearance.Draw(context, dest);
		}
	}
}
