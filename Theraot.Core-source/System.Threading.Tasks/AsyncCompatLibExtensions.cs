using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

public static class AsyncCompatLibExtensions
{
	public static ConfiguredTaskAwaitable<TResult> ConfigureAwait<TResult>(this Task<TResult> task, bool continueOnCapturedContext)
	{
		if (task == null)
		{
			throw new ArgumentNullException("task");
		}
		return new ConfiguredTaskAwaitable<TResult>(task, continueOnCapturedContext);
	}

	public static ConfiguredTaskAwaitable ConfigureAwait(this Task task, bool continueOnCapturedContext)
	{
		if (task == null)
		{
			throw new ArgumentNullException("task");
		}
		return new ConfiguredTaskAwaitable(task, continueOnCapturedContext);
	}

	public static TaskAwaiter GetAwaiter(this Task task)
	{
		if (task == null)
		{
			throw new ArgumentNullException("task");
		}
		return new TaskAwaiter(task);
	}

	public static TaskAwaiter<TResult> GetAwaiter<TResult>(this Task<TResult> task)
	{
		if (task == null)
		{
			throw new ArgumentNullException("task");
		}
		return new TaskAwaiter<TResult>(task);
	}
}
