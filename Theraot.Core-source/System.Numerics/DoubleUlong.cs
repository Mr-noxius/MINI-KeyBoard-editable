using System.Runtime.InteropServices;

namespace System.Numerics;

[StructLayout(LayoutKind.Explicit)]
internal struct DoubleUlong
{
	[FieldOffset(0)]
	public double Dbl;

	[FieldOffset(0)]
	public ulong Uu;
}
