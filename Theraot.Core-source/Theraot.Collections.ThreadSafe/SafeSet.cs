using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public class SafeSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private const int _defaultProbing = 1;

	private readonly IEqualityComparer<T> _comparer;

	private Bucket<T> _bucket;

	private int _probing;

	public IEqualityComparer<T> Comparer => _comparer;

	public int Count => _bucket.Count;

	bool ICollection<T>.IsReadOnly => false;

	public SafeSet()
		: this((IEqualityComparer<T>)EqualityComparer<T>.Default, 1)
	{
	}

	public SafeSet(int initialProbing)
		: this((IEqualityComparer<T>)EqualityComparer<T>.Default, initialProbing)
	{
	}

	public SafeSet(IEqualityComparer<T> comparer)
		: this(comparer, 1)
	{
	}

	public SafeSet(IEqualityComparer<T> comparer, int initialProbing)
	{
		_comparer = comparer ?? EqualityComparer<T>.Default;
		_bucket = new Bucket<T>();
		_probing = initialProbing;
	}

	public bool Add(T item)
	{
		int hashCode = _comparer.GetHashCode(item);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.Insert(hashCode + num, item, out var previous))
			{
				return true;
			}
			if (_comparer.Equals(previous, item))
			{
				break;
			}
			num++;
		}
		return false;
	}

	public void AddNew(T value)
	{
		int hashCode = _comparer.GetHashCode(value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			if (_bucket.Insert(hashCode + num, value, out var previous))
			{
				return;
			}
			if (_comparer.Equals(previous, value))
			{
				break;
			}
			num++;
		}
		throw new ArgumentException("the value is already present");
	}

	public void Clear()
	{
		_bucket = new Bucket<T>();
	}

	public IEnumerable<T> ClearEnumerable()
	{
		return Interlocked.Exchange(ref _bucket, _bucket = new Bucket<T>());
	}

	public bool Contains(T value)
	{
		int hashCode = _comparer.GetHashCode(value);
		for (int i = 0; i < _probing; i++)
		{
			if (_bucket.TryGet(hashCode + i, out var value2) && _comparer.Equals(value2, value))
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(int hashCode, Predicate<T> check)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		for (int i = 0; i < _probing; i++)
		{
			if (_bucket.TryGet(hashCode + i, out var value) && _comparer.GetHashCode(value) == hashCode && check(value))
			{
				return true;
			}
		}
		return false;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		_bucket.CopyTo(array, arrayIndex);
	}

	public void ExceptWith(IEnumerable<T> other)
	{
		Extensions.ExceptWith(this, other);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _bucket.GetEnumerator();
	}

	public IList<T> GetValues()
	{
		List<T> list = new List<T>(_bucket.Count);
		foreach (T item in _bucket)
		{
			list.Add(item);
		}
		return list;
	}

	public void IntersectWith(IEnumerable<T> other)
	{
		Extensions.IntersectWith(this, other);
	}

	public bool IsProperSubsetOf(IEnumerable<T> other)
	{
		return Extensions.IsProperSubsetOf(this, other);
	}

	public bool IsProperSupersetOf(IEnumerable<T> other)
	{
		return Extensions.IsProperSupersetOf(this, other);
	}

	public bool IsSubsetOf(IEnumerable<T> other)
	{
		return Extensions.IsSubsetOf(this, other);
	}

	public bool IsSupersetOf(IEnumerable<T> other)
	{
		return Extensions.IsSupersetOf(this, other);
	}

	public bool Overlaps(IEnumerable<T> other)
	{
		return Extensions.Overlaps(this, other);
	}

	public bool Remove(T value)
	{
		int hashCode = _comparer.GetHashCode(value);
		for (int i = 0; i < _probing; i++)
		{
			bool done = false;
			bool result = _bucket.RemoveAt(hashCode + i, delegate(T found)
			{
				if (_comparer.Equals(found, value))
				{
					done = true;
					return true;
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

	public bool Remove(T value, out T previous)
	{
		int hashCode = _comparer.GetHashCode(value);
		for (int i = 0; i < _probing; i++)
		{
			bool done = false;
			T tmp = default(T);
			bool result = _bucket.RemoveAt(hashCode + i, delegate(T found)
			{
				tmp = found;
				if (_comparer.Equals(found, value))
				{
					done = true;
					return true;
				}
				return false;
			});
			if (done)
			{
				previous = tmp;
				return result;
			}
		}
		previous = default(T);
		return false;
	}

	public bool Remove(int hashCode, Predicate<T> check, out T value)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		value = default(T);
		for (int i = 0; i < _probing; i++)
		{
			bool done = false;
			T previous = default(T);
			bool result = _bucket.RemoveAt(hashCode + i, delegate(T found)
			{
				previous = found;
				if (_comparer.GetHashCode(found) == hashCode && check(found))
				{
					done = true;
					return true;
				}
				return false;
			});
			if (done)
			{
				value = previous;
				return result;
			}
		}
		return false;
	}

	public int RemoveWhere(Predicate<T> check)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		IEnumerable<T> enumerable = _bucket.Where(check);
		int num = 0;
		foreach (T item in enumerable)
		{
			if (Remove(item))
			{
				num++;
			}
		}
		return num;
	}

	public IEnumerable<T> RemoveWhereEnumerable(Predicate<T> check)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		IEnumerable<T> matches = _bucket.Where(check);
		foreach (T value in matches)
		{
			if (Remove(value))
			{
				yield return value;
			}
		}
	}

	public bool SetEquals(IEnumerable<T> other)
	{
		return Extensions.SetEquals(this, other);
	}

	public void SymmetricExceptWith(IEnumerable<T> other)
	{
		Extensions.SymmetricExceptWith(this, other);
	}

	public bool TryGetValue(int hashCode, Predicate<T> check, out T value)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		value = default(T);
		for (int i = 0; i < _probing; i++)
		{
			if (_bucket.TryGet(hashCode + i, out var value2) && _comparer.GetHashCode(value2) == hashCode && check(value2))
			{
				value = value2;
				return true;
			}
		}
		return false;
	}

	public void UnionWith(IEnumerable<T> other)
	{
		Extensions.UnionWith(this, other);
	}

	public IEnumerable<T> Where(Predicate<T> check)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		return _bucket.Where(check);
	}

	void ICollection<T>.Add(T item)
	{
		AddNew(item);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	internal bool TryAdd(T value, Predicate<T> valueOverwriteCheck)
	{
		if (valueOverwriteCheck == null)
		{
			throw new ArgumentNullException("valueOverwriteCheck");
		}
		int hashCode = _comparer.GetHashCode(value);
		int num = 0;
		while (true)
		{
			ExtendProbingIfNeeded(num);
			Predicate<T> check = delegate(T found)
			{
				if (_comparer.Equals(found, value))
				{
					throw new ArgumentException("The item has already been added");
				}
				return valueOverwriteCheck(found);
			};
			try
			{
				if (_bucket.InsertOrUpdate(hashCode + num, value, check, out var _))
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

	private void ExtendProbingIfNeeded(int attempts)
	{
		int num = attempts - _probing;
		if (num > 0)
		{
			Interlocked.Add(ref _probing, num);
		}
	}
}
