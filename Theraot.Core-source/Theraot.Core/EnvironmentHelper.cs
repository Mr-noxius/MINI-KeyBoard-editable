using System;

namespace Theraot.Core;

public static class EnvironmentHelper
{
	private static readonly int _processorCount = Environment.ProcessorCount;

	public static bool Is64BitProcess => IntPtr.Size == 8;

	public static bool IsSingleCPU => _processorCount == 1;
}
