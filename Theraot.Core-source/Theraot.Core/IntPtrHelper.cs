using System;

namespace Theraot.Core;

public static class IntPtrHelper
{
	public static IntPtr Add(IntPtr pointer, int offset)
	{
		return IntPtr.Add(pointer, offset);
	}

	public static IntPtr Subtract(IntPtr pointer, int offset)
	{
		return IntPtr.Subtract(pointer, offset);
	}
}
