using System.Reflection;
using System.Reflection.Emit;

namespace System.Dynamic.Utils;

internal static class DelegateHelpers
{
	private static readonly MethodInfo _funcInvoke = typeof(Func<object[], object>).GetMethod("Invoke");

	internal static Delegate CreateObjectArrayDelegate(Type delegateType, Func<object[], object> handler)
	{
		return CreateObjectArrayDelegateRefEmit(delegateType, handler);
	}

	private static Delegate CreateObjectArrayDelegateRefEmit(Type delegateType, Func<object[], object> handler)
	{
		MethodInfo method = delegateType.GetMethod("Invoke");
		Type returnType = method.ReturnType;
		bool flag = returnType != typeof(void);
		ParameterInfo[] parameters = method.GetParameters();
		Type[] array = new Type[parameters.Length + 1];
		array[0] = typeof(Func<object[], object>);
		for (int i = 0; i < parameters.Length; i++)
		{
			array[i + 1] = parameters[i].ParameterType;
		}
		DynamicMethod dynamicMethod = new DynamicMethod("Thunk", returnType, array);
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		LocalBuilder local = iLGenerator.DeclareLocal(typeof(object[]));
		LocalBuilder local2 = iLGenerator.DeclareLocal(typeof(object));
		iLGenerator.Emit(OpCodes.Ldc_I4, parameters.Length);
		iLGenerator.Emit(OpCodes.Newarr, typeof(object));
		iLGenerator.Emit(OpCodes.Stloc, local);
		bool flag2 = false;
		for (int j = 0; j < parameters.Length; j++)
		{
			bool isByRef = parameters[j].ParameterType.IsByRef;
			Type type = parameters[j].ParameterType;
			if (isByRef)
			{
				type = type.GetElementType();
			}
			flag2 = flag2 || isByRef;
			iLGenerator.Emit(OpCodes.Ldloc, local);
			iLGenerator.Emit(OpCodes.Ldc_I4, j);
			iLGenerator.Emit(OpCodes.Ldarg, j + 1);
			if (isByRef)
			{
				iLGenerator.Emit(OpCodes.Ldobj, type);
			}
			Type cls = ConvertToBoxableType(type);
			iLGenerator.Emit(OpCodes.Box, cls);
			iLGenerator.Emit(OpCodes.Stelem_Ref);
		}
		if (flag2)
		{
			iLGenerator.BeginExceptionBlock();
		}
		iLGenerator.Emit(OpCodes.Ldarg_0);
		iLGenerator.Emit(OpCodes.Ldloc, local);
		MethodInfo funcInvoke = _funcInvoke;
		iLGenerator.Emit(OpCodes.Callvirt, funcInvoke);
		iLGenerator.Emit(OpCodes.Stloc, local2);
		if (flag2)
		{
			iLGenerator.BeginFinallyBlock();
			for (int k = 0; k < parameters.Length; k++)
			{
				if (parameters[k].ParameterType.IsByRef)
				{
					Type elementType = parameters[k].ParameterType.GetElementType();
					iLGenerator.Emit(OpCodes.Ldarg, k + 1);
					iLGenerator.Emit(OpCodes.Ldloc, local);
					iLGenerator.Emit(OpCodes.Ldc_I4, k);
					iLGenerator.Emit(OpCodes.Ldelem_Ref);
					iLGenerator.Emit(OpCodes.Unbox_Any, elementType);
					iLGenerator.Emit(OpCodes.Stobj, elementType);
				}
			}
			iLGenerator.EndExceptionBlock();
		}
		if (flag)
		{
			iLGenerator.Emit(OpCodes.Ldloc, local2);
			iLGenerator.Emit(OpCodes.Unbox_Any, ConvertToBoxableType(returnType));
		}
		iLGenerator.Emit(OpCodes.Ret);
		return dynamicMethod.CreateDelegate(delegateType, handler);
	}

	private static Type ConvertToBoxableType(Type t)
	{
		if (!t.IsPointer)
		{
			return t;
		}
		return typeof(IntPtr);
	}
}
