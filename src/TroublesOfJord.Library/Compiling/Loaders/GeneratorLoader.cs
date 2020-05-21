using RogueSharp.MapCreation;
using System.Runtime.Remoting.Messaging;
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
			IMapCreationStrategy<Map, Tile> strategy = null;

			var type = EnsureString(data, "Type");
			if (type == "Rooms")
			{
				var space = module.EnsureTileInfo(EnsureString(data, "SpaceTileId"));
				var wall = module.EnsureTileInfo(EnsureString(data, "FillerTileId"));


				strategy = new RandomRoomsMapCreationStrategy<Map, Tile>(space, wall,
					EnsureInt(data, "Width"),
					EnsureInt(data, "Height"),
					EnsureInt(data, "MaximumRoomsCount"),
					EnsureInt(data, "MaximumRoomWidth"),
					EnsureInt(data, "MinimumRoomWidth"));
			}
			else
			{
				RaiseError("Could not resolve type {0}. Source = {1}", type, data.Source);
			}

			var result = new BaseGenerator
			{
				MapCreationStrategy = strategy
			};

			return result;
		}
	}
}
