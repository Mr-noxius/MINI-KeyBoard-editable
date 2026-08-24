using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections;

[DebuggerNonUserCode]
public class ProgressiveList<T> : ProgressiveCollection<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private readonly IList<T> _cache;

	public T this[int index]
	{
		get
		{
			if (index >= _cache.Count)
			{
				base.Progressor.While(() => _cache.Count < index + 1).Consume();
			}
			return _cache[index];
		}
	}

	T IList<T>.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public ProgressiveList(IEnumerable<T> wrapped)
		: this(wrapped, (IList<T>)new List<T>(), (IEqualityComparer<T>)null)
	{
	}

	public ProgressiveList(Progressor<T> wrapped)
		: this(wrapped, (IList<T>)new List<T>(), (IEqualityComparer<T>)null)
	{
	}

	public ProgressiveList(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
		: this(wrapped, (IList<T>)new List<T>(), comparer)
	{
	}

	public ProgressiveList(Progressor<T> wrapped, IEqualityComparer<T> comparer)
		: this(wrapped, (IList<T>)new List<T>(), comparer)
	{
	}

	protected ProgressiveList(IEnumerable<T> wrapped, IList<T> cache, IEqualityComparer<T> comparer)
		: base(wrapped, (ICollection<T>)cache, comparer)
	{
		if (cache == null)
		{
			throw new ArgumentNullException("cache");
		}
		_cache = cache;
	}

	protected ProgressiveList(Progressor<T> wrapped, IList<T> cache, IEqualityComparer<T> comparer)
		: base(wrapped, (ICollection<T>)cache, comparer)
	{
		if (cache == null)
		{
			throw new ArgumentNullException("cache");
		}
		_cache = cache;
	}

	public int IndexOf(T item)
	{
		int num = _cache.IndexOf(item, base.Comparer);
		if (num >= 0)
		{
			return num;
		}
		int index = _cache.Count - 1;
		bool found = false;
		base.Progressor.While(delegate(T input)
		{
			index++;
			if (base.Comparer.Equals(input, item))
			{
				found = true;
				return false;
			}
			return true;
		}).Consume();
		if (found)
		{
			return index;
		}
		return -1;
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException();
	}

	void IList<T>.Insert(int index, T item)
	{
		throw new NotSupportedException();
	}

	void IList<T>.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	protected override bool CacheContains(T item)
	{
		return _cache.Contains(item, base.Comparer);
	}
}
