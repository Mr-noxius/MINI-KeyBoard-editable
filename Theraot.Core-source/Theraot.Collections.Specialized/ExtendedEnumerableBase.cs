using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections.Specialized;

[DebuggerNonUserCode]
public abstract class ExtendedEnumerableBase<T> : IEnumerable<T>, IEnumerable
{
	private readonly IEnumerable<T> _append;

	private readonly IEnumerable<T> _target;

	protected IEnumerable<T> Append => _append;

	protected IEnumerable<T> Target => _target;

	protected ExtendedEnumerableBase(IEnumerable<T> target, IEnumerable<T> append)
	{
		_target = target ?? ((IEnumerable<T>)ArrayReservoir<T>.EmptyArray);
		_append = append ?? ((IEnumerable<T>)ArrayReservoir<T>.EmptyArray);
	}

	public abstract IEnumerator<T> GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
