using System.Threading;
using Theraot.Core;

namespace System;

public class Progress<T> : IProgress<T>
{
	private readonly Action<T> _post;

	public event NewEventHandler<T> ProgressChanged;

	public Progress()
	{
		SynchronizationContext context = SynchronizationContext.Current;
		if (context == null)
		{
			_post = delegate(T value)
			{
				ThreadPool.QueueUserWorkItem(Callback, value);
			};
		}
		else
		{
			_post = delegate(T value)
			{
				context.Post(Callback, value);
			};
		}
	}

	public Progress(Action<T> handler)
		: this()
	{
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		ProgressChanged += delegate(object sender, T args)
		{
			handler(args);
		};
	}

	public void Report(T value)
	{
		OnReport(value);
	}

	protected virtual void OnReport(T value)
	{
		if (ProgressChanged != null)
		{
			_post(value);
		}
	}

	private void Callback(object value)
	{
		T value2 = (T)value;
		ProgressChanged?.Invoke(this, value2);
	}
}
