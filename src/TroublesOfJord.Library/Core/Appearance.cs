using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using System;

namespace TroublesOfJord.Core
{
	public class Appearance
	{
		private readonly string _symbol;

		public string Symbol
		{
			get
			{
				return _symbol;
			}
		}

		public Color Color
		{
			get; private set;
		}

		public TextureRegion Image;

		public Appearance(string symbol, Color color, TextureRegion image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			_symbol = symbol.ToString();
			Color = color;
			Image = image;
		}

		public void Draw(SpriteBatch batch, Rectangle rect, float opacity = 1.0f)
		{
			var pos = new Vector2(rect.X + (rect.Width - Image.Size.X) / 2,
				rect.Y + (rect.Height - Image.Size.Y) / 2);

			Image.Draw(batch, pos, Color * opacity);
		}
	}
}