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
using Wanderers.Compiling.Loaders;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Wanderers.Generation;

namespace Wanderers.MapEditor
{
	public class Studio : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private readonly State _state;
		private Desktop _desktop;
		private Grid _statisticsGrid;
		private Label _gcMemoryLabel;
		private Label _fpsLabel;
		private Label _widgetsCountLabel;
		private string _modulePath;
		private bool _isDirty;
		private readonly int[] _customColors;
		private Compiler _compiler;
		private string _lastFolder;

		public string ModulePath
		{
			get
			{
				return _modulePath;
			}

			set
			{
				if (_modulePath == value)
				{
					return;
				}

				_modulePath = value;
				UpdateMenuFile();
				UpdateTitle();
			}
		}

		public Map Map
		{
			get
			{
				return UI._mapEditor.Map;
			}

			set
			{
				UI._mapEditor.Map = value;

				UpdateToolbox();
				UpdateMenuFile();
				UpdateTitle();

				UI._mapNavigation.OnMapChanged();
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

		public StudioWidget UI { get; private set; }

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

			if (_state != null)
			{
				if (!string.IsNullOrEmpty(_state.ModulePath))
				{
					LoadModule(_state.ModulePath);

					if (!string.IsNullOrEmpty(_state.MapId))
					{
						SetMap(_state.MapId);
					}
				}
			}
		}

		private void BuildUI()
		{
			_desktop = new Desktop();

			_desktop.KeyDown += (s, a) =>
			{
				if (_desktop.HasModalWindow || UI._mainMenu.IsOpen)
				{
					return;
				}

				if (_desktop.DownKeys.Contains(Keys.LeftControl) || _desktop.DownKeys.Contains(Keys.RightControl))
				{
					if (_desktop.DownKeys.Contains(Keys.O))
					{
						OpenProjectItemOnClicked(this, EventArgs.Empty);
					}
					else if (_desktop.DownKeys.Contains(Keys.W))
					{
						OnSwitchMapMenuItemSelected(this, EventArgs.Empty);
					}
					else if (_desktop.DownKeys.Contains(Keys.N))
					{
						OnNewMapSelected(this, EventArgs.Empty);
					}
					else if (_desktop.DownKeys.Contains(Keys.S))
					{
						SaveMapSelected(this, EventArgs.Empty);
					}
					else if (_desktop.DownKeys.Contains(Keys.Q))
					{
						Exit();
					}
				}
			};

			UI = new StudioWidget();

			UI._openModuleMenuItem.Selected += OpenProjectItemOnClicked;

			UI._switchMapMenuItem.Selected += OnSwitchMapMenuItemSelected;
			UI._newMapMenuItem.Selected += OnNewMapSelected;
			UI._resizeMapMenuItem.Selected += OnResizeMapSelected;
			UI._saveMapMenuItem.Selected += SaveMapSelected;
			UI._saveMapAsMenuItem.Selected += SaveMapAsSelected;

			UI._debugOptionsMenuItem.Selected += DebugOptionsItemOnSelected;
			UI._quitMenuItem.Selected += QuitItemOnDown;

			UI._aboutMenuItem.Selected += AboutItemOnClicked;

			UI._comboItemTypes.SelectedIndex = 0;
			UI._comboItemTypes.SelectedIndexChanged += OnComboTypesIndexChanged;
			UI._mapEditor.MarkPositionChanged += (s, a) =>
			{
				var pos = UI._mapEditor.MarkPosition;
				UI._textPosition.Text = pos == null ? string.Empty : string.Format("X = {0}, Y = {1}", pos.Value.X, pos.Value.Y);
			};

			_desktop.Widgets.Add(UI);

			UI._topSplitPane.SetSplitterPosition(0, _state != null ? _state.TopSplitterPosition : 0.75f);

			_statisticsGrid = new Grid
			{
				Visible = false
			};

			_statisticsGrid.RowsProportions.Add(new Proportion());
			_statisticsGrid.RowsProportions.Add(new Proportion());
			_statisticsGrid.RowsProportions.Add(new Proportion());

			_gcMemoryLabel = new Label
			{
				Text = "GC Memory: ",
				Font = DefaultAssets.FontSmall
			};
			_statisticsGrid.Widgets.Add(_gcMemoryLabel);

			_fpsLabel = new Label
			{
				Text = "FPS: ",
				Font = DefaultAssets.FontSmall,
				GridRow = 1
			};
			_statisticsGrid.Widgets.Add(_fpsLabel);

			_widgetsCountLabel = new Label
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

		private void SetNewMap(Map map)
		{
			Map = map;
			IsDirty = false;
		}

		private void OnNewGeneratedMap()
		{
			var dlg = new NewGeneratedMapDialog();

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				try
				{
					var id = dlg._textId.Text;
					var generator = (BaseGenerator)dlg._comboGenerator.SelectedItem.Tag;

					var newMap = generator.Generate();
					SetNewMap(newMap);
				}
				catch (Exception ex)
				{
					SetMessageBoxText(dlg, "Error: " + ex.Message);
				}
			};

			dlg.ShowModal(_desktop);
		}

		private void OnNewMapSelected(object sender, EventArgs e)
		{
			var dlg = new NewMapDialog();

			switch (_state.LastNewMapType)
			{
				case 0:
					dlg._radioSingleTileMap.IsPressed = true;
					break;
				case 1:
					dlg._radioGeneratedMap.IsPressed = true;
					break;
			}

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				try
				{

					if (dlg._radioSingleTileMap.IsPressed)
					{
						throw new NotImplementedException();
						_state.LastNewMapType = 0;
					}
					else
					{
						OnNewGeneratedMap();
						_state.LastNewMapType = 1;
					}
				}
				catch(Exception ex)
				{
					ReportError(ex.Message);
				}
			};

			dlg.ShowModal(_desktop);
		}

		private static void SetMessageBoxText(Dialog dlg, string newText)
		{
			var Label = (Label)dlg.Content;

			Label.Text = newText;
		}

/*		private void GenerateGlobalMap()
		{
			var dlg = Dialog.CreateMessageBox("Generation...", string.Empty);

			dlg.ButtonOk.Enabled = false;
			dlg.Width = 400;

			Map map = null;

			Task.Run(() =>
			{
				try
				{
					var context = new GenerationContext
					{
						InfoHandler = (s) => SetMessageBoxText(dlg, s)
					};

					// Generate land
					var landGenerator = new LandGenerator(context);
					var landGeneratorConfig = new LandGeneratorConfig();
					var generationResult = landGenerator.Generate(landGeneratorConfig);

					// Generate locations
					var locationGenerator = new LocationsGenerator(context);
					var locationsGeneratorConfig = new LocationsGeneratorConfig();
					locationGenerator.Generate(locationsGeneratorConfig, generationResult);

					map = new Map
					{
						Size = new Point(landGeneratorConfig.WorldSize, landGeneratorConfig.WorldSize)
					};

					for (var i = 0; i < generationResult.Height; ++i)
					{
						for (var j = 0; j < generationResult.Width; ++j)
						{
							var tileType = generationResult.GetWorldMapTileType(j, i);
							TileInfo tileInfo = null;
							switch (tileType)
							{
								case WorldMapTileType.Water:
									tileInfo = TJ.Module.TileInfos["Water"];
									break;
								case WorldMapTileType.Mountain:
									tileInfo = TJ.Module.TileInfos["Mountain"];
									break;
								case WorldMapTileType.Forest:
									tileInfo = TJ.Module.TileInfos["Tree"];
									break;
								case WorldMapTileType.Road:
									tileInfo = TJ.Module.TileInfos["Road"];
									break;
								case WorldMapTileType.Wall:
									tileInfo = TJ.Module.TileInfos["Wall"];
									break;
								default:
									tileInfo = TJ.Module.TileInfos["Grass"];
									break;
							}

							var tile = new Tile();
							tile.Info = tileInfo;
							map.SetTileAt(new Point(j, i), tile);
						}
					}

					SetMessageBoxText(dlg, "Done.");
				}
				catch (Exception ex)
				{
					SetMessageBoxText(dlg, "Error: " + ex.Message);
				}
				finally
				{
					dlg.ButtonOk.Enabled = true;
				}
			});

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result || map == null)
				{
					if (!dlg.ButtonOk.Enabled)
					{
						// Generation is still running
						// Abort it

					}

					return;
				}

				Map = map;

				FilePath = string.Empty;
				IsDirty = false;
			};

			dlg.ShowModal(_desktop);
		}*/

		private void SaveMapAsSelected(object sender, EventArgs e)
		{
			Save(true);
		}

		private void OnResizeMapSelected(object sender, EventArgs e)
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

		private void SaveMapSelected(object sender, EventArgs eventArgs)
		{
			Save(false);
		}

		private void OnSwitchMapMenuItemSelected(object sender, EventArgs e)
		{
			var dlg = new ChooseMapDialog();

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				var map = (Map)dlg._listMaps.SelectedItem.Tag;
				SetMap(map.Id);
			};

			dlg.ShowModal(_desktop);
		}

		private void OpenProjectItemOnClicked(object sender, EventArgs eventArgs)
		{
			var dlg = new FileDialog(FileDialogMode.ChooseFolder);

			if (!string.IsNullOrEmpty(ModulePath))
			{
				dlg.Folder = Path.GetDirectoryName(ModulePath);
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

				LoadModule(filePath);
			};

			dlg.ShowModal(_desktop);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_gcMemoryLabel.Text = string.Format("GC Memory: {0} kb", GC.GetTotalMemory(false) / 1024);
//			_fpsLabel.Text = string.Format("FPS: {0:0.##}", _fpsCounter.FramesPerSecond);
			_widgetsCountLabel.Text = string.Format("Total Widgets: {0}", _desktop.CalculateTotalWidgets(true));

			GraphicsDevice.Clear(Color.Black);

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
				TopSplitterPosition = UI._topSplitPane.GetSplitterPosition(0),
				ModulePath = _modulePath,
				MapId = Map != null?Map.Id:string.Empty,
				LastFolder = _lastFolder,
				CustomColors = _customColors,
			};

			state.Save();
		}

		private void ProcessSave(string filePath)
		{
			var result = MapLoader.SaveMapToString(UI._mapEditor.Map);
			File.WriteAllText(filePath, result);
			Map.Source = filePath;
			IsDirty = false;
		}

		private void Save(bool setFileName)
		{
			var filePath = Map.Source;
			if (string.IsNullOrEmpty(filePath) || setFileName)
			{
				var dlg = new FileDialog(FileDialogMode.SaveFile)
				{
					Filter = "*.json"
				};

				if (!string.IsNullOrEmpty(filePath))
				{
					dlg.FilePath = filePath;
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
				ProcessSave(filePath);
			}
		}

		private void LoadModule(string modulePath)
		{
			try
			{
				CompilerParams.Verbose = true;
				_compiler = new Compiler();

				// Load module
				Module newDocument = _compiler.Process(modulePath);
				TJ.Module = newDocument;
				ModulePath = modulePath;

				Map = null;
				IsDirty = false;
			}
			catch (Exception ex)
			{
				ReportError(ex.Message);
			}
		}

		private void SetMap(string mapId)
		{
			try
			{
				Map = TJ.Module.Maps[mapId];
			}
			catch (Exception ex)
			{
				ReportError(ex.Message);
			}
		}

		private void UpdateToolbox()
		{
			UI._listBoxItems.Items.Clear();

			switch (UI._comboItemTypes.SelectedIndex)
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

						UI._listBoxItems.Items.Add(item);
					}
					break;
				case 1:
					// Eraser
					var erase = new ListItem
					{
						Text = "erase",
						Tag = null
					};

					UI._listBoxItems.Items.Add(erase);

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

						UI._listBoxItems.Items.Add(item);
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
			var title = "Wanderers Map Editor";
			
			if (!string.IsNullOrEmpty(_modulePath))
			{
				title = _modulePath;

				if (Map != null && !string.IsNullOrEmpty(Map.Source))
				{
					title = Map.Source;
					if (_isDirty)
					{
						title += " *";
					}
				}
			}

			Window.Title = title;
		}

		private void UpdateMenuFile()
		{
			var moduleLoaded = !string.IsNullOrEmpty(_modulePath);

			UI._newMapMenuItem.Enabled = moduleLoaded;
			UI._switchMapMenuItem.Enabled = moduleLoaded;
			UI._saveMapMenuItem.Enabled = moduleLoaded && Map != null;
			UI._saveMapAsMenuItem.Enabled = moduleLoaded && Map != null;
			UI._resizeMapMenuItem.Enabled = moduleLoaded && Map != null;
		}
	}
}