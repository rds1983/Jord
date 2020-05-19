using Microsoft.Xna.Framework.Input;
using Myra.Extended.Widgets;
using Myra.Graphics2D.UI;
using System;
using TroublesOfJord.Core;
using TroublesOfJord.Utils;

namespace TroublesOfJord.UI
{
	public partial class GameView
	{
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
			MovementDirection? direction = null;

			if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.NumPad4))
			{
				direction = MovementDirection.Left;
			}
			else if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.NumPad6))
			{
				direction = MovementDirection.Right;
			}
			else if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.NumPad8))
			{
				direction = MovementDirection.Up;
			}
			else if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.NumPad2))
			{
				direction = MovementDirection.Down;
			}
			else if (keys.IsKeyDown(Keys.NumPad7))
			{
				direction = MovementDirection.UpLeft;
			}
			else if (keys.IsKeyDown(Keys.NumPad9))
			{
				direction = MovementDirection.UpRight;
			}
			else if (keys.IsKeyDown(Keys.NumPad1))
			{
				direction = MovementDirection.DownLeft;
			}
			else if (keys.IsKeyDown(Keys.NumPad3))
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
						dialog.ShowModal();
					}
					else
					{
						var isRunning = keys.IsKeyDown(Keys.LeftShift) || keys.IsKeyDown(Keys.RightShift);
						result = TJ.Session.MovePlayer(direction.Value, isRunning);
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
			if (!Active || !TJ.Session.AcceptsInput)
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
					TJ.Session.UpdateTilesVisibility();
				}
			}

			_lastKeys = keys;
		}

		public override void InternalRender(RenderContext batch)
		{
			base.InternalRender(batch);

			ProcessInput();
			TJ.Session.Update();
		}
	}
}