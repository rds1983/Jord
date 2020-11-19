using System;
using System.Globalization;

namespace TroublesOfJord.Utils
{
	public static class StringUtils
	{
		public static string FormatMessage(string message, params object[] args)
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

		public static string Format(this float f)
		{
			return f.ToString("0.##", CultureInfo.InvariantCulture);
		}
	}
}
