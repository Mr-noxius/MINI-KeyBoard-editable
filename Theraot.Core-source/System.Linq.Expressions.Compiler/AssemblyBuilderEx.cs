using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace System.Linq.Expressions.Compiler;

internal static class AssemblyBuilderEx
{
	public static AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, CustomAttributeBuilder[] assemblyAttributes)
	{
		return Thread.GetDomain().DefineDynamicAssembly(name, access, assemblyAttributes);
	}
}
