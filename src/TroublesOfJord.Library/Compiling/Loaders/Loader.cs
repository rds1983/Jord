using System;
using System.Collections.Generic;
using TroublesOfJord.Core;

namespace TroublesOfJord.Compiling.Loaders
{
	public class Loader<T>: BaseLoader where T: BaseObject
	{
		public override string TypeName => typeof(T).Name;
		public override Type Type => typeof(T);

		public Loader(string jsonArrayName): base(jsonArrayName)
		{
		}

		public virtual void FillData(CompilerContext context, Dictionary<string, T> output)
		{
			foreach (var pair in _sourceData)
			{
				var item = (T)LoadItem(context, pair.Key, pair.Value);
				output[item.Id] = item;

				if (CompilerParams.Verbose)
				{
					TJ.LogInfo("Added to {0}, id: '{1}', value: '{2}'", JsonArrayName, item.Id, item.ToString());
				}
			}
		}
	}
}
