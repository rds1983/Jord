using System;
using System.Collections.Generic;
using Wanderers.Core.Items;
namespace Wanderers.Compiling.Loaders
{
	public class ItemLoader: Loader<BaseItemInfo>
	{
		public ItemLoader() : base("ItemInfos")
		{
		}

		private void FillItems(CompilerContext context, Dictionary<string, BaseItemInfo> output)
		{
			var assembly = GetType().Assembly;
			foreach (var pair in _sourceData)
			{
				var typeName = pair.Value.Object["type"].ToString();

				var fullTypeName = "Wanderers.Core.Items." + typeName + "Info";
				var type = assembly.GetType(fullTypeName);

				if (type == null)
				{
					throw new Exception(string.Format("Could not resolve item type '{0}'", typeName));
				}

				var props = CompilerUtils.GetMembers(type);
				var item = (BaseItemInfo)LoadItem(context, type, pair.Key, pair.Value);
				output[item.Id] = item;

				if (CompilerParams.Verbose)
				{
					TJ.LogInfo("Added to {0}, id: '{1}', value: '{2}'", JsonArrayName, item.Id, item.ToString());
				}
			}
		}
	}
}
