using System.Collections.Generic;

namespace System.Linq;

internal class SortSequenceContext<TElement, TKey> : SortContext<TElement>
{
	private readonly IComparer<TKey> _comparer;

	private readonly Func<TElement, TKey> _selector;

	private TKey[] _keys;

	public SortSequenceContext(Func<TElement, TKey> selector, IComparer<TKey> comparer, SortDirection direction, SortContext<TElement> childContext)
		: base(direction, childContext)
	{
		_selector = selector;
		_comparer = comparer;
	}

	public override int Compare(int firstIndex, int secondIndex)
	{
		int num = _comparer.Compare(_keys[firstIndex], _keys[secondIndex]);
		if (num == 0)
		{
			if (base.ChildContext != null)
			{
				return base.ChildContext.Compare(firstIndex, secondIndex);
			}
			num = ((base.Direction == SortDirection.Descending) ? (secondIndex - firstIndex) : (firstIndex - secondIndex));
		}
		if (base.Direction != SortDirection.Descending)
		{
			return num;
		}
		return -num;
	}

	public override void Initialize(TElement[] elements)
	{
		if (base.ChildContext != null)
		{
			base.ChildContext.Initialize(elements);
		}
		_keys = new TKey[elements.Length];
		for (int i = 0; i < _keys.Length; i++)
		{
			_keys[i] = _selector(elements[i]);
		}
	}
}
