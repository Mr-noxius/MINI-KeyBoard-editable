using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe;

[DebuggerNonUserCode]
public sealed class WeakDelegateCollection : WeakCollection<Delegate, WeakDelegateNeedle>
{
	private readonly Action<object[]> _invoke;

	public WeakDelegateCollection()
	{
		_invoke = InvokeExtracted;
	}

	public WeakDelegateCollection(bool autoRemoveDeadItems, bool reentryGuard)
		: base(autoRemoveDeadItems)
	{
		if (reentryGuard)
		{
			_invoke = InvokeExtracted;
			return;
		}
		ReentryGuard guard = new ReentryGuard();
		_invoke = delegate(object[] input)
		{
			guard.Execute(delegate
			{
				InvokeExtracted(input);
			});
		};
	}

	public WeakDelegateCollection(bool autoRemoveDeadItems, bool reentryGuard, int maxProbing)
		: base(autoRemoveDeadItems, maxProbing)
	{
		if (reentryGuard)
		{
			_invoke = InvokeExtracted;
			return;
		}
		ReentryGuard guard = new ReentryGuard();
		_invoke = delegate(object[] input)
		{
			guard.Execute(delegate
			{
				InvokeExtracted(input);
			});
		};
	}

	public WeakDelegateCollection(int maxProbing)
		: base(maxProbing)
	{
		_invoke = InvokeExtracted;
	}

	public void Add(MethodInfo method, object target)
	{
		if (method == null)
		{
			throw new ArgumentNullException("method");
		}
		Add(new WeakDelegateNeedle(method, target));
	}

	public bool Contains(MethodInfo method, object target)
	{
		if (method == null)
		{
			throw new ArgumentNullException("method");
		}
		return Contains((WeakDelegateNeedle item) => item.Equals(method, target));
	}

	public void Invoke(params object[] args)
	{
		_invoke(args);
	}

	public bool Remove(MethodInfo method, object target)
	{
		if (method == null)
		{
			throw new ArgumentNullException("method");
		}
		using (IEnumerator<Delegate> enumerator = RemoveWhereEnumerable((WeakDelegateNeedle item) => item.Equals(method, target)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Delegate current = enumerator.Current;
				GC.KeepAlive(current);
				return true;
			}
		}
		return false;
	}

	private void InvokeExtracted(object[] args)
	{
		foreach (WeakDelegateNeedle item in GetNeedleEnumerable())
		{
			try
			{
				item.TryInvoke(args);
			}
			catch (NullReferenceException)
			{
			}
		}
	}
}
