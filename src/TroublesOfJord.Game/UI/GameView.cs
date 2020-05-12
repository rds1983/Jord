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

		private void ProcessInput()
		{
			if (!Active || _delayStarted != null)
			{
				return;
			}

			var keys = Keyboard.GetState();

			if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.NumPad4))
			{
				TJ.Player.MoveTo(new Point(-1, 0));
			}
			else if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.NumPad6))
			{
				TJ.Player.MoveTo(new Point(1, 0));
			}
			else if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.NumPad8))
			{
				TJ.Player.MoveTo(new Point(0, -1));
			}
			else if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.NumPad2))
			{
				TJ.Player.MoveTo(new Point(0, 1));
			}
			else if (keys.IsKeyDown(Keys.NumPad7))
			{
				TJ.Player.MoveTo(new Point(-1, -1));
			}
			else if (keys.IsKeyDown(Keys.NumPad9))
			{
				TJ.Player.MoveTo(new Point(1, -1));
			}
			else if (keys.IsKeyDown(Keys.NumPad1))
			{
				TJ.Player.MoveTo(new Point(-1, 1));
			}
			else if (keys.IsKeyDown(Keys.NumPad3))
			{
				TJ.Player.MoveTo(new Point(1, 1));
			}
			else if (keys.IsKeyDown(Keys.I))
			{
				var inventoryWindow = new InventoryWindow();
				inventoryWindow.ShowModal();
			}
			else if (keys.IsKeyDown(Keys.E))
			{
				if (TJ.Session.Player.Enter())
				{
					MapNavigation.InvalidateImage();
				}
			}

			var isRunning = keys.IsKeyDown(Keys.LeftShift) || keys.IsKeyDown(Keys.RightShift);
			if (!isRunning)
			{
				_delayStarted = DateTime.Now;
				_delayInMs = Config.TurnDelayInMs;
			}
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