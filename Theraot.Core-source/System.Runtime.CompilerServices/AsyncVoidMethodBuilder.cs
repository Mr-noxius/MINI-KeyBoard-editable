using System.Diagnostics;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

public struct AsyncVoidMethodBuilder : IAsyncMethodBuilder
{
	private readonly SynchronizationContext _synchronizationContext;

	private System.Runtime.CompilerServices.AsyncMethodBuilderCore _coreState;

	private object _objectIdForDebugger;

	private static int _preventUnobservedTaskExceptionsInvoked;

	private object ObjectIdForDebugger => _objectIdForDebugger ?? (_objectIdForDebugger = new object());

	static AsyncVoidMethodBuilder()
	{
		try
		{
			PreventUnobservedTaskExceptions();
		}
		catch (Exception obj)
		{
			GC.KeepAlive(obj);
		}
	}

	private AsyncVoidMethodBuilder(SynchronizationContext synchronizationContext)
	{
		_synchronizationContext = synchronizationContext;
		synchronizationContext?.OperationStarted();
		_coreState = default(System.Runtime.CompilerServices.AsyncMethodBuilderCore);
		_objectIdForDebugger = null;
	}

	internal static void PreventUnobservedTaskExceptions()
	{
		if (Interlocked.CompareExchange(ref _preventUnobservedTaskExceptionsInvoked, 1, 0) == 0)
		{
			TaskScheduler.UnobservedTaskException += delegate(object s, UnobservedTaskExceptionEventArgs e)
			{
				e.SetObserved();
			};
		}
	}

	public static AsyncVoidMethodBuilder Create()
	{
		return new AsyncVoidMethodBuilder(SynchronizationContext.Current);
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

	public void SetResult()
	{
		if (_synchronizationContext != null)
		{
			NotifySynchronizationContextOfCompletion();
		}
	}

	public void SetException(Exception exception)
	{
		if (exception == null)
		{
			throw new ArgumentNullException("exception");
		}
		if (_synchronizationContext != null)
		{
			try
			{
				System.Runtime.CompilerServices.AsyncMethodBuilderCore.ThrowOnContext(exception, _synchronizationContext);
				return;
			}
			finally
			{
				NotifySynchronizationContextOfCompletion();
			}
		}
		System.Runtime.CompilerServices.AsyncMethodBuilderCore.ThrowOnContext(exception, null);
	}

	private void NotifySynchronizationContextOfCompletion()
	{
		try
		{
			_synchronizationContext.OperationCompleted();
		}
		catch (Exception exception)
		{
			System.Runtime.CompilerServices.AsyncMethodBuilderCore.ThrowOnContext(exception, null);
		}
	}
}
