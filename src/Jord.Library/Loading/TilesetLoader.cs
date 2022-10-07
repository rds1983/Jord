using Jord.Core;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Jord.Loading
{
	class TilesetLoader : BaseObjectLoader<Tileset>
	{
		public static readonly TilesetLoader Instance = new TilesetLoader();

		protected override Tileset CreateObject(string source, JObject data, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			secondRunAction = null;

			var result = new Tileset
			{
				Id = data.EnsureId(),
				Width = data.EnsureInt("Width"),
				Height = data.EnsureInt("Height"),
			};

			var folder = Path.GetDirectoryName(source);
			var atlasFile = Path.ChangeExtension(source, "xmat");

			result.TextureAtlas = TextureRegionAtlas.FromXml(File.ReadAllText(atlasFile),
				fileName =>
				{
					var path = Path.Combine(folder, fileName);

					using (var stream = File.OpenRead(path))
					{
						return Texture2D.FromStream(TJ.GraphicsDevice, stream);
					}
				});

			var tileImages = (JObject)data["TileImages"];
			foreach(var pair in tileImages)
			{
				result.TileImages[pair.Key] = result.TextureAtlas[pair.Value.ToString()];
			}

			return result;
		}
	}
}
