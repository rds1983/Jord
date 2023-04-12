using Jord.Core;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra;
using Myra.Graphics2D;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using Jord.Utils;
using System;

namespace Jord.UI
{
	public class MapNavigationBase : Widget
	{
		[Flags]
		public enum InvalidateStatus
		{
			None = 0,
			InvalidTiles = 1,
			InvalidObjects = 2,
			InvalidAll = InvalidObjects | InvalidTiles,
		}

		private class LayerInfo
		{
			public RenderTarget2D RenderTarget;
			public TextureRegion Image;
		}

		private readonly SpriteBatch _spriteBatch;
		private readonly LayerInfo[] _layers;
		private Vector2 _tileSize;
		private InvalidateStatus _status;

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

			_layers = new LayerInfo[2];
			for(var i = 0; i < _layers.Length; i++)
			{
				_layers[i] = new LayerInfo();
			}
		}

		public void UpdateImage()
		{
			if (_status == InvalidateStatus.None)
			{
				return;
			}

			if (Map == null)
			{
				_layers[0].Image = null;
				_status = InvalidateStatus.None;
				return;
			}

			var bounds = GetMapBounds();

			if (_layers[0].RenderTarget == null ||
				_layers[0].RenderTarget.Width != bounds.Width ||
				_layers[0].RenderTarget.Height != bounds.Height)
			{
				for (var i = 0; i < _layers.Length; ++i)
				{
					_layers[i].RenderTarget = new RenderTarget2D(MyraEnvironment.GraphicsDevice, bounds.Width, bounds.Height);
				}
			}

			try
			{

				_tileSize = new Vector2((float)bounds.Width / Map.Width, (float)bounds.Height / Map.Height);

				var font = TJ.Database.Settings.FontSystem.GetFont(_tileSize.Y);

				// Redraw tiles
				if (_status.HasFlag(InvalidateStatus.InvalidTiles))
				{
					_spriteBatch.Begin();
					MyraEnvironment.GraphicsDevice.SetRenderTarget(_layers[0].RenderTarget);
					MyraEnvironment.GraphicsDevice.Clear(Color.Black);
					for (var mapY = 0; mapY < Map.Height; ++mapY)
					{
						for (var mapX = 0; mapX < Map.Width; ++mapX)
						{
							var tile = Map[mapX, mapY];
							if (!IgnoreFov && !tile.IsExplored)
							{
								continue;
							}

							var pos = new Vector2(mapX, mapY);
							var screen = pos * _tileSize;
							var opacity = 1.0f;
							var appearance = tile.Appearance;

							if (appearance != null)
							{
								var sz = font.MeasureString(appearance.Symbol);
								var screen2 = new Vector2((int)(screen.X + sz.X / 2),
									(int)(screen.Y + sz.Y / 2));

								_spriteBatch.DrawString(font, appearance.Symbol, screen2, appearance.Color * opacity);
							}
						}
					}

					_layers[0].Image = new TextureRegion(_layers[0].RenderTarget);
					_spriteBatch.End();
				}

				// Redraw objects
				if (_status.HasFlag(InvalidateStatus.InvalidObjects))
				{
					_spriteBatch.Begin();
					MyraEnvironment.GraphicsDevice.SetRenderTarget(_layers[1].RenderTarget);
					MyraEnvironment.GraphicsDevice.Clear(Color.Transparent);

					for (var mapY = 0; mapY < Map.Height; ++mapY)
					{
						for (var mapX = 0; mapX < Map.Width; ++mapX)
						{
							var tile = Map[mapX, mapY];

							if (!IgnoreFov && !tile.IsInFov)
							{
								continue;
							}

							var pos = new Vector2(mapX, mapY);
							var screen = pos * _tileSize;
							var opacity = 1.0f;
							Appearance appearance = null;

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

					_layers[1].Image = new TextureRegion(_layers[1].RenderTarget);
					_spriteBatch.End();
				}
			}
			finally
			{
				MyraEnvironment.GraphicsDevice.SetRenderTarget(null);
			}

			_status = InvalidateStatus.None;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			UpdateImage();

			if (_layers[0].Image == null)
			{
				return;
			}

			var bounds = GetMapBounds();

			for (var i = 0; i < _layers.Length; ++i)
			{
				_layers[i].Image.Draw(context, bounds);
			}

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

		public void InvalidateImage(bool onlyObjects = false)
		{
			if (!onlyObjects)
			{
				_status = InvalidateStatus.InvalidAll;
			} else if (!_status.HasFlag(InvalidateStatus.InvalidObjects))
			{
				_status |= InvalidateStatus.InvalidObjects;
			}
		}
	}
}