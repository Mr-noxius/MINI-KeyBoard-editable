using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Theraot.Collections;
using Theraot.Core;

namespace System.Dynamic.Utils;

internal static class ExpressionUtils
{
	public static ReadOnlyCollection<T> ReturnReadOnly<T>(ref IList<T> collection)
	{
		IList<T> list = collection;
		if (list is ReadOnlyCollection<T> result)
		{
			return result;
		}
		Interlocked.CompareExchange(ref collection, list.ToReadOnly(), list);
		return (ReadOnlyCollection<T>)collection;
	}

	public static ReadOnlyCollection<Expression> ReturnReadOnly(IArgumentProvider provider, ref object collection)
	{
		if (collection is Expression expression)
		{
			Interlocked.CompareExchange(ref collection, new ReadOnlyCollection<Expression>(new ListArgumentProvider(provider, expression)), expression);
		}
		return (ReadOnlyCollection<Expression>)collection;
	}

	public static T ReturnObject<T>(object collectionOrT) where T : class
	{
		if (collectionOrT is T result)
		{
			return result;
		}
		return ((ReadOnlyCollection<T>)collectionOrT)[0];
	}

	public static void ValidateArgumentTypes(MethodBase method, ExpressionType nodeKind, ref ReadOnlyCollection<Expression> arguments)
	{
		ParameterInfo[] parametersForValidation = GetParametersForValidation(method, nodeKind);
		ValidateArgumentCount(method, nodeKind, arguments.Count, parametersForValidation);
		Expression[] array = null;
		int num = parametersForValidation.Length;
		for (int i = 0; i < num; i++)
		{
			Expression arg = arguments[i];
			ParameterInfo pi = parametersForValidation[i];
			arg = ValidateOneArgument(method, nodeKind, arg, pi);
			if (array == null && arg != arguments[i])
			{
				array = new Expression[arguments.Count];
				for (int j = 0; j < i; j++)
				{
					array[j] = arguments[j];
				}
			}
			if (array != null)
			{
				array[i] = arg;
			}
		}
		if (array != null)
		{
			arguments = new ReadOnlyCollection<Expression>(array);
		}
	}

	public static void ValidateArgumentCount(MethodBase method, ExpressionType nodeKind, int count, ParameterInfo[] pis)
	{
		if (pis.Length != count)
		{
			switch (nodeKind)
			{
			case ExpressionType.New:
				throw Error.IncorrectNumberOfConstructorArguments();
			case ExpressionType.Invoke:
				throw Error.IncorrectNumberOfLambdaArguments();
			case ExpressionType.Call:
			case ExpressionType.Dynamic:
				throw Error.IncorrectNumberOfMethodCallArguments(method);
			default:
				throw System.Dynamic.Utils.ContractUtils.Unreachable;
			}
		}
	}

	public static Expression ValidateOneArgument(MethodBase method, ExpressionType nodeKind, Expression arg, ParameterInfo pi)
	{
		RequiresCanRead(arg, "arguments");
		Type type = pi.ParameterType;
		if (type.IsByRef)
		{
			type = type.GetElementType();
		}
		TypeHelper.ValidateType(type);
		if (!TypeHelper.AreReferenceAssignable(type, arg.Type) && !TryQuote(type, ref arg))
		{
			switch (nodeKind)
			{
			case ExpressionType.New:
				throw Error.ExpressionTypeDoesNotMatchConstructorParameter(arg.Type, type);
			case ExpressionType.Invoke:
				throw Error.ExpressionTypeDoesNotMatchParameter(arg.Type, type);
			case ExpressionType.Call:
			case ExpressionType.Dynamic:
				throw Error.ExpressionTypeDoesNotMatchMethodParameter(arg.Type, type, method);
			default:
				throw System.Dynamic.Utils.ContractUtils.Unreachable;
			}
		}
		return arg;
	}

	public static void RequiresCanRead(Expression expression, string paramName)
	{
		if (expression == null)
		{
			throw new ArgumentNullException(paramName);
		}
		switch (expression.NodeType)
		{
		case ExpressionType.Index:
		{
			IndexExpression indexExpression = (IndexExpression)expression;
			if (indexExpression.Indexer != null && !indexExpression.Indexer.CanRead)
			{
				throw new ArgumentException(Strings.ExpressionMustBeReadable, paramName);
			}
			break;
		}
		case ExpressionType.MemberAccess:
		{
			MemberExpression memberExpression = (MemberExpression)expression;
			PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
			if (propertyInfo != null && !propertyInfo.CanRead)
			{
				throw new ArgumentException(Strings.ExpressionMustBeReadable, paramName);
			}
			break;
		}
		}
	}

	public static bool TryQuote(Type parameterType, ref Expression argument)
	{
		Type typeFromHandle = typeof(LambdaExpression);
		if (parameterType.IsSameOrSubclassOf(typeFromHandle) && parameterType.IsInstanceOfType(argument))
		{
			argument = Expression.Quote(argument);
			return true;
		}
		return false;
	}

	internal static ParameterInfo[] GetParametersForValidation(MethodBase method, ExpressionType nodeKind)
	{
		ParameterInfo[] array = method.GetParameters();
		if (nodeKind == ExpressionType.Dynamic)
		{
			array = array.RemoveFirst();
		}
		return array;
	}
}
