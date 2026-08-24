namespace Theraot.Threading.Needles;

public interface IRecyclableNeedle<T> : INeedle<T>, IReadOnlyNeedle<T>
{
	void Free();
}
