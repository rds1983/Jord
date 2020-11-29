using Cyotek.Drawing.BitmapFont;
using StbImageSharp;
using System;
using System.IO;
using System.Linq;

namespace Jord.FontToTextureAtlas
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Jord.FontToTextureAtlas converts BMFont to LibGDX TextureAtlas.");
				Console.WriteLine("Usage: ToMyraStylesheetConverter <input.fnt> <output.atlas>");

				return;
			}

			try
			{
				var data = new BitmapFont();
				data.Load(args[0]);

				ImageResult imageResult = null;
				using (var stream = File.OpenRead(data.Pages[0].FileName))
				{
					imageResult = ImageResult.FromStream(stream);
				}

				using(var stream = File.OpenWrite(args[1]))
				using(var writer = new StreamWriter(stream))
				{
					var page = data.Pages[0];
					writer.WriteLine();
					writer.WriteLine(page.FileName);
					writer.WriteLine("size: {0},{1}", imageResult.Width, imageResult.Height);
					writer.WriteLine("format: RGBA8888");
					writer.WriteLine("filter: Nearest,Nearest");
					writer.WriteLine("repeat: none");

					var characters = data.Characters.Values.OrderBy(c => c.Char);
					foreach (var character in characters)
					{
						if (character.Char <= 32 || character.Char >= 128)
						{
							continue;
						}

						var bounds = character.Bounds;

						writer.WriteLine(character.Char.ToString());
						writer.WriteLine("  rotate: false");
						writer.WriteLine("  xy: {0},{1}", bounds.X, bounds.Y);
						writer.WriteLine("  size: {0},{1}", bounds.Width, bounds.Height);
						writer.WriteLine("  orig: {0},{1}", bounds.Width, bounds.Height);
						writer.WriteLine("  offset: 0, 0");
						writer.WriteLine("  index: -1");
					}

					writer.WriteLine();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
