using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Core;

namespace System.Linq;

internal class QueryableTransformer : ExpressionTransformer
{
	protected override Expression VisitConstant(ConstantExpression constant)
	{
		if (!(constant.Value is IQueryableEnumerable queryableEnumerable))
		{
			return constant;
		}
		IEnumerable enumerable = queryableEnumerable.GetEnumerable();
		if (enumerable != null)
		{
			return Expression.Constant(enumerable);
		}
		return Visit(queryableEnumerable.Expression);
	}

	protected override Expression VisitLambda(LambdaExpression lambda)
	{
		return lambda;
	}

	protected override Expression VisitMethodCall(MethodCallExpression methodCall)
	{
		if (IsQueryableExtension(methodCall.Method))
		{
			return ReplaceQueryableMethod(methodCall);
		}
		return base.VisitMethodCall(methodCall);
	}

	private static Type GetComparableType(Type type)
	{
		if (type.IsGenericInstanceOf(typeof(IQueryable<>)))
		{
			type = typeof(IEnumerable<>).MakeGenericTypeFrom(type);
		}
		else if (type.IsGenericInstanceOf(typeof(IOrderedQueryable<>)))
		{
			type = typeof(IOrderedEnumerable<>).MakeGenericTypeFrom(type);
		}
		else if (type.IsGenericInstanceOf(typeof(Expression<>)))
		{
			type = type.GetFirstGenericArgument();
		}
		else if (type == typeof(IQueryable))
		{
			type = typeof(IEnumerable);
		}
		return type;
	}

	private static MethodInfo GetMatchingMethod(MethodInfo method, Type declaring)
	{
		MethodInfo[] methods = declaring.GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (MethodMatch(methodInfo, method))
			{
				if (method.IsGenericMethod)
				{
					return methodInfo.MakeGenericMethodFrom(method);
				}
				return methodInfo;
			}
		}
		return null;
	}

	private static Type GetTargetDeclaringType(MethodInfo method)
	{
		if (!(method.DeclaringType == typeof(Queryable)))
		{
			return method.DeclaringType;
		}
		return typeof(Enumerable);
	}

	private static bool HasExtensionAttribute(MethodInfo method)
	{
		return method.GetCustomAttributes(typeof(ExtensionAttribute), inherit: false).Length > 0;
	}

	private static bool IsQueryableExtension(MethodInfo method)
	{
		if (HasExtensionAttribute(method))
		{
			return method.GetParameters()[0].ParameterType.IsAssignableTo(typeof(IQueryable));
		}
		return false;
	}

	private static bool MethodMatch(MethodInfo candidate, MethodInfo method)
	{
		if (candidate.Name != method.Name || !HasExtensionAttribute(candidate))
		{
			return false;
		}
		Type[] parameterTypes = method.GetParameterTypes();
		if (parameterTypes.Length != candidate.GetParameters().Length)
		{
			return false;
		}
		if (method.IsGenericMethod)
		{
			if (!candidate.IsGenericMethod || candidate.GetGenericArguments().Length != method.GetGenericArguments().Length)
			{
				return false;
			}
			candidate = candidate.MakeGenericMethodFrom(method);
		}
		if (!TypeMatch(candidate.ReturnType, method.ReturnType))
		{
			return false;
		}
		Type[] parameterTypes2 = candidate.GetParameterTypes();
		if (parameterTypes2[0] != GetComparableType(parameterTypes[0]))
		{
			return false;
		}
		for (int i = 1; i < parameterTypes2.Length; i++)
		{
			if (!TypeMatch(parameterTypes2[i], parameterTypes[i]))
			{
				return false;
			}
		}
		return true;
	}

	private static MethodInfo ReplaceQueryableMethod(MethodInfo method)
	{
		Type targetDeclaringType = GetTargetDeclaringType(method);
		MethodInfo matchingMethod = GetMatchingMethod(method, targetDeclaringType);
		if (matchingMethod != null)
		{
			return matchingMethod;
		}
		throw new InvalidOperationException($"There is no method {method.Name} on type {targetDeclaringType.FullName} that matches the specified arguments");
	}

	private static bool TypeMatch(Type candidate, Type type)
	{
		if (candidate == type)
		{
			return true;
		}
		return candidate == GetComparableType(type);
	}

	private static Expression UnquoteIfNeeded(Expression expression, Type delegateType)
	{
		if (expression.NodeType != ExpressionType.Quote)
		{
			return expression;
		}
		LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)expression).Operand;
		if (lambdaExpression.Type == delegateType)
		{
			return lambdaExpression;
		}
		return expression;
	}

	private MethodCallExpression ReplaceQueryableMethod(MethodCallExpression old)
	{
		Expression instance = null;
		if (old.Object != null)
		{
			instance = Visit(old.Object);
		}
		MethodInfo methodInfo = ReplaceQueryableMethod(old.Method);
		ParameterInfo[] parameters = methodInfo.GetParameters();
		Expression[] array = new Expression[old.Arguments.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = UnquoteIfNeeded(Visit(old.Arguments[i]), parameters[i].ParameterType);
		}
		return Expression.Call(instance, methodInfo, array);
	}
}
