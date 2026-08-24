using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

public struct AsyncTaskMethodBuilder : IAsyncMethodBuilder
{
	private static readonly TaskCompletionSource<VoidTaskResult> _cachedCompleted = AsyncTaskMethodBuilder<VoidTaskResult>._defaultResultTask;

	private AsyncTaskMethodBuilder<VoidTaskResult> _builder;

	public Task Task => _builder.Task;

	private object ObjectIdForDebugger => Task;

	public static AsyncTaskMethodBuilder Create()
	{
		return default(AsyncTaskMethodBuilder);
	}

	[DebuggerStepThrough]
	public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
	{
		_builder.Start(ref stateMachine);
	}

	public void SetStateMachine(IAsyncStateMachine stateMachine)
	{
		_builder.SetStateMachine(stateMachine);
	}

	void IAsyncMethodBuilder.PreBoxInitialization()
	{
		GC.KeepAlive(Task);
	}

	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		_builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
	}

	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		_builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
	}

	public void SetResult()
	{
		_builder.SetResult(_cachedCompleted);
	}

	public void SetException(Exception exception)
	{
		_builder.SetException(exception);
	}

	internal void SetNotificationForWaitCompletion(bool enabled)
	{
		_builder.SetNotificationForWaitCompletion(enabled);
	}
}
public struct AsyncTaskMethodBuilder<TResult> : IAsyncMethodBuilder
{
	internal static readonly TaskCompletionSource<TResult> _defaultResultTask;

	private System.Runtime.CompilerServices.AsyncMethodBuilderCore _coreState;

	private TaskCompletionSource<TResult> _task;

	internal TaskCompletionSource<TResult> CompletionSource
	{
		get
		{
			TaskCompletionSource<TResult> taskCompletionSource = _task;
			if (taskCompletionSource == null)
			{
				taskCompletionSource = (_task = new TaskCompletionSource<TResult>());
			}
			return taskCompletionSource;
		}
	}

	public Task<TResult> Task => CompletionSource.Task;

	private object ObjectIdForDebugger => Task;

	static AsyncTaskMethodBuilder()
	{
		_defaultResultTask = AsyncMethodTaskCache<TResult>.CreateCompleted(default(TResult));
		try
		{
			AsyncVoidMethodBuilder.PreventUnobservedTaskExceptions();
		}
		catch (Exception obj)
		{
			GC.KeepAlive(obj);
		}
	}

	public static AsyncTaskMethodBuilder<TResult> Create()
	{
		return default(AsyncTaskMethodBuilder<TResult>);
	}

	[DebuggerStepThrough]
	public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
	{
		_coreState.Start(ref stateMachine);
	}

	public void SetStateMachine(IAsyncStateMachine stateMachine)
	{
		_coreState.SetStateMachine(stateMachine);
	}

	void IAsyncMethodBuilder.PreBoxInitialization()
	{
		GC.KeepAlive(Task);
	}

	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		try
		{
			Action completionAction = _coreState.GetCompletionAction(ref this, ref stateMachine);
			awaiter.OnCompleted(completionAction);
		}
		catch (Exception exception)
		{
			System.Runtime.CompilerServices.AsyncMethodBuilderCore.ThrowOnContext(exception, null);
		}
	}

	[SecuritySafeCritical]
	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		try
		{
			Action completionAction = _coreState.GetCompletionAction(ref this, ref stateMachine);
			awaiter.UnsafeOnCompleted(completionAction);
		}
		catch (Exception exception)
		{
			System.Runtime.CompilerServices.AsyncMethodBuilderCore.ThrowOnContext(exception, null);
		}
	}

	public void SetResult(TResult result)
	{
		TaskCompletionSource<TResult> task = _task;
		if (task == null)
		{
			_task = GetTaskForResult(result);
		}
		else if (!task.TrySetResult(result))
		{
			throw new InvalidOperationException("The Task was already completed.");
		}
	}

	internal void SetResult(TaskCompletionSource<TResult> completedTask)
	{
		if (_task == null)
		{
			_task = completedTask;
		}
		else
		{
			SetResult(default(TResult));
		}
	}

	public void SetException(Exception exception)
	{
		if (exception == null)
		{
			throw new ArgumentNullException("exception");
		}
		TaskCompletionSource<TResult> completionSource = CompletionSource;
		if (!((exception is OperationCanceledException) ? completionSource.TrySetCanceled() : completionSource.TrySetException(exception)))
		{
			throw new InvalidOperationException("The Task was already completed.");
		}
	}

	internal void SetNotificationForWaitCompletion(bool enabled)
	{
		GC.KeepAlive(enabled);
	}

	private TaskCompletionSource<TResult> GetTaskForResult(TResult result)
	{
		AsyncMethodTaskCache<TResult> singleton = AsyncMethodTaskCache<TResult>.Singleton;
		if (singleton == null)
		{
			return AsyncMethodTaskCache<TResult>.CreateCompleted(result);
		}
		return singleton.FromResult(result);
	}
}
