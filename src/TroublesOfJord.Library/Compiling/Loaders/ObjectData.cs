using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TroublesOfJord.Compiling.Loaders
{
	public class ObjectData
	{
		public string Source;
		public JObject Data;
		public Dictionary<string, string> Properties;
	}
}
