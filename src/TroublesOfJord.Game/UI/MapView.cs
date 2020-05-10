using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.UI;
using System;
using TroublesOfJord.Core;
using TroublesOfJord.Utils;

namespace TroublesOfJord.UI
{
	public class MapView : MapRender
	{
		private bool DrawHighlight;

		protected override void BeforeDraw(RenderContext context)
		{
			base.BeforeDraw(context);

			Map = TJ.Session.Player.Map;

			var tl = new Vector2(TJ.Session.Player.DisplayPosition.X - GridSize.X / 2,
				TJ.Session.Player.DisplayPosition.Y - GridSize.Y / 2);

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

		protected override void BeforeDrawTile(RenderContext context, Tile tile)
		{
			base.BeforeDrawTile(context, tile);

			if (Config.DrawHighlight && tile.Highlighted)
			{
				var screen = GameToScreen(tile.Position);

				var rect = new Rectangle(screen.X, screen.Y, TileSize.X, TileSize.Y);
				context.Batch.FillRectangle(rect, Color.LightGreen);
			}
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			var gameCoords = ScreenToGame(Desktop.MousePosition);
			if (gameCoords.X < 0 || gameCoords.Y < 0 || gameCoords.X >= Map.Size.X || gameCoords.Y >= Map.Size.Y)
			{
				return;
			}

			var tile = Map[gameCoords.ToPoint()];
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
						dialog.ShowModal();
					};
				}
			}

			TJ.Session.Player.InitiateMovement(gameCoords.ToPoint(), distance, finished);
		}
	}
}
