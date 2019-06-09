using System;
using Myra;
using Myra.Graphics2D.UI;
using Wanderers.Core;
using Wanderers.MapEditor.UI;
using Microsoft.Xna.Framework;
using Wanderers.Compiling;
using Myra.Graphics2D.UI.File;
using Wanderers.Utils;
using System.IO;

namespace Wanderers.MapEditor
{
	public class Studio : Game
	{
		private const string MapFilter = "*.json";

		private readonly GraphicsDeviceManager _graphics;
		private readonly State _state;
		private Desktop _desktop;
		private StudioWidget _ui;
		private Grid _statisticsGrid;
		private TextBlock _gcMemoryLabel;
		private TextBlock _fpsLabel;
		private TextBlock _widgetsCountLabel;
//		private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();
		private string _filePath;
		private bool _isDirty;
		private readonly int[] _customColors;
		private Compiler _compiler;
		private string _lastFolder;
		private string _modulePath;
		private MapData _mapData = null;

		public string FilePath
		{
			get
			{
				return _filePath;
			}

			set
			{
				if (value == _filePath)
				{
					return;
				}

				_filePath = value;

				if (!string.IsNullOrEmpty(_filePath))
				{
					// Store last folder
					try
					{
						_lastFolder = Path.GetDirectoryName(_filePath);
					}
					catch (Exception)
					{
					}
				}

				UpdateTitle();
				UpdateMenuFile();
			}
		}

		public Map Map
		{
			get
			{
				return _ui._mapEditor.Map;
			}

			set
			{
				_ui._mapEditor.Map = value;
			}
		}

		public bool IsDirty
		{
			get { return _isDirty; }

			set
			{
				if (value == _isDirty)
				{
					return;
				}

				_isDirty = value;
				UpdateTitle();
			}
		}

		public StudioWidget UI
		{
			get
			{
				return _ui;
			}
		}

		public static Studio Instance { get; private set; }

		public bool ShowDebugInfo
		{
			get
			{
				return _statisticsGrid.Visible;
			}

			set
			{
				_statisticsGrid.Visible = value;
			}
		}

		public Studio(State state)
		{
			Instance = this;

			_graphics = new GraphicsDeviceManager(this);

			IsMouseVisible = true;
			Window.AllowUserResizing = true;

			// Restore state
			_state = state;

			if (_state != null)
			{
				_graphics.PreferredBackBufferWidth = _state.Size.X;
				_graphics.PreferredBackBufferHeight = _state.Size.Y;
				_customColors = _state.CustomColors;

				_lastFolder = _state.LastFolder;
			}
			else
			{
				_graphics.PreferredBackBufferWidth = 1280;
				_graphics.PreferredBackBufferHeight = 800;
			}
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			BuildUI();

			if (_state == null || string.IsNullOrEmpty(_state.EditedFile))
			{
			}
			else
			{
				Load(_state.EditedFile);
			}

		}

		private void BuildUI()
		{
#if DEBUG
#endif

			_desktop = new Desktop();

			_ui = new StudioWidget();

			_ui._openMenuItem.Selected += OpenProjectItemOnClicked;
			_ui._resizeMenuItem.Selected += OnResizeItemClicked;
			_ui._saveMenuItem.Selected += SaveItemOnClicked;
			_ui._saveAsMenuItem.Selected += _saveAsMenuItem_Selected;
			_ui._debugOptionsMenuItem.Selected += DebugOptionsItemOnSelected;
			_ui._quitMenuItem.Selected += QuitItemOnDown;

			_ui._aboutMenuItem.Selected += AboutItemOnClicked;

			_ui._comboItemTypes.SelectedIndex = 0;
			_ui._comboItemTypes.SelectedIndexChanged += OnComboTypesIndexChanged;
			_ui._mapEditor.MarkPositionChanged += (s, a) =>
			{
				var pos = _ui._mapEditor.MarkPosition;
				_ui._textPosition.Text = pos == null ? string.Empty : string.Format("X = {0}, Y = {1}", pos.Value.X, pos.Value.Y);
			};

			_desktop.Widgets.Add(_ui);

			_ui._topSplitPane.SetSplitterPosition(0, _state != null ? _state.TopSplitterPosition : 0.75f);

			_statisticsGrid = new Grid
			{
				Visible = false
			};

			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());

			_gcMemoryLabel = new TextBlock
			{
				Text = "GC Memory: ",
				Font = DefaultAssets.FontSmall
			};
			_statisticsGrid.Widgets.Add(_gcMemoryLabel);

			_fpsLabel = new TextBlock
			{
				Text = "FPS: ",
				Font = DefaultAssets.FontSmall,
				GridRow = 1
			};
			_statisticsGrid.Widgets.Add(_fpsLabel);

			_widgetsCountLabel = new TextBlock
			{
				Text = "Total Widgets: ",
				Font = DefaultAssets.FontSmall,
				GridRow = 2
			};
			_statisticsGrid.Widgets.Add(_widgetsCountLabel);

			_statisticsGrid.HorizontalAlignment = HorizontalAlignment.Left;
			_statisticsGrid.VerticalAlignment = VerticalAlignment.Bottom;
			_statisticsGrid.Left = 10;
			_statisticsGrid.Top = -10;
			_desktop.Widgets.Add(_statisticsGrid);

			UpdateMenuFile();
		}

		private void _saveAsMenuItem_Selected(object sender, EventArgs e)
		{
			Save(true);
		}

		private void OnResizeItemClicked(object sender, EventArgs e)
		{
			var dlg = new ResizeMapDialog();
			dlg._spinWidth.Value = Map.Size.X;
			dlg._spinHeight.Value = Map.Size.Y;

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				var oldSize = Map.Size;
				var oldTiles = Map.Tiles;

				// Resize
				Map.Size = new Point((int)dlg._spinWidth.Value, (int)dlg._spinHeight.Value);

				// Update tiles
				for (var y = 0; y < Map.Size.Y; ++y)
				{
					for (var x = 0; x < Map.Size.X; ++x)
					{
						var pos = new Point(x, y);
						if (x < oldSize.X && y < oldSize.Y)
						{
							// Old tile
							Map.SetTileAt(pos, Map.GetTileAt(oldTiles, x, y));
						}
						else
						{
							// Filler
							Map.GetTileAt(pos).Info = (TileInfo)dlg._comboFiller.SelectedItem.Tag;
						}
					}
				}
			};

			dlg.ShowModal(_desktop);
		}

		private void OnComboTypesIndexChanged(object sender, EventArgs e)
		{
			UpdateToolbox();
		}

		private void DebugOptionsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var dlg = new DebugOptionsDialog();

			dlg.AddOption("Show debug info",
						() => { ShowDebugInfo = true; },
						() => { ShowDebugInfo = false; });

			dlg.ShowModal(_desktop);
		}


		private void QuitItemOnDown(object sender, EventArgs eventArgs)
		{
			Exit();
		}

		private void AboutItemOnClicked(object sender, EventArgs eventArgs)
		{
			var dialog = Dialog.CreateMessageBox("About", "Wanderers Studio " + TJ.Version);
			dialog.ShowModal(_desktop);
		}

		private void SaveItemOnClicked(object sender, EventArgs eventArgs)
		{
			Save(false);
		}

		private void OpenProjectItemOnClicked(object sender, EventArgs eventArgs)
		{
			var dlg = new FileDialog(FileDialogMode.OpenFile)
			{
				Filter = MapFilter
			};

			if (!string.IsNullOrEmpty(FilePath))
			{
				dlg.Folder = Path.GetDirectoryName(FilePath);
			}
			else if (!string.IsNullOrEmpty(_lastFolder))
			{
				dlg.Folder = _lastFolder;
			}

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				var filePath = dlg.FilePath;
				if (string.IsNullOrEmpty(filePath))
				{
					return;
				}

				Load(filePath);
			};

			dlg.ShowModal(_desktop);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

//			_fpsCounter.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_gcMemoryLabel.Text = string.Format("GC Memory: {0} kb", GC.GetTotalMemory(false) / 1024);
//			_fpsLabel.Text = string.Format("FPS: {0:0.##}", _fpsCounter.FramesPerSecond);
			_widgetsCountLabel.Text = string.Format("Total Widgets: {0}", _desktop.CalculateTotalWidgets(true));

			GraphicsDevice.Clear(Color.Black);

			_desktop.Bounds = new Rectangle(0, 0,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render();

//			_fpsCounter.Draw(gameTime);
		}

		protected override void EndRun()
		{
			base.EndRun();

			var state = new State
			{
				Size = new Point(GraphicsDevice.PresentationParameters.BackBufferWidth,
					GraphicsDevice.PresentationParameters.BackBufferHeight),
				TopSplitterPosition = _ui._topSplitPane.GetSplitterPosition(0),
				EditedFile = FilePath,
				LastFolder = _lastFolder,
				CustomColors = _customColors,
			};

			state.Save();
		}

		private void ProcessSave(string filePath)
		{
			var result = Compiler.SaveMapToString(_ui._mapEditor.Map);
			File.WriteAllText(filePath, result);
			FilePath = filePath;
			IsDirty = false;
		}

		private void Save(bool setFileName)
		{
			var filePath = FilePath;
			if (string.IsNullOrEmpty(FilePath) || setFileName)
			{
				var dlg = new FileDialog(FileDialogMode.SaveFile)
				{
					Filter = "*.json"
				};

				if (!string.IsNullOrEmpty(FilePath))
				{
					dlg.FilePath = FilePath;
				}

				dlg.ShowModal(_desktop);

				dlg.Closed += (s, a) =>
				{
					if (dlg.Result)
					{
						ProcessSave(dlg.FilePath);
					}
				};
			}
			else
			{
				ProcessSave(FilePath);
			}
		}

		private void Load(string filePath)
		{
			try
			{
				var modulePath = Path.GetDirectoryName(Path.GetDirectoryName(filePath));
				if (modulePath != _modulePath)
				{
					_compiler = new Compiler
					{
						Params =
					{
						Verbose = true
					}
					};

					// Load module
					Module newDocument = _compiler.Process(modulePath, true);
					TJ.Module = newDocument;
					_modulePath = modulePath;
				}

				var json = File.ReadAllText(filePath);

				if (_mapData != null)
				{
					// Remove old map data from module
					foreach (var pair in _mapData.CreatureInfos)
					{
						TJ.Module.CreatureInfos.Remove(pair.Key);
					}

					foreach (var pair in _mapData.TileInfos)
					{
						TJ.Module.TileInfos.Remove(pair.Key);
					}

					_mapData = null;
				}

				_mapData = Compiler.LoadMapData(_compiler, filePath);

				if (_mapData != null)
				{
					// Add new map data to the module
					foreach (var pair in _mapData.CreatureInfos)
					{
						TJ.Module.CreatureInfos.Add(pair.Key, pair.Value);
					}

					foreach (var pair in _mapData.TileInfos)
					{
						TJ.Module.TileInfos.Add(pair.Key, pair.Value);
					}
				}

				var map = Compiler.LoadMapFromJson(_compiler, TJ.Module, json);
				_ui._mapEditor.Map = map;

				UpdateToolbox();

				_ui._mapNavigation.Invalidate();

				FilePath = filePath;
				IsDirty = false;
			}
			catch (Exception ex)
			{
				ReportError(ex.Message);
			}
		}

		private void UpdateToolbox()
		{
			_ui._listBoxItems.Items.Clear();

			switch (_ui._comboItemTypes.SelectedIndex)
			{
				case 0:
					foreach (var info in TJ.Module.TileInfos)
					{
						var item = new ListItem
						{
							Text = info.Key,
							Tag = info.Value
						};

						var renderable = new AppearanceRenderable(info.Value.Image)
						{
							Font = DefaultAssets.Font
						};

						item.Image = renderable;
						item.ImageTextSpacing = 8;

						_ui._listBoxItems.Items.Add(item);
					}
					break;
				case 1:
					// Eraser
					var erase = new ListItem
					{
						Text = "erase",
						Tag = null
					};

					_ui._listBoxItems.Items.Add(erase);

					foreach (var info in TJ.Module.CreatureInfos)
					{
						var item = new ListItem
						{
							Text = info.Key,
							Tag = info.Value
						};

						var renderable = new AppearanceRenderable(info.Value.Image)
						{
							Font = DefaultAssets.Font
						};

						item.Image = renderable;
						item.ImageTextSpacing = 8;

						_ui._listBoxItems.Items.Add(item);
					}
					break;
			}
		}

		private void ReportError(string message)
		{
			Dialog dlg = Dialog.CreateMessageBox("Error", message);
			dlg.ShowModal(_desktop);
		}

		private void UpdateTitle()
		{
			var title = string.IsNullOrEmpty(_filePath) ? "Wanderers Map Editor" : _filePath;

			if (_isDirty)
			{
				title += " *";
			}

			Window.Title = title;
		}

		private void UpdateMenuFile()
		{
			_ui._resizeMenuItem.Enabled = !string.IsNullOrEmpty(FilePath);
		}
	}
}