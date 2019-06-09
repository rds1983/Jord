using System;

namespace Wanderers.Generator
{
	public class BaseGenerator
	{
		private readonly GenerationContext _context;

		public BaseGenerator(GenerationContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			_context = context;
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

		public void LogInfo(string message, params object[] args)
		{
			if (_context.InfoHandler == null)
			{
				return;
			}

			_context.InfoHandler(FormatMessage(message, args));
		}
	}
}
