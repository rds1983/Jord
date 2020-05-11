using Cyotek.Drawing.BitmapFont;
using System;
using System.Linq;
using System.Xml.Linq;

namespace TroublesOfJord.FontToTextureAtlas
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
				Console.WriteLine("TroublesOfJord.FontToTextureAtlas converts BMFont to Myra TextureAtlas.");
				Console.WriteLine("Usage: ToMyraStylesheetConverter <input.fnt> <output.xml>");

				return;
			}

			try
			{
				var data = new BitmapFont();
				data.Load(args[0]);

				var xDoc = new XDocument();

				var root = new XElement("TextureAtlas");
				root.Add(new XAttribute("Image", data.Pages[0].FileName));

				var characters = data.Characters.Values.OrderBy(c => c.Char);
				foreach (var character in characters)
				{
					if (character.Char <= 32 || character.Char >= 128)
					{
						continue;
					}

					var bounds = character.Bounds;

					var element = new XElement("TextureRegion");

					element.Add(new XAttribute("Id", character.Char.ToString()));
					element.Add(new XAttribute("Left", bounds.Left));
					element.Add(new XAttribute("Top", bounds.Top));
					element.Add(new XAttribute("Width", bounds.Width));
					element.Add(new XAttribute("Height", bounds.Height));

					root.Add(element);
				}

				xDoc.Add(root);

				xDoc.Save(args[1]);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
