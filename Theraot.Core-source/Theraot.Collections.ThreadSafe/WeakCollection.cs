using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe;

[DebuggerDisplay("Count={Count}")]
[DebuggerNonUserCode]
public class WeakCollection<T, TNeedle> : ICollection<T>, IEnumerable<T>, IEnumerable where T : class where TNeedle : WeakNeedle<T>
{
	private readonly IEqualityComparer<T> _comparer;

	private readonly SafeDictionary<int, TNeedle> _wrapped;

	private StructNeedle<WeakNeedle<EventHandler>> _eventHandler;

	private int _maxIndex;

	public bool AutoRemoveDeadItems
	{
		get
		{
			return _eventHandler.IsAlive;
		}
		set
		{
			if (value)
			{
				RegisterForAutoRemoveDeadItems();
			}
			else
			{
				UnRegisterForAutoRemoveDeadItems();
			}
		}
	}

	public int Count => _wrapped.Count;

	bool ICollection<T>.IsReadOnly => false;

	public WeakCollection()
		: this((IEqualityComparer<T>)null, true)
	{
	}

	public WeakCollection(IEqualityComparer<T> comparer)
		: this(comparer, true)
	{
	}

	public WeakCollection(bool autoRemoveDeadItems)
		: this((IEqualityComparer<T>)null, autoRemoveDeadItems)
	{
	}

	public WeakCollection(IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
	{
		_maxIndex = -1;
		_comparer = comparer ?? EqualityComparer<T>.Default;
		_wrapped = new SafeDictionary<int, TNeedle>();
		if (autoRemoveDeadItems)
		{
			RegisterForAutoRemoveDeadItemsExtracted();
		}
		else
		{
			GC.SuppressFinalize(this);
		}
	}

	public WeakCollection(IEqualityComparer<T> comparer, int initialProbing)
		: this(comparer, true, initialProbing)
	{
	}

	public WeakCollection(bool autoRemoveDeadItems, int initialProbing)
		: this((IEqualityComparer<T>)null, autoRemoveDeadItems, initialProbing)
	{
	}

	public WeakCollection(IEqualityComparer<T> comparer, bool autoRemoveDeadItems, int initialProbing)
	{
		_maxIndex = -1;
		_comparer = comparer ?? EqualityComparer<T>.Default;
		_wrapped = new SafeDictionary<int, TNeedle>(initialProbing);
		if (autoRemoveDeadItems)
		{
			RegisterForAutoRemoveDeadItemsExtracted();
		}
		else
		{
			GC.SuppressFinalize(this);
		}
	}

	public WeakCollection(int initialProbing)
		: this((IEqualityComparer<T>)null, true, initialProbing)
	{
	}

	~WeakCollection()
	{
		UnRegisterForAutoRemoveDeadItemsExtracted();
	}

	public void Add(T item)
	{
		TNeedle value = NeedleHelper.CreateNeedle<T, TNeedle>(item);
		_wrapped.Set(Interlocked.Increment(ref _maxIndex), value);
	}

	public void Clear()
	{
		IEnumerable<KeyValuePair<int, TNeedle>> enumerable = _wrapped.ClearEnumerable();
		foreach (KeyValuePair<int, TNeedle> item in enumerable)
		{
			TNeedle value = item.Value;
			value.Dispose();
		}
	}

	public bool Contains(T item)
	{
		using (IEnumerator<T> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				if (_comparer.Equals(current, item))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool Contains(Predicate<T> itemCheck)
	{
		using (IEnumerator<T> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				if (itemCheck(current))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Extensions.CopyTo(this, array, arrayIndex);
	}

	public bool Equals(T x, T y)
	{
		return _comparer.Equals(x, y);
	}

	public IEnumerator<T> GetEnumerator()
	{
		foreach (KeyValuePair<int, TNeedle> pair in _wrapped)
		{
			KeyValuePair<int, TNeedle> keyValuePair = pair;
			TNeedle value = keyValuePair.Value;
			if (value.TryGetValue(out var result))
			{
				yield return result;
			}
		}
	}

	public bool Remove(T item)
	{
		Predicate<TNeedle> valueCheck = (TNeedle input) => input.TryGetValue(out var value) && _comparer.Equals(item, value);
		using (IEnumerator<TNeedle> enumerator = _wrapped.RemoveWhereValueEnumerable(valueCheck).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				TNeedle current = enumerator.Current;
				current.Dispose();
				return true;
			}
		}
		return false;
	}

	public int RemoveDeadItems()
	{
		return _wrapped.RemoveWhere(delegate(KeyValuePair<int, TNeedle> input)
		{
			TNeedle value = input.Value;
			return !value.IsAlive;
		});
	}

	public int RemoveWhere(Predicate<T> itemCheck)
	{
		Predicate<TNeedle> valueCheck = (TNeedle input) => input.TryGetValue(out var value) && itemCheck(value);
		return _wrapped.RemoveWhereValue(valueCheck);
	}

	public IEnumerable<T> RemoveWhereEnumerable(Predicate<T> itemCheck)
	{
		Predicate<TNeedle> check = (TNeedle input) => input.TryGetValue(out var value2) && itemCheck(value2);
		foreach (TNeedle removed in _wrapped.RemoveWhereValueEnumerable(check))
		{
			TNeedle val = removed;
			if (val.TryGetValue(out var value))
			{
				yield return value;
			}
			TNeedle val2 = removed;
			val2.Dispose();
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	protected void Add(TNeedle needle)
	{
		_wrapped.Set(Interlocked.Increment(ref _maxIndex), needle);
	}

	protected bool Contains(Predicate<TNeedle> needleCheck)
	{
		foreach (KeyValuePair<int, TNeedle> item in _wrapped)
		{
			if (needleCheck(item.Value))
			{
				return true;
			}
		}
		return false;
	}

	protected IEnumerable<TNeedle> GetNeedleEnumerable()
	{
		foreach (KeyValuePair<int, TNeedle> pair in _wrapped)
		{
			KeyValuePair<int, TNeedle> keyValuePair = pair;
			yield return keyValuePair.Value;
		}
	}

	protected IEnumerable<T> RemoveWhereEnumerable(Predicate<TNeedle> needleCheck)
	{
		foreach (TNeedle removed in _wrapped.RemoveWhereValueEnumerable(needleCheck))
		{
			TNeedle val = removed;
			if (val.TryGetValue(out var value))
			{
				yield return value;
			}
			TNeedle val2 = removed;
			val2.Dispose();
		}
	}

	private void GarbageCollected(object sender, EventArgs e)
	{
		RemoveDeadItems();
	}

	private void RegisterForAutoRemoveDeadItems()
	{
		if (RegisterForAutoRemoveDeadItemsExtracted())
		{
			GC.ReRegisterForFinalize(this);
		}
	}

	private bool RegisterForAutoRemoveDeadItemsExtracted()
	{
		bool result = false;
		EventHandler eventHandler;
		if (object.ReferenceEquals(_eventHandler.Value, null))
		{
			eventHandler = GarbageCollected;
			_eventHandler = new WeakNeedle<EventHandler>(eventHandler);
			result = true;
		}
		else
		{
			eventHandler = _eventHandler.Value.Value;
			if (!_eventHandler.IsAlive)
			{
				eventHandler = GarbageCollected;
				_eventHandler.Value = eventHandler;
				result = true;
			}
		}
		GCMonitor.Collected += eventHandler;
		return result;
	}

	private void UnRegisterForAutoRemoveDeadItems()
	{
		if (UnRegisterForAutoRemoveDeadItemsExtracted())
		{
			GC.SuppressFinalize(this);
		}
	}

	private bool UnRegisterForAutoRemoveDeadItemsExtracted()
	{
		if (_eventHandler.Value.Retrieve<EventHandler, WeakNeedle<EventHandler>>(out var target))
		{
			GCMonitor.Collected -= target;
			_eventHandler.Value = null;
			return true;
		}
		_eventHandler.Value = null;
		return false;
	}
}
