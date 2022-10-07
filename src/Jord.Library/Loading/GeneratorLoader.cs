using Jord.Core;
using Jord.Generation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Jord.Loading
{
	class GeneratorLoader : BaseObjectLoader<BaseGenerator>
	{
		public static readonly GeneratorLoader Instance = new GeneratorLoader();

		protected override BaseGenerator CreateObject(string source, JObject data, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			BaseGenerator generator = null;

			var type = data.EnsureString("Type");
			if (type == "Rooms")
			{
				generator = new RoomsGenerator(
					data.EnsureInt("Width"),
					data.EnsureInt("Height"),
					data.EnsureInt("MaximumRoomsCount"),
					data.EnsureInt("MinimumRoomWidth"),
					data.EnsureInt("MaximumRoomWidth"));
			}
			else
			{
				RaiseError($"Could not resolve type {type}.");
			}

			secondRunAction = db => SecondRun(generator, data, db);

			return generator;
		}

		private void SecondRun(BaseGenerator result, JObject data, Database database)
		{
			var asRoomsGenerator = result as RoomsGenerator;
			if (asRoomsGenerator != null)
			{
				asRoomsGenerator.Space = database.TileInfos.Ensure(data.EnsureString("SpaceTileId"));
				asRoomsGenerator.Wall = database.TileInfos.Ensure(data.EnsureString("FillerTileId"));
			}
		}
	}
}
