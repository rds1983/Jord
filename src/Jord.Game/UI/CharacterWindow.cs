using Jord.Core.Abilities;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using System.Collections.Generic;

namespace Jord.UI
{
	public partial class CharacterWindow
	{
		private class WidgetPerk
		{
			public TextButton Widget;
			public Perk Perk;
		}

		private class Connection
		{
			public TextButton Widget { get; }
			public TextButton Widget2 { get; }

			public Connection(TextButton widget, TextButton widget2)
			{
				Widget = widget;
				Widget2 = widget2;
			}
		}

		public CharacterWindow()
		{
			BuildUI();

			var player = TJ.Player;

			var nextLevel = TJ.Database.LevelCosts[player.Level + 1];
			var title = $"{player.Name}, {player.Class}, {player.Level}, {player.Experience.FormatNumber()}/{nextLevel.Experience.FormatNumber()}";
			Title = title;

			_tabControlMain.Items.Clear();

			var perksOrderedByCategory = new Dictionary<string, List<Perk>>();
			foreach (var perk in TJ.Database.Perks)
			{
				List<Perk> categoryPerks;
				if (!perksOrderedByCategory.TryGetValue(perk.Value.Category, out categoryPerks))
				{
					categoryPerks = new List<Perk>();
					perksOrderedByCategory[perk.Value.Category] = categoryPerks;
				}

				categoryPerks.Add(perk.Value);
			}

			foreach (var pair in perksOrderedByCategory)
			{
				BuildCategoryTab(pair.Key, pair.Value);
			}
		}

		private void BuildCategoryTab(string category, List<Perk> perks)
		{
			var orderedPerks = new Dictionary<int, List<Perk>>();
			foreach (var perk in perks)
			{
				List<Perk> tierPerks;
				if (!orderedPerks.TryGetValue(perk.Tier, out tierPerks))
				{
					tierPerks = new List<Perk>();
					orderedPerks[perk.Tier] = tierPerks;
				}

				tierPerks.Add(perk);
			}

			var topPanel = new VerticalStackPanel
			{
				Spacing = 8,
				HorizontalAlignment = HorizontalAlignment.Center
			};

			var widgetPerks = new List<WidgetPerk>();
			var connections = new List<Connection>();
			foreach (var pair in orderedPerks)
			{
				if (topPanel.Widgets.Count > 0)
				{
					// Add spacing
					var spacePanel = new Panel
					{
						Height = 24
					};

					topPanel.Widgets.Add(spacePanel);
				}

				var tierPanel = new HorizontalStackPanel
				{
					Spacing = 8,
					HorizontalAlignment = HorizontalAlignment.Center
				};


				foreach (var perk in pair.Value)
				{
					var buttonPerk = new TextButton
					{
						Text = perk.Name,
						Toggleable = true,
						Tag = perk
					};

					tierPanel.Widgets.Add(buttonPerk);

					if (perk.RequiresPerks != null && perk.RequiresPerks.Length > 0)
					{
						var parentPerk = perk.RequiresPerks[0];

						foreach (var widgetPerk in widgetPerks)
						{
							if (parentPerk == widgetPerk.Perk)
							{
								connections.Add(new Connection(widgetPerk.Widget, buttonPerk));
							}
						}
					}

					widgetPerks.Add(new WidgetPerk
					{
						Widget = buttonPerk,
						Perk = perk
					});
				}

				topPanel.Widgets.Add(tierPanel);
			}

			var tabItem = new TabItem
			{
				Text = category,
				Content = topPanel,
				Tag = connections.ToArray()
			};

			_tabControlMain.Items.Add(tabItem);
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			var tabItem = _tabControlMain.SelectedItem;

			var connections = (Connection[])tabItem.Tag;
			foreach (var connection in connections)
			{
				var start = new Vector2(connection.Widget.Bounds.X + connection.Widget.Bounds.Width / 2, connection.Widget.Bounds.Y + connection.Widget.Bounds.Height);
				start = ToLocal(connection.Widget.ToGlobal(start));

				var end = new Vector2(connection.Widget2.Bounds.X + connection.Widget2.Bounds.Width / 2, connection.Widget2.Bounds.Y);
				end = ToLocal(connection.Widget2.ToGlobal(end));

				context.DrawLine(start, end, Color.Green, 2);
			}
		}
	}
}