﻿using System.IO;
using Jord.Loading;
using Jord.Storage;
using Jord.UI;
using Jord.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;
using Jord.Core;
using FontStashSharp;
using System.Linq;

namespace Jord
{
	public class JordGame : Game
	{
		private const string DataPath = "data";
		private readonly GraphicsDeviceManager _graphics;
		private readonly FpsCounter _fpsCounter = new FpsCounter();

		public Desktop Desktop { get; private set; }

		public MapView MapView { get => ((GameView)Desktop.Root).MapView; }

		public static JordGame Instance { get; private set; }

		public int? StartGameIndex;

		public JordGame()
		{
			Instance = this;

			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1440,
				PreferredBackBufferHeight = 900
			};

			IsMouseVisible = true;
			Window.AllowUserResizing = true;
			Window.Title = "Troubles of Jord";
			IsFixedTimeStep = false;

			TJ.GameLogHandler = message =>
			{
				var asGameView = Desktop.Widgets[0] as GameView;
				if (asGameView == null)
				{
					return;
				}

				asGameView.LogView.Log(message);
			};
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

			MyraEnvironment.Game = this;
			MyraEnvironment.EnableModalDarkening = true;

			TJ.GraphicsDevice = GraphicsDevice;
			Desktop = new Desktop();

			/*			MyraEnvironment.DisableClipping = true;
						MyraEnvironment.DrawFocusedWidgetFrame = true;
						MyraEnvironment.DrawWidgetsFrames = true;*/

			LoadSettings.Verbose = true;

			var compiler = new DatabaseLoader();
			TJ.Database = compiler.Process(Path.Combine(Files.ExecutableFolder, DataPath));

			if (StartGameIndex == null)
			{
				SwitchToMainMenu();
			}
			else
			{
				if (!Slot.Exists(StartGameIndex.Value))
				{
					TJ.LogInfo("Slot {0} isn't used", StartGameIndex.Value);
					SwitchToMainMenu();
				}
				else
				{
					Play(StartGameIndex.Value);
				}
			}
		}

		private T SwitchTo<T>() where T : Widget, new()
		{
			var widget = new T();
			Desktop.Root = widget;

			return widget;
		}

		public void SwitchToMainMenu()
		{
			SwitchTo<MainMenu>();
		}

		public void Play(int slotIndex)
		{
			if (TJ.Player != null)
			{
				TJ.Player.Stats.Life.Changed -= Life_Changed;
			}

			TJ.Session = new GameSession(slotIndex);

			TJ.Player.Stats.Life.Changed += Life_Changed;

			SwitchTo<GameView>();

			var gameView = (GameView)Desktop.Root;
			Desktop.FocusedKeyboardWidget = gameView;

			TJ.Session.MapNavigationBase = gameView.MapNavigation;
			TJ.Session.UpdateTilesVisibility();

			// Prevents camera from moving during the attack
			TJ.Player.StartAttack += (s, a) => this.MapView.CameraType = MapViewCameraType.PlayerPosition;
			TJ.Player.EndAttack += (s, a) => this.MapView.CameraType = MapViewCameraType.PlayerDisplayPosition;

			UpdateStats();

			TJ.GameLog("Welcome to 'Troubles of Jord' version {0}.", TJ.Version);
		}

		private void UpdateStats()
		{
			var gameView = (GameView)Desktop.Widgets[0];

			var life = TJ.Player.Stats.Life;
			gameView._labelHp.Text = string.Format("H: {0}/{1}", (int)life.CurrentHP, life.MaximumHP);
			gameView._labelMana.Text = string.Format("M: {0}/{1}", (int)life.CurrentMana, life.MaximumMana);
			gameView._labelStamina.Text = string.Format("S: {0}/{1}", (int)life.CurrentStamina, life.MaximumStamina);
		}

		private void Life_Changed(object sender, System.EventArgs e)
		{
			UpdateStats();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			TJ.ActivityService.Update();

			_fpsCounter.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (_graphics.PreferredBackBufferWidth != Window.ClientBounds.Width ||
				_graphics.PreferredBackBufferHeight != Window.ClientBounds.Height)
			{
				_graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				_graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				_graphics.ApplyChanges();
			}

			GraphicsDevice.Clear(Color.Black);

			var asGameView = Desktop.Widgets[0] as GameView;
			if (asGameView != null)
			{
				asGameView._labelFps.Text = _fpsCounter.FramesPerSecond.ToString();
			}


			Desktop.Render();

			_fpsCounter.Draw(gameTime);
		}

		protected override void EndRun()
		{
			base.EndRun();

			// Save current game
			if (TJ.Session != null)
			{
				TJ.Session.Slot.PlayerData = new PlayerData(TJ.Session.Player);
				TJ.Session.Slot.Save();
			}
		}
	}
}