using Jord.Core;

namespace Jord.Serialization.Loaders
{
	class TileInfoLoader: Loader<TileInfo>
	{
		public TileInfoLoader(): base("TileInfos")
		{
		}

		public override TileInfo LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;

			var result = new TileInfo
			{
				Passable = dataObj.EnsureBool("Passable")
			};

			module.EnsureBaseMapObject(dataObj, result, "Tile" + id);

			return result;
		}
	}
}
