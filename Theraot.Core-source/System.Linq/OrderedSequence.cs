using System.Collections.Generic;

namespace System.Linq;

internal class OrderedSequence<TElement, TKey> : System.Linq.OrderedEnumerable<TElement>
{
	private readonly IComparer<TKey> _comparer;

	private readonly SortDirection _direction;

	private readonly System.Linq.OrderedEnumerable<TElement> _parent;

	private readonly Func<TElement, TKey> _selector;

	internal OrderedSequence(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, SortDirection direction)
		: base(source)
	{
		_selector = keySelector;
		_comparer = comparer ?? Comparer<TKey>.Default;
		_direction = direction;
	}

	internal OrderedSequence(System.Linq.OrderedEnumerable<TElement> parent, IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, SortDirection direction)
		: this(source, keySelector, comparer, direction)
	{
		_parent = parent;
	}

	public override SortContext<TElement> CreateContext(SortContext<TElement> current)
	{
		SortContext<TElement> sortContext = new SortSequenceContext<TElement, TKey>(_selector, _comparer, _direction, current);
		if (_parent != null)
		{
			return _parent.CreateContext(sortContext);
		}
		return sortContext;
	}

	protected override IEnumerable<TElement> Sort(IEnumerable<TElement> source)
	{
		return QuickSort<TElement>.Sort(source, CreateContext(null));
	}
}
