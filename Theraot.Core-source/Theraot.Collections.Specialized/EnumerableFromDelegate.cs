using System;
using System.Collections;
using System.Collections.Generic;
using Theraot.Core;

namespace Theraot.Collections.Specialized;

public class EnumerableFromDelegate<T> : IEnumerable<T>, IEnumerable
{
	private readonly Func<IEnumerator<T>> _getEnumerator;

	public EnumerableFromDelegate(Func<IEnumerator> getEnumerator)
	{
		_getEnumerator = getEnumerator.ChainConversion(ConvertEnumerator);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _getEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _getEnumerator();
	}

	private static IEnumerator<T> ConvertEnumerator(IEnumerator enumerator)
	{
		if (enumerator == null)
		{
			return null;
		}
		if (enumerator is IEnumerator<T> result)
		{
			return result;
		}
		return ConvertEnumeratorExtracted(enumerator);
	}

	private static IEnumerator<T> ConvertEnumeratorExtracted(IEnumerator enumerator)
	{
		try
		{
			while (enumerator.MoveNext())
			{
				object element = enumerator.Current;
				if (element is T)
				{
					yield return (T)element;
				}
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}
}
