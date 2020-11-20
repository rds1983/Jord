using Newtonsoft.Json.Linq;
using TroublesOfJord.Core;

namespace TroublesOfJord.Compiling.Loaders
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

			var exits = dataObj.EnsureJArray("Exits");
			foreach(JObject exitObj in exits)
			{
				var exit = new Exit
				{
					MapId = exitObj.EnsureString("MapId"),
					TileInfoId = exitObj.EnsureString("TileInfoId"),
				};

				result.Exits.Add(exit);
			}

			return result;
		}
	}
}