using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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

		public static object LoadObject(CompilerContext context, Type type, string id, ObjectData data)
		{
			var item = Activator.CreateInstance(type);
			var members = CompilerUtils.GetMembers(type);
			foreach (var p in members)
			{
				if (item is WeaponInfo && p.Name == "SubType")
				{
					// Special case
					continue;
				}

				Type propertyType = p.Type;
				if (propertyType == typeof(Appearance))
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

				if (propertyType == typeof(string))
				{
					p.SetValue(item, token.ToString());
				}
				else if (propertyType == typeof(Color))
				{
					var c = context.EnsureColor(token.ToString(), data.Source);
					p.SetValue(item, c);
				}
				else if (propertyType.IsPrimitive)
				{
					var val = Convert.ChangeType(token.ToString(), propertyType);
					p.SetValue(item, val);
				}
				else if (propertyType.IsEnum)
				{
					var enumValue = Enum.Parse(propertyType, token.ToString());
					p.SetValue(item, enumValue);
				} else
				{
					var value = p.GetValue(item);
					var asList = value as IList;
					if (asList != null)
					{
						var collectionType = propertyType.GetGenericArguments()[0];
						var jarr = (JArray)token;
						foreach(JObject val in jarr)
						{
							var collectionItem = LoadObject(context, collectionType, id, new ObjectData
							{
								Object = val,
								Source = data.Source
							});

							asList.Add(collectionItem);
						}
					}
				}
			}

			return item;
		}

		public static ItemWithId LoadItem(CompilerContext context, Type type, string id, ObjectData data)
		{
			var item = (ItemWithId)LoadObject(context, type, id, data);

			item.Id = id;

			return item;
		}

		public virtual ItemWithId LoadItem(CompilerContext context, string id, ObjectData data)
		{
			return LoadItem(context, Type, id, data);
		}

		public void FillData<T>(CompilerContext context, Dictionary<string, T> output) where T : ItemWithId, new()
		{
			foreach (var pair in _sourceData)
			{
				var item = (T)LoadItem(context, pair.Key, pair.Value);
				output[item.Id] = item;

				if (CompilerParams.Verbose)
				{
					TJ.LogInfo("Added to {0}, id: '{1}', value: '{2}'", JsonArrayName, item.Id, item.ToString());
				}
			}
		}
	}
}