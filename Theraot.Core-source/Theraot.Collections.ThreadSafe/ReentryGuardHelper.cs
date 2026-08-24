using System;
using System.Collections.Generic;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe;

internal static class ReentryGuardHelper
{
	[ThreadStatic]
	private static HashSet<RuntimeUniqueIdProdiver.UniqueId> _guard;

	public static bool Enter(RuntimeUniqueIdProdiver.UniqueId id)
	{
		HashSet<RuntimeUniqueIdProdiver.UniqueId> guard = _guard;
		if (guard == null)
		{
			HashSet<RuntimeUniqueIdProdiver.UniqueId> hashSet = new HashSet<RuntimeUniqueIdProdiver.UniqueId>();
			hashSet.Add(id);
			_guard = hashSet;
			return true;
		}
		if (!guard.Contains(id))
		{
			guard.Add(id);
			return true;
		}
		return false;
	}

	public static bool IsTaken(RuntimeUniqueIdProdiver.UniqueId id)
	{
		return _guard.Contains(id);
	}

	public static void Leave(RuntimeUniqueIdProdiver.UniqueId id)
	{
		_guard?.Remove(id);
	}
}
