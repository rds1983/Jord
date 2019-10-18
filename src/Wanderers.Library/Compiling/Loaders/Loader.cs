using Wanderers.Core;

namespace Wanderers.Compiling
{
	public class Loader<T>: BaseLoader where T: ItemWithId
	{
		public override string TypeName => typeof(T).Name;
		public override string JsonArrayName => TypeName + "s";
	}
}
