using System.Collections;
using System.Collections.Generic;

namespace System.Linq;

internal interface IQueryableEnumerable : IQueryable, IEnumerable
{
	IEnumerable GetEnumerable();
}
internal interface IQueryableEnumerable<TElement> : IQueryableEnumerable, IOrderedQueryable<TElement>, IQueryable<TElement>, IEnumerable<TElement>, IOrderedQueryable, IQueryable, IEnumerable
{
}
