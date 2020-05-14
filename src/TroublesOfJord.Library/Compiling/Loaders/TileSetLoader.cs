using Myra;
using Myra.Graphics2D.TextureAtlases;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IO;
using TroublesOfJord.Core;
using XNAssets.Utility;

namespace TroublesOfJord.Compiling.Loaders
{
	class TileSetLoader : Loader<TileSet>
	{
		public TileSetLoader() : base(string.Empty)
		{
		}

		public override TileSet LoadItem(Module module, string id, ObjectData data)
		{
			var result = new TileSet();

			var textureAtlasFolder = Path.GetDirectoryName(data.Source);
			var textureAtlasFile = Path.Combine(textureAtlasFolder, EnsureString(data, "TextureAtlas"));

			var textureAtlasData = File.ReadAllText(textureAtlasFile);
			result.TextureAtlas = TextureRegionAtlas.Load(textureAtlasData,
				n =>
				{
					using (var stream = File.OpenRead(Path.Combine(textureAtlasFolder, n)))
						return Texture2DExtensions.FromStream(MyraEnvironment.GraphicsDevice, stream, false);
				}
			);

			var appearancesObj = EnsureJObject(data.Data, data.Source, "Images");
			foreach(var pair in appearancesObj)
			{
				var appearanceObj = JConvertT<JObject>(pair.Value, data.Source);

				var regionId = EnsureString(appearanceObj, data.Source, "Symbol");
				var color = EnsureColor(appearanceObj, data.Source, "Color");

				TextureRegion image;
				if (!result.TextureAtlas.Regions.TryGetValue(regionId, out image))
				{
					RaiseError("Could not find TextureRegion with id '{0}'. Source = {1}", regionId, data.Source);
				}

				var appearance = new Core.Appearance(color, image);
				result.Appearances[pair.Key] = appearance;
			}

			return result;
		}
	}
}