using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[Serializable]
[DebuggerNonUserCode]
public class Needle<T> : IEquatable<Needle<T>>, IRecyclableNeedle<T>, INeedle<T>, IPromise<T>, IPromise, IReadOnlyNeedle<T>
{
	private readonly int _hashCode;

	private INeedle<T> _target;

	public Exception Exception
	{
		get
		{
			if (_target is ExceptionStructNeedle<T> exceptionStructNeedle)
			{
				return exceptionStructNeedle.Exception;
			}
			return null;
		}
	}

	bool IPromise.IsCanceled => false;

	bool IPromise.IsCompleted => IsAlive;

	public bool IsAlive => _target?.IsAlive ?? false;

	public bool IsFaulted => _target is ExceptionStructNeedle<T>;

	public virtual T Value
	{
		get
		{
			return _target.Value;
		}
		set
		{
			SetTargetValue(value);
		}
	}

	public Needle()
	{
		_target = null;
		_hashCode = base.GetHashCode();
	}

	public Needle(T target)
	{
		if (object.ReferenceEquals(target, null))
		{
			_target = null;
			_hashCode = base.GetHashCode();
		}
		else
		{
			_target = new StructNeedle<T>(target);
			_hashCode = target.GetHashCode();
		}
	}

	public static explicit operator T(Needle<T> needle)
	{
		if (needle == null)
		{
			throw new ArgumentNullException("needle");
		}
		return needle.Value;
	}

	public static implicit operator Needle<T>(T field)
	{
		return new Needle<T>(field);
	}

	public static bool operator !=(Needle<T> left, Needle<T> right)
	{
		return NotEqualsExtracted(left, right);
	}

	public static bool operator ==(Needle<T> left, Needle<T> right)
	{
		return EqualsExtracted(left, right);
	}

	public override bool Equals(object obj)
	{
		Needle<T> needle = obj as Needle<T>;
		if (needle != null)
		{
			return EqualsExtracted(this, needle);
		}
		INeedle<T> target = _target;
		if (_target == null)
		{
			return obj == null;
		}
		if (obj == null)
		{
			return false;
		}
		return target.Equals(obj);
	}

	public bool Equals(Needle<T> other)
	{
		INeedle<T> target = _target;
		if (target == null)
		{
			return other._target == null;
		}
		return EqualsExtracted(this, other);
	}

	public virtual void Free()
	{
		_target = null;
	}

	public override int GetHashCode()
	{
		return _hashCode;
	}

	public override string ToString()
	{
		T value = Value;
		if (IsAlive)
		{
			return value.ToString();
		}
		return "<Dead Needle>";
	}

	protected void SetTargetError(Exception error)
	{
		_target = new ExceptionStructNeedle<T>(error);
	}

	protected void SetTargetValue(T value)
	{
		if (_target is StructNeedle<T>)
		{
			try
			{
				_target.Value = value;
				return;
			}
			catch (NotSupportedException)
			{
			}
		}
		_target = new StructNeedle<T>(value);
	}

	private static bool EqualsExtracted(Needle<T> left, Needle<T> right)
	{
		if (object.ReferenceEquals(left, null))
		{
			return object.ReferenceEquals(right, null);
		}
		if (object.ReferenceEquals(right, null))
		{
			return false;
		}
		INeedle<T> target = left._target;
		INeedle<T> target2 = right._target;
		if (target == null)
		{
			return target2 == null;
		}
		if (target2 == null)
		{
			return false;
		}
		return target.Equals(target2);
	}

	private static bool NotEqualsExtracted(Needle<T> left, Needle<T> right)
	{
		if (object.ReferenceEquals(left, null))
		{
			return !object.ReferenceEquals(right, null);
		}
		if (object.ReferenceEquals(right, null))
		{
			return true;
		}
		INeedle<T> target = left._target;
		INeedle<T> target2 = right._target;
		if (target == null)
		{
			return target2 != null;
		}
		if (target2 == null)
		{
			return true;
		}
		return !target.Equals(target2);
	}
}
