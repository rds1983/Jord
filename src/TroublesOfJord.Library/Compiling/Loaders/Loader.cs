using Microsoft.Xna.Framework;
using Myra;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TroublesOfJord.Core;
using TroublesOfJord.Utils;

namespace TroublesOfJord.Compiling.Loaders
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

		public void SafelyAddObject(string id, string source, JObject obj)
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
				Data = obj
			};

			_sourceData[id] = od;
		}

		public static Color EnsureColor(JObject obj, string source, string fieldName)
		{
			var s = EnsureString(obj, source, fieldName);
			var result = ColorStorage.FromName(s);
			if (result == null)
			{
				RaiseError("Could not find color '{0}'. Source: {1}", s, source);
			}

			return result.Value;
		}

		public static JToken EnsureJToken(JObject obj, string source, string fieldName)
		{
			var token = obj[fieldName];
			if (token == null)
			{
				RaiseError("Could not find mandatory '{0}' field. Source: {1}", fieldName, source);
			}

			return token;
		}

		public static T JConvertT<T>(JToken token, string source) where T: JToken
		{
			var asT = token as T;
			if (asT == null)
			{
				RaiseError("Could not cast '{0}' to '{1}'. Source: {2}", token.ToString(), typeof(T).Name, source);
			}

			return asT;
		}

		public static T EnsureT<T>(JObject obj, string source, string fieldName) where T : JToken
		{
			var token = EnsureJToken(obj, source, fieldName);
			return JConvertT<T>(token, source);
		}

		public static JArray EnsureJArray(JObject obj, string source, string fieldName)
		{
			return EnsureT<JArray>(obj, source, fieldName);
		}

		public static JObject EnsureJObject(JObject obj, string source, string fieldName)
		{
			return EnsureT<JObject>(obj, source, fieldName);
		}

		public static JObject EnsureJObject(ObjectData obj, string fieldName)
		{
			return EnsureT<JObject>(obj.Data, obj.Source, fieldName);
		}

		public static string EnsureString(JObject obj, string source, string fieldName)
		{
			var token = EnsureJToken(obj, source, fieldName);
			return token.ToString();
		}

		public static string EnsureString(ObjectData data, string fieldName)
		{
			return EnsureString(data.Data, data.Source, fieldName);
		}

		public static int StringToInt(string value, string source)
		{
			int result;
			if (!int.TryParse(value, out result))
			{
				RaiseError("Can't parse '{0}' as int value. Source: {1}", value, source);
			}

			return result;
		}

		public static int EnsureInt(JObject obj, string source, string fieldName)
		{
			var value = EnsureString(obj, source, fieldName);
			return StringToInt(value, source);
		}

		public static int EnsureInt(ObjectData data, string fieldName)
		{
			return EnsureInt(data.Data, data.Source, fieldName);
		}

		public static bool EnsureBool(JObject obj, string source, string fieldName)
		{
			var value = EnsureString(obj, source, fieldName);
			bool result;
			if (!bool.TryParse(value, out result))
			{
				RaiseError("Can't parse '{0}' as bool value. Source: {1}", value, source);
			}

			return result;
		}

		public static bool EnsureBool(ObjectData data, string fieldName)
		{
			return EnsureBool(data.Data, data.Source, fieldName);
		}

		public static Point EnsurePoint(JObject obj, string source, string fieldName)
		{
			var sizeObj = (JObject)EnsureJToken(obj, source, fieldName);

			return new Point(EnsureInt(sizeObj, source, "X"), EnsureInt(sizeObj, source, "Y"));
		}

		public static Appearance EnsureAppearance(Module module, JObject obj, string source, string fieldName)
		{
			var value = EnsureString(obj, source, fieldName);

			return module.EnsureAppearance(value);
		}

		public static Appearance EnsureAppearance(Module module, ObjectData data, string fieldName)
		{
			return EnsureAppearance(module, data.Data, data.Source, fieldName);
		}

		public static void EnsureBaseMapObject(Module module, ObjectData data, BaseMapObject output)
		{
			output.Image = EnsureAppearance(module, data, Compiler.ImageName);
			var symbolStr = EnsureString(data, "Symbol");
			if (symbolStr.Length != 1)
			{
				RaiseError("Unable to read '{0}' as symbol. Source = {1}", symbolStr, data.Source);
			}

			output.Symbol = symbolStr[0];
		}

		public static T EnsureEnum<T>(JObject obj, string source, string fieldName)
		{
			var value = EnsureString(obj, source, fieldName);

			return (T)Enum.Parse(typeof(T), value);
		}

		public static T EnsureEnum<T>(ObjectData data, string fieldName)
		{
			return EnsureEnum<T>(data.Data, data.Source, fieldName);
		}

		public static AttackInfo ParseAttack(JObject obj, string source)
		{
			return new AttackInfo(EnsureEnum<AttackType>(obj, source, "AttackType"),
				EnsureInt(obj, source, "MinDamage"),
				EnsureInt(obj, source, "MaxDamage"));
		}

		public static string EnsureId(JObject obj, string source)
		{
			return EnsureString(obj, source, Compiler.IdName);
		}

		public static string EnsureId(ObjectData data)
		{
			return EnsureId(data.Data, data.Source);
		}

		public static JToken Optional(JObject obj, string fieldName)
		{
			return obj[fieldName];
		}

		public static JObject OptionalJObject(JObject obj, string fieldName)
		{
			return (JObject)Optional(obj, fieldName);
		}

		public static string OptionalString(JObject obj, string fieldName)
		{
			var token = Optional(obj, fieldName);
			if (token == null)
			{
				return null;
			}
			return token.ToString();
		}

		public static int OptionalInt(JObject obj, string source, string fieldName, int def = 0)
		{
			var value = OptionalString(obj, fieldName);
			if (value == null)
			{
				return def;
			}

			int result;
			if (!int.TryParse(value, out result))
			{
				RaiseError("Can't parse '{0}' as int value. Source: {1}", value, source);
			}

			return result;
		}

		public static int OptionalInt(ObjectData data, string fieldName, int def = 0)
		{
			return OptionalInt(data.Data, data.Source, fieldName, def);
		}

		public static bool OptionalBool(JObject obj, string source, string fieldName, bool def)
		{
			var value = OptionalString(obj, fieldName);
			if (value == null)
			{
				return def;
			}

			bool result;
			if (!bool.TryParse(value, out result))
			{
				RaiseError("Can't parse '{0}' as bool value. Source: {1}", value, source);
			}

			return result;
		}

		public static bool OptionalBool(ObjectData data, string fieldName, bool def)
		{
			return OptionalBool(data.Data, data.Source, fieldName, def);
		}

		protected static void RaiseError(string message, params object[] args)
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

		public abstract T LoadItem(Module module, string id, ObjectData data);

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
					throw new Exception("Compilation Error. Message = '" + ex.Message + "', Id = '" + pair.Key + ", Source = '" + pair.Value.Source + "'", ex);
				}
			}
		}
	}
}