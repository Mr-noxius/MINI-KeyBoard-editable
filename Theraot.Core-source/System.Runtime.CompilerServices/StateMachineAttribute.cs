namespace System.Runtime.CompilerServices;

[Serializable]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class StateMachineAttribute : Attribute
{
	public Type StateMachineType { get; private set; }

	public StateMachineAttribute(Type stateMachineType)
	{
		StateMachineType = stateMachineType;
	}
}
