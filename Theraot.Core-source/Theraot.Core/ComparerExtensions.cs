using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.Core;

public static class ComparerExtensions
{
	public static IComparer<T> ToComparer<T>(this Comparison<T> comparison)
	{
		return new CustomComparer<T>(comparison);
	}
}
