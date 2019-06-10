using System.IO;
using Wanderers.Compiling;
using Wanderers.Storage;
using Wanderers.UI;
using Wanderers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;

namespace Wanderers
{
	public class WanderersGame : Game
	{
		private const string DataPath = "../../../../../data";

		private static WanderersGame _instance;

		private readonly GraphicsDeviceManager _graphics;
		private Desktop _desktop;

		public static WanderersGame Instance
		{
			get { return _instance; }
		}


		public Desktop Desktop
		{
			get { return _desktop; }
		}

		public WanderersGame()
		{
			_instance = this;

			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1440,
				PreferredBackBufferHeight = 900
			};

			IsMouseVisible = true;
			Window.AllowUserResizing = true;
			Window.Title = "Troubles of Jord";
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

/*			MyraEnvironment.DisableClipping = true;
			MyraEnvironment.DrawFocusedWidgetFrame = true;
			MyraEnvironment.DrawWidgetsFrames = true;*/

			_desktop = new Desktop();

			var compiler = new Compiler
			{
				Params =
				{
					Verbose = true
				}
			};

			TJ.Module = compiler.Process(Path.Combine(Files.ExecutableFolder, DataPath));

			SwitchToMainMenu();
		}

		private T SwitchTo<T>() where T : Widget, new()
		{
			_desktop.Widgets.Clear();
			var widget = new T();
			_desktop.Widgets.Add(widget);

			return widget;
		}

		public void SwitchToMainMenu()
		{
			SwitchTo<MainMenu>();
		}

		public void GameLog(string message)
		{
			var asGameView = _desktop.Widgets[0] as GameView;
			if (asGameView == null)
			{
				return;
			}

			asGameView.LogView.Log(message);
		}


		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (TJ.GameSession != null)
			{
				TJ.GameSession.OnTimer();
			}
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

			_desktop.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render();
		}

		protected override void EndRun()
		{
			base.EndRun();

			// Save current game
			if (TJ.GameSession != null)
			{
				TJ.GameSession.Slot.CharacterData = new CharacterData(TJ.GameSession.Character);
				TJ.GameSession.Slot.Save();
			}
		}
	}
}