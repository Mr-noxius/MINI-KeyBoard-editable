using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security;

namespace System.Runtime.CompilerServices;

internal static class ContractHelperEx
{
	[SecuritySafeCritical]
	internal static void Fail(string message)
	{
		if (Debugger.IsAttached)
		{
			Debugger.Break();
		}
		else
		{
			Environment.FailFast(message);
		}
	}

	internal static string GetFailureMessage(ContractFailureKind failureKind)
	{
		return GetFailureMessage(failureKind, "");
	}

	internal static string GetFailureMessage(ContractFailureKind failureKind, string conditionText)
	{
		bool flag = !string.IsNullOrEmpty(conditionText);
		return failureKind switch
		{
			ContractFailureKind.Assert => flag ? $"Assertion failed: {conditionText}" : "Assertion failed.", 
			ContractFailureKind.Assume => flag ? $"Assumption failed: {conditionText}" : "Assumption failed.", 
			ContractFailureKind.Precondition => flag ? $"Precondition failed: {conditionText}" : "Precondition failed.", 
			ContractFailureKind.Postcondition => flag ? $"Postcondition failed: {conditionText}" : "Postcondition failed.", 
			ContractFailureKind.Invariant => flag ? $"Invariant failed: {conditionText}" : "Invariant failed.", 
			ContractFailureKind.PostconditionOnException => flag ? $"Postcondition failed after throwing an exception: {conditionText}" : "Postcondition failed after throwing an exception.", 
			_ => "Assumption failed.", 
		};
	}
}
