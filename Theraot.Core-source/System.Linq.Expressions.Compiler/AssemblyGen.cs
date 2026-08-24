using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Text;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace System.Linq.Expressions.Compiler;

internal sealed class AssemblyGen
{
	private static System.Linq.Expressions.Compiler.AssemblyGen _assembly;

	private readonly AssemblyBuilder _assemblyBuilder;

	private readonly ModuleBuilder _moduleBuilder;

	private int _index;

	private static System.Linq.Expressions.Compiler.AssemblyGen Assembly
	{
		get
		{
			if (_assembly == null)
			{
				Interlocked.CompareExchange(ref _assembly, new System.Linq.Expressions.Compiler.AssemblyGen(), null);
			}
			return _assembly;
		}
	}

	private AssemblyGen()
	{
		AssemblyName assemblyName = new AssemblyName("Snippets");
		CustomAttributeBuilder[] assemblyAttributes = new CustomAttributeBuilder[1]
		{
			new CustomAttributeBuilder(typeof(SecurityTransparentAttribute).GetConstructor(Type.EmptyTypes), ArrayReservoir<object>.EmptyArray)
		};
		_assemblyBuilder = AssemblyBuilderEx.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run, assemblyAttributes);
		_moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName.Name);
	}

	private TypeBuilder DefineType(string name, Type parent, TypeAttributes attr)
	{
		System.Dynamic.Utils.ContractUtils.RequiresNotNull(name, "name");
		System.Dynamic.Utils.ContractUtils.RequiresNotNull(parent, "parent");
		StringBuilder stringBuilder = new StringBuilder(name);
		int value = Interlocked.Increment(ref _index);
		stringBuilder.Append("$");
		stringBuilder.Append(value);
		stringBuilder.Replace('+', '_').Replace('[', '_').Replace(']', '_')
			.Replace('*', '_')
			.Replace('&', '_')
			.Replace(',', '_')
			.Replace('\\', '_');
		name = stringBuilder.ToString();
		return _moduleBuilder.DefineType(name, attr, parent);
	}

	internal static TypeBuilder DefineDelegateType(string name)
	{
		return Assembly.DefineType(name, typeof(MulticastDelegate), TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoClass);
	}
}
