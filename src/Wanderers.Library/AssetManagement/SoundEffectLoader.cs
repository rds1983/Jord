using Microsoft.Xna.Framework.Audio;

namespace Wanderers.AssetManagement
{
	public class SoundEffectLoader: IAssetLoader<SoundEffect>
	{
		public SoundEffect Load(AssetLoaderContext context, string assetName)
		{
			using (var stream = context.Open(assetName))
			{
				return SoundEffect.FromStream(stream);
			}
		}
	}
}
