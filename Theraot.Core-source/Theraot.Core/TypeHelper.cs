using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Core;

[DebuggerNonUserCode]
public static class TypeHelper
{
	private static class ConstructorHelper<TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<TReturn> _create;

		private static readonly Func<TReturn> _createOrDefault;

		private static readonly Func<TReturn> _createOrFail;

		public static Func<TReturn> Create => _create;

		public static Func<TReturn> CreateOrDefault => _createOrDefault;

		public static Func<TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] emptyTypes = Type.EmptyTypes;
			_constructorInfo = typeof(TReturn).GetConstructor(emptyTypes);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor()
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with no type arguments.", new object[1] { typeof(TReturn) }));
			}
			return (TReturn)_constructorInfo.Invoke(EmptyObjects);
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T, TReturn> _create;

		private static readonly Func<T, TReturn> _createOrDefault;

		private static readonly Func<T, TReturn> _createOrFail;

		public static Func<T, TReturn> Create => _create;

		public static Func<T, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[1] { typeof(T) };
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T obj)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type argument {1}", new object[2]
				{
					typeof(TReturn),
					typeof(T).Name
				}));
			}
			return (TReturn)_constructorInfo.Invoke(new object[1] { obj });
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, TReturn> _create;

		private static readonly Func<T1, T2, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, TReturn> _createOrFail;

		public static Func<T1, T2, TReturn> Create => _create;

		public static Func<T1, T2, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[2]
			{
				typeof(T1),
				typeof(T2)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}", new object[3]
				{
					typeof(TReturn),
					typeof(T1).Name,
					typeof(T2).Name
				}));
			}
			return (TReturn)_constructorInfo.Invoke(new object[2] { arg1, arg2 });
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, TReturn> _create;

		private static readonly Func<T1, T2, T3, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, TReturn> _createOrFail;

		public static Func<T1, T2, T3, TReturn> Create => _create;

		public static Func<T1, T2, T3, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[3]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[3] { arg1, arg2, arg3 });
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[4]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[4] { arg1, arg2, arg3, arg4 });
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[5]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[5] { arg1, arg2, arg3, arg4, arg5 });
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[6]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[6] { arg1, arg2, arg3, arg4, arg5, arg6 });
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, T7, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, T7, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, T7, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[7]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6),
				typeof(T7)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[7] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[8]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6),
				typeof(T7),
				typeof(T8)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[8] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[9]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6),
				typeof(T7),
				typeof(T8),
				typeof(T9)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[9] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[10]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6),
				typeof(T7),
				typeof(T8),
				typeof(T9),
				typeof(T10)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[10] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[11]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6),
				typeof(T7),
				typeof(T8),
				typeof(T9),
				typeof(T10),
				typeof(T11)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[11]
			{
				arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10,
				arg11
			});
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[12]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6),
				typeof(T7),
				typeof(T8),
				typeof(T9),
				typeof(T10),
				typeof(T11),
				typeof(T12)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[12]
			{
				arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10,
				arg11, arg12
			});
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[13]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6),
				typeof(T7),
				typeof(T8),
				typeof(T9),
				typeof(T10),
				typeof(T11),
				typeof(T12),
				typeof(T13)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[13]
			{
				arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10,
				arg11, arg12, arg13
			});
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[14]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6),
				typeof(T7),
				typeof(T8),
				typeof(T9),
				typeof(T10),
				typeof(T11),
				typeof(T12),
				typeof(T13),
				typeof(T14)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name, typeof(T14).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[14]
			{
				arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10,
				arg11, arg12, arg13, arg14
			});
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[15]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6),
				typeof(T7),
				typeof(T8),
				typeof(T9),
				typeof(T10),
				typeof(T11),
				typeof(T12),
				typeof(T13),
				typeof(T14),
				typeof(T15)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name, typeof(T14).Name, typeof(T15).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[15]
			{
				arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10,
				arg11, arg12, arg13, arg14, arg15
			});
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>
	{
		private static readonly ConstructorInfo _constructorInfo;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> _create;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> _createOrDefault;

		private static readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> _createOrFail;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> Create => _create;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> CreateOrDefault => _createOrDefault;

		public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> CreateOrFail => _createOrFail;

		public static bool HasConstructor => _constructorInfo != null;

		static ConstructorHelper()
		{
			Type[] types = new Type[16]
			{
				typeof(T1),
				typeof(T2),
				typeof(T3),
				typeof(T4),
				typeof(T5),
				typeof(T6),
				typeof(T7),
				typeof(T8),
				typeof(T9),
				typeof(T10),
				typeof(T11),
				typeof(T12),
				typeof(T13),
				typeof(T14),
				typeof(T15),
				typeof(T16)
			};
			_constructorInfo = typeof(TReturn).GetConstructor(types);
			_create = InvokeConstructor;
			if (HasConstructor)
			{
				_createOrDefault = _create;
				_createOrFail = _create;
			}
			else
			{
				_createOrDefault = FuncHelper.GetDefaultFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>();
				Type typeInfo = IntrospectionExtensions.GetTypeInfo(typeof(TReturn));
				_createOrFail = (typeInfo.IsValueType ? _createOrDefault : FuncHelper.GetThrowFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>(CreateMissingMemberException()));
			}
		}

		public static TReturn InvokeConstructor(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			if (_constructorInfo == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name, typeof(T14).Name, typeof(T15).Name, typeof(T16).Name));
			}
			return (TReturn)_constructorInfo.Invoke(new object[16]
			{
				arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10,
				arg11, arg12, arg13, arg14, arg15, arg16
			});
		}

		private static MissingMemberException CreateMissingMemberException()
		{
			return new MissingMemberException();
		}
	}

	private static class BinaryPortableInfo<T>
	{
		private static readonly bool _result;

		public static bool Result => _result;

		static BinaryPortableInfo()
		{
			_result = GetBinaryPortableResult(typeof(T));
		}
	}

	private static class BlittableInfo<T>
	{
		private static readonly bool _result;

		public static bool Result => _result;

		static BlittableInfo()
		{
			_result = GetBlittableResult(typeof(T));
		}
	}

	private static class ValueTypeRecursiveInfo<T>
	{
		private static readonly bool _result;

		public static bool Result => _result;

		static ValueTypeRecursiveInfo()
		{
			_result = GetValueTypeRecursiveResult(typeof(T));
		}
	}

	private static readonly Type[] _known;

	private static readonly Assembly[] _knownAssembies;

	public static object[] EmptyObjects => ArrayReservoir<object>.EmptyArray;

	public static bool AreReferenceAssignable(Type target, Type source)
	{
		if (target == source)
		{
			return true;
		}
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(target);
		Type typeInfo2 = IntrospectionExtensions.GetTypeInfo(source);
		if (!typeInfo.IsValueType && !typeInfo2.IsValueType && target.IsAssignableFrom(source))
		{
			return true;
		}
		return false;
	}

	static TypeHelper()
	{
		_known = new Type[18]
		{
			typeof(object),
			typeof(BitConverter),
			typeof(StructuralComparisons),
			typeof(CancelEventArgs),
			typeof(Console),
			typeof(Debug),
			typeof(BufferedStream),
			typeof(File),
			typeof(FileAccess),
			typeof(ResourceReader),
			typeof(IStrongBox),
			typeof(AsnEncodedData),
			typeof(AsymmetricAlgorithm),
			typeof(IIdentity),
			typeof(BarrierPostPhaseException),
			typeof(TaskExtensions),
			typeof(Uri),
			typeof(TypeHelper)
		};
		List<Assembly> list = new List<Assembly>();
		Type[] known = _known;
		foreach (Type type in known)
		{
			Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
			Assembly assembly = typeInfo.Assembly;
			if (!list.Contains(assembly))
			{
				list.Add(assembly);
			}
		}
		_knownAssembies = list.ToArray();
	}

	public static MethodInfo FindConversionOperator(MethodInfo[] methods, Type typeFrom, Type typeTo, bool implicitOnly)
	{
		foreach (MethodInfo methodInfo in methods)
		{
			if ((!(methodInfo.Name != "op_Implicit") || (!implicitOnly && !(methodInfo.Name != "op_Explicit"))) && !(methodInfo.ReturnType != typeTo))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (!(parameters[0].ParameterType != typeFrom))
				{
					return methodInfo;
				}
			}
		}
		return null;
	}

	public static Type FindGenericType(Type definition, Type type)
	{
		while (type != null && type != typeof(object))
		{
			if (type.IsConstructedGenericType() && type.GetGenericTypeDefinition() == definition)
			{
				return type;
			}
			Type typeInfo = IntrospectionExtensions.GetTypeInfo(definition);
			Type typeInfo2 = IntrospectionExtensions.GetTypeInfo(type);
			if (typeInfo.IsInterface)
			{
				Type[] interfaces = typeInfo2.GetInterfaces();
				foreach (Type type2 in interfaces)
				{
					Type type3 = FindGenericType(definition, type2);
					if (type3 != null)
					{
						return type3;
					}
				}
			}
			type = typeInfo2.BaseType;
		}
		return null;
	}

	public static MethodInfo GetStaticMethod(this Type type, string name)
	{
		MethodInfo[] methods = type.GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.Name == name && methodInfo.IsStatic)
			{
				return methodInfo;
			}
		}
		return null;
	}

	public static MethodInfo GetStaticMethod(this Type type, string name, Type[] types)
	{
		MethodInfo[] methods = type.GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.Name == name && methodInfo.IsStatic && methodInfo.MatchesArgumentTypes(types))
			{
				return methodInfo;
			}
		}
		return null;
	}

	public static MethodInfo GetBooleanOperator(Type type, string name)
	{
		do
		{
			MethodInfo staticMethod = type.GetStaticMethod(name, new Type[1] { type });
			if (staticMethod != null && staticMethod.IsSpecialName && !staticMethod.ContainsGenericParameters)
			{
				return staticMethod;
			}
			Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
			type = typeInfo.BaseType;
		}
		while (type != null);
		return null;
	}

	public static MethodInfo[] GetMethodsIgnoreCase(this Type type, BindingFlags flags, string name)
	{
		List<MethodInfo> list = new List<MethodInfo>();
		MethodInfo[] methods = type.GetMethods(flags);
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
			{
				list.Add(methodInfo);
			}
		}
		return list.ToArray();
	}

	public static Type GetNonNullableType(this Type type)
	{
		if (type.IsNullableType())
		{
			return type.GetGenericArguments()[0];
		}
		return type;
	}

	public static Type GetNonRefType(this Type type)
	{
		if (!type.IsByRef)
		{
			return type;
		}
		return type.GetElementType();
	}

	public static Type GetNullableType(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsValueType && !type.IsNullableType())
		{
			return typeof(Nullable<>).MakeGenericType(type);
		}
		return type;
	}

	public static MethodInfo[] GetStaticMethods(this Type type)
	{
		List<MethodInfo> list = new List<MethodInfo>();
		MethodInfo[] methods = type.GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.IsStatic)
			{
				list.Add(methodInfo);
			}
		}
		return list.ToArray();
	}

	public static TypeCode GetTypeCode(this Type type)
	{
		if (type == null)
		{
			return TypeCode.Empty;
		}
		while (true)
		{
			Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
			if (!typeInfo.IsEnum)
			{
				break;
			}
			type = Enum.GetUnderlyingType(type);
		}
		if (type == typeof(bool))
		{
			return TypeCode.Boolean;
		}
		if (type == typeof(char))
		{
			return TypeCode.Char;
		}
		if (type == typeof(sbyte))
		{
			return TypeCode.SByte;
		}
		if (type == typeof(byte))
		{
			return TypeCode.Byte;
		}
		if (type == typeof(short))
		{
			return TypeCode.Int16;
		}
		if (type == typeof(ushort))
		{
			return TypeCode.UInt16;
		}
		if (type == typeof(int))
		{
			return TypeCode.Int32;
		}
		if (type == typeof(uint))
		{
			return TypeCode.UInt32;
		}
		if (type == typeof(long))
		{
			return TypeCode.Int64;
		}
		if (type == typeof(ulong))
		{
			return TypeCode.UInt64;
		}
		if (type == typeof(float))
		{
			return TypeCode.Single;
		}
		if (type == typeof(double))
		{
			return TypeCode.Double;
		}
		if (type == typeof(decimal))
		{
			return TypeCode.Decimal;
		}
		if (type == typeof(DateTime))
		{
			return TypeCode.DateTime;
		}
		if (type == typeof(string))
		{
			return TypeCode.String;
		}
		return TypeCode.Object;
	}

	public static MethodInfo GetUserDefinedConversionMethod(Type source, Type target, bool implicitOnly)
	{
		Type nonNullableType = source.GetNonNullableType();
		Type nonNullableType2 = target.GetNonNullableType();
		MethodInfo[] array = null;
		MethodInfo[] array2 = null;
		if (nonNullableType == source)
		{
			if (nonNullableType2 == target)
			{
				return FindConversionOperator(array = nonNullableType.GetStaticMethods(), source, target, implicitOnly) ?? FindConversionOperator(array2 = nonNullableType2.GetStaticMethods(), source, target, implicitOnly);
			}
			return FindConversionOperator(array = nonNullableType.GetStaticMethods(), source, target, implicitOnly) ?? FindConversionOperator(array2 = nonNullableType2.GetStaticMethods(), source, target, implicitOnly) ?? FindConversionOperator(array, source, nonNullableType2, implicitOnly) ?? FindConversionOperator(array2, source, nonNullableType2, implicitOnly);
		}
		if (nonNullableType2 == target)
		{
			return FindConversionOperator(array = nonNullableType.GetStaticMethods(), source, target, implicitOnly) ?? FindConversionOperator(array2 = nonNullableType2.GetStaticMethods(), source, target, implicitOnly) ?? FindConversionOperator(array, nonNullableType, target, implicitOnly) ?? FindConversionOperator(array2, nonNullableType, target, implicitOnly);
		}
		return FindConversionOperator(array = nonNullableType.GetStaticMethods(), source, target, implicitOnly) ?? FindConversionOperator(array2 = nonNullableType2.GetStaticMethods(), source, target, implicitOnly) ?? FindConversionOperator(array, nonNullableType, target, implicitOnly) ?? FindConversionOperator(array2, nonNullableType, target, implicitOnly) ?? FindConversionOperator(array, source, nonNullableType2, implicitOnly) ?? FindConversionOperator(array2, source, nonNullableType2, implicitOnly) ?? FindConversionOperator(array, nonNullableType, nonNullableType2, implicitOnly) ?? FindConversionOperator(array2, nonNullableType, nonNullableType2, implicitOnly);
	}

	public static bool HasBuiltInEqualityOperator(Type left, Type right)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(left);
		Type typeInfo2 = IntrospectionExtensions.GetTypeInfo(right);
		if (typeInfo.IsInterface && !typeInfo2.IsValueType)
		{
			return true;
		}
		if (typeInfo2.IsInterface && !typeInfo.IsValueType)
		{
			return true;
		}
		if (!typeInfo.IsValueType && !typeInfo2.IsValueType && (left.IsReferenceAssignableFrom(right) || right.IsReferenceAssignableFrom(left)))
		{
			return true;
		}
		if (left != right)
		{
			return false;
		}
		Type nonNullableType = left.GetNonNullableType();
		Type typeInfo3 = IntrospectionExtensions.GetTypeInfo(nonNullableType);
		if (nonNullableType == typeof(bool) || nonNullableType.IsNumeric() || typeInfo3.IsEnum)
		{
			return true;
		}
		return false;
	}

	public static bool HasIdentityPrimitiveOrNullableConversion(Type source, Type target)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (source == target)
		{
			return true;
		}
		if (source.IsNullableType() && target == source.GetNonNullableType())
		{
			return true;
		}
		if (target.IsNullableType() && source == target.GetNonNullableType())
		{
			return true;
		}
		if (IsConvertible(source) && IsConvertible(target) && target.GetNonNullableType() != typeof(bool))
		{
			return true;
		}
		return false;
	}

	public static bool HasReferenceConversion(Type source, Type target)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (source == typeof(void) || target == typeof(void))
		{
			return false;
		}
		Type nonNullableType = source.GetNonNullableType();
		Type nonNullableType2 = target.GetNonNullableType();
		if (nonNullableType.IsAssignableFrom(nonNullableType2))
		{
			return true;
		}
		if (nonNullableType2.IsAssignableFrom(nonNullableType))
		{
			return true;
		}
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(source);
		Type typeInfo2 = IntrospectionExtensions.GetTypeInfo(target);
		if (typeInfo.IsInterface || typeInfo2.IsInterface)
		{
			return true;
		}
		if (IsLegalExplicitVariantDelegateConversion(source, target))
		{
			return true;
		}
		if (source == typeof(object) || target == typeof(object))
		{
			return true;
		}
		return false;
	}

	public static bool HasReferenceEquality(Type left, Type right)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(left);
		Type typeInfo2 = IntrospectionExtensions.GetTypeInfo(right);
		if (typeInfo.IsValueType || typeInfo2.IsValueType)
		{
			return false;
		}
		if (!typeInfo.IsInterface && !typeInfo2.IsInterface && !left.IsReferenceAssignableFrom(right))
		{
			return right.IsReferenceAssignableFrom(left);
		}
		return true;
	}

	public static bool IsArithmetic(this Type type)
	{
		type = type.GetNonNullableType();
		if (type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(float) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
		{
			return true;
		}
		return false;
	}

	public static bool IsBool(this Type type)
	{
		return type.GetNonNullableType() == typeof(bool);
	}

	public static bool IsConstructedGenericType(this Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsGenericType)
		{
			return !typeInfo.IsGenericTypeDefinition;
		}
		return false;
	}

	public static bool IsContravariant(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return PrivateIsContravariant(type);
	}

	public static bool IsConvertible(Type type)
	{
		type = type.GetNonNullableType();
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsEnum)
		{
			return true;
		}
		if (type == typeof(bool) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong) || type == typeof(float) || type == typeof(double) || type == typeof(char))
		{
			return true;
		}
		return false;
	}

	public static bool IsCovariant(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return PrivateIsCovariant(type);
	}

	public static bool IsDelegate(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return PrivateIsDelegate(type);
	}

	public static bool IsImplicitBoxingConversion(Type source, Type target)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(source);
		if (typeInfo.IsValueType && (target == typeof(object) || target == typeof(ValueType)))
		{
			return true;
		}
		if (typeInfo.IsEnum && target == typeof(Enum))
		{
			return true;
		}
		return false;
	}

	public static bool IsImplicitlyConvertible(Type source, Type target)
	{
		if (!(source == target) && !IsImplicitNumericConversion(source, target) && !IsImplicitReferenceConversion(source, target) && !IsImplicitBoxingConversion(source, target))
		{
			return IsImplicitNullableConversion(source, target);
		}
		return true;
	}

	public static bool IsImplicitNullableConversion(Type source, Type target)
	{
		if (target.IsNullableType())
		{
			return IsImplicitlyConvertible(source.GetNonNullableType(), target.GetNonNullableType());
		}
		return false;
	}

	public static bool IsImplicitNumericConversion(Type source, Type target)
	{
		if (source == typeof(sbyte))
		{
			if (target == typeof(short) || target == typeof(int) || target == typeof(long) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(byte))
		{
			if (target == typeof(short) || target == typeof(ushort) || target == typeof(int) || target == typeof(uint) || target == typeof(long) || target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(short))
		{
			if (target == typeof(int) || target == typeof(long) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(ushort))
		{
			if (target == typeof(int) || target == typeof(uint) || target == typeof(long) || target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(int))
		{
			if (target == typeof(long) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(uint))
		{
			if (target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(long) || target == typeof(ulong))
		{
			if (target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(char))
		{
			if (target == typeof(ushort) || target == typeof(int) || target == typeof(uint) || target == typeof(long) || target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
			{
				return true;
			}
		}
		else if (source == typeof(float))
		{
			return target == typeof(double);
		}
		return false;
	}

	public static bool IsImplicitReferenceConversion(Type source, Type target)
	{
		return target.IsAssignableFrom(source);
	}

	public static bool IsInteger(this Type type)
	{
		type = type.GetNonNullableType();
		return type.IsPrimitiveInteger();
	}

	public static bool IsIntegerOrBool(this Type type)
	{
		type = type.GetNonNullableType();
		if (type == typeof(bool) || type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
		{
			return true;
		}
		return false;
	}

	public static bool IsInvariant(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return PrivateIsInvariant(type);
	}

	public static bool IsLegalExplicitVariantDelegateConversion(Type source, Type target)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(source);
		Type typeInfo2 = IntrospectionExtensions.GetTypeInfo(target);
		if (!PrivateIsDelegate(source) || !PrivateIsDelegate(target) || !typeInfo.IsGenericType || !typeInfo2.IsGenericType)
		{
			return false;
		}
		Type genericTypeDefinition = source.GetGenericTypeDefinition();
		if (target.GetGenericTypeDefinition() != genericTypeDefinition || genericTypeDefinition == null)
		{
			return false;
		}
		Type[] genericArguments = genericTypeDefinition.GetGenericArguments();
		Type[] genericArguments2 = source.GetGenericArguments();
		Type[] genericArguments3 = target.GetGenericArguments();
		for (int i = 0; i < genericArguments.Length; i++)
		{
			Type type = genericArguments2[i];
			Type type2 = genericArguments3[i];
			if (type == type2)
			{
				continue;
			}
			Type type3 = genericArguments[i];
			if (PrivateIsInvariant(type3))
			{
				return false;
			}
			if (PrivateIsCovariant(type3))
			{
				if (!HasReferenceConversion(type, type2))
				{
					return false;
				}
			}
			else if (PrivateIsContravariant(type3))
			{
				Type typeInfo3 = IntrospectionExtensions.GetTypeInfo(type);
				Type typeInfo4 = IntrospectionExtensions.GetTypeInfo(type2);
				if (typeInfo3.IsValueType || typeInfo4.IsValueType)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool IsNullableType(this Type type)
	{
		if (type.IsConstructedGenericType())
		{
			return type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		return false;
	}

	public static bool IsNumeric(this Type type)
	{
		type = type.GetNonNullableType();
		if (type == typeof(char) || type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(float) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
		{
			return true;
		}
		return false;
	}

	internal static bool IsUnsignedInteger(this Type type)
	{
		type = type.GetNonNullableType();
		if (type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
		{
			return true;
		}
		return false;
	}

	public static bool IsValidInstanceType(MemberInfo member, Type instanceType)
	{
		Type declaringType = member.DeclaringType;
		if (declaringType == null)
		{
			return false;
		}
		if (declaringType.IsReferenceAssignableFrom(instanceType))
		{
			return true;
		}
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(instanceType);
		if (typeInfo.IsValueType)
		{
			if (declaringType.IsReferenceAssignableFrom(typeof(object)))
			{
				return true;
			}
			if (declaringType.IsReferenceAssignableFrom(typeof(ValueType)))
			{
				return true;
			}
			if (typeInfo.IsEnum && declaringType.IsReferenceAssignableFrom(typeof(Enum)))
			{
				return true;
			}
			Type typeInfo2 = IntrospectionExtensions.GetTypeInfo(declaringType);
			if (typeInfo2.IsInterface)
			{
				Type[] interfaces = instanceType.GetInterfaces();
				foreach (Type source in interfaces)
				{
					if (declaringType.IsReferenceAssignableFrom(source))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool IsReferenceAssignableFrom(this Type type, Type source)
	{
		if (type == source)
		{
			return true;
		}
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		Type typeInfo2 = IntrospectionExtensions.GetTypeInfo(source);
		if (!typeInfo.IsValueType && !typeInfo2.IsValueType && type.IsAssignableFrom(source))
		{
			return true;
		}
		return false;
	}

	public static bool MatchesArgumentTypes(this MethodInfo method, Type[] argTypes)
	{
		if (method == null || argTypes == null)
		{
			return false;
		}
		ParameterInfo[] parameters = method.GetParameters();
		if (parameters.Length != argTypes.Length)
		{
			return false;
		}
		for (int i = 0; i < parameters.Length; i++)
		{
			if (!parameters[i].ParameterType.IsReferenceAssignableFrom(argTypes[i]))
			{
				return false;
			}
		}
		return true;
	}

	internal static bool IsByRefParameter(this ParameterInfo pi)
	{
		if (pi.ParameterType.IsByRef)
		{
			return true;
		}
		return (pi.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out;
	}

	internal static bool IsFloatingPoint(this Type type)
	{
		type = type.GetNonNullableType();
		if (type == typeof(float) || type == typeof(double))
		{
			return true;
		}
		return false;
	}

	internal static bool IsUnsigned(this Type type)
	{
		type = type.GetNonNullableType();
		if (type == typeof(byte) || type == typeof(char) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
		{
			return true;
		}
		return false;
	}

	internal static void ValidateType(Type type)
	{
		if (type != typeof(void))
		{
			Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
			if (typeInfo.IsGenericTypeDefinition)
			{
				throw new ArgumentException("type is Generic");
			}
			if (typeInfo.ContainsGenericParameters)
			{
				throw new ArgumentException("type contains generic parameters.");
			}
		}
	}

	private static bool PrivateIsContravariant(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return 0 != (typeInfo.GenericParameterAttributes & GenericParameterAttributes.Contravariant);
	}

	private static bool PrivateIsCovariant(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return 0 != (typeInfo.GenericParameterAttributes & GenericParameterAttributes.Covariant);
	}

	private static bool PrivateIsDelegate(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return typeInfo.IsSubclassOf(typeof(MulticastDelegate));
	}

	private static bool PrivateIsInvariant(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return 0 == (typeInfo.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
	}

	public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType)
	{
		DynamicMethod dynamicMethod = methodInfo as DynamicMethod;
		if (dynamicMethod != null)
		{
			return dynamicMethod.CreateDelegate(delegateType);
		}
		return Delegate.CreateDelegate(delegateType, methodInfo);
	}

	public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType, object target)
	{
		DynamicMethod dynamicMethod = methodInfo as DynamicMethod;
		if (dynamicMethod != null)
		{
			return dynamicMethod.CreateDelegate(delegateType, target);
		}
		return Delegate.CreateDelegate(delegateType, target, methodInfo);
	}

	public static TReturn Create<TReturn>()
	{
		if (ConstructorHelper<TReturn>.HasConstructor)
		{
			return ConstructorHelper<TReturn>.InvokeConstructor();
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with no type arguments.", new object[1] { typeof(TReturn) }));
	}

	public static TReturn CreateOrDefault<TReturn>()
	{
		return ConstructorHelper<TReturn>.CreateOrDefault();
	}

	public static TReturn CreateOrFail<TReturn>()
	{
		return ConstructorHelper<TReturn>.CreateOrFail();
	}

	public static Func<TReturn> GetCreate<TReturn>()
	{
		if (ConstructorHelper<TReturn>.HasConstructor)
		{
			return ConstructorHelper<TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with no type arguments.", new object[1] { typeof(TReturn) }));
	}

	public static bool TryGetCreate<TReturn>(out Func<TReturn> create)
	{
		if (ConstructorHelper<TReturn>.HasConstructor)
		{
			create = ConstructorHelper<TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<TReturn> GetCreateOrDefault<TReturn>()
	{
		return ConstructorHelper<TReturn>.CreateOrDefault;
	}

	public static Func<TReturn> GetCreateOrFail<TReturn>()
	{
		return ConstructorHelper<TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<TReturn>()
	{
		return ConstructorHelper<TReturn>.HasConstructor;
	}

	public static TReturn Create<T, TReturn>(T obj)
	{
		if (ConstructorHelper<T, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T, TReturn>.InvokeConstructor(obj);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type argument {1}", new object[2]
		{
			typeof(TReturn),
			typeof(T).Name
		}));
	}

	public static TReturn CreateOrDefault<T, TReturn>(T obj)
	{
		return ConstructorHelper<T, TReturn>.CreateOrDefault(obj);
	}

	public static TReturn CreateOrFail<T, TReturn>(T obj)
	{
		return ConstructorHelper<T, TReturn>.CreateOrFail(obj);
	}

	public static Func<T, TReturn> GetCreate<T, TReturn>()
	{
		if (ConstructorHelper<T, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type argument {1}", new object[2]
		{
			typeof(TReturn),
			typeof(T).Name
		}));
	}

	public static bool TryGetCreate<T, TReturn>(out Func<T, TReturn> create)
	{
		if (ConstructorHelper<T, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T, TReturn> GetCreateOrDefault<T, TReturn>()
	{
		return ConstructorHelper<T, TReturn>.CreateOrDefault;
	}

	public static Func<T, TReturn> GetCreateOrFail<T, TReturn>()
	{
		return ConstructorHelper<T, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T, TReturn>()
	{
		return ConstructorHelper<T, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, TReturn>(T1 arg1, T2 arg2)
	{
		if (ConstructorHelper<T1, T2, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, TReturn>.InvokeConstructor(arg1, arg2);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}", new object[3]
		{
			typeof(TReturn),
			typeof(T1).Name,
			typeof(T2).Name
		}));
	}

	public static TReturn CreateOrDefault<T1, T2, TReturn>(T1 arg1, T2 arg2)
	{
		return ConstructorHelper<T1, T2, TReturn>.CreateOrDefault(arg1, arg2);
	}

	public static TReturn CreateOrFail<T1, T2, TReturn>(T1 arg1, T2 arg2)
	{
		return ConstructorHelper<T1, T2, TReturn>.CreateOrFail(arg1, arg2);
	}

	public static Func<T1, T2, TReturn> GetCreate<T1, T2, TReturn>()
	{
		if (ConstructorHelper<T1, T2, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}", new object[3]
		{
			typeof(TReturn),
			typeof(T1).Name,
			typeof(T2).Name
		}));
	}

	public static bool TryGetCreate<T1, T2, TReturn>(out Func<T1, T2, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, TReturn> GetCreateOrDefault<T1, T2, TReturn>()
	{
		return ConstructorHelper<T1, T2, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, TReturn> GetCreateOrFail<T1, T2, TReturn>()
	{
		return ConstructorHelper<T1, T2, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, TReturn>()
	{
		return ConstructorHelper<T1, T2, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, TReturn>(T1 arg1, T2 arg2, T3 arg3)
	{
		if (ConstructorHelper<T1, T2, T3, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, TReturn>.InvokeConstructor(arg1, arg2, arg3);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, TReturn>(T1 arg1, T2 arg2, T3 arg3)
	{
		return ConstructorHelper<T1, T2, T3, TReturn>.CreateOrDefault(arg1, arg2, arg3);
	}

	public static TReturn CreateOrFail<T1, T2, T3, TReturn>(T1 arg1, T2 arg2, T3 arg3)
	{
		return ConstructorHelper<T1, T2, T3, TReturn>.CreateOrFail(arg1, arg2, arg3);
	}

	public static Func<T1, T2, T3, TReturn> GetCreate<T1, T2, T3, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, TReturn>(out Func<T1, T2, T3, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, TReturn> GetCreateOrDefault<T1, T2, T3, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, TReturn> GetCreateOrFail<T1, T2, T3, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (ConstructorHelper<T1, T2, T3, T4, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		return ConstructorHelper<T1, T2, T3, T4, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		return ConstructorHelper<T1, T2, T3, T4, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4);
	}

	public static Func<T1, T2, T3, T4, TReturn> GetCreate<T1, T2, T3, T4, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, TReturn>(out Func<T1, T2, T3, T4, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, TReturn> GetCreateOrDefault<T1, T2, T3, T4, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, TReturn> GetCreateOrFail<T1, T2, T3, T4, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5);
	}

	public static Func<T1, T2, T3, T4, T5, TReturn> GetCreate<T1, T2, T3, T4, T5, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, TReturn>(out Func<T1, T2, T3, T4, T5, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6);
	}

	public static Func<T1, T2, T3, T4, T5, T6, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, TReturn>(out Func<T1, T2, T3, T4, T5, T6, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, T7, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, T7, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, T7, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, T7, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, T7, TReturn>(out Func<T1, T2, T3, T4, T5, T6, T7, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, T7, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, T7, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, T7, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(out Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(out Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(out Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(out Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(out Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(out Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name, typeof(T14).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name, typeof(T14).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(out Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name, typeof(T14).Name, typeof(T15).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name, typeof(T14).Name, typeof(T15).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(out Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>.HasConstructor;
	}

	public static TReturn Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.InvokeConstructor(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name, typeof(T14).Name, typeof(T15).Name, typeof(T16).Name));
	}

	public static TReturn CreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.CreateOrDefault(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
	}

	public static TReturn CreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.CreateOrFail(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> GetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>()
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.HasConstructor)
		{
			return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.Create;
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no constructor for {0} with the type arguments {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}", typeof(TReturn), typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5).Name, typeof(T6).Name, typeof(T7).Name, typeof(T8).Name, typeof(T9).Name, typeof(T10).Name, typeof(T11).Name, typeof(T12).Name, typeof(T13).Name, typeof(T14).Name, typeof(T15).Name, typeof(T16).Name));
	}

	public static bool TryGetCreate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>(out Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> create)
	{
		if (ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.HasConstructor)
		{
			create = ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.Create;
			return true;
		}
		create = null;
		return false;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> GetCreateOrDefault<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.CreateOrDefault;
	}

	public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> GetCreateOrFail<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.CreateOrFail;
	}

	public static bool HasConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>()
	{
		return ConstructorHelper<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>.HasConstructor;
	}

	public static TTarget As<TTarget>(object source) where TTarget : class
	{
		return As(source, (Func<TTarget>)delegate
		{
			throw new InvalidOperationException("Cannot convert to " + typeof(TTarget).Name);
		});
	}

	public static TTarget As<TTarget>(object source, TTarget def) where TTarget : class
	{
		return As(source, () => def);
	}

	public static TTarget As<TTarget>(object source, Func<TTarget> alternative) where TTarget : class
	{
		if (alternative == null)
		{
			throw new ArgumentNullException("alternative");
		}
		TTarget val = source as TTarget;
		if (val == null)
		{
			return alternative();
		}
		return val;
	}

	public static bool CanBe<T>(this Type type, T value)
	{
		if (object.ReferenceEquals(value, null))
		{
			return type.CanBeNull();
		}
		return value.GetType().IsAssignableTo(type);
	}

	public static bool CanBeNull(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsValueType)
		{
			return !object.ReferenceEquals(Nullable.GetUnderlyingType(type), null);
		}
		return true;
	}

	public static TTarget Cast<TTarget>(object source)
	{
		return Cast(source, (Func<TTarget>)delegate
		{
			throw new InvalidOperationException("Cannot convert to " + typeof(TTarget).Name);
		});
	}

	public static TTarget Cast<TTarget>(object source, TTarget def)
	{
		return Cast(source, () => def);
	}

	public static TTarget Cast<TTarget>(object source, Func<TTarget> alternative)
	{
		if (alternative == null)
		{
			throw new ArgumentNullException("alternative");
		}
		try
		{
			return (TTarget)source;
		}
		catch
		{
			return alternative();
		}
	}

	public static object Create(this Type type, params object[] arguments)
	{
		return Activator.CreateInstance(type, arguments);
	}

	public static TReturn Default<TReturn>()
	{
		return FuncHelper.GetDefaultFunc<TReturn>()();
	}

	public static TAttribute[] GetAttributes<TAttribute>(this ICustomAttributeProvider item, bool inherit) where TAttribute : Attribute
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute), inherit);
	}

	public static TAttribute[] GetAttributes<TAttribute>(this Type type, bool inherit) where TAttribute : Attribute
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		return (TAttribute[])typeInfo.GetCustomAttributes(typeof(TAttribute), inherit);
	}

	public static Func<TReturn> GetDefault<TReturn>()
	{
		return FuncHelper.GetDefaultFunc<TReturn>();
	}

	public static MethodInfo GetDelegateMethodInfo(Type delegateType)
	{
		if (delegateType == null)
		{
			throw new ArgumentNullException("delegateType");
		}
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(delegateType);
		if (typeInfo.BaseType != typeof(MulticastDelegate))
		{
			throw new ArgumentException("Not a delegate.");
		}
		MethodInfo method = delegateType.GetMethod("Invoke");
		if (method == null)
		{
			throw new ArgumentException("Not a delegate.");
		}
		return method;
	}

	public static ParameterInfo[] GetDelegateParameters(Type delegateType)
	{
		return GetDelegateMethodInfo(delegateType).GetParameters();
	}

	public static Type GetDelegateReturnType(Type delegateType)
	{
		return GetDelegateMethodInfo(delegateType).ReturnType;
	}

	public static Type GetNonRefType(this ParameterInfo parameterInfo)
	{
		Type type = parameterInfo.ParameterType;
		if (type.IsByRef)
		{
			type = type.GetElementType();
		}
		return type;
	}

	public static Type GetNotNullableType(this Type type)
	{
		Type underlyingType = Nullable.GetUnderlyingType(type);
		if (underlyingType == null)
		{
			return type;
		}
		return underlyingType;
	}

	public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider item) where TAttribute : Attribute
	{
		TAttribute[] attributes = item.GetAttributes<TAttribute>(inherit: true);
		if (attributes != null)
		{
			return attributes.Length > 0;
		}
		return false;
	}

	public static bool HasConstructor(this Type type, params Type[] typeArguments)
	{
		ConstructorInfo constructor = type.GetConstructor(typeArguments);
		return constructor == null;
	}

	public static bool IsArrayTypeAssignableTo(Type type, Type target)
	{
		if (!type.IsArray || !target.IsArray)
		{
			return false;
		}
		if (type.GetArrayRank() != target.GetArrayRank())
		{
			return false;
		}
		return type.GetElementType().IsAssignableTo(target.GetElementType());
	}

	public static bool IsArrayTypeAssignableToInterface(Type type, Type target)
	{
		if (!type.IsArray)
		{
			return false;
		}
		if (target.IsGenericInstanceOf(typeof(IList<>)) || target.IsGenericInstanceOf(typeof(ICollection<>)) || target.IsGenericInstanceOf(typeof(IEnumerable<>)))
		{
			return type.GetElementType() == target.GetGenericArguments()[0];
		}
		return false;
	}

	public static bool IsAssignableTo(this Type type, Type target)
	{
		if (!target.IsAssignableFrom(type) && !IsArrayTypeAssignableTo(type, target))
		{
			return IsArrayTypeAssignableToInterface(type, target);
		}
		return true;
	}

	public static bool IsAssignableTo(this Type type, ParameterInfo parameterInfo)
	{
		return type.GetNotNullableType().IsAssignableTo(parameterInfo.GetNonRefType());
	}

	public static bool IsAtomic(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (!typeInfo.IsClass)
		{
			if (typeInfo.IsPrimitive)
			{
				return Marshal.SizeOf(type) <= IntPtr.Size;
			}
			return false;
		}
		return true;
	}

	public static bool IsBinaryPortable(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return IsBinaryPortableExtracted(type);
	}

	public static bool IsBlittable(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return IsBlittableExtracted(type);
	}

	public static bool IsGenericImplementationOf(this Type type, Type interfaceGenericTypeDefinition)
	{
		Type[] interfaces = type.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			if (type2.IsGenericInstanceOf(interfaceGenericTypeDefinition))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsGenericImplementationOf(this Type type, params Type[] interfaceGenericTypeDefinitions)
	{
		Type[] interfaces = type.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			Type typeInfo = IntrospectionExtensions.GetTypeInfo(type2);
			if (typeInfo.IsGenericTypeDefinition)
			{
				Type match = type2.GetGenericTypeDefinition();
				if (Array.Exists(interfaceGenericTypeDefinitions, (Type item) => item == match))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsGenericImplementationOf(this Type type, out Type interfaceType, Type interfaceGenericTypeDefinition)
	{
		Type[] interfaces = type.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			if (type2.IsGenericInstanceOf(interfaceGenericTypeDefinition))
			{
				interfaceType = type2;
				return true;
			}
		}
		interfaceType = null;
		return false;
	}

	public static bool IsGenericImplementationOf(this Type type, out Type interfaceType, params Type[] interfaceGenericTypeDefinitions)
	{
		Type[] interfaces = type.GetInterfaces();
		foreach (Type currentInterface in interfaceGenericTypeDefinitions)
		{
			Predicate<Type> match = (Type item) => item.IsGenericInstanceOf(currentInterface);
			int num = Array.FindIndex(interfaces, match);
			if (num != -1)
			{
				interfaceType = interfaces[num];
				return true;
			}
		}
		interfaceType = null;
		return false;
	}

	public static bool IsGenericInstanceOf(this Type type, Type genericTypeDefinition)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (!typeInfo.IsGenericType)
		{
			return false;
		}
		return type.GetGenericTypeDefinition() == genericTypeDefinition;
	}

	public static bool IsImplementationOf(this Type type, Type interfaceType)
	{
		Type[] interfaces = type.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			if (type2 == interfaceType)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsImplementationOf(this Type type, params Type[] interfaceTypes)
	{
		Type[] interfaces = type.GetInterfaces();
		foreach (Type currentInterface in interfaces)
		{
			Predicate<Type> match = (Type item) => currentInterface == item;
			if (Array.Exists(interfaceTypes, match))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsImplementationOf(this Type type, out Type interfaceType, params Type[] interfaceTypes)
	{
		Type[] interfaces = type.GetInterfaces();
		foreach (Type currentInterface in interfaceTypes)
		{
			Predicate<Type> match = (Type item) => item == currentInterface;
			int num = Array.FindIndex(interfaces, match);
			if (num != -1)
			{
				interfaceType = interfaces[num];
				return true;
			}
		}
		interfaceType = null;
		return false;
	}

	public static bool IsNullable(this Type type)
	{
		return Nullable.GetUnderlyingType(type) != null;
	}

	public static bool IsPrimitiveInteger(this Type type)
	{
		if (type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
		{
			return true;
		}
		return false;
	}

	public static bool IsSameOrSubclassOf(this Type type, Type baseType)
	{
		if (type == baseType)
		{
			return true;
		}
		while (type != null)
		{
			Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
			type = typeInfo.BaseType;
			if (type == baseType)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsValueTypeRecursive(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return IsValueTypeRecursiveExtracted(type);
	}

	public static Type MakeNullableType(this Type self)
	{
		return typeof(Nullable<>).MakeGenericType(self);
	}

	internal static bool CanCache(this Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		Assembly assembly = typeInfo.Assembly;
		if (Array.IndexOf(_knownAssembies, assembly) == -1)
		{
			return false;
		}
		if (typeInfo.IsGenericType)
		{
			Type[] genericArguments = type.GetGenericArguments();
			foreach (Type type2 in genericArguments)
			{
				if (!type2.CanCache())
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool GetBinaryPortableResult(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsPrimitive)
		{
			if (type != typeof(IntPtr) && type != typeof(UIntPtr) && type != typeof(char))
			{
				return type != typeof(bool);
			}
			return false;
		}
		if (typeInfo.IsValueType)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!IsBinaryPortableExtracted(fieldInfo.FieldType))
				{
					return false;
				}
			}
			if (!typeInfo.IsAutoLayout)
			{
				return typeInfo.StructLayoutAttribute.Pack > 0;
			}
			return false;
		}
		return false;
	}

	private static bool GetBlittableResult(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsPrimitive)
		{
			if (type == typeof(char) || type == typeof(bool))
			{
				return false;
			}
			return true;
		}
		if (typeInfo.IsValueType)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!IsBlittableExtracted(fieldInfo.FieldType))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private static bool GetValueTypeRecursiveResult(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (typeInfo.IsPrimitive)
		{
			return true;
		}
		if (typeInfo.IsValueType)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!IsValueTypeRecursiveExtracted(fieldInfo.FieldType))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private static bool IsBinaryPortableExtracted(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (!typeInfo.IsValueType)
		{
			return false;
		}
		if (type.CanCache())
		{
			PropertyInfo property = typeof(BinaryPortableInfo<>).MakeGenericType(type).GetProperty("Result", BindingFlags.Static | BindingFlags.Public);
			return (bool)property.GetValue(null, null);
		}
		return GetBinaryPortableResult(type);
	}

	private static bool IsBlittableExtracted(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (!typeInfo.IsValueType)
		{
			return false;
		}
		if (type.CanCache())
		{
			PropertyInfo property = typeof(BlittableInfo<>).MakeGenericType(type).GetProperty("Result", BindingFlags.Static | BindingFlags.Public);
			return (bool)property.GetValue(null, null);
		}
		return GetBlittableResult(type);
	}

	private static bool IsValueTypeRecursiveExtracted(Type type)
	{
		Type typeInfo = IntrospectionExtensions.GetTypeInfo(type);
		if (!typeInfo.IsValueType)
		{
			return false;
		}
		if (type.CanCache())
		{
			PropertyInfo property = typeof(ValueTypeRecursiveInfo<>).MakeGenericType(type).GetProperty("Result", BindingFlags.Static | BindingFlags.Public);
			return (bool)property.GetValue(null, null);
		}
		return GetValueTypeRecursiveResult(type);
	}

	public static object GetValue(this PropertyInfo info, object obj)
	{
		return info.GetValue(obj, null);
	}

	public static void SetValue(this PropertyInfo info, object obj, object value)
	{
		info.SetValue(obj, value, null);
	}
}
