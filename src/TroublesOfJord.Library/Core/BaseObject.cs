using System;
using TroublesOfJord.Compiling;

namespace TroublesOfJord.Core
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
