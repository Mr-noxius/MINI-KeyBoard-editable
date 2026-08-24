using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[Serializable]
[DebuggerNonUserCode]
public class ReadOnlyPromise : IWaitablePromise, IPromise
{
	private readonly IPromise _promised;

	private readonly Action _wait;

	public Exception Exception => _promised.Exception;

	public bool IsCanceled => _promised.IsCanceled;

	public bool IsCompleted => _promised.IsCompleted;

	public bool IsFaulted => _promised.IsFaulted;

	public ReadOnlyPromise(IPromise promised, bool allowWait)
	{
		_promised = promised;
		if (allowWait)
		{
			if (_promised is IWaitablePromise waitablePromise)
			{
				_wait = waitablePromise.Wait;
				return;
			}
			_wait = delegate
			{
				ThreadingHelper.SpinWaitUntil(() => _promised.IsCompleted);
			};
		}
		else
		{
			_wait = delegate
			{
				throw new InvalidOperationException();
			};
		}
	}

	public override int GetHashCode()
	{
		return _promised.GetHashCode();
	}

	public override string ToString()
	{
		return $"{{Promise: {_promised}}}";
	}

	public void Wait()
	{
		_wait();
	}
}
