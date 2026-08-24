using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Collections.Specialized;

[DebuggerNonUserCode]
public class ConversionEqualityComparer<TInput, TOutput> : IEqualityComparer<TInput>
{
	private readonly IEqualityComparer<TOutput> _comparer;

	private readonly Func<TInput, TOutput> _converter;

	public ConversionEqualityComparer(IEqualityComparer<TOutput> comparer, Func<TInput, TOutput> converter)
	{
		_comparer = comparer ?? EqualityComparer<TOutput>.Default;
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		_converter = converter;
	}

	public bool Equals(TInput x, TInput y)
	{
		return _comparer.Equals(_converter(x), _converter(y));
	}

	public int GetHashCode(TInput obj)
	{
		return _comparer.GetHashCode(_converter(obj));
	}
}
