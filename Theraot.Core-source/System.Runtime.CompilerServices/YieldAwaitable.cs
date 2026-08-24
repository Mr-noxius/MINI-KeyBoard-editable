using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct YieldAwaitable
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public struct YieldAwaiter : ICriticalNotifyCompletion, INotifyCompletion
	{
		private static readonly WaitCallback _waitCallbackRunAction = RunAction;

		public bool IsCompleted => false;

		public void GetResult()
		{
		}

		[SecuritySafeCritical]
		public void OnCompleted(Action continuation)
		{
			if (continuation == null)
			{
				throw new ArgumentNullException("continuation");
			}
			if (TaskScheduler.Current == TaskScheduler.Default)
			{
				ThreadPool.QueueUserWorkItem(_waitCallbackRunAction, continuation);
			}
			else
			{
				Task.Factory.StartNew(continuation, default(CancellationToken), TaskCreationOptions.PreferFairness, TaskScheduler.Current);
			}
		}

		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation)
		{
			if (continuation == null)
			{
				throw new ArgumentNullException("continuation");
			}
			if (TaskScheduler.Current == TaskScheduler.Default)
			{
				ThreadPool.UnsafeQueueUserWorkItem(_waitCallbackRunAction, continuation);
			}
			else
			{
				Task.Factory.StartNew(continuation, default(CancellationToken), TaskCreationOptions.PreferFairness, TaskScheduler.Current);
			}
		}

		private static void RunAction(object state)
		{
			((Action)state)();
		}
	}

	public YieldAwaiter GetAwaiter()
	{
		return default(YieldAwaiter);
	}
}
