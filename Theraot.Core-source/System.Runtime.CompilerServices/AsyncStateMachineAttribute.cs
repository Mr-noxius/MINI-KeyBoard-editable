namespace System.Runtime.CompilerServices;

[Serializable]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class AsyncStateMachineAttribute : StateMachineAttribute
{
	public AsyncStateMachineAttribute(Type stateMachineType)
		: base(stateMachineType)
	{
	}
}
