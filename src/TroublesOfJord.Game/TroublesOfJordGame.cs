using System.IO;
using TroublesOfJord.Compiling;
using TroublesOfJord.Storage;
using TroublesOfJord.UI;
using TroublesOfJord.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;
using TroublesOfJord.Core;

namespace TroublesOfJord
{
	public class TroublesOfJordGame : Game
	{
		private const string DataPath = "data";
		private readonly GraphicsDeviceManager _graphics;
		private Desktop _desktop;

		public Desktop Desktop
		{
			get
			{
				return _desktop;
			}
		}

		public static TroublesOfJordGame Instance { get; private set; }

		public int? StartGameIndex;

		public TroublesOfJordGame()
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

			TJ.GameLogHandler = message =>
			{
				var asGameView = _desktop.Widgets[0] as GameView;
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

			MyraEnvironment.Game = this;
			_desktop = new Desktop();

/*			MyraEnvironment.DisableClipping = true;
			MyraEnvironment.DrawFocusedWidgetFrame = true;
			MyraEnvironment.DrawWidgetsFrames = true;*/

			CompilerParams.Verbose = true;

			var compiler = new Compiler();
			TJ.Module = compiler.Process(Path.Combine(Files.ExecutableFolder, DataPath));

			if (StartGameIndex == null)
			{
				SwitchToMainMenu();
			} else
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
			_desktop.Root = widget;

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

			var gameView = (GameView)_desktop.Root;
			_desktop.FocusedKeyboardWidget = gameView;

			TJ.Session.MapNavigationBase = gameView.MapNavigation;
			TJ.Session.UpdateTilesVisibility();

			UpdateStats();

			TJ.GameLog("Welcome to 'Troubles of Jord' version {0}.", TJ.Version);
		}

		private void UpdateStats()
		{
			var gameView = (GameView)_desktop.Widgets[0];

			var life = TJ.Player.Stats.Life;
			gameView._labelHp.Text = string.Format("H: {0}/{1}", life.CurrentHP, life.MaximumHP);
			gameView._labelMana.Text = string.Format("M: {0}/{1}", life.CurrentMana, life.MaximumMana);
			gameView._labelStamina.Text = string.Format("S: {0}/{1}", life.CurrentStamina, life.MaximumStamina);
		}

		private void Life_Changed(object sender, System.EventArgs e)
		{
			UpdateStats();
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

			_desktop.Render();
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