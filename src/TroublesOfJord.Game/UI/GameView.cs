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

			_buttonUse.Click += (s, a) => TJ.Session.PlayerOperate();
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
					var creature = (NonPlayer)player.Map[newPos].Creature;
					var handled = true;
					if (creature != null)
					{
						switch (creature.Info.CreatureType)
						{
							case CreatureType.Merchant:
								// Initiate trade
								var dialog = new TradeDialog(player, creature);
								dialog.ShowModal(Desktop);
								break;
							case CreatureType.Instructor:
								Dialog messageBox;

								if (player.Level < TJ.Module.MaximumLevel)
								{
									var nextLevel = TJ.Module.LevelCosts[player.Level + 1];
									if (nextLevel.Experience <= player.Experience && nextLevel.Gold <= player.Gold)
									{
										var str = Strings.BuildNextLevelOffer(player.Experience, player.Gold,
											nextLevel.Experience, nextLevel.Gold);

										messageBox = Dialog.CreateMessageBox(Strings.InstructorCaption, str);
										messageBox.Closed += (s, a) =>
										{
											if (!messageBox.Result)
											{
												return;
											}

											// Level up
											player.Experience -= nextLevel.Experience;
											player.Gold -= nextLevel.Gold;
											player.Level++;

											TJ.GameLog(Strings.BuildNextLevel(player.Level, player.ClassPointsLeft));
										};
									}
									else
									{
										var str = Strings.BuildNextLevelRequirements(player.Experience, player.Gold,
											nextLevel.Experience, nextLevel.Gold);

										messageBox = Dialog.CreateMessageBox(Strings.InstructorCaption, str);
										messageBox.ButtonCancel.Visible = false;
									}
								}
								else
								{
									messageBox = Dialog.CreateMessageBox(Strings.InstructorCaption, Strings.ReachedMaximumLevel);
									messageBox.ButtonCancel.Visible = false;
								}

								messageBox.IsDraggable = false;
								messageBox.ShowModal(Desktop);

								break;
							default:
								handled = false;
								break;
						}
					}

					if (creature == null || !handled)
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
			} else if (key == Keys.E)
			{
				TJ.Session.PlayerOperate();
			}

			UpdateUseButton();
		}

		private void UpdateUseButton()
		{
			var type = TJ.Session.GetPlayerOperate();

			if (type == null)
			{
				_buttonUse.Text = @"\c[green]E\c[white]|Use";
				_buttonUse.Enabled = false;
			} else
			{
				_buttonUse.Text = @"\c[green]E\c[white]|Enter";
				_buttonUse.Enabled = true;
			}
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