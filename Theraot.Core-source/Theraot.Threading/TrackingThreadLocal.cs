using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Theraot.Threading;

[DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
public sealed class TrackingThreadLocal<T> : IThreadLocal<T>, IDisposable, IWaitablePromise<T>, IPromise<T>, IWaitablePromise, ICacheNeedle<T>, INeedle<T>, IReadOnlyNeedle<T>, IPromise, IObserver<T>
{
	private const int _maxProbingHint = 4;

	private int _disposing;

	private SafeDictionary<Thread, INeedle<T>> _slots;

	private Func<T> _valueFactory;

	public bool IsValueCreated
	{
		get
		{
			if (Volatile.Read(ref _disposing) == 1)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			if (_slots.TryGetValue(Thread.CurrentThread, out var value))
			{
				return value is ReadOnlyStructNeedle<T>;
			}
			return false;
		}
	}

	public T Value
	{
		get
		{
			return GetValue(Thread.CurrentThread);
		}
		set
		{
			SetValue(Thread.CurrentThread, value);
		}
	}

	public IList<T> Values => _slots.ConvertFiltered((KeyValuePair<Thread, INeedle<T>> input) => input.Value.Value, (KeyValuePair<Thread, INeedle<T>> input) => input.Value is ReadOnlyStructNeedle<T>);

	Exception IPromise.Exception => null;

	bool IReadOnlyNeedle<T>.IsAlive => IsValueCreated;

	bool IPromise.IsCanceled => false;

	bool IPromise.IsCompleted => IsValueCreated;

	bool IPromise.IsFaulted => false;

	T IThreadLocal<T>.ValueForDebugDisplay
	{
		get
		{
			if (!TryGetValue(Thread.CurrentThread, out var target))
			{
				return default(T);
			}
			return target;
		}
	}

	public TrackingThreadLocal(Func<T> valueFactory)
	{
		if (valueFactory == null)
		{
			throw new ArgumentNullException("valueFactory");
		}
		_valueFactory = valueFactory;
		_slots = new SafeDictionary<Thread, INeedle<T>>(4);
	}

	[DebuggerNonUserCode]
	public void Dispose()
	{
		if (Interlocked.CompareExchange(ref _disposing, 1, 0) == 0)
		{
			_slots = null;
			_valueFactory = null;
		}
	}

	public void EraseValue()
	{
		EraseValue(Thread.CurrentThread);
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "[ThreadLocal: IsValueCreated={0}, Value={1}]", new object[2] { IsValueCreated, Value });
	}

	public bool TryGetValue(Thread thread, out T target)
	{
		if (Volatile.Read(ref _disposing) == 1)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		if (_slots.TryGetValue(thread, out var value))
		{
			target = value.Value;
			return true;
		}
		target = default(T);
		return false;
	}

	public bool TryGetValue(out T value)
	{
		return TryGetValue(Thread.CurrentThread, out value);
	}

	void IObserver<T>.OnCompleted()
	{
	}

	void IObserver<T>.OnError(Exception error)
	{
		SetError(Thread.CurrentThread, error);
	}

	void IObserver<T>.OnNext(T value)
	{
		Value = value;
	}

	void IWaitablePromise.Wait()
	{
		GC.KeepAlive(Value);
	}

	private void EraseValue(Thread thread)
	{
		if (Volatile.Read(ref _disposing) == 1)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		_slots.Remove(thread);
	}

	private T GetValue(Thread thread)
	{
		if (Volatile.Read(ref _disposing) == 1)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		if (_slots.TryGetOrAdd(thread, ThreadLocalHelper<T>.RecursionGuardNeedle, out var stored))
		{
			try
			{
				stored = new ReadOnlyStructNeedle<T>(_valueFactory());
			}
			catch (Exception ex)
			{
				if (ex != ThreadLocalHelper.RecursionGuardException)
				{
					stored = new ExceptionStructNeedle<T>(ex);
				}
			}
			_slots.Set(thread, stored);
		}
		return stored.Value;
	}

	private void SetError(Thread thread, Exception error)
	{
		if (Volatile.Read(ref _disposing) == 1)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		_slots.Set(thread, new ExceptionStructNeedle<T>(error));
	}

	private void SetValue(Thread thread, T value)
	{
		if (Volatile.Read(ref _disposing) == 1)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		_slots.Set(thread, new ReadOnlyStructNeedle<T>(value));
	}
}
