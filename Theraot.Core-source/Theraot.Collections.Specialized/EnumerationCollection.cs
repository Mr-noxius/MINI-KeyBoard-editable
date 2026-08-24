using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections.Specialized;

[DebuggerNonUserCode]
public class EnumerationCollection<T> : ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
{
	private readonly Func<T, bool> _contains;

	private readonly Func<int> _count;

	private readonly IEnumerable<T> _wrapped;

	public int Count => _count();

	bool ICollection<T>.IsReadOnly => true;

	public EnumerationCollection(IEnumerable<T> wrapped)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		_wrapped = wrapped;
		_count = _wrapped.Count<T>;
		_contains = (T item) => _wrapped.Contains(item, EqualityComparer<T>.Default);
	}

	public EnumerationCollection(T[] wrapped)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		_wrapped = wrapped;
		_count = () => wrapped.Length;
		_contains = (T item) => Array.IndexOf(wrapped, item) >= 0;
	}

	public EnumerationCollection(ICollection<T> wrapped)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		_wrapped = wrapped;
		_count = () => wrapped.Count;
		_contains = wrapped.Contains;
	}

	public EnumerationCollection(IEnumerable<T> wrapped, Func<int> count)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		_wrapped = wrapped;
		if (count == null)
		{
			throw new ArgumentNullException("count");
		}
		_count = count;
		_contains = (T item) => _wrapped.Contains(item, EqualityComparer<T>.Default);
	}

	public EnumerationCollection(IEnumerable<T> wrapped, Func<T, bool> contains)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		_wrapped = wrapped;
		_count = _wrapped.Count<T>;
		if (contains == null)
		{
			throw new ArgumentNullException("contains");
		}
		_contains = contains;
	}

	public EnumerationCollection(IEnumerable<T> wrapped, Func<int> count, Func<T, bool> contains)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		_wrapped = wrapped;
		if (count == null)
		{
			throw new ArgumentNullException("count");
		}
		_count = count;
		if (contains == null)
		{
			throw new ArgumentNullException("contains");
		}
		_contains = contains;
	}

	public bool Contains(T item)
	{
		return _contains(item);
	}

	public bool Contains(T item, IEqualityComparer<T> comparer)
	{
		return Enumerable.Contains(this, item, comparer);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		_wrapped.CopyTo(array, arrayIndex);
	}

	public void CopyTo(T[] array)
	{
		_wrapped.CopyTo(array, 0);
	}

	public void CopyTo(T[] array, int arrayIndex, int countLimit)
	{
		Extensions.CanCopyTo(array, arrayIndex, countLimit);
		Extensions.CopyTo(this, array, arrayIndex, countLimit);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _wrapped.GetEnumerator();
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}

	public int IndexOf(T item)
	{
		return _wrapped.IndexOf(item);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public T[] ToArray()
	{
		T[] array = new T[_count()];
		CopyTo(array);
		return array;
	}
}
