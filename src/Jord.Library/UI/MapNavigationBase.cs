using Jord.Core;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra;
using Myra.Graphics2D;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using Jord.Utils;

namespace Jord.UI
{
	public class MapNavigationBase : Widget
	{
		private TextureRegion _image;
		private readonly SpriteBatch _spriteBatch;
		private RenderTarget2D _renderTarget;
		private Vector2 _tileSize;
		private bool _dirty = true;

		public MapRender MapEditor { get; set; }

		public Map Map => MapEditor.Map;
		public Vector2 TopLeft => MapEditor.TopLeft;
		public Point GridSize => MapEditor.GridSize;


		public bool IgnoreFov;

		public MapNavigationBase()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			_spriteBatch = new SpriteBatch(MyraEnvironment.GraphicsDevice);
		}

		public void UpdateImage()
		{
			if (_dirty == false)
			{
				return;
			}

			if (Map == null)
			{
				_image = null;
				_dirty = false;
				return;
			}

			var bounds = GetMapBounds();

			if (_renderTarget == null ||
				_renderTarget.Width != bounds.Width ||
				_renderTarget.Height != bounds.Height)
			{
				_renderTarget = new RenderTarget2D(MyraEnvironment.GraphicsDevice, bounds.Width, bounds.Height, false, SurfaceFormat.Color, DepthFormat.None);
			}

			try
			{
				MyraEnvironment.GraphicsDevice.SetRenderTarget(_renderTarget);
				MyraEnvironment.GraphicsDevice.Clear(Color.Black);

				_spriteBatch.Begin();

				_tileSize = new Vector2((float)bounds.Width / Map.Width, (float)bounds.Height / Map.Height);

				var font = TJ.Database.Settings.FontSystem.GetFont(_tileSize.Y);
				for (var mapY = 0; mapY < Map.Height; ++mapY)
				{
					for (var mapX = 0; mapX < Map.Width; ++mapX)
					{
						var tile = Map[mapX, mapY];

						var pos = new Vector2(mapX, mapY);
						var screen = pos * _tileSize;
						var opacity = 1.0f;
						var appearance = tile.Appearance;

						if (tile.Object != null)
						{
							appearance = tile.Object.Image;
						}

						if (tile.Inventory.Items.Count > 0)
						{
							appearance = tile.Inventory.Items[0].Item.Info.Image;
						}

						if (tile.Creature != null)
						{
							screen = tile.Creature.DisplayPosition * _tileSize;
							appearance = tile.Creature.Image;
							opacity = tile.Creature.Opacity;
						}

						if (appearance != null)
						{
							var sz = font.MeasureString(appearance.Symbol);
							var screen2 = new Vector2((int)(screen.X + sz.X / 2),
								(int)(screen.Y + sz.Y / 2));

							_spriteBatch.DrawString(font, appearance.Symbol, screen2, appearance.Color * opacity);
						}
					}
				}
			}
			finally
			{
				_spriteBatch.End();
				MyraEnvironment.GraphicsDevice.SetRenderTarget(null);
			}

			_image = new TextureRegion(_renderTarget);
			_dirty = false;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			UpdateImage();

			if (_image == null)
			{
				return;
			}

			var bounds = GetMapBounds();
			_image.Draw(context, bounds);

			var topLeft = GameToScreen(TopLeft);
			var size = GameToScreen(TopLeft + GridSize.ToVector2()) - topLeft;
			context.DrawRectangle(new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)size.X, (int)size.Y), Color.White);
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

		public Vector2 GameToScreen(Vector2 pos)
		{
			var bounds = GetMapBounds();
			return new Vector2(bounds.Left + pos.X * _tileSize.X, bounds.Top + pos.Y * _tileSize.Y);
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

		public override void InternalArrange()
		{
			base.InternalArrange();
			InvalidateImage();
		}

		public void InvalidateImage()
		{
			_dirty = true;
		}
	}
}