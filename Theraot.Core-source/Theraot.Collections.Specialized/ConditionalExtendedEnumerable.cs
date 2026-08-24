using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Core;

namespace Theraot.Collections.Specialized;

[DebuggerNonUserCode]
public sealed class ConditionalExtendedEnumerable<T> : ExtendedEnumerableBase<T>, IEnumerable<T>, IEnumerable
{
	private readonly Func<bool> _enumerateAppend;

	private readonly Func<bool> _enumerateTarget;

	public ConditionalExtendedEnumerable(IEnumerable<T> target, IEnumerable<T> append, Func<bool> enumerateTarget, Func<bool> enumerateAppend)
		: base(target, append)
	{
		if (enumerateTarget == null)
		{
			throw new ArgumentNullException("enumerateTarget");
		}
		_enumerateTarget = enumerateTarget;
		_enumerateAppend = enumerateAppend ?? ((append == null) ? FuncHelper.GetFallacyFunc() : FuncHelper.GetTautologyFunc());
	}

	public override IEnumerator<T> GetEnumerator()
	{
		if (_enumerateTarget())
		{
			foreach (T item in base.Target)
			{
				yield return item;
			}
		}
		if (!_enumerateAppend())
		{
			yield break;
		}
		foreach (T item2 in base.Append)
		{
			yield return item2;
		}
	}
}
