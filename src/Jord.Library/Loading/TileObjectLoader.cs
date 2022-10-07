using Jord.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Jord.Loading
{
	class TileObjectLoader: BaseMapObjectLoader<TileObject>
	{
		public static readonly TileObjectLoader Instance = new TileObjectLoader();

		protected override TileObject CreateObject(string source, JObject obj, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			var result = new TileObject
			{
				Type = obj.EnsureEnum<TileObjectType>("Type")
			};

			secondRunAction = null;

			return result;
		}
	}
}
