using System.Collections.Generic;

namespace System.Linq;

internal class QuickSort<TElement>
{
	private readonly SortContext<TElement> _context;

	private readonly TElement[] _elements;

	private readonly int[] _indexes;

	private QuickSort(IEnumerable<TElement> source, SortContext<TElement> context)
	{
		_elements = source.ToArray();
		_indexes = CreateIndexes(_elements.Length);
		_context = context;
	}

	public static IEnumerable<TElement> Sort(IEnumerable<TElement> source, SortContext<TElement> context)
	{
		QuickSort<TElement> sorter = new QuickSort<TElement>(source, context);
		sorter.PerformSort();
		try
		{
			int[] indexes = sorter._indexes;
			foreach (int item in indexes)
			{
				yield return sorter._elements[item];
			}
		}
		finally
		{
		}
	}

	private static int[] CreateIndexes(int length)
	{
		int[] array = new int[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = i;
		}
		return array;
	}

	private int CompareItems(int firstIndex, int secondIndex)
	{
		return _context.Compare(firstIndex, secondIndex);
	}

	private void InsertionSort(int left, int right)
	{
		for (int i = left + 1; i <= right; i++)
		{
			int num = _indexes[i];
			int num2 = i;
			while (num2 > left && CompareItems(num, _indexes[num2 - 1]) < 0)
			{
				_indexes[num2] = _indexes[num2 - 1];
				num2--;
			}
			_indexes[num2] = num;
		}
	}

	private int MedianOfThree(int left, int right)
	{
		int num = (left + right) / 2;
		if (CompareItems(_indexes[num], _indexes[left]) < 0)
		{
			Swap(left, num);
		}
		if (CompareItems(_indexes[right], _indexes[left]) < 0)
		{
			Swap(left, right);
		}
		if (CompareItems(_indexes[right], _indexes[num]) < 0)
		{
			Swap(num, right);
		}
		Swap(num, right - 1);
		return _indexes[right - 1];
	}

	private void PerformSort()
	{
		if (_elements.Length > 1)
		{
			_context.Initialize(_elements);
			Sort(0, _indexes.Length - 1);
		}
	}

	private void Sort(int left, int right)
	{
		if (left + 3 <= right)
		{
			int num = left;
			int num2 = right - 1;
			int secondIndex = MedianOfThree(left, right);
			while (true)
			{
				if (CompareItems(_indexes[++num], secondIndex) >= 0)
				{
					while (CompareItems(_indexes[--num2], secondIndex) > 0)
					{
					}
					if (num >= num2)
					{
						break;
					}
					Swap(num, num2);
				}
			}
			Swap(num, right - 1);
			Sort(left, num - 1);
			Sort(num + 1, right);
		}
		else
		{
			InsertionSort(left, right);
		}
	}

	private void Swap(int left, int right)
	{
		int num = _indexes[right];
		_indexes[right] = _indexes[left];
		_indexes[left] = num;
	}
}
