namespace System.Reflection;

public static class RuntimeReflectionExtensions
{
	public static MethodInfo GetMethodInfo(this Delegate del)
	{
		return del.Method;
	}
}
