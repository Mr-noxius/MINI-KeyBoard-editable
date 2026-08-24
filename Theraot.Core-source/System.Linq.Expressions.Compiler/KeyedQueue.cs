using System.Collections.Generic;

namespace System.Linq.Expressions.Compiler;

internal sealed class KeyedQueue<TK, TV>
{
	private readonly Dictionary<TK, Queue<TV>> _data;

	internal KeyedQueue()
	{
		_data = new Dictionary<TK, Queue<TV>>();
	}

	internal void Enqueue(TK key, TV value)
	{
		if (!_data.TryGetValue(key, out var value2))
		{
			_data.Add(key, value2 = new Queue<TV>());
		}
		value2.Enqueue(value);
	}

	internal TV Dequeue(TK key)
	{
		if (!_data.TryGetValue(key, out var value))
		{
			throw System.Linq.Expressions.Error.QueueEmpty();
		}
		TV result = value.Dequeue();
		if (value.Count == 0)
		{
			_data.Remove(key);
		}
		return result;
	}

	internal bool TryDequeue(TK key, out TV value)
	{
		if (_data.TryGetValue(key, out var value2) && value2.Count > 0)
		{
			value = value2.Dequeue();
			if (value2.Count == 0)
			{
				_data.Remove(key);
			}
			return true;
		}
		value = default(TV);
		return false;
	}

	internal TV Peek(TK key)
	{
		if (!_data.TryGetValue(key, out var value))
		{
			throw System.Linq.Expressions.Error.QueueEmpty();
		}
		return value.Peek();
	}

	internal int GetCount(TK key)
	{
		if (!_data.TryGetValue(key, out var value))
		{
			return 0;
		}
		return value.Count;
	}

	internal void Clear()
	{
		_data.Clear();
	}
}
