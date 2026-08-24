using System;
using System.Diagnostics;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading;

[DebuggerNonUserCode]
public sealed class Disposable : IDisposable
{
	private Action _release;

	private int _status;

	public bool IsDisposed
	{
		[DebuggerNonUserCode]
		get
		{
			return _status == -1;
		}
	}

	private Disposable(Action release)
	{
		if (release == null)
		{
			throw new ArgumentNullException("release");
		}
		_release = release;
	}

	public static Disposable Create()
	{
		return new Disposable(ActionHelper.GetNoopAction());
	}

	public static Disposable Create(Action release)
	{
		return new Disposable(release);
	}

	public bool Dispose(Func<bool> condition)
	{
		if (condition == null)
		{
			throw new ArgumentNullException("condition");
		}
		return DisposedConditional(FuncHelper.GetFallacyFunc(), delegate
		{
			if (condition())
			{
				Dispose();
				return true;
			}
			return false;
		});
	}

	[DebuggerNonUserCode]
	~Disposable()
	{
		try
		{
		}
		finally
		{
			try
			{
				Dispose(disposeManagedResources: false);
			}
			catch (Exception obj)
			{
				GC.KeepAlive(obj);
			}
		}
	}

	[DebuggerNonUserCode]
	public void Dispose()
	{
		try
		{
			Dispose(disposeManagedResources: true);
		}
		finally
		{
			GC.SuppressFinalize(this);
		}
	}

	[DebuggerNonUserCode]
	public void DisposedConditional(Action whenDisposed, Action whenNotDisposed)
	{
		if (_status == -1)
		{
			if (!object.ReferenceEquals(whenDisposed, null))
			{
				whenDisposed();
			}
		}
		else
		{
			if (object.ReferenceEquals(whenNotDisposed, null))
			{
				return;
			}
			if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
			{
				try
				{
					whenNotDisposed();
					return;
				}
				finally
				{
					Interlocked.Decrement(ref _status);
				}
			}
			if (!object.ReferenceEquals(whenDisposed, null))
			{
				whenDisposed();
			}
		}
	}

	[DebuggerNonUserCode]
	public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
	{
		if (_status == -1)
		{
			if (object.ReferenceEquals(whenDisposed, null))
			{
				return default(TReturn);
			}
			return whenDisposed();
		}
		if (object.ReferenceEquals(whenNotDisposed, null))
		{
			return default(TReturn);
		}
		if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
		{
			try
			{
				return whenNotDisposed();
			}
			finally
			{
				Interlocked.Decrement(ref _status);
			}
		}
		if (object.ReferenceEquals(whenDisposed, null))
		{
			return default(TReturn);
		}
		return whenDisposed();
	}

	[DebuggerNonUserCode]
	private void Dispose(bool disposeManagedResources)
	{
		GC.KeepAlive(disposeManagedResources);
		if (TakeDisposalExecution())
		{
			try
			{
				_release();
			}
			finally
			{
				_release = null;
			}
		}
	}

	private bool TakeDisposalExecution()
	{
		if (_status != -1)
		{
			return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
		}
		return false;
	}
}
