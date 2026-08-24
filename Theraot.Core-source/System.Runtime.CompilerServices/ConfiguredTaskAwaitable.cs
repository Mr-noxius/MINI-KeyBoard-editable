using System.Security;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

public struct ConfiguredTaskAwaitable<TResult>
{
	public struct ConfiguredTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
	{
		private readonly Task<TResult> _task;

		private readonly bool _continueOnCapturedContext;

		public bool IsCompleted => _task.IsCompleted;

		internal ConfiguredTaskAwaiter(Task<TResult> task, bool continueOnCapturedContext)
		{
			_task = task;
			_continueOnCapturedContext = continueOnCapturedContext;
		}

		public void OnCompleted(Action continuation)
		{
			TaskAwaiter.OnCompletedInternal(_task, continuation, _continueOnCapturedContext);
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

	private readonly ConfiguredTaskAwaiter _configuredTaskAwaiter;

	internal ConfiguredTaskAwaitable(Task<TResult> task, bool continueOnCapturedContext)
	{
		_configuredTaskAwaiter = new ConfiguredTaskAwaiter(task, continueOnCapturedContext);
	}

	public ConfiguredTaskAwaiter GetAwaiter()
	{
		return _configuredTaskAwaiter;
	}
}
public struct ConfiguredTaskAwaitable
{
	public struct ConfiguredTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
	{
		private readonly Task _task;

		private readonly bool _continueOnCapturedContext;

		public bool IsCompleted => _task.IsCompleted;

		internal ConfiguredTaskAwaiter(Task task, bool continueOnCapturedContext)
		{
			_task = task;
			_continueOnCapturedContext = continueOnCapturedContext;
		}

		public void OnCompleted(Action continuation)
		{
			TaskAwaiter.OnCompletedInternal(_task, continuation, _continueOnCapturedContext);
		}

		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation)
		{
			TaskAwaiter.OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
		}

		public void GetResult()
		{
			TaskAwaiter.ValidateEnd(_task);
		}
	}

	private readonly ConfiguredTaskAwaiter _configuredTaskAwaiter;

	internal ConfiguredTaskAwaitable(Task task, bool continueOnCapturedContext)
	{
		_configuredTaskAwaiter = new ConfiguredTaskAwaiter(task, continueOnCapturedContext);
	}

	public ConfiguredTaskAwaiter GetAwaiter()
	{
		return _configuredTaskAwaiter;
	}
}
