using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

public struct TaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
{
	internal const bool continueOnCapturedContextDefaultValue = true;

	private const string _invalidOperationExceptionTaskNotCompleted = "The task has not yet completed.";

	private static readonly MethodInfo _prepForRemoting = GetPrepForRemotingMethodInfo();

	private static readonly object[] _emptyParams = new object[0];

	private readonly Task _task;

	public bool IsCompleted => _task.IsCompleted;

	private static bool IsValidLocationForInlining
	{
		get
		{
			SynchronizationContext current = SynchronizationContext.Current;
			if (current != null && current.GetType() != typeof(SynchronizationContext))
			{
				return false;
			}
			return TaskScheduler.Current == TaskScheduler.Default;
		}
	}

	internal TaskAwaiter(Task task)
	{
		_task = task;
	}

	public void OnCompleted(Action continuation)
	{
		OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
	}

	[SecurityCritical]
	public void UnsafeOnCompleted(Action continuation)
	{
		OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
	}

	public void GetResult()
	{
		ValidateEnd(_task);
	}

	internal static void ValidateEnd(Task task)
	{
		if (task.Status != TaskStatus.RanToCompletion)
		{
			HandleNonSuccess(task);
		}
	}

	private static void HandleNonSuccess(Task task)
	{
		if (!task.IsCompleted)
		{
			try
			{
				task.Wait();
			}
			catch (Exception obj)
			{
				GC.KeepAlive(obj);
			}
		}
		if (task.Status != TaskStatus.RanToCompletion)
		{
			ThrowForNonSuccess(task);
		}
	}

	private static void ThrowForNonSuccess(Task task)
	{
		switch (task.Status)
		{
		case TaskStatus.Canceled:
			throw new TaskCanceledException(task);
		case TaskStatus.Faulted:
			throw PrepareExceptionForRethrow(task.Exception.InnerException);
		default:
			throw new InvalidOperationException("The task has not yet completed.");
		}
	}

	internal static void OnCompletedInternal(Task task, Action continuation, bool continueOnCapturedContext)
	{
		if (continuation == null)
		{
			throw new ArgumentNullException("continuation");
		}
		SynchronizationContext syncContext = (continueOnCapturedContext ? SynchronizationContext.Current : null);
		if (syncContext != null && syncContext.GetType() != typeof(SynchronizationContext))
		{
			task.ContinueWith(delegate
			{
				try
				{
					syncContext.Post(delegate(object state)
					{
						((Action)state)();
					}, continuation);
				}
				catch (Exception exception)
				{
					System.Runtime.CompilerServices.AsyncMethodBuilderCore.ThrowOnContext(exception, null);
				}
			}, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
			return;
		}
		TaskScheduler taskScheduler = (continueOnCapturedContext ? TaskScheduler.Current : TaskScheduler.Default);
		if (task.IsCompleted)
		{
			Task.Factory.StartNew(delegate(object state)
			{
				((Action)state)();
			}, continuation, CancellationToken.None, TaskCreationOptions.None, taskScheduler);
			return;
		}
		if (taskScheduler != TaskScheduler.Default)
		{
			task.ContinueWith(delegate
			{
				RunNoException(continuation);
			}, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, taskScheduler);
			return;
		}
		task.ContinueWith(delegate
		{
			if (IsValidLocationForInlining)
			{
				RunNoException(continuation);
			}
			else
			{
				Task.Factory.StartNew(delegate(object state)
				{
					RunNoException((Action)state);
				}, continuation, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
			}
		}, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
	}

	private static void RunNoException(Action continuation)
	{
		if (continuation == null)
		{
			return;
		}
		try
		{
			continuation();
		}
		catch (Exception exception)
		{
			System.Runtime.CompilerServices.AsyncMethodBuilderCore.ThrowOnContext(exception, null);
		}
	}

	internal static Exception PrepareExceptionForRethrow(Exception exc)
	{
		if (_prepForRemoting != null)
		{
			try
			{
				_prepForRemoting.Invoke(exc, _emptyParams);
			}
			catch (Exception obj)
			{
				GC.KeepAlive(obj);
			}
		}
		return exc;
	}

	private static MethodInfo GetPrepForRemotingMethodInfo()
	{
		try
		{
			return typeof(Exception).GetMethod("PrepForRemoting", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		catch (Exception obj)
		{
			GC.KeepAlive(obj);
			return null;
		}
	}
}
public struct TaskAwaiter<TResult> : ICriticalNotifyCompletion, INotifyCompletion
{
	private readonly Task<TResult> _task;

	public bool IsCompleted => _task.IsCompleted;

	internal TaskAwaiter(Task<TResult> task)
	{
		_task = task;
	}

	public void OnCompleted(Action continuation)
	{
		TaskAwaiter.OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
	}

	[SecurityCritical]
	public void UnsafeOnCompleted(Action continuation)
	{
		TaskAwaiter.OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
	}

	public TResult GetResult()
	{
		TaskAwaiter.ValidateEnd(_task);
		return _task.Result;
	}
}
