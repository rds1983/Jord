﻿using System;
using Myra;
using Myra.Graphics2D.UI;
using Jord.Core;
using Jord.MapEditor.UI;
using Microsoft.Xna.Framework;
using Jord.Loading;
using Myra.Graphics2D.UI.File;
using Jord.Utils;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Jord.Generation;
using Microsoft.Xna.Framework.Graphics;

namespace Jord.MapEditor
{
	public class Studio : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private readonly State _state;
		private Grid _statisticsGrid;
		private Label _gcMemoryLabel;
		private Label _fpsLabel;
		private Label _widgetsCountLabel;
		private string _modulePath;
		private bool _isDirty;
		private readonly int[] _customColors;
		private readonly GenerationSettings _generationSettings = new GenerationSettings();
		private DatabaseLoader _compiler;
		private string _lastFolder;
		private Desktop _desktop;

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
				IsDirty = true;
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
				_generationSettings = _state.GenerationSettings;
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

			GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

			MyraEnvironment.Game = this;
			TJ.GraphicsDevice = GraphicsDevice;

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
				if (_desktop.HasModalWidget || UI._mainMenu.IsOpen)
				{
					return;
				}

				if (_desktop.IsKeyDown(Keys.LeftControl) || _desktop.IsKeyDown(Keys.RightControl))
				{
					if (_desktop.IsKeyDown(Keys.O))
					{
						OpenProjectItemOnClicked(this, EventArgs.Empty);
					}
					else if (_desktop.IsKeyDown(Keys.W))
					{
						OnSwitchMapMenuItemSelected(this, EventArgs.Empty);
					}
					else if (_desktop.IsKeyDown(Keys.N))
					{
						OnNewMapSelected(this, EventArgs.Empty);
					}
					else if (_desktop.IsKeyDown(Keys.S))
					{
						SaveMapSelected(this, EventArgs.Empty);
					}
					else if (_desktop.IsKeyDown(Keys.Q))
					{
						Exit();
					}
				}
			};

			UI = new StudioWidget();

			UI._openModuleMenuItem.Selected += OpenProjectItemOnClicked;

			UI._switchMapMenuItem.Selected += OnSwitchMapMenuItemSelected;
			UI._newMapMenuItem.Selected += OnNewMapSelected;
			UI._generateMapMenuItem.Selected += OnNewGeneratedMap;
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
				Font = UIUtility.DefaultFont
			};
			_statisticsGrid.Widgets.Add(_gcMemoryLabel);

			_fpsLabel = new Label
			{
				Text = "FPS: ",
				Font = UIUtility.DefaultFont,
				GridRow = 1
			};
			_statisticsGrid.Widgets.Add(_fpsLabel);

			_widgetsCountLabel = new Label
			{
				Text = "Total Widgets: ",
				Font = UIUtility.DefaultFont,
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

		private void OnNewGeneratedMap(object sender, EventArgs args)
		{
			var dlg = new GenerateMapDialog();

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				try
				{
					SetNewMap(dlg.Map);
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

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				try
				{

					throw new NotImplementedException();
				}
				catch (Exception ex)
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

					dlg.ShowModal();
				}*/

		private void SaveMapAsSelected(object sender, EventArgs e)
		{
			Save(true);
		}

		private void ResizeMap(ChangeSizeAction action, ChangeSizeDirection direction, int amount, TileInfo filler)
		{
			Map newMap = null;
			if (action == ChangeSizeAction.Expand)
			{
				var newSize = new Point(Map.Width, Map.Height);
				if (direction == ChangeSizeDirection.Left || direction == ChangeSizeDirection.Right)
				{
					newSize.X += amount;
				}
				else
				{
					newSize.Y += amount;
				}

				newMap = new Map(newSize);

				var startPos = Point.Zero;
				if (direction == ChangeSizeDirection.Left)
				{
					startPos.X = amount;
				}
				else if (direction == ChangeSizeDirection.Top)
				{
					startPos.Y = amount;
				}

				// Copy old map
				for (var y = 0; y < Map.Height; ++y)
				{
					for (var x = 0; x < Map.Width; ++x)
					{
						newMap[x + startPos.X, y + startPos.Y] = Map[x, y];
					}
				}

				// Fill
				switch (direction)
				{
					case ChangeSizeDirection.Left:
						for (var y = 0; y < newMap.Height; ++y)
						{
							for (var x = 0; x < amount; ++x)
							{
								newMap[x, y].Info = filler;
							}
						}
						break;
					case ChangeSizeDirection.Top:
						for (var y = 0; y < amount; ++y)
						{
							for (var x = 0; x < newMap.Width; ++x)
							{
								newMap[x, y].Info = filler;
							}
						}
						break;
					case ChangeSizeDirection.Right:
						for (var y = 0; y < newMap.Height; ++y)
						{
							for (var x = Map.Width; x < Map.Width + amount; ++x)
							{
								newMap[x, y].Info = filler;
							}
						}
						break;
					case ChangeSizeDirection.Bottom:
						for (var y = Map.Height; y < Map.Height + amount; ++y)
						{
							for (var x = 0; x < newMap.Width; ++x)
							{
								newMap[x, y].Info = filler;
							}
						}
						break;
				}
			}
			else
			{
				var newSize = new Point(Map.Width, Map.Height);
				if (direction == ChangeSizeDirection.Left || direction == ChangeSizeDirection.Right)
				{
					newSize.X -= amount;
				}
				else
				{
					newSize.Y -= amount;
				}

				newMap = new Map(newSize);

				var startPos = Point.Zero;
				if (direction == ChangeSizeDirection.Left)
				{
					startPos.X = amount;
				}
				else if (direction == ChangeSizeDirection.Top)
				{
					startPos.Y = amount;
				}

				// Copy old map
				for (var y = 0; y < newMap.Height; ++y)
				{
					for (var x = 0; x < newMap.Width; ++x)
					{
						newMap[x, y] = Map[x + startPos.X, y + startPos.Y];
					}
				}
			}

			newMap.UpdateTilesCoords();
			Map = newMap;
		}

		private void OnResizeMapSelected(object sender, EventArgs e)
		{
			var dlg = new ChangeSizeDialog
			{
				Map = Map
			};

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				ResizeMap(dlg.Action, dlg.Direction, dlg.Amount, dlg.FillerTile);
			};

			dlg.ShowModal(_desktop);
		}

		private void OnComboTypesIndexChanged(object sender, EventArgs e)
		{
			UpdateToolbox();
		}

		private void DebugOptionsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var dlg = new DebugOptionsWindow();

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
			var dialog = Dialog.CreateMessageBox("About", "Jord Studio " + TJ.Version);
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
				MapId = Map != null ? Map.Id : string.Empty,
				LastFolder = _lastFolder,
				CustomColors = _customColors,
				GenerationSettings = _generationSettings,
			};

			state.Save();
		}

		private void ProcessSave(string filePath)
		{
			var name = Path.GetFileNameWithoutExtension(filePath);
			
			if (string.IsNullOrEmpty(UI._mapEditor.Map.Id))
			{
				UI._mapEditor.Map.Id = name;
			}

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
				LoadSettings.Verbose = true;
				_compiler = new DatabaseLoader();

				// Load module
				Database newDocument = _compiler.Process(modulePath);
				TJ.Database = newDocument;
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
				Map = TJ.Database.Maps[mapId];
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
					foreach (var info in TJ.Database.TileInfos)
					{
						var item = new ListItem
						{
							Text = info.Key,
							Tag = info.Value
						};

						item.Image = new AppearanceRenderable(info.Value.Image);
						item.ImageTextSpacing = 8;

						UI._listBoxItems.Items.Add(item);
					}
					break;
				case 1:
					{
						// Eraser
						var erase = new ListItem
						{
							Text = "erase",
							Tag = null
						};

						UI._listBoxItems.Items.Add(erase);

						foreach (var info in TJ.Database.TileObjects)
						{
							var item = new ListItem
							{
								Text = info.Key,
								Tag = info.Value
							};

							item.Image = new AppearanceRenderable(info.Value.Image);
							item.ImageTextSpacing = 8;

							UI._listBoxItems.Items.Add(item);
						}
						break;
					}
				case 2:
					{
						// Eraser
						var erase = new ListItem
						{
							Text = "erase",
							Tag = null
						};

						UI._listBoxItems.Items.Add(erase);

						foreach (var info in TJ.Database.CreatureInfos)
						{
							var item = new ListItem
							{
								Text = info.Key,
								Tag = info.Value
							};

							item.Image = new AppearanceRenderable(info.Value.Image);
							item.ImageTextSpacing = 8;

							UI._listBoxItems.Items.Add(item);
						}
						break;
					}
			}
		}

		private void ReportError(string message)
		{
			Dialog dlg = Dialog.CreateMessageBox("Error", message);
			dlg.ShowModal(_desktop);
		}

		private void UpdateTitle()
		{
			var title = "Jord Map Editor";

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
			UI._generateMapMenuItem.Enabled = moduleLoaded;
			UI._saveMapMenuItem.Enabled = moduleLoaded && Map != null;
			UI._saveMapAsMenuItem.Enabled = moduleLoaded && Map != null;
			UI._resizeMapMenuItem.Enabled = moduleLoaded && Map != null;
		}
	}
}