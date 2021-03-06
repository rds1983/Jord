﻿using Microsoft.Xna.Framework;
using Myra;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using Jord.Core;
using Jord.Utils;

namespace Jord.Compiling.Loaders
{
	public abstract class BaseLoader
	{
		protected readonly Dictionary<string, ObjectData> _sourceData = new Dictionary<string, ObjectData>();

		public abstract string TypeName { get; }
		public string JsonArrayName { get; private set; }
		public abstract Type Type { get; }

		public BaseLoader(string jsonArrayName)
		{
			JsonArrayName = jsonArrayName;
		}

		public void SafelyAddObject(string id, string source, JObject obj, Dictionary<string, string> properties = null)
		{
			ObjectData od;
			if (_sourceData.TryGetValue(id, out od))
			{
				RaiseError("Two {0} with same id '{1}' had been declared. Conflicting source: '{2}' and '{3}'",
					TypeName, id, source, od.Source);
			}

			od = new ObjectData
			{
				Source = source,
				Data = obj,
				Properties = properties
			};

			_sourceData[id] = od;
		}

		protected static void RaiseError(string message, params object[] args)
		{
			LoaderExtensions.RaiseError(message, args);
		}
	}

	internal static class LoaderExtensions
	{
		public static Color EnsureColor(this JObject obj, string fieldName)
		{
			var s = obj.EnsureString(fieldName);
			var result = ColorStorage.FromName(s);
			if (result == null)
			{
				RaiseError("Could not find color '{0}'.", s);
			}

			return result.Value;
		}

		public static JToken EnsureJToken(this JObject obj, string fieldName)
		{
			var token = obj[fieldName];
			if (token == null)
			{
				RaiseError("Could not find mandatory '{0}' field.", fieldName);
			}

			return token;
		}

		public static T JConvertT<T>(this JToken token) where T : JToken
		{
			var asT = token as T;
			if (asT == null)
			{
				RaiseError("Could not cast '{0}' to '{1}'.", token.ToString(), typeof(T).Name);
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
				RaiseError("Can't parse '{0}' as int value.", value);
			}

			return result;
		}

		public static float ToFloat(this string value)
		{
			float result;
			if (!float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
			{
				RaiseError("Can't parse '{0}' as float value.", value);
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
				RaiseError("Can't parse '{0}' as bool value.", value);
			}

			return result;
		}

		public static Point EnsurePoint(this JObject obj, string fieldName)
		{
			var sizeObj = (JObject)obj.EnsureJToken(fieldName);

			return new Point(sizeObj.EnsureInt("X"), sizeObj.EnsureInt("Y"));
		}

		public static void EnsureBaseMapObject(this Module module, JObject obj, BaseMapObject output, string defaultImageName)
		{
			var imageName = obj.OptionalString(Compiler.ImageName, defaultImageName);
			output.Image = module.Appearances.Ensure(imageName);
			var symbolStr = obj.EnsureString("Symbol");
			if (symbolStr.Length != 1)
			{
				RaiseError("Unable to read '{0}' as symbol.", symbolStr);
			}

			output.Symbol = symbolStr[0];
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
			return obj.EnsureString(Compiler.IdName);
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
				RaiseError("Can't parse '{0}' as bool value.", value);
			}

			return result;
		}

		public static void RaiseError(string message, params object[] args)
		{
			throw new Exception(StringUtils.FormatMessage(message, args));
		}
	}

	public abstract class Loader<T> : BaseLoader where T : IBaseObject
	{
		public override string TypeName => typeof(T).Name;
		public override Type Type => typeof(T);

		public Loader(string jsonArrayName) : base(jsonArrayName)
		{
		}

		public abstract T LoadItem(Module module, string id, ObjectData od);

		public void FillData(Module module, Dictionary<string, T> output)
		{
			foreach (var pair in _sourceData)
			{
				try
				{
					var item = LoadItem(module, pair.Key, pair.Value);

					item.Id = pair.Key;
					item.Source = pair.Value.Source;
					output[item.Id] = item;

					if (CompilerParams.Verbose)
					{
						TJ.LogInfo("Added to {0}, id: '{1}', value: '{2}'", JsonArrayName, item.Id, item.ToString());
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Compilation Error. Message = '" + ex.Message + "', Id = '" + pair.Key + "', Source = '" + pair.Value.Source + "'", ex);
				}
			}
		}
	}
}