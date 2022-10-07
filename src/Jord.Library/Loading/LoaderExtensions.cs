using FontStashSharp.RichText;
using Jord.Core;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace Jord.Loading
{
	internal static class LoaderExtensions
	{
		public static Color EnsureColor(this JObject obj, string fieldName)
		{
			var s = obj.EnsureString(fieldName);
			var result = ColorStorage.FromName(s);
			if (result == null)
			{
				RaiseError($"Could not find color '{s}'.");
			}

			return result.Value;
		}

		public static JToken EnsureJToken(this JObject obj, string fieldName)
		{
			var token = obj[fieldName];
			if (token == null)
			{
				RaiseError($"Could not find mandatory '{fieldName}' field.");
			}

			return token;
		}

		public static T JConvertT<T>(this JToken token) where T : JToken
		{
			var asT = token as T;
			if (asT == null)
			{
				RaiseError($"Could not cast '{token}' to '{typeof(T).Name}'.");
			}

			return asT;
		}

		public static T EnsureT<T>(this JObject obj, string fieldName) where T : JToken
		{
			var token = obj.EnsureJToken(fieldName);
			return JConvertT<T>(token);
		}

		public static JArray EnsureJArray(this JObject obj, string fieldName)
		{
			return EnsureT<JArray>(obj, fieldName);
		}

		public static JObject EnsureJObject(this JObject obj, string fieldName)
		{
			return EnsureT<JObject>(obj, fieldName);
		}

		public static string EnsureString(this JObject obj, string fieldName)
		{
			var token = obj.EnsureJToken(fieldName);
			return token.ToString();
		}

		public static T ToEnum<T>(this string value)
		{
			return (T)Enum.Parse(typeof(T), value);
		}

		public static int ToInt(this JToken value)
		{
			return value.ToString().ToInt();
		}

		public static int ToInt(this string value)
		{
			int result;
			if (!int.TryParse(value, out result))
			{
				RaiseError($"Can't parse '{value}' as int value.");
			}

			return result;
		}

		public static float ToFloat(this string value)
		{
			float result;
			if (!float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
			{
				RaiseError($"Can't parse '{value}' as float value.");
			}

			return result;
		}

		public static int EnsureInt(this JObject obj, string fieldName)
		{
			var value = obj.EnsureString(fieldName);
			return ToInt(value);
		}

		public static float EnsureFloat(this JObject obj, string fieldName)
		{
			var value = obj.EnsureString(fieldName);
			return ToFloat(value);
		}

		public static bool EnsureBool(this JObject obj, string fieldName)
		{
			var value = obj.EnsureString(fieldName);
			bool result;
			if (!bool.TryParse(value, out result))
			{
				RaiseError($"Can't parse '{value}' as bool value.");
			}

			return result;
		}

		public static Point EnsurePoint(this JObject obj, string fieldName)
		{
			var sizeObj = (JObject)obj.EnsureJToken(fieldName);

			return new Point(sizeObj.EnsureInt("X"), sizeObj.EnsureInt("Y"));
		}

		public static T EnsureEnum<T>(this JObject obj, string fieldName)
		{
			var value = obj.EnsureString(fieldName);

			return value.ToEnum<T>();
		}

		public static AttackInfo ParseAttack(this JObject obj)
		{
			return new AttackInfo(EnsureEnum<AttackType>(obj, "AttackType"),
				obj.EnsureInt("MinDamage"),
				obj.EnsureInt("MaxDamage"));
		}

		public static string EnsureId(this JObject obj)
		{
			return obj.EnsureString("Id");
		}

		public static JToken Optional(this JObject obj, string fieldName)
		{
			return obj[fieldName];
		}

		public static JObject OptionalJObject(this JObject obj, string fieldName)
		{
			return (JObject)Optional(obj, fieldName);
		}

		public static JArray OptionalJArray(this JObject obj, string fieldName)
		{
			return (JArray)Optional(obj, fieldName);
		}

		public static string OptionalString(this JObject obj, string fieldName, string def = null)
		{
			var token = Optional(obj, fieldName);
			if (token == null)
			{
				return def;
			}
			return token.ToString();
		}

		public static int? OptionalNullableInt(this JObject obj, string fieldName, int? def = 0)
		{
			var value = OptionalString(obj, fieldName);
			if (value == null)
			{
				return def;
			}

			return ToInt(value);
		}

		public static int OptionalInt(this JObject obj, string fieldName, int def = 0)
		{
			var value = OptionalString(obj, fieldName);
			if (value == null)
			{
				return def;
			}

			return ToInt(value);
		}

		public static float OptionalFloat(this JObject obj, string fieldName, float def = 0)
		{
			var value = OptionalString(obj, fieldName);
			if (value == null)
			{
				return def;
			}

			return ToFloat(value);
		}

		public static bool OptionalBool(this JObject obj, string fieldName, bool def)
		{
			var value = OptionalString(obj, fieldName);
			if (value == null)
			{
				return def;
			}

			bool result;
			if (!bool.TryParse(value, out result))
			{
				RaiseError($"Can't parse '{value}' as bool value.");
			}

			return result;
		}

		public static void RaiseError(string message)
		{
			throw new Exception(message);
		}
	}
}
