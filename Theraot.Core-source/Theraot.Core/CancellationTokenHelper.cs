using System;
using System.Threading;

namespace Theraot.Core;

public static class CancellationTokenHelper
{
	public static void ThrowIfSourceDisposed(this CancellationToken cancellationToken)
	{
		GC.KeepAlive(cancellationToken.WaitHandle);
	}
}
