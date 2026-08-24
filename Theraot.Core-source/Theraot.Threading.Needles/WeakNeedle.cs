using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public class WeakNeedle<T> : IEquatable<WeakNeedle<T>>, IRecyclableNeedle<T>, ICacheNeedle<T>, INeedle<T>, IReadOnlyNeedle<T>, IPromise where T : class
{
	private readonly int _hashCode;

	private readonly bool _trackResurrection;

	private volatile bool _faultExpected;

	private GCHandle _handle;

	private int _managedDisposal;

	private int _status;

	public Exception Exception
	{
		get
		{
			if (ReadTarget(out var target) && target is Exception result && _faultExpected)
			{
				return result;
			}
			return null;
		}
	}

	bool IPromise.IsCanceled => false;

	bool IPromise.IsCompleted => true;

	public bool IsAlive
	{
		get
		{
			if (ReadTarget(out var target) && target is T && !_faultExpected)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsFaulted
	{
		get
		{
			if (ReadTarget(out var target) && target is Exception && _faultExpected)
			{
				return true;
			}
			return false;
		}
	}

	public virtual bool TrackResurrection => _trackResurrection;

	public virtual T Value
	{
		[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
		get
		{
			if (ReadTarget(out var target) && target is T result && !_faultExpected)
			{
				return result;
			}
			return null;
		}
		[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
		set
		{
			SetTargetValue(value);
		}
	}

	public bool IsDisposed
	{
		[DebuggerNonUserCode]
		get
		{
			return _status == -1;
		}
	}

	public WeakNeedle()
		: this(false)
	{
	}

	public WeakNeedle(bool trackResurrection)
	{
		_trackResurrection = trackResurrection;
		_hashCode = base.GetHashCode();
	}

	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	public WeakNeedle(T target)
		: this(target, false)
	{
	}

	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	public WeakNeedle(T target, bool trackResurrection)
	{
		if (target == null)
		{
			_hashCode = base.GetHashCode();
		}
		else
		{
			SetTargetValue(target);
			_hashCode = target.GetHashCode();
		}
		_trackResurrection = trackResurrection;
	}

	public static explicit operator T(WeakNeedle<T> needle)
	{
		if (needle == null)
		{
			throw new ArgumentNullException("needle");
		}
		return needle.Value;
	}

	public static implicit operator WeakNeedle<T>(T field)
	{
		return new WeakNeedle<T>(field);
	}

	public static bool operator !=(WeakNeedle<T> left, WeakNeedle<T> right)
	{
		return NotEqualsExtracted(left, right);
	}

	public static bool operator ==(WeakNeedle<T> left, WeakNeedle<T> right)
	{
		return EqualsExtracted(left, right);
	}

	public sealed override bool Equals(object obj)
	{
		WeakNeedle<T> weakNeedle = obj as WeakNeedle<T>;
		if (weakNeedle != null)
		{
			return EqualsExtractedExtracted(this, weakNeedle);
		}
		if (obj is T y)
		{
			T value = Value;
			if (IsAlive)
			{
				return EqualityComparer<T>.Default.Equals(value, y);
			}
			return false;
		}
		return false;
	}

	public bool Equals(WeakNeedle<T> other)
	{
		if (!object.ReferenceEquals(other, null))
		{
			return EqualsExtractedExtracted(this, other);
		}
		return false;
	}

	public void Free()
	{
		Dispose();
	}

	public sealed override int GetHashCode()
	{
		return _hashCode;
	}

	public override string ToString()
	{
		T value = Value;
		if (IsAlive)
		{
			return value.ToString();
		}
		return "<Dead Needle>";
	}

	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	public virtual bool TryGetValue(out T value)
	{
		value = null;
		if (ReadTarget(out var target) && target is T val)
		{
			value = val;
			return true;
		}
		return false;
	}

	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	protected void SetTargetError(Exception error)
	{
		_faultExpected = true;
		WriteTarget(error);
	}

	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	protected void SetTargetValue(T value)
	{
		_faultExpected = false;
		WriteTarget(value);
	}

	private static bool EqualsExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
	{
		if (object.ReferenceEquals(left, null))
		{
			return object.ReferenceEquals(right, null);
		}
		if (!object.ReferenceEquals(right, null))
		{
			return EqualsExtractedExtracted(left, right);
		}
		return false;
	}

	private static bool EqualsExtractedExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
	{
		T value = left.Value;
		if (left.IsAlive)
		{
			T value2 = right.Value;
			if (right.IsAlive)
			{
				return EqualityComparer<T>.Default.Equals(value, value2);
			}
			return false;
		}
		return !right.IsAlive;
	}

	private static bool NotEqualsExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
	{
		if (object.ReferenceEquals(left, null))
		{
			return !object.ReferenceEquals(right, null);
		}
		if (!object.ReferenceEquals(right, null))
		{
			return NotEqualsExtractedExtracted(left, right);
		}
		return true;
	}

	private static bool NotEqualsExtractedExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
	{
		T value = left.Value;
		if (left.IsAlive)
		{
			T value2 = right.Value;
			if (right.IsAlive)
			{
				return !EqualityComparer<T>.Default.Equals(value, value2);
			}
			return true;
		}
		return right.IsAlive;
	}

	private bool ReadTarget(out object target)
	{
		target = null;
		if (_handle.IsAllocated)
		{
			try
			{
				target = _handle.Target;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	private void ReleaseExtracted()
	{
		if (_handle.IsAllocated)
		{
			try
			{
				_handle.Free();
			}
			catch (InvalidOperationException)
			{
			}
		}
	}

	private void ReportManagedDisposal()
	{
		Volatile.Write(ref _managedDisposal, 1);
	}

	private void WriteTarget(object target)
	{
		if (_status == -1 || !ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
		{
			ReleaseExtracted();
			_handle = GCHandle.Alloc(target, (!_trackResurrection) ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
			if (Interlocked.CompareExchange(ref _managedDisposal, 0, 1) == 1)
			{
				GC.ReRegisterForFinalize(this);
			}
			UnDispose();
			return;
		}
		try
		{
			GCHandle handle = _handle;
			if (handle.IsAllocated)
			{
				try
				{
					handle.Target = target;
					return;
				}
				catch (InvalidOperationException)
				{
					_handle = GCHandle.Alloc(target, (!_trackResurrection) ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
				}
			}
			else
			{
				_handle = GCHandle.Alloc(target, (!_trackResurrection) ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
			}
			if (handle.IsAllocated)
			{
				handle.Free();
				try
				{
					handle.Free();
					return;
				}
				catch (InvalidOperationException)
				{
					return;
				}
			}
		}
		finally
		{
			Interlocked.Decrement(ref _status);
		}
	}

	[DebuggerNonUserCode]
	~WeakNeedle()
	{
		try
		{
		}
		finally
		{
			Dispose(disposeManagedResources: false);
		}
	}

	[DebuggerNonUserCode]
	public void Dispose()
	{
		try
		{
			Dispose(disposeManagedResources: true);
		}
		finally
		{
			GC.SuppressFinalize(this);
		}
	}

	[DebuggerNonUserCode]
	public void DisposedConditional(Action whenDisposed, Action whenNotDisposed)
	{
		if (_status == -1)
		{
			whenDisposed?.Invoke();
		}
		else
		{
			if (whenNotDisposed == null)
			{
				return;
			}
			if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
			{
				try
				{
					whenNotDisposed();
					return;
				}
				finally
				{
					Interlocked.Decrement(ref _status);
				}
			}
			whenDisposed?.Invoke();
		}
	}

	[DebuggerNonUserCode]
	public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
	{
		if (_status == -1)
		{
			if (whenDisposed == null)
			{
				return default(TReturn);
			}
			return whenDisposed();
		}
		if (whenNotDisposed == null)
		{
			return default(TReturn);
		}
		if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
		{
			try
			{
				return whenNotDisposed();
			}
			finally
			{
				Interlocked.Decrement(ref _status);
			}
		}
		if (whenDisposed == null)
		{
			return default(TReturn);
		}
		return whenDisposed();
	}

	[DebuggerNonUserCode]
	protected virtual void Dispose(bool disposeManagedResources)
	{
		try
		{
			if (!TakeDisposalExecution())
			{
				return;
			}
			try
			{
				if (disposeManagedResources)
				{
					ReportManagedDisposal();
				}
			}
			finally
			{
				ReleaseExtracted();
			}
		}
		catch (Exception obj)
		{
			GC.KeepAlive(obj);
		}
	}

	[DebuggerNonUserCode]
	protected void ProtectedCheckDisposed(string exceptionMessegeWhenDisposed)
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException(exceptionMessegeWhenDisposed);
		}
	}

	protected bool TakeDisposalExecution()
	{
		if (_status == -1)
		{
			return false;
		}
		return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
	}

	[DebuggerNonUserCode]
	protected void ThrowDisposedexception()
	{
		throw new ObjectDisposedException(GetType().FullName);
	}

	[DebuggerNonUserCode]
	protected TReturn ThrowDisposedexception<TReturn>()
	{
		throw new ObjectDisposedException(GetType().FullName);
	}

	[DebuggerNonUserCode]
	protected bool UnDispose()
	{
		if (Volatile.Read(ref _status) == -1)
		{
			Volatile.Write(ref _status, 0);
			return true;
		}
		return false;
	}
}
