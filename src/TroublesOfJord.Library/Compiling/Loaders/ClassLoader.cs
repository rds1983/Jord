using TroublesOfJord.Core;

namespace TroublesOfJord.Compiling.Loaders
{
	class ClassLoader: Loader<Class>
	{
		public ClassLoader(): base("Classes")
		{
		}

		public override Class LoadItem(Module module, string id, ObjectData data)
		{
			return new Class
			{
				Name = EnsureString(data, Compiler.NameName)
			};
		}
	}
}
