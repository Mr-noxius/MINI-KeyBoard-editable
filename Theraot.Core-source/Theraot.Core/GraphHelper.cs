using System;
using System.Collections.Generic;

namespace Theraot.Core;

public static class GraphHelper
{
	public static IEnumerable<TOutput> ExploreBreadthFirstGraph<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("next");
		}
		Queue<TInput> queue = new Queue<TInput>();
		queue.Enqueue(initial);
		return ExploreBreadthFirstGraphExtracted(queue, next, resultSelector);
	}

	public static IEnumerable<TOutput> ExploreBreadthFirstGraph<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreBreadthFirstGraphExtracted(new Queue<TInput>(initial), next, resultSelector);
	}

	public static IEnumerable<T> ExploreBreadthFirstGraph<T>(T initial, Func<T, IEnumerable<T>> next)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		Queue<T> queue = new Queue<T>();
		queue.Enqueue(initial);
		return ExploreBreadthFirstGraphExtracted(queue, next);
	}

	public static IEnumerable<T> ExploreBreadthFirstGraph<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreBreadthFirstGraphExtracted(new Queue<T>(initial), next);
	}

	public static IEnumerable<TOutput> ExploreBreadthFirstTree<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("next");
		}
		Queue<TInput> queue = new Queue<TInput>();
		queue.Enqueue(initial);
		return ExploreBreadthFirstTreeExtracted(queue, next, resultSelector);
	}

	public static IEnumerable<TOutput> ExploreBreadthFirstTree<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreBreadthFirstTreeExtracted(new Queue<TInput>(initial), next, resultSelector);
	}

	public static IEnumerable<T> ExploreBreadthFirstTree<T>(T initial, Func<T, IEnumerable<T>> next)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		Queue<T> queue = new Queue<T>();
		queue.Enqueue(initial);
		return ExploreBreadthFirstTreeExtracted(queue, next);
	}

	public static IEnumerable<T> ExploreBreadthFirstTree<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreBreadthFirstTreeExtracted(new Queue<T>(initial), next);
	}

	private static IEnumerable<TOutput> ExploreBreadthFirstGraphExtracted<TInput, TOutput>(Queue<TInput> queue, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		HashSet<TInput> known = new HashSet<TInput>();
		IEnumerator<TInput> branches = null;
		while (true)
		{
			if (branches == null)
			{
				if (queue.Count > 0)
				{
					TInput arg = queue.Dequeue();
					branches = next(arg).GetEnumerator();
					continue;
				}
				break;
			}
			bool advanced;
			try
			{
				advanced = branches.MoveNext();
			}
			catch
			{
				branches.Dispose();
				throw;
			}
			if (advanced)
			{
				TInput found = branches.Current;
				if (known.Add(found))
				{
					yield return resultSelector(found);
					queue.Enqueue(found);
				}
			}
			else
			{
				branches.Dispose();
				branches = null;
			}
		}
	}

	private static IEnumerable<T> ExploreBreadthFirstGraphExtracted<T>(Queue<T> queue, Func<T, IEnumerable<T>> next)
	{
		HashSet<T> known = new HashSet<T>();
		IEnumerator<T> branches = null;
		while (true)
		{
			if (branches == null)
			{
				if (queue.Count > 0)
				{
					T arg = queue.Dequeue();
					branches = next(arg).GetEnumerator();
					continue;
				}
				break;
			}
			bool advanced;
			try
			{
				advanced = branches.MoveNext();
			}
			catch
			{
				branches.Dispose();
				throw;
			}
			if (advanced)
			{
				T found = branches.Current;
				if (known.Add(found))
				{
					yield return found;
					queue.Enqueue(found);
				}
			}
			else
			{
				branches.Dispose();
				branches = null;
			}
		}
	}

	private static IEnumerable<TOutput> ExploreBreadthFirstTreeExtracted<TInput, TOutput>(Queue<TInput> queue, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		IEnumerator<TInput> branches = null;
		while (true)
		{
			if (branches == null)
			{
				if (queue.Count > 0)
				{
					TInput arg = queue.Dequeue();
					branches = next(arg).GetEnumerator();
					continue;
				}
				break;
			}
			bool advanced;
			try
			{
				advanced = branches.MoveNext();
			}
			catch
			{
				branches.Dispose();
				throw;
			}
			if (advanced)
			{
				TInput found = branches.Current;
				yield return resultSelector(found);
				queue.Enqueue(found);
			}
			else
			{
				branches.Dispose();
				branches = null;
			}
		}
	}

	private static IEnumerable<T> ExploreBreadthFirstTreeExtracted<T>(Queue<T> queue, Func<T, IEnumerable<T>> next)
	{
		IEnumerator<T> branches = null;
		while (true)
		{
			if (branches == null)
			{
				if (queue.Count > 0)
				{
					T arg = queue.Dequeue();
					branches = next(arg).GetEnumerator();
					continue;
				}
				break;
			}
			bool advanced;
			try
			{
				advanced = branches.MoveNext();
			}
			catch
			{
				branches.Dispose();
				throw;
			}
			if (advanced)
			{
				T found = branches.Current;
				yield return found;
				queue.Enqueue(found);
			}
			else
			{
				branches.Dispose();
				branches = null;
			}
		}
	}

	public static IEnumerable<TOutput> ExploreDepthFirstGraph<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("next");
		}
		Stack<TInput> stack = new Stack<TInput>();
		stack.Push(initial);
		return ExploreDepthFirstGraphExtracted(stack, next, resultSelector);
	}

	public static IEnumerable<TOutput> ExploreDepthFirstGraph<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreDepthFirstGraphExtracted(new Stack<TInput>(initial), next, resultSelector);
	}

	public static IEnumerable<T> ExploreDepthFirstGraph<T>(T initial, Func<T, IEnumerable<T>> next)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		Stack<T> stack = new Stack<T>();
		stack.Push(initial);
		return ExploreDepthFirstGraphExtracted(stack, next);
	}

	public static IEnumerable<T> ExploreDepthFirstGraph<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreDepthFirstGraphExtracted(new Stack<T>(initial), next);
	}

	public static IEnumerable<TOutput> ExploreDepthFirstTree<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("next");
		}
		Stack<TInput> stack = new Stack<TInput>();
		stack.Push(initial);
		return ExploreDepthFirstTreeExtracted(stack, next, resultSelector);
	}

	public static IEnumerable<TOutput> ExploreDepthFirstTree<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreDepthFirstTreeExtracted(new Stack<TInput>(initial), next, resultSelector);
	}

	public static IEnumerable<T> ExploreDepthFirstTree<T>(T initial, Func<T, IEnumerable<T>> next)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		Stack<T> stack = new Stack<T>();
		stack.Push(initial);
		return ExploreDepthFirstTreeExtracted(stack, next);
	}

	public static IEnumerable<T> ExploreDepthFirstTree<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreDepthFirstTreeExtracted(new Stack<T>(initial), next);
	}

	private static IEnumerable<TOutput> ExploreDepthFirstGraphExtracted<TInput, TOutput>(Stack<TInput> stack, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		HashSet<TInput> known = new HashSet<TInput>();
		IEnumerator<TInput> branches = null;
		while (true)
		{
			if (branches == null)
			{
				if (stack.Count > 0)
				{
					TInput found = stack.Pop();
					if (known.Add(found))
					{
						yield return resultSelector(found);
						branches = next(found).GetEnumerator();
					}
					continue;
				}
				break;
			}
			bool flag;
			try
			{
				flag = branches.MoveNext();
			}
			catch
			{
				branches.Dispose();
				throw;
			}
			if (flag)
			{
				TInput current = branches.Current;
				stack.Push(current);
			}
			else
			{
				branches.Dispose();
				branches = null;
			}
		}
	}

	private static IEnumerable<T> ExploreDepthFirstGraphExtracted<T>(Stack<T> stack, Func<T, IEnumerable<T>> next)
	{
		HashSet<T> known = new HashSet<T>();
		IEnumerator<T> branches = null;
		while (true)
		{
			if (branches == null)
			{
				if (stack.Count > 0)
				{
					T found = stack.Pop();
					if (known.Add(found))
					{
						yield return found;
						branches = next(found).GetEnumerator();
					}
					continue;
				}
				break;
			}
			bool flag;
			try
			{
				flag = branches.MoveNext();
			}
			catch
			{
				branches.Dispose();
				throw;
			}
			if (flag)
			{
				T current = branches.Current;
				stack.Push(current);
			}
			else
			{
				branches.Dispose();
				branches = null;
			}
		}
	}

	private static IEnumerable<TOutput> ExploreDepthFirstTreeExtracted<TInput, TOutput>(Stack<TInput> stack, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
	{
		IEnumerator<TInput> branches = null;
		while (true)
		{
			if (branches == null)
			{
				if (stack.Count > 0)
				{
					TInput found = stack.Pop();
					yield return resultSelector(found);
					branches = next(found).GetEnumerator();
					continue;
				}
				break;
			}
			bool flag;
			try
			{
				flag = branches.MoveNext();
			}
			catch
			{
				branches.Dispose();
				throw;
			}
			if (flag)
			{
				TInput current = branches.Current;
				stack.Push(current);
			}
			else
			{
				branches.Dispose();
				branches = null;
			}
		}
	}

	private static IEnumerable<T> ExploreDepthFirstTreeExtracted<T>(Stack<T> stack, Func<T, IEnumerable<T>> next)
	{
		IEnumerator<T> branches = null;
		while (true)
		{
			if (branches == null)
			{
				if (stack.Count > 0)
				{
					T found = stack.Pop();
					yield return found;
					branches = next(found).GetEnumerator();
					continue;
				}
				break;
			}
			bool flag;
			try
			{
				flag = branches.MoveNext();
			}
			catch
			{
				branches.Dispose();
				throw;
			}
			if (flag)
			{
				T current = branches.Current;
				stack.Push(current);
			}
			else
			{
				branches.Dispose();
				branches = null;
			}
		}
	}
}
