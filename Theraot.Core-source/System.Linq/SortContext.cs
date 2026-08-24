namespace System.Linq;

internal abstract class SortContext<TElement>
{
	protected SortContext<TElement> ChildContext { get; set; }

	protected SortDirection Direction { get; set; }

	protected SortContext(SortDirection direction, SortContext<TElement> childContext)
	{
		Direction = direction;
		ChildContext = childContext;
	}

	public abstract int Compare(int firstIndex, int secondIndex);

	public abstract void Initialize(TElement[] elements);
}
