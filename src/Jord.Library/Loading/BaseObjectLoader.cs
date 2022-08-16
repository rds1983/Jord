using Jord.Core;
using Newtonsoft.Json.Linq;
using System;

namespace Jord.Loading
{
	public abstract class BaseObjectLoader<T> where T : BaseObject
	{
		public T LoadObject(string source, JObject data, out Action<Database> secondRunAction)
		{
			var result = CreateObject(source, data, out secondRunAction);
			result.Source = source;
			FillBaseData(data, result);

			return result;
		}

		protected abstract T CreateObject(string source, JObject data, out Action<Database> secondRunAction);

		public virtual void FillBaseData(JObject data, T output)
		{
		}

		protected static void RaiseError(string message) => LoaderExtensions.RaiseError(message);
	}
}