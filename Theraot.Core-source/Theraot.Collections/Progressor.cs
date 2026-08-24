using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections;

[Serializable]
public sealed class Progressor<T> : IObservable<T>
{
	private ProxyObservable<T> _proxy;

	private TryTake<T> _tryTake;

	private bool _done;

	public bool IsClosed => _tryTake == null;

	public Progressor(Progressor<T> wrapped)
	{
		Progressor<T> progressor = this;
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		int control = 0;
		Predicate<T> newFilter = (T item) => Volatile.Read(ref control) == 0;
		SafeQueue<T> buffer = new SafeQueue<T>();
		wrapped.SubscribeAction(delegate(T item)
		{
			if (newFilter(item))
			{
				buffer.Add(item);
			}
		});
		_proxy = new ProxyObservable<T>();
		_tryTake = delegate(out T value)
		{
			Interlocked.Increment(ref control);
			try
			{
				if (buffer.TryTake(out value) || wrapped.TryTake(out value))
				{
					progressor._proxy.OnNext(value);
					return true;
				}
				progressor._done = wrapped._done;
				return false;
			}
			finally
			{
				Interlocked.Decrement(ref control);
			}
		};
	}

	public Progressor(IEnumerable<T> preface, Progressor<T> wrapped)
	{
		Progressor<T> progressor = this;
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		if (preface == null)
		{
			throw new ArgumentNullException("preface");
		}
		IEnumerator<T> enumerator = preface.GetEnumerator();
		if (enumerator == null)
		{
			throw new ArgumentException("preface.GetEnumerator()");
		}
		int control = 0;
		int guard = 0;
		Predicate<T> newFilter = (T item) => Volatile.Read(ref control) == 0;
		SafeQueue<T> buffer = new SafeQueue<T>();
		wrapped.SubscribeAction(delegate(T item)
		{
			if (newFilter(item))
			{
				buffer.Add(item);
			}
		});
		_proxy = new ProxyObservable<T>();
		TryTake<T> tryTakeReplacement = delegate(out T value)
		{
			Interlocked.Increment(ref control);
			try
			{
				if (buffer.TryTake(out value) || wrapped.TryTake(out value))
				{
					progressor._proxy.OnNext(value);
					return true;
				}
				progressor._done = wrapped._done;
				return false;
			}
			finally
			{
				Interlocked.Decrement(ref control);
			}
		};
		_tryTake = delegate(out T value)
		{
			value = default(T);
			if (Volatile.Read(ref guard) == 0)
			{
				bool flag;
				lock (enumerator)
				{
					flag = enumerator.MoveNext();
					if (flag)
					{
						value = enumerator.Current;
					}
				}
				if (flag)
				{
					progressor._proxy.OnNext(value);
					return true;
				}
				enumerator.Dispose();
				Interlocked.CompareExchange(ref guard, 1, 0);
			}
			if (Interlocked.CompareExchange(ref guard, 2, 1) == 1)
			{
				progressor._tryTake = tryTakeReplacement;
				Volatile.Write(ref guard, 3);
			}
			else
			{
				ThreadingHelper.SpinWaitUntil(ref guard, 3);
			}
			TryTake<T> tryTake = progressor._tryTake;
			return tryTake(out value);
		};
	}

	public Progressor(T[] wrapped)
	{
		Progressor<T> progressor = this;
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		int guard = 0;
		int index = -1;
		_proxy = new ProxyObservable<T>();
		TryTake<T> tryTakeReplacement = delegate(out T value)
		{
			value = default(T);
			return false;
		};
		_tryTake = delegate(out T value)
		{
			value = default(T);
			if (Volatile.Read(ref guard) == 0)
			{
				int num = Interlocked.Increment(ref index);
				if (num < wrapped.Length)
				{
					value = wrapped[num];
					progressor._proxy.OnNext(value);
					return true;
				}
				Interlocked.CompareExchange(ref guard, 1, 0);
			}
			if (Interlocked.CompareExchange(ref guard, 2, 1) == 1)
			{
				progressor._tryTake = tryTakeReplacement;
			}
			return false;
		};
	}

	public Progressor(T[] preface, Progressor<T> wrapped)
	{
		Progressor<T> progressor = this;
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		if (preface == null)
		{
			throw new ArgumentNullException("preface");
		}
		int control = 0;
		int guard = 0;
		int index = -1;
		Predicate<T> newFilter = (T item) => Volatile.Read(ref control) == 0;
		SafeQueue<T> buffer = new SafeQueue<T>();
		wrapped.SubscribeAction(delegate(T item)
		{
			if (newFilter(item))
			{
				buffer.Add(item);
			}
		});
		_proxy = new ProxyObservable<T>();
		TryTake<T> tryTakeReplacement = delegate(out T value)
		{
			Interlocked.Increment(ref control);
			try
			{
				if (buffer.TryTake(out value) || wrapped.TryTake(out value))
				{
					progressor._proxy.OnNext(value);
					return true;
				}
				progressor._done = wrapped._done;
				return false;
			}
			finally
			{
				Interlocked.Decrement(ref control);
			}
		};
		_tryTake = delegate(out T value)
		{
			if (Volatile.Read(ref guard) == 0)
			{
				int num = Interlocked.Increment(ref index);
				if (num < preface.Length)
				{
					value = preface[num];
					progressor._proxy.OnNext(value);
					return true;
				}
				Interlocked.CompareExchange(ref guard, 1, 0);
			}
			if (Interlocked.CompareExchange(ref guard, 2, 1) == 1)
			{
				progressor._tryTake = tryTakeReplacement;
				Volatile.Write(ref guard, 3);
			}
			else
			{
				ThreadingHelper.SpinWaitUntil(ref guard, 3);
			}
			TryTake<T> tryTake = progressor._tryTake;
			return tryTake(out value);
		};
	}

	public Progressor(IEnumerable<T> wrapped)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		IEnumerator<T> enumerator = wrapped.GetEnumerator();
		if (enumerator == null)
		{
			throw new ArgumentException("wrapped.GetEnumerator()");
		}
		int guard = 0;
		_proxy = new ProxyObservable<T>();
		TryTake<T> tryTakeReplacement = delegate(out T value)
		{
			value = default(T);
			return false;
		};
		_tryTake = delegate(out T value)
		{
			value = default(T);
			if (Volatile.Read(ref guard) == 0)
			{
				bool flag;
				lock (enumerator)
				{
					flag = enumerator.MoveNext();
					if (flag)
					{
						value = enumerator.Current;
					}
				}
				if (flag)
				{
					_proxy.OnNext(value);
					return true;
				}
				enumerator.Dispose();
				Interlocked.CompareExchange(ref guard, 1, 0);
			}
			if (Interlocked.CompareExchange(ref guard, 2, 1) == 1)
			{
				_tryTake = tryTakeReplacement;
			}
			return false;
		};
	}

	public Progressor(TryTake<T> tryTake, bool doneOnFalse)
	{
		Progressor<T> progressor = this;
		if (tryTake == null)
		{
			throw new ArgumentNullException("tryTake");
		}
		TryTake<T> tryTakeCopy = tryTake;
		_proxy = new ProxyObservable<T>();
		_tryTake = delegate(out T value)
		{
			if (tryTakeCopy(out value))
			{
				progressor._proxy.OnNext(value);
				return true;
			}
			progressor._done = doneOnFalse;
			return false;
		};
	}

	public Progressor(TryTake<T> tryTake, Func<bool> isDone)
	{
		Progressor<T> progressor = this;
		if (tryTake == null)
		{
			throw new ArgumentNullException("tryTake");
		}
		if (isDone == null)
		{
			throw new ArgumentNullException("isDone");
		}
		TryTake<T> tryTakeCopy = tryTake;
		_proxy = new ProxyObservable<T>();
		_tryTake = delegate(out T value)
		{
			if (tryTakeCopy(out value))
			{
				progressor._proxy.OnNext(value);
				return true;
			}
			progressor._done = new ValueFuncClosure<bool>(isDone).InvokeReturn();
			return false;
		};
	}

	public Progressor(IObservable<T> wrapped)
	{
		SafeQueue<T> buffer = new SafeQueue<T>();
		Action onCompleted = delegate
		{
			_done = true;
		};
		wrapped.Subscribe(new CustomObserver<T>(onCompleted, delegate
		{
			_done = true;
		}, buffer.Add));
		_proxy = new ProxyObservable<T>();
		_tryTake = delegate(out T value)
		{
			if (buffer.TryTake(out value))
			{
				_proxy.OnNext(value);
				return true;
			}
			value = default(T);
			return false;
		};
	}

	private Progressor(TryTake<T> tryTake, ProxyObservable<T> proxy)
	{
		_proxy = proxy;
		_tryTake = tryTake;
	}

	public static Progressor<T> CreateConverted<TInput>(Progressor<TInput> wrapped, Func<TInput, T> converter)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		int control = 0;
		Predicate<TInput> newFilter = (TInput item) => Volatile.Read(ref control) == 0;
		SafeQueue<T> buffer = new SafeQueue<T>();
		ProxyObservable<T> proxy = new ProxyObservable<T>();
		Progressor<T> result = new Progressor<T>(delegate(out T value)
		{
			Interlocked.Increment(ref control);
			try
			{
				if (buffer.TryTake(out value))
				{
					proxy.OnNext(value);
					return true;
				}
				if (wrapped.TryTake(out var item))
				{
					value = converter(item);
					proxy.OnNext(value);
					return true;
				}
				value = default(T);
				return false;
			}
			finally
			{
				Interlocked.Decrement(ref control);
			}
		}, proxy);
		wrapped.Subscribe(new CustomObserver<TInput>(delegate
		{
			result._done = true;
		}, delegate
		{
			result._done = true;
		}, delegate(TInput item)
		{
			if (newFilter(item))
			{
				buffer.Add(converter(item));
			}
		}));
		return result;
	}

	public static Progressor<T> CreatedFiltered(Progressor<T> wrapped, Predicate<T> filter)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		int control = 0;
		Predicate<T> newFilter = (T item) => Volatile.Read(ref control) == 0 && filter(item);
		SafeQueue<T> buffer = new SafeQueue<T>();
		ProxyObservable<T> proxy = new ProxyObservable<T>();
		Progressor<T> result = new Progressor<T>(delegate(out T value)
		{
			Volatile.Write(ref control, 1);
			try
			{
				while (true)
				{
					if (buffer.TryTake(out value))
					{
						proxy.OnNext(value);
						return true;
					}
					if (!wrapped.TryTake(out value))
					{
						break;
					}
					if (filter(value))
					{
						proxy.OnNext(value);
						return true;
					}
				}
				value = default(T);
				return false;
			}
			finally
			{
				Interlocked.Decrement(ref control);
			}
		}, proxy);
		wrapped.Subscribe(new CustomObserver<T>(delegate
		{
			result._done = true;
		}, delegate
		{
			result._done = true;
		}, delegate(T item)
		{
			if (newFilter(item))
			{
				buffer.Add(item);
			}
		}));
		return result;
	}

	public static Progressor<T> CreatedFilteredConverted<TInput>(Progressor<TInput> wrapped, Predicate<TInput> filter, Func<TInput, T> converter)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		int control = 0;
		Predicate<TInput> newFilter = (TInput item) => Volatile.Read(ref control) == 0 && filter(item);
		SafeQueue<T> buffer = new SafeQueue<T>();
		ProxyObservable<T> proxy = new ProxyObservable<T>();
		Progressor<T> result = new Progressor<T>(delegate(out T value)
		{
			Interlocked.Increment(ref control);
			try
			{
				while (true)
				{
					if (buffer.TryTake(out value))
					{
						proxy.OnNext(value);
						return true;
					}
					if (!wrapped.TryTake(out var item))
					{
						break;
					}
					if (filter(item))
					{
						value = converter(item);
						proxy.OnNext(value);
						return true;
					}
				}
				value = default(T);
				return false;
			}
			finally
			{
				Interlocked.Decrement(ref control);
			}
		}, proxy);
		wrapped.Subscribe(new CustomObserver<TInput>(delegate
		{
			result._done = true;
		}, delegate
		{
			result._done = true;
		}, delegate(TInput item)
		{
			if (newFilter(item))
			{
				buffer.Add(converter(item));
			}
		}));
		return result;
	}

	public static Progressor<T> CreateDistinct(Progressor<T> wrapped)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		int control = 0;
		SafeDictionary<T, bool> buffer = new SafeDictionary<T, bool>();
		Predicate<T> newFilter = (T item) => Volatile.Read(ref control) == 0;
		ProxyObservable<T> proxy = new ProxyObservable<T>();
		Progressor<T> result = new Progressor<T>(delegate(out T value)
		{
			Interlocked.Increment(ref control);
			try
			{
				while (true)
				{
					using (IEnumerator<KeyValuePair<T, bool>> enumerator = buffer.Where((KeyValuePair<T, bool> item) => !item.Value).GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							value = enumerator.Current.Key;
							buffer.Set(value, value: true);
							proxy.OnNext(value);
							return true;
						}
					}
					if (!wrapped.TryTake(out value))
					{
						break;
					}
					if (!buffer.TryGetValue(value, out var value2) || !value2)
					{
						buffer.Set(value, value: true);
						proxy.OnNext(value);
						return true;
					}
				}
				return false;
			}
			finally
			{
				Interlocked.Decrement(ref control);
			}
		}, proxy);
		wrapped.Subscribe(new CustomObserver<T>(delegate
		{
			result._done = true;
		}, delegate
		{
			result._done = true;
		}, delegate(T item)
		{
			if (newFilter(item))
			{
				buffer.TryAdd(item, value: false);
			}
		}));
		return result;
	}

	public IEnumerable<T> AsEnumerable()
	{
		while (true)
		{
			TryTake<T> tryTake = _tryTake;
			if (tryTake(out var item))
			{
				yield return item;
				continue;
			}
			break;
		}
	}

	public void Close()
	{
		_tryTake = null;
		_proxy.OnCompleted();
		_proxy = null;
	}

	public IDisposable Subscribe(IObserver<T> observer)
	{
		if (_proxy != null)
		{
			return _proxy.Subscribe(observer);
		}
		return Disposable.Create(ActionHelper.GetNoopAction());
	}

	public bool TryTake(out T item)
	{
		if (_tryTake != null)
		{
			if (_tryTake(out item))
			{
				return true;
			}
			if (_done)
			{
				Close();
			}
			return false;
		}
		item = default(T);
		return false;
	}

	public IEnumerable<T> While(Predicate<T> condition)
	{
		if (condition == null)
		{
			throw new ArgumentNullException("condition");
		}
		while (true)
		{
			TryTake<T> tryTake = _tryTake;
			if (tryTake(out var item) && condition(item))
			{
				yield return item;
				continue;
			}
			break;
		}
	}

	public IEnumerable<T> While(Func<bool> condition)
	{
		if (condition == null)
		{
			throw new ArgumentNullException("condition");
		}
		while (true)
		{
			TryTake<T> tryTake = _tryTake;
			if (tryTake(out var item) && condition())
			{
				yield return item;
				continue;
			}
			break;
		}
	}
}
