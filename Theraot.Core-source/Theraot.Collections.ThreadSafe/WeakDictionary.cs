using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Theraot.Collections.Specialized;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe;

[DebuggerNonUserCode]
[DebuggerDisplay("Count={Count}")]
public class WeakDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable where TKey : class
{
	private readonly KeyCollection<TKey, TValue> _keyCollection;

	private readonly IEqualityComparer<TKey> _keyComparer;

	private readonly ValueCollection<TKey, TValue> _valueCollection;

	private readonly SafeDictionary<WeakNeedle<TKey>, TValue> _wrapped;

	private readonly NeedleReservoir<TKey, WeakNeedle<TKey>> _reservoir;

	private EventHandler _handle;

	public bool AutoRemoveDeadItems
	{
		get
		{
			return _handle != null;
		}
		set
		{
			EventHandler handle = _handle;
			if (value)
			{
				EventHandler value2 = delegate
				{
					RemoveDeadItems();
				};
				if (handle == null && Interlocked.CompareExchange(ref _handle, value2, null) == null)
				{
					GCMonitor.Collected += value2;
				}
			}
			else if (handle != null && Interlocked.CompareExchange(ref _handle, null, handle) == handle)
			{
				GCMonitor.Collected -= handle;
			}
		}
	}

	public int Count => _wrapped.Count;

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

	public IEqualityComparer<TKey> KeyComparer => _keyComparer;

	public ICollection<TKey> Keys => _keyCollection;

	public ICollection<TValue> Values => _valueCollection;

	protected SafeDictionary<WeakNeedle<TKey>, TValue> Wrapped => _wrapped;

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

	public WeakDictionary()
		: this((IEqualityComparer<TKey>)null)
	{
	}

	public WeakDictionary(IEqualityComparer<TKey> comparer)
	{
		_keyComparer = comparer ?? EqualityComparer<TKey>.Default;
		NeedleConversionEqualityComparer<WeakNeedle<TKey>, TKey> comparer2 = new NeedleConversionEqualityComparer<WeakNeedle<TKey>, TKey>(_keyComparer);
		_wrapped = new SafeDictionary<WeakNeedle<TKey>, TValue>(comparer2);
		_keyCollection = new KeyCollection<TKey, TValue>(this);
		_valueCollection = new ValueCollection<TKey, TValue>(this);
		_reservoir = new NeedleReservoir<TKey, WeakNeedle<TKey>>((TKey key) => new WeakNeedle<TKey>(key));
	}

	public WeakDictionary(IEqualityComparer<TKey> comparer, int initialProbing)
	{
		_keyComparer = comparer ?? EqualityComparer<TKey>.Default;
		NeedleConversionEqualityComparer<WeakNeedle<TKey>, TKey> comparer2 = new NeedleConversionEqualityComparer<WeakNeedle<TKey>, TKey>(_keyComparer);
		_wrapped = new SafeDictionary<WeakNeedle<TKey>, TValue>(comparer2, initialProbing);
		_keyCollection = new KeyCollection<TKey, TValue>(this);
		_valueCollection = new ValueCollection<TKey, TValue>(this);
		_reservoir = new NeedleReservoir<TKey, WeakNeedle<TKey>>((TKey key) => new WeakNeedle<TKey>(key));
	}

	void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
	{
		AddNew(key, value);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		AddNew(item.Key, item.Value);
	}

	public void AddNew(TKey key, TValue value)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		try
		{
			_wrapped.AddNew(weakNeedle, (WeakNeedle<TKey> input) => !input.IsAlive, value);
		}
		catch (ArgumentException)
		{
			_reservoir.DonateNeedle(weakNeedle);
			throw;
		}
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
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		Func<WeakNeedle<TKey>, TValue, TValue> updateValueFactory2 = (WeakNeedle<TKey> pairKey, TValue foundValue) => PrivateTryGetValue(pairKey, out var foundKey) ? updateValueFactory(foundKey, foundValue) : addValueFactory(key);
		Func<WeakNeedle<TKey>, TValue> addValueFactory2 = (WeakNeedle<TKey> input) => addValueFactory(key);
		TValue result = _wrapped.AddOrUpdate(weakNeedle, addValueFactory2, updateValueFactory2, out var isNew);
		if (!isNew)
		{
			_reservoir.DonateNeedle(weakNeedle);
		}
		return result;
	}

	public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
	{
		if (updateValueFactory == null)
		{
			throw new ArgumentNullException("updateValueFactory");
		}
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		Func<WeakNeedle<TKey>, TValue, TValue> updateValueFactory2 = (WeakNeedle<TKey> pairKey, TValue foundValue) => PrivateTryGetValue(pairKey, out var foundKey) ? updateValueFactory(foundKey, foundValue) : addValue;
		TValue result = _wrapped.AddOrUpdate(weakNeedle, addValue, updateValueFactory2, out var isNew);
		if (!isNew)
		{
			_reservoir.DonateNeedle(weakNeedle);
		}
		return result;
	}

	public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory, out bool added)
	{
		if (addValueFactory == null)
		{
			throw new ArgumentNullException("addValueFactory");
		}
		if (updateValueFactory == null)
		{
			throw new ArgumentNullException("updateValueFactory");
		}
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		Func<WeakNeedle<TKey>, TValue, TValue> updateValueFactory2 = (WeakNeedle<TKey> pairKey, TValue foundValue) => PrivateTryGetValue(pairKey, out var foundKey) ? updateValueFactory(foundKey, foundValue) : addValueFactory(key);
		Func<WeakNeedle<TKey>, TValue> addValueFactory2 = (WeakNeedle<TKey> input) => addValueFactory(key);
		TValue result = _wrapped.AddOrUpdate(weakNeedle, addValueFactory2, updateValueFactory2, out added);
		if (!added)
		{
			_reservoir.DonateNeedle(weakNeedle);
		}
		return result;
	}

	public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory, out bool added)
	{
		if (updateValueFactory == null)
		{
			throw new ArgumentNullException("updateValueFactory");
		}
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		Func<WeakNeedle<TKey>, TValue, TValue> updateValueFactory2 = (WeakNeedle<TKey> pairKey, TValue foundValue) => PrivateTryGetValue(pairKey, out var foundKey) ? updateValueFactory(foundKey, foundValue) : addValue;
		TValue result = _wrapped.AddOrUpdate(weakNeedle, addValue, updateValueFactory2, out added);
		if (!added)
		{
			_reservoir.DonateNeedle(weakNeedle);
		}
		return result;
	}

	public void Clear()
	{
		foreach (KeyValuePair<WeakNeedle<TKey>, TValue> item in _wrapped.ClearEnumerable())
		{
			_reservoir.DonateNeedle(item.Key);
		}
	}

	public IEnumerable<KeyValuePair<TKey, TValue>> ClearEnumerable()
	{
		foreach (KeyValuePair<WeakNeedle<TKey>, TValue> item in _wrapped.ClearEnumerable())
		{
			KeyValuePair<WeakNeedle<TKey>, TValue> keyValuePair = item;
			if (PrivateTryGetValue(keyValuePair.Key, out var foundKey))
			{
				KeyValuePair<WeakNeedle<TKey>, TValue> keyValuePair2 = item;
				yield return new KeyValuePair<TKey, TValue>(value: keyValuePair2.Value, key: foundKey);
				NeedleReservoir<TKey, WeakNeedle<TKey>> reservoir = _reservoir;
				KeyValuePair<WeakNeedle<TKey>, TValue> keyValuePair3 = item;
				reservoir.DonateNeedle(keyValuePair3.Key);
			}
		}
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		Predicate<WeakNeedle<TKey>> keyCheck = (WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && _keyComparer.Equals(foundKey, item.Key);
		return _wrapped.ContainsKey(_keyComparer.GetHashCode(item.Key), keyCheck, (TValue input) => EqualityComparer<TValue>.Default.Equals(input, item.Value));
	}

	public bool ContainsKey(TKey key)
	{
		return _wrapped.ContainsKey(_keyComparer.GetHashCode(key), (WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && _keyComparer.Equals(foundKey, key));
	}

	public bool ContainsKey(int hashCode, Predicate<TKey> keyCheck)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		return _wrapped.ContainsKey(hashCode, (WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && keyCheck(foundKey));
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
		return _wrapped.ContainsKey(hashCode, (WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && keyCheck(foundKey), valueCheck);
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (_wrapped.Count > array.Length - arrayIndex)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
		GetPairs().CopyTo(array, arrayIndex);
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		foreach (KeyValuePair<WeakNeedle<TKey>, TValue> pair in _wrapped)
		{
			KeyValuePair<WeakNeedle<TKey>, TValue> keyValuePair = pair;
			if (PrivateTryGetValue(keyValuePair.Key, out var foundKey))
			{
				TKey key = foundKey;
				KeyValuePair<WeakNeedle<TKey>, TValue> keyValuePair2 = pair;
				yield return new KeyValuePair<TKey, TValue>(key, keyValuePair2.Value);
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public TValue GetOrAdd(TKey key, TValue value)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		if (!_wrapped.TryGetOrAdd(weakNeedle, (WeakNeedle<TKey> input) => !input.IsAlive, value, out var stored))
		{
			_reservoir.DonateNeedle(weakNeedle);
		}
		return stored;
	}

	public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
	{
		if (valueFactory == null)
		{
			throw new ArgumentNullException("valueFactory");
		}
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		TValue result;
		Func<WeakNeedle<TKey>, TValue, TValue> updateValueFactory = (WeakNeedle<TKey> pairKey, TValue foundValue) => result = valueFactory(PrivateTryGetValue(pairKey, out var foundKey) ? foundKey : key);
		if (_wrapped.TryGetOrAdd(weakNeedle, () => valueFactory(key), updateValueFactory, out result))
		{
			return result;
		}
		_reservoir.DonateNeedle(weakNeedle);
		return result;
	}

	public IList<KeyValuePair<TKey, TValue>> GetPairs()
	{
		List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>(_wrapped.Count);
		foreach (KeyValuePair<WeakNeedle<TKey>, TValue> item in _wrapped)
		{
			if (PrivateTryGetValue(item.Key, out var foundKey))
			{
				TValue value = item.Value;
				list.Add(new KeyValuePair<TKey, TValue>(foundKey, value));
			}
		}
		return list;
	}

	public bool Remove(TKey key, Predicate<TValue> valueCheck, out TValue value)
	{
		return _wrapped.Remove(_keyComparer.GetHashCode(key), (WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && _keyComparer.Equals(foundKey, key), valueCheck, out value);
	}

	public bool Remove(TKey key)
	{
		TValue value;
		return _wrapped.Remove(_keyComparer.GetHashCode(key), (WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && _keyComparer.Equals(foundKey, key), out value);
	}

	public bool Remove(TKey key, out TValue value)
	{
		return _wrapped.Remove(_keyComparer.GetHashCode(key), (WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && _keyComparer.Equals(foundKey, key), out value);
	}

	public bool Remove(int hashCode, Predicate<TKey> keyCheck, out TValue value)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		return _wrapped.Remove(hashCode, (WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && keyCheck(foundKey), out value);
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
		return _wrapped.Remove(hashCode, (WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && keyCheck(foundKey), valueCheck, out value);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		Predicate<WeakNeedle<TKey>> keyCheck = (WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && _keyComparer.Equals(foundKey, item.Key);
		TValue value;
		return _wrapped.Remove(_keyComparer.GetHashCode(item.Key), keyCheck, (TValue input) => EqualityComparer<TValue>.Default.Equals(input, item.Value), out value);
	}

	public int RemoveDeadItems()
	{
		return _wrapped.RemoveWhereKey((WeakNeedle<TKey> key) => !key.IsAlive);
	}

	public int RemoveWhereKey(Predicate<TKey> keyCheck)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		return _wrapped.RemoveWhereKey((WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && keyCheck(foundKey));
	}

	public IEnumerable<TValue> RemoveWhereKeyEnumerable(Predicate<TKey> keyCheck)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		return _wrapped.RemoveWhereKeyEnumerable((WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && keyCheck(foundKey));
	}

	public int RemoveWhereValue(Predicate<TValue> valueCheck)
	{
		if (valueCheck == null)
		{
			throw new ArgumentNullException("valueCheck");
		}
		return _wrapped.RemoveWhereValue(valueCheck);
	}

	public IEnumerable<TValue> RemoveWhereValueEnumerable(Predicate<TValue> valueCheck)
	{
		if (valueCheck == null)
		{
			throw new ArgumentNullException("valueCheck");
		}
		return _wrapped.RemoveWhereValueEnumerable(valueCheck);
	}

	public void Set(TKey key, TValue value)
	{
		WeakNeedle<TKey> key2 = PrivateGetNeedle(key);
		_wrapped.Set(key2, (WeakNeedle<TKey> input) => !input.IsAlive, value);
	}

	public void Set(TKey key, TValue value, out bool isNew)
	{
		WeakNeedle<TKey> key2 = PrivateGetNeedle(key);
		_wrapped.Set(key2, (WeakNeedle<TKey> input) => !input.IsAlive, value, out isNew);
	}

	public bool TryAdd(TKey key, TValue value)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		if (_wrapped.TryAdd(weakNeedle, (WeakNeedle<TKey> input) => !input.IsAlive, value))
		{
			return true;
		}
		_reservoir.DonateNeedle(weakNeedle);
		return false;
	}

	public bool TryAdd(TKey key, TValue value, out KeyValuePair<TKey, TValue> stored)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		Predicate<WeakNeedle<TKey>> keyOverwriteCheck = delegate(WeakNeedle<TKey> found)
		{
			if (PrivateTryGetValue(found, out var foundKey))
			{
				key = foundKey;
				return false;
			}
			return true;
		};
		bool flag = _wrapped.TryAdd(weakNeedle, keyOverwriteCheck, value, out var stored2);
		if (!flag)
		{
			_reservoir.DonateNeedle(weakNeedle);
		}
		stored = new KeyValuePair<TKey, TValue>(key, stored2.Value);
		return flag;
	}

	public bool TryGetOrAdd(TKey key, Func<TKey, TValue> valueFactory, out TValue stored)
	{
		if (valueFactory == null)
		{
			throw new ArgumentNullException("valueFactory");
		}
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		Func<WeakNeedle<TKey>, TValue, TValue> updateValueFactory = (WeakNeedle<TKey> pairKey, TValue foundValue) => valueFactory(PrivateTryGetValue(pairKey, out var foundKey) ? foundKey : key);
		if (_wrapped.TryGetOrAdd(weakNeedle, () => valueFactory(key), updateValueFactory, out stored))
		{
			return true;
		}
		_reservoir.DonateNeedle(weakNeedle);
		return false;
	}

	public bool TryGetOrAdd(TKey key, TValue value, out TValue stored)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		if (_wrapped.TryGetOrAdd(weakNeedle, (WeakNeedle<TKey> input) => !input.IsAlive, value, out stored))
		{
			return true;
		}
		_reservoir.DonateNeedle(weakNeedle);
		return false;
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		Predicate<WeakNeedle<TKey>> keyCheck = (WeakNeedle<TKey> found) => PrivateTryGetValue(found, out var foundKey) && _keyComparer.Equals(key, foundKey);
		return _wrapped.TryGetValue(_keyComparer.GetHashCode(key), keyCheck, out value);
	}

	public bool TryGetValue(int hashCode, Predicate<TKey> keyCheck, out TValue value)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		Predicate<WeakNeedle<TKey>> keyCheck2 = (WeakNeedle<TKey> found) => PrivateTryGetValue(found, out var foundKey) && keyCheck(foundKey);
		return _wrapped.TryGetValue(hashCode, keyCheck2, out value);
	}

	public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		if (_wrapped.TryUpdate(weakNeedle, newValue, comparisonValue))
		{
			return true;
		}
		_reservoir.DonateNeedle(weakNeedle);
		return false;
	}

	public bool TryUpdate(TKey key, TValue newValue, Predicate<TValue> valueCheck)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		if (_wrapped.TryUpdate(weakNeedle, newValue, valueCheck))
		{
			return true;
		}
		_reservoir.DonateNeedle(weakNeedle);
		return false;
	}

	public IEnumerable<TValue> Where(Predicate<TKey> keyCheck)
	{
		if (keyCheck == null)
		{
			throw new ArgumentNullException("keyCheck");
		}
		return _wrapped.Where((WeakNeedle<TKey> input) => PrivateTryGetValue(input, out var foundKey) && keyCheck(foundKey));
	}

	internal void AddNew(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		try
		{
			_wrapped.AddNew(weakNeedle, (WeakNeedle<TKey> input) => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value);
		}
		catch (ArgumentException)
		{
			_reservoir.DonateNeedle(weakNeedle);
			throw;
		}
	}

	internal TValue GetOrAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		if (!_wrapped.TryGetOrAdd(weakNeedle, (WeakNeedle<TKey> input) => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value, out var stored))
		{
			_reservoir.DonateNeedle(weakNeedle);
		}
		return stored;
	}

	internal void Set(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
	{
		WeakNeedle<TKey> key2 = PrivateGetNeedle(key);
		_wrapped.Set(key2, (WeakNeedle<TKey> input) => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value);
	}

	internal void Set(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value, out bool isNew)
	{
		WeakNeedle<TKey> key2 = PrivateGetNeedle(key);
		_wrapped.Set(key2, (WeakNeedle<TKey> input) => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value, out isNew);
	}

	internal bool TryAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		if (_wrapped.TryAdd(weakNeedle, (WeakNeedle<TKey> input) => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value))
		{
			return true;
		}
		_reservoir.DonateNeedle(weakNeedle);
		return false;
	}

	internal bool TryGetOrAdd(TKey key, Predicate<TKey> keyOverwriteCheck, TValue value, out TValue stored)
	{
		WeakNeedle<TKey> weakNeedle = PrivateGetNeedle(key);
		if (_wrapped.TryGetOrAdd(weakNeedle, (WeakNeedle<TKey> input) => !PrivateTryGetValue(input, out var foundKey) || keyOverwriteCheck(foundKey), value, out stored))
		{
			return true;
		}
		_reservoir.DonateNeedle(weakNeedle);
		return false;
	}

	protected bool Contains(KeyValuePair<TKey, TValue> item)
	{
		return ((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(item);
	}

	private WeakNeedle<TKey> PrivateGetNeedle(TKey key)
	{
		return _reservoir.GetNeedle(key);
	}

	private static bool PrivateTryGetValue(WeakNeedle<TKey> needle, out TKey foundKey)
	{
		return needle.TryGetValue(out foundKey);
	}
}
