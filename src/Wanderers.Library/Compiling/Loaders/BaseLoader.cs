using Microsoft.Xna.Framework;
using Myra.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Wanderers.Core;
using Wanderers.Core.Items;

namespace Wanderers.Compiling.Loaders
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
				throw new Exception(string.Format(
					"Two {0} with same id '{1}' had been declared. Conflicting source: '{2}' and '{3}'",
					TypeName, id, source, od.Source));
			}

			od = new ObjectData
			{
				Source = source,
				Object = obj
			};

			_sourceData[id] = od;
		}

		public static ItemWithId LoadObject(CompilerContext context, Type type, string id, ObjectData data)
		{
			var item = (ItemWithId)Activator.CreateInstance(type);

			item.Id = id;
			var props = CompilerUtils.GetProperties(type);
			foreach (var p in props)
			{
				if (item is WeaponInfo && p.Name == "SubType")
				{
					// Special case
					continue;
				}

				if (p.PropertyType == typeof(Appearance))
				{
					// Special case
					var symbol = data.Object["Symbol"].ToString()[0];
					var color = context.EnsureColor(data.Object["Color"].ToString(), data.Source);

					var appearance = new Appearance(symbol, color);

					p.SetValue(item, appearance);

					continue;
				}

				var name = p.Name;

				JToken token;
				if (!data.Object.TryGetValue(name, out token))
				{
					var optionalFieldAttr = p.FindAttribute<OptionalFieldAttribute>();
					if (optionalFieldAttr == null)
					{
						throw new Exception(string.Format(
							"Could not find mandatory field {0} for {1}, id: '{2}', source: '{3}'",
							name, type.Name, id, data.Source));
					}
					else
					{
						continue;
					}
				}

				if (p.PropertyType == typeof(string))
				{
					p.SetValue(item, token.ToString());
				}
				else if (p.PropertyType == typeof(Color))
				{
					var c = context.EnsureColor(token.ToString(), data.Source);
					p.SetValue(item, c);
				}
				else if (p.PropertyType.IsPrimitive)
				{
					var val = Convert.ChangeType(token.ToString(), p.PropertyType);
					p.SetValue(item, val);
				}
				else if (p.PropertyType.IsEnum)
				{
					var enumValue = Enum.Parse(p.PropertyType, token.ToString());
					p.SetValue(item, enumValue);
				}
			}

			return item;
		}

		public virtual ItemWithId LoadObject(CompilerContext context, string id, ObjectData data)
		{
			return LoadObject(context, Type, id, data);
		}

		public void FillData<T>(CompilerContext context, Dictionary<string, T> output) where T : ItemWithId, new()
		{
			foreach (var pair in _sourceData)
			{
				var item = (T)LoadObject(context, pair.Key, pair.Value);
				output[item.Id] = item;

				if (CompilerParams.Verbose)
				{
					TJ.LogInfo("Added to {0}, id: '{1}', value: '{2}'", JsonArrayName, item.Id, item.ToString());
				}
			}
		}
	}
}