﻿using System.IO;

namespace Wanderers.AssetManagement
{
	public class FileAssetResolver : IAssetResolver
	{
		public string BaseFolder { get; set; }

		public FileAssetResolver()
		{
			BaseFolder = string.Empty;
		}

		public FileAssetResolver(string baseFolder)
		{
			BaseFolder = baseFolder;
		}

		public Stream Open(string assetName)
		{
			if (!string.IsNullOrEmpty(BaseFolder))
			{
				assetName = Path.Combine(BaseFolder, assetName);
			}

			return File.OpenRead(assetName);
		}
	}
}
