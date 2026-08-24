using System;
using System.Diagnostics;
using System.Reflection;
using Theraot.Core;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public sealed class WeakDelegateNeedle : WeakNeedle<Delegate>, IEquatable<Delegate>, IEquatable<WeakDelegateNeedle>
{
	public MethodInfo Method
	{
		get
		{
			Delegate value = Value;
			if (base.IsAlive)
			{
				return RuntimeReflectionExtensions.GetMethodInfo(value);
			}
			return null;
		}
	}

	public WeakDelegateNeedle(Delegate handler)
		: base(Check.NotNullArgument(handler, "handler"))
	{
		if ((object)handler == null)
		{
			throw new ArgumentNullException("handler");
		}
	}

	public WeakDelegateNeedle(MethodInfo methodInfo, object target)
		: base(BuildDelegate(methodInfo, target))
	{
	}

	public bool Equals(Delegate other)
	{
		MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo(other);
		if (!object.ReferenceEquals(null, other))
		{
			return Equals(methodInfo, other.Target);
		}
		return false;
	}

	public bool Equals(MethodInfo method, object target)
	{
		Delegate value = Value;
		if (base.IsAlive)
		{
			if (RuntimeReflectionExtensions.GetMethodInfo(value).Equals(method))
			{
				return object.ReferenceEquals(value.Target, target);
			}
			return false;
		}
		return false;
	}

	public bool Equals(WeakDelegateNeedle other)
	{
		if (object.ReferenceEquals(null, other))
		{
			return false;
		}
		Delegate value = Value;
		if (base.IsAlive)
		{
			Delegate value2 = other.Value;
			if (other.IsAlive)
			{
				MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo(value2);
				if (RuntimeReflectionExtensions.GetMethodInfo(value).Equals(methodInfo))
				{
					return object.ReferenceEquals(value.Target, value2.Target);
				}
				return false;
			}
			return false;
		}
		return !other.IsAlive;
	}

	public void Invoke(object[] args)
	{
		TryInvoke(args);
	}

	public bool TryInvoke(object[] args)
	{
		Delegate value = Value;
		if (base.IsAlive)
		{
			value.DynamicInvoke(args);
			return true;
		}
		return false;
	}

	public bool TryInvoke(object[] args, out object result)
	{
		Delegate value = Value;
		if (base.IsAlive)
		{
			result = value.DynamicInvoke(args);
			return true;
		}
		result = null;
		return false;
	}

	public bool TryInvoke<TResult>(object[] args, out TResult result)
	{
		Delegate value = Value;
		if (base.IsAlive)
		{
			result = (TResult)value.DynamicInvoke(args);
			return true;
		}
		result = default(TResult);
		return false;
	}

	private static Delegate BuildDelegate(MethodInfo methodInfo, object target)
	{
		if (object.ReferenceEquals(methodInfo, null))
		{
			throw new ArgumentNullException("methodInfo");
		}
		if (methodInfo.IsStatic != object.ReferenceEquals(null, target))
		{
			if (object.ReferenceEquals(target, null))
			{
				throw new ArgumentNullException("target", "target is null and the method is not static.");
			}
			throw new ArgumentException("target is not null and the method is static", "target");
		}
		Type declaringType = methodInfo.DeclaringType;
		if (object.ReferenceEquals(declaringType, null))
		{
			throw new ArgumentException("methodInfo.DeclaringType is null", "methodInfo");
		}
		return TypeHelper.CreateDelegate(methodInfo, declaringType, target);
	}
}
