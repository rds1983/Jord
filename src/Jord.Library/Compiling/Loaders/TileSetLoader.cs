using Myra;
using Myra.Graphics2D.TextureAtlases;
using Newtonsoft.Json.Linq;
using System.IO;
using Jord.Core;
using XNAssets.Utility;

namespace Jord.Compiling.Loaders
{
	class TileSetLoader : Loader<TileSet>
	{
		public TileSetLoader() : base(string.Empty)
		{
		}

		public override TileSet LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;

			var result = new TileSet();

			var textureAtlasFolder = Path.GetDirectoryName(data.Source);
			var textureAtlasFile = Path.Combine(textureAtlasFolder, dataObj.EnsureString("TextureAtlas"));

			var textureAtlasData = File.ReadAllText(textureAtlasFile);
			result.TextureAtlas = TextureRegionAtlas.Load(textureAtlasData,
				n =>
				{
					using (var stream = File.OpenRead(Path.Combine(textureAtlasFolder, n)))
						return Texture2DExtensions.FromStream(MyraEnvironment.GraphicsDevice, stream, false);
				}
			);

			var appearancesObj = dataObj.EnsureJObject("Images");
			foreach (var pair in appearancesObj)
			{
				var appearanceObj = pair.Value.JConvertT<JObject>();

				var regionId = appearanceObj.EnsureString("Symbol");
				var color = appearanceObj.EnsureColor("Color");

				TextureRegion image;
				if (!result.TextureAtlas.Regions.TryGetValue(regionId, out image))
				{
					RaiseError("Could not find TextureRegion with id '{0}'.", regionId);
				}

				var appearance = new Appearance(color, image);
				result.Appearances[pair.Key] = appearance;
			}

			return result;
		}
	}
}