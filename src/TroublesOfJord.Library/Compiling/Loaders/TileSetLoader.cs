using TroublesOfJord.Core;

namespace TroublesOfJord.Compiling.Loaders
{
	public class TileSetLoader: Loader<TileSet>
	{
		public TileSetLoader(): base("TileSets")
		{
		}

		public override BaseObject LoadItem(Module module, string id, ObjectData data)
		{
			if (module.TileSets.ContainsKey(id))
			{
				RaiseError("There's already MapTemplate with id '{0}'", id);
			}

			var tileSet = (TileSet)base.LoadItem(module, id, data);

			return tileSet;
		}
	}
}
