using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading;

public class Timeout : IPromise
{
	private const int _canceled = 4;

	private const int _canceling = 3;

	private const int _changing = 6;

	private const int _created = 0;

	private const int _executed = 2;

	private const int _executing = 1;

	protected Action Callback;

	private static readonly Bucket<Timeout> _root = new Bucket<Timeout>();

	private static int _lastRootIndex = -1;

	private readonly int _hashcode;

	private int _rootIndex = -1;

	private long _startTime;

	private int _status;

	private long _targetTime;

	private Timer _wrapped;

	Exception IPromise.Exception => null;

	public bool IsCanceled => Volatile.Read(ref _status) == 4;

	public bool IsCompleted => Volatile.Read(ref _status) == 2;

	bool IPromise.IsFaulted => false;

	public Timeout(Action callback, long dueTime)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		Callback = callback;
		Start(dueTime);
		_hashcode = (int)DateTime.Now.Ticks;
	}

	public Timeout(Action callback, long dueTime, CancellationToken token)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		if (token.IsCancellationRequested)
		{
			Callback = null;
			_wrapped = null;
			_status = 4;
		}
		else
		{
			Callback = callback;
			Start(dueTime);
			token.Register(Cancel);
		}
		_hashcode = (int)DateTime.Now.Ticks;
	}

	public Timeout(Action callback, TimeSpan dueTime)
		: this(callback, (long)dueTime.TotalMilliseconds)
	{
	}

	public Timeout(Action callback, TimeSpan dueTime, CancellationToken token)
		: this(callback, (long)dueTime.TotalMilliseconds, token)
	{
	}

	protected Timeout()
	{
		_hashcode = (int)DateTime.Now.Ticks;
	}

	~Timeout()
	{
		Close();
	}

	public static void Launch(Action callback, long dueTime)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		Timeout timeout = new Timeout();
		timeout.Callback = delegate
		{
			try
			{
				callback();
			}
			finally
			{
				UnRoot(timeout);
			}
		};
		timeout.Start(dueTime);
		Root(timeout);
	}

	public static void Launch(Action callback, long dueTime, CancellationToken token)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		if (token.IsCancellationRequested)
		{
			return;
		}
		Timeout timeout = new Timeout();
		timeout.Callback = delegate
		{
			try
			{
				callback();
			}
			finally
			{
				UnRoot(timeout);
			}
		};
		timeout.Start(dueTime);
		token.Register(timeout.Cancel);
		Root(timeout);
	}

	public static void Launch(Action callback, TimeSpan dueTime)
	{
		Launch(callback, (long)dueTime.TotalMilliseconds);
	}

	public static void Launch(Action callback, TimeSpan dueTime, CancellationToken token)
	{
		Launch(callback, (long)dueTime.TotalMilliseconds, token);
	}

	public void Cancel()
	{
		if (Interlocked.CompareExchange(ref _status, 3, 0) == 0)
		{
			Close();
			Volatile.Write(ref _status, 4);
		}
	}

	public bool Change(long dueTime)
	{
		if (Interlocked.CompareExchange(ref _status, 6, 0) == 0)
		{
			_startTime = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
			_targetTime = _startTime + dueTime;
			Timer timer = Interlocked.CompareExchange(ref _wrapped, null, null);
			if (timer == null)
			{
				return false;
			}
			timer.Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1.0));
			Volatile.Write(ref _status, 0);
			return true;
		}
		return false;
	}

	public void Change(TimeSpan dueTime)
	{
		Change((long)dueTime.TotalMilliseconds);
	}

	public long CheckRemaining()
	{
		long num = _targetTime - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
		if (num <= 0)
		{
			Finish(null);
			return 0L;
		}
		return num;
	}

	public override bool Equals(object obj)
	{
		if (obj is Timeout)
		{
			return this == obj;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _hashcode;
	}

	protected static void Root(Timeout timeout)
	{
		timeout._rootIndex = Interlocked.Increment(ref _lastRootIndex);
		_root.Set(timeout._rootIndex, timeout);
	}

	protected static void UnRoot(Timeout timeout)
	{
		int num = Interlocked.Exchange(ref timeout._rootIndex, -1);
		if (num != -1)
		{
			_root.RemoveAt(num);
		}
	}

	protected void Start(long dueTime)
	{
		_startTime = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
		_targetTime = _startTime + dueTime;
		_wrapped = new Timer(Finish, null, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1.0));
	}

	private void Close()
	{
		Interlocked.Exchange(ref _wrapped, null)?.Dispose();
		Volatile.Write(ref Callback, null);
		GC.SuppressFinalize(this);
	}

	private void Finish(object state)
	{
		GC.KeepAlive(state);
		ThreadingHelper.SpinWaitWhile(ref _status, 6);
		if (Interlocked.CompareExchange(ref _status, 1, 0) == 0)
		{
			Action action = Volatile.Read(ref Callback);
			if (action != null)
			{
				action();
				Close();
				Volatile.Write(ref _status, 2);
			}
		}
	}
}
public class Timeout<T> : Timeout
{
	public Timeout(Action<T> callback, long dueTime, T target)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		Callback = new ValueActionClosure<T>(callback, target).Invoke;
		Start(dueTime);
	}

	public Timeout(Action<T> callback, long dueTime, CancellationToken token, T target)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		if (token.IsCancellationRequested)
		{
			Callback = null;
			Cancel();
		}
		else
		{
			Callback = new ValueActionClosure<T>(callback, target).Invoke;
			Start(dueTime);
			token.Register(base.Cancel);
		}
	}

	public Timeout(Action<T> callback, TimeSpan dueTime, T target)
		: this(callback, (long)dueTime.TotalMilliseconds, target)
	{
	}

	public Timeout(Action<T> callback, TimeSpan dueTime, CancellationToken token, T target)
		: this(callback, (long)dueTime.TotalMilliseconds, token, target)
	{
	}

	private Timeout()
	{
	}

	public static void Launch(Action<T> callback, long dueTime, T target)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		Timeout<T> timeout = new Timeout<T>();
		timeout.Callback = delegate
		{
			try
			{
				callback(target);
			}
			finally
			{
				Timeout.UnRoot(timeout);
			}
		};
		timeout.Start(dueTime);
		Timeout.Root(timeout);
	}

	public static void Launch(Action<T> callback, long dueTime, CancellationToken token, T target)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		if (token.IsCancellationRequested)
		{
			return;
		}
		Timeout<T> timeout = new Timeout<T>();
		timeout.Callback = delegate
		{
			try
			{
				callback(target);
			}
			finally
			{
				Timeout.UnRoot(timeout);
			}
		};
		timeout.Start(dueTime);
		token.Register(timeout.Cancel);
		Timeout.Root(timeout);
	}

	public static void Launch(Action<T> callback, TimeSpan dueTime, T target)
	{
		Launch(callback, (long)dueTime.TotalMilliseconds, target);
	}

	public static void Launch(Action<T> callback, TimeSpan dueTime, CancellationToken token, T target)
	{
		Launch(callback, (long)dueTime.TotalMilliseconds, token, target);
	}
}
