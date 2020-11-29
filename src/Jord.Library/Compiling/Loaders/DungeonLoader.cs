using Newtonsoft.Json.Linq;
using Jord.Core;

namespace Jord.Compiling.Loaders
{
	class DungeonLoader : Loader<Dungeon>
	{
		public DungeonLoader() : base("Dungeons")
		{
		}

		public override Dungeon LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;

			if (module.Maps.ContainsKey(id))
			{
				RaiseError("There's already Map with id '{0}'", id);
			}

			var result = new Dungeon
			{
				GeneratorId = dataObj.EnsureString("GeneratorId"),
				Name = dataObj.EnsureString("Name"),
				Levels = dataObj.EnsureInt("Levels")
			};

			var exitObj = dataObj.EnsureJObject("Exit");
			result.Exit = new Exit
			{
				MapId = exitObj.EnsureString("MapId"),
				TileInfoId = exitObj.EnsureString("TileInfoId"),
			};

			if(result.Levels > 1)
			{
				result.ExitDownTileInfoId = dataObj.EnsureString("ExitDownTileInfoId");
			}

			return result;
		}
	}
}