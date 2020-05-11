using System;
using System.Collections.Generic;
using TroublesOfJord.Core;
using TroublesOfJord.Generation;

namespace TroublesOfJord.Compiling.Loaders
{
	public class GeneratorLoader : Loader<BaseGenerator>
	{
		public GeneratorLoader() : base("GeneratorConfigs")
		{
		}

		public override void FillData(Module module, Dictionary<string, BaseGenerator> output)
		{
			var assembly = GetType().Assembly;
			foreach (var pair in _sourceData)
			{
				var typeName = pair.Value.Data["Type"].ToString();

				var fullTypeName = "TroublesOfJord.Generation." + typeName + "Generator";
				var type = assembly.GetType(fullTypeName);

				if (type == null)
				{
					throw new Exception(string.Format("Could not resolve item type '{0}'", typeName));
				}

				var props = CompilerUtils.GetMembers(type);
				var item = (BaseGenerator)LoadItem(module, type, pair.Key, pair.Value);
				output[item.Id] = item;

				if (CompilerParams.Verbose)
				{
					TJ.LogInfo("Added to {0}, id: '{1}', value: '{2}'", JsonArrayName, item.Id, item.ToString());
				}
			}
		}
	}
}
