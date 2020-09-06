using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Utility;
using System;
using TroublesOfJord.Core;

namespace TroublesOfJord.UI
{
	public partial class CharacterWindow
	{
		public CharacterWindow()
		{
			BuildUI();

			var player = TJ.Player;
			_labelDescription.Text = string.Format("{0}, {1}, {2}",
				player.Name, player.Class.Name, player.Level);

			if (player.Level < TJ.Module.MaximumLevel)
			{
				var nextLevel = TJ.Module.LevelCosts[player.Level + 1];
				_labelExperience.Text = Experience(string.Format("{0}/{1}", 
					player.Experience.FormatNumber(), nextLevel.Experience.FormatNumber()));
				_labelGold.Text = Gold(string.Format("{0}/{1}",
					player.Gold.FormatNumber(), nextLevel.Gold.FormatNumber()));
			}
			else
			{
				_labelExperience.Text = Experience(player.Experience.FormatNumber());
				_labelGold.Text = Gold(player.Gold.FormatNumber());
			}

			_buttonConfirm.Click += _buttonConfirm_Click;
			_buttonReset.Click += _buttonReset_Click;

			ResetPoints();
		}

		private void _buttonConfirm_Click(object sender, System.EventArgs e)
		{
			var dialog = Dialog.CreateMessageBox(Strings.Confirm, Confirm);
			dialog.Closed += (s, a) =>
			{
				if (!dialog.Result)
				{
					return;
				}

				var player = TJ.Player;
				foreach (var widget in _gridClasses.Widgets)
				{
					var asSpinButton = widget as SpinButton;
					if (asSpinButton == null)
					{
						continue;
					}

					var cls = (Class)asSpinButton.Tag;
					player.SetClassLevel(cls.Id, (int)asSpinButton.Value);
				}

				UpdateEnabled();
			};

			dialog.ShowModal(Desktop);
		}

		private void _buttonReset_Click(object sender, System.EventArgs e)
		{
			ResetPoints();
		}

		private void ResetPoints()
		{
			_gridClasses.Widgets.Clear();

			var player = TJ.Player;

			var totalLevels = 0;
			var cnt = 0;
			foreach (var pair in TJ.Module.Classes)
			{
				var level = player.GetClassLevel(pair.Key);

				var label = new Label
				{
					Text = pair.Value.Name,
					GridRow = cnt
				};

				if (player.Class.Id == pair.Key)
				{
					label.TextColor = Color.Red;
				}

				_gridClasses.Widgets.Add(label);

				var spinButton = new SpinButton
				{
					Value = level,
					GridColumn = 1,
					GridRow = cnt,
					Width = 40,
					Minimum = level,
					Maximum = Config.MaximumClassLevel,
					Integer = true,
					Nullable = false,
					Tag = pair.Value
				};

				spinButton.TextBox.Readonly = true;

				spinButton.ValueChanging += SpinButton_ValueChanging;
				spinButton.ValueChanged += SpinButton_ValueChanged;

				_gridClasses.Widgets.Add(spinButton);

				totalLevels += level;
				++cnt;
			}

			UpdatePointsLeft();
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			var enabled = false;

			var player = TJ.Player;

			foreach (var widget in _gridClasses.Widgets)
			{
				var asSpinButton = widget as SpinButton;
				if (asSpinButton == null)
				{
					continue;
				}

				var cls = (Class)asSpinButton.Tag;
				if (player.GetClassLevel(cls.Id) < (int)asSpinButton.Value)
				{
					enabled = true;
					break;
				}
			}

			_buttonReset.Enabled = enabled;
			_buttonConfirm.Enabled = enabled;
		}

		private void UpdatePointsLeft()
		{
			_labelPointsLeft.Text = ClassPointsLeft(CalculatePointsLeft());
		}

		private void SpinButton_ValueChanged(object sender, ValueChangedEventArgs<float?> e)
		{
			UpdatePointsLeft();
			UpdateEnabled();
		}


		private int CalculatePointsLeft()
		{
			var player = TJ.Player;

			var totalLevels = 0;
			foreach (var widget in _gridClasses.Widgets)
			{
				var asSpinButton = widget as SpinButton;
				if (asSpinButton == null)
				{
					continue;
				}

				totalLevels += (int)asSpinButton.Value;
			}

			return Math.Max(0, player.Level - totalLevels);
		}

		private void SpinButton_ValueChanging(object sender, ValueChangingEventArgs<float?> e)
		{
			// Check that the new points sum doesnt exceed player level
			var player = TJ.Player;

			var totalLevels = 0;
			var spinButton = (SpinButton)sender;
			SpinButton spinButtonPrimary = null;
			foreach (var widget in _gridClasses.Widgets)
			{
				var asSpinButton = widget as SpinButton;
				if (asSpinButton == null)
				{
					continue;
				}

				if (asSpinButton != spinButton)
				{
					totalLevels += (int)asSpinButton.Value;
				} else
				{
					totalLevels += (int)e.NewValue;
				}

				if (player.Class.Id == ((Class)asSpinButton.Tag).Id)
				{
					spinButtonPrimary = asSpinButton;
				}
			}

			var pointsLeft = player.Level - totalLevels;
			if (pointsLeft < 0)
			{
				var error = Dialog.CreateMessageBox(Strings.Error, NoPointsLeft);
				error.ShowModal(Desktop);		
				e.Cancel = true;
				return;
			}

			// Now check that non-primary class level isnt higher than primary
			var isPrimary = player.Class.Id == ((Class)spinButton.Tag).Id;

			if (!isPrimary)
			{
				if ((int)e.NewValue > (int)spinButtonPrimary.Value)
				{
					var error = Dialog.CreateMessageBox(Strings.Error, NonPrimaryLevelHigher);
					error.ShowModal(Desktop);
					e.Cancel = true;
					return;
				}
			} else
			{
				foreach (var widget in _gridClasses.Widgets)
				{
					var asSpinButton = widget as SpinButton;
					if (asSpinButton == null || asSpinButton == spinButtonPrimary)
					{
						continue;
					}

					if ((int)e.NewValue < (int)asSpinButton.Value)
					{
						var error = Dialog.CreateMessageBox(Strings.Error, PrimaryLevelLower);
						error.ShowModal(Desktop);
						e.Cancel = true;
						return;
					}
				}
			}
		}
	}
}