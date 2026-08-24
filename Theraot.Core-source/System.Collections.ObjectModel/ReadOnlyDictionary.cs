using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace System.Collections.ObjectModel;

[Serializable]
public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	[Serializable]
	public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable
	{
		private readonly ICollection<TValue> _wrapped;

		public int Count => _wrapped.Count;

		bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

		object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

		bool ICollection<TValue>.IsReadOnly => true;

		internal ValueCollection(ICollection<TValue> wrapped)
		{
			if (wrapped == null)
			{
				throw new ArgumentNullException("wrapped");
			}
			_wrapped = wrapped;
		}

		public void CopyTo(TValue[] array, int arrayIndex)
		{
			_wrapped.CopyTo(array, arrayIndex);
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return _wrapped.GetEnumerator();
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
			return _wrapped.Contains(item);
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

	[Serializable]
	public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable
	{
		private readonly ICollection<TKey> _wrapped;

		public int Count => _wrapped.Count;

		bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

		object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

		bool ICollection<TKey>.IsReadOnly => true;

		internal KeyCollection(ICollection<TKey> wrapped)
		{
			if (wrapped == null)
			{
				throw new ArgumentNullException("wrapped");
			}
			_wrapped = wrapped;
		}

		public void CopyTo(TKey[] array, int arrayIndex)
		{
			_wrapped.CopyTo(array, arrayIndex);
		}

		public IEnumerator<TKey> GetEnumerator()
		{
			return _wrapped.GetEnumerator();
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
			return _wrapped.Contains(item);
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

	private readonly KeyCollection _keys;

	private readonly ValueCollection _values;

	private readonly IDictionary<TKey, TValue> _wrapped;

	public int Count => _wrapped.Count;

	public IDictionary<TKey, TValue> Dictionary => _wrapped;

	bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

	object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

	bool IDictionary.IsFixedSize => ((IDictionary)_wrapped).IsFixedSize;

	bool IDictionary.IsReadOnly => true;

	ICollection IDictionary.Keys => _keys;

	ICollection IDictionary.Values => _values;

	ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keys;

	ICollection<TValue> IDictionary<TKey, TValue>.Values => _values;

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _keys;

	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _values;

	public KeyCollection Keys => _keys;

	public ValueCollection Values => _values;

	object IDictionary.this[object key]
	{
		get
		{
			if (object.ReferenceEquals(key, null))
			{
				throw new ArgumentNullException("key");
			}
			if (key is TKey)
			{
				return this[(TKey)key];
			}
			return null;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	TValue IDictionary<TKey, TValue>.this[TKey key]
	{
		get
		{
			return this[key];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public TValue this[TKey key] => _wrapped[key];

	public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		_wrapped = dictionary;
		_keys = new KeyCollection(new DelegatedCollection<TKey>(() => _wrapped.Keys));
		_values = new ValueCollection(new DelegatedCollection<TValue>(() => _wrapped.Values));
	}

	public bool ContainsKey(TKey key)
	{
		return _wrapped.ContainsKey(key);
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return _wrapped.GetEnumerator();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)_wrapped).CopyTo(array, index);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		GC.KeepAlive(item);
		throw new NotSupportedException();
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		return _wrapped.Contains(item);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		_wrapped.CopyTo(array, arrayIndex);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		GC.KeepAlive(item);
		throw new NotSupportedException();
	}

	void IDictionary.Add(object key, object value)
	{
		throw new NotSupportedException();
	}

	void IDictionary.Clear()
	{
		throw new NotSupportedException();
	}

	bool IDictionary.Contains(object key)
	{
		if (object.ReferenceEquals(key, null))
		{
			throw new ArgumentNullException("key");
		}
		if (key is TKey)
		{
			return ContainsKey((TKey)key);
		}
		return false;
	}

	IDictionaryEnumerator IDictionary.GetEnumerator()
	{
		return ((IDictionary)_wrapped).GetEnumerator();
	}

	void IDictionary.Remove(object key)
	{
		throw new NotSupportedException();
	}

	void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
	{
		throw new NotSupportedException();
	}

	bool IDictionary<TKey, TValue>.Remove(TKey key)
	{
		throw new NotSupportedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return _wrapped.TryGetValue(key, out value);
	}
}
