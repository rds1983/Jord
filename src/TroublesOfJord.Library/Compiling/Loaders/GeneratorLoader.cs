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
			BaseGenerator generator = null;

			var type = EnsureString(data, "Type");
			if (type == "Rooms")
			{
				var space = module.EnsureTileInfo(EnsureString(data, "SpaceTileId"));
				var wall = module.EnsureTileInfo(EnsureString(data, "FillerTileId"));


				generator = new RoomsGenerator(space, wall,
					EnsureInt(data, "Width"),
					EnsureInt(data, "Height"),
					EnsureInt(data, "MaximumRoomsCount"),
					EnsureInt(data, "MinimumRoomWidth"),
					EnsureInt(data, "MaximumRoomWidth"));
			}
			else
			{
				RaiseError("Could not resolve type {0}. Source = {1}", type, data.Source);
			}

			return generator;
		}
	}
}
