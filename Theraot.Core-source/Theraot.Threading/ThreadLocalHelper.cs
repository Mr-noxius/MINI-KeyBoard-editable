using System;
using Theraot.Threading.Needles;

namespace Theraot.Threading;

internal static class ThreadLocalHelper
{
	private static readonly Exception _recursionGuardException;

	public static Exception RecursionGuardException => _recursionGuardException;

	static ThreadLocalHelper()
	{
		_recursionGuardException = GetInvalidOperationException();
	}

	private static InvalidOperationException GetInvalidOperationException()
	{
		return new InvalidOperationException("Recursion");
	}
}
internal static class ThreadLocalHelper<T>
{
	private static readonly INeedle<T> _recursionGuardNeedle = new ExceptionStructNeedle<T>(ThreadLocalHelper.RecursionGuardException);

	public static INeedle<T> RecursionGuardNeedle => _recursionGuardNeedle;
}
