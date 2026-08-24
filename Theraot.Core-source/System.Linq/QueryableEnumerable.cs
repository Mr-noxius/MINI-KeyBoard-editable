using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq;

internal class QueryableEnumerable<TElement> : IQueryableEnumerable<TElement>, IQueryableEnumerable, IOrderedQueryable<TElement>, IQueryable<TElement>, IEnumerable<TElement>, IOrderedQueryable, IQueryable, IEnumerable, IQueryProvider
{
	private readonly IEnumerable<TElement> _enumerable;

	private readonly Expression _expression;

	public Type ElementType => typeof(TElement);

	public Expression Expression => _expression;

	public IQueryProvider Provider => this;

	public QueryableEnumerable(IEnumerable<TElement> enumerable)
	{
		_expression = Expression.Constant(this);
		_enumerable = enumerable;
	}

	public QueryableEnumerable(Expression expression)
	{
		_expression = expression;
	}

	public IQueryable CreateQuery(Expression expression)
	{
		return (IQueryable)Activator.CreateInstance(typeof(QueryableEnumerable<>).MakeGenericType(expression.Type.GetFirstGenericArgument()), expression);
	}

	public IQueryable<TElem> CreateQuery<TElem>(Expression expression)
	{
		return new QueryableEnumerable<TElem>(expression);
	}

	public object Execute(Expression expression)
	{
		LambdaExpression lambdaExpression = Expression.Lambda(TransformQueryable(expression));
		return lambdaExpression.Compile().DynamicInvoke();
	}

	public TResult Execute<TResult>(Expression expression)
	{
		Expression<Func<TResult>> expression2 = Expression.Lambda<Func<TResult>>(TransformQueryable(expression), new ParameterExpression[0]);
		return expression2.Compile()();
	}

	public IEnumerable GetEnumerable()
	{
		return _enumerable;
	}

	public IEnumerator<TElement> GetEnumerator()
	{
		return Execute<IEnumerable<TElement>>(_expression).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override string ToString()
	{
		if (_enumerable != null)
		{
			return _enumerable.ToString();
		}
		if (_expression == null)
		{
			return base.ToString();
		}
		if (!(_expression is ConstantExpression constantExpression) || constantExpression.Value != this)
		{
			return _expression.ToString();
		}
		return base.ToString();
	}

	private static Expression TransformQueryable(Expression expression)
	{
		return new QueryableTransformer().Transform(expression);
	}
}
