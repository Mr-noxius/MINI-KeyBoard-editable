namespace System.Dynamic.Utils;

internal static class Error
{
	internal static Exception EnumerationIsDone()
	{
		return new InvalidOperationException(Strings.EnumerationIsDone);
	}

	internal static Exception CollectionModifiedWhileEnumerating()
	{
		return new InvalidOperationException(Strings.CollectionModifiedWhileEnumerating);
	}

	internal static Exception TypeContainsGenericParameters(object p0)
	{
		return new ArgumentException(Strings.TypeContainsGenericParameters(p0));
	}

	internal static Exception TypeIsGeneric(object p0)
	{
		return new ArgumentException(Strings.TypeIsGeneric(p0));
	}

	internal static Exception IncorrectNumberOfConstructorArguments()
	{
		return new ArgumentException(Strings.IncorrectNumberOfConstructorArguments);
	}

	internal static Exception ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2)
	{
		return new ArgumentException(Strings.ExpressionTypeDoesNotMatchMethodParameter(p0, p1, p2));
	}

	internal static Exception ExpressionTypeDoesNotMatchParameter(object p0, object p1)
	{
		return new ArgumentException(Strings.ExpressionTypeDoesNotMatchParameter(p0, p1));
	}

	internal static Exception IncorrectNumberOfLambdaArguments()
	{
		return new InvalidOperationException(Strings.IncorrectNumberOfLambdaArguments);
	}

	internal static Exception IncorrectNumberOfMethodCallArguments(object p0)
	{
		return new ArgumentException(Strings.IncorrectNumberOfMethodCallArguments(p0));
	}

	internal static Exception ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1)
	{
		return new ArgumentException(Strings.ExpressionTypeDoesNotMatchConstructorParameter(p0, p1));
	}
}
