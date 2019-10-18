using Myra.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wanderers.Compiling
{
	public static class CompilerUtils
	{
		public const string IdName = "Id";
		public const string MapName = "Map";

		private static readonly Dictionary<Type, List<PropertyInfo>> _propsCache = new Dictionary<Type, List<PropertyInfo>>();

		public static List<PropertyInfo> GetProperties(Type type)
		{
			List<PropertyInfo> result;
			if (_propsCache.TryGetValue(type, out result))
			{
				return result;
			}

			var props = new List<PropertyInfo>();
			var properties = from p in type.GetProperties() select p;
			foreach (var property in properties)
			{
				if (property.GetGetMethod() == null ||
					!property.GetGetMethod().IsPublic ||
					property.GetGetMethod().IsStatic ||
					property.GetSetMethod() == null ||
					!property.GetSetMethod().IsPublic)
				{
					continue;
				}

				var ignoreFieldAttr = property.FindAttribute<IgnoreFieldAttribute>();
				if (ignoreFieldAttr != null)
				{
					continue;
				}

				props.Add(property);
			}

			_propsCache[type] = props;

			return props;
		}

		public static List<PropertyInfo> GetProperties<T>()
		{
			return GetProperties(typeof(T));
		}
	}
}
