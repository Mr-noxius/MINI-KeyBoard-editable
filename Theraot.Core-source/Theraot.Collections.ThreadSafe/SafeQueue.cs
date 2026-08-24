using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public sealed class SafeQueue<T> : IEnumerable<T>, IEnumerable
{
	[Serializable]
	private class Node
	{
		internal readonly FixedSizeQueue<T> Queue;

		internal Node Next;

		public Node()
		{
			Queue = new FixedSizeQueue<T>(64);
		}

		public Node(IEnumerable<T> source)
		{
			Queue = new FixedSizeQueue<T>(source);
		}
	}

	private int _count;

	private Node _root;

	private Node _tail;

	public int Count => Volatile.Read(ref _count);

	public SafeQueue()
	{
		_root = new Node();
		_tail = _root;
	}

	public SafeQueue(IEnumerable<T> source)
	{
		_root = new Node(source);
		_count = _root.Queue.Count;
		_tail = _root;
	}

	public void Add(T item)
	{
		while (!_tail.Queue.Add(item))
		{
			Node node = new Node();
			Node node2 = Interlocked.CompareExchange(ref _tail.Next, node, null);
			_tail = node2 ?? node;
		}
		Interlocked.Increment(ref _count);
	}

	public IEnumerator<T> GetEnumerator()
	{
		Node root = _root;
		do
		{
			foreach (T item in root.Queue)
			{
				yield return item;
			}
			root = root.Next;
		}
		while (root != null);
	}

	public bool TryPeek(out T item)
	{
		Node node = _root;
		while (true)
		{
			if (_root.Queue.TryPeek(out item))
			{
				return true;
			}
			if (node.Next == null)
			{
				break;
			}
			Node node2 = Interlocked.CompareExchange(ref _root, node.Next, node);
			node = ((node2 == node) ? node.Next : node2);
		}
		item = default(T);
		return false;
	}

	public bool TryTake(out T item)
	{
		Node node = _root;
		while (true)
		{
			if (_root.Queue.TryTake(out item))
			{
				Interlocked.Decrement(ref _count);
				return true;
			}
			if (node.Next == null)
			{
				break;
			}
			Node node2 = Interlocked.CompareExchange(ref _root, node.Next, node);
			node = ((node2 == node) ? node.Next : node2);
		}
		item = default(T);
		return false;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
