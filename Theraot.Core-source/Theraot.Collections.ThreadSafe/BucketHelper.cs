using System;

namespace Theraot.Collections.ThreadSafe;

public static class BucketHelper
{
	private static readonly object _null;

	internal static object Null => _null;

	static BucketHelper()
	{
		_null = new object();
	}

	public static T GetOrInsert<T>(this IBucket<T> bucket, int index, T item)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (bucket.Insert(index, item, out var previous))
		{
			return item;
		}
		return previous;
	}

	public static T GetOrInsert<T>(this IBucket<T> bucket, int index, Func<T> itemFactory)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (!bucket.TryGet(index, out var value))
		{
			T val = itemFactory();
			if (bucket.Insert(index, val, out value))
			{
				return val;
			}
		}
		return value;
	}

	public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, Predicate<T> check)
	{
		bool isNew;
		return bucket.InsertOrUpdate(index, item, itemUpdateFactory, check, out isNew);
	}

	public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isNew)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		isNew = true;
		while (true)
		{
			if (isNew)
			{
				if (bucket.Insert(index, item, out var _))
				{
					return true;
				}
				isNew = false;
				continue;
			}
			if (bucket.Update(index, itemUpdateFactory, check, out isNew))
			{
				return true;
			}
			if (!isNew)
			{
				break;
			}
		}
		return false;
	}

	public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory)
	{
		bucket.InsertOrUpdate(index, item, itemUpdateFactory, out var _);
	}

	public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, out bool isNew)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		isNew = true;
		while (true)
		{
			if (isNew)
			{
				if (bucket.Insert(index, item, out var _))
				{
					break;
				}
				isNew = false;
			}
			else if (bucket.Update(index, itemUpdateFactory, Tautology, out isNew))
			{
				break;
			}
		}
	}

	public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check)
	{
		bool isNew;
		return bucket.InsertOrUpdate(index, item, check, out isNew);
	}

	public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out bool isNew)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		isNew = true;
		while (true)
		{
			if (isNew)
			{
				if (bucket.Insert(index, item, out var _))
				{
					return true;
				}
				isNew = false;
				continue;
			}
			if (bucket.Update(index, (T _) => item, check, out isNew))
			{
				return true;
			}
			if (!isNew)
			{
				break;
			}
		}
		return false;
	}

	public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, Predicate<T> check)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		bool isNew;
		return bucket.InsertOrUpdate(index, itemFactory, itemUpdateFactory, check, out isNew);
	}

	public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isNew)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		isNew = true;
		bool flag = false;
		T item = default(T);
		while (true)
		{
			if (isNew)
			{
				if (!flag)
				{
					item = itemFactory();
					flag = true;
				}
				if (bucket.Insert(index, item, out var _))
				{
					return true;
				}
				isNew = false;
			}
			else
			{
				if (bucket.Update(index, itemUpdateFactory, check, out isNew))
				{
					return true;
				}
				if (!isNew)
				{
					break;
				}
			}
		}
		return false;
	}

	public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		bucket.InsertOrUpdate(index, itemFactory, itemUpdateFactory, out var _);
	}

	public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, out bool isNew)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		isNew = true;
		bool flag = false;
		T item = default(T);
		while (true)
		{
			if (isNew)
			{
				if (!flag)
				{
					item = itemFactory();
					flag = true;
				}
				if (bucket.Insert(index, item, out var _))
				{
					break;
				}
				isNew = false;
			}
			else if (bucket.Update(index, itemUpdateFactory, Tautology, out isNew))
			{
				break;
			}
		}
	}

	public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Predicate<T> check)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		bool isNew;
		return bucket.InsertOrUpdate(index, itemFactory, check, out isNew);
	}

	public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Predicate<T> check, out bool isNew)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		isNew = true;
		bool flag = false;
		T item = default(T);
		while (true)
		{
			if (isNew)
			{
				if (!flag)
				{
					item = itemFactory();
					flag = true;
				}
				if (bucket.Insert(index, item, out var _))
				{
					return true;
				}
				isNew = false;
			}
			else
			{
				T result = itemFactory();
				if (bucket.Update(index, (T _) => result, check, out isNew))
				{
					return true;
				}
				if (!isNew)
				{
					break;
				}
			}
		}
		return false;
	}

	public static void Set<T>(this IBucket<T> bucket, int index, T value)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		bucket.Set(index, value, out var _);
	}

	public static bool TryGetOrInsert<T>(this IBucket<T> bucket, int index, T item, out T stored)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (bucket.Insert(index, item, out var previous))
		{
			stored = item;
			return true;
		}
		stored = previous;
		return false;
	}

	public static bool TryGetOrInsert<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, out T stored)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (bucket.TryGet(index, out stored))
		{
			return false;
		}
		T val = itemFactory();
		if (bucket.Insert(index, val, out stored))
		{
			stored = val;
			return true;
		}
		return false;
	}

	public static bool Update<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		bool isEmpty;
		return bucket.Update(index, (T _) => item, check, out isEmpty);
	}

	public static bool Update<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out bool isEmpty)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		return bucket.Update(index, (T _) => item, check, out isEmpty);
	}

	public static bool Update<T>(this IBucket<T> bucket, int index, Func<T, T> itemUpdateFactory)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		bool isEmpty;
		return bucket.Update(index, itemUpdateFactory, Tautology, out isEmpty);
	}

	public static bool Update<T>(this IBucket<T> bucket, int index, Func<T, T> itemUpdateFactory, out bool isEmpty)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		return bucket.Update(index, itemUpdateFactory, Tautology, out isEmpty);
	}

	private static bool Tautology<T>(T item)
	{
		GC.KeepAlive(item);
		return true;
	}
}
