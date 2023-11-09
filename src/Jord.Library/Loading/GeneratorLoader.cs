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
			switch (type)
			{
				case "City":
					generator = new CityGenerator(data.EnsureInt("Width"), data.EnsureInt("Height"), data.EnsureInt("BuildingsCount"));
					break;
				case "Dungeon":
					generator = new DungeonGenerator(
						data.EnsureInt("Width"),
						data.EnsureInt("Height"),
						data.EnsureInt("MinimumRoomsCount"),
						data.EnsureInt("MaximumRoomsCount"),
						data.EnsureInt("MinimumRoomWidth"),
						data.EnsureInt("MaximumRoomWidth"),
						data.EnsureInt("MinimumRoomHeight"),
						data.EnsureInt("MaximumRoomHeight"));
					break;
				default:
					RaiseError($"Could not resolve type {type}.");
					break;
			}

			secondRunAction = db => SecondRun(generator, data, db);

			return generator;
		}

		private void SecondRun(BaseGenerator result, JObject data, Database database)
		{
			var asDungeonGenerator = result as DungeonGenerator;
			if (asDungeonGenerator != null)
			{
				asDungeonGenerator.Space = database.TileInfos.Ensure(data.EnsureString("SpaceTileId"));
				asDungeonGenerator.Wall = database.TileInfos.Ensure(data.EnsureString("FillerTileId"));
			}
		}
	}
}
