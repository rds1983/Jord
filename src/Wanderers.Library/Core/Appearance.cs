using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wanderers.Core
{
	public class Appearance
	{
		public static Appearance MerchantSign = new Appearance('$', Color.Gold);

		private readonly string _symbol;

		public char Symbol
		{
			get
			{
				return _symbol[0];
			}
		}

		public Color Color
		{
			get; private set;
		}

		public Appearance(char symbol, Color color)
		{
			_symbol = symbol.ToString();
			Color = color;
		}

		public void Draw(SpriteBatch batch, SpriteFont font, Rectangle rect, float opacity = 1.0f)
		{
			var measureSize = font.MeasureString(_symbol);

			var pos = new Vector2(rect.X + (rect.Width - measureSize.X) / 2,
				rect.Y + (rect.Height - measureSize.Y) / 2);

			batch.DrawString(font, _symbol, pos, Color * opacity);
		}
	}
}