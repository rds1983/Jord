using Jord.Core;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using Jord.UI;
using System.Xml.Serialization;
using System.ComponentModel;
using Myra.Graphics2D;

namespace Jord.MapEditor.UI
{
	public class MapEditor : MapRender
	{
		private Point? _markPosition;

		[XmlIgnore]
		[Browsable(false)]
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

				MarkPositionChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public event EventHandler MarkPositionChanged;

		public MapEditor()
		{
			IgnoreFov = true;
		}

		protected override void BeforeDraw(RenderContext context)
		{
			base.BeforeDraw(context);

			if (Map == null)
			{
				return;
			}

			if (Map.SpawnSpot != null)
			{
				var screen = GameToScreen(Map.SpawnSpot.Value);

				var rect = new Rectangle(screen.X, screen.Y, TileSize.X, TileSize.Y);
				context.FillRectangle(rect, Color.LightGreen);
			}

			if (MarkPosition != null)
			{
				var screen = GameToScreen(MarkPosition.Value);

				var rect = new Rectangle(screen.X, screen.Y, TileSize.X, TileSize.Y);
				context.FillRectangle(rect, Color.Blue);
			}
		}

		protected override void BeforeDrawTile(RenderContext context, Tile tile)
		{
			base.BeforeDrawTile(context, tile);

			if (tile.Exit == null)
			{
				return;
			}

			var screen = GameToScreen(tile.Position);

			var rect = new Rectangle(screen.X, screen.Y, TileSize.X, TileSize.Y);
			context.FillRectangle(rect, Color.Blue);
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			if (Map == null)
			{
				return;
			}

			var gameCoords = ScreenToGame(Desktop.MousePosition);

			if (gameCoords.X >= Map.Width || gameCoords.Y >= Map.Height)
			{
				MarkPosition = null;
			}
			else
			{
				MarkPosition = new Point((int)gameCoords.X, (int)gameCoords.Y);
			}

			if (Desktop.IsTouchDown)
			{
				ProcessMouseDown();
			}
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			MarkPosition = null;
		}

		public override bool OnTouchDown()
		{
			var result = base.OnTouchDown();
			ProcessMouseDown();

			return result;
		}

		private void ProcessMouseDown()
		{
			if (Studio.Instance == null || MarkPosition == null)
			{
				return;
			}

			switch (Studio.Instance.UI._comboItemTypes.SelectedIndex)
			{
				case 0:
					if (Studio.Instance.UI._listBoxItems.SelectedIndex >= 0)
					{
						var info = (TileInfo)Studio.Instance.UI._listBoxItems.SelectedItem.Tag;

						Map[MarkPosition.Value].Info = info;
					}
					break;
				case 1:
					if (Studio.Instance.UI._listBoxItems.SelectedIndex >= 0)
					{
						var tile = Map[MarkPosition.Value];
						if (tile.Object != null)
						{
							// Remove existing
							tile.Object = null;
						}

						var tag = Studio.Instance.UI._listBoxItems.SelectedItem.Tag;
						if (tag == null)
						{
							// Eraser
							break;
						}

						tile.Object = (TileObject)tag;
					}
					break;
				case 2:
					if (Studio.Instance.UI._listBoxItems.SelectedIndex >= 0)
					{
						var tile = Map[MarkPosition.Value];
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
						npc.Place(Map, tile.Position);
					}
					break;
			}

			Studio.Instance.IsDirty = true;
			Studio.Instance.UI._mapNavigation.OnMapChanged();
		}
	}
}