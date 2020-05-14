using TroublesOfJord.Core;

namespace TroublesOfJord.Compiling.Loaders
{
	class TileInfoLoader: Loader<TileInfo>
	{
		public TileInfoLoader(): base("TileInfos")
		{
		}

		public override TileInfo LoadItem(Module module, string id, ObjectData data)
		{
			var result = new TileInfo
			{
				Passable = EnsureBool(data, "Passable")
			};

			EnsureBaseMapObject(module, data, result);

			return result;
		}
	}
}
