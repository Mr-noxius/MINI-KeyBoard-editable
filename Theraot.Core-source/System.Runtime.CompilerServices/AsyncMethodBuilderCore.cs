using System.Diagnostics;
using System.Security;
using System.Threading;

namespace System.Runtime.CompilerServices;

internal struct AsyncMethodBuilderCore
{
	private sealed class MoveNextRunner
	{
		private readonly ExecutionContext _context;

		internal IAsyncStateMachine _stateMachine;

		[SecurityCritical]
		private static ContextCallback _invokeMoveNext;

		[SecurityCritical]
		internal MoveNextRunner(ExecutionContext context)
		{
			_context = context;
		}

		[SecuritySafeCritical]
		internal void Run()
		{
			if (_context == null)
			{
				_stateMachine.MoveNext();
				return;
			}
			ContextCallback callback = InvokeMoveNext;
			ExecutionContext.Run(_context, callback, _stateMachine);
		}

		[SecurityCritical]
		private static void InvokeMoveNext(object stateMachine)
		{
			((IAsyncStateMachine)stateMachine).MoveNext();
		}
	}

	internal IAsyncStateMachine _stateMachine;

	[DebuggerStepThrough]
	[SecuritySafeCritical]
	internal void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
	{
		if (object.ReferenceEquals(stateMachine, null))
		{
			throw new ArgumentNullException("stateMachine");
		}
		stateMachine.MoveNext();
	}

	public void SetStateMachine(IAsyncStateMachine stateMachine)
	{
		if (stateMachine == null)
		{
			throw new ArgumentNullException("stateMachine");
		}
		if (_stateMachine != null)
		{
			throw new InvalidOperationException("The builder was not properly initialized.");
		}
		_stateMachine = stateMachine;
	}

	[SecuritySafeCritical]
	internal Action GetCompletionAction<TMethodBuilder, TStateMachine>(ref TMethodBuilder builder, ref TStateMachine stateMachine) where TMethodBuilder : IAsyncMethodBuilder where TStateMachine : IAsyncStateMachine
	{
		MoveNextRunner moveNextRunner = new MoveNextRunner(ExecutionContext.Capture());
		Action result = moveNextRunner.Run;
		if (_stateMachine == null)
		{
			builder.PreBoxInitialization();
			_stateMachine = stateMachine;
			_stateMachine.SetStateMachine(_stateMachine);
		}
		moveNextRunner._stateMachine = _stateMachine;
		return result;
	}

	internal static void ThrowOnContext(Exception exception, SynchronizationContext targetContext)
	{
		if (targetContext != null)
		{
			try
			{
				targetContext.Post(delegate(object state)
				{
					throw TaskAwaiter.PrepareExceptionForRethrow((Exception)state);
				}, exception);
				return;
			}
			catch (Exception ex)
			{
				exception = new AggregateException(exception, ex);
			}
		}
		ThreadPool.QueueUserWorkItem(delegate(object state)
		{
			throw TaskAwaiter.PrepareExceptionForRethrow((Exception)state);
		}, exception);
	}
}
