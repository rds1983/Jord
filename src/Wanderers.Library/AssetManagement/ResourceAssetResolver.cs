﻿using System;
using System.IO;
using System.Reflection;

namespace Wanderers.AssetManagement
{
	public class ResourceAssetResolver : IAssetResolver
	{
		private Assembly _assembly;

		public Assembly Assembly
		{
			get { return _assembly; }

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_assembly = value;
			}
		}

		public string Prefix { get; set; }

		public ResourceAssetResolver(Assembly assembly, string prefix)
		{
			Assembly = assembly;
			Prefix = prefix;
		}

		public Stream Open(string assetName)
		{
			// Once you figure out the name, pass it in as the argument here.
			var stream = Assembly.GetManifestResourceStream(Prefix + assetName);

			return stream;
		}
	}
}
