using Jord.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Jord.Loading
{
	class TileInfoLoader: BaseMapObjectLoader<TileInfo>
	{
		public static readonly TileInfoLoader Instance = new TileInfoLoader();

		protected override TileInfo CreateObject(string source, JObject data, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			var result = new TileInfo
			{
				Passable = data.EnsureBool("Passable")
			};

			result.Transparent = data.OptionalBool("Transparent", result.Passable);

			secondRunAction = null;

			return result;
		}
	}
}
