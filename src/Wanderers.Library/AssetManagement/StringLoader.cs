namespace Wanderers.AssetManagement
{
	public class StringLoader: IAssetLoader<string>
	{
		public string Load(AssetLoaderContext context, string assetName)
		{
			return context.ReadAsText(assetName);
		}
	}
}
