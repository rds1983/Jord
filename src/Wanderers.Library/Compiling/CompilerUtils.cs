using Myra.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wanderers.Compiling
{
	public interface IMemberWrapper
	{
		string Name { get; }
		Type Type { get; }
		object GetValue(object obj);
		void SetValue(object obj, object value);
		T FindAttribute<T>() where T : Attribute;
	}

	public static class CompilerUtils
	{
		public abstract class MemberWrapperT<T>: IMemberWrapper where T : MemberInfo
		{
			protected readonly T _info;

			public string Name => _info.Name;

			public abstract Type Type { get; }

			public MemberWrapperT(T info)
			{
				_info = info;
			}

			public abstract object GetValue(object obj);
			public abstract void SetValue(object obj, object value);
			public abstract T2 FindAttribute<T2>() where T2 : Attribute;
		}

		public class PropertyWrapper: MemberWrapperT<PropertyInfo>
		{
			public override Type Type => _info.PropertyType;

			public PropertyWrapper(PropertyInfo info) : base(info)
			{
			}

			public override object GetValue(object obj)
			{
				return _info.GetValue(obj);
			}

			public override void SetValue(object obj, object value)
			{
				_info.SetValue(obj, value);
			}

			public override T FindAttribute<T>()
			{
				return _info.FindAttribute<T>();
			}
		}

		public class FieldWrapper : MemberWrapperT<FieldInfo>
		{
			public override Type Type => _info.FieldType;

			public FieldWrapper(FieldInfo info): base(info)
			{
			}

			public override object GetValue(object obj)
			{
				return _info.GetValue(obj);
			}

			public override void SetValue(object obj, object value)
			{
				_info.SetValue(obj, value);
			}

			public override T FindAttribute<T>()
			{
				return _info.FindAttribute<T>();
			}
		}

		public const string IdName = "Id";
		public const string MapName = "Map";

		private static readonly Dictionary<Type, List<IMemberWrapper>> _membersCache = new Dictionary<Type, List<IMemberWrapper>>();

		public static List<IMemberWrapper> GetMembers(Type type)
		{
			List<IMemberWrapper> result;
			if (_membersCache.TryGetValue(type, out result))
			{
				return result;
			}

			result = new List<IMemberWrapper>();
			var properties = from p in type.GetProperties() select p;
			foreach (var property in properties)
			{
				if (property.GetGetMethod() == null ||
					!property.GetGetMethod().IsPublic ||
					property.GetGetMethod().IsStatic)
				{
					continue;
				}

				var ignoreFieldAttr = property.FindAttribute<IgnoreFieldAttribute>();
				if (ignoreFieldAttr != null)
				{
					continue;
				}

				result.Add(new PropertyWrapper(property));
			}

			var fields = from p in type.GetFields() select p;
			foreach (var field in fields)
			{
				if (!field.IsPublic ||
					field.IsStatic ||
					field.IsInitOnly)
				{
					continue;
				}

				var ignoreFieldAttr = field.FindAttribute<IgnoreFieldAttribute>();
				if (ignoreFieldAttr != null)
				{
					continue;
				}

				result.Add(new FieldWrapper(field));
			}

			_membersCache[type] = result;

			return result;
		}

		public static List<IMemberWrapper> GetMembers<T>()
		{
			return GetMembers(typeof(T));
		}
	}
}
