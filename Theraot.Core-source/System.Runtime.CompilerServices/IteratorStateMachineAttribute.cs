namespace System.Runtime.CompilerServices;

[Serializable]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class IteratorStateMachineAttribute : StateMachineAttribute
{
	public IteratorStateMachineAttribute(Type stateMachineType)
		: base(stateMachineType)
	{
	}
}
