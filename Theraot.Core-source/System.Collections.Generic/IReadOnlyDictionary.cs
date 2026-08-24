namespace System.Collections.Generic;

public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	IEnumerable<TKey> Keys { get; }

	IEnumerable<TValue> Values { get; }

	TValue this[TKey key] { get; }

	bool ContainsKey(TKey key);

	bool TryGetValue(TKey key, out TValue value);
}
