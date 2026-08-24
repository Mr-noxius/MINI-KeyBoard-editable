using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public sealed class FixedSizeQueue<T> : IEnumerable<T>, IEnumerable
{
	private readonly int _capacity;

	private readonly FixedSizeBucket<T> _entries;

	private int _indexDequeue;

	private int _indexEnqueue;

	private int _preCount;

	public int Capacity => _capacity;

	public int Count => _entries.Count;

	public FixedSizeQueue(int capacity)
	{
		_capacity = ((NumericHelper.PopulationCount(capacity) == 1) ? capacity : NumericHelper.NextPowerOf2(capacity));
		_preCount = 0;
		_indexEnqueue = 0;
		_indexDequeue = 0;
		_entries = new FixedSizeBucket<T>(_capacity);
	}

	public FixedSizeQueue(IEnumerable<T> source)
	{
		_indexDequeue = 0;
		_entries = new FixedSizeBucket<T>(source);
		_capacity = _entries.Capacity;
		_indexEnqueue = _entries.Count;
		_preCount = _indexEnqueue;
	}

	public bool Add(T item)
	{
		if (_entries.Count < _capacity)
		{
			int num = Interlocked.Increment(ref _preCount);
			if (num <= _capacity)
			{
				int index = (Interlocked.Increment(ref _indexEnqueue) - 1) & (_capacity - 1);
				if (_entries.InsertInternal(index, item))
				{
					return true;
				}
			}
			Interlocked.Decrement(ref _preCount);
		}
		return false;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _entries.GetEnumerator();
	}

	public T Peek()
	{
		int num = Interlocked.Add(ref _indexEnqueue, 0);
		if (num < _capacity && num > 0 && _entries.TryGet(num, out var value))
		{
			return value;
		}
		throw new InvalidOperationException("Empty");
	}

	public bool TryGet(int index, out T item)
	{
		return _entries.TryGet(index, out item);
	}

	public bool TryPeek(out T item)
	{
		item = default(T);
		int num = Interlocked.Add(ref _indexDequeue, 0);
		if (num < _capacity && num > 0)
		{
			return _entries.TryGetInternal(num, out item);
		}
		return false;
	}

	public bool TryTake(out T item)
	{
		if (_entries.Count > 0)
		{
			int num = Interlocked.Decrement(ref _preCount);
			if (num >= 0)
			{
				int index = (Interlocked.Increment(ref _indexDequeue) - 1) & (_capacity - 1);
				if (_entries.RemoveAtInternal(index, out item))
				{
					return true;
				}
			}
			Interlocked.Increment(ref _preCount);
		}
		item = default(T);
		return false;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
