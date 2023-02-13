using System;
using System.Reflection;
using Jord.Core;
using Jord.Storage;
using Jord.Utils;
using Microsoft.Xna.Framework.Graphics;
using Database = Jord.Core.Database;

namespace Jord
{
	public static class TJ
	{
		private static Database _database;

		public static Action<string> InfoLogHandler = Console.WriteLine;
		public static Action<string> WarnLogHandler = Console.WriteLine;
		public static Action<string> ErrorLogHandler = Console.WriteLine;
		public static Action<string> GameLogHandler = Console.WriteLine;

		public static GraphicsDevice GraphicsDevice { get; set; }

		public static Database Database
		{
			get => _database;
			set
			{
				_database = value;
				_database.Tileset = _database.Settings.Tileset;
			}
		}

		public static Tileset Tileset => _database.Tileset;

		public static StorageService StorageService { get; } = new StorageService();
		public static ActivityService ActivityService { get; } = new ActivityService();

		public static GameSession Session { get; set; }

		public static Player Player
		{
			get
			{
				if (Session == null)
				{
					return null;
				}

				return Session.Player;
			}
		}

		public static string Version
		{
			get
			{
				var assembly = typeof(TJ).GetTypeInfo().Assembly;
				var name = new AssemblyName(assembly.FullName);

				return name.Version.ToString();
			}
		}

		public static void LogInfo(string message, params object[] args)
		{
			if (InfoLogHandler == null)
			{
				return;
			}

			InfoLogHandler(StringUtils.FormatMessage(message, args));
		}

		public static void LogWarn(string message, params object[] args)
		{
			if (WarnLogHandler == null)
			{
				return;
			}

			WarnLogHandler(StringUtils.FormatMessage(message, args));
		}

		public static void LogError(string message, params object[] args)
		{
			if (ErrorLogHandler == null)
			{
				return;
			}

			ErrorLogHandler(StringUtils.FormatMessage(message, args));
		}

		public static void GameLog(string message, params object[] args)
		{
			if (GameLogHandler == null)
			{
				return;
			}

			GameLogHandler(StringUtils.FormatMessage(message, args));
		}
	}
}