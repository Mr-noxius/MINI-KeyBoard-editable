using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public sealed class FixedSizeBucket<T> : IBucket<T>, IEnumerable<T>, IEnumerable
{
	private readonly int _capacity;

	private int _count;

	private object[] _entries;

	public int Capacity => _capacity;

	public int Count => _count;

	public FixedSizeBucket(int capacity)
	{
		_count = 0;
		_entries = ArrayReservoir<object>.GetArray(capacity);
		_capacity = _entries.Length;
	}

	public FixedSizeBucket(IEnumerable<T> source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		_entries = ArrayReservoir<object>.GetArray((source as ICollection<T>)?.Count ?? 64);
		_capacity = _entries.Length;
		foreach (T item in source)
		{
			if (_count == _capacity)
			{
				object[] entries = _entries;
				_entries = ArrayReservoir<object>.GetArray(_capacity << 1);
				if (entries != null)
				{
					Array.Copy(entries, 0, _entries, 0, _count);
					ArrayReservoir<object>.DonateArray(entries);
				}
				_capacity = _entries.Length;
			}
			_entries[_count] = ((object)item) ?? BucketHelper.Null;
			_count++;
		}
	}

	public FixedSizeBucket(T[] source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		_entries = ArrayReservoir<object>.GetArray(source.Length);
		_capacity = _entries.Length;
		foreach (T val in source)
		{
			_entries[_count] = ((object)val) ?? BucketHelper.Null;
			_count++;
		}
	}

	~FixedSizeBucket()
	{
		if (!GCMonitor.FinalizingForUnload)
		{
			object[] entries = _entries;
			if (entries != null)
			{
				ArrayReservoir<object>.DonateArray(entries);
				_entries = null;
			}
		}
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (_count > array.Length - arrayIndex)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
		try
		{
			object[] entries = _entries;
			foreach (object obj in entries)
			{
				if (obj != null)
				{
					if (obj == BucketHelper.Null)
					{
						array[arrayIndex] = default(T);
					}
					else
					{
						array[arrayIndex] = (T)obj;
					}
					arrayIndex++;
				}
			}
		}
		catch (IndexOutOfRangeException ex)
		{
			throw new ArgumentOutOfRangeException("array", ex.Message);
		}
	}

	public bool Exchange(int index, T item, out T previous)
	{
		if (index < 0 || index >= _capacity)
		{
			throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
		}
		return ExchangeInternal(index, item, out previous);
	}

	public IEnumerator<T> GetEnumerator()
	{
		try
		{
			object[] entries = _entries;
			foreach (object entry in entries)
			{
				if (entry != null)
				{
					if (entry == BucketHelper.Null)
					{
						yield return default(T);
					}
					else
					{
						yield return (T)entry;
					}
				}
			}
		}
		finally
		{
		}
	}

	public bool Insert(int index, T item)
	{
		if (index < 0 || index >= _capacity)
		{
			throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity.");
		}
		return InsertInternal(index, item);
	}

	public bool Insert(int index, T item, out T previous)
	{
		if (index < 0 || index >= _capacity)
		{
			throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
		}
		return InsertInternal(index, item, out previous);
	}

	public bool RemoveAt(int index)
	{
		if (index < 0 || index >= _capacity)
		{
			throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
		}
		object obj = Interlocked.Exchange(ref _entries[index], null);
		if (obj == null)
		{
			return false;
		}
		Interlocked.Decrement(ref _count);
		return true;
	}

	public bool RemoveAt(int index, out T previous)
	{
		if (index < 0 || index >= _capacity)
		{
			throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
		}
		return RemoveAtInternal(index, out previous);
	}

	public bool RemoveAt(int index, Predicate<T> check)
	{
		if (index < 0 || index >= _capacity)
		{
			throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
		}
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		object obj = Interlocked.CompareExchange(ref _entries[index], null, null);
		if (obj != null)
		{
			T obj2 = ((obj == BucketHelper.Null) ? default(T) : ((T)obj));
			if (check(obj2))
			{
				object obj3 = Interlocked.CompareExchange(ref _entries[index], null, obj);
				if (obj == obj3)
				{
					Interlocked.Decrement(ref _count);
					return true;
				}
			}
		}
		return false;
	}

	public void Set(int index, T item, out bool isNew)
	{
		if (index < 0 || index >= _capacity)
		{
			throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
		}
		SetInternal(index, item, out isNew);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool TryGet(int index, out T value)
	{
		if (index < 0 || index >= _capacity)
		{
			throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
		}
		return TryGetInternal(index, out value);
	}

	public bool Update(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty)
	{
		if (index < 0 || index >= _capacity)
		{
			throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity.");
		}
		if (itemUpdateFactory == null)
		{
			throw new ArgumentNullException("itemUpdateFactory");
		}
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		return UpdateInternal(index, itemUpdateFactory, check, out isEmpty);
	}

	public IEnumerable<T> Where(Predicate<T> check)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		try
		{
			object[] entries = _entries;
			foreach (object entry in entries)
			{
				if (entry != null)
				{
					T yield = default(T);
					if (entry != BucketHelper.Null)
					{
						yield = (T)entry;
					}
					if (check(yield))
					{
						yield return yield;
					}
				}
			}
		}
		finally
		{
		}
	}

	internal bool ExchangeInternal(int index, T item, out T previous)
	{
		previous = default(T);
		object obj = Interlocked.Exchange(ref _entries[index], ((object)item) ?? BucketHelper.Null);
		if (obj == null)
		{
			Interlocked.Increment(ref _count);
			return true;
		}
		if (obj != BucketHelper.Null)
		{
			previous = (T)obj;
		}
		return false;
	}

	internal bool InsertInternal(int index, T item, out T previous)
	{
		previous = default(T);
		object obj = Interlocked.CompareExchange(ref _entries[index], ((object)item) ?? BucketHelper.Null, null);
		if (obj == null)
		{
			Interlocked.Increment(ref _count);
			return true;
		}
		if (obj != BucketHelper.Null)
		{
			previous = (T)obj;
		}
		return false;
	}

	internal bool InsertInternal(int index, T item)
	{
		object obj = Interlocked.CompareExchange(ref _entries[index], ((object)item) ?? BucketHelper.Null, null);
		if (obj == null)
		{
			Interlocked.Increment(ref _count);
			return true;
		}
		return false;
	}

	internal bool RemoveAtInternal(int index, out T previous)
	{
		previous = default(T);
		object obj = Interlocked.Exchange(ref _entries[index], null);
		if (obj == null)
		{
			return false;
		}
		Interlocked.Decrement(ref _count);
		if (obj != BucketHelper.Null)
		{
			previous = (T)obj;
		}
		return true;
	}

	internal void SetInternal(int index, T item, out bool isNew)
	{
		isNew = Interlocked.Exchange(ref _entries[index], ((object)item) ?? BucketHelper.Null) == null;
		if (isNew)
		{
			Interlocked.Increment(ref _count);
		}
	}

	internal bool TryGetInternal(int index, out T value)
	{
		object obj = Interlocked.CompareExchange(ref _entries[index], null, null);
		if (obj == null)
		{
			value = default(T);
			return false;
		}
		if (obj == BucketHelper.Null)
		{
			value = default(T);
		}
		else
		{
			value = (T)obj;
		}
		return true;
	}

	internal bool UpdateInternal(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty)
	{
		object obj = Interlocked.CompareExchange(ref _entries[index], null, null);
		object obj2 = BucketHelper.Null;
		bool result = false;
		if (obj != null)
		{
			T val = ((obj == BucketHelper.Null) ? default(T) : ((T)obj));
			if (check(val))
			{
				T val2 = itemUpdateFactory(val);
				obj2 = Interlocked.CompareExchange(ref _entries[index], ((object)val2) ?? BucketHelper.Null, obj);
				result = obj == obj2;
			}
		}
		isEmpty = obj == null || obj2 == null;
		return result;
	}
}
