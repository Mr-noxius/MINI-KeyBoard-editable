using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

[StructLayout(LayoutKind.Auto)]
public struct ConfiguredValueTaskAwaitable<TResult>
{
	[StructLayout(LayoutKind.Auto)]
	public struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
	{
		private readonly ValueTask<TResult> _value;

		private readonly bool _continueOnCapturedContext;

		public bool IsCompleted => _value.IsCompleted;

		internal ConfiguredValueTaskAwaiter(ValueTask<TResult> value, bool continueOnCapturedContext)
		{
			_value = value;
			_continueOnCapturedContext = continueOnCapturedContext;
		}

		public TResult GetResult()
		{
			if (_value._task != null)
			{
				return AsyncCompatLibExtensions.GetAwaiter(_value._task).GetResult();
			}
			return _value._result;
		}

		public void OnCompleted(Action continuation)
		{
			AsyncCompatLibExtensions.ConfigureAwait(_value.AsTask(), _continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			AsyncCompatLibExtensions.ConfigureAwait(_value.AsTask(), _continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
		}
	}

	private readonly ValueTask<TResult> _value;

	private readonly bool _continueOnCapturedContext;

	internal ConfiguredValueTaskAwaitable(ValueTask<TResult> value, bool continueOnCapturedContext)
	{
		_value = value;
		_continueOnCapturedContext = continueOnCapturedContext;
	}

	public ConfiguredValueTaskAwaiter GetAwaiter()
	{
		return new ConfiguredValueTaskAwaiter(_value, _continueOnCapturedContext);
	}
}
