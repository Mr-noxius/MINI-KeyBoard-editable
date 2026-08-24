using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[Serializable]
[DebuggerNonUserCode]
public class ReadOnlyPromiseNeedle<T> : ReadOnlyPromise, IWaitablePromise<T>, IPromise<T>, IWaitablePromise, ICacheNeedle<T>, INeedle<T>, IReadOnlyNeedle<T>, IPromise, IEquatable<ReadOnlyPromiseNeedle<T>>
{
	private readonly ICacheNeedle<T> _promised;

	T INeedle<T>.Value
	{
		get
		{
			return _promised.Value;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public bool IsAlive => _promised.IsAlive;

	public T Value => _promised.Value;

	public ReadOnlyPromiseNeedle(ICacheNeedle<T> promised, bool allowWait)
		: base(promised, allowWait)
	{
		_promised = promised;
	}

	public static bool operator !=(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
	{
		return NotEqualsExtracted(left, right);
	}

	public static bool operator ==(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
	{
		return EqualsExtracted(left, right);
	}

	public static explicit operator T(ReadOnlyPromiseNeedle<T> needle)
	{
		if (needle == null)
		{
			throw new ArgumentNullException("needle");
		}
		return needle.Value;
	}

	public override bool Equals(object obj)
	{
		ReadOnlyPromiseNeedle<T> right = obj as ReadOnlyPromiseNeedle<T>;
		if (obj != null)
		{
			return EqualsExtracted(this, right);
		}
		if (_promised.IsCompleted)
		{
			return _promised.Value.Equals(obj);
		}
		return false;
	}

	public bool Equals(ReadOnlyPromiseNeedle<T> other)
	{
		return EqualsExtracted(this, other);
	}

	public override int GetHashCode()
	{
		return _promised.GetHashCode();
	}

	public override string ToString()
	{
		return $"{{Promise: {_promised}}}";
	}

	public bool TryGetValue(out T value)
	{
		return _promised.TryGetValue(out value);
	}

	private static bool EqualsExtracted(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
	{
		if (left == null)
		{
			return right == null;
		}
		return left.Equals(right);
	}

	private static bool NotEqualsExtracted(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
	{
		if (left == null)
		{
			return right != null;
		}
		return !left.Equals(right);
	}
}
