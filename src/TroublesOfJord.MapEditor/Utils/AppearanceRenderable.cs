using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using System;
using TroublesOfJord.Core;

namespace TroublesOfJord.Utils
{
	public class AppearanceRenderable : IImage
	{
		private readonly Appearance _appearance;

		public Point Size
		{
			get
			{
				return _appearance.Image.Size;
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
			_appearance.Draw(batch, dest);
		}
	}
}
