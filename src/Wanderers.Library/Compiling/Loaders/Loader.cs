using System;
using Wanderers.Core;

namespace Wanderers.Compiling.Loaders
{
	public class Loader<T>: BaseLoader where T: ItemWithId
	{
		public override string TypeName => typeof(T).Name;
		public override Type Type => typeof(T);

		public Loader(string jsonArrayName): base(jsonArrayName)
		{
		}
	}
}
