/* Generated by MyraPad at 10/7/2022 10:38:50 AM */
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Properties;
using FontStashSharp.RichText;

namespace Jord.UI
{
	partial class InventoryWindow: Window
	{
		private void BuildUI()
		{
			var label1 = new Label();
			label1.Text = "Equipment";

			var horizontalSeparator1 = new HorizontalSeparator();

			var label2 = new Label();
			label2.Text = "<light>";

			var label3 = new Label();
			label3.Text = "eye of the deep";
			label3.GridColumn = 1;

			var label4 = new Label();
			label4.Text = "<left finger>";
			label4.GridRow = 1;

			var label5 = new Label();
			label5.Text = "a carved wooden ring";
			label5.GridColumn = 1;
			label5.GridRow = 1;

			var label6 = new Label();
			label6.Text = "<right finger>";
			label6.GridRow = 2;

			var label7 = new Label();
			label7.Text = "a carved wooden ring";
			label7.GridColumn = 1;
			label7.GridRow = 2;

			var label8 = new Label();
			label8.Text = "<neck 1>";
			label8.GridRow = 3;

			var label9 = new Label();
			label9.Text = "the amulet of the sanguineous magi";
			label9.GridColumn = 1;
			label9.GridRow = 3;

			var label10 = new Label();
			label10.Text = "<neck 2>";
			label10.GridRow = 4;

			var label11 = new Label();
			label11.Text = "the amulet of the sanguineous magi";
			label11.GridColumn = 1;
			label11.GridRow = 4;

			var label12 = new Label();
			label12.Text = "<body>";
			label12.GridRow = 5;

			var label13 = new Label();
			label13.Text = "a gilded corset of crimson and gold";
			label13.GridColumn = 1;
			label13.GridRow = 5;

			var label14 = new Label();
			label14.Text = "<head>";
			label14.GridRow = 6;

			var label15 = new Label();
			label15.Text = "The Crown of Thorns";
			label15.GridColumn = 1;
			label15.GridRow = 6;

			var label16 = new Label();
			label16.Text = "<legs>";
			label16.GridRow = 7;

			var label17 = new Label();
			label17.Text = "Ogre Hide Leggings";
			label17.GridColumn = 1;
			label17.GridRow = 7;

			var label18 = new Label();
			label18.Text = "<feet>";
			label18.GridRow = 8;

			var label19 = new Label();
			label19.Text = "a pair of dolphin skin boots";
			label19.GridColumn = 1;
			label19.GridRow = 8;

			var label20 = new Label();
			label20.Text = "<hands>";
			label20.GridRow = 9;

			var label21 = new Label();
			label21.Text = "gloves of the yellow rose";
			label21.GridColumn = 1;
			label21.GridRow = 9;

			var label22 = new Label();
			label22.Text = "<arms>";
			label22.GridRow = 10;

			var label23 = new Label();
			label23.Text = "a set of tidal sleeves";
			label23.GridColumn = 1;
			label23.GridRow = 10;

			var label24 = new Label();
			label24.Text = "<shield>";
			label24.GridRow = 11;

			var label25 = new Label();
			label25.Text = "an intricately decorated shield";
			label25.GridColumn = 1;
			label25.GridRow = 11;

			var label26 = new Label();
			label26.Text = "<about body>";
			label26.GridRow = 12;

			var label27 = new Label();
			label27.Text = "a dragonwing tunic";
			label27.GridColumn = 1;
			label27.GridRow = 12;

			var label28 = new Label();
			label28.Text = "<waist>";
			label28.GridRow = 13;

			var label29 = new Label();
			label29.Text = "a golden belt";
			label29.GridColumn = 1;
			label29.GridRow = 13;

			var label30 = new Label();
			label30.Text = "<left wrist>";
			label30.GridRow = 14;

			var label31 = new Label();
			label31.Text = "tanzanite bracelet";
			label31.GridColumn = 1;
			label31.GridRow = 14;

			var label32 = new Label();
			label32.Text = "<right wrist>";
			label32.GridRow = 15;

			var label33 = new Label();
			label33.Text = "tanzanite bracelet";
			label33.GridColumn = 1;
			label33.GridRow = 15;

			var label34 = new Label();
			label34.Text = "<wielded>";
			label34.GridRow = 16;

			var label35 = new Label();
			label35.Text = "a secari dagger";
			label35.GridColumn = 1;
			label35.GridRow = 16;

			var label36 = new Label();
			label36.Text = "<held>";
			label36.GridRow = 17;

			var label37 = new Label();
			label37.Text = "a moss-covered pebble";
			label37.GridColumn = 1;
			label37.GridRow = 17;

			_gridEquipment = new Grid();
			_gridEquipment.ColumnSpacing = 8;
			_gridEquipment.RowSpacing = 4;
			_gridEquipment.DefaultColumnProportion = new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			};
			_gridEquipment.DefaultRowProportion = new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			};
			_gridEquipment.GridSelectionMode = Myra.Graphics2D.UI.GridSelectionMode.Row;
			_gridEquipment.GridRow = 1;
			_gridEquipment.Id = "_gridEquipment";
			_gridEquipment.Widgets.Add(label2);
			_gridEquipment.Widgets.Add(label3);
			_gridEquipment.Widgets.Add(label4);
			_gridEquipment.Widgets.Add(label5);
			_gridEquipment.Widgets.Add(label6);
			_gridEquipment.Widgets.Add(label7);
			_gridEquipment.Widgets.Add(label8);
			_gridEquipment.Widgets.Add(label9);
			_gridEquipment.Widgets.Add(label10);
			_gridEquipment.Widgets.Add(label11);
			_gridEquipment.Widgets.Add(label12);
			_gridEquipment.Widgets.Add(label13);
			_gridEquipment.Widgets.Add(label14);
			_gridEquipment.Widgets.Add(label15);
			_gridEquipment.Widgets.Add(label16);
			_gridEquipment.Widgets.Add(label17);
			_gridEquipment.Widgets.Add(label18);
			_gridEquipment.Widgets.Add(label19);
			_gridEquipment.Widgets.Add(label20);
			_gridEquipment.Widgets.Add(label21);
			_gridEquipment.Widgets.Add(label22);
			_gridEquipment.Widgets.Add(label23);
			_gridEquipment.Widgets.Add(label24);
			_gridEquipment.Widgets.Add(label25);
			_gridEquipment.Widgets.Add(label26);
			_gridEquipment.Widgets.Add(label27);
			_gridEquipment.Widgets.Add(label28);
			_gridEquipment.Widgets.Add(label29);
			_gridEquipment.Widgets.Add(label30);
			_gridEquipment.Widgets.Add(label31);
			_gridEquipment.Widgets.Add(label32);
			_gridEquipment.Widgets.Add(label33);
			_gridEquipment.Widgets.Add(label34);
			_gridEquipment.Widgets.Add(label35);
			_gridEquipment.Widgets.Add(label36);
			_gridEquipment.Widgets.Add(label37);

			var scrollViewer1 = new ScrollViewer();
			scrollViewer1.Content = _gridEquipment;

			var verticalStackPanel1 = new VerticalStackPanel();
			verticalStackPanel1.Widgets.Add(label1);
			verticalStackPanel1.Widgets.Add(horizontalSeparator1);
			verticalStackPanel1.Widgets.Add(scrollViewer1);

			var verticalSeparator1 = new VerticalSeparator();

			var label38 = new Label();
			label38.Text = "Inventory";

			_textGold = new Label();
			_textGold.Text = "Gold: 50000";
			_textGold.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Right;
			_textGold.Id = "_textGold";

			var panel1 = new Panel();
			panel1.Widgets.Add(label38);
			panel1.Widgets.Add(_textGold);

			var horizontalSeparator2 = new HorizontalSeparator();

			var label39 = new Label();
			label39.Text = "iron rations(100)";

			var label40 = new Label();
			label40.Text = "canteen";
			label40.GridRow = 1;

			var label41 = new Label();
			label41.Text = "barrel";
			label41.GridRow = 2;

			var label42 = new Label();
			label42.Text = "short sword";
			label42.GridRow = 3;

			var label43 = new Label();
			label43.Text = "leather armor";
			label43.GridRow = 4;

			var label44 = new Label();
			label44.Text = "leather leggings";
			label44.GridRow = 5;

			var label45 = new Label();
			label45.Text = "small shield";
			label45.GridRow = 6;

			_gridInventory = new Grid();
			_gridInventory.ColumnSpacing = 8;
			_gridInventory.RowSpacing = 4;
			_gridInventory.DefaultRowProportion = new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			};
			_gridInventory.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Fill,
			});
			_gridInventory.GridSelectionMode = Myra.Graphics2D.UI.GridSelectionMode.Row;
			_gridInventory.Id = "_gridInventory";
			_gridInventory.Widgets.Add(label39);
			_gridInventory.Widgets.Add(label40);
			_gridInventory.Widgets.Add(label41);
			_gridInventory.Widgets.Add(label42);
			_gridInventory.Widgets.Add(label43);
			_gridInventory.Widgets.Add(label44);
			_gridInventory.Widgets.Add(label45);

			var scrollViewer2 = new ScrollViewer();
			scrollViewer2.Content = _gridInventory;

			var verticalStackPanel2 = new VerticalStackPanel();
			verticalStackPanel2.Widgets.Add(panel1);
			verticalStackPanel2.Widgets.Add(horizontalSeparator2);
			verticalStackPanel2.Widgets.Add(scrollViewer2);

			var horizontalStackPanel1 = new HorizontalStackPanel();
			horizontalStackPanel1.DefaultProportion = new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Part,
			};
			horizontalStackPanel1.Proportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Part,
			});
			horizontalStackPanel1.Proportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			});
			horizontalStackPanel1.Proportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Part,
			});
			horizontalStackPanel1.Widgets.Add(verticalStackPanel1);
			horizontalStackPanel1.Widgets.Add(verticalSeparator1);
			horizontalStackPanel1.Widgets.Add(verticalStackPanel2);

			var horizontalSeparator3 = new HorizontalSeparator();

			_buttonEquip = new TextButton();
			_buttonEquip.Text = "/c[green]E/c[white]quip";
			_buttonEquip.Width = 100;
			_buttonEquip.Id = "_buttonEquip";

			_buttonUse = new TextButton();
			_buttonUse.Text = "/c[green]U/c[white]se";
			_buttonUse.Width = 100;
			_buttonUse.Id = "_buttonUse";

			_buttonDrop = new TextButton();
			_buttonDrop.Text = "/c[green]D/c[white]rop";
			_buttonDrop.Width = 100;
			_buttonDrop.Id = "_buttonDrop";

			var horizontalStackPanel2 = new HorizontalStackPanel();
			horizontalStackPanel2.Spacing = 8;
			horizontalStackPanel2.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Right;
			horizontalStackPanel2.Widgets.Add(_buttonEquip);
			horizontalStackPanel2.Widgets.Add(_buttonUse);
			horizontalStackPanel2.Widgets.Add(_buttonDrop);

			var horizontalSeparator4 = new HorizontalSeparator();

			_textDescription = new Label();
			_textDescription.Text = "short sword - weapon, damage: 3-8";
			_textDescription.Id = "_textDescription";

			var horizontalSeparator5 = new HorizontalSeparator();

			_textAc = new Label();
			_textAc.Text = "AC: 100";
			_textAc.Id = "_textAc";

			_textHitRoll = new Label();
			_textHitRoll.Text = "Hit Roll: 20";
			_textHitRoll.GridColumn = 1;
			_textHitRoll.Id = "_textHitRoll";

			_textAttacks = new Label();
			_textAttacks.Text = "Attacks: 4-8/6-10";
			_textAttacks.GridColumn = 2;
			_textAttacks.Id = "_textAttacks";

			var grid1 = new Grid();
			grid1.ColumnSpacing = 8;
			grid1.RowSpacing = 8;
			grid1.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Part,
			});
			grid1.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Part,
			});
			grid1.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Part,
			});
			grid1.Widgets.Add(_textAc);
			grid1.Widgets.Add(_textHitRoll);
			grid1.Widgets.Add(_textAttacks);

			var verticalStackPanel3 = new VerticalStackPanel();
			verticalStackPanel3.Proportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Fill,
			});
			verticalStackPanel3.Width = 1000;
			verticalStackPanel3.Padding = new Thickness(0, 16);
			verticalStackPanel3.Widgets.Add(horizontalStackPanel1);
			verticalStackPanel3.Widgets.Add(horizontalSeparator3);
			verticalStackPanel3.Widgets.Add(horizontalStackPanel2);
			verticalStackPanel3.Widgets.Add(horizontalSeparator4);
			verticalStackPanel3.Widgets.Add(_textDescription);
			verticalStackPanel3.Widgets.Add(horizontalSeparator5);
			verticalStackPanel3.Widgets.Add(grid1);

			
			Title = "Items";
			Left = 30;
			Top = 35;
			Content = verticalStackPanel3;
		}

		
		public Grid _gridEquipment;
		public Label _textGold;
		public Grid _gridInventory;
		public TextButton _buttonEquip;
		public TextButton _buttonUse;
		public TextButton _buttonDrop;
		public Label _textDescription;
		public Label _textAc;
		public Label _textHitRoll;
		public Label _textAttacks;
	}
}
