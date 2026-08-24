namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
public sealed class AsyncMethodBuilderAttribute : Attribute
{
	private readonly Type _builderType;

	public Type BuilderType => _builderType;

	public AsyncMethodBuilderAttribute(Type builderType)
	{
		_builderType = builderType;
	}
}
