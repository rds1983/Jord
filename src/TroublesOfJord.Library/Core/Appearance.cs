using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using System;

namespace TroublesOfJord.Core
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

		public void Draw(SpriteBatch batch, Rectangle rect, float opacity = 1.0f)
		{
			var r = new Rectangle(rect.X + (rect.Width - TextureRegion.Size.X) / 2,
				rect.Y + (rect.Height - TextureRegion.Size.Y) / 2,
				TextureRegion.Size.X,
				TextureRegion.Size.Y);

			TextureRegion.Draw(batch, r, Color * opacity);
		}
	}
}