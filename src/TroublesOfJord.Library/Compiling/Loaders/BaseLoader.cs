using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.TextureAtlases;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TroublesOfJord.Core;
using TroublesOfJord.Core.Items;
using TroublesOfJord.Utils;
using XNAssets.Utility;

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

		private static Color EnsureColor(string id, string source)
		{
			var result = ColorStorage.FromName(id);
			if (result == null)
			{
				throw new Exception(string.Format("Could not find color '{0}'. Source: {1}", id, source));
			}

			return result.Value;
		}

		private static bool TryLoadPrimitive(Type type, JToken data, string source, out object result)
		{
			var loaded = true;
			result = null;
			if (type == typeof(string))
			{
				result = data.ToString();
			}
			else if (type == typeof(Color))
			{
				result = EnsureColor(data.ToString(), source);
			}
			else if (type.IsPrimitive)
			{
				result = Convert.ChangeType(data.ToString(), type);
			}
			else if (type.IsEnum)
			{
				result = Enum.Parse(type, data.ToString());
			} else
			{
				loaded = false;
			}

			return loaded;
		}

		public static object LoadData(Module context, Type type, string id, JObject data, string source)
		{
			// Erase Nullable
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = type.GenericTypeArguments[0];
			}

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
					var symbol = data["Symbol"].ToString();
					var color = EnsureColor(data["Color"].ToString(), source);

					var region = context.CurrentTileSet.TextureAtlas.EnsureRegion(symbol);
					var appearance = new Appearance(symbol, color, region);

					p.SetValue(item, appearance);

					continue;
				}
				if (propertyType == typeof(TextureRegionAtlas))
				{
					var textureAtlasFolder = Path.GetDirectoryName(source);
					var textureAtlasFile = Path.Combine(textureAtlasFolder, data[p.Name].ToString());

					var textureAtlasData = File.ReadAllText(textureAtlasFile);
					var textureAtlas = TextureRegionAtlas.Load(textureAtlasData,
						n =>
						{
							using (var stream = File.OpenRead(Path.Combine(textureAtlasFolder, n)))
								return  Texture2DExtensions.FromStream(MyraEnvironment.GraphicsDevice, stream, false);
						}
					);
					p.SetValue(item, textureAtlas);

					continue;
				}

				var name = p.Name;

				JToken token;
				if (!data.TryGetValue(name, out token))
				{
					var optionalFieldAttr = p.FindAttribute<OptionalFieldAttribute>();
					if (optionalFieldAttr == null)
					{
						RaiseError("Could not find mandatory field {0} for {1}, id: '{2}', source: '{3}'",
							name, propertyType.Name, id, source);
					}
					else
					{
						continue;
					}
				}

				do
				{
					object objectVal;
					if (TryLoadPrimitive(propertyType, token, source, out objectVal))
					{
						p.SetValue(item, objectVal);
						break;
					}

					var value = p.GetValue(item);
					var asList = value as IList;
					if (asList != null)
					{
						var collectionType = propertyType.GetGenericArguments()[0];
						var jarr = (JArray)token;
						foreach (JObject val in jarr)
						{
							var collectionItem = LoadData(context, collectionType, id, val, source);
							asList.Add(collectionItem);
						}

						break;
					}

					var asDict = value as IDictionary;
					if (asDict != null)
					{
						var keyType = propertyType.GetGenericArguments()[0];
						var collectionType = propertyType.GetGenericArguments()[1];
						var jarr = (JObject)token;
						foreach (var pair in jarr)
						{
							object key = pair.Key;
							if (keyType.IsEnum)
							{
								key = Enum.Parse(keyType, pair.Key.ToString());
							}

							if (TryLoadPrimitive(collectionType, pair.Value, source, out objectVal))
							{
								asDict[key] = objectVal;
							}
							else
							{
								asDict[key] = LoadData(context, collectionType, id, (JObject)pair.Value, source);
							}
						}

						break;
					}

					// Sub object
					var subObject = LoadData(context, p.Type, string.Empty, (JObject)token, source);
					p.SetValue(item, subObject);

					break;
				}
				while (true);
			}

			return item;
		}

		public static BaseObject LoadItem(Module context, Type type, string id, ObjectData data)
		{
			var item = (BaseObject)LoadData(context, type, id, data.Data, data.Source);

			item.Id = id;
			item.Source = data.Source;

			return item;
		}

		public virtual BaseObject LoadItem(Module context, string id, ObjectData data)
		{
			return LoadItem(context, Type, id, data);
		}

		public static JToken SaveObject(object obj)
		{
			var type = obj.GetType();
			if (type == typeof(string) || type.IsPrimitive || type.IsEnum)
			{
				return obj.ToString();
			}

			var asList = obj as IList;
			if (asList != null)
			{
				var arr = new JArray();
				foreach (var subItem in asList)
				{
					var subObj = SaveObject(subItem);
					arr.Add(subObj);
				}

				return arr;
			}

			var asDict = obj as IDictionary;
			if (asDict != null)
			{
				var dict = new JObject();
				foreach (DictionaryEntry pair in asDict)
				{
					dict[pair.Key.ToString()] = SaveObject(pair.Value);
				}

				return dict;
			}

			var result = new JObject();

			var members = CompilerUtils.GetMembers(type);
			foreach (var p in members)
			{
				var value = p.GetValue(obj);
				result.Add(p.Name, SaveObject(value));
			}

			return result;
		}

		protected static void RaiseError(string message, params object[] args)
		{
			throw new Exception(StringUtils.FormatMessage(message, args));
		}
	}
}