using Jord.Core;

namespace Jord.Data.Loaders
{
	class TileObjectLoader: Loader<TileObject>
	{
		public TileObjectLoader() : base("TileObjects")
		{
		}

		public override TileObject LoadItem(Module module, string id, ObjectData od)
		{
			var dataObj = od.Data;

			var result = new TileObject
			{
				Type = dataObj.EnsureEnum<TileObjectType>("Type")
			};

			module.EnsureBaseMapObject(dataObj, result, "TileObject" + id);

			return result;
		}
	}
}
