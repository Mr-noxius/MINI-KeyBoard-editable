using System;
using System.Diagnostics;
using System.Globalization;
using Theraot.Core;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public static class NeedleHelper
{
	private static class DeferredNeedleCreator<T, TNeedle> where TNeedle : INeedle<T>
	{
		private static readonly bool _canCreate;

		private static readonly Func<Func<T>, TNeedle> _create;

		public static bool CanCreate => _canCreate;

		static DeferredNeedleCreator()
		{
			_canCreate = TypeHelper.TryGetCreate(out _create);
			if (!_canCreate)
			{
				_canCreate = TypeHelper.TryGetCreate(out Func<T, TNeedle> tmpA);
				if (_canCreate)
				{
					_create = (Func<T> target) => tmpA(target());
				}
				else
				{
					_canCreate = TypeHelper.TryGetCreate(out Func<TNeedle> tmpB);
					if (_canCreate)
					{
						_create = delegate(Func<T> target)
						{
							TNeedle result = tmpB();
							result.Value = target();
							return result;
						};
					}
				}
			}
			if (!_canCreate)
			{
				_create = delegate
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", new object[1] { typeof(TNeedle).Name }));
				};
			}
		}

		public static TNeedle Create(Func<T> target)
		{
			return _create(target);
		}
	}

	private static class DeferredReadOnlyNeedleCreator<T, TNeedle> where TNeedle : IReadOnlyNeedle<T>
	{
		private static readonly bool _canCreate;

		private static readonly Func<Func<T>, TNeedle> _create;

		public static bool CanCreate => _canCreate;

		static DeferredReadOnlyNeedleCreator()
		{
			_canCreate = TypeHelper.TryGetCreate(out _create);
			if (!_canCreate)
			{
				_canCreate = TypeHelper.TryGetCreate(out Func<T, TNeedle> tmp);
				if (_canCreate)
				{
					_create = (Func<T> target) => tmp(target());
				}
			}
			if (!_canCreate)
			{
				_create = delegate
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", new object[1] { typeof(TNeedle).Name }));
				};
			}
		}

		public static TNeedle Create(Func<T> target)
		{
			return _create(target);
		}
	}

	private static class NeedleCreator<T, TNeedle> where TNeedle : INeedle<T>
	{
		private static readonly bool _canCreate;

		private static readonly Func<T, TNeedle> _create;

		public static bool CanCreate => _canCreate;

		static NeedleCreator()
		{
			_canCreate = TypeHelper.TryGetCreate(out _create);
			if (_canCreate)
			{
				return;
			}
			_canCreate = TypeHelper.TryGetCreate(out Func<TNeedle> tmpA);
			if (_canCreate)
			{
				_create = delegate(T target)
				{
					TNeedle result = tmpA();
					result.Value = target;
					return result;
				};
			}
			else
			{
				_canCreate = TypeHelper.TryGetCreate(out Func<Func<T>, TNeedle> tmpB);
				if (_canCreate)
				{
					_create = (T target) => tmpB(() => target);
				}
			}
			if (!_canCreate)
			{
				_create = delegate
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", new object[1] { typeof(TNeedle).Name }));
				};
			}
		}

		public static TNeedle Create(T target)
		{
			return _create(target);
		}
	}

	private static class NestedNeedleCreator<T, TNeedle> where TNeedle : INeedle<T>
	{
		private static readonly bool _canCreate;

		private static readonly Func<INeedle<T>, TNeedle> _create;

		public static bool CanCreate => _canCreate;

		static NestedNeedleCreator()
		{
			_canCreate = TypeHelper.TryGetCreate(out _create);
			if (_canCreate)
			{
				return;
			}
			_canCreate = TypeHelper.TryGetCreate(out Func<Func<INeedle<T>>, TNeedle> tmp);
			if (_canCreate)
			{
				_create = (INeedle<T> target) => tmp(() => target);
			}
			if (!_canCreate)
			{
				_create = delegate
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", new object[1] { typeof(TNeedle).Name }));
				};
			}
		}

		public static TNeedle Create(INeedle<T> target)
		{
			return _create(target);
		}
	}

	private static class NestedReadOnlyNeedleCreator<T, TNeedle> where TNeedle : IReadOnlyNeedle<T>
	{
		private static readonly bool _canCreate;

		private static readonly Func<IReadOnlyNeedle<T>, TNeedle> _create;

		public static bool CanCreate => _canCreate;

		static NestedReadOnlyNeedleCreator()
		{
			_canCreate = TypeHelper.TryGetCreate(out _create);
			if (!_canCreate)
			{
				_canCreate = TypeHelper.TryGetCreate(out Func<Func<IReadOnlyNeedle<T>>, TNeedle> tmp);
				if (_canCreate)
				{
					_create = (IReadOnlyNeedle<T> target) => tmp(() => target);
				}
			}
			if (!_canCreate)
			{
				_create = delegate
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", new object[1] { typeof(TNeedle).Name }));
				};
			}
		}

		public static TNeedle Create(IReadOnlyNeedle<T> target)
		{
			return _create(target);
		}
	}

	private static class ReadOnlyNeedleCreator<T, TNeedle> where TNeedle : IReadOnlyNeedle<T>
	{
		private static readonly bool _canCreate;

		private static readonly Func<T, TNeedle> _create;

		public static bool CanCreate => _canCreate;

		static ReadOnlyNeedleCreator()
		{
			_canCreate = TypeHelper.TryGetCreate(out _create);
			if (!_canCreate)
			{
				_canCreate = TypeHelper.TryGetCreate(out Func<Func<T>, TNeedle> tmp);
				if (_canCreate)
				{
					_create = (T target) => tmp(() => target);
				}
			}
			if (!_canCreate)
			{
				_create = delegate
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", new object[1] { typeof(TNeedle).Name }));
				};
			}
		}

		public static TNeedle Create(T target)
		{
			return _create(target);
		}
	}

	public static bool CanCreateDeferredNeedle<T, TNeedle>() where TNeedle : INeedle<T>
	{
		return DeferredNeedleCreator<T, TNeedle>.CanCreate;
	}

	public static bool CanCreateDeferredReadOnlyNeedle<T, TNeedle>() where TNeedle : IReadOnlyNeedle<T>
	{
		return DeferredReadOnlyNeedleCreator<T, TNeedle>.CanCreate;
	}

	public static bool CanCreateNeedle<T, TNeedle>() where TNeedle : INeedle<T>
	{
		return NeedleCreator<T, TNeedle>.CanCreate;
	}

	public static bool CanCreateNestedNeedle<T, TNeedle>() where TNeedle : INeedle<T>
	{
		return NestedNeedleCreator<T, TNeedle>.CanCreate;
	}

	public static bool CanCreateNestedReadOnlyNeedle<T, TNeedle>() where TNeedle : IReadOnlyNeedle<T>
	{
		return NestedReadOnlyNeedleCreator<T, TNeedle>.CanCreate;
	}

	public static bool CanCreateReadOnlyNeedle<T, TNeedle>() where TNeedle : IReadOnlyNeedle<T>
	{
		return ReadOnlyNeedleCreator<T, TNeedle>.CanCreate;
	}

	public static TNeedle CreateDeferredNeedle<T, TNeedle>(Func<T> target) where TNeedle : INeedle<T>
	{
		return DeferredNeedleCreator<T, TNeedle>.Create(target);
	}

	public static TNeedle CreateDeferredReadOnlyNeedle<T, TNeedle>(Func<T> target) where TNeedle : IReadOnlyNeedle<T>
	{
		return DeferredReadOnlyNeedleCreator<T, TNeedle>.Create(target);
	}

	public static TNeedle CreateNeedle<T, TNeedle>(T target) where TNeedle : INeedle<T>
	{
		return NeedleCreator<T, TNeedle>.Create(target);
	}

	public static TNeedle CreateNestedNeedle<T, TNeedle>(INeedle<T> target) where TNeedle : INeedle<T>
	{
		return NestedNeedleCreator<T, TNeedle>.Create(target);
	}

	public static TNeedle CreateReadOnlyNeedle<T, TNeedle>(T target) where TNeedle : IReadOnlyNeedle<T>
	{
		return ReadOnlyNeedleCreator<T, TNeedle>.Create(target);
	}

	public static TNeedle CreateReadOnlyNestedNeedle<T, TNeedle>(IReadOnlyNeedle<T> target) where TNeedle : IReadOnlyNeedle<T>
	{
		return NestedReadOnlyNeedleCreator<T, TNeedle>.Create(target);
	}

	public static bool Retrieve<T, TNeedle>(this TNeedle needle, out T target) where TNeedle : IRecyclableNeedle<T>
	{
		if (object.ReferenceEquals(needle, null))
		{
			target = default(T);
			return false;
		}
		bool flag;
		if (!(needle is ICacheNeedle<T> cacheNeedle))
		{
			target = needle.Value;
			flag = needle.IsAlive;
		}
		else
		{
			flag = cacheNeedle.TryGetValue(out target);
		}
		if (flag)
		{
			needle.Free();
		}
		return flag;
	}

	public static bool TryGetValue<T>(this IReadOnlyNeedle<T> needle, out T target)
	{
		if (needle == null)
		{
			target = default(T);
			return false;
		}
		if (needle is ICacheNeedle<T> cacheNeedle)
		{
			return cacheNeedle.TryGetValue(out target);
		}
		target = ((INeedle<T>)needle).Value;
		return needle.IsAlive;
	}
}
