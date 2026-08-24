using System;
using System.Diagnostics;
using System.Threading;

namespace Theraot.Threading;

[DebuggerNonUserCode]
public static class ThreadingHelper
{
	internal const int _sleepCountHint = 10;

	private const int _maxTime = 200;

	public static void MemoryBarrier()
	{
		Thread.MemoryBarrier();
	}

	internal static long Milliseconds(long ticks)
	{
		return ticks / 10000;
	}

	internal static long TicksNow()
	{
		return DateTime.Now.Ticks;
	}

	public static void SpinWaitSet(ref int check, int value, int comparand)
	{
		SpinWait spinWait = default(SpinWait);
		while (Interlocked.CompareExchange(ref check, value, comparand) != comparand)
		{
			spinWait.SpinOnce();
		}
	}

	public static void SpinWaitSet(ref int check, int value, int comparand, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Interlocked.CompareExchange(ref check, value, comparand) != comparand)
			{
				spinWait.SpinOnce();
				continue;
			}
			break;
		}
	}

	public static bool SpinWaitSet(ref int check, int value, int comparand, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			SpinWaitSet(ref check, value, comparand);
			return true;
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSet(ref int check, int value, int comparand, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			SpinWaitSet(ref check, value, comparand, cancellationToken);
			return true;
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSet(ref int check, int value, int comparand, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSet(ref int check, int value, int comparand, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSet(ref int check, int value, int comparand, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSet(ref int check, int value, int comparand, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static void SpinWaitUntil(ref int check, int comparand)
	{
		SpinWait spinWait = default(SpinWait);
		while (Volatile.Read(ref check) != comparand)
		{
			spinWait.SpinOnce();
		}
	}

	public static void SpinWaitUntil(ref int check, int comparand, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) == comparand)
			{
				break;
			}
			spinWait.SpinOnce();
		}
	}

	public static bool SpinWaitUntil(ref int check, int comparand, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			SpinWaitUntil(ref check, comparand);
			return true;
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			if (Volatile.Read(ref check) == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitUntil(ref int check, int comparand, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			SpinWaitUntil(ref check, comparand, cancellationToken);
			return true;
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitUntil(ref int check, int comparand, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			if (Volatile.Read(ref check) == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitUntil(ref int check, int comparand, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitUntil(ref int check, int comparand, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			if (Volatile.Read(ref check) == comparand)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitUntil(ref int check, int comparand, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) == comparand)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static void SpinWaitUntil(Func<bool> verification)
	{
		SpinWait spinWait = default(SpinWait);
		while (!verification())
		{
			spinWait.SpinOnce();
		}
	}

	public static void SpinWaitUntil(Func<bool> verification, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (verification())
			{
				break;
			}
			spinWait.SpinOnce();
		}
	}

	public static bool SpinWaitUntil(Func<bool> verification, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			SpinWaitUntil(verification);
			return true;
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			if (verification())
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitUntil(Func<bool> verification, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			SpinWaitUntil(verification, cancellationToken);
			return true;
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (verification())
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitUntil(Func<bool> verification, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			if (verification())
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitUntil(Func<bool> verification, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (verification())
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitUntil(Func<bool> verification, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			if (verification())
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitUntil(Func<bool> verification, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (verification())
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static void SpinWaitWhile(ref int check, int comparand)
	{
		SpinWait spinWait = default(SpinWait);
		while (Volatile.Read(ref check) == comparand)
		{
			spinWait.SpinOnce();
		}
	}

	public static void SpinWaitWhile(ref int check, int comparand, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) != comparand)
			{
				break;
			}
			spinWait.SpinOnce();
		}
	}

	public static bool SpinWaitWhile(ref int check, int comparand, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			SpinWaitWhile(ref check, comparand);
			return true;
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			if (Volatile.Read(ref check) != comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitWhile(ref int check, int comparand, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			SpinWaitWhile(ref check, comparand, cancellationToken);
			return true;
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) != comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitWhile(ref int check, int comparand, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			if (Volatile.Read(ref check) != comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitWhile(ref int check, int comparand, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) != comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitWhile(ref int check, int comparand, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			if (Volatile.Read(ref check) != comparand)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitWhile(ref int check, int comparand, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) != comparand)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static void SpinWaitWhileNull<T>(ref T check) where T : class
	{
		SpinWait spinWait = default(SpinWait);
		while (Volatile.Read(ref check) == null)
		{
			spinWait.SpinOnce();
		}
	}

	public static void SpinWaitWhileNull<T>(ref T check, CancellationToken cancellationToken) where T : class
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) != null)
			{
				break;
			}
			spinWait.SpinOnce();
		}
	}

	public static bool SpinWaitWhileNull<T>(ref T check, int milliseconds) where T : class
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			SpinWaitWhileNull(ref check);
			return true;
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			if (Volatile.Read(ref check) != null)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitWhileNull<T>(ref T check, int milliseconds, CancellationToken cancellationToken) where T : class
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			SpinWaitWhileNull(ref check, cancellationToken);
			return true;
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) != null)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitWhileNull<T>(ref T check, TimeSpan timeout) where T : class
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			if (Volatile.Read(ref check) != null)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitWhileNull<T>(ref T check, TimeSpan timeout, CancellationToken cancellationToken) where T : class
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) != null)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitWhileNull<T>(ref T check, IComparable<TimeSpan> timeout) where T : class
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			if (Volatile.Read(ref check) != null)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitWhileNull<T>(ref T check, IComparable<TimeSpan> timeout, CancellationToken cancellationToken) where T : class
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			if (Volatile.Read(ref check) != null)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSet(ref int check, int value)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			int num = Volatile.Read(ref check);
			int num2 = Interlocked.CompareExchange(ref check, num + value, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeSet(ref int check, int value, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			int num2 = Interlocked.CompareExchange(ref check, num + value, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeSet(ref int check, int value, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeSet(ref check, value);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			int num2 = Volatile.Read(ref check);
			int num3 = Interlocked.CompareExchange(ref check, num2 + value, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSet(ref int check, int value, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeSet(ref check, value);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num2 = Volatile.Read(ref check);
			int num3 = Interlocked.CompareExchange(ref check, num2 + value, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSet(ref int check, int value, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			int num3 = Volatile.Read(ref check);
			int num4 = Interlocked.CompareExchange(ref check, num3 + value, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSet(ref int check, int value, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num3 = Volatile.Read(ref check);
			int num4 = Interlocked.CompareExchange(ref check, num3 + value, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSet(ref int check, int value, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			int num = Volatile.Read(ref check);
			int num2 = Interlocked.CompareExchange(ref check, num + value, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSet(ref int check, int value, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			int num2 = Interlocked.CompareExchange(ref check, num + value, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchange(ref int check, int value, out int result)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			int num = Volatile.Read(ref check);
			int num2 = Interlocked.CompareExchange(ref check, num + value, num);
			result = num2 + value;
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			int num2 = Interlocked.CompareExchange(ref check, num + value, num);
			result = num2 + value;
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeExchange(ref check, value, out result);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			int num2 = Volatile.Read(ref check);
			int num3 = Interlocked.CompareExchange(ref check, num2 + value, num2);
			result = num3 + value;
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeExchange(ref check, value, out result);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num2 = Volatile.Read(ref check);
			int num3 = Interlocked.CompareExchange(ref check, num2 + value, num2);
			result = num3 + value;
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			int num3 = Volatile.Read(ref check);
			int num4 = Interlocked.CompareExchange(ref check, num3 + value, num3);
			result = num4 + value;
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num3 = Volatile.Read(ref check);
			int num4 = Interlocked.CompareExchange(ref check, num3 + value, num3);
			result = num4 + value;
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			int num = Volatile.Read(ref check);
			int num2 = Interlocked.CompareExchange(ref check, num + value, num);
			result = num2 + value;
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchange(ref int check, int value, out int result, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			int num2 = Interlocked.CompareExchange(ref check, num + value, num);
			result = num2 + value;
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			int num = Volatile.Read(ref check);
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, value, comparand);
			if (num2 == comparand)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, value, comparand);
			if (num2 == comparand)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitSetUnless(ref check, value, comparand, unless);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			int num2 = Volatile.Read(ref check);
			if (num2 == unless)
			{
				return false;
			}
			int num3 = Interlocked.CompareExchange(ref check, value, comparand);
			if (num3 == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitSetUnless(ref check, value, comparand, unless);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num2 = Volatile.Read(ref check);
			if (num2 == unless)
			{
				return false;
			}
			int num3 = Interlocked.CompareExchange(ref check, value, comparand);
			if (num3 == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			int num3 = Volatile.Read(ref check);
			if (num3 == unless)
			{
				return false;
			}
			int num4 = Interlocked.CompareExchange(ref check, value, comparand);
			if (num4 == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num3 = Volatile.Read(ref check);
			if (num3 == unless)
			{
				return false;
			}
			int num4 = Interlocked.CompareExchange(ref check, value, comparand);
			if (num4 == comparand)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			int num = Volatile.Read(ref check);
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, value, comparand);
			if (num2 == comparand)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, value, comparand);
			if (num2 == comparand)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			int num = Volatile.Read(ref check);
			int value2 = num + value;
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			int value2 = num + value;
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeSetUnless(ref check, value, unless);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			int num2 = Volatile.Read(ref check);
			int value2 = num2 + value;
			if (num2 == unless)
			{
				return false;
			}
			int num3 = Interlocked.CompareExchange(ref check, value2, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeSetUnless(ref check, value, unless);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num2 = Volatile.Read(ref check);
			int value2 = num2 + value;
			if (num2 == unless)
			{
				return false;
			}
			int num3 = Interlocked.CompareExchange(ref check, value2, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			int num3 = Volatile.Read(ref check);
			int value2 = num3 + value;
			if (num3 == unless)
			{
				return false;
			}
			int num4 = Interlocked.CompareExchange(ref check, value2, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num3 = Volatile.Read(ref check);
			int value2 = num3 + value;
			if (num3 == unless)
			{
				return false;
			}
			int num4 = Interlocked.CompareExchange(ref check, value2, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			int num = Volatile.Read(ref check);
			int value2 = num + value;
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnless(ref int check, int value, int unless, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			int value2 = num + value;
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			int num = Volatile.Read(ref check);
			result = num + value;
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, result, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			result = num + value;
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, result, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeExchangeUnless(ref check, value, unless, out result);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			int num2 = Volatile.Read(ref check);
			result = num2 + value;
			if (num2 == unless)
			{
				return false;
			}
			int num3 = Interlocked.CompareExchange(ref check, result, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeExchangeUnless(ref check, value, unless, out result);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num2 = Volatile.Read(ref check);
			result = num2 + value;
			if (num2 == unless)
			{
				return false;
			}
			int num3 = Interlocked.CompareExchange(ref check, result, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			int num3 = Volatile.Read(ref check);
			result = num3 + value;
			if (num3 == unless)
			{
				return false;
			}
			int num4 = Interlocked.CompareExchange(ref check, result, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num3 = Volatile.Read(ref check);
			result = num3 + value;
			if (num3 == unless)
			{
				return false;
			}
			int num4 = Interlocked.CompareExchange(ref check, result, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			int num = Volatile.Read(ref check);
			result = num + value;
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, result, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnless(ref int check, int value, int unless, out int result, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			result = num + value;
			if (num == unless)
			{
				return false;
			}
			int num2 = Interlocked.CompareExchange(ref check, result, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			int num = Volatile.Read(ref check);
			if (num < 0 || num < -value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			if (num < 0 || num < -value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeSetUnlessNegative(ref check, value);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			int num2 = Volatile.Read(ref check);
			if (num2 < 0 || num2 < -value)
			{
				return false;
			}
			int value2 = num2 + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeSetUnlessNegative(ref check, value);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num2 = Volatile.Read(ref check);
			if (num2 < 0 || num2 < -value)
			{
				return false;
			}
			int value2 = num2 + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			int num3 = Volatile.Read(ref check);
			if (num3 < 0 || num3 < -value)
			{
				return false;
			}
			int value2 = num3 + value;
			int num4 = Interlocked.CompareExchange(ref check, value2, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num3 = Volatile.Read(ref check);
			if (num3 < 0 || num3 < -value)
			{
				return false;
			}
			int value2 = num3 + value;
			int num4 = Interlocked.CompareExchange(ref check, value2, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			int num = Volatile.Read(ref check);
			if (num < 0 || num < -value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessNegative(ref int check, int value, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			if (num < 0 || num < -value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue < 0 || lastValue < -value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue < 0 || lastValue < -value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeExchangeUnlessNegative(ref check, value, out lastValue);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue < 0 || lastValue < -value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num2 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeExchangeUnlessNegative(ref check, value, out lastValue);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue < 0 || lastValue < -value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num2 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue < 0 || lastValue < -value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num3 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue < 0 || lastValue < -value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num3 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue < 0 || lastValue < -value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue < 0 || lastValue < -value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			int num = Volatile.Read(ref check);
			if (num > maxValue || num > maxValue - value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			if (num > maxValue || num > maxValue - value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeSetUnlessExcess(ref check, value, maxValue);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			int num2 = Volatile.Read(ref check);
			if (num2 > maxValue || num2 > maxValue - value)
			{
				return false;
			}
			int value2 = num2 + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeSetUnlessExcess(ref check, value, maxValue);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num2 = Volatile.Read(ref check);
			if (num2 > maxValue || num2 > maxValue - value)
			{
				return false;
			}
			int value2 = num2 + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			int num3 = Volatile.Read(ref check);
			if (num3 > maxValue || num3 > maxValue - value)
			{
				return false;
			}
			int value2 = num3 + value;
			int num4 = Interlocked.CompareExchange(ref check, value2, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num3 = Volatile.Read(ref check);
			if (num3 > maxValue || num3 > maxValue - value)
			{
				return false;
			}
			int value2 = num3 + value;
			int num4 = Interlocked.CompareExchange(ref check, value2, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			int num = Volatile.Read(ref check);
			if (num > maxValue || num > maxValue - value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetUnlessExcess(ref int check, int value, int maxValue, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			if (num > maxValue || num > maxValue - value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue > maxValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue > maxValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeExchangeUnlessExcess(ref check, value, maxValue, out lastValue);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue > maxValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num2 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeExchangeUnlessExcess(ref check, value, maxValue, out lastValue);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue > maxValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num2 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue > maxValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num3 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue > maxValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num3 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue > maxValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeUnlessExcess(ref int check, int value, int maxValue, out int lastValue, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue > maxValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			int num = Volatile.Read(ref check);
			if (num < minValue || num > maxValue || num + value < minValue || num > maxValue - value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			if (num < minValue || num > maxValue || num + value < minValue || num > maxValue - value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeSetBounded(ref check, value, minValue, maxValue);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			int num2 = Volatile.Read(ref check);
			if (num2 < minValue || num2 > maxValue || num2 + value < minValue || num2 > maxValue - value)
			{
				return false;
			}
			int value2 = num2 + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeSetBounded(ref check, value, minValue, maxValue);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num2 = Volatile.Read(ref check);
			if (num2 < minValue || num2 > maxValue || num2 + value < minValue || num2 > maxValue - value)
			{
				return false;
			}
			int value2 = num2 + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, num2);
			if (num3 == num2)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			int num3 = Volatile.Read(ref check);
			if (num3 < minValue || num3 > maxValue || num3 + value < minValue || num3 > maxValue - value)
			{
				return false;
			}
			int value2 = num3 + value;
			int num4 = Interlocked.CompareExchange(ref check, value2, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num3 = Volatile.Read(ref check);
			if (num3 < minValue || num3 > maxValue || num3 + value < minValue || num3 > maxValue - value)
			{
				return false;
			}
			int value2 = num3 + value;
			int num4 = Interlocked.CompareExchange(ref check, value2, num3);
			if (num4 == num3)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			int num = Volatile.Read(ref check);
			if (num < minValue || num > maxValue || num + value < minValue || num > maxValue - value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeSetBounded(ref int check, int value, int minValue, int maxValue, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			int num = Volatile.Read(ref check);
			if (num < minValue || num > maxValue || num + value < minValue || num > maxValue - value)
			{
				return false;
			}
			int value2 = num + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, num);
			if (num2 == num)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue < minValue || lastValue > maxValue || lastValue + value < minValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue < minValue || lastValue > maxValue || lastValue + value < minValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return true;
	}

	public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, int milliseconds)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeExchangeBounded(ref check, value, minValue, maxValue, out lastValue);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue < minValue || lastValue > maxValue || lastValue + value < minValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num2 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, int milliseconds, CancellationToken cancellationToken)
	{
		if (milliseconds < -1)
		{
			throw new ArgumentOutOfRangeException("milliseconds");
		}
		if (milliseconds == -1)
		{
			return SpinWaitRelativeExchangeBounded(ref check, value, minValue, maxValue, out lastValue);
		}
		SpinWait spinWait = default(SpinWait);
		long num = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue < minValue || lastValue > maxValue || lastValue + value < minValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num2 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num2 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num) >= milliseconds)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue < minValue || lastValue > maxValue || lastValue + value < minValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num3 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, TimeSpan timeout, CancellationToken cancellationToken)
	{
		long num = (long)timeout.TotalMilliseconds;
		SpinWait spinWait = default(SpinWait);
		long num2 = TicksNow();
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue < minValue || lastValue > maxValue || lastValue + value < minValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num3 = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num3 == lastValue)
			{
				return true;
			}
			if (Milliseconds(TicksNow() - num2) >= num)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, IComparable<TimeSpan> timeout)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			lastValue = Volatile.Read(ref check);
			if (lastValue < minValue || lastValue > maxValue || lastValue + value < minValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue, IComparable<TimeSpan> timeout, CancellationToken cancellationToken)
	{
		SpinWait spinWait = default(SpinWait);
		DateTime now = DateTime.Now;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GC.KeepAlive(cancellationToken.WaitHandle);
			lastValue = Volatile.Read(ref check);
			if (lastValue < minValue || lastValue > maxValue || lastValue + value < minValue || lastValue > maxValue - value)
			{
				return false;
			}
			int value2 = lastValue + value;
			int num = Interlocked.CompareExchange(ref check, value2, lastValue);
			if (num == lastValue)
			{
				return true;
			}
			if (timeout.CompareTo(DateTime.Now.Subtract(now)) <= 0)
			{
				break;
			}
			spinWait.SpinOnce();
		}
		return false;
	}
}
