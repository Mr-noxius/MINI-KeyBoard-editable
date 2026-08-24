using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections.Specialized;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public sealed class SafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	private const int _defaultProbing = 1;

	private readonly KeyCollection<TKey, TValue> _keyCollection;

	private readonly IEqualityComparer<TKey> _keyComparer;

	private readonly ValueCollection<TKey, TValue> _valueCollection;

	private readonly IEqualityComparer<TValue> _valueComparer;

	private Bucket<KeyValuePair<TKey, TValue>> _bucket;

	private int _probing;

	public int Count => _bucket.Count;

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

	public IEqualityComparer<TKey> KeyComparer => _keyComparer;

	public ICollection<TKey> Keys => _keyCollection;

	public ICollection<TValue> Values => _valueCollection;

	public TValue this[TKey key]
	{
		get
		{
			if (TryGetValue(key, out var value))
			{
				return value;
			}
			throw new KeyNotFoundException();
		}
		set
		{
			Set(key, value);
		}
	}

	public SafeDictionary()
		: this((IEqualityComparer<TKey>)EqualityComparer<TKey>.Default, 1)
	{
	}

	public SafeDictionary(int initialProbing)
		: this((IEqualityComparer<TKey>)EqualityComparer<TKey>.Default, initialProbing)
	{
	}

	public SafeDictionary(IEqualityComparer<TKey> comparer)
		: this(comparer, 1)
	{
	}

	public SafeDictionary(IEqualityComparer<TKey> comparer, int initialProbing)
	{
		_keyComparer = comparer ?? EqualityComparer<TKey>.Default;
		_valueComparer = EqualityComparer<TValue>.Default;
		_bucket = new Bucket<KeyValuePair<TKey, TValue>>();
		_probing = initialProbing;
		_keyCollection = new KeyCollection<TKey, TValue>(this);
		_valueCollection = new ValueCollection<TKey, TValue>(this);
	}

	public void AddNew(TKey key, TValue value)
	{
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int hashCode = GetHashCode(key);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.Insert(hashCode + num, item, out var previous))
			{
				return;
			}
			if (_keyComparer.Equals(previous.Key, key))
			{
				break;
			}
			num++;
		}
		throw new ArgumentException("An item with the same key has already been added", "key");
	}

	public void Clear()
	{
		Interlocked.Exchange(ref _bucket, _bucket = new Bucket<KeyValuePair<TKey, TValue>>());
	}

	public IEnumerable<KeyValuePair<TKey, TValue>> ClearEnumerable()
	{
		return Interlocked.Exchange(ref _bucket, _bucket = new Bucket<KeyValuePair<TKey, TValue>>());
	}

	public bool ContainsKey(TKey key)
	{
		int hashCode = GetHashCode(key);
		for (int i = 0; i < _probing; i++)
		{
			if (_bucket.TryGet(hashCode + i, out var value) && _keyComparer.Equals(value.Key, key))
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsKey(int hashCode, Predicate<TKey> keyCheck)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		for (int i = 0; i < _probing; i++)
		{
			if (_bucket.TryGet(hashCode + i, out var value) && GetHashCode(value.Key) == hashCode && keyCheck(value.Key))
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsKey(int hashCode, Predicate<TKey> keyCheck, Predicate<TValue> valueCheck)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		if (valueCheck == null)
		{
			throw new ArgumentNullException("valueCheck");
		}
		for (int i = 0; i < _probing; i++)
		{
			if (_bucket.TryGet(hashCode + i, out var value) && GetHashCode(value.Key) == hashCode && keyCheck(value.Key) && valueCheck(value.Value))
			{
				return true;
			}
		}
		return false;
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		_bucket.CopyTo(array, arrayIndex);
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return _bucket.GetEnumerator();
	}

	public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
	{
		if (valueFactory == null)
		{
			throw new ArgumentNullException("valueFactory");
		}
		int hashCode = GetHashCode(key);
		int num = 0;
		KeyValuePair<TKey, TValue> stored;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.TryGetOrInsert(hashCode + num, () => new KeyValuePair<TKey, TValue>(key, valueFactory(key)), out stored))
			{
				return stored.Value;
			}
			if (_keyComparer.Equals(stored.Key, key))
			{
				break;
			}
			num++;
		}
		return stored.Value;
	}

	public TValue GetOrAdd(TKey key, TValue value)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		KeyValuePair<TKey, TValue> stored;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.TryGetOrInsert(hashCode + num, item, out stored))
			{
				return stored.Value;
			}
			if (_keyComparer.Equals(stored.Key, key))
			{
				break;
			}
			num++;
		}
		return stored.Value;
	}

	public IList<KeyValuePair<TKey, TValue>> GetPairs()
	{
		List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>(_bucket.Count);
		list.AddRange(_bucket);
		return list;
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		AddNew(item.Key, item.Value);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		int hashCode = GetHashCode(item.Key);
		for (int i = 0; i < _probing; i++)
		{
			if (_bucket.TryGet(hashCode + i, out var value) && _keyComparer.Equals(value.Key, item.Key))
			{
				if (_valueComparer.Equals(value.Value, item.Value))
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		int hashCode = GetHashCode(item.Key);
		for (int i = 0; i < _probing; i++)
		{
			bool done = false;
			bool result = _bucket.RemoveAt(hashCode + i, delegate(KeyValuePair<TKey, TValue> found)
			{
				if (_keyComparer.Equals(found.Key, item.Key))
				{
					done = true;
					if (_valueComparer.Equals(found.Value, item.Value))
					{
						return true;
					}
				}
				return false;
			});
			if (done)
			{
				return result;
			}
		}
		return false;
	}

	void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
	{
		AddNew(key, value);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Remove(TKey key)
	{
		int hashCode = GetHashCode(key);
		for (int i = 0; i < _probing; i++)
		{
			bool done = false;
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				if (_keyComparer.Equals(found.Key, key))
				{
					done = true;
					return true;
				}
				return false;
			};
			bool result = _bucket.RemoveAt(hashCode + i, check);
			if (done)
			{
				return result;
			}
		}
		return false;
	}

	public bool Remove(TKey key, out TValue value)
	{
		value = default(TValue);
		int hashCode = GetHashCode(key);
		for (int i = 0; i < _probing; i++)
		{
			bool done = false;
			KeyValuePair<TKey, TValue> previous = default(KeyValuePair<TKey, TValue>);
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				previous = found;
				if (_keyComparer.Equals(found.Key, key))
				{
					done = true;
					return true;
				}
				return false;
			};
			bool result = _bucket.RemoveAt(hashCode + i, check);
			if (done)
			{
				value = previous.Value;
				return result;
			}
		}
		return false;
	}

	public bool Remove(int hashCode, Predicate<TKey> keyCheck, out TValue value)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		value = default(TValue);
		for (int i = 0; i < _probing; i++)
		{
			bool done = false;
			KeyValuePair<TKey, TValue> previous = default(KeyValuePair<TKey, TValue>);
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				previous = found;
				if (GetHashCode(found.Key) == hashCode && keyCheck(found.Key))
				{
					done = true;
					return true;
				}
				return false;
			};
			bool result = _bucket.RemoveAt(hashCode + i, check);
			if (done)
			{
				value = previous.Value;
				return result;
			}
		}
		return false;
	}

	public bool Remove(TKey key, Predicate<TValue> valueCheck, out TValue value)
	{
		if (valueCheck == null)
		{
			throw new ArgumentNullException("valueCheck");
		}
		value = default(TValue);
		int hashCode = GetHashCode(key);
		for (int i = 0; i < _probing; i++)
		{
			bool done = false;
			KeyValuePair<TKey, TValue> previous = default(KeyValuePair<TKey, TValue>);
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				previous = found;
				if (_keyComparer.Equals(found.Key, key))
				{
					done = true;
					if (valueCheck(found.Value))
					{
						return true;
					}
				}
				return false;
			};
			bool result = _bucket.RemoveAt(hashCode + i, check);
			if (done)
			{
				value = previous.Value;
				return result;
			}
		}
		return false;
	}

	public bool Remove(int hashCode, Predicate<TKey> keyCheck, Predicate<TValue> valueCheck, out TValue value)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		if (valueCheck == null)
		{
			throw new ArgumentNullException("valueCheck");
		}
		value = default(TValue);
		for (int i = 0; i < _probing; i++)
		{
			bool done = false;
			KeyValuePair<TKey, TValue> previous = default(KeyValuePair<TKey, TValue>);
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				previous = found;
				if (GetHashCode(found.Key) == hashCode && keyCheck(found.Key))
				{
					done = true;
					if (valueCheck(found.Value))
					{
						return true;
					}
				}
				return false;
			};
			bool result = _bucket.RemoveAt(hashCode + i, check);
			if (done)
			{
				value = previous.Value;
				return result;
			}
		}
		return false;
	}

	public int RemoveWhereKey(Predicate<TKey> keyCheck)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		IEnumerable<KeyValuePair<TKey, TValue>> source = _bucket.Where((KeyValuePair<TKey, TValue> pair) => keyCheck(pair.Key));
		return source.Count((KeyValuePair<TKey, TValue> pair) => Remove(pair.Key));
	}

	public IEnumerable<TValue> RemoveWhereKeyEnumerable(Predicate<TKey> keyCheck)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		IEnumerable<KeyValuePair<TKey, TValue>> source = _bucket.Where((KeyValuePair<TKey, TValue> pair) => keyCheck(pair.Key));
		return from pair in source
			where Remove(pair.Key)
			select pair.Value;
	}

	public int RemoveWhereValue(Predicate<TValue> valueCheck)
	{
		if (valueCheck == null)
		{
			throw new ArgumentNullException("valueCheck");
		}
		IEnumerable<KeyValuePair<TKey, TValue>> source = _bucket.Where((KeyValuePair<TKey, TValue> pair) => valueCheck(pair.Value));
		return source.Count((KeyValuePair<TKey, TValue> pair) => Remove(pair.Key));
	}

	public IEnumerable<TValue> RemoveWhereValueEnumerable(Predicate<TValue> valueCheck)
	{
		if (valueCheck == null)
		{
			throw new ArgumentNullException("valueCheck");
		}
		IEnumerable<KeyValuePair<TKey, TValue>> source = _bucket.Where((KeyValuePair<TKey, TValue> pair) => valueCheck(pair.Value));
		return from pair in source
			where Remove(pair.Key)
			select pair.Value;
	}

	public void Set(TKey key, TValue value)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.InsertOrUpdate(hashCode + num, item, (KeyValuePair<TKey, TValue> found) => _keyComparer.Equals(found.Key, key), out var _))
			{
				break;
			}
			num++;
		}
	}

	public void Set(TKey key, TValue value, out bool isNew)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.InsertOrUpdate(hashCode + num, item, (KeyValuePair<TKey, TValue> found) => _keyComparer.Equals(found.Key, key), out isNew))
			{
				break;
			}
			num++;
		}
	}

	public bool TryAdd(TKey key, TValue value)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.Insert(hashCode + num, item, out var previous))
			{
				return true;
			}
			if (_keyComparer.Equals(previous.Key, key))
			{
				break;
			}
			num++;
		}
		return false;
	}

	public bool TryAdd(TKey key, TValue value, out KeyValuePair<TKey, TValue> stored)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> keyValuePair = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.Insert(hashCode + num, keyValuePair, out stored))
			{
				stored = keyValuePair;
				return true;
			}
			if (_keyComparer.Equals(stored.Key, key))
			{
				break;
			}
			num++;
		}
		return false;
	}

	public bool TryGetOrAdd(TKey key, TValue value, out TValue stored)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		KeyValuePair<TKey, TValue> stored2;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.TryGetOrInsert(hashCode + num, item, out stored2))
			{
				stored = stored2.Value;
				return true;
			}
			if (_keyComparer.Equals(stored2.Key, key))
			{
				break;
			}
			num++;
		}
		stored = stored2.Value;
		return false;
	}

	public bool TryGetOrAdd(TKey key, Func<TKey, TValue> valueFactory, out TValue stored)
	{
		if (valueFactory == null)
		{
			throw new ArgumentException("valueFactory");
		}
		int hashCode = GetHashCode(key);
		int num = 0;
		KeyValuePair<TKey, TValue> stored2;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.TryGetOrInsert(hashCode + num, () => new KeyValuePair<TKey, TValue>(key, valueFactory(key)), out stored2))
			{
				stored = stored2.Value;
				return true;
			}
			if (_keyComparer.Equals(stored2.Key, key))
			{
				break;
			}
			num++;
		}
		stored = stored2.Value;
		return false;
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		value = default(TValue);
		int hashCode = GetHashCode(key);
		for (int i = 0; i < _probing; i++)
		{
			if (_bucket.TryGet(hashCode + i, out var value2) && _keyComparer.Equals(value2.Key, key))
			{
				value = value2.Value;
				return true;
			}
		}
		return false;
	}

	public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, newValue);
		for (int i = 0; i < _probing; i++)
		{
			bool keyMatch = false;
			ExtendProbingIfNeeded(i);
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				keyMatch = _keyComparer.Equals(found.Key, key);
				return keyMatch && _valueComparer.Equals(found.Value, comparisonValue);
			};
			if (_bucket.Update(hashCode + i, item, check))
			{
				return true;
			}
			if (keyMatch)
			{
				return false;
			}
		}
		return false;
	}

	public bool TryUpdate(TKey key, TValue newValue, Predicate<TValue> valueCheck)
	{
		if (valueCheck == null)
		{
			throw new ArgumentNullException("valueCheck");
		}
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> insertPair = default(KeyValuePair<TKey, TValue>);
		ref KeyValuePair<TKey, TValue> reference = ref insertPair;
		reference = new KeyValuePair<TKey, TValue>(key, newValue);
		for (int i = 0; i < _probing; i++)
		{
			bool keyMatch = false;
			ExtendProbingIfNeeded(i);
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				keyMatch = _keyComparer.Equals(found.Key, key);
				return keyMatch && valueCheck(found.Value);
			};
			if (_bucket.Update(hashCode + i, (KeyValuePair<TKey, TValue> _) => insertPair, check, out var _))
			{
				return true;
			}
			if (keyMatch)
			{
				return false;
			}
		}
		return false;
	}

	public IEnumerable<TValue> Where(Predicate<TKey> keyCheck)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		IEnumerable<KeyValuePair<TKey, TValue>> source = _bucket.Where((KeyValuePair<TKey, TValue> pair) => keyCheck(pair.Key));
		return source.Select((KeyValuePair<TKey, TValue> pair) => pair.Value);
	}

	internal void AddNew(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				if (_keyComparer.Equals(found.Key, key))
				{
					throw CreateKeyArgumentException(null);
				}
				return keyOverwriteCheck(found.Key);
			};
			_bucket.InsertOrUpdate(hashCode + num, item, check, out var isNew);
			if (isNew)
			{
				break;
			}
			num++;
		}
	}

	internal void Set(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			Predicate<KeyValuePair<TKey, TValue>> check = (KeyValuePair<TKey, TValue> found) => _keyComparer.Equals(found.Key, key) || keyOverwriteCheck(found.Key);
			if (_bucket.InsertOrUpdate(hashCode + num, item, check, out var _))
			{
				break;
			}
			num++;
		}
	}

	internal void Set(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value, out bool isNew)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			Predicate<KeyValuePair<TKey, TValue>> check = (KeyValuePair<TKey, TValue> found) => _keyComparer.Equals(found.Key, key) || keyOverwriteCheck(found.Key);
			if (_bucket.InsertOrUpdate(hashCode + num, item, check, out isNew))
			{
				break;
			}
			num++;
		}
	}

	internal bool TryAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				if (_keyComparer.Equals(found.Key, key))
				{
					throw CreateKeyArgumentException(null);
				}
				return keyOverwriteCheck(found.Key);
			};
			try
			{
				_bucket.InsertOrUpdate(hashCode + num, item, check, out var isNew);
				if (isNew)
				{
					return true;
				}
			}
			catch (ArgumentException)
			{
				return false;
			}
			num++;
		}
	}

	internal bool TryGetOrAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value, out TValue stored)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				if (_keyComparer.Equals(found.Key, key))
				{
					value = found.Value;
					throw CreateKeyArgumentException(null);
				}
				return keyOverwriteCheck(found.Key);
			};
			try
			{
				_bucket.InsertOrUpdate(hashCode + num, item, check, out var isNew);
				if (isNew)
				{
					stored = value;
					return true;
				}
			}
			catch (ArgumentException)
			{
				stored = value;
				return false;
			}
			num++;
		}
	}

	private void ExtendProbingIfNeeded(int attempts)
	{
		int num = 1 + attempts - _probing;
		if (num > 0)
		{
			Interlocked.Add(ref _probing, num);
		}
	}

	private int GetHashCode(TKey key)
	{
		int num = _keyComparer.GetHashCode(key);
		if (num < 0)
		{
			num = -num;
		}
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
	{
		if (addValueFactory == null)
		{
			throw new ArgumentNullException("addValueFactory");
		}
		if (updateValueFactory == null)
		{
			throw new ArgumentNullException("updateValueFactory");
		}
		int hashCode = GetHashCode(key);
		int num = 0;
		KeyValuePair<TKey, TValue> insertPair = default(KeyValuePair<TKey, TValue>);
		KeyValuePair<TKey, TValue> updatePair = default(KeyValuePair<TKey, TValue>);
		bool isNew;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			Func<KeyValuePair<TKey, TValue>> itemFactory = delegate
			{
				ref KeyValuePair<TKey, TValue> reference = ref insertPair;
				reference = new KeyValuePair<TKey, TValue>(key, addValueFactory(key));
				return reference;
			};
			Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> itemUpdateFactory = delegate(KeyValuePair<TKey, TValue> found)
			{
				ref KeyValuePair<TKey, TValue> reference = ref updatePair;
				reference = new KeyValuePair<TKey, TValue>(key, updateValueFactory(found.Key, found.Value));
				return reference;
			};
			Predicate<KeyValuePair<TKey, TValue>> check = (KeyValuePair<TKey, TValue> found) => _keyComparer.Equals(key, found.Key);
			if (_bucket.InsertOrUpdate(hashCode + num, itemFactory, itemUpdateFactory, check, out isNew))
			{
				break;
			}
			num++;
		}
		if (!isNew)
		{
			return updatePair.Value;
		}
		return insertPair.Value;
	}

	public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
	{
		if (updateValueFactory == null)
		{
			throw new ArgumentNullException("updateValueFactory");
		}
		int hashCode = GetHashCode(key);
		int num = 0;
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, addValue);
		KeyValuePair<TKey, TValue> updatePair = default(KeyValuePair<TKey, TValue>);
		bool isNew;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> itemUpdateFactory = delegate(KeyValuePair<TKey, TValue> found)
			{
				ref KeyValuePair<TKey, TValue> reference = ref updatePair;
				reference = new KeyValuePair<TKey, TValue>(key, updateValueFactory(found.Key, found.Value));
				return reference;
			};
			Predicate<KeyValuePair<TKey, TValue>> check = (KeyValuePair<TKey, TValue> found) => _keyComparer.Equals(key, found.Key);
			if (_bucket.InsertOrUpdate(hashCode + num, item, itemUpdateFactory, check, out isNew))
			{
				break;
			}
			num++;
		}
		if (!isNew)
		{
			return updatePair.Value;
		}
		return item.Value;
	}

	public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory, out bool isNew)
	{
		if (addValueFactory == null)
		{
			throw new ArgumentNullException("addValueFactory");
		}
		if (updateValueFactory == null)
		{
			throw new ArgumentNullException("updateValueFactory");
		}
		int hashCode = GetHashCode(key);
		int num = 0;
		KeyValuePair<TKey, TValue> insertPair = default(KeyValuePair<TKey, TValue>);
		KeyValuePair<TKey, TValue> updatePair = default(KeyValuePair<TKey, TValue>);
		while (true)
		{
			ExtendProbingIfNeeded(num);
			Func<KeyValuePair<TKey, TValue>> itemFactory = delegate
			{
				ref KeyValuePair<TKey, TValue> reference = ref insertPair;
				reference = new KeyValuePair<TKey, TValue>(key, addValueFactory(key));
				return reference;
			};
			Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> itemUpdateFactory = delegate(KeyValuePair<TKey, TValue> found)
			{
				ref KeyValuePair<TKey, TValue> reference = ref updatePair;
				reference = new KeyValuePair<TKey, TValue>(key, updateValueFactory(found.Key, found.Value));
				return reference;
			};
			Predicate<KeyValuePair<TKey, TValue>> check = (KeyValuePair<TKey, TValue> found) => _keyComparer.Equals(key, found.Key);
			if (_bucket.InsertOrUpdate(hashCode + num, itemFactory, itemUpdateFactory, check, out isNew))
			{
				break;
			}
			num++;
		}
		if (!isNew)
		{
			return updatePair.Value;
		}
		return insertPair.Value;
	}

	public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory, out bool isNew)
	{
		if (object.ReferenceEquals(updateValueFactory, null))
		{
			throw new ArgumentNullException("updateValueFactory");
		}
		int hashCode = GetHashCode(key);
		int num = 0;
		KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, addValue);
		KeyValuePair<TKey, TValue> updatePair = default(KeyValuePair<TKey, TValue>);
		while (true)
		{
			ExtendProbingIfNeeded(num);
			Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> itemUpdateFactory = delegate(KeyValuePair<TKey, TValue> found)
			{
				ref KeyValuePair<TKey, TValue> reference = ref updatePair;
				reference = new KeyValuePair<TKey, TValue>(key, updateValueFactory(found.Key, found.Value));
				return reference;
			};
			Predicate<KeyValuePair<TKey, TValue>> check = (KeyValuePair<TKey, TValue> found) => _keyComparer.Equals(key, found.Key);
			if (_bucket.InsertOrUpdate(hashCode + num, item, itemUpdateFactory, check, out isNew))
			{
				break;
			}
			num++;
		}
		if (!isNew)
		{
			return updatePair.Value;
		}
		return item.Value;
	}

	public bool TryGetValue(int hashCode, Predicate<TKey> keyCheck, out TValue value)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		value = default(TValue);
		for (int i = 0; i < _probing; i++)
		{
			if (_bucket.TryGet(hashCode + i, out var value2) && GetHashCode(value2.Key) == hashCode && keyCheck(value2.Key))
			{
				value = value2.Value;
				return true;
			}
		}
		return false;
	}

	internal bool TryAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value, out KeyValuePair<TKey, TValue> stored)
	{
		int hashCode = GetHashCode(key);
		KeyValuePair<TKey, TValue> keyValuePair = new KeyValuePair<TKey, TValue>(key, value);
		int num = 0;
		while (true)
		{
			KeyValuePair<TKey, TValue> foundPair = keyValuePair;
			ExtendProbingIfNeeded(num);
			Predicate<KeyValuePair<TKey, TValue>> check = delegate(KeyValuePair<TKey, TValue> found)
			{
				foundPair = found;
				if (_keyComparer.Equals(foundPair.Key, key))
				{
					throw CreateKeyArgumentException(null);
				}
				return keyOverwriteCheck(foundPair.Key);
			};
			try
			{
				_bucket.InsertOrUpdate(hashCode + num, keyValuePair, check, out var isNew);
				if (isNew)
				{
					stored = keyValuePair;
					return true;
				}
			}
			catch (ArgumentException)
			{
				stored = foundPair;
				return false;
			}
			num++;
		}
	}

	internal bool TryGetOrAdd(TKey key, Func<TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory, out TValue stored)
	{
		int hashCode = GetHashCode(key);
		int num = 0;
		while (true)
		{
			TValue value = default(TValue);
			ExtendProbingIfNeeded(num);
			Func<KeyValuePair<TKey, TValue>> itemFactory = () => new KeyValuePair<TKey, TValue>(key, value = addValueFactory());
			Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> itemUpdateFactory = delegate(KeyValuePair<TKey, TValue> found)
			{
				if (_keyComparer.Equals(found.Key, key))
				{
					value = found.Value;
					throw CreateKeyArgumentException(null);
				}
				value = updateValueFactory(found.Key, found.Value);
				return new KeyValuePair<TKey, TValue>(key, value);
			};
			try
			{
				_bucket.InsertOrUpdate(hashCode + num, itemFactory, itemUpdateFactory, out var isNew);
				if (isNew)
				{
					stored = value;
					return true;
				}
			}
			catch (ArgumentException)
			{
				stored = value;
				return false;
			}
			num++;
		}
	}

	private static ArgumentException CreateKeyArgumentException(object key)
	{
		GC.KeepAlive(key);
		return new ArgumentException("An item with the same key has already been added", "key");
	}
}
