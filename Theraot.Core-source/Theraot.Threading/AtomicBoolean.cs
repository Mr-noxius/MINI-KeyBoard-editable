using System.Runtime.CompilerServices;
using System.Threading;

namespace Theraot.Threading;

public class AtomicBoolean
{
	private const int _set = 1;

	private const int _unset = 0;

	private int _value;

	public bool Value
	{
		get
		{
			return _value == 1;
		}
		set
		{
			Exchange(value);
		}
	}

	public static implicit operator AtomicBoolean(bool value)
	{
		AtomicBoolean atomicBoolean = new AtomicBoolean();
		atomicBoolean.Value = value;
		return atomicBoolean;
	}

	public static implicit operator bool(AtomicBoolean atomicBoolean)
	{
		return atomicBoolean.Value;
	}

	public static bool operator !=(AtomicBoolean left, AtomicBoolean right)
	{
		if (!(left == null))
		{
			return !left.Equals(right);
		}
		return right != null;
	}

	public static bool operator ==(AtomicBoolean left, AtomicBoolean right)
	{
		if (!(left == null))
		{
			return left.Equals(right);
		}
		return right == null;
	}

	public bool CompareExchange(bool expected, bool newVal)
	{
		int value = (newVal ? 1 : 0);
		int num = (expected ? 1 : 0);
		return Interlocked.CompareExchange(ref _value, value, num) == num;
	}

	public bool Equals(AtomicBoolean obj)
	{
		return _value == obj._value;
	}

	public override bool Equals(object obj)
	{
		if (obj is AtomicBoolean)
		{
			return Equals((AtomicBoolean)obj);
		}
		return false;
	}

	public bool Exchange(bool newVal)
	{
		int value = (newVal ? 1 : 0);
		return Interlocked.Exchange(ref _value, value) == 1;
	}

	public override int GetHashCode()
	{
		return RuntimeHelpers.GetHashCode(this);
	}

	public override string ToString()
	{
		return Value.ToString();
	}

	internal bool TryRelaxedSet()
	{
		if (_value == 0)
		{
			return !Exchange(newVal: true);
		}
		return false;
	}

	public bool TrySet()
	{
		return !Exchange(newVal: true);
	}
}
