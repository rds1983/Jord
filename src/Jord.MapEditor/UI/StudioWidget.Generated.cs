/* Generated by MyraPad at 10/9/2022 5:19:41 AM */
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Properties;
using FontStashSharp.RichText;

namespace Jord.MapEditor.UI
{
	partial class StudioWidget: Grid
	{
		private void BuildUI()
		{
			_openModuleMenuItem = new MenuItem();
			_openModuleMenuItem.Text = "&Open Module...";
			_openModuleMenuItem.ShortcutText = "Ctrl+O";
			_openModuleMenuItem.Id = "_openModuleMenuItem";

			_reloadModuleMenuItem = new MenuItem();
			_reloadModuleMenuItem.Text = "&Reload Module";
			_reloadModuleMenuItem.ShortcutText = "Ctrl+R";
			_reloadModuleMenuItem.Id = "_reloadModuleMenuItem";

			_generateWorldMenuItem = new MenuItem();
			_generateWorldMenuItem.Text = "&Generate World...";
			_generateWorldMenuItem.ShortcutText = "Ctrl+G";
			_generateWorldMenuItem.Id = "_generateWorldMenuItem";

			var menuSeparator1 = new MenuSeparator();

			_switchMapMenuItem = new MenuItem();
			_switchMapMenuItem.Text = "S&witch Map...";
			_switchMapMenuItem.ShortcutText = "Ctrl+W";
			_switchMapMenuItem.Id = "_switchMapMenuItem";

			_newMapMenuItem = new MenuItem();
			_newMapMenuItem.Text = "&New Map...";
			_newMapMenuItem.ShortcutText = "Ctrl+N";
			_newMapMenuItem.Id = "_newMapMenuItem";

			_saveMapMenuItem = new MenuItem();
			_saveMapMenuItem.Text = "&Save Map";
			_saveMapMenuItem.ShortcutText = "Ctrl+S";
			_saveMapMenuItem.Id = "_saveMapMenuItem";

			_saveMapAsMenuItem = new MenuItem();
			_saveMapAsMenuItem.Text = "Save Map &As";
			_saveMapAsMenuItem.Id = "_saveMapAsMenuItem";

			_resizeMapMenuItem = new MenuItem();
			_resizeMapMenuItem.Text = "Resize &Map";
			_resizeMapMenuItem.Id = "_resizeMapMenuItem";

			var menuSeparator2 = new MenuSeparator();

			_debugOptionsMenuItem = new MenuItem();
			_debugOptionsMenuItem.Text = "&UI Debug Options";
			_debugOptionsMenuItem.Id = "_debugOptionsMenuItem";

			var menuSeparator3 = new MenuSeparator();

			_quitMenuItem = new MenuItem();
			_quitMenuItem.Text = "&Quit";
			_quitMenuItem.ShortcutText = "Ctrl+Q";
			_quitMenuItem.Id = "_quitMenuItem";

			var menuItem1 = new MenuItem();
			menuItem1.Text = "&File";
			menuItem1.Items.Add(_openModuleMenuItem);
			menuItem1.Items.Add(_reloadModuleMenuItem);
			menuItem1.Items.Add(_generateWorldMenuItem);
			menuItem1.Items.Add(menuSeparator1);
			menuItem1.Items.Add(_switchMapMenuItem);
			menuItem1.Items.Add(_newMapMenuItem);
			menuItem1.Items.Add(_saveMapMenuItem);
			menuItem1.Items.Add(_saveMapAsMenuItem);
			menuItem1.Items.Add(_resizeMapMenuItem);
			menuItem1.Items.Add(menuSeparator2);
			menuItem1.Items.Add(_debugOptionsMenuItem);
			menuItem1.Items.Add(menuSeparator3);
			menuItem1.Items.Add(_quitMenuItem);

			_aboutMenuItem = new MenuItem();
			_aboutMenuItem.Text = "&About";
			_aboutMenuItem.Id = "_aboutMenuItem";

			var menuItem2 = new MenuItem();
			menuItem2.Text = "&Help";
			menuItem2.Items.Add(_aboutMenuItem);

			_mainMenu = new HorizontalMenu();
			_mainMenu.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Stretch;
			_mainMenu.Id = "_mainMenu";
			_mainMenu.Items.Add(menuItem1);
			_mainMenu.Items.Add(menuItem2);

			_textPosition = new Label();
			_textPosition.GridRow = 1;
			_textPosition.Id = "_textPosition";

			_leftContainer = new Grid();
			_leftContainer.RowsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Fill,
			});
			_leftContainer.RowsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			});
			_leftContainer.Id = "_leftContainer";
			_leftContainer.Widgets.Add(_textPosition);

			_mapViewerContainer = new Panel();
			_mapViewerContainer.Id = "_mapViewerContainer";

			_tilesItem = new ListItem();
			_tilesItem.Text = "Tiles";
			_tilesItem.Id = "_tilesItem";

			_tileObjectsItem = new ListItem();
			_tileObjectsItem.Text = "Tile Objects";
			_tileObjectsItem.Id = "_tileObjectsItem";

			_creaturesItem = new ListItem();
			_creaturesItem.Text = "Creatures";
			_creaturesItem.Id = "_creaturesItem";

			_comboItemTypes = new ComboBox();
			_comboItemTypes.Id = "_comboItemTypes";
			_comboItemTypes.Items.Add(_tilesItem);
			_comboItemTypes.Items.Add(_tileObjectsItem);
			_comboItemTypes.Items.Add(_creaturesItem);

			var listItem1 = new ListItem();
			listItem1.Text = "testTile1";

			var listItem2 = new ListItem();
			listItem2.Text = "testTile2";

			_listBoxItems = new ListBox();
			_listBoxItems.GridRow = 1;
			_listBoxItems.Id = "_listBoxItems";
			_listBoxItems.Items.Add(listItem1);
			_listBoxItems.Items.Add(listItem2);

			var grid1 = new Grid();
			grid1.DefaultRowProportion = new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			};
			grid1.Widgets.Add(_comboItemTypes);
			grid1.Widgets.Add(_listBoxItems);

			var verticalSplitPane1 = new VerticalSplitPane();
			verticalSplitPane1.Widgets.Add(_mapViewerContainer);
			verticalSplitPane1.Widgets.Add(grid1);

			_topSplitPane = new HorizontalSplitPane();
			_topSplitPane.GridRow = 1;
			_topSplitPane.Id = "_topSplitPane";
			_topSplitPane.Widgets.Add(_leftContainer);
			_topSplitPane.Widgets.Add(verticalSplitPane1);

			
			RowsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			});
			RowsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Fill,
			});
			Id = "Root";
			Widgets.Add(_mainMenu);
			Widgets.Add(_topSplitPane);
		}

		
		public MenuItem _openModuleMenuItem;
		public MenuItem _reloadModuleMenuItem;
		public MenuItem _generateWorldMenuItem;
		public MenuItem _switchMapMenuItem;
		public MenuItem _newMapMenuItem;
		public MenuItem _saveMapMenuItem;
		public MenuItem _saveMapAsMenuItem;
		public MenuItem _resizeMapMenuItem;
		public MenuItem _debugOptionsMenuItem;
		public MenuItem _quitMenuItem;
		public MenuItem _aboutMenuItem;
		public HorizontalMenu _mainMenu;
		public Label _textPosition;
		public Grid _leftContainer;
		public Panel _mapViewerContainer;
		public ListItem _tilesItem;
		public ListItem _tileObjectsItem;
		public ListItem _creaturesItem;
		public ComboBox _comboItemTypes;
		public ListBox _listBoxItems;
		public HorizontalSplitPane _topSplitPane;
	}
}
