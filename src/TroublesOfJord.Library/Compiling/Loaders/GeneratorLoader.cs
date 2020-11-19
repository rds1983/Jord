using Newtonsoft.Json.Linq;
using TroublesOfJord.Core;
using TroublesOfJord.Generation;

namespace TroublesOfJord.Compiling.Loaders
{
	class GeneratorLoader : Loader<BaseGenerator>
	{
		public GeneratorLoader() : base("GeneratorConfigs")
		{
		}

		public override BaseGenerator LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;

			BaseGenerator generator = null;

			var type = dataObj.EnsureString("Type");
			if (type == "Rooms")
			{
				var space = module.TileInfos.Ensure(dataObj.EnsureString("SpaceTileId"));
				var wall = module.TileInfos.Ensure(dataObj.EnsureString("FillerTileId"));


				generator = new RoomsGenerator(space, wall,
					dataObj.EnsureInt("Width"),
					dataObj.EnsureInt("Height"),
					dataObj.EnsureInt("MaximumRoomsCount"),
					dataObj.EnsureInt("MinimumRoomWidth"),
					dataObj.EnsureInt("MaximumRoomWidth"));
			}
			else
			{
				RaiseError("Could not resolve type {0}.", type);
			}

			return generator;
		}
	}
}
