using Newtonsoft.Json.Linq;
using TroublesOfJord.Core;

namespace TroublesOfJord.Compiling.Loaders
{
	class MapTemplateLoader : Loader<MapTemplate>
	{
		public MapTemplateLoader() : base("MapTemplates")
		{
		}

		public override MapTemplate LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;

			if (module.Maps.ContainsKey(id))
			{
				RaiseError("There's already Map with id '{0}'", id);
			}

			var result = new MapTemplate
			{
				GeneratorId = dataObj.EnsureString("GeneratorId")
			};

			result.Name = dataObj.EnsureString("Name");

			var exits = dataObj.EnsureJArray("Exits");
			foreach(JObject exitObj in exits)
			{
				var exit = new Exit
				{
					MapId = exitObj.EnsureString("MapId"),
					ExitMapId = exitObj.EnsureString("ExitMapId"),
					TileInfoId = exitObj.EnsureString("TileInfoId"),
				};

				result.Exits.Add(exit);
			}

			var creatures = dataObj.EnsureJObject("Creatures");
			foreach(var pair in creatures)
			{
				result.Creatures[pair.Key] = int.Parse(pair.Value.ToString());
			}

			return result;
		}
	}
}