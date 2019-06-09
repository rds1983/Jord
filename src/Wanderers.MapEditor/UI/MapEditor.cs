using Wanderers.Core;
using Microsoft.Xna.Framework;
using Myra;
using Myra.Attributes;
using Myra.Graphics2D.UI;
using Newtonsoft.Json;
using System;
using Wanderers.UI;
using Wanderers.Utils;

namespace Wanderers.MapEditor.UI
{
	public class MapEditor : MapRender
	{
		private Point? _markPosition;

		[JsonIgnore]
		[HiddenInEditor]
		public Point? MarkPosition
		{
			get
			{
				return _markPosition;
			}

			set
			{
				if (value == _markPosition)
				{
					return;
				}

				_markPosition = value;

				var ev = MarkPositionChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler MarkPositionChanged;

		protected override void BeforeDraw(RenderContext context)
		{
			base.BeforeDraw(context);

			if (Map.SpawnSpot != null)
			{
				var screen = GameToScreen(Map.SpawnSpot.Value);

				var rect = new Rectangle(screen.X, screen.Y, TileSize.X, TileSize.Y);
				context.Batch.FillRectangle(rect, Color.LightGreen);
			}

			if (MarkPosition != null)
			{
				var screen = GameToScreen(MarkPosition.Value);

				var rect = new Rectangle(screen.X, screen.Y, TileSize.X, TileSize.Y);
				context.Batch.FillRectangle(rect, Color.Blue);
			}
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			if (Map == null)
			{
				return;
			}

			var gameCoords = ScreenToGame(MousePosition);

			if (gameCoords.X >= Map.Size.X || gameCoords.Y >= Map.Size.Y)
			{
				MarkPosition = null;
			}
			else
			{
				MarkPosition = new Point((int)gameCoords.X, (int)gameCoords.Y);
			}

			if (Desktop.LastMouseInfo.IsLeftButtonDown)
			{
				ProcessMouseDown();
			}
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			MarkPosition = null;
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			if (mb == MouseButtons.Left)
			{
				ProcessMouseDown();
			}
		}

		private void ProcessMouseDown()
		{
			if (Studio.Instance == null)
			{
				return;
			}

			switch (Studio.Instance.UI._comboItemTypes.SelectedIndex)
			{
				case 0:
					if (Studio.Instance.UI._listBoxItems.SelectedIndex >= 0)
					{
						var info = (TileInfo)Studio.Instance.UI._listBoxItems.SelectedItem.Tag;

						Map.GetTileAt(MarkPosition.Value).Info = info;

						Studio.Instance.UI._mapNavigation.Invalidate();
					}
					break;
				case 1:
					if (Studio.Instance.UI._listBoxItems.SelectedIndex >= 0)
					{
						var tile = Map.GetTileAt(MarkPosition.Value);
						if (tile.Creature != null)
						{
							// Remove existing
							tile.Creature.Remove();
						}

						var tag = Studio.Instance.UI._listBoxItems.SelectedItem.Tag;
						if (tag == null)
						{
							// Eraser
							break;
						}

						var info = (CreatureInfo)tag;
						var npc = new NonPlayer(info);
						npc.Place(Map, tile.Position.ToVector2());
					}
					break;
			}

			Studio.Instance.UI._mapNavigation.Invalidate();
		}
	}
}