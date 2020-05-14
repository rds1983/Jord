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
			BaseGenerator result = null;

			var type = EnsureString(data, "Type");
			if (type == "Rooms")
			{
				var roomsConfig = new RoomsGenerator
				{
					MinimumRoomsCount = EnsureInt(data, "MinimumRoomsCount"),
					MaximumRoomsCount = EnsureInt(data, "MaximumRoomsCount"),
					MinimumRoomWidth = EnsureInt(data, "MinimumRoomWidth"),
					MaximumRoomWidth = EnsureInt(data, "MaximumRoomWidth"),
					MinimumRoomHeight = EnsureInt(data, "MinimumRoomHeight"),
					MaximumRoomHeight = EnsureInt(data, "MaximumRoomHeight"),
					RoomBorderSize = EnsureInt(data, "RoomBorderSize"),
					FillerTileId = EnsureString(data, "FillerTileId"),
					SpaceTileId = EnsureString(data, "SpaceTileId"),
				};

				result = roomsConfig;
			}
			else
			{
				RaiseError("Could not resolve type {0}. Source = {1}", type, data.Source);
			}

			result.Width = EnsureInt(data, "Width");
			result.Height = EnsureInt(data, "Height");

			return result;
		}
	}
}
