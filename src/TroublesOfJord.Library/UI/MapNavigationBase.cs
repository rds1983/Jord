using System.Collections.Generic;
using TroublesOfJord.Core;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra;
using System;
using TroublesOfJord.Utils;

namespace TroublesOfJord.UI
{
	public class MapNavigationBase : Widget
	{
		public static readonly Point TileSize = new Point(2, 2);
		private static readonly HashSet<string> _secondLayerTiles = new HashSet<string>() { "Wall", "Ground" };

		private ColorBuffer _colorBuffer;
		private TextureRegion _image;
		private bool _dirty = true;

		public MapRender MapEditor { get; set; }

		public Map Map => MapEditor.Map;
		public Vector2 TopLeft => MapEditor.TopLeft;
		public Point GridSize => MapEditor.GridSize;

		public float MapSizeRatio
		{
			get
			{
				return (float)Map.Size.X / Map.Size.Y;
			}
		}

		public MapNavigationBase()
		{
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
					var tile = Map[mapX, mapY];
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

								if (!tile.Visible)
								{
									continue;
								}

								var color = tile.Info.Image.Color;
								if (tile.Creature != null && !(tile.Creature is Player))
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

			int x, width;
			GetScreenHorizontal(out x, out width);

			var bounds = ActualBounds;
			bounds.X = x;
			bounds.Width = width;
			context.Draw(_image, bounds);

			var topLeft = GameToScreen(TopLeft);
			var size = GameToScreen(TopLeft + GridSize.ToVector2()) - topLeft;
			context.DrawRectangle(new Rectangle(topLeft.X, topLeft.Y, size.X, size.Y), Color.White);
		}

		public void DrawAppearance(RenderContext context, Color color, Rectangle rect)
		{
			context.FillRectangle(rect, color);
		}

		private void GetScreenHorizontal(out int x, out int width)
		{
			var bounds = ActualBounds;

			width = (int)(bounds.Height * MapSizeRatio);
			x = bounds.X + (bounds.Width - width) / 2;
		}

		public Point GameToScreen(Vector2 gamePosition)
		{
			int x, width;
			GetScreenHorizontal(out x, out width);

			return new Point(x + (int)(gamePosition.X * width / Map.Size.X),
				ActualBounds.Y + (int)(gamePosition.Y * ActualBounds.Height / Map.Size.Y));
		}

		public Vector2 ScreenToGame(Point position)
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

		public void InvalidateImage()
		{
			_dirty = true;
		}
	}
}