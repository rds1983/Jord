using Jord.Core;
using Newtonsoft.Json.Linq;

namespace Jord.Loading
{
	abstract class BaseMapObjectLoader<T> : BaseObjectLoader<T> where T: BaseMapObject
	{
		public override void FillBaseData(JObject data, T output)
		{
			base.FillBaseData(data, output);

			var symbolStr = data.EnsureString("Symbol");
			if (symbolStr.Length != 1)
			{
				RaiseError($"Unable to read '{symbolStr}' as symbol.");
			}

			var color = data.EnsureColor("Color");

			output.Image = new Appearance(symbolStr, color, null);
			output.Symbol = symbolStr[0];
		}
	}
}
