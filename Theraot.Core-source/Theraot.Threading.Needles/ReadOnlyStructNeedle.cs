using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[Serializable]
[DebuggerNonUserCode]
public struct ReadOnlyStructNeedle<T>(T target) : INeedle<T>, IReadOnlyNeedle<T>, IEquatable<ReadOnlyStructNeedle<T>>
{
	private readonly T _value = target;

	T INeedle<T>.Value
	{
		get
		{
			return _value;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public bool IsAlive => !object.ReferenceEquals(_value, null);

	public T Value => _value;

	public static explicit operator T(ReadOnlyStructNeedle<T> needle)
	{
		return needle._value;
	}

	public static implicit operator ReadOnlyStructNeedle<T>(T field)
	{
		return new ReadOnlyStructNeedle<T>(field);
	}

	public static bool operator !=(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
	{
		return NotEqualsExtracted(left, right);
	}

	public static bool operator ==(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
	{
		return EqualsExtracted(left, right);
	}

	public override bool Equals(object obj)
	{
		if (obj is ReadOnlyStructNeedle<T>)
		{
			return EqualsExtracted(this, (ReadOnlyStructNeedle<T>)obj);
		}
		if (obj is T)
		{
			T value = _value;
			if (IsAlive)
			{
				return EqualityComparer<T>.Default.Equals(value, (T)obj);
			}
			return false;
		}
		return false;
	}

	public bool Equals(ReadOnlyStructNeedle<T> other)
	{
		return EqualsExtracted(this, other);
	}

	public override int GetHashCode()
	{
		return ((ValueType)this).GetHashCode();
	}

	public override string ToString()
	{
		if (IsAlive)
		{
			return _value.ToString();
		}
		return "<Dead Needle>";
	}

	private static bool EqualsExtracted(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
	{
		T value = left._value;
		if (left.IsAlive)
		{
			T value2 = right._value;
			if (right.IsAlive)
			{
				return EqualityComparer<T>.Default.Equals(value, value2);
			}
			return false;
		}
		return !right.IsAlive;
	}

	private static bool NotEqualsExtracted(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
	{
		T value = left._value;
		if (left.IsAlive)
		{
			T value2 = right._value;
			if (right.IsAlive)
			{
				return !EqualityComparer<T>.Default.Equals(value, value2);
			}
			return true;
		}
		return right.IsAlive;
	}
}
