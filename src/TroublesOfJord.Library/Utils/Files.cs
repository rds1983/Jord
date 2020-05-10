using System.IO;

namespace TroublesOfJord.Utils
{
	public static class Files
	{
		public static string ExecutableFolder
		{
			get
			{
				return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			}
		}
	}
}