using System.Collections.Generic;
using Wanderers.Core;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Newtonsoft.Json;
using Wanderers.Utils;
using Microsoft.Xna.Framework.Input;
using Myra;
using System;

namespace Wanderers.MapEditor.UI
{
	public class MapNavigation : Widget
	{
		private static readonly Point TileSize = new Point(2, 2);
		private static readonly HashSet<string> _secondLayerTiles = new HashSet<string>() { "wall", "ground" };

		private ColorBuffer _colorBuffer;
		private TextureRegion _image;
		private bool _dirty = true;

		[JsonIgnore]
		[HiddenInEditor]
		public MapEditor MapEditor { get; set; }

		[JsonIgnore]
		[HiddenInEditor]
		public Map Map
		{
			get
			{
				if (MapEditor == null)
				{
					return null;
				}

				return MapEditor.Map;
			}
		}

		public MapNavigation()
		{
			CanFocus = true;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		private void UpdateLayer(Func<Tile, bool> checker)
		{
			// Bottom layer
			for (var mapY = 0; mapY < Map.Size.Y; ++mapY)
			{
				for (var mapX = 0; mapX < Map.Size.X; ++mapX)
				{
					var tile = Map.GetTileAt(mapX, mapY);
					var rect = new Rectangle(mapX, mapY, TileSize.X, TileSize.Y);

					if (checker(tile))
					{
						for (var y = rect.Top; y < rect.Bottom; ++y)
						{
							for (var x = rect.Left; x < rect.Right; ++x)
							{
								if (x >= Map.Size.X ||
									y >= Map.Size.Y)
								{
									continue;
								}

								var color = tile.Info.Image.Color;
								if (tile.Creature != null)
								{
									color = tile.Creature.Image.Color;
								}

								_colorBuffer[x, y] = color;
							}
						}
					}
				}
			}
		}

		private void UpdateImage()
		{
			if (!_dirty)
			{
				return;
			}

			if (_colorBuffer == null || 
				_colorBuffer.Width != Map.Size.X ||
				_colorBuffer.Height != Map.Size.Y)
			{
				_colorBuffer = new ColorBuffer(Map.Size.X, Map.Size.Y);
			}

			// Bottom layer
			UpdateLayer(tile => !_secondLayerTiles.Contains(tile.Info.Id));

			// Top layer
			UpdateLayer(tile => _secondLayerTiles.Contains(tile.Info.Id));

			var texture = _colorBuffer.CreateTexture2D(MyraEnvironment.GraphicsDevice);
			_image = new TextureRegion(texture);

			_dirty = false;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (Map == null)
			{
				return;
			}

			UpdateImage();

			context.Draw(_image, ActualBounds);

			var topLeft = GameToScreen(MapEditor.TopLeft);
			var size = GameToScreen(MapEditor.TopLeft + MapEditor.GridSize.ToVector2()) - topLeft;
			context.DrawRectangle(new Rectangle(topLeft.X, topLeft.Y, size.X, size.Y), Color.White);
		}

		private void DrawAppearance(RenderContext context, Color color, Rectangle rect)
		{
			context.FillRectangle(rect, color);
		}

		private Point GameToScreen(Vector2 gamePosition)
		{
			return new Point(ActualBounds.X + (int)(gamePosition.X * ActualBounds.Width / Map.Size.X),
				ActualBounds.Y + (int)(gamePosition.Y * ActualBounds.Height / Map.Size.Y));
		}

		private Point GameToScreen(Point gamePosition)
		{
			return new Point(ActualBounds.X + gamePosition.X * ActualBounds.Width / Map.Size.X,
				ActualBounds.Y + gamePosition.Y * ActualBounds.Height / Map.Size.Y);
		}

		private Vector2 ScreenToGame(Point position)
		{
			position -= ActualBounds.Location;

			var tilePosition = new Vector2
			{
				X = position.X * Map.Size.X / ActualBounds.Width,
				Y = position.Y * Map.Size.Y / ActualBounds.Height,
			};

			if (tilePosition.X < 0)
			{
				tilePosition.X = 0;
			}

			if (tilePosition.X >= Map.Size.X)
			{
				tilePosition.X = Map.Size.X - 1;
			}

			if (tilePosition.Y < 0)
			{
				tilePosition.Y = 0;
			}

			if (tilePosition.Y >= Map.Size.Y)
			{
				tilePosition.Y = Map.Size.Y - 1;
			}

			return tilePosition;
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

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			if (Map == null)
			{
				return;
			}

			if (mb == MouseButtons.Left)
			{
				ProcessMouseDown();
			}
		}

		private void ProcessMouseDown()
		{
			var pos = ScreenToGame(Desktop.MousePosition);

			pos.X -= MapEditor.GridSize.X / 2;
			pos.Y -= MapEditor.GridSize.Y / 2;

			if (pos.X < 0)
			{
				pos.X = 0;
			}

			if (pos.X + MapEditor.GridSize.X >= Map.Size.X)
			{
				pos.X = Map.Size.X - MapEditor.GridSize.X;
			}

			if (pos.Y < 0)
			{
				pos.Y = 0;
			}

			if (pos.Y + MapEditor.GridSize.Y >= Map.Size.Y)
			{
				pos.Y = Map.Size.Y - MapEditor.GridSize.Y;
			}

			MapEditor.TopLeft = pos;
		}

		public void Invalidate()
		{
			_dirty = true;
		}
	}
}