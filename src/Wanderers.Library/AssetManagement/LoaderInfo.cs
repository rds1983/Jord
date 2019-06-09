﻿namespace Wanderers.AssetManagement
{
	public class LoaderInfo
	{
		public object Loader { get; private set; }
		public bool StoreInCache { get; set; }

		internal LoaderInfo(object loader, bool storeInCache = true)
		{
			Loader = loader;
			StoreInCache = storeInCache;
		}
	}
}
