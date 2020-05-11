using TroublesOfJord.Core;

namespace TroublesOfJord.Compiling.Loaders
{
	public class TileSetLoader: Loader<TileSet>
	{
		public TileSetLoader(): base("TileSets")
		{
		}

		public override BaseObject LoadItem(CompilerContext context, string id, ObjectData data)
		{
			if (context.Module.TileSets.ContainsKey(id))
			{
				RaiseError("There's already MapTemplate with id '{0}'", id);
			}

			var tileSet = (TileSet)base.LoadItem(context, id, data);


			return tileSet;
		}
	}
}
