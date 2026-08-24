using System.Collections;
using System.Collections.Generic;

namespace System.Linq;

internal abstract class OrderedEnumerable<TElement> : IOrderedEnumerable<TElement>, IEnumerable<TElement>, IEnumerable
{
	private readonly IEnumerable<TElement> _source;

	protected OrderedEnumerable(IEnumerable<TElement> source)
	{
		_source = source;
	}

	public abstract SortContext<TElement> CreateContext(SortContext<TElement> current);

	public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> selector, IComparer<TKey> comparer, bool descending)
	{
		return new OrderedSequence<TElement, TKey>(this, _source, selector, comparer, descending ? SortDirection.Descending : SortDirection.Ascending);
	}

	public IEnumerator<TElement> GetEnumerator()
	{
		return Sort(_source).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	protected abstract IEnumerable<TElement> Sort(IEnumerable<TElement> source);
}
