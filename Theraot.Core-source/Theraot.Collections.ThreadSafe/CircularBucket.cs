using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public sealed class CircularBucket<T> : IEnumerable<T>, IEnumerable
{
	private readonly int _capacity;

	private readonly FixedSizeBucket<T> _entries;

	private int _index;

	public int Capacity => _capacity;

	public int Count => _entries.Count;

	public CircularBucket(int capacity)
	{
		_capacity = ((NumericHelper.PopulationCount(capacity) == 1) ? capacity : NumericHelper.NextPowerOf2(capacity));
		_index = -1;
		_entries = new FixedSizeBucket<T>(_capacity);
	}

	public int Add(T item)
	{
		int num = Interlocked.Increment(ref _index) & (_capacity - 1);
		_entries.SetInternal(num, item, out var _);
		return num;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _entries.GetEnumerator();
	}

	public bool RemoveAt(int index)
	{
		return _entries.RemoveAt(index);
	}

	public bool RemoveAt(int index, out T previous)
	{
		return _entries.RemoveAt(index, out previous);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public int TryAdd(T item)
	{
		int num = Interlocked.Increment(ref _index) & (_capacity - 1);
		if (_entries.InsertInternal(num, item))
		{
			return num;
		}
		return -1;
	}

	public bool TryGet(int index, out T value)
	{
		return _entries.TryGet(index, out value);
	}
}
