using Microsoft.Xna.Framework.Input;
using Myra.Extended.Widgets;
using Myra.Graphics2D.UI;
using System;
using TroublesOfJord.Core;

namespace TroublesOfJord.UI
{
	public partial class GameView
	{
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

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (k == Keys.I)
			{
				var inventoryWindow = new InventoryWindow();
				inventoryWindow.ShowModal();
			}

			if (k == Keys.E)
			{
				if (TJ.Session.Player.Enter())
				{
					MapNavigation.InvalidateImage();
				}
			}
		}
	}
}