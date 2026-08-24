using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public sealed class Bucket<T> : IBucket<T>, IEnumerable<T>, IEnumerable
{
	private readonly BucketCore _bucketCore;

	private int _count;

	public int Count => _count;

	public Bucket()
	{
		_bucketCore = new BucketCore(7);
	}

	public Bucket(IEnumerable<T> source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		_bucketCore = new BucketCore(7);
		int num = 0;
		foreach (T item in source)
		{
			T copy = item;
			_bucketCore.DoMayIncrement(num, delegate(ref object target)
			{
				return Interlocked.Exchange(ref target, ((object)copy) ?? BucketHelper.Null) == null;
			});
			num++;
			_count++;
		}
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Extensions.CopyTo(this, array, arrayIndex);
	}

	public IEnumerable<T> EnumerateRange(int indexFrom, int indexTo)
	{
		foreach (object value in _bucketCore.EnumerateRange(indexFrom, indexTo))
		{
			yield return (value == BucketHelper.Null) ? default(T) : ((T)value);
		}
	}

	public bool Exchange(int index, T item, out T previous)
	{
		object found = BucketHelper.Null;
		previous = default(T);
		if (_bucketCore.DoMayIncrement(index, delegate(ref object target)
		{
			found = Interlocked.Exchange(ref target, ((object)item) ?? BucketHelper.Null);
			return found == null;
		}))
		{
			Interlocked.Increment(ref _count);
			return true;
		}
		if (found != BucketHelper.Null)
		{
			previous = (T)found;
		}
		return false;
	}

	public IEnumerator<T> GetEnumerator()
	{
		foreach (object value in _bucketCore)
		{
			yield return (value == BucketHelper.Null) ? default(T) : ((T)value);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Insert(int index, T item)
	{
		bool flag = _bucketCore.DoMayIncrement(index, delegate(ref object target)
		{
			object obj = Interlocked.CompareExchange(ref target, ((object)item) ?? BucketHelper.Null, null);
			return obj == null;
		});
		if (flag)
		{
			Interlocked.Increment(ref _count);
		}
		return flag;
	}

	public bool Insert(int index, T item, out T previous)
	{
		object found = BucketHelper.Null;
		previous = default(T);
		if (_bucketCore.DoMayIncrement(index, delegate(ref object target)
		{
			found = Interlocked.CompareExchange(ref target, ((object)item) ?? BucketHelper.Null, null);
			return found == null;
		}))
		{
			Interlocked.Increment(ref _count);
			return true;
		}
		if (found != BucketHelper.Null)
		{
			previous = (T)found;
		}
		return false;
	}

	public bool RemoveAt(int index)
	{
		bool flag = _bucketCore.DoMayDecrement(index, delegate(ref object target)
		{
			return Interlocked.Exchange(ref target, null) != null;
		});
		if (flag)
		{
			Interlocked.Decrement(ref _count);
		}
		return flag;
	}

	public bool RemoveAt(int index, out T previous)
	{
		object found = BucketHelper.Null;
		previous = default(T);
		if (!_bucketCore.DoMayDecrement(index, delegate(ref object target)
		{
			found = Interlocked.Exchange(ref target, null);
			return found != null;
		}))
		{
			return false;
		}
		Interlocked.Decrement(ref _count);
		if (found != BucketHelper.Null)
		{
			previous = (T)found;
		}
		return true;
	}

	public bool RemoveAt(int index, Predicate<T> check)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		return _bucketCore.DoMayDecrement(index, delegate(ref object target)
		{
			object obj = Interlocked.CompareExchange(ref target, null, null);
			if (obj != null)
			{
				T obj2 = ((obj == BucketHelper.Null) ? default(T) : ((T)obj));
				if (check(obj2))
				{
					object obj3 = Interlocked.CompareExchange(ref target, null, obj);
					if (obj == obj3)
					{
						Interlocked.Decrement(ref _count);
						return true;
					}
				}
			}
			return false;
		});
	}

	public void Set(int index, T item, out bool isNew)
	{
		isNew = _bucketCore.DoMayIncrement(index, delegate(ref object target)
		{
			return Interlocked.Exchange(ref target, ((object)item) ?? BucketHelper.Null) == null;
		});
		if (isNew)
		{
			Interlocked.Increment(ref _count);
		}
	}

	public bool TryGet(int index, out T value)
	{
		object found = BucketHelper.Null;
		value = default(T);
		if (!_bucketCore.Do(index, delegate(ref object target)
		{
			found = Interlocked.CompareExchange(ref target, null, null);
			return true;
		}) || found == null)
		{
			return false;
		}
		if (found != BucketHelper.Null)
		{
			value = (T)found;
		}
		return true;
	}

	public bool Update(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty)
	{
		if (itemUpdateFactory == null)
		{
			throw new ArgumentNullException("itemUpdateFactory");
		}
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		object found = BucketHelper.Null;
		object compare = BucketHelper.Null;
		bool result = false;
		if (!_bucketCore.Do(index, delegate(ref object target)
		{
			found = Interlocked.CompareExchange(ref target, null, null);
			if (found != null)
			{
				T val = ((found == BucketHelper.Null) ? default(T) : ((T)found));
				if (check(val))
				{
					T val2 = itemUpdateFactory(val);
					compare = Interlocked.CompareExchange(ref target, ((object)val2) ?? BucketHelper.Null, found);
					result = found == compare;
				}
			}
			return true;
		}))
		{
			isEmpty = true;
			return false;
		}
		isEmpty = found == null || compare == null;
		return result;
	}

	public IEnumerable<T> Where(Predicate<T> check)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		foreach (object value in _bucketCore)
		{
			T castedValue = ((value == BucketHelper.Null) ? default(T) : ((T)value));
			if (check(castedValue))
			{
				yield return castedValue;
			}
		}
	}
}
