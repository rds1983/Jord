using System.IO;

namespace Wanderers.AssetManagement
{
	public interface IAssetResolver
	{
		Stream Open(string assetName);
	}
}
