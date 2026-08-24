using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

internal sealed class AsyncMethodBooleanTaskCache : AsyncMethodTaskCache<bool>
{
	private readonly TaskCompletionSource<bool> _true = AsyncMethodTaskCache<bool>.CreateCompleted(result: true);

	private readonly TaskCompletionSource<bool> _false = AsyncMethodTaskCache<bool>.CreateCompleted(result: false);

	internal override TaskCompletionSource<bool> FromResult(bool result)
	{
		if (!result)
		{
			return _false;
		}
		return _true;
	}
}
