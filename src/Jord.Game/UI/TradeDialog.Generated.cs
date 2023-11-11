/* Generated by MyraPad at 11/12/2023 5:37:24 AM */
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
	partial class TradeDialog: Dialog
	{
		private void BuildUI()
		{
			_textNameLeft = new Label();
			_textNameLeft.Text = "player";
			_textNameLeft.Id = "_textNameLeft";

			_textGoldLeft = new Label();
			_textGoldLeft.Text = "Gold: 1000";
			_textGoldLeft.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Right;
			_textGoldLeft.Id = "_textGoldLeft";

			var panel1 = new Panel();
			panel1.Widgets.Add(_textNameLeft);
			panel1.Widgets.Add(_textGoldLeft);

			var horizontalSeparator1 = new HorizontalSeparator();

			var label1 = new Label();
			label1.Text = "iron rations(5)";
			Grid.SetColumn(label1, 1);

			var label2 = new Label();
			label2.Text = "5";
			label2.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Right;
			Grid.SetColumn(label2, 2);

			var label3 = new Label();
			label3.Text = "canteen";
			Grid.SetColumn(label3, 1);
			Grid.SetRow(label3, 1);

			var label4 = new Label();
			label4.Text = "7";
			Grid.SetColumn(label4, 2);
			Grid.SetRow(label4, 1);

			_gridLeft = new Grid();
			_gridLeft.ColumnSpacing = 8;
			_gridLeft.RowSpacing = 4;
			_gridLeft.DefaultRowProportion = new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			};
			_gridLeft.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Pixels,
				Value = 16,
			});
			_gridLeft.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Fill,
			});
			_gridLeft.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			});
			_gridLeft.GridSelectionMode = Myra.Graphics2D.UI.GridSelectionMode.Row;
			_gridLeft.Id = "_gridLeft";
			_gridLeft.Widgets.Add(label1);
			_gridLeft.Widgets.Add(label2);
			_gridLeft.Widgets.Add(label3);
			_gridLeft.Widgets.Add(label4);

			var scrollViewer1 = new ScrollViewer();
			scrollViewer1.Content = _gridLeft;

			var verticalStackPanel1 = new VerticalStackPanel();
			StackPanel.SetProportionType(verticalStackPanel1, Myra.Graphics2D.UI.ProportionType.Part);
			verticalStackPanel1.Widgets.Add(panel1);
			verticalStackPanel1.Widgets.Add(horizontalSeparator1);
			verticalStackPanel1.Widgets.Add(scrollViewer1);

			_textGoldTransfer = new Label();
			_textGoldTransfer.Text = "500";
			_textGoldTransfer.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Center;
			_textGoldTransfer.Id = "_textGoldTransfer";

			_panelArrow = new Panel();
			_panelArrow.Height = 20;
			_panelArrow.Id = "_panelArrow";

			_gridGoldTransfer = new VerticalStackPanel();
			_gridGoldTransfer.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Center;
			_gridGoldTransfer.Id = "_gridGoldTransfer";
			StackPanel.SetProportionType(_gridGoldTransfer, Myra.Graphics2D.UI.ProportionType.Pixels);
			StackPanel.SetProportionValue(_gridGoldTransfer, 100);
			_gridGoldTransfer.Widgets.Add(_textGoldTransfer);
			_gridGoldTransfer.Widgets.Add(_panelArrow);

			_textNameRight = new Label();
			_textNameRight.Text = "merchant";
			_textNameRight.Id = "_textNameRight";

			_textGoldRight = new Label();
			_textGoldRight.Text = "Gold: 50000";
			_textGoldRight.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Right;
			_textGoldRight.Id = "_textGoldRight";

			var panel2 = new Panel();
			panel2.Widgets.Add(_textNameRight);
			panel2.Widgets.Add(_textGoldRight);

			var horizontalSeparator2 = new HorizontalSeparator();

			var label5 = new Label();
			label5.Text = "iron rations(100)";
			Grid.SetColumn(label5, 1);

			var label6 = new Label();
			label6.Text = "10";
			label6.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Right;
			Grid.SetColumn(label6, 2);

			var label7 = new Label();
			label7.Text = "canteen";
			Grid.SetColumn(label7, 1);
			Grid.SetRow(label7, 1);

			var label8 = new Label();
			label8.Text = "15";
			Grid.SetColumn(label8, 2);
			Grid.SetRow(label8, 1);

			var label9 = new Label();
			label9.Text = "barrel";
			Grid.SetColumn(label9, 1);
			Grid.SetRow(label9, 2);

			var label10 = new Label();
			label10.Text = "25";
			Grid.SetColumn(label10, 2);
			Grid.SetRow(label10, 2);

			var label11 = new Label();
			label11.Text = "short sword";
			Grid.SetColumn(label11, 1);
			Grid.SetRow(label11, 3);

			var label12 = new Label();
			label12.Text = "50";
			Grid.SetColumn(label12, 2);
			Grid.SetRow(label12, 3);

			var label13 = new Label();
			label13.Text = "leather armor";
			Grid.SetColumn(label13, 1);
			Grid.SetRow(label13, 4);

			var label14 = new Label();
			label14.Text = "40";
			Grid.SetColumn(label14, 2);
			Grid.SetRow(label14, 4);

			var label15 = new Label();
			label15.Text = "leather leggings";
			Grid.SetColumn(label15, 1);
			Grid.SetRow(label15, 5);

			var label16 = new Label();
			label16.Text = "30";
			Grid.SetColumn(label16, 2);
			Grid.SetRow(label16, 5);

			var label17 = new Label();
			label17.Text = "small shield";
			Grid.SetColumn(label17, 1);
			Grid.SetRow(label17, 6);

			var label18 = new Label();
			label18.Text = "25";
			Grid.SetColumn(label18, 2);
			Grid.SetRow(label18, 6);

			_gridRight = new Grid();
			_gridRight.ColumnSpacing = 8;
			_gridRight.RowSpacing = 4;
			_gridRight.DefaultRowProportion = new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			};
			_gridRight.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Pixels,
				Value = 16,
			});
			_gridRight.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Fill,
			});
			_gridRight.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			});
			_gridRight.GridSelectionMode = Myra.Graphics2D.UI.GridSelectionMode.Row;
			_gridRight.Id = "_gridRight";
			_gridRight.Widgets.Add(label5);
			_gridRight.Widgets.Add(label6);
			_gridRight.Widgets.Add(label7);
			_gridRight.Widgets.Add(label8);
			_gridRight.Widgets.Add(label9);
			_gridRight.Widgets.Add(label10);
			_gridRight.Widgets.Add(label11);
			_gridRight.Widgets.Add(label12);
			_gridRight.Widgets.Add(label13);
			_gridRight.Widgets.Add(label14);
			_gridRight.Widgets.Add(label15);
			_gridRight.Widgets.Add(label16);
			_gridRight.Widgets.Add(label17);
			_gridRight.Widgets.Add(label18);

			var scrollViewer2 = new ScrollViewer();
			scrollViewer2.Content = _gridRight;

			var verticalStackPanel2 = new VerticalStackPanel();
			StackPanel.SetProportionType(verticalStackPanel2, Myra.Graphics2D.UI.ProportionType.Part);
			verticalStackPanel2.Widgets.Add(panel2);
			verticalStackPanel2.Widgets.Add(horizontalSeparator2);
			verticalStackPanel2.Widgets.Add(scrollViewer2);

			var horizontalStackPanel1 = new HorizontalStackPanel();
			horizontalStackPanel1.Spacing = 8;
			StackPanel.SetProportionType(horizontalStackPanel1, Myra.Graphics2D.UI.ProportionType.Fill);
			horizontalStackPanel1.Widgets.Add(verticalStackPanel1);
			horizontalStackPanel1.Widgets.Add(_gridGoldTransfer);
			horizontalStackPanel1.Widgets.Add(verticalStackPanel2);

			var horizontalSeparator3 = new HorizontalSeparator();

			_textDescription = new Label();
			_textDescription.Text = "short sword - weapon, damage: 3-8";
			_textDescription.Id = "_textDescription";

			var verticalStackPanel3 = new VerticalStackPanel();
			verticalStackPanel3.Width = 800;
			verticalStackPanel3.Height = 400;
			verticalStackPanel3.Padding = new Thickness(0, 16);
			verticalStackPanel3.Widgets.Add(horizontalStackPanel1);
			verticalStackPanel3.Widgets.Add(horizontalSeparator3);
			verticalStackPanel3.Widgets.Add(_textDescription);

			
			Title = "Trade";
			Left = 553;
			Top = 238;
			Content = verticalStackPanel3;
		}

		
		public Label _textNameLeft;
		public Label _textGoldLeft;
		public Grid _gridLeft;
		public Label _textGoldTransfer;
		public Panel _panelArrow;
		public VerticalStackPanel _gridGoldTransfer;
		public Label _textNameRight;
		public Label _textGoldRight;
		public Grid _gridRight;
		public Label _textDescription;
	}
}
