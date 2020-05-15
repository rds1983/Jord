using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Extended.Widgets;
using Myra.Graphics2D.UI;
using System;
using TroublesOfJord.Core;

namespace TroublesOfJord.UI
{
	public partial class GameView
	{
		private DateTime? _delayStarted;
		private int _delayInMs = 0;
		private KeyboardState _lastKeys;
		public MapView MapView { get; } = new MapView();
		public MiniMap MapNavigation { get; } = new MiniMap();
		public LogView LogView { get; } = new LogView();
		public readonly SkillWidget[] Skills = new SkillWidget[10];


		protected override bool AcceptsKeyboardFocus => true;

		public GameView()
		{
			BuildUI();

			_skillsContainer.Widgets.Add(new VerticalSeparator());
			for (var i = 0; i < Skills.Length; ++i)
			{
				var skillWidget = new SkillWidget
				{
					Width = 50
				};
				skillWidget._labelIndex.Text = i < Skills.Length - 1 ? (i + 1).ToString() : 0.ToString();

				skillWidget._checkBoxAuto.Visible = false;
				skillWidget._buttonSkill.Visible = false;

				skillWidget._buttonSkill.TouchDown += _buttonSkill_TouchDown;

				Skills[i] = skillWidget;
				
				_skillsContainer.Widgets.Add(Skills[i]);
				_skillsContainer.Widgets.Add(new VerticalSeparator());
			}

			_mapViewContainer.Widgets.Add(MapView);

			MapNavigation.MapEditor = MapView;
			_mapContainer.Widgets.Add(MapNavigation);

			LogView.ShowVerticalScrollBar = false;
			_logContainer.Widgets.Add(LogView);

			UpdateSkills();
		}

		private void _buttonSkill_TouchDown(object sender, EventArgs e)
		{
			var widget = (Widget)sender;
			var ability = (IUsableAbility)widget.Tag;

			ability.Use();
		}

		private void UpdateSkills()
		{
			var usableAbilities = TJ.Player.UsableAbilities;

			var i = 0;
			for (; i < Math.Min(usableAbilities.Length, Skills.Length); ++i)
			{
				var ability = usableAbilities[i];
				var skillWidget = Skills[i];
				skillWidget._checkBoxAuto.Visible = ability.CanAuto;
				skillWidget._buttonSkill.Text = ability.Name;
				skillWidget._buttonSkill.Visible = true;
				skillWidget.Tag = ability;
			}

			for (; i < Skills.Length; ++i)
			{
				var skillWidget = Skills[i];
				skillWidget._checkBoxAuto.Visible = false;
				skillWidget._buttonSkill.Visible = false;
			}
		}

		private bool ProcessMovement(ref KeyboardState keys)
		{
			var isMovement = true;

			var delta = Point.Zero;
			if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.NumPad4))
			{
				delta = new Point(-1, 0);
			}
			else if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.NumPad6))
			{
				delta = new Point(1, 0);
			}
			else if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.NumPad8))
			{
				delta = new Point(0, -1);
			}
			else if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.NumPad2))
			{
				delta = new Point(0, 1);
			}
			else if (keys.IsKeyDown(Keys.NumPad7))
			{
				delta = new Point(-1, -1);
			}
			else if (keys.IsKeyDown(Keys.NumPad9))
			{
				delta = new Point(1, -1);
			}
			else if (keys.IsKeyDown(Keys.NumPad1))
			{
				delta = new Point(-1, 1);
			}
			else if (keys.IsKeyDown(Keys.NumPad3))
			{
				delta = new Point(1, 1);
			} else
			{
				isMovement = false;
			}

			var result = false;
			if (isMovement)
			{
				var newPos = TJ.Player.Position + delta;
				if (newPos.X >= 0 && newPos.X < TJ.Player.Map.Size.X && newPos.Y >= 0 && newPos.Y < TJ.Player.Map.Size.Y)
				{
					var creature = (NonPlayer)TJ.Player.Map[newPos].Creature;
					if (creature != null && creature.Info.IsMerchant)
					{
						// Initiate trade
						var dialog = new TradeDialog(TJ.Session.Player, creature);
						dialog.ShowModal();
					}
					else
					{
						result = TJ.Player.MoveTo(delta);
					}

				}
			}

			return result;
		}

		private bool IsKeyPressed(ref KeyboardState keys, Keys key)
		{
			return keys.IsKeyDown(key) && !_lastKeys.IsKeyDown(key);
		}

		private void ProcessInput()
		{
			if (!Active || _delayStarted != null)
			{
				return;
			}

			var keys = Keyboard.GetState();

			var acted = ProcessMovement(ref keys);
			if (acted)
			{
			}
			else if (IsKeyPressed(ref keys, Keys.I))
			{
				var inventoryWindow = new InventoryWindow();
				inventoryWindow.ShowModal();
			}
			else if (IsKeyPressed(ref keys, Keys.E))
			{
				if (TJ.Session.Player.Enter())
				{
					MapNavigation.InvalidateImage();
				}
			}

			if (acted)
			{
				var isRunning = keys.IsKeyDown(Keys.LeftShift) || keys.IsKeyDown(Keys.RightShift);
				_delayStarted = DateTime.Now;
				if (!isRunning)
				{
					_delayInMs = Config.TurnDelayInMs;
				} else
				{
					_delayInMs = Config.TurnDelayInMs / 2;
				}

				// Let npcs act
				var map = TJ.Player.Map;
				for (var x = 0; x < map.Size.X; ++x)
				{
					for (var y = 0; y < map.Size.Y; ++y)
					{
						var npc = map[x, y].Creature as NonPlayer;
						if (npc == null)
						{
							continue;
						}

						npc.Act();
					}
				}
			}

			_lastKeys = keys;
		}

		public override void InternalRender(RenderContext batch)
		{
			base.InternalRender(batch);

			ProcessInput();

			if (_delayStarted != null)
			{
				var passed = (DateTime.Now - _delayStarted.Value).TotalMilliseconds;
				if (passed >= _delayInMs)
				{
					_delayStarted = null;
				}
			}
		}
	}
}