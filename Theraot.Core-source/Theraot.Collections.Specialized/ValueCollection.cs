using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections.Specialized;

[Serializable]
public sealed class ValueCollection<TKey, TValue> : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>, IEnumerable<TValue>, IEnumerable
{
	private readonly IDictionary<TKey, TValue> _wrapped;

	public int Count => _wrapped.Count;

	bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

	object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

	bool ICollection<TValue>.IsReadOnly => true;

	internal ValueCollection(IDictionary<TKey, TValue> wrapped)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		_wrapped = wrapped;
	}

	public void CopyTo(TValue[] array, int arrayIndex)
	{
		Extensions.CanCopyTo(_wrapped.Count, array, arrayIndex);
		_wrapped.ConvertProgressive((KeyValuePair<TKey, TValue> pair) => pair.Value).CopyTo(array, arrayIndex);
	}

	public IEnumerator<TValue> GetEnumerator()
	{
		return _wrapped.ConvertProgressive((KeyValuePair<TKey, TValue> pair) => pair.Value).GetEnumerator();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)_wrapped).CopyTo(array, index);
	}

	void ICollection<TValue>.Add(TValue item)
	{
		throw new NotSupportedException();
	}

	void ICollection<TValue>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<TValue>.Contains(TValue item)
	{
		return _wrapped.Where((KeyValuePair<TKey, TValue> pair) => EqualityComparer<TValue>.Default.Equals(item, pair.Value)).HasAtLeast(1);
	}

	bool ICollection<TValue>.Remove(TValue item)
	{
		throw new NotSupportedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
