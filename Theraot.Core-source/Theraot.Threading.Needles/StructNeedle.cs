using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[Serializable]
[DebuggerNonUserCode]
public struct StructNeedle<T>(T target) : IEquatable<StructNeedle<T>>, IRecyclableNeedle<T>, INeedle<T>, IReadOnlyNeedle<T>
{
	private T _value = target;

	public bool IsAlive => !object.ReferenceEquals(Value, null);

	public T Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	public static explicit operator T(StructNeedle<T> needle)
	{
		return needle.Value;
	}

	public static implicit operator StructNeedle<T>(T field)
	{
		return new StructNeedle<T>(field);
	}

	public static bool operator !=(StructNeedle<T> left, StructNeedle<T> right)
	{
		return NotEqualsExtracted(left, right);
	}

	public static bool operator ==(StructNeedle<T> left, StructNeedle<T> right)
	{
		return EqualsExtracted(left, right);
	}

	public override bool Equals(object obj)
	{
		if (obj is StructNeedle<T>)
		{
			return EqualsExtracted(this, (StructNeedle<T>)obj);
		}
		if (obj is T)
		{
			T value = Value;
			if (IsAlive)
			{
				return EqualityComparer<T>.Default.Equals(value, (T)obj);
			}
			return false;
		}
		return false;
	}

	public bool Equals(StructNeedle<T> other)
	{
		return EqualsExtracted(this, other);
	}

	public override int GetHashCode()
	{
		return ((ValueType)this).GetHashCode();
	}

	void IRecyclableNeedle<T>.Free()
	{
		Value = default(T);
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

	private static bool EqualsExtracted(StructNeedle<T> left, StructNeedle<T> right)
	{
		T value = left.Value;
		if (left.IsAlive)
		{
			T value2 = right.Value;
			if (right.IsAlive)
			{
				return EqualityComparer<T>.Default.Equals(value, value2);
			}
			return false;
		}
		return !right.IsAlive;
	}

	private static bool NotEqualsExtracted(StructNeedle<T> left, StructNeedle<T> right)
	{
		T value = left.Value;
		if (left.IsAlive)
		{
			T value2 = right.Value;
			if (right.IsAlive)
			{
				return !EqualityComparer<T>.Default.Equals(value, value2);
			}
			return true;
		}
		return right.IsAlive;
	}
}
