using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Collections.Specialized;

[DebuggerNonUserCode]
public sealed class ExtendedEnumerable<T> : ExtendedEnumerableBase<T>, IEnumerable<T>, IEnumerable
{
	public ExtendedEnumerable(IEnumerable<T> target, IEnumerable<T> append)
		: base(target, append)
	{
	}

	public override IEnumerator<T> GetEnumerator()
	{
		foreach (T item in base.Target)
		{
			yield return item;
		}
		foreach (T item2 in base.Append)
		{
			yield return item2;
		}
	}
}
