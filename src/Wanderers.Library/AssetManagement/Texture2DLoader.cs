using Microsoft.Xna.Framework.Graphics;

namespace Wanderers.AssetManagement
{
	public class Texture2DLoader : IAssetLoader<Texture2D>
	{
		public Texture2D Load(AssetLoaderContext context, string assetName)
		{
			using (var stream = context.Open(assetName))
			{
				return Texture2D.FromStream(context.GraphicsDevice, stream);
			}
		}
	}
}