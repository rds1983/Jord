namespace Wanderers.Utils
{
	public static class Strings
	{
		public static string LowercaseFirstLetter(this string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}

			return char.ToLowerInvariant(s[0]) + s.Substring(1);
		}

		public static string UppercaseFirstLetter(this string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}

			return char.ToUpperInvariant(s[0]) + s.Substring(1);
		}
	}
}