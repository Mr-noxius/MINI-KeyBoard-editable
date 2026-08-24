using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe;

public static class ArrayReservoir<T>
{
	private const int _capacityCount = 8;

	private const int _maxCapacity = 1024;

	private const int _maxCapacityLog2 = 10;

	private const int _minCapacity = 8;

	private const int _minCapacityLog2 = 3;

	private const int _poolSize = 16;

	private static readonly T[] _emptyArray;

	private static readonly Pool<T[]>[] _pools;

	private static int _done;

	public static T[] EmptyArray => _emptyArray;

	static ArrayReservoir()
	{
		if (typeof(T) == typeof(Type))
		{
			_emptyArray = (T[])(object)Type.EmptyTypes;
		}
		else
		{
			_emptyArray = new T[0];
		}
		_pools = new Pool<T[]>[8];
		for (int i = 0; i < 8; i++)
		{
			int currentIndex = i;
			_pools[i] = new Pool<T[]>(16, delegate(T[] item)
			{
				int length = 8 << currentIndex;
				Array.Clear(item, 0, length);
			});
		}
		Volatile.Write(ref _done, 1);
	}

	internal static void DonateArray(T[] donation)
	{
		if (donation == null || Volatile.Read(ref _done) == 0)
		{
			return;
		}
		Pool<T[]>[] pools = _pools;
		if (pools != null)
		{
			int num = donation.Length;
			if (num != 0 && num >= 8 && num <= 1024)
			{
				num = ((NumericHelper.PopulationCount(num) == 1) ? num : NumericHelper.NextPowerOf2(num));
				int num2 = NumericHelper.Log2(num) - 3;
				pools[num2]?.Donate(donation);
			}
		}
	}

	internal static T[] GetArray(int capacity)
	{
		if (capacity == 0)
		{
			return _emptyArray;
		}
		if (capacity < 8)
		{
			capacity = 8;
		}
		capacity = ((NumericHelper.PopulationCount(capacity) == 1) ? capacity : NumericHelper.NextPowerOf2(capacity));
		if (capacity <= 1024 && Volatile.Read(ref _done) == 1)
		{
			int num = NumericHelper.Log2(capacity) - 3;
			Pool<T[]> pool = _pools[num];
			if (pool.TryGet(out var result))
			{
				return result;
			}
		}
		return new T[capacity];
	}
}
