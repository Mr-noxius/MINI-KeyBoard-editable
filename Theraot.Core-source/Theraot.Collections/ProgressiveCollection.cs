using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections;

[Serializable]
[DebuggerNonUserCode]
public class ProgressiveCollection<T> : IReadOnlyCollection<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private readonly ICollection<T> _cache;

	private readonly IEqualityComparer<T> _comparer;

	private readonly Progressor<T> _progressor;

	public int Count
	{
		get
		{
			_progressor.AsEnumerable().Consume();
			return _cache.Count;
		}
	}

	public bool EndOfEnumeration => _progressor.IsClosed;

	bool ICollection<T>.IsReadOnly => true;

	protected IEqualityComparer<T> Comparer => _comparer;

	protected Progressor<T> Progressor => _progressor;

	public ProgressiveCollection(IEnumerable<T> wrapped)
		: this(wrapped, (ICollection<T>)new HashSet<T>(), (IEqualityComparer<T>)null)
	{
	}

	public ProgressiveCollection(Progressor<T> wrapped)
		: this(wrapped, (ICollection<T>)new HashSet<T>(), (IEqualityComparer<T>)null)
	{
	}

	public ProgressiveCollection(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
		: this(wrapped, (ICollection<T>)new HashSet<T>(comparer), comparer)
	{
	}

	public ProgressiveCollection(Progressor<T> wrapped, IEqualityComparer<T> comparer)
		: this(wrapped, (ICollection<T>)new HashSet<T>(comparer), comparer)
	{
	}

	protected ProgressiveCollection(IEnumerable<T> wrapped, ICollection<T> cache, IEqualityComparer<T> comparer)
	{
		if (cache == null)
		{
			throw new ArgumentNullException("cache");
		}
		_cache = cache;
		_progressor = new Progressor<T>(wrapped);
		_progressor.SubscribeAction(delegate(T obj)
		{
			_cache.Add(obj);
		});
		_comparer = comparer ?? EqualityComparer<T>.Default;
	}

	protected ProgressiveCollection(Progressor<T> wrapped, ICollection<T> cache, IEqualityComparer<T> comparer)
	{
		if (cache == null)
		{
			throw new ArgumentNullException("cache");
		}
		if (wrapped == null)
		{
			throw new ArgumentNullException("cache");
		}
		_cache = cache;
		_progressor = new Progressor<T>(wrapped);
		_progressor.SubscribeAction(delegate(T obj)
		{
			_cache.Add(obj);
		});
		_comparer = comparer ?? EqualityComparer<T>.Default;
	}

	protected ProgressiveCollection(TryTake<T> tryTake, ICollection<T> cache, IEqualityComparer<T> comparer)
	{
		if (cache == null)
		{
			throw new ArgumentNullException("cache");
		}
		_cache = cache;
		_progressor = new Progressor<T>(tryTake, doneOnFalse: false);
		_progressor.SubscribeAction(delegate(T obj)
		{
			_cache.Add(obj);
		});
		_comparer = comparer ?? EqualityComparer<T>.Default;
	}

	public void Close()
	{
		_progressor.Close();
	}

	public bool Contains(T item)
	{
		if (CacheContains(item))
		{
			return true;
		}
		T item2;
		while (_progressor.TryTake(out item2))
		{
			if (_comparer.Equals(item, item2))
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(T item, IEqualityComparer<T> comparer)
	{
		return Enumerable.Contains(this, item, comparer);
	}

	public void CopyTo(T[] array)
	{
		_progressor.AsEnumerable().Consume();
		_cache.CopyTo(array, 0);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		_progressor.AsEnumerable().Consume();
		_cache.CopyTo(array, arrayIndex);
	}

	public void CopyTo(T[] array, int arrayIndex, int countLimit)
	{
		Extensions.CanCopyTo(array, arrayIndex, countLimit);
		_progressor.While(() => _cache.Count < countLimit).Consume();
		_cache.CopyTo(array, arrayIndex, countLimit);
	}

	public IEnumerator<T> GetEnumerator()
	{
		foreach (T item2 in _cache)
		{
			yield return item2;
		}
		T item;
		while (_progressor.TryTake(out item))
		{
			yield return item;
		}
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}

	protected virtual bool CacheContains(T item)
	{
		return _cache.Contains(item, _comparer);
	}
}
