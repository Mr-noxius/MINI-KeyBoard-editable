using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections.Specialized;

[DebuggerNonUserCode]
public class DelegatedCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
	private readonly IReadOnlyCollection<T> _readOnly;

	private readonly Func<ICollection<T>> _wrapped;

	public IReadOnlyCollection<T> AsReadOnly => _readOnly;

	public int Count => Instance.Count;

	public bool IsReadOnly => Instance.IsReadOnly;

	private ICollection<T> Instance => _wrapped() ?? ((ICollection<T>)ArrayReservoir<T>.EmptyArray);

	public DelegatedCollection(Func<ICollection<T>> wrapped)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		_wrapped = wrapped;
		_readOnly = new ExtendedReadOnlyCollection<T>(this);
	}

	public void Add(T item)
	{
		Instance.Add(item);
	}

	public void Clear()
	{
		Instance.Clear();
	}

	public bool Contains(T item)
	{
		return Instance.Contains(item);
	}

	public bool Contains(T item, IEqualityComparer<T> comparer)
	{
		return Instance.Contains(item, comparer);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Instance.CopyTo(array, arrayIndex);
	}

	public void CopyTo(T[] array)
	{
		Instance.CopyTo(array, 0);
	}

	public void CopyTo(T[] array, int arrayIndex, int countLimit)
	{
		Extensions.CanCopyTo(array, arrayIndex, countLimit);
		Instance.CopyTo(array, arrayIndex, countLimit);
	}

	public IEnumerator<T> GetEnumerator()
	{
		ICollection<T> collection = Instance;
		foreach (T item in collection)
		{
			if (!object.ReferenceEquals(collection, Instance))
			{
				throw new InvalidOperationException();
			}
			yield return item;
		}
	}

	public bool Remove(T item)
	{
		return Instance.Remove(item);
	}

	public bool Remove(T item, IEqualityComparer<T> comparer)
	{
		if (comparer == null)
		{
			comparer = EqualityComparer<T>.Default;
		}
		using (IEnumerator<T> enumerator = Instance.RemoveWhereEnumerable((T input) => comparer.Equals(input, item)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				GC.KeepAlive(current);
				return true;
			}
		}
		return false;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public T[] ToArray()
	{
		T[] array = new T[Instance.Count];
		Instance.CopyTo(array, 0);
		return array;
	}
}
