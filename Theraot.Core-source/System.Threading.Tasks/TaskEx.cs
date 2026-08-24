using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

public static class TaskEx
{
	private const string ArgumentOutOfRange_TimeoutNonNegativeOrMinusOne = "The timeout must be non-negative or -1, and it must be less than or equal to Int32.MaxValue.";

	private static readonly Task _preCanceledTask;

	private static readonly Task _preCompletedTask;

	static TaskEx()
	{
		_preCanceledTask = ((Func<Task>)delegate
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
			taskCompletionSource.TrySetCanceled();
			return taskCompletionSource.Task;
		})();
		_preCompletedTask = FromResult(result: false);
	}

	public static Task Delay(int dueTime)
	{
		return Delay(dueTime, CancellationToken.None);
	}

	public static Task Delay(TimeSpan dueTime)
	{
		return Delay(dueTime, CancellationToken.None);
	}

	public static Task Delay(TimeSpan dueTime, CancellationToken cancellationToken)
	{
		long num = (long)dueTime.TotalMilliseconds;
		if (num < -1 || num > int.MaxValue)
		{
			throw new ArgumentOutOfRangeException("dueTime", "The timeout must be non-negative or -1, and it must be less than or equal to Int32.MaxValue.");
		}
		return Delay((int)num, cancellationToken);
	}

	public static Task Delay(int dueTime, CancellationToken cancellationToken)
	{
		if (dueTime < -1)
		{
			throw new ArgumentOutOfRangeException("dueTime", "The timeout must be non-negative or -1, and it must be less than or equal to Int32.MaxValue.");
		}
		if (cancellationToken.IsCancellationRequested)
		{
			return _preCanceledTask;
		}
		if (dueTime == 0)
		{
			return _preCompletedTask;
		}
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		CancellationTokenRegistration ctr = default(CancellationTokenRegistration);
		Timer timer = null;
		timer = new Timer(delegate
		{
			ctr.Dispose();
			timer.Dispose();
			tcs.TrySetResult(result: true);
		}, null, -1, -1);
		if (cancellationToken.CanBeCanceled)
		{
			ctr = cancellationToken.Register(delegate
			{
				timer.Dispose();
				tcs.TrySetCanceled();
			});
		}
		timer.Change(dueTime, -1);
		return tcs.Task;
	}

	public static Task<TResult> FromResult<TResult>(TResult result)
	{
		TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>(result);
		taskCompletionSource.TrySetResult(result);
		return taskCompletionSource.Task;
	}

	public static Task Run(Action action)
	{
		return Run(action, CancellationToken.None);
	}

	public static Task Run(Action action, CancellationToken cancellationToken)
	{
		return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
	}

	public static Task<TResult> Run<TResult>(Func<TResult> function)
	{
		return Run(function, CancellationToken.None);
	}

	public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
	{
		return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
	}

	public static Task Run(Func<Task> function)
	{
		return Run(function, CancellationToken.None);
	}

	public static Task Run(Func<Task> function, CancellationToken cancellationToken)
	{
		return TaskEx.Run<Task>(function, cancellationToken).Unwrap();
	}

	public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
	{
		return Run(function, CancellationToken.None);
	}

	public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
	{
		return TaskEx.Run<Task<TResult>>(function, cancellationToken).Unwrap();
	}

	public static Task WhenAll(params Task[] tasks)
	{
		return WhenAll((IEnumerable<Task>)tasks);
	}

	public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
	{
		return WhenAll((IEnumerable<Task<TResult>>)tasks);
	}

	public static Task WhenAll(IEnumerable<Task> tasks)
	{
		return WhenAllCore(tasks, delegate(Task[] completedTasks, TaskCompletionSource<object> tcs)
		{
			tcs.TrySetResult(null);
		});
	}

	public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
	{
		return WhenAllCore(tasks.Cast<Task>(), delegate(Task[] completedTasks, TaskCompletionSource<TResult[]> tcs)
		{
			tcs.TrySetResult((from Task<TResult> t in completedTasks
				select t.Result).ToArray());
		});
	}

	public static Task<Task> WhenAny(params Task[] tasks)
	{
		return WhenAny((IEnumerable<Task>)tasks);
	}

	public static Task<Task> WhenAny(IEnumerable<Task> tasks)
	{
		if (tasks == null)
		{
			throw new ArgumentNullException("tasks");
		}
		TaskCompletionSource<Task> taskCompletionSource = new TaskCompletionSource<Task>();
		Task.Factory.ContinueWhenAny((tasks as Task[]) ?? tasks.ToArray(), (Func<Task, bool>)taskCompletionSource.TrySetResult, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		return taskCompletionSource.Task;
	}

	public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
	{
		return WhenAny((IEnumerable<Task<TResult>>)tasks);
	}

	public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
	{
		if (tasks == null)
		{
			throw new ArgumentNullException("tasks");
		}
		TaskCompletionSource<Task<TResult>> taskCompletionSource = new TaskCompletionSource<Task<TResult>>();
		Task.Factory.ContinueWhenAny((tasks as Task<TResult>[]) ?? tasks.ToArray(), (Func<Task<TResult>, bool>)taskCompletionSource.TrySetResult, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		return taskCompletionSource.Task;
	}

	public static YieldAwaitable Yield()
	{
		return default(YieldAwaitable);
	}

	private static void AddPotentiallyUnwrappedExceptions(ref List<Exception> targetList, Exception exception)
	{
		AggregateException ex = exception as AggregateException;
		if (targetList == null)
		{
			targetList = new List<Exception>();
		}
		if (ex != null)
		{
			targetList.Add((ex.InnerExceptions.Count == 1) ? exception.InnerException : exception);
		}
		else
		{
			targetList.Add(exception);
		}
	}

	private static Task<TResult> WhenAllCore<TResult>(IEnumerable<Task> tasks, Action<Task[], TaskCompletionSource<TResult>> setResultAction)
	{
		if (tasks == null)
		{
			throw new ArgumentNullException("tasks");
		}
		TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
		Task[] array = (tasks as Task[]) ?? tasks.ToArray();
		if (array.Length == 0)
		{
			setResultAction(array, tcs);
		}
		else
		{
			Task.Factory.ContinueWhenAll(array, delegate(Task[] completedTasks)
			{
				List<Exception> targetList = null;
				bool flag = false;
				foreach (Task task in completedTasks)
				{
					if (task.IsFaulted)
					{
						AddPotentiallyUnwrappedExceptions(ref targetList, task.Exception);
					}
					else if (task.IsCanceled)
					{
						flag = true;
					}
				}
				if (targetList != null && targetList.Count > 0)
				{
					tcs.TrySetException(targetList);
				}
				else if (flag)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					setResultAction(completedTasks, tcs);
				}
			}, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}
		return tcs.Task;
	}
}
