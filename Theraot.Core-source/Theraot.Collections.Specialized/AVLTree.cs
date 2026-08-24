using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.Specialized;

[Serializable]
public sealed class AVLTree<TKey, TValue> : IEnumerable<AVLTree<TKey, TValue>.AVLNode>, IEnumerable
{
	[Serializable]
	public sealed class AVLNode
	{
		private readonly TKey _key;

		private readonly TValue _value;

		private int _balance;

		private int _depth;

		private AVLNode _left;

		private AVLNode _right;

		public TKey Key => _key;

		public AVLNode Left => _left;

		public AVLNode Right => _right;

		public TValue Value => _value;

		private AVLNode(TKey key, TValue value)
		{
			_key = key;
			_value = value;
			_left = null;
			_right = null;
		}

		public int CompareTo(AVLNode other, IComparer<TKey> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			if (other == null)
			{
				return 1;
			}
			return comparer.Compare(_key, other._key);
		}

		internal static void Add(ref AVLNode node, TKey key, TValue value, Comparison<TKey> comparison)
		{
			AVLNode created = new AVLNode(key, value);
			AddExtracted(ref node, key, comparison, created);
		}

		internal static bool AddNonDuplicate(ref AVLNode node, TKey key, TValue value, Comparison<TKey> comparison)
		{
			return AddNonDuplicateExtracted(ref node, key, value, comparison, null);
		}

		internal static void Bound(AVLNode node, TKey key, Comparison<TKey> comparison, out AVLNode lower, out AVLNode upper)
		{
			lower = null;
			upper = null;
			while (node != null)
			{
				int num = comparison(key, node._key);
				if (num <= 0)
				{
					upper = node;
				}
				if (num >= 0)
				{
					lower = node;
				}
				if (num == 0)
				{
					break;
				}
				node = ((num > 0) ? node._right : node._left);
			}
		}

		internal static IEnumerable<AVLNode> EnumerateFrom(AVLNode node, TKey key, Comparison<TKey> comparison)
		{
			Stack<AVLNode> stack = new Stack<AVLNode>();
			while (node != null)
			{
				int num = comparison(key, node._key);
				if (num == 0)
				{
					break;
				}
				if (num > 0)
				{
					node = node._right;
					continue;
				}
				stack.Push(node);
				node = node._left;
			}
			while (true)
			{
				if (node != null)
				{
					yield return node;
					foreach (AVLNode item in EnumerateRoot(node._right))
					{
						yield return item;
					}
				}
				if (stack.Count != 0)
				{
					node = stack.Pop();
					continue;
				}
				break;
			}
		}

		internal static IEnumerable<AVLNode> EnumerateRoot(AVLNode node)
		{
			if (node == null)
			{
				yield break;
			}
			Stack<AVLNode> stack = new Stack<AVLNode>();
			while (true)
			{
				if (node != null)
				{
					stack.Push(node);
					node = node.Left;
					continue;
				}
				if (stack.Count > 0)
				{
					node = stack.Pop();
					yield return node;
					node = node.Right;
					continue;
				}
				break;
			}
		}

		internal static AVLNode Get(AVLNode node, TKey key, Comparison<TKey> comparison)
		{
			while (node != null)
			{
				int num = comparison(key, node._key);
				if (num == 0)
				{
					break;
				}
				node = ((num > 0) ? node._right : node._left);
			}
			return node;
		}

		internal static AVLNode GetFirst(AVLNode node)
		{
			AVLNode result = null;
			while (node != null)
			{
				result = node;
				node = node._left;
			}
			return result;
		}

		internal static AVLNode GetLast(AVLNode node)
		{
			AVLNode result = null;
			while (node != null)
			{
				result = node;
				node = node._right;
			}
			return result;
		}

		internal static AVLNode GetNearestLeft(AVLNode node, TKey key, Comparison<TKey> comparison)
		{
			AVLNode result = null;
			while (node != null)
			{
				int num = comparison(key, node._key);
				if (num >= 0)
				{
					result = node;
				}
				if (num == 0)
				{
					break;
				}
				node = ((num < 0) ? node._left : node._right);
			}
			return result;
		}

		internal static AVLNode GetNearestRight(AVLNode node, TKey key, Comparison<TKey> comparison)
		{
			AVLNode result = null;
			while (node != null)
			{
				int num = comparison(key, node._key);
				if (num <= 0)
				{
					result = node;
				}
				if (num == 0)
				{
					break;
				}
				node = ((num > 0) ? node._right : node._left);
			}
			return result;
		}

		internal static AVLNode GetOrAdd(ref AVLNode node, TKey key, Func<TKey, TValue> factory, Comparison<TKey> comparison, out bool isNew)
		{
			return GetOrAddExtracted(ref node, key, factory, comparison, null, out isNew);
		}

		internal static bool Remove(ref AVLNode node, TKey key, Comparison<TKey> comparison)
		{
			if (node == null)
			{
				return false;
			}
			int num = comparison(key, node._key);
			if (num == 0)
			{
				return RemoveExtracted(ref node);
			}
			try
			{
				if (num < 0)
				{
					return Remove(ref node._left, key, comparison);
				}
				return Remove(ref node._right, key, comparison);
			}
			finally
			{
				MakeBalanced(ref node);
			}
		}

		internal static AVLNode RemoveNearestLeft(ref AVLNode node, TKey key, Comparison<TKey> comparison)
		{
			AVLNode result = null;
			return RemoveNearestLeftExtracted(ref node, ref result, key, comparison);
		}

		internal static AVLNode RemoveNearestRight(ref AVLNode node, TKey key, Comparison<TKey> comparison)
		{
			AVLNode result = null;
			return RemoveNearestRightExtracted(ref node, ref result, key, comparison);
		}

		private static void AddExtracted(ref AVLNode node, TKey key, Comparison<TKey> comparison, AVLNode created)
		{
			int num;
			if (node == null || (num = comparison(key, node._key)) == 0)
			{
				if (Interlocked.CompareExchange(ref node, created, null) == null)
				{
					return;
				}
				num = -node._balance;
			}
			if (num < 0)
			{
				AddExtracted(ref node._left, key, comparison, created);
			}
			else
			{
				AddExtracted(ref node._right, key, comparison, created);
			}
			MakeBalanced(ref node);
		}

		private static bool AddNonDuplicateExtracted(ref AVLNode node, TKey key, TValue value, Comparison<TKey> comparison, AVLNode created)
		{
			if (node == null)
			{
				if (created == null)
				{
					created = new AVLNode(key, value);
				}
				AVLNode aVLNode = Interlocked.CompareExchange(ref node, created, null);
				if (aVLNode == null)
				{
					return true;
				}
				node = aVLNode;
			}
			int num = comparison(key, node._key);
			if (num == 0)
			{
				return false;
			}
			try
			{
				if (num < 0)
				{
					return AddNonDuplicateExtracted(ref node._left, key, value, comparison, created);
				}
				return AddNonDuplicateExtracted(ref node._right, key, value, comparison, created);
			}
			finally
			{
				MakeBalanced(ref node);
			}
		}

		private static void DoubleLeft(ref AVLNode node)
		{
			if (node._right != null)
			{
				RotateRight(ref node._right);
				RotateLeft(ref node);
			}
		}

		private static void DoubleRight(ref AVLNode node)
		{
			if (node._left != null)
			{
				RotateLeft(ref node._left);
				RotateRight(ref node);
			}
		}

		private static AVLNode GetOrAddExtracted(ref AVLNode node, TKey key, Func<TKey, TValue> factory, Comparison<TKey> comparison, AVLNode created, out bool isNew)
		{
			if (node == null)
			{
				if (created == null)
				{
					created = new AVLNode(key, factory(key));
					factory = null;
				}
				AVLNode aVLNode = Interlocked.CompareExchange(ref node, created, null);
				if (aVLNode == null)
				{
					isNew = true;
					return created;
				}
				node = aVLNode;
			}
			int num = comparison(key, node._key);
			if (num == 0)
			{
				isNew = false;
				return node;
			}
			try
			{
				if (num < 0)
				{
					return GetOrAddExtracted(ref node._left, key, factory, comparison, created, out isNew);
				}
				return GetOrAddExtracted(ref node._right, key, factory, comparison, created, out isNew);
			}
			finally
			{
				MakeBalanced(ref node);
			}
		}

		private static void MakeBalanced(ref AVLNode node)
		{
			AVLNode aVLNode;
			do
			{
				aVLNode = node;
				Update(node);
				if (node._balance >= 2)
				{
					if (node._right._balance <= 1)
					{
						DoubleLeft(ref node);
					}
					else
					{
						RotateLeft(ref node);
					}
				}
				else if (node._balance <= -2)
				{
					if (node._left._balance >= 1)
					{
						DoubleRight(ref node);
					}
					else
					{
						RotateRight(ref node);
					}
				}
			}
			while (node != aVLNode);
		}

		private static bool RemoveExtracted(ref AVLNode node)
		{
			if (node == null)
			{
				return false;
			}
			if (node._right == null)
			{
				node = node._left;
			}
			else if (node._left == null)
			{
				node = node._right;
			}
			else
			{
				AVLNode aVLNode = node._right;
				AVLNode aVLNode2 = aVLNode;
				while (aVLNode2._left != null)
				{
					aVLNode = aVLNode2;
					aVLNode2 = aVLNode._left;
				}
				if (object.ReferenceEquals(aVLNode, aVLNode2))
				{
					node._right = aVLNode2._right;
				}
				else
				{
					aVLNode._left = aVLNode2._right;
				}
				AVLNode left = node._left;
				AVLNode right = node._right;
				int balance = node._balance;
				node = new AVLNode(aVLNode2._key, aVLNode2._value)
				{
					_left = left,
					_right = right,
					_balance = balance
				};
			}
			if (node != null)
			{
				MakeBalanced(ref node);
			}
			return true;
		}

		private static AVLNode RemoveNearestLeftExtracted(ref AVLNode node, ref AVLNode result, TKey key, Comparison<TKey> comparison)
		{
			if (node == null)
			{
				return null;
			}
			int num = comparison(key, node._key);
			AVLNode result2;
			if (num == 0)
			{
				result2 = node;
				RemoveExtracted(ref node);
				return result2;
			}
			if (num < 0)
			{
				result2 = RemoveNearestLeftExtracted(ref node._left, ref result, key, comparison);
				if (result2 == null)
				{
					result2 = result;
					RemoveExtracted(ref result);
				}
				MakeBalanced(ref node);
			}
			else
			{
				result2 = RemoveNearestLeftExtracted(ref node._right, ref node, key, comparison);
				if (result2 == null)
				{
					result2 = node;
					RemoveExtracted(ref node);
				}
			}
			return result2;
		}

		private static AVLNode RemoveNearestRightExtracted(ref AVLNode node, ref AVLNode result, TKey key, Comparison<TKey> comparison)
		{
			if (node == null)
			{
				return null;
			}
			int num = comparison(key, node._key);
			AVLNode result2;
			if (num == 0)
			{
				result2 = node;
				RemoveExtracted(ref node);
				return result2;
			}
			if (num < 0)
			{
				result2 = RemoveNearestRightExtracted(ref node._left, ref node, key, comparison);
				if (result2 == null)
				{
					result2 = node;
					RemoveExtracted(ref node);
				}
			}
			else
			{
				result2 = RemoveNearestRightExtracted(ref node._right, ref result, key, comparison);
				if (result2 == null)
				{
					result2 = result;
					RemoveExtracted(ref result);
				}
				MakeBalanced(ref node);
			}
			return result2;
		}

		private static void RotateLeft(ref AVLNode node)
		{
			AVLNode aVLNode = node;
			AVLNode right = node._right;
			if (right != null)
			{
				AVLNode left = right._left;
				node._right = left;
				right._left = aVLNode;
				node = right;
				Update(aVLNode);
				Update(right);
			}
		}

		private static void RotateRight(ref AVLNode node)
		{
			AVLNode aVLNode = node;
			AVLNode left = node._left;
			if (left != null)
			{
				AVLNode right = left._right;
				node._left = right;
				left._right = aVLNode;
				node = left;
				Update(aVLNode);
				Update(left);
			}
		}

		private static void Update(AVLNode node)
		{
			AVLNode right = node._right;
			AVLNode left = node._left;
			node._depth = Math.Max((right != null) ? (right._depth + 1) : 0, (left != null) ? (left._depth + 1) : 0);
			node._balance = ((right != null) ? (right._depth + 1) : 0) - ((left != null) ? (left._depth + 1) : 0);
		}
	}

	private readonly Comparison<TKey> _comparison;

	private int _count;

	private AVLNode _root;

	public int Count => _count;

	public AVLNode Root => _root;

	public AVLTree()
	{
		_root = null;
		_comparison = Comparer<TKey>.Default.Compare;
	}

	public AVLTree(IComparer<TKey> comparer)
	{
		_root = null;
		_comparison = (comparer ?? Comparer<TKey>.Default).Compare;
	}

	public AVLTree(Comparison<TKey> comparison)
	{
		_root = null;
		_comparison = comparison ?? new Comparison<TKey>(Comparer<TKey>.Default.Compare);
	}

	public void Add(TKey key, TValue value)
	{
		AVLNode.Add(ref _root, key, value, _comparison);
		_count++;
	}

	public bool AddNonDuplicate(TKey key, TValue value)
	{
		if (AVLNode.AddNonDuplicate(ref _root, key, value, _comparison))
		{
			_count++;
			return true;
		}
		return false;
	}

	public void Bound(TKey key, out AVLNode lower, out AVLNode upper)
	{
		AVLNode.Bound(_root, key, _comparison, out lower, out upper);
	}

	public void Clear()
	{
		_root = null;
		_count = 0;
	}

	public AVLNode Get(TKey key)
	{
		return AVLNode.Get(_root, key, _comparison);
	}

	public IEnumerator<AVLNode> GetEnumerator()
	{
		return AVLNode.EnumerateRoot(_root).GetEnumerator();
	}

	public AVLNode GetNearestLeft(TKey key)
	{
		return AVLNode.GetNearestLeft(_root, key, _comparison);
	}

	public AVLNode GetNearestRight(TKey key)
	{
		return AVLNode.GetNearestRight(_root, key, _comparison);
	}

	public AVLNode GetOrAdd(TKey key, Func<TKey, TValue> factory)
	{
		AVLNode orAdd = AVLNode.GetOrAdd(ref _root, key, factory, _comparison, out var isNew);
		if (isNew)
		{
			_count++;
		}
		return orAdd;
	}

	public IEnumerable<AVLNode> Range(TKey lower, TKey upper)
	{
		foreach (AVLNode item in AVLNode.EnumerateFrom(_root, lower, _comparison))
		{
			Comparison<TKey> comparison = _comparison;
			if (comparison(item.Key, upper) <= 0)
			{
				yield return item;
				continue;
			}
			break;
		}
	}

	public bool Remove(TKey key)
	{
		if (AVLNode.Remove(ref _root, key, _comparison))
		{
			_count--;
			return true;
		}
		return false;
	}

	public AVLNode RemoveNearestLeft(TKey key)
	{
		return AVLNode.RemoveNearestLeft(ref _root, key, _comparison);
	}

	public AVLNode RemoveNearestRight(TKey key)
	{
		return AVLNode.RemoveNearestRight(ref _root, key, _comparison);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
