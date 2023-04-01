using System.Collections.Generic;
using Jord.Core;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra;
using System;
using Jord.Utils;
using Myra.Graphics2D;

namespace Jord.UI
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
				return (float)Map.Width / Map.Height;
			}
		}

		public bool IgnoreFov;

		public MapNavigationBase()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		private void UpdateLayer(Func<Tile, bool> checker)
		{
			// Bottom layer
			for (var mapY = 0; mapY < Map.Height; ++mapY)
			{
				for (var mapX = 0; mapX < Map.Width; ++mapX)
				{
					var tile = Map[mapX, mapY];
					var rect = new Rectangle(mapX, mapY, TileSize.X, TileSize.Y);

					if (checker(tile))
					{
						for (var y = rect.Top; y < rect.Bottom; ++y)
						{
							for (var x = rect.Left; x < rect.Right; ++x)
							{
								if (x >= Map.Width ||
									y >= Map.Height)
								{
									continue;
								}

								if (!IgnoreFov && !tile.IsExplored)
								{
									continue;
								}

								var isInFov = IgnoreFov || tile.IsInFov;

								var color = tile.Info.Image.Color;

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
				_colorBuffer.Width != Map.Width ||
				_colorBuffer.Height != Map.Height)
			{
				_colorBuffer = new ColorBuffer(Map.Width, Map.Height);
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

			var bounds = GetMapBounds();
			_image.Draw(context, bounds);

			var topLeft = GameToScreen(TopLeft);
			var size = GameToScreen(TopLeft + GridSize.ToVector2()) - topLeft;
			context.DrawRectangle(new Rectangle(topLeft.X, topLeft.Y, size.X, size.Y), Color.White);
		}

		public void DrawAppearance(RenderContext context, Color color, Rectangle rect)
		{
			context.FillRectangle(rect, color);
		}

		private Rectangle GetMapBounds()
		{
			var bounds = ActualBounds;

			// Keep the map size ratio
			if (bounds.Width < bounds.Height)
			{
				var newHeight = bounds.Width * Map.Height / Map.Width;
				var newY = bounds.Y + (bounds.Height - newHeight) / 2;
				bounds.Y = newY;
				bounds.Height = newHeight;
			}
			else
			{
				var newWidth = bounds.Height * Map.Width / Map.Height;
				var newX = bounds.X + (bounds.Width - newWidth) / 2;
				bounds.X = newX;
				bounds.Width = newWidth;
			}

			return bounds;
		}

		public Point GameToScreen(Vector2 gamePosition)
		{
			var bounds = GetMapBounds();

			return new Point(bounds.X + (int)(gamePosition.X * bounds.Width / Map.Width),
				bounds.Y + (int)(gamePosition.Y * bounds.Height / Map.Height));
		}

		public Vector2 ScreenToGame(Point position)
		{
			position = ToLocal(position);

			var bounds = GetMapBounds();

			var tilePosition = new Vector2
			{
				X = (position.X - bounds.X) * Map.Width / bounds.Width,
				Y = (position.Y - bounds.Y) * Map.Height / bounds.Height,
			};

			if (tilePosition.X < 0)
			{
				tilePosition.X = 0;
			}

			if (tilePosition.X >= Map.Width)
			{
				tilePosition.X = Map.Width - 1;
			}

			if (tilePosition.Y < 0)
			{
				tilePosition.Y = 0;
			}

			if (tilePosition.Y >= Map.Height)
			{
				tilePosition.Y = Map.Height - 1;
			}

			return tilePosition;
		}

		public void InvalidateImage()
		{
			_dirty = true;
		}
	}
}