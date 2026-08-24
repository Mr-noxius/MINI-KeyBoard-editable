using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public class PromiseNeedle<T> : Promise, IWaitablePromise<T>, IPromise<T>, IWaitablePromise, IRecyclableNeedle<T>, ICacheNeedle<T>, INeedle<T>, IReadOnlyNeedle<T>, IPromise
{
	private readonly int _hashCode;

	private T _target;

	public bool IsAlive => !object.ReferenceEquals(_target, null);

	public virtual T Value
	{
		get
		{
			Exception exception = base.Exception;
			if (exception == null)
			{
				return _target;
			}
			throw exception;
		}
		set
		{
			_target = value;
			SetCompleted();
		}
	}

	public PromiseNeedle(bool done)
		: base(done)
	{
		_target = default(T);
		_hashCode = base.GetHashCode();
	}

	public PromiseNeedle(Exception exception)
		: base(exception)
	{
		_target = default(T);
		_hashCode = exception.GetHashCode();
	}

	protected PromiseNeedle(T target)
		: base(done: true)
	{
		_target = target;
		_hashCode = (object.ReferenceEquals(target, null) ? base.GetHashCode() : target.GetHashCode());
	}

	public static PromiseNeedle<T> CreateFromValue(T target)
	{
		return new PromiseNeedle<T>(target);
	}

	public static bool operator !=(PromiseNeedle<T> left, PromiseNeedle<T> right)
	{
		return NotEqualsExtracted(left, right);
	}

	public static bool operator ==(PromiseNeedle<T> left, PromiseNeedle<T> right)
	{
		return EqualsExtracted(left, right);
	}

	public static explicit operator T(PromiseNeedle<T> needle)
	{
		if (needle == null)
		{
			throw new ArgumentNullException("needle");
		}
		return needle.Value;
	}

	public override bool Equals(object obj)
	{
		PromiseNeedle<T> promiseNeedle = obj as PromiseNeedle<T>;
		if (promiseNeedle != null)
		{
			return EqualsExtracted(this, promiseNeedle);
		}
		if (base.IsCompleted)
		{
			return Value.Equals(obj);
		}
		return false;
	}

	public bool Equals(PromiseNeedle<T> other)
	{
		return EqualsExtracted(this, other);
	}

	public override void Free()
	{
		base.Free();
		_target = default(T);
	}

	public override int GetHashCode()
	{
		return _hashCode;
	}

	public override string ToString()
	{
		if (!base.IsCompleted)
		{
			return "[Not Created]";
		}
		if (base.Exception != null)
		{
			return base.Exception.ToString();
		}
		return _target.ToString();
	}

	public bool TryGetValue(out T value)
	{
		bool isCompleted = base.IsCompleted;
		value = _target;
		return isCompleted;
	}

	private static bool EqualsExtracted(PromiseNeedle<T> left, PromiseNeedle<T> right)
	{
		if (object.ReferenceEquals(left, null))
		{
			return object.ReferenceEquals(right, null);
		}
		return left.Equals(right);
	}

	private static bool NotEqualsExtracted(PromiseNeedle<T> left, PromiseNeedle<T> right)
	{
		if (object.ReferenceEquals(left, null))
		{
			return !object.ReferenceEquals(right, null);
		}
		return !left.Equals(right);
	}
}
