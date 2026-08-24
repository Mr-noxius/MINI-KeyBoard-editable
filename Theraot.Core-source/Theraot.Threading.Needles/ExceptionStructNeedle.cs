using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[Serializable]
[DebuggerNonUserCode]
public struct ExceptionStructNeedle<T>(Exception exception) : INeedle<T>, IReadOnlyNeedle<T>, IEquatable<ExceptionStructNeedle<T>>
{
	private readonly Exception _exception = exception;

	public Exception Exception => _exception;

	T INeedle<T>.Value
	{
		get
		{
			throw _exception;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public bool IsAlive => false;

	public T Value
	{
		get
		{
			throw _exception;
		}
	}

	public static explicit operator Exception(ExceptionStructNeedle<T> needle)
	{
		return needle._exception;
	}

	public static implicit operator ExceptionStructNeedle<T>(Exception exception)
	{
		return new ExceptionStructNeedle<T>(exception);
	}

	public static bool operator !=(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
	{
		return NotEqualsExtracted(left, right);
	}

	public static bool operator ==(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
	{
		return EqualsExtracted(left, right);
	}

	public override bool Equals(object obj)
	{
		if (obj is ExceptionStructNeedle<T>)
		{
			return EqualsExtracted(this, (ExceptionStructNeedle<T>)obj);
		}
		if (obj is Exception)
		{
			return obj.Equals(_exception);
		}
		return false;
	}

	public bool Equals(ExceptionStructNeedle<T> other)
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
			return $"<Exception: {_exception}>";
		}
		return "<Dead Needle>";
	}

	private static bool EqualsExtracted(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
	{
		Exception exception = left._exception;
		Exception exception2 = right._exception;
		return exception?.Equals(exception2) ?? (exception2 == null);
	}

	private static bool NotEqualsExtracted(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
	{
		Exception exception = left._exception;
		Exception exception2 = right._exception;
		if (exception == null)
		{
			return exception2 != null;
		}
		return !exception.Equals(exception2);
	}
}
