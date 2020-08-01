using Microsoft.Xna.Framework.Input;
using Myra.Extended.Widgets;
using Myra.Graphics2D.UI;
using System;
using System.Linq;
using TroublesOfJord.Core;
using TroublesOfJord.Utils;

namespace TroublesOfJord.UI
{
	public partial class GameView
	{
		private const int RepeatKeyDownStartInMs = 400;
		private const int RepeatKeyDownInternalInMs = 40;

		private DateTime?  _lastKeyDown;
		private int _keyDownCount = 0;
		private Keys[] _downKeys, _lastDownKeys;

		public MapView MapView { get; } = new MapView();
		public MiniMap MapNavigation { get; } = new MiniMap();
		public LogView LogView { get; } = new LogView();

		public GameView()
		{
			AcceptsKeyboardFocus = true;

			BuildUI();

			_mapViewContainer.Widgets.Add(MapView);

			MapNavigation.MapEditor = MapView;
			_mapContainer.Widgets.Add(MapNavigation);

			LogView.ShowVerticalScrollBar = false;
			_logContainer.Widgets.Add(LogView);
		}

		private void UpdateKeyboardInput()
		{
			_downKeys = Keyboard.GetState().GetPressedKeys();

			if (_downKeys != null && _lastDownKeys != null)
			{
				var now = DateTime.Now;
				foreach (var key in _downKeys)
				{
					if (!_lastDownKeys.Contains(key))
					{
						KeyDownHandler(key);

						_lastKeyDown = now;
						_keyDownCount = 0;
					}
					else if (_lastKeyDown != null &&
					  ((_keyDownCount == 0 && (now - _lastKeyDown.Value).TotalMilliseconds > RepeatKeyDownStartInMs) ||
					  (_keyDownCount > 0 && (now - _lastKeyDown.Value).TotalMilliseconds > RepeatKeyDownInternalInMs)))
					{
						KeyDownHandler(key);

						_lastKeyDown = now;
						++_keyDownCount;
					}
				}
			}

			_lastDownKeys = _downKeys;
		}

		private bool ProcessMovement(Keys key)
		{
			MovementDirection? direction = null;

			if (key == Keys.Left || key == Keys.NumPad4)
			{
				direction = MovementDirection.Left;
			}
			else if (key == Keys.Right || key == Keys.NumPad6)
			{
				direction = MovementDirection.Right;
			}
			else if (key == Keys.Up || key == Keys.NumPad8)
			{
				direction = MovementDirection.Up;
			}
			else if (key == Keys.Down || key == Keys.NumPad2)
			{
				direction = MovementDirection.Down;
			}
			else if (key == Keys.NumPad7)
			{
				direction = MovementDirection.UpLeft;
			}
			else if (key == Keys.NumPad9)
			{
				direction = MovementDirection.UpRight;
			}
			else if (key == Keys.NumPad1)
			{
				direction = MovementDirection.DownLeft;
			}
			else if (key == Keys.NumPad3)
			{
				direction = MovementDirection.DownRight;
			}

			var result = false;
			if (direction != null)
			{
				var delta = direction.Value.GetDelta();
				var newPos = TJ.Player.Position + delta;
				if (newPos.X >= 0 && newPos.X < TJ.Player.Map.Width && newPos.Y >= 0 && newPos.Y < TJ.Player.Map.Height)
				{
					var creature = (NonPlayer)TJ.Player.Map[newPos].Creature;
					if (creature != null && creature.Info.IsMerchant)
					{
						// Initiate trade
						var dialog = new TradeDialog(TJ.Session.Player, creature);
						dialog.ShowModal(Desktop);
					}
					else
					{
						var isRunning = _downKeys.Contains(Keys.LeftShift) || _downKeys.Contains(Keys.RightShift);
						result = TJ.Session.MovePlayer(direction.Value, isRunning);
					}
				}
			}

			return result;
		}

		private void KeyDownHandler(Keys key)
		{
			if (!Active)
			{
				return;
			}

			var acted = ProcessMovement(key);
			if (acted)
			{
			}
			else if (key == Keys.I)
			{
				var inventoryWindow = new InventoryWindow();
				inventoryWindow.ShowModal(Desktop);
			}
			else if (key == Keys.E)
			{
				if (TJ.Session.Player.Enter())
				{
					TJ.Session.UpdateTilesVisibility();
				}
			}
		}

		public override void InternalRender(RenderContext batch)
		{
			base.InternalRender(batch);

			UpdateKeyboardInput();
		}
	}
}