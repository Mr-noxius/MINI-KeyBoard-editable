using System.Threading;
using Theraot.Core;

namespace System.Dynamic.Utils;

internal class CacheDict<TKey, TValue>
{
	private sealed class Entry
	{
		internal readonly int Hash;

		internal readonly TKey Key;

		internal readonly TValue Value;

		internal Entry(int hash, TKey key, TValue value)
		{
			Hash = hash;
			Key = key;
			Value = value;
		}
	}

	private readonly int _mask;

	private readonly Entry[] _entries;

	internal TValue this[TKey key]
	{
		set
		{
			Add(key, value);
		}
	}

	internal CacheDict(int size)
	{
		int num = NumericHelper.NextPowerOf2(size - 1);
		_mask = num - 1;
		_entries = new Entry[num];
	}

	internal bool TryGetValue(TKey key, out TValue value)
	{
		int hashCode = key.GetHashCode();
		int num = hashCode & _mask;
		Entry entry = Volatile.Read(ref _entries[num]);
		if (entry != null && entry.Hash == hashCode && entry.Key.Equals(key))
		{
			value = entry.Value;
			return true;
		}
		value = default(TValue);
		return false;
	}

	internal void Add(TKey key, TValue value)
	{
		int hashCode = key.GetHashCode();
		int num = hashCode & _mask;
		Entry entry = Volatile.Read(ref _entries[num]);
		if (entry == null || entry.Hash != hashCode || !entry.Key.Equals(key))
		{
			Volatile.Write(ref _entries[num], new Entry(hashCode, key, value));
		}
	}
}
