using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading;

[DebuggerNonUserCode]
[DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
public sealed class NoTrackingThreadLocal<T> : IThreadLocal<T>, IDisposable, IWaitablePromise<T>, IPromise<T>, IWaitablePromise, ICacheNeedle<T>, INeedle<T>, IReadOnlyNeedle<T>, IPromise, IObserver<T>
{
	private int _disposing;

	private LocalDataStoreSlot _slot;

	private Func<T> _valueFactory;

	public bool IsValueCreated
	{
		get
		{
			if (Thread.VolatileRead(ref _disposing) == 1)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			return Thread.GetData(_slot) is ReadOnlyStructNeedle<T>;
		}
	}

	public T Value
	{
		get
		{
			if (Thread.VolatileRead(ref _disposing) == 1)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			object data = Thread.GetData(_slot);
			if (!(data is INeedle<T> needle))
			{
				try
				{
					Thread.SetData(_slot, ThreadLocalHelper<T>.RecursionGuardNeedle);
					T val = _valueFactory();
					Thread.SetData(_slot, new ReadOnlyStructNeedle<T>(val));
					return val;
				}
				catch (Exception ex)
				{
					if (!object.ReferenceEquals(ex, ThreadLocalHelper.RecursionGuardException))
					{
						Thread.SetData(_slot, new ExceptionStructNeedle<T>(ex));
					}
					throw;
				}
			}
			return needle.Value;
		}
		set
		{
			if (Thread.VolatileRead(ref _disposing) == 1)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			Thread.SetData(_slot, new ReadOnlyStructNeedle<T>(value));
		}
	}

	Exception IPromise.Exception => null;

	bool IReadOnlyNeedle<T>.IsAlive => IsValueCreated;

	bool IPromise.IsCanceled => false;

	bool IPromise.IsCompleted => IsValueCreated;

	bool IPromise.IsFaulted => false;

	T IThreadLocal<T>.ValueForDebugDisplay => ValueForDebugDisplay;

	IList<T> IThreadLocal<T>.Values
	{
		get
		{
			throw new InvalidOperationException();
		}
	}

	internal T ValueForDebugDisplay
	{
		get
		{
			if (!TryGetValue(out var target))
			{
				return default(T);
			}
			return target;
		}
	}

	public NoTrackingThreadLocal()
		: this(TypeHelper.GetCreateOrDefault<T>())
	{
	}

	public NoTrackingThreadLocal(Func<T> valueFactory)
	{
		if (valueFactory == null)
		{
			throw new ArgumentNullException("valueFactory");
		}
		_valueFactory = valueFactory;
		_slot = Thread.AllocateDataSlot();
	}

	[DebuggerNonUserCode]
	public void Dispose()
	{
		if (Interlocked.CompareExchange(ref _disposing, 1, 0) == 0)
		{
			_slot = null;
			_valueFactory = null;
		}
	}

	public void EraseValue()
	{
		if (Thread.VolatileRead(ref _disposing) == 1)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		Thread.SetData(_slot, null);
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "[ThreadLocal: IsValueCreated={0}, Value={1}]", new object[2] { IsValueCreated, Value });
	}

	public bool TryGetValue(out T target)
	{
		object data = Thread.GetData(_slot);
		if (!(data is INeedle<T> needle))
		{
			target = default(T);
			return false;
		}
		target = needle.Value;
		return true;
	}

	void IObserver<T>.OnCompleted()
	{
		GC.KeepAlive(Value);
	}

	void IObserver<T>.OnError(Exception error)
	{
		if (Thread.VolatileRead(ref _disposing) == 1)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		Thread.SetData(_slot, new ExceptionStructNeedle<T>(error));
	}

	void IObserver<T>.OnNext(T value)
	{
		Value = value;
	}

	void IWaitablePromise.Wait()
	{
		GC.KeepAlive(Value);
	}
}
