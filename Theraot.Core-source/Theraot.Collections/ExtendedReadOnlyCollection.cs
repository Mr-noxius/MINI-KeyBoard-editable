using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections;

[Serializable]
[DebuggerNonUserCode]
public sealed class ExtendedReadOnlyCollection<T> : IReadOnlyCollection<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private readonly ICollection<T> _wrapped;

	public IReadOnlyCollection<T> AsReadOnly => this;

	public int Count => _wrapped.Count;

	bool ICollection<T>.IsReadOnly => true;

	public ExtendedReadOnlyCollection(ICollection<T> wrapped)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		_wrapped = wrapped;
	}

	public bool Contains(T item)
	{
		return _wrapped.Contains(item);
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

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public T[] ToArray()
	{
		T[] array = new T[_wrapped.Count];
		CopyTo(array);
		return array;
	}
}
