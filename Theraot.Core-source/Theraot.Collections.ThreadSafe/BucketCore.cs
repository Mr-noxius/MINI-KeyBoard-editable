using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
internal class BucketCore : IEnumerable<object>, IEnumerable
{
	private const int _capacity = 32;

	private const long _lvl1 = 32L;

	private const long _lvl2 = 1024L;

	private const long _lvl3 = 32768L;

	private const long _lvl4 = 1048576L;

	private const long _lvl5 = 33554432L;

	private const long _lvl6 = 1073741824L;

	private const long _lvl7 = 34359738368L;

	private readonly object[] _arrayFirst;

	private readonly object[] _arraySecond;

	private readonly int[] _arrayUse;

	private readonly int _level;

	public long Length => _level switch
	{
		1 => 32L, 
		2 => 1024L, 
		3 => 32768L, 
		4 => 1048576L, 
		5 => 33554432L, 
		6 => 1073741824L, 
		7 => 34359738368L, 
		_ => 0L, 
	};

	public BucketCore(int level)
	{
		if (level < 0 || level > 7)
		{
			throw new ArgumentOutOfRangeException("level", "level < 0 || level > 7");
		}
		_level = level;
		_arrayFirst = new object[32];
		_arraySecond = new object[32];
		_arrayUse = new int[32];
	}

	public bool Do(int index, DoAction callback)
	{
		if (_level == 1)
		{
			int num = SubIndex(index);
			return Do(ref _arrayUse[num], ref _arrayFirst[num], ref _arraySecond[num], callback);
		}
		int num2 = SubIndex(index);
		return Do(ref _arrayUse[num2], ref _arrayFirst[num2], ref _arraySecond[num2], delegate(ref object target)
		{
			try
			{
				return ((BucketCore)target).Do(index, callback);
			}
			catch (NullReferenceException)
			{
				return false;
			}
		});
	}

	public bool DoMayDecrement(int index, DoAction callback)
	{
		if (_level == 1)
		{
			int num = SubIndex(index);
			return DoMayDecrement(ref _arrayUse[num], ref _arrayFirst[num], ref _arraySecond[num], callback);
		}
		int num2 = SubIndex(index);
		return DoMayDecrement(ref _arrayUse[num2], ref _arrayFirst[num2], ref _arraySecond[num2], delegate(ref object target)
		{
			try
			{
				return ((BucketCore)target).DoMayDecrement(index, callback);
			}
			catch (NullReferenceException)
			{
				return false;
			}
		});
	}

	public bool DoMayIncrement(int index, DoAction callback)
	{
		if (_level == 1)
		{
			int num = SubIndex(index);
			return DoMayIncrement(ref _arrayUse[num], ref _arrayFirst[num], ref _arraySecond[num], FuncHelper.GetDefaultFunc<object>(), callback);
		}
		int num2 = SubIndex(index);
		return DoMayIncrement(ref _arrayUse[num2], ref _arrayFirst[num2], ref _arraySecond[num2], () => new BucketCore(_level - 1), delegate(ref object target)
		{
			try
			{
				return ((BucketCore)target).DoMayIncrement(index, callback);
			}
			catch (NullReferenceException)
			{
				return false;
			}
		});
	}

	public IEnumerable<object> EnumerateRange(int indexFrom, int indexTo)
	{
		if (indexFrom < 0)
		{
			throw new ArgumentOutOfRangeException("indexFrom", "indexFrom < 0");
		}
		if (indexTo < 0)
		{
			throw new ArgumentOutOfRangeException("indexTo", "indexTo < 0");
		}
		int startSubIndex = SubIndex(indexFrom);
		int endSubIndex = SubIndex(indexTo);
		return PrivateEnumerableRange(indexFrom, indexTo, startSubIndex, endSubIndex);
	}

	private IEnumerable<object> PrivateEnumerableRange(int indexFrom, int indexTo, int startSubIndex, int endSubIndex)
	{
		int step = ((endSubIndex - startSubIndex >= 0) ? 1 : (-1));
		for (int subindex = startSubIndex; subindex < endSubIndex + 1; subindex += step)
		{
			try
			{
				Interlocked.Increment(ref _arrayUse[subindex]);
				object foundFirst = Interlocked.CompareExchange(ref _arrayFirst[subindex], null, null);
				if (_level == 1)
				{
					if (foundFirst != null)
					{
						yield return foundFirst;
					}
				}
				else
				{
					if (!(foundFirst is BucketCore core))
					{
						continue;
					}
					int subIndexFrom = ((subindex == startSubIndex) ? core.SubIndex(indexFrom) : 0);
					int subIndexTo = ((subindex == endSubIndex) ? core.SubIndex(indexTo) : 31);
					foreach (object item in core.PrivateEnumerableRange(indexFrom, indexTo, subIndexFrom, subIndexTo))
					{
						yield return item;
					}
					continue;
				}
			}
			finally
			{
				DoLeave(ref _arrayUse[subindex], ref _arrayFirst[subindex], ref _arraySecond[subindex]);
			}
		}
	}

	public IEnumerator<object> GetEnumerator()
	{
		for (int subindex = 0; subindex < 32; subindex++)
		{
			object foundFirst = Interlocked.CompareExchange(ref _arrayFirst[subindex], null, null);
			if (foundFirst == null)
			{
				continue;
			}
			try
			{
				Interlocked.Increment(ref _arrayUse[subindex]);
				if (_level == 1)
				{
					yield return foundFirst;
					continue;
				}
				foreach (object item in (BucketCore)foundFirst)
				{
					yield return item;
				}
			}
			finally
			{
				DoLeave(ref _arrayUse[subindex], ref _arrayFirst[subindex], ref _arraySecond[subindex]);
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private static bool Do(ref int use, ref object first, ref object second, DoAction callback)
	{
		object obj = Interlocked.CompareExchange(ref first, null, null);
		if (obj == null)
		{
			return false;
		}
		try
		{
			Interlocked.Increment(ref use);
			return callback(ref first);
		}
		finally
		{
			DoLeave(ref use, ref first, ref second);
		}
	}

	private static void DoEnsureSize(ref int use, ref object first, ref object second, Func<object> factory)
	{
		try
		{
			Interlocked.Increment(ref use);
			object obj = Interlocked.CompareExchange(ref first, null, null);
			object obj2 = Interlocked.CompareExchange(ref second, obj, null);
			if (obj2 != null || obj != null)
			{
				return;
			}
			object obj3 = factory();
			obj = Interlocked.CompareExchange(ref first, obj3, null);
			if (obj == null)
			{
				if (obj3 != null)
				{
					Interlocked.Increment(ref use);
				}
				Interlocked.CompareExchange(ref second, obj3, null);
			}
		}
		finally
		{
			DoLeave(ref use, ref first, ref second);
		}
	}

	private static void DoLeave(ref int use, ref object first, ref object second)
	{
		if (Interlocked.Decrement(ref use) == 0)
		{
			Interlocked.Exchange(ref second, null);
			Interlocked.Exchange(ref first, null);
			object value = Interlocked.CompareExchange(ref second, null, null);
			Interlocked.CompareExchange(ref first, value, null);
		}
	}

	private static bool DoMayDecrement(ref int use, ref object first, ref object second, DoAction callback)
	{
		try
		{
			Interlocked.Increment(ref use);
			object value = Interlocked.CompareExchange(ref first, null, null);
			Interlocked.CompareExchange(ref second, value, null);
			if (callback(ref second))
			{
				Interlocked.Decrement(ref use);
				return true;
			}
			return false;
		}
		finally
		{
			DoLeave(ref use, ref first, ref second);
		}
	}

	private static bool DoMayIncrement(ref int use, ref object first, ref object second, Func<object> factory, DoAction callback)
	{
		try
		{
			Interlocked.Increment(ref use);
			DoEnsureSize(ref use, ref first, ref second, factory);
			if (callback(ref first))
			{
				Interlocked.Increment(ref use);
				return true;
			}
			return false;
		}
		finally
		{
			DoLeave(ref use, ref first, ref second);
		}
	}

	private int SubIndex(int index)
	{
		return (index >> 5 * (_level - 1)) & 0x1F;
	}
}
