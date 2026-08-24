using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections;

[DebuggerNonUserCode]
public sealed class EmptyCollection<T> : ReadOnlyCollection<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
{
	private static readonly EmptyCollection<T> _instance = new EmptyCollection<T>();

	public static EmptyCollection<T> Instance => _instance;

	private EmptyCollection()
		: base((IList<T>)ArrayReservoir<T>.EmptyArray)
	{
	}
}
