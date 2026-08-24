using System;
using System.Diagnostics;

namespace Theraot.Core;

[DebuggerNonUserCode]
public static class ActionHelper
{
	private static class HelperNullAction
	{
		private static readonly Action _instance;

		public static Action Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction()
		{
		}
	}

	private static class HelperNullAction<T>
	{
		private static readonly Action<T> _instance;

		public static Action<T> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T obj)
		{
		}
	}

	private static class HelperNullAction<T1, T2>
	{
		private static readonly Action<T1, T2> _instance;

		public static Action<T1, T2> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3>
	{
		private static readonly Action<T1, T2, T3> _instance;

		public static Action<T1, T2, T3> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4>
	{
		private static readonly Action<T1, T2, T3, T4> _instance;

		public static Action<T1, T2, T3, T4> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5>
	{
		private static readonly Action<T1, T2, T3, T4, T5> _instance;

		public static Action<T1, T2, T3, T4, T5> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6> _instance;

		public static Action<T1, T2, T3, T4, T5, T6> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6, T7> _instance;

		public static Action<T1, T2, T3, T4, T5, T6, T7> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6, T7, T8> _instance;

		public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> _instance;

		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> _instance;

		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> _instance;

		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> _instance;

		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> _instance;

		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> _instance;

		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> _instance;

		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
		}
	}

	private static class HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
	{
		private static readonly Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> _instance;

		public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Instance => _instance;

		static HelperNullAction()
		{
			_instance = NullAction;
		}

		private static void NullAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
		}
	}

	public static Action GetNoopAction()
	{
		return HelperNullAction.Instance;
	}

	public static Action<T> GetNoopAction<T>()
	{
		return HelperNullAction<T>.Instance;
	}

	public static Action<T1, T2> GetNoopAction<T1, T2>()
	{
		return HelperNullAction<T1, T2>.Instance;
	}

	public static Action<T1, T2, T3> GetNoopAction<T1, T2, T3>()
	{
		return HelperNullAction<T1, T2, T3>.Instance;
	}

	public static Action<T1, T2, T3, T4> GetNoopAction<T1, T2, T3, T4>()
	{
		return HelperNullAction<T1, T2, T3, T4>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5> GetNoopAction<T1, T2, T3, T4, T5>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6> GetNoopAction<T1, T2, T3, T4, T5, T6>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7> GetNoopAction<T1, T2, T3, T4, T5, T6, T7>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Instance;
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> GetNoopAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>()
	{
		return HelperNullAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Instance;
	}

	public static Action GetThrowAction(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T> GetThrowAction<T>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2> GetThrowAction<T1, T2>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3> GetThrowAction<T1, T2, T3>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4> GetThrowAction<T1, T2, T3, T4>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5> GetThrowAction<T1, T2, T3, T4, T5>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6> GetThrowAction<T1, T2, T3, T4, T5, T6>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7> GetThrowAction<T1, T2, T3, T4, T5, T6, T7>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8> GetThrowAction<T1, T2, T3, T4, T5, T6, T7, T8>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> GetThrowAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> GetThrowAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GetThrowAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GetThrowAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> GetThrowAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> GetThrowAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> GetThrowAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}

	public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> GetThrowAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Exception exception)
	{
		return delegate
		{
			throw exception;
		};
	}
}
