using Jord.Core;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework.Input;
using Jord.UI;

namespace Jord.MapEditor.UI
{
	public class MapNavigation : MapNavigationBase
	{
		public MapNavigation()
		{
			IgnoreFov = true;
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			if (Map == null)
			{
				return;
			}

			var state = Mouse.GetState();
			if (state.LeftButton == ButtonState.Pressed)
			{
				ProcessMouseDown();
			}
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (Map == null)
			{
				return;
			}

			ProcessMouseDown();
		}

		private Vector2 FixPos(Vector2 pos)
		{
			if (pos.X < 0)
			{
				pos.X = 0;
			}

			if (Map != null && pos.X + MapEditor.GridSize.X >= Map.Width)
			{
				pos.X = Map.Width - MapEditor.GridSize.X;
			}

			if (pos.Y < 0)
			{
				pos.Y = 0;
			}

			if (Map != null && pos.Y + MapEditor.GridSize.Y >= Map.Height)
			{
				pos.Y = Map.Height - MapEditor.GridSize.Y;
			}

			return pos;
		}

		private void ProcessMouseDown()
		{
			var pos = ScreenToGame(Desktop.MousePosition);

			pos.X -= MapEditor.GridSize.X / 2;
			pos.Y -= MapEditor.GridSize.Y / 2;

			pos = FixPos(pos);

			MapEditor.TopLeft = pos;
		}

		public void OnMapChanged()
		{
			InvalidateImage();
			MapEditor.TopLeft = FixPos(MapEditor.TopLeft);
		}
	}
}