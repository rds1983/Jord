using System.IO;

namespace Wanderers.Utils
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