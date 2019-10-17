using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using Wanderers.Core;
using Wanderers.Utils;

namespace Wanderers.UI
{
	public class MapView: MapRender
	{
		protected override void BeforeDraw(RenderContext context)
		{
			base.BeforeDraw(context);

			var tl = new Vector2(TJ.Session.Player.Position.X - GridSize.X / 2,
				TJ.Session.Player.Position.Y - GridSize.Y / 2);

			if (tl.X < 0)
			{
				tl.X = 0;
			}

			if (tl.X >= Map.Size.X - GridSize.X)
			{
				tl.X = Map.Size.X - GridSize.X;
			}

			if (tl.Y < 0)
			{
				tl.Y = 0;
			}

			if (tl.Y >= Map.Size.Y - GridSize.Y)
			{
				tl.Y = Map.Size.Y - GridSize.Y;
			}

			TopLeft = tl;
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			var gameCoords = ScreenToGame(Desktop.MousePosition);
			if (gameCoords.X < 0 || gameCoords.Y < 0 || gameCoords.X >= Map.Size.X || gameCoords.Y >= Map.Size.Y)
			{
				return;
			}

			var tile = Map.GetTileAt(gameCoords);
			if (!tile.Info.Passable)
			{
				return;
			}

			if (tile == TJ.Session.Player.Tile)
			{
				return;
			}

			var distance = 0.0f;
			Action finished = null;
			if (tile.Creature != null)
			{
				var npc = (NonPlayer)tile.Creature;

				if (npc.Info.IsAttackable)
				{
					distance = 1.0f;

					finished = () =>
					{
						TJ.Session.Player.Attack(npc);
					};
				}
				else if (npc.Info.IsMerchant)
				{
					distance = 2.0f;

					finished = () =>
					{
						var dialog = new TradeDialog(TJ.Session.Player, tile.Creature);
						dialog.ShowModal(Desktop);
					};
				}
			}

			TJ.Session.Player.InitiateMovement(gameCoords.ToPoint(), distance, finished);
		}
	}
}
