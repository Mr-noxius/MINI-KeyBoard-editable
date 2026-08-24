namespace System.Dynamic.Utils;

internal static class Strings
{
	internal static string InvalidArgumentValue => System.SR.InvalidArgumentValue;

	internal static string NonEmptyCollectionRequired => System.SR.NonEmptyCollectionRequired;

	internal static string CollectionModifiedWhileEnumerating => System.SR.CollectionModifiedWhileEnumerating;

	internal static string EnumerationIsDone => System.SR.EnumerationIsDone;

	internal static string ExpressionMustBeReadable => System.SR.ExpressionMustBeReadable;

	internal static string IncorrectNumberOfLambdaArguments => System.SR.IncorrectNumberOfLambdaArguments;

	internal static string IncorrectNumberOfConstructorArguments => System.SR.IncorrectNumberOfConstructorArguments;

	internal static string InvalidNullValue(object p0)
	{
		return System.SR.Format(System.SR.InvalidNullValue, p0);
	}

	internal static string InvalidObjectType(object p0, object p1)
	{
		return System.SR.Format(System.SR.InvalidObjectType, p0, p1);
	}

	internal static string TypeContainsGenericParameters(object p0)
	{
		return System.SR.Format(System.SR.TypeContainsGenericParameters, p0);
	}

	internal static string TypeIsGeneric(object p0)
	{
		return System.SR.Format(System.SR.TypeIsGeneric, p0);
	}

	internal static string ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2)
	{
		return System.SR.Format(System.SR.ExpressionTypeDoesNotMatchMethodParameter, p0, p1, p2);
	}

	internal static string ExpressionTypeDoesNotMatchParameter(object p0, object p1)
	{
		return System.SR.Format(System.SR.ExpressionTypeDoesNotMatchParameter, p0, p1);
	}

	internal static string ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1)
	{
		return System.SR.Format(System.SR.ExpressionTypeDoesNotMatchConstructorParameter, p0, p1);
	}

	internal static string IncorrectNumberOfMethodCallArguments(object p0)
	{
		return System.SR.Format(System.SR.IncorrectNumberOfMethodCallArguments, p0);
	}
}
