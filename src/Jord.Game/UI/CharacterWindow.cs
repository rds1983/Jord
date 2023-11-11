using Jord.Core.Abilities;
using Jord.Utils;
using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jord.UI
{
	public partial class CharacterWindow
	{
		private class Connection
		{
			public ToggleButton Widget { get; }
			public ToggleButton Widget2 { get; }

			public Connection(ToggleButton widget, ToggleButton widget2)
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

			UpdateLabelPerkPoints();
		}

		private void BuildCategoryTab(string category, List<Perk> perks)
		{
			var topPanel = new Grid
			{
				ColumnSpacing = 8,
				RowSpacing = 8,
				DefaultColumnProportion = Proportion.Auto,
				HorizontalAlignment = HorizontalAlignment.Center,
				BeforeRender = RenderConnections
			};

			foreach (var perk in perks)
			{
				var buttonPerk = new ToggleButton
				{
					Content = new Label
					{
						Text = perk.Name,
					},
					HorizontalAlignment = HorizontalAlignment.Center,
					Tag = perk,
					Tooltip = perk.Description
				};

				buttonPerk.PressedChangingByUser += ButtonHandler;

				if (TJ.Player.Perks.Contains(perk))
				{
					buttonPerk.IsPressed = true;
				}

				Grid.SetColumn(buttonPerk, perk.Position.X);
				Grid.SetRow(buttonPerk, perk.Position.Y);

				topPanel.Widgets.Add(buttonPerk);
			}

			// Second run: set connections
			var connections = new List<Connection>();
			foreach (ToggleButton widget in topPanel.Widgets)
			{
				var perk = (Perk)widget.Tag;
				if (perk.RequiresPerks == null || perk.RequiresPerks.Length == 0)
				{
					continue;
				}

				var parentPerk = perk.RequiresPerks[0];

				var widget2 = (from ToggleButton w in topPanel.Widgets where (Perk)w.Tag == parentPerk select w).FirstOrDefault();
				if (widget2 != null)
				{
					connections.Add(new Connection(widget2, widget));
				}
			}

			var tabItem = new TabItem
			{
				Text = category,
				Content = topPanel,
				Tag = connections.ToArray()
			};

			_tabControlMain.Items.Add(tabItem);
		}

		private void ButtonHandler(object sender, ValueChangingEventArgs<bool> args)
		{
			var buttonPerk = (ToggleButton)sender;
			if (buttonPerk.IsPressed)
			{
				// The perk was already taken
				args.Cancel = true;
				return;
			}

			if (TJ.Player.PerkPointsLeft == 0)
			{
				// No perk points left
				var message = Dialog.CreateMessageBox("Take Perk", "No perk points left");
				message.ShowModal(Desktop);

				args.Cancel = true;
				return;
			}

			var perk = (Perk)buttonPerk.Tag;

			var confirmationDialog = Dialog.CreateMessageBox("Take Perk", $"Are you sure you want to spend a perk point for '{perk.Name}'?");
			confirmationDialog.ShowModal(Desktop);

			confirmationDialog.Closed += (s, a) =>
			{
				if (!confirmationDialog.Result)
				{
					return;
				}

				TJ.Player.Perks.Add(perk);
				buttonPerk.IsPressed = true;
				UpdateLabelPerkPoints();
			};

			args.Cancel = true;
		}

		private void RenderConnections(RenderContext context)
		{
			var tabItem = _tabControlMain.SelectedItem;
			var panel = (Grid)tabItem.Content;

			var connections = (Connection[])tabItem.Tag;
			foreach (var connection in connections)
			{
				var start = panel.ToLocal(connection.Widget.ToGlobal(connection.Widget.Bounds.Center.ToVector2()));
				var end = panel.ToLocal(connection.Widget2.ToGlobal(connection.Widget2.Bounds.Center.ToVector2()));

				var delta = end - start;

				// Forbid minor rotates
				if (Math.Abs(delta.X) < 5)
				{
					end.X = start.X;
				}

				if (Math.Abs(delta.Y) < 5)
				{
					end.Y = start.Y;
				}

				context.DrawLine(start, end, Color.Green, 2);
			}
		}

		private void UpdateLabelPerkPoints()
		{
			_labelPerkPoints.Text = $"Perk Points Left: {TJ.Player.PerkPointsLeft}";
		}
	}
}