using Newtonsoft.Json;
using System.IO;

namespace Wanderers.Storage
{
	public static class Serialization
	{
		public static string SaveToString(object obj)
		{
			var result = JsonConvert.SerializeObject(obj, Formatting.Indented,
			                                         new JsonSerializerSettings
			                                         	{
			                                         		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
			                                         		TypeNameHandling = TypeNameHandling.Objects
			                                         	});

			return result;
		}

		public static void SaveToPath(object obj, string path)
		{
			var data = SaveToString(obj);
			File.WriteAllText(path, data);
		}

		public static T LoadFromData<T>(string data)
		{
			var result = (T)JsonConvert.DeserializeObject(data, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Objects
			});

			return result;
		}

		public static T LoadFromPath<T>(string path)
		{
			return LoadFromData<T>(File.ReadAllText(path));
		}

	}
}