using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Wanderers.AssetManagement
{
	public class TextureRegionAtlasLoader : IAssetLoader<TextureRegionAtlas>
	{
		private int JsonToInt(JObject o, string s)
		{
			return int.Parse(o[s].ToString());
		}

		public TextureRegionAtlas Load(AssetLoaderContext context, string assetName)
		{
			if (!assetName.EndsWith(".json"))
			{
				assetName += ".json";
			}

			var imageName = Path.ChangeExtension(assetName, ".png");

			var texture = context.Load<Texture2D>(imageName);
			var data = context.ReadAsText(assetName);

			return TextureRegionAtlas.FromJson(data, texture);
		}
	}
}