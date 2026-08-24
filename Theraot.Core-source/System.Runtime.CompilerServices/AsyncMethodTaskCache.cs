using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

internal class AsyncMethodTaskCache<TResult>
{
	internal static readonly AsyncMethodTaskCache<TResult> Singleton;

	static AsyncMethodTaskCache()
	{
		Singleton = CreateCache();
	}

	internal static TaskCompletionSource<TResult> CreateCompleted(TResult result)
	{
		TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
		taskCompletionSource.TrySetResult(result);
		return taskCompletionSource;
	}

	private static AsyncMethodTaskCache<TResult> CreateCache()
	{
		Type typeFromHandle = typeof(TResult);
		if (typeFromHandle == typeof(bool))
		{
			return (AsyncMethodTaskCache<TResult>)(object)new AsyncMethodBooleanTaskCache();
		}
		if (typeFromHandle == typeof(int))
		{
			return (AsyncMethodTaskCache<TResult>)(object)new AsyncMethodInt32TaskCache();
		}
		return null;
	}

	internal virtual TaskCompletionSource<TResult> FromResult(TResult result)
	{
		return CreateCompleted(result);
	}
}
