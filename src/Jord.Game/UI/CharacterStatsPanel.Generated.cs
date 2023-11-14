/* Generated by MyraPad at 11/13/2023 8:15:04 AM */
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Properties;
using FontStashSharp.RichText;
using AssetManagementBase;

#if STRIDE
using Stride.Core.Mathematics;
#elif PLATFORM_AGNOSTIC
using System.Drawing;
using System.Numerics;
using Color = FontStashSharp.FSColor;
#else
// MonoGame/FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace Jord.UI
{
	partial class CharacterStatsPanel: Grid
	{
		private void BuildUI()
		{
			var label1 = new Label();
			label1.Text = "Melee Mastery:";

			_labelMeleeMastery = new Label();
			_labelMeleeMastery.Text = "30";
			_labelMeleeMastery.Id = "_labelMeleeMastery";
			Grid.SetColumn(_labelMeleeMastery, 1);

			var label2 = new Label();
			label2.Text = "Armor Class:";
			Grid.SetRow(label2, 1);

			_labelArmorRating = new Label();
			_labelArmorRating.Text = "30";
			_labelArmorRating.Id = "_labelArmorRating";
			Grid.SetColumn(_labelArmorRating, 1);
			Grid.SetRow(_labelArmorRating, 1);

			var label3 = new Label();
			label3.Text = "Evasion Rating:";
			Grid.SetRow(label3, 2);

			_labelEvasionRating = new Label();
			_labelEvasionRating.Text = "30";
			_labelEvasionRating.Id = "_labelEvasionRating";
			Grid.SetColumn(_labelEvasionRating, 1);
			Grid.SetRow(_labelEvasionRating, 2);

			var label4 = new Label();
			label4.Text = "Blocking Rating:";
			Grid.SetRow(label4, 3);

			_labelBlockingRating = new Label();
			_labelBlockingRating.Text = "30";
			_labelBlockingRating.Id = "_labelBlockingRating";
			Grid.SetColumn(_labelBlockingRating, 1);
			Grid.SetRow(_labelBlockingRating, 3);

			
			ColumnSpacing = 8;
			RowSpacing = 8;
			DefaultColumnProportion = new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			};
			DefaultRowProportion = new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			};
			Widgets.Add(label1);
			Widgets.Add(_labelMeleeMastery);
			Widgets.Add(label2);
			Widgets.Add(_labelArmorRating);
			Widgets.Add(label3);
			Widgets.Add(_labelEvasionRating);
			Widgets.Add(label4);
			Widgets.Add(_labelBlockingRating);
		}

		
		public Label _labelMeleeMastery;
		public Label _labelArmorRating;
		public Label _labelEvasionRating;
		public Label _labelBlockingRating;
	}
}