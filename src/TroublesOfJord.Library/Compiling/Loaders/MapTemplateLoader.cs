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
			if (module.Maps.ContainsKey(id))
			{
				RaiseError("There's already Map with id '{0}'", id);
			}

			var result = new MapTemplate
			{
				GeneratorId = EnsureString(data, "GeneratorId")
			};

			result.Name = EnsureString(data, "Name");

			var exits = EnsureJArray(data.Data, data.Source, "Exits");
			foreach(JObject exitObj in exits)
			{
				var exit = new Exit
				{
					MapId = EnsureString(exitObj, data.Source, "MapId"),
					ExitMapId = EnsureString(exitObj, data.Source, "ExitMapId"),
					TileInfoId = EnsureString(exitObj, data.Source, "TileInfoId"),
				};

				result.Exits.Add(exit);
			}

			var creatures = EnsureJObject(data.Data, data.Source, "Creatures");
			foreach(var pair in creatures)
			{
				result.Creatures[pair.Key] = int.Parse(pair.Value.ToString());
			}

			return result;
		}
	}
}