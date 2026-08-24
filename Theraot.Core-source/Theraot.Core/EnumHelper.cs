using System;

namespace Theraot.Core;

public static class EnumHelper
{
	public static bool HasFlag(Enum value, Enum flag)
	{
		return value.HasFlag(flag);
	}
}
