using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

internal sealed class AsyncMethodInt32TaskCache : AsyncMethodTaskCache<int>
{
	private const int _minInt32ValueInclusive = -1;

	private const int _maxInt32ValueExclusive = 9;

	private static readonly TaskCompletionSource<int>[] _int32Tasks = CreateInt32Tasks();

	private static TaskCompletionSource<int>[] CreateInt32Tasks()
	{
		TaskCompletionSource<int>[] array = new TaskCompletionSource<int>[10];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = AsyncMethodTaskCache<int>.CreateCompleted(i - 1);
		}
		return array;
	}

	internal override TaskCompletionSource<int> FromResult(int result)
	{
		if (result < -1 || result >= 9)
		{
			return AsyncMethodTaskCache<int>.CreateCompleted(result);
		}
		return _int32Tasks[result - -1];
	}
}
