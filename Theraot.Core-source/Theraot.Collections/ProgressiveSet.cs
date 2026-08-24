using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Core;

namespace Theraot.Collections;

[Serializable]
[DebuggerNonUserCode]
public class ProgressiveSet<T> : ProgressiveCollection<T>, ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	bool ICollection<T>.IsReadOnly => true;

	public ProgressiveSet(IEnumerable<T> wrapped)
		: this(wrapped, (ISet<T>)new ExtendedSet<T>(), (IEqualityComparer<T>)null)
	{
	}

	public ProgressiveSet(Progressor<T> wrapped)
		: this(wrapped, (ISet<T>)new ExtendedSet<T>(), (IEqualityComparer<T>)null)
	{
	}

	public ProgressiveSet(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
		: this(wrapped, (ISet<T>)new ExtendedSet<T>(comparer), (IEqualityComparer<T>)null)
	{
	}

	public ProgressiveSet(Progressor<T> wrapped, IEqualityComparer<T> comparer)
		: this(wrapped, (ISet<T>)new ExtendedSet<T>(comparer), (IEqualityComparer<T>)null)
	{
	}

	protected ProgressiveSet(IEnumerable<T> wrapped, ISet<T> cache, IEqualityComparer<T> comparer)
		: this(Check.NotNullArgument(wrapped, "wrapped").GetEnumerator(), cache, comparer)
	{
	}

	protected ProgressiveSet(Progressor<T> wrapped, ISet<T> cache, IEqualityComparer<T> comparer)
		: base((TryTake<T>)delegate(out T value)
		{
			while (wrapped.TryTake(out value))
			{
				if (!cache.Contains(value))
				{
					return true;
				}
			}
			return false;
		}, (ICollection<T>)cache, comparer)
	{
	}

	private ProgressiveSet(IEnumerator<T> enumerator, ISet<T> cache, IEqualityComparer<T> comparer)
		: base((TryTake<T>)delegate(out T value)
		{
			while (enumerator.MoveNext())
			{
				value = enumerator.Current;
				if (!cache.Contains(value))
				{
					return true;
				}
			}
			enumerator.Dispose();
			value = default(T);
			return false;
		}, (ICollection<T>)cache, comparer)
	{
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

	public bool SetEquals(IEnumerable<T> other)
	{
		return Extensions.SetEquals(this, other);
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	bool ISet<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException();
	}

	void ISet<T>.ExceptWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ISet<T>.IntersectWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}

	void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	void ISet<T>.UnionWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}
}
