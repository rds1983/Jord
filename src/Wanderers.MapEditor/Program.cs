using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Myra.Utility;

namespace Wanderers.MapEditor
{
	static class Program
	{

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
/*				var type = typeof (Color);

				var colors = type.GetRuntimeProperties();
				var sb = new StringBuilder();

				sb.Append("{");
				sb.Append("\"colors\": {");
				foreach (var c in colors)
				{
					if (!c.GetMethod.IsStatic &&
					    c.PropertyType != typeof (Color))
					{
						continue;
					}

					var value = (Color) c.GetValue(null, null);

					var name = char.ToLowerInvariant(c.Name[0]) + c.Name.Substring(1);
					sb.Append(string.Format("\"{0}\": \"{1}\",\n", name, value.ToHexString()));
				}				
				sb.Append("}");
				sb.Append("}");
				
				File.WriteAllText("Colors.json", sb.ToString());*/
				
				var state = State.Load();
				using (var game = new Studio(state))
				{
					game.Run();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
