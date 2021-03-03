using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using System;

namespace Jord.Core
{
	public class Appearance
	{
		public Color Color
		{
			get; private set;
		}

		public TextureRegion TextureRegion;

		public Appearance(Color color, TextureRegion image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			Color = color;
			TextureRegion = image;
		}

		public void Draw(RenderContext context, Rectangle rect, float opacity = 1.0f)
		{
			var r = new Rectangle(rect.X + (rect.Width - TextureRegion.Size.X) / 2,
				rect.Y + (rect.Height - TextureRegion.Size.Y) / 2,
				TextureRegion.Size.X,
				TextureRegion.Size.Y);

			TextureRegion.Draw(context, r, Color * opacity);
		}
	}
}