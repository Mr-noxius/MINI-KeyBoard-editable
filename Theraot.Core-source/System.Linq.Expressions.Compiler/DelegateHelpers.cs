using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Theraot.Collections;
using Theraot.Core;

namespace System.Linq.Expressions.Compiler;

internal static class DelegateHelpers
{
	internal class TypeInfo
	{
		public Type DelegateType;

		public Dictionary<Type, TypeInfo> TypeChain;
	}

	private const int MaximumArity = 17;

	private const MethodAttributes _ctorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName;

	private const MethodImplAttributes _implAttributes = MethodImplAttributes.CodeTypeMask;

	private const MethodAttributes _invokeAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask;

	private static TypeInfo _DelegateCache = new TypeInfo();

	private static readonly Type[] _delegateCtorSignature = new Type[2]
	{
		typeof(object),
		typeof(IntPtr)
	};

	private static TypeInfo NextTypeInfo(Type initialArg, TypeInfo curTypeInfo)
	{
		if (curTypeInfo.TypeChain == null)
		{
			curTypeInfo.TypeChain = new Dictionary<Type, TypeInfo>();
		}
		if (!curTypeInfo.TypeChain.TryGetValue(initialArg, out var value))
		{
			value = new TypeInfo();
			if (initialArg.CanCache())
			{
				curTypeInfo.TypeChain[initialArg] = value;
			}
		}
		return value;
	}

	internal static Type MakeNewDelegate(Type[] types)
	{
		bool flag;
		if (types.Length > 17)
		{
			flag = true;
		}
		else
		{
			flag = false;
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsByRef)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			return MakeNewCustomDelegate(types);
		}
		if (types[types.Length - 1] == typeof(void))
		{
			return GetActionType(types.RemoveLast());
		}
		return GetFuncType(types);
	}

	internal static Type GetFuncType(Type[] types)
	{
		return types.Length switch
		{
			1 => typeof(Func<>).MakeGenericType(types), 
			2 => typeof(Func<, >).MakeGenericType(types), 
			3 => typeof(Func<, , >).MakeGenericType(types), 
			4 => typeof(Func<, , , >).MakeGenericType(types), 
			5 => typeof(Func<, , , , >).MakeGenericType(types), 
			6 => typeof(Func<, , , , , >).MakeGenericType(types), 
			7 => typeof(Func<, , , , , , >).MakeGenericType(types), 
			8 => typeof(Func<, , , , , , , >).MakeGenericType(types), 
			9 => typeof(Func<, , , , , , , , >).MakeGenericType(types), 
			10 => typeof(Func<, , , , , , , , , >).MakeGenericType(types), 
			11 => typeof(Func<, , , , , , , , , , >).MakeGenericType(types), 
			12 => typeof(Func<, , , , , , , , , , , >).MakeGenericType(types), 
			13 => typeof(Func<, , , , , , , , , , , , >).MakeGenericType(types), 
			14 => typeof(Func<, , , , , , , , , , , , , >).MakeGenericType(types), 
			15 => typeof(Func<, , , , , , , , , , , , , , >).MakeGenericType(types), 
			16 => typeof(Func<, , , , , , , , , , , , , , , >).MakeGenericType(types), 
			17 => typeof(Func<, , , , , , , , , , , , , , , , >).MakeGenericType(types), 
			_ => null, 
		};
	}

	internal static Type GetActionType(Type[] types)
	{
		return types.Length switch
		{
			0 => typeof(Action), 
			1 => typeof(Action<>).MakeGenericType(types), 
			2 => typeof(Action<, >).MakeGenericType(types), 
			3 => typeof(Action<, , >).MakeGenericType(types), 
			4 => typeof(Action<, , , >).MakeGenericType(types), 
			5 => typeof(Action<, , , , >).MakeGenericType(types), 
			6 => typeof(Action<, , , , , >).MakeGenericType(types), 
			7 => typeof(Action<, , , , , , >).MakeGenericType(types), 
			8 => typeof(Action<, , , , , , , >).MakeGenericType(types), 
			9 => typeof(Action<, , , , , , , , >).MakeGenericType(types), 
			10 => typeof(Action<, , , , , , , , , >).MakeGenericType(types), 
			11 => typeof(Action<, , , , , , , , , , >).MakeGenericType(types), 
			12 => typeof(Action<, , , , , , , , , , , >).MakeGenericType(types), 
			13 => typeof(Action<, , , , , , , , , , , , >).MakeGenericType(types), 
			14 => typeof(Action<, , , , , , , , , , , , , >).MakeGenericType(types), 
			15 => typeof(Action<, , , , , , , , , , , , , , >).MakeGenericType(types), 
			16 => typeof(Action<, , , , , , , , , , , , , , , >).MakeGenericType(types), 
			_ => null, 
		};
	}

	private static Type MakeNewCustomDelegate(Type[] types)
	{
		Type returnType = types[types.Length - 1];
		Type[] parameterTypes = types.RemoveLast();
		TypeBuilder typeBuilder = System.Linq.Expressions.Compiler.AssemblyGen.DefineDelegateType("Delegate" + types.Length);
		typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName, CallingConventions.Standard, _delegateCtorSignature).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
		typeBuilder.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, returnType, parameterTypes).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
		return typeBuilder.CreateType();
	}

	internal static Type MakeDelegateType(Type[] types)
	{
		lock (_DelegateCache)
		{
			TypeInfo typeInfo = _DelegateCache;
			foreach (Type initialArg in types)
			{
				typeInfo = NextTypeInfo(initialArg, typeInfo);
			}
			typeInfo.DelegateType = typeInfo.DelegateType ?? MakeNewDelegate((Type[])types.Clone());
			return typeInfo.DelegateType;
		}
	}
}
