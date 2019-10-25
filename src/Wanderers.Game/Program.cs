using System;
using System.Text.RegularExpressions;

namespace Wanderers
{
	internal class Program
	{
		private static readonly Regex ArgumentRegex = new Regex(@"/(\w+):(\w+)");

		public static void Main(string[] args)
		{
			try
			{
				int? startGameIndex = null;
				foreach (var arg in args)
				{
					var match = ArgumentRegex.Match(arg);
					if (match != null && match.Success)
					{
						var command = match.Groups[1].Value.ToLower();
						var value = match.Groups[2].Value;

						switch (command)
						{
							case "start":
								startGameIndex = int.Parse(value);
								break;
						}
					}
				}

				using (var game = new WanderersGame())
				{
					game.StartGameIndex = startGameIndex;
					game.Run();
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}