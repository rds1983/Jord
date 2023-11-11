/* Generated by MyraPad at 11/12/2023 5:34:19 AM */
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
	partial class CraftingWindow: Window
	{
		private void BuildUI()
		{
			var horizontalSeparator1 = new HorizontalSeparator();

			var listItem1 = new ListItem();
			listItem1.Text = "leather jacket";
			listItem1.Color = ColorStorage.CreateColor(140, 140, 140, 255);

			var listItem2 = new ListItem();
			listItem2.Text = "leather pants";
			listItem2.Color = ColorStorage.CreateColor(140, 140, 140, 255);

			var listItem3 = new ListItem();
			listItem3.Text = "leather sleeves";

			var listItem4 = new ListItem();
			listItem4.Text = "leather cap";

			var listItem5 = new ListItem();
			listItem5.Text = "leather gloves";

			var listItem6 = new ListItem();
			listItem6.Text = "leather boots";

			_listBoxRecipes = new ListBox();
			_listBoxRecipes.Width = 200;
			_listBoxRecipes.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Stretch;
			_listBoxRecipes.Id = "_listBoxRecipes";
			_listBoxRecipes.Items.Add(listItem1);
			_listBoxRecipes.Items.Add(listItem2);
			_listBoxRecipes.Items.Add(listItem3);
			_listBoxRecipes.Items.Add(listItem4);
			_listBoxRecipes.Items.Add(listItem5);
			_listBoxRecipes.Items.Add(listItem6);

			var verticalSeparator1 = new VerticalSeparator();

			_labelDescription = new Label();
			_labelDescription.Text = "armor, ac: 4";
			_labelDescription.Wrap = true;
			_labelDescription.Width = 300;
			_labelDescription.Id = "_labelDescription";
			StackPanel.SetProportionType(_labelDescription, Myra.Graphics2D.UI.ProportionType.Part);

			var horizontalSeparator2 = new HorizontalSeparator();

			_labelRequires = new Label();
			_labelRequires.Text = "2 Leather (3), Iron Ingot (2), 3 Steel Ingot (4), 4 Mithril Ingot (5)";
			_labelRequires.Wrap = true;
			_labelRequires.Width = 300;
			_labelRequires.Id = "_labelRequires";
			StackPanel.SetProportionType(_labelRequires, Myra.Graphics2D.UI.ProportionType.Part);

			var verticalStackPanel1 = new VerticalStackPanel();
			verticalStackPanel1.Widgets.Add(_labelDescription);
			verticalStackPanel1.Widgets.Add(horizontalSeparator2);
			verticalStackPanel1.Widgets.Add(_labelRequires);

			var horizontalStackPanel1 = new HorizontalStackPanel();
			StackPanel.SetProportionType(horizontalStackPanel1, Myra.Graphics2D.UI.ProportionType.Fill);
			horizontalStackPanel1.Widgets.Add(_listBoxRecipes);
			horizontalStackPanel1.Widgets.Add(verticalSeparator1);
			horizontalStackPanel1.Widgets.Add(verticalStackPanel1);

			var horizontalSeparator3 = new HorizontalSeparator();

			_buttonCreate = new TextButton();
			_buttonCreate.Text = "/c[green]C/c[white]reate";
			_buttonCreate.Width = 120;
			_buttonCreate.Id = "_buttonCreate";

			var horizontalStackPanel2 = new HorizontalStackPanel();
			horizontalStackPanel2.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Right;
			horizontalStackPanel2.Widgets.Add(_buttonCreate);

			var verticalStackPanel2 = new VerticalStackPanel();
			verticalStackPanel2.Widgets.Add(horizontalSeparator1);
			verticalStackPanel2.Widgets.Add(horizontalStackPanel1);
			verticalStackPanel2.Widgets.Add(horizontalSeparator3);
			verticalStackPanel2.Widgets.Add(horizontalStackPanel2);

			
			Title = "Crafting Bench";
			Left = 702;
			Top = 222;
			Height = 500;
			Content = verticalStackPanel2;
		}

		
		public ListBox _listBoxRecipes;
		public Label _labelDescription;
		public Label _labelRequires;
		public TextButton _buttonCreate;
	}
}
