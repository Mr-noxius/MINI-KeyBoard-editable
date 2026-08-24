using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Core;

namespace System.Linq.Expressions;

internal static class Extensions
{
	public static Type GetFirstGenericArgument(this Type self)
	{
		return self.GetGenericArguments()[0];
	}

	public static MethodInfo GetInvokeMethod(this Type self)
	{
		return self.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
	}

	public static Type GetNotNullableType(this Type self)
	{
		if (!self.IsNullable())
		{
			return self;
		}
		return self.GetGenericArguments()[0];
	}

	public static Type[] GetParameterTypes(this MethodBase self)
	{
		ParameterInfo[] parameters = self.GetParameters();
		Type[] array = new Type[parameters.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = parameters[i].ParameterType;
		}
		return array;
	}

	public static bool IsExpression(this Type self)
	{
		if (!(self == typeof(Expression)))
		{
			return self.IsSubclassOf(typeof(Expression));
		}
		return true;
	}

	public static MethodInfo MakeGenericMethodFrom(this MethodInfo self, MethodInfo method)
	{
		return self.MakeGenericMethod(method.GetGenericArguments());
	}

	public static Type MakeGenericTypeFrom(this Type self, Type type)
	{
		return self.MakeGenericType(type.GetGenericArguments());
	}

	public static Type MakeStrongBoxType(this Type self)
	{
		return typeof(StrongBox<>).MakeGenericType(self);
	}

	public static void OnFieldOrProperty(this MemberInfo self, Action<FieldInfo> onfield, Action<PropertyInfo> onprop)
	{
		switch (self.MemberType)
		{
		case MemberTypes.Field:
			onfield((FieldInfo)self);
			break;
		case MemberTypes.Property:
			onprop((PropertyInfo)self);
			break;
		default:
			throw new ArgumentException();
		}
	}

	public static T OnFieldOrProperty<T>(this MemberInfo self, Func<FieldInfo, T> onfield, Func<PropertyInfo, T> onprop)
	{
		return self.MemberType switch
		{
			MemberTypes.Field => onfield((FieldInfo)self), 
			MemberTypes.Property => onprop((PropertyInfo)self), 
			_ => throw new ArgumentException(), 
		};
	}
}
