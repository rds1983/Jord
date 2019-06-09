using System;
using System.Reflection;
using Wanderers.Core;
using Wanderers.Storage;
using Module = Wanderers.Core.Module;

namespace Wanderers
{
	public static class TJ
	{
		private static readonly StorageService _storageService = new StorageService();

		public static Action<string> InfoLogHandler = Console.WriteLine;
		public static Action<string> WarnLogHandler = Console.WriteLine;
		public static Action<string> ErrorLogHandler = Console.WriteLine;

		public static Module Module { get; set; }

		public static StorageService StorageService
		{
			get
			{
				return _storageService;
			}
		}

		public static GameSession GameSession
		{
			get; set;
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

		private static string FormatMessage(string message, params object[] args)
		{
			string str;
			try
			{
				if (args != null && args.Length > 0)
				{
					str = string.Format(message, args);
				}
				else
				{
					str = message;
				}
			}
			catch (FormatException)
			{
				str = message;
			}

			return str;
		}

		public static void LogInfo(string message, params object[] args)
		{
			if (InfoLogHandler == null)
			{
				return;
			}

			InfoLogHandler(FormatMessage(message, args));
		}

		public static void LogWarn(string message, params object[] args)
		{
			if (WarnLogHandler == null)
			{
				return;
			}

			WarnLogHandler(FormatMessage(message, args));
		}

		public static void LogError(string message, params object[] args)
		{
			if (ErrorLogHandler == null)
			{
				return;
			}

			ErrorLogHandler(FormatMessage(message, args));
		}
	}
}