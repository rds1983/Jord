using System;
using System.Collections.Generic;
using System.Text;
using TroublesOfJord.Compiling;

namespace TroublesOfJord.Core.Items
{
	public class BaseItemInfo : BaseObject
	{
		private static readonly Dictionary<Type, string> _typeDisplayNames = new Dictionary<Type, string>();

		public string Name
		{
			get; set;
		}

		public int Price
		{
			get; set;
		}

		[IgnoreField]
		public string TypeDisplayName
		{
			get
			{
				return GetTypeDisplayName(GetType());
			}
		}

		public virtual string BuildDescription()
		{
			return TypeDisplayName;
		}

		private static string GetTypeDisplayName(Type type)
		{
			string result;
			if (_typeDisplayNames.TryGetValue(type, out result))
			{
				return result;
			}

			var sb = new StringBuilder();
			var input = type.Name.Replace("Info", string.Empty);

			for (var i = 0; i < input.Length; ++i)
			{
				var c = input[i];

				if (i > 0 && char.IsUpper(c))
				{
					sb.Append(" ");
				}

				sb.Append(char.ToLower(c));
			}

			_typeDisplayNames[type] = sb.ToString();

			return sb.ToString();
		}
	}
}
