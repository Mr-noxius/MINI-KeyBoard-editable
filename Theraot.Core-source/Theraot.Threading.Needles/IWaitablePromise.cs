namespace Theraot.Threading.Needles;

public interface IWaitablePromise : IPromise
{
	void Wait();
}
public interface IWaitablePromise<out T> : IPromise<T>, IReadOnlyNeedle<T>, IWaitablePromise, IPromise
{
}
