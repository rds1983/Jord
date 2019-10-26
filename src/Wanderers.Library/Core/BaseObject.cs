using System;
using Wanderers.Compiling;

namespace Wanderers.Core
{
	public class BaseObject
	{
		[IgnoreField]
		public string Id;

		[IgnoreField]
		public string Source;

		public override string ToString()
		{
			return Id;
		}
	}
}
