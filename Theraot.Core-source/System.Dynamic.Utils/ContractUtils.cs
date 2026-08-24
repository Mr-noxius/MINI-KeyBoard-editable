using System.Collections.Generic;
using System.Globalization;

namespace System.Dynamic.Utils;

internal static class ContractUtils
{
	public static Exception Unreachable => new InvalidOperationException("Code supposed to be unreachable");

	public static void Requires(bool precondition, string paramName)
	{
		if (!precondition)
		{
			throw new ArgumentException("Invalid argument value", paramName);
		}
	}

	public static void RequiresNotNull(object value, string paramName)
	{
		if (value == null)
		{
			throw new ArgumentNullException(paramName);
		}
	}

	public static void RequiresNotEmpty<T>(ICollection<T> collection, string paramName)
	{
		RequiresNotNull(collection, paramName);
		if (collection.Count == 0)
		{
			throw new ArgumentException("Non empty collection required", paramName);
		}
	}

	public static void RequiresNotNullItems<T>(IList<T> array, string arrayName)
	{
		RequiresNotNull(array, arrayName);
		for (int i = 0; i < array.Count; i++)
		{
			if (object.ReferenceEquals(array[i], null))
			{
				throw new ArgumentNullException(string.Format(CultureInfo.CurrentCulture, "{0}[{1}]", new object[2] { arrayName, i }));
			}
		}
	}

	public static void RequiresArrayRange<T>(IList<T> array, int offset, int count, string offsetName, string countName)
	{
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException(countName);
		}
		if (offset < 0 || array.Count - offset < count)
		{
			throw new ArgumentOutOfRangeException(offsetName);
		}
	}
}
