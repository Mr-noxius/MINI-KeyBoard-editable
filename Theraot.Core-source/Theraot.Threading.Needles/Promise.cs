using System;
using System.Diagnostics;
using System.Threading;

namespace Theraot.Threading.Needles;

[Serializable]
[DebuggerNonUserCode]
public class Promise : IWaitablePromise, IPromise
{
	private readonly int _hashCode;

	private Exception _exception;

	private StructNeedle<ManualResetEventSlim> _waitHandle;

	public Exception Exception => _exception;

	bool IPromise.IsCanceled => false;

	public bool IsCompleted => _waitHandle.Value?.IsSet ?? true;

	public bool IsFaulted => _exception != null;

	protected IRecyclableNeedle<ManualResetEventSlim> WaitHandle => _waitHandle;

	public Promise(bool done)
	{
		_exception = null;
		_hashCode = base.GetHashCode();
		if (!done)
		{
			_waitHandle = new ManualResetEventSlim(initialState: false);
		}
	}

	public Promise(Exception exception)
	{
		_exception = exception;
		_hashCode = exception.GetHashCode();
		_waitHandle = new ManualResetEventSlim(initialState: true);
	}

	~Promise()
	{
		ReleaseWaitHandle(done: false);
	}

	public virtual void Free()
	{
		ManualResetEventSlim value = _waitHandle.Value;
		if (value == null)
		{
			_waitHandle.Value = new ManualResetEventSlim(initialState: false);
		}
		else
		{
			value.Reset();
		}
		_exception = null;
	}

	public virtual void Free(Action beforeFree)
	{
		if (beforeFree == null)
		{
			throw new ArgumentNullException("beforeFree");
		}
		ManualResetEventSlim value = _waitHandle.Value;
		if (value == null || value.IsSet)
		{
			try
			{
				beforeFree();
				return;
			}
			finally
			{
				if (value == null)
				{
					_waitHandle.Value = new ManualResetEventSlim(initialState: false);
				}
				else
				{
					value.Reset();
				}
				_exception = null;
			}
		}
		value.Reset();
		_exception = null;
	}

	public override int GetHashCode()
	{
		return _hashCode;
	}

	public void SetCompleted()
	{
		_exception = null;
		ReleaseWaitHandle(done: true);
	}

	public void SetError(Exception error)
	{
		_exception = error;
		ReleaseWaitHandle(done: true);
	}

	public override string ToString()
	{
		if (!IsCompleted)
		{
			return "[Not Created]";
		}
		if (_exception != null)
		{
			return _exception.ToString();
		}
		return "[Done]";
	}

	public virtual void Wait()
	{
		ManualResetEventSlim value = _waitHandle.Value;
		if (value != null)
		{
			try
			{
				value.Wait();
			}
			catch (ObjectDisposedException obj)
			{
				GC.KeepAlive(obj);
			}
		}
	}

	public virtual void Wait(CancellationToken cancellationToken)
	{
		ManualResetEventSlim value = _waitHandle.Value;
		if (value != null)
		{
			try
			{
				value.Wait(cancellationToken);
			}
			catch (ObjectDisposedException obj)
			{
				GC.KeepAlive(obj);
			}
		}
	}

	public virtual void Wait(int milliseconds)
	{
		ManualResetEventSlim value = _waitHandle.Value;
		if (value != null)
		{
			try
			{
				value.Wait(milliseconds);
			}
			catch (ObjectDisposedException obj)
			{
				GC.KeepAlive(obj);
			}
		}
	}

	public virtual void Wait(TimeSpan timeout)
	{
		ManualResetEventSlim value = _waitHandle.Value;
		if (value != null)
		{
			try
			{
				value.Wait(timeout);
			}
			catch (ObjectDisposedException obj)
			{
				GC.KeepAlive(obj);
			}
		}
	}

	public virtual void Wait(int milliseconds, CancellationToken cancellationToken)
	{
		ManualResetEventSlim value = _waitHandle.Value;
		if (value != null)
		{
			try
			{
				value.Wait(milliseconds, cancellationToken);
			}
			catch (ObjectDisposedException obj)
			{
				GC.KeepAlive(obj);
			}
		}
	}

	public virtual void Wait(TimeSpan timeout, CancellationToken cancellationToken)
	{
		ManualResetEventSlim value = _waitHandle.Value;
		if (value != null)
		{
			try
			{
				value.Wait(timeout, cancellationToken);
			}
			catch (ObjectDisposedException obj)
			{
				GC.KeepAlive(obj);
			}
		}
	}

	protected void ReleaseWaitHandle(bool done)
	{
		ManualResetEventSlim value = _waitHandle.Value;
		if (value != null)
		{
			if (done)
			{
				value.Set();
			}
			value.Dispose();
		}
		_waitHandle.Value = null;
	}
}
