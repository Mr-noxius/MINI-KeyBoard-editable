using System;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Threading.Needles;

namespace Theraot.Collections.Specialized;

[DebuggerNonUserCode]
public sealed class NeedleConversionEqualityComparer<TNeedle, T> : ConversionEqualityComparer<TNeedle, T>, IEqualityComparer<TNeedle> where TNeedle : INeedle<T>
{
	public NeedleConversionEqualityComparer(IEqualityComparer<T> comparer)
		: base(comparer, (Func<TNeedle, T>)Conversion)
	{
	}

	private static T Conversion(TNeedle needle)
	{
		if (object.ReferenceEquals(needle, null))
		{
			return default(T);
		}
		return needle.Value;
	}
}
