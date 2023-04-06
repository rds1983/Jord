using Microsoft.Xna.Framework.Input;
using Myra.Extended.Widgets;
using Myra.Graphics2D.UI;
using System;
using System.Linq;
using Jord.Core;
using Jord.Utils;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Jord.Core.Abilities;

namespace Jord.UI
{
	public partial class GameView
	{
		private const int RepeatKeyDownStartInMs = 400;
		private const int RepeatKeyDownInternalInMs = 40;

		private DateTime? _lastKeyDown;
		private int _keyDownCount = 0;
		private Keys[] _downKeys, _lastDownKeys;

		public MapView MapView { get; } = new MapView();
		public MiniMap MapNavigation { get; } = new MiniMap();
		public LogView LogView { get; } = new LogView();

		public GameView()
		{
			AcceptsKeyboardFocus = true;

			BuildUI();

			_buttonUse.Click += _buttonUse_Click;
			_buttonAbilities.Click += (s, a) => ShowAbilities();
			_buttonCharacter.Click += (s, a) => ShowCharacter();
			_buttonInventory.Click += (s, a) => ShowInventory();

			_mapViewContainer.Widgets.Add(MapView);

			MapNavigation.MapEditor = MapView;
			_mapContainer.Widgets.Add(MapNavigation);

			LogView.ShowVerticalScrollBar = false;
			_logContainer.Widgets.Add(LogView);

			UpdateUseButton();
		}

		private void UpdateUseButton()
		{
			var player = TJ.Player;
			var enabled = true;

			string text;
			if (player.Tile != null && player.Tile.Inventory.Items.Count > 0)
			{
				text = @"/c[green]E/c[white]|Take";
			}
			else if (player.CanEnter())
			{
				text = @"/c[green]E/c[white]|Enter";
			}
			else
			{
				text = @"/c[green]E/c[white]|Use";
				enabled = false;
			}

			_buttonUse.Text = text;
			_buttonUse.Enabled = enabled;
		}

		private void _buttonUse_Click(object sender, EventArgs e)
		{
			var player = TJ.Player;

			if (player.Tile != null && player.Tile.Inventory.Items.Count > 0)
			{
				player.TakeLyingItem(0, 1);
			}
			else if (player.CanEnter())
			{
				TJ.Session.PlayerEnter();
			}
		}

		private void UpdateKeyboardInput()
		{
			if (TJ.ActivityService.IsBusy)
			{
				return;
			}

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

		private bool ProcessMovementTileObject(Point newPos)
		{
			var player = TJ.Player;

			if (player.Map[newPos].Object == null)
			{
				return false;
			}

			switch (player.Map[newPos].Object.Type)
			{
				case TileObjectType.TanningBench:
					{
						Window window;
						if (player.CalculateBonus(BonusType.WorkWithLeather) == 0)
						{
							window = Dialog.CreateMessageBox(Strings.Error, CantWorkWithLeather);
						}
						else
						{
							window = new TanningWindow();
						}

						window.ShowModal(Desktop);
					}
					break;
				case TileObjectType.CraftingBench:
					{
						Window window;
						if (player.CalculateBonus(BonusType.WorkWithLeather) == 0)
						{
							window = Dialog.CreateMessageBox(Strings.Error, CantForgeThings);
						}
						else
						{
							window = new CraftingWindow();
						}

						window.ShowModal(Desktop);
					}
					
					break;
			}

			return true;
		}

		private bool ProcessMovementCreature(Point newPos)
		{
			var player = TJ.Player;
			var asNpc = player.Map[newPos].Creature as NonPlayer;
			if (asNpc == null)
			{
				return false;
			}

			var handled = true;
			switch (asNpc.Info.CreatureType)
			{
				case CreatureType.Merchant:
					// Initiate trade
					var dialog = new TradeDialog(asNpc);
					dialog.ShowModal(Desktop);
					break;

				default:
					handled = false;
					break;
			}

			return handled;
		}

		private bool ProcessMovement(Keys key)
		{
			if (key == Keys.NumPad5)
			{
				// Wait
				TJ.Session.WaitPlayer();
				return true;
			}

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
				var player = TJ.Player;
				var newPos = player.Position + delta;
				if (newPos.X >= 0 && newPos.X < player.Map.Width &&
					newPos.Y >= 0 && newPos.Y < player.Map.Height)
				{
					var handled = ProcessMovementTileObject(newPos);

					if (!handled)
					{
						handled = ProcessMovementCreature(newPos);
					}

					if (!handled)
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
				ShowInventory();
			}
			else if (key == Keys.C)
			{
				ShowCharacter();
			}
			else if (key == Keys.A)
			{
				ShowAbilities();
			} else if (key == Keys.E && _buttonUse.Enabled)
			{
				_buttonUse.DoClick();
			}

			UpdateUseButton();
		}

		private void ShowInventory()
		{
			var inventoryWindow = new InventoryWindow();
			inventoryWindow.ShowModal(Desktop);
		}

		private void ShowCharacter()
		{
			var characterWindow = new CharacterWindow();
			characterWindow.ShowModal(Desktop);
		}

		private void ShowAbilities()
		{
			var abilitiesWindow = new AbilitiesWindow();
			abilitiesWindow.ShowModal(Desktop);
		}

		public override void InternalRender(RenderContext batch)
		{
			base.InternalRender(batch);

			UpdateKeyboardInput();
		}
	}
}