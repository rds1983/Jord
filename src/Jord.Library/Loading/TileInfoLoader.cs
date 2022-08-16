using Jord.Core;
using Newtonsoft.Json.Linq;
using System;

namespace Jord.Loading
{
	class TileInfoLoader: BaseMapObjectLoader<TileInfo>
	{
		public static readonly TileInfoLoader Instance = new TileInfoLoader();

		protected override TileInfo CreateObject(string source, JObject data, out Action<Database> secondRunAction)
		{
			var result = new TileInfo
			{
				Passable = data.EnsureBool("Passable")
			};

			secondRunAction = null;

			return result;
		}
	}
}
