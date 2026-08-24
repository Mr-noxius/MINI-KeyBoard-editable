using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Collections;

[DebuggerNonUserCode]
public static class Extensions
{
	internal class PartitionEnumerable<T> : IEnumerable<IEnumerable<T>>, IEnumerable
	{
		private readonly IEnumerable<T> _source;

		private readonly int _partitionSize;

		public PartitionEnumerable(IEnumerable<T> source, int partitionSize)
		{
			_source = source;
			_partitionSize = partitionSize;
		}

		public IEnumerator<IEnumerable<T>> GetEnumerator()
		{
			List<T> group = new List<T>();
			int count = _partitionSize;
			foreach (T item in _source)
			{
				group.Add(item);
				count--;
				if (count == 0)
				{
					yield return group;
					group = new List<T>();
					count = _partitionSize;
				}
			}
			if (count < _partitionSize)
			{
				yield return group;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> items, int partitionSize)
	{
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		if (partitionSize < 1)
		{
			throw new ArgumentOutOfRangeException("partitionSize");
		}
		return new PartitionEnumerable<T>(items, partitionSize);
	}

	public static int AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		int num = 0;
		foreach (T item in items)
		{
			collection.Add(item);
			num++;
		}
		return num;
	}

	public static IEnumerable<T> AddRangeEnumerable<T>(this ICollection<T> collection, IEnumerable<T> items)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		foreach (T item in items)
		{
			collection.Add(item);
			yield return item;
		}
	}

	public static ICollection<T> AsCollection<T>(IEnumerable<T> source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (source is string && typeof(T) == typeof(char))
		{
			return (ICollection<T>)(object)(source as string).ToCharArray();
		}
		ICollection<T> collection = source as ICollection<T>;
		return collection ?? new ProgressiveCollection<T>(source);
	}

	public static ICollection<T> AsDistinctCollection<T>(IEnumerable<T> source)
	{
		return AsSet(source);
	}

	public static IList<T> AsList<T>(IEnumerable<T> source)
	{
		if (!(source is IList<T> result))
		{
			return new ProgressiveList<T>(source);
		}
		return result;
	}

	public static ISet<T> AsSet<T>(IEnumerable<T> source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		ISet<T> set = source as ISet<T>;
		return set ?? new ProgressiveSet<T>(source);
	}

	public static IEnumerable<T> AsUnaryEnumerable<T>(this T source)
	{
		yield return source;
	}

	public static IList<T> AsUnaryList<T>(this T source)
	{
		return new ProgressiveList<T>(source.AsUnaryEnumerable());
	}

	public static ISet<T> AsUnarySet<T>(this T source)
	{
		return new ProgressiveSet<T>(source.AsUnaryEnumerable());
	}

	public static bool HasAtLeast<TSource>(this IEnumerable<TSource> source, int count)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (count == 0)
		{
			return true;
		}
		if (source is string && typeof(TSource) == typeof(char))
		{
			return (source as string).Length >= count;
		}
		if (source is ICollection<TSource> collection)
		{
			return collection.Count >= count;
		}
		int num = 0;
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				num = checked(num + 1);
				if (num == count)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static IEnumerable<T> SkipItems<T>(this IEnumerable<T> source, int skipCount)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return SkipItemsExtracted(source, skipCount);
	}

	public static IEnumerable<T> SkipItems<T>(this IEnumerable<T> source, Predicate<T> predicateCount, int skipCount)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicateCount != null)
		{
			return SkipItemsExtracted(source, predicateCount, skipCount);
		}
		return SkipItemsExtracted(source, skipCount);
	}

	public static IEnumerable<T> StepItems<T>(this IEnumerable<T> source, int stepCount)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return source.StepItemsExtracted(stepCount);
	}

	public static IEnumerable<T> TakeItems<T>(this IEnumerable<T> source, int takeCount)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return TakeItemsExtracted(source, takeCount);
	}

	public static IEnumerable<T> TakeItems<T>(this IEnumerable<T> source, Predicate<T> predicateCount, int takeCount)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicateCount != null)
		{
			return TakeItemsExtracted(source, predicateCount, takeCount);
		}
		return TakeItemsExtracted(source, takeCount);
	}

	private static IEnumerable<T> SkipItemsExtracted<T>(IEnumerable<T> source, int skipCount)
	{
		int count = 0;
		foreach (T item in source)
		{
			if (count < skipCount)
			{
				count++;
			}
			else
			{
				yield return item;
			}
		}
	}

	private static IEnumerable<T> SkipItemsExtracted<T>(IEnumerable<T> source, Predicate<T> predicateCount, int skipCount)
	{
		int count = 0;
		foreach (T item in source)
		{
			if (count < skipCount)
			{
				if (predicateCount(item))
				{
					count++;
				}
			}
			else
			{
				yield return item;
			}
		}
	}

	private static IEnumerable<T> StepItemsExtracted<T>(this IEnumerable<T> source, int stepCount)
	{
		int count = 0;
		foreach (T item in source)
		{
			if (count % stepCount == 0)
			{
				count++;
				continue;
			}
			yield return item;
			count++;
		}
	}

	private static IEnumerable<T> TakeItemsExtracted<T>(IEnumerable<T> source, int takeCount)
	{
		int count = 0;
		foreach (T item in source)
		{
			if (count != takeCount)
			{
				yield return item;
				count++;
				continue;
			}
			break;
		}
	}

	private static IEnumerable<T> TakeItemsExtracted<T>(IEnumerable<T> source, Predicate<T> predicateCount, int takeCount)
	{
		int count = 0;
		foreach (T item in source)
		{
			if (count != takeCount)
			{
				yield return item;
				if (predicateCount(item))
				{
					count++;
				}
				continue;
			}
			break;
		}
	}

	public static void Add<T>(this Stack<T> stack, T item)
	{
		if (stack == null)
		{
			throw new ArgumentNullException("stack");
		}
		stack.Push(item);
	}

	public static void Add<T>(this Queue<T> queue, T item)
	{
		if (queue == null)
		{
			throw new ArgumentNullException("queue");
		}
		queue.Enqueue(item);
	}

	public static T[] AddFirst<T>(this IList<T> list, T item)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		T[] array = new T[list.Count + 1];
		array[0] = item;
		list.CopyTo(array, 1);
		return array;
	}

	public static void CanCopyTo(int count, Array array)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (count > array.Length)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
	}

	public static void CanCopyTo(int count, Array array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (count > array.Length - arrayIndex)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
	}

	public static void CanCopyTo<T>(int count, T[] array)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (count > array.Length)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
	}

	public static void CanCopyTo<T>(int count, T[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (count > array.Length - arrayIndex)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
	}

	public static void CanCopyTo<T>(T[] array, int arrayIndex, int countLimit)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (countLimit < 0)
		{
			throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
		}
		if (countLimit > array.Length - arrayIndex)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
	}

	public static IEnumerable<T> Clone<T>(this IEnumerable<T> target)
	{
		return new List<T>(target);
	}

	public static void Consume<T>(this IEnumerable<T> source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		foreach (T item in source)
		{
			GC.KeepAlive(item);
		}
	}

	public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> items)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
		ICollection<T> source2 = AsCollection(source);
		foreach (T item in items)
		{
			if (source2.Contains(item, comparer))
			{
				return true;
			}
		}
		return false;
	}

	public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> items, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		comparer = comparer ?? EqualityComparer<T>.Default;
		ICollection<T> source2 = AsCollection(source);
		foreach (T item in items)
		{
			if (source2.Contains(item, comparer))
			{
				return true;
			}
		}
		return false;
	}

	public static List<TOutput> ConvertAll<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		List<TOutput> list = new List<TOutput>();
		foreach (T item in source)
		{
			list.Add(converter(item));
		}
		return list;
	}

	public static TList ConvertAll<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter) where TList : ICollection<TOutput>, new()
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		TList result = new TList();
		foreach (T item in source)
		{
			result.Add(converter(item));
		}
		return result;
	}

	public static List<TOutput> ConvertFiltered<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		List<TOutput> list = new List<TOutput>();
		foreach (T item in source)
		{
			if (filter(item))
			{
				list.Add(converter(item));
			}
		}
		return list;
	}

	public static List<TOutput> ConvertFiltered<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		int num = 0;
		List<TOutput> list = new List<TOutput>();
		foreach (T item in source)
		{
			if (filter(item, num))
			{
				list.Add(converter(item));
			}
			num++;
		}
		return list;
	}

	public static TList ConvertFiltered<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter) where TList : ICollection<TOutput>, new()
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		TList result = new TList();
		foreach (T item in source)
		{
			if (filter(item))
			{
				result.Add(converter(item));
			}
		}
		return result;
	}

	public static TList ConvertFiltered<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter) where TList : ICollection<TOutput>, new()
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		int num = 0;
		TList result = new TList();
		foreach (T item in source)
		{
			if (filter(item, num))
			{
				result.Add(converter(item));
			}
			num++;
		}
		return result;
	}

	public static List<KeyValuePair<int, TOutput>> ConvertIndexed<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		int num = 0;
		List<KeyValuePair<int, TOutput>> list = new List<KeyValuePair<int, TOutput>>();
		foreach (T item in source)
		{
			if (filter(item))
			{
				list.Add(new KeyValuePair<int, TOutput>(num, converter(item)));
			}
			num++;
		}
		return list;
	}

	public static List<KeyValuePair<int, TOutput>> ConvertIndexed<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		int num = 0;
		List<KeyValuePair<int, TOutput>> list = new List<KeyValuePair<int, TOutput>>();
		foreach (T item in source)
		{
			if (filter(item, num))
			{
				list.Add(new KeyValuePair<int, TOutput>(num, converter(item)));
			}
			num++;
		}
		return list;
	}

	public static TList ConvertIndexed<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter) where TList : ICollection<KeyValuePair<int, TOutput>>, new()
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		int num = 0;
		TList result = new TList();
		foreach (T item in source)
		{
			if (filter(item))
			{
				result.Add(new KeyValuePair<int, TOutput>(num, converter(item)));
			}
			num++;
		}
		return result;
	}

	public static TList ConvertIndexed<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter) where TList : ICollection<KeyValuePair<int, TOutput>>, new()
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		int num = 0;
		TList result = new TList();
		foreach (T item in source)
		{
			if (filter(item, num))
			{
				result.Add(new KeyValuePair<int, TOutput>(num, converter(item)));
			}
			num++;
		}
		return result;
	}

	public static IEnumerable<TOutput> ConvertProgressive<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		foreach (T item in source)
		{
			yield return converter(item);
		}
	}

	public static IEnumerable<TOutput> ConvertProgressiveFiltered<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		foreach (T item in source)
		{
			if (filter(item))
			{
				yield return converter(item);
			}
		}
	}

	public static IEnumerable<TOutput> ConvertProgressiveFiltered<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		int index = 0;
		foreach (T item in source)
		{
			if (filter(item, index))
			{
				yield return converter(item);
			}
			index++;
		}
	}

	public static IEnumerable<KeyValuePair<int, TOutput>> ConvertProgressiveIndexed<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		int index = 0;
		foreach (T item in source)
		{
			if (filter(item))
			{
				yield return new KeyValuePair<int, TOutput>(index, converter(item));
			}
			index++;
		}
	}

	public static IEnumerable<KeyValuePair<int, TOutput>> ConvertProgressiveIndexed<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Func<T, int, bool> filter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		int index = 0;
		foreach (T item in source)
		{
			if (filter(item, index))
			{
				yield return new KeyValuePair<int, TOutput>(index, converter(item));
			}
			index++;
		}
	}

	public static T[] Copy<T>(this T[] array)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		T[] array2 = new T[array.Length];
		Array.Copy(array, array2, array.Length);
		return array2;
	}

	public static void CopyTo<T>(this IEnumerable<T> source, T[] array)
	{
		source.CopyTo(array, 0);
	}

	public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		try
		{
			int num = arrayIndex;
			foreach (T item in source)
			{
				array[num] = item;
				num++;
			}
		}
		catch (IndexOutOfRangeException ex)
		{
			throw new ArgumentException(ex.Message, "array");
		}
	}

	public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex, int countLimit)
	{
		source.TakeItems(countLimit).CopyTo(array, arrayIndex);
	}

	public static int CountContiguousItems<T>(this IEnumerable<T> source, T item)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		foreach (T item2 in source)
		{
			if (equalityComparer.Equals(item2, item))
			{
				num++;
				continue;
			}
			break;
		}
		return num;
	}

	public static int CountContiguousItemsWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		foreach (T item in source)
		{
			if (predicate(item))
			{
				num++;
				continue;
			}
			break;
		}
		return num;
	}

	public static int CountItems<T>(this IEnumerable<T> source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		foreach (T item in source)
		{
			num++;
			GC.KeepAlive(item);
		}
		return num;
	}

	public static int CountItems<T>(this IEnumerable<T> source, T item)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		foreach (T item2 in source)
		{
			if (equalityComparer.Equals(item2, item))
			{
				num++;
			}
		}
		return num;
	}

	public static int CountItemsWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		foreach (T item in source)
		{
			if (predicate(item))
			{
				num++;
			}
		}
		return num;
	}

	public static void DeprecatedCopyTo<T>(this IEnumerable<T> source, Array array)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		int num = 0;
		foreach (T item in source)
		{
			array.SetValue(item, num++);
		}
	}

	public static void DeprecatedCopyTo<T>(this IEnumerable<T> source, Array array, int index)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		foreach (T item in source)
		{
			array.SetValue(item, index++);
		}
	}

	public static IEnumerable<T> EmptyChecked<T>(this IEnumerable<T> source, Action onEmpty)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (onEmpty == null)
		{
			throw new ArgumentException("onEmpty");
		}
		if (source is ICollection<T> { Count: 0 })
		{
			onEmpty();
			return ArrayReservoir<T>.EmptyArray;
		}
		return NullOrEmptyCheckedExtracted(source, onEmpty);
	}

	public static IEnumerable<T> EmptyChecked<T>(this IEnumerable<T> source, Action onEmpty, Action onNotEmpty)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (onEmpty == null)
		{
			throw new ArgumentException("onEmpty");
		}
		if (source is ICollection<T> collection)
		{
			if (collection.Count == 0)
			{
				onEmpty();
				return ArrayReservoir<T>.EmptyArray;
			}
			onNotEmpty();
		}
		return NullOrEmptyCheckedExtracted(source, onEmpty, onNotEmpty);
	}

	public static IEnumerable<T> EmptyChecked<T>(this IEnumerable<T> source, Action onEmpty, Action onUnknownSize, Action<int> onKnownSize)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (onEmpty == null)
		{
			throw new ArgumentException("onEmpty");
		}
		if (source is ICollection<T> collection)
		{
			if (collection.Count == 0)
			{
				onEmpty();
				return ArrayReservoir<T>.EmptyArray;
			}
			onKnownSize(collection.Count);
		}
		return NullOrEmptyCheckedExtracted(source, onEmpty, onUnknownSize);
	}

	public static int ExceptWith<T>(this ICollection<T> source, IEnumerable<T> other)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		int num = 0;
		foreach (T item in other)
		{
			while (source.Remove(item))
			{
				num++;
			}
		}
		return num;
	}

	public static IEnumerable<T> ExceptWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		foreach (T item in other)
		{
			while (source.Remove(item))
			{
				yield return item;
			}
		}
	}

	public static bool Exists<T>(this IEnumerable<T> source, T value)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		IEqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		foreach (T item in source)
		{
			if (equalityComparer.Equals(item, value))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Exists<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		comparer = comparer ?? EqualityComparer<T>.Default;
		foreach (T item in source)
		{
			if (comparer.Equals(item, value))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Exists<T>(this IEnumerable<T> source, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		foreach (T item in source)
		{
			if (predicate(item))
			{
				return true;
			}
		}
		return false;
	}

	public static T Find<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		int num2 = index + count;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext() && num != num2)
		{
			if (predicate(enumerator.Current))
			{
				return enumerator.Current;
			}
			num++;
		}
		return default(T);
	}

	public static T Find<T>(this IEnumerable<T> source, int index, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext())
		{
			if (predicate(enumerator.Current))
			{
				return enumerator.Current;
			}
			num++;
		}
		return default(T);
	}

	public static T Find<T>(this IEnumerable<T> source, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (predicate(enumerator.Current))
			{
				return enumerator.Current;
			}
		}
		return default(T);
	}

	public static int FindIndex<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		int num2 = index + count;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext() && num != num2)
		{
			if (predicate(enumerator.Current))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		foreach (T item in source)
		{
			if (predicate(item))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static int FindIndex<T>(this IEnumerable<T> source, int index, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext())
		{
			if (predicate(enumerator.Current))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static T FindLast<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		int num2 = index + count;
		T result = default(T);
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext() && num != num2)
		{
			if (predicate(enumerator.Current))
			{
				result = enumerator.Current;
			}
			num++;
		}
		return result;
	}

	public static T FindLast<T>(this IEnumerable<T> source, int index, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		T result = default(T);
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext())
		{
			if (predicate(enumerator.Current))
			{
				result = enumerator.Current;
			}
			num++;
		}
		return result;
	}

	public static T FindLast<T>(this IEnumerable<T> source, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		T result = default(T);
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (predicate(enumerator.Current))
			{
				result = enumerator.Current;
			}
		}
		return result;
	}

	public static int FindLastIndex<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		int num2 = index + count;
		int result = -1;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext() && num != num2)
		{
			if (predicate(enumerator.Current))
			{
				result = num;
			}
			num++;
		}
		return result;
	}

	public static int FindLastIndex<T>(this IEnumerable<T> source, int index, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		int result = -1;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext())
		{
			if (predicate(enumerator.Current))
			{
				result = num;
			}
			num++;
		}
		return result;
	}

	public static int FindLastIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		int result = -1;
		foreach (T item in source)
		{
			if (predicate(item))
			{
				result = num;
			}
			num++;
		}
		return result;
	}

	public static List<T> FindWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		List<T> list = new List<T>();
		foreach (T item in source)
		{
			if (predicate(item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static TList FindWhere<T, TList>(this IEnumerable<T> source, Predicate<T> predicate) where TList : ICollection<T>, new()
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		TList result = new TList();
		foreach (T item in source)
		{
			if (predicate(item))
			{
				result.Add(item);
			}
		}
		return result;
	}

	public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return FlattenExtracted(source);
	}

	public static void For<T>(this IEnumerable<T> source, Action<int, T> action)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		int num = 0;
		foreach (T item in source)
		{
			action(num, item);
			num++;
		}
	}

	public static void For<T>(this IEnumerable<T> source, Action<int, T> action, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		int num = 0;
		foreach (T item in source)
		{
			if (predicate(item))
			{
				action(num, item);
			}
			num++;
		}
	}

	public static void For<T>(this IEnumerable<T> source, Action<int, T> action, Func<T, int, bool> filter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		int num = 0;
		foreach (T item in source)
		{
			if (filter(item, num))
			{
				action(num, item);
			}
			num++;
		}
	}

	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		foreach (T item in source)
		{
			action(item);
		}
	}

	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action, Predicate<T> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		foreach (T item in source)
		{
			if (predicate(item))
			{
				action(item);
			}
		}
	}

	public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		if (dictionary.TryGetValue(key, out var value))
		{
			return value;
		}
		TValue val = TypeHelper.CreateOrDefault<TValue>();
		dictionary.Add(key, val);
		return val;
	}

	public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue newValue)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		if (dictionary.TryGetValue(key, out var value))
		{
			return value;
		}
		dictionary.Add(key, newValue);
		return newValue;
	}

	public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> create)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		if (dictionary.TryGetValue(key, out var value))
		{
			return value;
		}
		TValue val = ((create == null) ? default(TValue) : create());
		dictionary.Add(key, val);
		return val;
	}

	public static int IndexOf<T>(this IEnumerable<T> source, T item, int index, int count)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		int num2 = index + count;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext() && num != num2)
		{
			if (equalityComparer.Equals(enumerator.Current, item))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static int IndexOf<T>(this IEnumerable<T> source, T item, int index, int count, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		int num2 = index + count;
		comparer = comparer ?? EqualityComparer<T>.Default;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext() && num != num2)
		{
			if (comparer.Equals(enumerator.Current, item))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static int IndexOf<T>(this IEnumerable<T> source, T item, int index)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext())
		{
			if (equalityComparer.Equals(enumerator.Current, item))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static int IndexOf<T>(this IEnumerable<T> source, T item, int index, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		comparer = comparer ?? EqualityComparer<T>.Default;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext())
		{
			if (comparer.Equals(enumerator.Current, item))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static int IndexOf<T>(this IEnumerable<T> source, T item)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		foreach (T item2 in source)
		{
			if (equalityComparer.Equals(item2, item))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static int IndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		comparer = comparer ?? EqualityComparer<T>.Default;
		foreach (T item2 in source)
		{
			if (comparer.Equals(item2, item))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static IEnumerable<T> InterleaveMany<T>(this IEnumerable<IEnumerable<T>> source)
	{
		// ILSpy could not decompile this. Please report the exception below,
		// along with the assembly it came from, at https://github.com/icsharpcode/ILSpy/issues/new
		// System.IndexOutOfRangeException: Index was outside the bounds of the array.
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VariableScope.TryGetExistingName(ILFunction function, Int32 index) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 273
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 571
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitStLoc(StLoc inst, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 8010
		//    at ICSharpCode.Decompiler.IL.StLoc.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 2571
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VisitChildren(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 536
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 590
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitBlock(Block block, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 7894
		//    at ICSharpCode.Decompiler.IL.Block.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 883
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VisitChildren(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 536
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 590
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitBlockContainer(BlockContainer container, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 7890
		//    at ICSharpCode.Decompiler.IL.BlockContainer.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 858
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VisitChildren(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 536
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 590
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitTryFinally(TryFinally inst, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 7958
		//    at ICSharpCode.Decompiler.IL.TryFinally.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 1805
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VisitChildren(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 536
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 590
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitBlock(Block block, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 7894
		//    at ICSharpCode.Decompiler.IL.Block.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 883
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VisitChildren(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 536
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 590
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitBlockContainer(BlockContainer container, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 7890
		//    at ICSharpCode.Decompiler.IL.BlockContainer.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 858
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Run(ILFunction function, ILTransformContext context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 479
		//    at ICSharpCode.Decompiler.CSharp.CSharpDecompiler.DecompileBody(IMethod method, EntityDeclaration entityDecl, DecompileRun decompileRun, ITypeResolveContext decompilationContext, ExtensionInfo extensionInfo) in /_/ICSharpCode.Decompiler/CSharp/CSharpDecompiler.cs:line 2303
	}

	public static int IntersectWith<T>(this ICollection<T> source, IEnumerable<T> other)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		ICollection<T> otherAsCollection = AsCollection(other);
		return source.RemoveWhere((T input) => !otherAsCollection.Contains(input));
	}

	public static int IntersectWith<T>(this ICollection<T> source, IEnumerable<T> other, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		comparer = comparer ?? EqualityComparer<T>.Default;
		ICollection<T> otherASCollection = AsCollection(other);
		return source.RemoveWhere((T input) => !otherASCollection.Contains(input, comparer));
	}

	public static IEnumerable<T> IntersectWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		ICollection<T> otherAsCollection = AsCollection(other);
		return source.RemoveWhereEnumerable((T input) => !otherAsCollection.Contains(input));
	}

	public static IEnumerable<T> IntersectWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		comparer = comparer ?? EqualityComparer<T>.Default;
		ICollection<T> otherAsCollection = AsCollection(other);
		return source.RemoveWhereEnumerable((T input) => !otherAsCollection.Contains(input, comparer));
	}

	public static bool IsEmpty<T>(this IEnumerable<T> source)
	{
		return !source.Any();
	}

	public static bool IsProperSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		return source.IsSubsetOf(other, proper: true);
	}

	public static bool IsProperSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		return source.IsSupersetOf(other, proper: true);
	}

	public static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		return source.IsSubsetOf(other, proper: false);
	}

	public static bool IsSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		return source.IsSupersetOf(other, proper: false);
	}

	public static int LastIndexOf<T>(this IEnumerable<T> source, T item, int index, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		comparer = comparer ?? EqualityComparer<T>.Default;
		int result = -1;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext())
		{
			if (comparer.Equals(enumerator.Current, item))
			{
				result = num;
			}
			num++;
		}
		return result;
	}

	public static int LastIndexOf<T>(this IEnumerable<T> source, T item, int index)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		int result = -1;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext())
		{
			if (equalityComparer.Equals(enumerator.Current, item))
			{
				result = num;
			}
			num++;
		}
		return result;
	}

	public static int LastIndexOf<T>(this IEnumerable<T> source, T item)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		int result = -1;
		foreach (T item2 in source)
		{
			if (equalityComparer.Equals(item2, item))
			{
				result = num;
			}
			num++;
		}
		return result;
	}

	public static int LastIndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		comparer = comparer ?? EqualityComparer<T>.Default;
		int result = -1;
		foreach (T item2 in source)
		{
			if (comparer.Equals(item2, item))
			{
				result = num;
			}
			num++;
		}
		return result;
	}

	public static int LastIndexOf<T>(this IEnumerable<T> source, T item, int index, int count, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		int num2 = index + count;
		comparer = comparer ?? EqualityComparer<T>.Default;
		int result = -1;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext() && num != num2)
		{
			if (comparer.Equals(enumerator.Current, item))
			{
				result = num;
			}
			num++;
		}
		return result;
	}

	public static int LastIndexOf<T>(this IEnumerable<T> source, T item, int index, int count)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		int num2 = index + count;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		int result = -1;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext() && num != num2)
		{
			if (equalityComparer.Equals(enumerator.Current, item))
			{
				result = num;
			}
			num++;
		}
		return result;
	}

	public static bool ListEquals<T>(this ICollection<T> first, ICollection<T> second)
	{
		if (first == null)
		{
			throw new ArgumentNullException("first");
		}
		if (second == null)
		{
			throw new ArgumentNullException("second");
		}
		if (first.Count != second.Count)
		{
			return false;
		}
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		IEnumerator<T> enumerator = first.GetEnumerator();
		IEnumerator<T> enumerator2 = second.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator2.MoveNext();
				if (!equalityComparer.Equals(enumerator.Current, enumerator2.Current))
				{
					return false;
				}
			}
			return true;
		}
		finally
		{
			enumerator.Dispose();
			enumerator2.Dispose();
		}
	}

	public static TOutput[] Map<TInput, TOutput>(this ICollection<TInput> source, Func<TInput, TOutput> select)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int count = source.Count;
		TOutput[] array = new TOutput[count];
		count = 0;
		foreach (TInput item in source)
		{
			array[count++] = select(item);
		}
		return array;
	}

	public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		T item = list[oldIndex];
		list.RemoveAt(oldIndex);
		if (newIndex > oldIndex)
		{
			newIndex--;
		}
		list.Insert(newIndex, item);
	}

	public static IEnumerable<T> NullOrEmptyChecked<T>(this IEnumerable<T> source, Action onEmpty)
	{
		if (onEmpty == null)
		{
			throw new ArgumentException("onEmpty");
		}
		if (source == null)
		{
			onEmpty();
			return ArrayReservoir<T>.EmptyArray;
		}
		if (source is ICollection<T> { Count: 0 })
		{
			onEmpty();
			return ArrayReservoir<T>.EmptyArray;
		}
		return NullOrEmptyCheckedExtracted(source, onEmpty);
	}

	public static IEnumerable<T> NullOrEmptyChecked<T>(this IEnumerable<T> source, Action onEmpty, Action onNotEmpty)
	{
		if (onEmpty == null)
		{
			throw new ArgumentException("onEmpty");
		}
		if (source == null)
		{
			onEmpty();
			return ArrayReservoir<T>.EmptyArray;
		}
		if (source is ICollection<T> collection)
		{
			if (collection.Count == 0)
			{
				onEmpty();
				return ArrayReservoir<T>.EmptyArray;
			}
			onNotEmpty();
		}
		return NullOrEmptyCheckedExtracted(source, onEmpty, onNotEmpty);
	}

	public static IEnumerable<T> NullOrEmptyChecked<T>(this IEnumerable<T> source, Action onEmpty, Action onUnknownSize, Action<int> onKnownSize)
	{
		if (onEmpty == null)
		{
			throw new ArgumentException("onEmpty");
		}
		if (source == null)
		{
			onEmpty();
			return ArrayReservoir<T>.EmptyArray;
		}
		if (source is ICollection<T> collection)
		{
			if (collection.Count == 0)
			{
				onEmpty();
				return ArrayReservoir<T>.EmptyArray;
			}
			onKnownSize(collection.Count);
		}
		return NullOrEmptyCheckedExtracted(source, onEmpty, onUnknownSize);
	}

	public static bool Overlaps<T>(this IEnumerable<T> source, IEnumerable<T> items)
	{
		return source.ContainsAny(items);
	}

	public static IEnumerable<TPackage> Pack<T, TPackage>(this IEnumerable<T> source, int size) where TPackage : ICollection<T>, new()
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int count = 0;
		TPackage currentPackage = new TPackage();
		foreach (T item in source)
		{
			currentPackage.Add(item);
			count++;
			if (count == size)
			{
				yield return currentPackage;
				currentPackage = new TPackage();
				count = 0;
			}
		}
		if (count > 0)
		{
			yield return currentPackage;
		}
	}

	public static IEnumerable<T[]> Pack<T>(this IEnumerable<T> source, int size)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int index = 0;
		T[] currentPackage = new T[size];
		foreach (T item in source)
		{
			currentPackage[index] = item;
			index++;
			if (index == size)
			{
				yield return currentPackage;
				currentPackage = new T[size];
				index = 0;
			}
		}
		if (index > 0)
		{
			Array.Resize(ref currentPackage, index);
			yield return currentPackage;
		}
	}

	public static bool Remove<T>(this ICollection<T> source, T item, IEqualityComparer<T> comparer)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		comparer = comparer ?? EqualityComparer<T>.Default;
		using IEnumerator<T> enumerator = source.RemoveWhereEnumerable((T input) => comparer.Equals(input, item)).GetEnumerator();
		return enumerator.MoveNext();
	}

	public static T[] RemoveFirst<T>(this T[] array)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		T[] array2 = new T[array.Length - 1];
		Array.Copy(array, 1, array2, 0, array2.Length);
		return array2;
	}

	public static T[] RemoveLast<T>(this T[] array)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		T[] array2 = new T[array.Length - 1];
		Array.Copy(array, 0, array2, 0, array2.Length);
		return array2;
	}

	public static int RemoveWhere<T>(this ICollection<T> source, Predicate<T> predicate)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		return source.RemoveWhere((IEnumerable<T> items) => Where(items, predicate));
	}

	public static int RemoveWhere<T>(this ICollection<T> source, Func<IEnumerable<T>, IEnumerable<T>> converter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		return source.ExceptWith(new List<T>(converter(source)));
	}

	public static IEnumerable<T> RemoveWhereEnumerable<T>(this ICollection<T> source, Predicate<T> predicate)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		return source.RemoveWhereEnumerable((IEnumerable<T> items) => Where(items, predicate));
	}

	public static IEnumerable<T> RemoveWhereEnumerable<T>(this ICollection<T> source, Func<IEnumerable<T>, IEnumerable<T>> converter)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		return source.ExceptWithEnumerable(new List<T>(converter(source)));
	}

	public static void Reverse<T>(this IList<T> list, int index, int count)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", "Non-negative number is required.");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Non-negative number is required.");
		}
		int count2 = list.Count;
		if (count > count2 - index)
		{
			throw new ArgumentException("The list does not contain the number of elements.", "list");
		}
		for (int i = index + count; index < i; i++)
		{
			SwapExtracted(list, index, i);
			index++;
		}
	}

	public static bool SetEquals<T>(this ICollection<T> source, IEnumerable<T> other)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		ICollection<T> thatAsCollection = AsCollection(other);
		using (IEnumerator<T> enumerator = thatAsCollection.Where((T input) => !source.Contains(input)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				GC.KeepAlive(current);
				return false;
			}
		}
		using (IEnumerator<T> enumerator2 = source.Where((T input) => !thatAsCollection.Contains(input)).GetEnumerator())
		{
			if (enumerator2.MoveNext())
			{
				T current2 = enumerator2.Current;
				GC.KeepAlive(current2);
				return false;
			}
		}
		return true;
	}

	public static void Sort<T>(this IList<T> list, int index, int count, IComparer<T> comparer)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		comparer = comparer ?? Comparer<T>.Default;
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", "Non-negative number is required.");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Non-negative number is required.");
		}
		int count2 = list.Count;
		if (count > count2 - index)
		{
			throw new ArgumentException("The list does not contain the number of elements.", "list");
		}
		SortExtracted(list, index, count + index, comparer);
	}

	public static void Swap<T>(this IList<T> list, int indexA, int indexB)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (indexA < 0)
		{
			throw new ArgumentOutOfRangeException("indexA", "Non-negative number is required.");
		}
		if (indexB < 0)
		{
			throw new ArgumentOutOfRangeException("indexB", "Non-negative number is required.");
		}
		int count = list.Count;
		if (indexA >= count || indexB >= count)
		{
			throw new ArgumentException("The list does not contain the number of elements.", "list");
		}
		if (indexA != indexB)
		{
			SwapExtracted(list, indexA, indexB);
		}
	}

	public static int SymmetricExceptWith<T>(this ICollection<T> source, IEnumerable<T> other)
	{
		return source.AddRange(Where(other.Distinct(), (T input) => !source.Remove(input)));
	}

	public static IEnumerable<T> SymmetricExceptWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other)
	{
		return source.AddRangeEnumerable(Where(other.Distinct(), (T input) => !source.Remove(input)));
	}

	public static T[] ToArray<T>(this ICollection<T> source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return new List<T>(source).ToArray();
	}

	public static ReadOnlyCollection<TSource> ToReadOnly<TSource>(this IEnumerable<TSource> source)
	{
		if (source == null)
		{
			return new ReadOnlyCollection<TSource>(ArrayReservoir<TSource>.EmptyArray);
		}
		if (source is ReadOnlyCollection<TSource> result)
		{
			return result;
		}
		return new ReadOnlyCollection<TSource>(source.ToArray());
	}

	public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		try
		{
			dictionary.Add(key, value);
			return true;
		}
		catch (ArgumentException obj)
		{
			GC.KeepAlive(obj);
			return false;
		}
	}

	public static bool TryFind<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate, out T founT)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		int num2 = index + count;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext() && num != num2)
		{
			if (predicate(enumerator.Current))
			{
				founT = enumerator.Current;
				return true;
			}
			num++;
		}
		founT = default(T);
		return false;
	}

	public static bool TryFind<T>(this IEnumerable<T> source, int index, Predicate<T> predicate, out T founT)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext())
		{
			if (predicate(enumerator.Current))
			{
				founT = enumerator.Current;
				return true;
			}
			num++;
		}
		founT = default(T);
		return false;
	}

	public static bool TryFind<T>(this IEnumerable<T> source, Predicate<T> predicate, out T founT)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (predicate(enumerator.Current))
			{
				founT = enumerator.Current;
				return true;
			}
		}
		founT = default(T);
		return false;
	}

	public static bool TryFindLast<T>(this IEnumerable<T> source, int index, int count, Predicate<T> predicate, out T foundItem)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		int num2 = index + count;
		foundItem = default(T);
		bool result = false;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext() && num != num2)
		{
			if (predicate(enumerator.Current))
			{
				foundItem = enumerator.Current;
				result = true;
			}
			num++;
		}
		return result;
	}

	public static bool TryFindLast<T>(this IEnumerable<T> source, int index, Predicate<T> predicate, out T foundItem)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = 0;
		foundItem = default(T);
		bool result = false;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
			if (num == index)
			{
				break;
			}
		}
		while (enumerator.MoveNext())
		{
			if (predicate(enumerator.Current))
			{
				foundItem = enumerator.Current;
				result = true;
			}
			num++;
		}
		return result;
	}

	public static bool TryFindLast<T>(this IEnumerable<T> source, Predicate<T> predicate, out T foundItem)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		foundItem = default(T);
		bool result = false;
		using IEnumerator<T> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (predicate(enumerator.Current))
			{
				foundItem = enumerator.Current;
				result = true;
			}
		}
		return result;
	}

	public static bool TryTake<T>(this Stack<T> stack, out T item)
	{
		if (stack == null)
		{
			throw new ArgumentNullException("stack");
		}
		try
		{
			item = stack.Pop();
			return true;
		}
		catch (InvalidOperationException)
		{
			item = default(T);
			return false;
		}
	}

	public static bool TryTake<T>(this Queue<T> queue, out T item)
	{
		if (queue == null)
		{
			throw new ArgumentNullException("queue");
		}
		try
		{
			item = queue.Dequeue();
			return true;
		}
		catch (InvalidOperationException)
		{
			item = default(T);
			return false;
		}
	}

	public static int UnionWith<T>(this ICollection<T> source, IEnumerable<T> other)
	{
		return source.AddRange(other.Where((T input) => !source.Contains(input)));
	}

	public static IEnumerable<T> UnionWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other)
	{
		return source.AddRangeEnumerable(other.Where((T input) => !source.Contains(input)));
	}

	public static IEnumerable<T> Where<T>(IEnumerable<T> source, Predicate<T> predicate)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return WhereExtracted(source, predicate);
	}

	public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return WhereExtracted(source, predicate);
	}

	public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate, Action whereNot)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (whereNot == null)
		{
			return WhereExtracted(source, predicate);
		}
		return WhereExtracted(source, predicate, whereNot);
	}

	public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate, Action<T> whereNot)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (whereNot == null)
		{
			return WhereExtracted(source, predicate);
		}
		return WhereExtracted(source, predicate, whereNot);
	}

	public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, int, bool> predicate, Action whereNot)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (whereNot == null)
		{
			return WhereExtracted(source, predicate);
		}
		return WhereExtracted(source, predicate, whereNot);
	}

	public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, int, bool> predicate, Action<T> whereNot)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (whereNot == null)
		{
			return WhereExtracted(source, predicate);
		}
		return WhereExtracted(source, predicate, whereNot);
	}

	public static IEnumerable<T> WhereType<T>(IEnumerable enumerable)
	{
		return new EnumerableFromDelegate<T>(enumerable.GetEnumerator);
	}

	public static IEnumerable<TResult> ZipMany<T, TResult>(this IEnumerable<IEnumerable<T>> source, Func<IEnumerable<T>, TResult> func)
	{
		// ILSpy could not decompile this. Please report the exception below,
		// along with the assembly it came from, at https://github.com/icsharpcode/ILSpy/issues/new
		// System.IndexOutOfRangeException: Index was outside the bounds of the array.
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VariableScope.TryGetExistingName(ILFunction function, Int32 index) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 273
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 571
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitStLoc(StLoc inst, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 8010
		//    at ICSharpCode.Decompiler.IL.StLoc.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 2571
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VisitChildren(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 536
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 590
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitBlock(Block block, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 7894
		//    at ICSharpCode.Decompiler.IL.Block.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 883
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VisitChildren(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 536
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 590
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitBlockContainer(BlockContainer container, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 7890
		//    at ICSharpCode.Decompiler.IL.BlockContainer.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 858
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VisitChildren(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 536
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 590
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitTryFinally(TryFinally inst, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 7958
		//    at ICSharpCode.Decompiler.IL.TryFinally.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 1805
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VisitChildren(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 536
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 590
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitBlock(Block block, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 7894
		//    at ICSharpCode.Decompiler.IL.Block.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 883
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.VisitChildren(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 536
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Default(ILInstruction inst, VariableScope context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 590
		//    at ICSharpCode.Decompiler.IL.ILVisitor`2.VisitBlockContainer(BlockContainer container, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 7890
		//    at ICSharpCode.Decompiler.IL.BlockContainer.AcceptVisitor[C,T](ILVisitor`2 visitor, C context) in /_/ICSharpCode.Decompiler/IL/Instructions.cs:line 858
		//    at ICSharpCode.Decompiler.IL.Transforms.AssignVariableNames.Run(ILFunction function, ILTransformContext context) in /_/ICSharpCode.Decompiler/IL/Transforms/AssignVariableNames.cs:line 479
		//    at ICSharpCode.Decompiler.CSharp.CSharpDecompiler.DecompileBody(IMethod method, EntityDeclaration entityDecl, DecompileRun decompileRun, ITypeResolveContext decompilationContext, ExtensionInfo extensionInfo) in /_/ICSharpCode.Decompiler/CSharp/CSharpDecompiler.cs:line 2303
	}

	private static IEnumerable<T> FlattenExtracted<T>(IEnumerable<IEnumerable<T>> source)
	{
		foreach (IEnumerable<T> key in source)
		{
			foreach (T item in key)
			{
				yield return item;
			}
		}
	}

	private static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other, bool proper)
	{
		ICollection<T> collection = AsDistinctCollection(source);
		ICollection<T> collection2 = AsDistinctCollection(other);
		int num = 0;
		int num2 = 0;
		foreach (T item in collection2)
		{
			num++;
			if (collection.Contains(item))
			{
				num2++;
			}
		}
		if (proper)
		{
			if (num2 == collection.Count)
			{
				return num > collection.Count;
			}
			return false;
		}
		return num2 == collection.Count;
	}

	private static bool IsSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other, bool proper)
	{
		ICollection<T> collection = AsDistinctCollection(source);
		ICollection<T> collection2 = AsDistinctCollection(other);
		int num = 0;
		foreach (T item in collection2)
		{
			num++;
			if (!collection.Contains(item))
			{
				return false;
			}
		}
		if (proper)
		{
			return num < collection.Count;
		}
		return true;
	}

	private static IEnumerable<T> NullOrEmptyCheckedExtracted<T>(IEnumerable<T> source, Action onEmpty)
	{
		IEnumerator<T> enumerator = source.GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				yield return enumerator.Current;
				while (enumerator.MoveNext())
				{
					yield return enumerator.Current;
				}
			}
			else
			{
				onEmpty();
			}
		}
		finally
		{
			enumerator.Dispose();
		}
	}

	private static IEnumerable<T> NullOrEmptyCheckedExtracted<T>(IEnumerable<T> source, Action onEmpty, Action onNotEmpty)
	{
		IEnumerator<T> enumerator = source.GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				onNotEmpty();
				yield return enumerator.Current;
				while (enumerator.MoveNext())
				{
					yield return enumerator.Current;
				}
			}
			else
			{
				onEmpty();
			}
		}
		finally
		{
			enumerator.Dispose();
		}
	}

	private static void SortExtracted<T>(IList<T> list, int indexStart, int indexEnd, IComparer<T> comparer)
	{
		int i = indexStart;
		int num = indexEnd;
		T val = list[i + (num - i) / 2];
		while (i <= num)
		{
			for (; i < indexEnd && comparer.Compare(list[i], val) < 0; i++)
			{
			}
			while (num > indexStart && comparer.Compare(val, list[num]) < 0)
			{
				num--;
			}
			if (i == num)
			{
				i++;
				num--;
			}
			else if (i < num)
			{
				SwapExtracted(list, i, num);
				i++;
				num--;
			}
		}
		if (indexStart < num)
		{
			SortExtracted(list, indexStart, num, comparer);
		}
		if (i < indexEnd)
		{
			SortExtracted(list, i, indexEnd, comparer);
		}
	}

	private static void SwapExtracted<T>(IList<T> list, int indexA, int indexB)
	{
		T value = list[indexA];
		T value2 = list[indexB];
		list[indexA] = value2;
		list[indexB] = value;
	}

	private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Predicate<T> predicate)
	{
		foreach (T item in source)
		{
			if (predicate(item))
			{
				yield return item;
			}
		}
	}

	private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, int, bool> predicate)
	{
		int index = 0;
		foreach (T item in source)
		{
			if (predicate(item, index))
			{
				yield return item;
			}
			index++;
		}
	}

	private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, bool> predicate, Action whereNot)
	{
		foreach (T item in source)
		{
			if (predicate(item))
			{
				yield return item;
			}
			else
			{
				whereNot();
			}
		}
	}

	private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, bool> predicate)
	{
		foreach (T item in source)
		{
			if (predicate(item))
			{
				yield return item;
			}
		}
	}

	private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, int, bool> predicate, Action whereNot)
	{
		int index = 0;
		foreach (T item in source)
		{
			if (predicate(item, index))
			{
				yield return item;
			}
			else
			{
				whereNot();
			}
			index++;
		}
	}

	private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, bool> predicate, Action<T> whereNot)
	{
		foreach (T item in source)
		{
			if (predicate(item))
			{
				yield return item;
			}
			else
			{
				whereNot(item);
			}
		}
	}

	private static IEnumerable<T> WhereExtracted<T>(IEnumerable<T> source, Func<T, int, bool> predicate, Action<T> whereNot)
	{
		int index = 0;
		foreach (T item in source)
		{
			if (predicate(item, index))
			{
				yield return item;
			}
			else
			{
				whereNot(item);
			}
			index++;
		}
	}
}
