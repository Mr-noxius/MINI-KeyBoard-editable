using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System;

[Serializable]
public sealed class WeakReference<T> : ISerializable where T : class
{
	private readonly bool _trackResurrection;

	private GCHandle _handle;

	public WeakReference(T target)
		: this(target, false)
	{
	}

	public WeakReference(T target, bool trackResurrection)
	{
		_trackResurrection = trackResurrection;
		SetTarget(target);
	}

	internal WeakReference(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		T target = (T)info.GetValue("TrackedObject", typeof(T));
		_trackResurrection = info.GetBoolean("TrackResurrection");
		SetTarget(target);
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		TryGetTarget(out var target);
		info.AddValue("TrackedObject", target, typeof(T));
		info.AddValue("TrackResurrection", _trackResurrection);
	}

	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	public void SetTarget(T value)
	{
		GCHandle handle = _handle;
		_handle = GetNewHandle(value, _trackResurrection);
		if (handle.IsAllocated)
		{
			handle.Free();
			try
			{
				handle.Free();
			}
			catch (InvalidOperationException obj)
			{
				GC.KeepAlive(obj);
			}
		}
		_handle = GetNewHandle(value, _trackResurrection);
	}

	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	public bool TryGetTarget(out T target)
	{
		target = null;
		if (!_handle.IsAllocated)
		{
			return false;
		}
		try
		{
			object target2 = _handle.Target;
			if (target2 == null)
			{
				return false;
			}
			target = target2 as T;
			return true;
		}
		catch (InvalidOperationException)
		{
			return false;
		}
	}

	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	private GCHandle GetNewHandle(T value, bool trackResurrection)
	{
		return GCHandle.Alloc(value, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
	}
}
