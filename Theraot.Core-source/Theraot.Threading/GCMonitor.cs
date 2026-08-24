using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading;

[DebuggerNonUserCode]
public static class GCMonitor
{
	private static class Internal
	{
		private static readonly WeakDelegateCollection _collectedEventHandlers;

		private static readonly WaitCallback _work;

		public static WeakDelegateCollection CollectedEventHandlers => _collectedEventHandlers;

		static Internal()
		{
			_work = delegate
			{
				RaiseCollected();
			};
			_collectedEventHandlers = new WeakDelegateCollection(autoRemoveDeadItems: false, reentryGuard: false, 128);
		}

		public static void Invoke()
		{
			ThreadPool.QueueUserWorkItem(_work);
		}

		private static void RaiseCollected()
		{
			if (Volatile.Read(ref _status) == 0)
			{
				try
				{
					_collectedEventHandlers.RemoveDeadItems();
					_collectedEventHandlers.Invoke(null, new EventArgs());
				}
				catch (Exception obj)
				{
					GC.KeepAlive(obj);
				}
				Volatile.Write(ref _status, 0);
			}
		}
	}

	[DebuggerNonUserCode]
	private sealed class GCProbe : CriticalFinalizerObject
	{
		~GCProbe()
		{
			try
			{
			}
			finally
			{
				try
				{
					if (Volatile.Read(ref _status) == 0)
					{
						GC.ReRegisterForFinalize(this);
						Internal.Invoke();
					}
				}
				catch (Exception obj)
				{
					GC.KeepAlive(obj);
				}
			}
		}
	}

	private const int _maxProbingHint = 128;

	private const int _statusFinished = 1;

	private const int _statusNotReady = -2;

	private const int _statusPending = -1;

	private const int _statusReady = 0;

	private static int _status;

	public static bool FinalizingForUnload => AppDomain.CurrentDomain.IsFinalizingForUnload();

	public static event EventHandler Collected
	{
		add
		{
			try
			{
				Initialize();
				Internal.CollectedEventHandlers.Add(value);
			}
			catch
			{
				if (object.ReferenceEquals(value, null))
				{
					return;
				}
				throw;
			}
		}
		remove
		{
			if (Volatile.Read(ref _status) != 0)
			{
				return;
			}
			try
			{
				Internal.CollectedEventHandlers.Remove(value);
			}
			catch
			{
				if (object.ReferenceEquals(value, null))
				{
					return;
				}
				throw;
			}
		}
	}

	static GCMonitor()
	{
		_status = -2;
		AppDomain currentDomain = AppDomain.CurrentDomain;
		currentDomain.ProcessExit += ReportApplicationDomainExit;
		currentDomain.DomainUnload += ReportApplicationDomainExit;
	}

	private static void Initialize()
	{
		switch (Interlocked.CompareExchange(ref _status, -1, -2))
		{
		case -2:
			GC.KeepAlive(new GCProbe());
			Volatile.Write(ref _status, 0);
			break;
		case -1:
			ThreadingHelper.SpinWaitUntil(ref _status, 0);
			break;
		}
	}

	private static void ReportApplicationDomainExit(object sender, EventArgs e)
	{
		Volatile.Write(ref _status, 1);
	}
}
