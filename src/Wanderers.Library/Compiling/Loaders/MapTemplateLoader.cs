using System.Collections.Generic;
using Wanderers.Core;

namespace Wanderers.Compiling.Loaders
{
	public class MapTemplateLoader : Loader<MapTemplate>
	{
		public MapTemplateLoader() : base("MapTemplates")
		{
		}

		public override BaseObject LoadItem(CompilerContext context, string id, ObjectData data)
		{
			if (context.Module.Maps.ContainsKey(id))
			{
				RaiseError("There's already Map with id '{0}'", id);
			}

			return base.LoadItem(context, id, data);
		}
	}
}