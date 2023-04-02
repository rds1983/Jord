using System;
using System.Linq;
using System.Reflection;
using DefaultEcs;
using DefaultEcs.System;
using Jord.Core;
using Jord.Services;
using Jord.Storage;
using Jord.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Database = Jord.Core.Database;

namespace Jord
{
	public static partial class TJ
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

		public static Settings Settings => Database.Settings;

		public static Tileset Tileset => _database != null ? _database.Tileset : null;

		public static StorageService StorageService { get; } = new StorageService();
		public static ActivityService ActivityService { get; } = new ActivityService();

		public static int? SlotIndex { get; set; }


		public static World World { get; } = new World();

		public static Entity PlayerEntity
		{
			get => World.GetEntities().With<Player>().AsEnumerable().First();
		}

		public static Player Player => PlayerEntity.Get<Player>();
		public static Point PlayerPosition => PlayerEntity.Get<Location>().Position;
		public static Vector2 PlayerDisplayPosition => PlayerEntity.Get<Location>().DisplayPosition;
		public static Tile PlayerTile => Map[PlayerPosition];

		public static Map Map { get; set; }

		public static string Version
		{
			get
			{
				var assembly = typeof(TJ).GetTypeInfo().Assembly;
				var name = new AssemblyName(assembly.FullName);

				return name.Version.ToString();
			}
		}

		private static SequentialSystem<float> _systems = new SequentialSystem<float>(
			new MapUpdateSystem()
		);

		private static void WorldAct()
		{
			_systems.Update(0);

			/*			var map = Player.Map;
						foreach (var creature in map.Creatures)
						{
							creature.RegenTurn();
							var npc = creature as NonPlayer;
							if (npc == null)
							{
								continue;
							}

							// Let npcs act
							npc.Act();
						}*/
		}

		public static Entity? GetEntityByCoords(Point pos)
		{
			foreach (var entity in World.GetEntities().With<Location>().AsEnumerable())
			{
				var epos = entity.Get<Location>();

				if (epos.Position == pos)
				{
					return entity;
				}

			}

			return null;
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