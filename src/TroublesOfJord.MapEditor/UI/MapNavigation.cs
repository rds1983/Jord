using TroublesOfJord.Core;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework.Input;
using System.Xml.Serialization;
using System.ComponentModel;
using TroublesOfJord.UI;

namespace TroublesOfJord.MapEditor.UI
{
	public class MapNavigation : MapNavigationBase
	{
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

			if (Map != null && pos.X + MapEditor.GridSize.X >= Map.Size.X)
			{
				pos.X = Map.Size.X - MapEditor.GridSize.X;
			}

			if (pos.Y < 0)
			{
				pos.Y = 0;
			}

			if (Map != null && pos.Y + MapEditor.GridSize.Y >= Map.Size.Y)
			{
				pos.Y = Map.Size.Y - MapEditor.GridSize.Y;
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