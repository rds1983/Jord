using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using System;
using System.IO;

namespace Jord.Core
{
	public class Appearance
	{
		public string Symbol { get; private set; }

		public Color Color { get; private set; }

		public TextureRegion TextureRegion { get; private set; }

		public Appearance(string symbol, Color color, TextureRegion image)
		{
			if (string.IsNullOrEmpty(symbol))
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			Symbol = symbol;
			Color = color;
			TextureRegion = image;
		}

		public void Draw(RenderContext context, Rectangle rect, float opacity = 1.0f)
		{
			if (TextureRegion != null)
			{
				var r = new Rectangle(rect.X + (rect.Width - TextureRegion.Size.X) / 2,
					rect.Y + (rect.Height - TextureRegion.Size.Y) / 2,
					TextureRegion.Size.X,
					TextureRegion.Size.Y);

				TextureRegion.Draw(context, r, Color.White * opacity);
			}
			else
			{
				var font = TJ.Database.ModuleInfo.Font;
				var sz = font.MeasureString(Symbol);

				var pos = new Vector2((int)(rect.X + (rect.Width - sz.X) / 2),
					(int)(rect.Y + (rect.Height - sz.Y) / 2));

				context.DrawString(font, Symbol, pos, Color * opacity);
			}
		}
	}
}