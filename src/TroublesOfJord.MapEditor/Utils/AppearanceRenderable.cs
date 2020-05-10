using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using System;
using TroublesOfJord.Core;

namespace TroublesOfJord.Utils
{
	public class AppearanceRenderable : IImage
	{
		private Point? _size;
		private SpriteFont _font;
		private readonly Appearance _appearance;

		public Point Size
		{
			get
			{
				if (_size != null)
				{
					return _size.Value;
				}

				_size = Font.MeasureString(_appearance.Symbol.ToString()).ToPoint();
				return _size.Value;
			}
		}

		public SpriteFont Font
		{
			get
			{
				return _font;
			}

			set
			{
				if (value == _font)
				{
					return;
				}

				_font = value;
				_size = null;
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

		public void Draw(SpriteBatch batch, Rectangle dest, Color color)
		{
			_appearance.Draw(batch, Font, dest);
		}
	}
}
