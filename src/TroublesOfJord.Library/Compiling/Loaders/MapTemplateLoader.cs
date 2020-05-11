using TroublesOfJord.Core;

namespace TroublesOfJord.Compiling.Loaders
{
	public class MapTemplateLoader : Loader<MapTemplate>
	{
		public MapTemplateLoader() : base("MapTemplates")
		{
		}

		public override BaseObject LoadItem(Module module, string id, ObjectData data)
		{
			if (module.Maps.ContainsKey(id))
			{
				RaiseError("There's already Map with id '{0}'", id);
			}

			return base.LoadItem(module, id, data);
		}
	}
}