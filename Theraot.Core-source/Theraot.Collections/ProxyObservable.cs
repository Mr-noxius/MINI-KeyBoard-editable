using System;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections;

[Serializable]
public sealed class ProxyObservable<T> : IProxyObservable<T>, IObservable<T>, IObserver<T>
{
	private readonly SafeSet<Needle<IObserver<T>>> _observers;

	public ProxyObservable()
	{
		_observers = new SafeSet<Needle<IObserver<T>>>();
	}

	public void OnCompleted()
	{
		foreach (Needle<IObserver<T>> observer in _observers)
		{
			observer.Value.OnCompleted();
		}
	}

	public void OnError(Exception error)
	{
		foreach (Needle<IObserver<T>> observer in _observers)
		{
			observer.Value.OnError(error);
		}
	}

	public void OnNext(T value)
	{
		foreach (Needle<IObserver<T>> observer in _observers)
		{
			observer.Value.OnNext(value);
		}
	}

	public IDisposable Subscribe(IObserver<T> observer)
	{
		Needle<IObserver<T>> needle = new Needle<IObserver<T>>(observer);
		_observers.AddNew(needle);
		return Disposable.Create(delegate
		{
			_observers.Remove(needle);
		});
	}
}
