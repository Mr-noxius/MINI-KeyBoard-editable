using System;

namespace Theraot.Threading.Needles;

public interface IPromise
{
	Exception Exception { get; }

	bool IsCanceled { get; }

	bool IsCompleted { get; }

	bool IsFaulted { get; }
}
public interface IPromise<out T> : IPromise, IReadOnlyNeedle<T>
{
}
