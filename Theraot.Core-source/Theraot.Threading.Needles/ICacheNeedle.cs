namespace Theraot.Threading.Needles;

public interface ICacheNeedle<T> : INeedle<T>, IReadOnlyNeedle<T>, IPromise
{
	bool TryGetValue(out T value);
}
