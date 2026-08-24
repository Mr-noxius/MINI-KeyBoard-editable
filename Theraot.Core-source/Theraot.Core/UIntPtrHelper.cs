using System;

namespace Theraot.Core;

public static class UIntPtrHelper
{
	[CLSCompliant(false)]
	public static UIntPtr Add(UIntPtr pointer, int offset)
	{
		return UIntPtr.Add(pointer, offset);
	}

	[CLSCompliant(false)]
	public static UIntPtr Subtract(UIntPtr pointer, int offset)
	{
		return UIntPtr.Subtract(pointer, offset);
	}
}
