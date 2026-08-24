using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.Collections.Specialized;

[Serializable]
public sealed class KeyCollection<TKey, TValue> : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey>, IEnumerable<TKey>, IEnumerable
{
	private readonly IDictionary<TKey, TValue> _wrapped;

	public int Count => _wrapped.Count;

	bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

	object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

	bool ICollection<TKey>.IsReadOnly => true;

	internal KeyCollection(IDictionary<TKey, TValue> wrapped)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		_wrapped = wrapped;
	}

	public void CopyTo(TKey[] array, int arrayIndex)
	{
		Extensions.CanCopyTo(_wrapped.Count, array, arrayIndex);
		_wrapped.ConvertProgressive((KeyValuePair<TKey, TValue> pair) => pair.Key).CopyTo(array, arrayIndex);
	}

	public IEnumerator<TKey> GetEnumerator()
	{
		return _wrapped.ConvertProgressive((KeyValuePair<TKey, TValue> pair) => pair.Key).GetEnumerator();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)_wrapped).CopyTo(array, index);
	}

	void ICollection<TKey>.Add(TKey item)
	{
		throw new NotSupportedException();
	}

	void ICollection<TKey>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<TKey>.Contains(TKey item)
	{
		return _wrapped.ContainsKey(item);
	}

	bool ICollection<TKey>.Remove(TKey item)
	{
		throw new NotSupportedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
