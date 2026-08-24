using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace System.Runtime.CompilerServices;

public static class ContractHelper
{
	internal const int Cor_E_Codecontractfailed = -2146233022;

	private static readonly object _lockObject = new object();

	private static volatile EventHandler<ContractFailedEventArgs> _contractFailedEvent;

	internal static event EventHandler<ContractFailedEventArgs> InternalContractFailed
	{
		[SecurityCritical]
		add
		{
			lock (_lockObject)
			{
				_contractFailedEvent = (EventHandler<ContractFailedEventArgs>)Delegate.Combine(_contractFailedEvent, value);
			}
		}
		[SecurityCritical]
		remove
		{
			lock (_lockObject)
			{
				_contractFailedEvent = (EventHandler<ContractFailedEventArgs>)Delegate.Remove(_contractFailedEvent, value);
			}
		}
	}

	[DebuggerNonUserCode]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public static string RaiseContractFailedEvent(ContractFailureKind failureKind, string userMessage, string conditionText, Exception innerException)
	{
		string resultFailureMessage = "Contract failed";
		RaiseContractFailedEventImplementation(failureKind, userMessage, conditionText, innerException, ref resultFailureMessage);
		return resultFailureMessage;
	}

	[DebuggerNonUserCode]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public static void TriggerFailure(ContractFailureKind kind, string displayMessage, string userMessage, string conditionText, Exception innerException)
	{
		TriggerFailureImplementation(kind, displayMessage, userMessage, conditionText, innerException);
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	private static string GetDisplayMessage(ContractFailureKind failureKind, string userMessage, string conditionText)
	{
		string failureMessage = ContractHelperEx.GetFailureMessage(failureKind, conditionText);
		if (!string.IsNullOrEmpty(userMessage))
		{
			return failureMessage + "  " + userMessage;
		}
		return failureMessage;
	}

	[SecuritySafeCritical]
	[DebuggerNonUserCode]
	private static void RaiseContractFailedEventImplementation(ContractFailureKind failureKind, string userMessage, string conditionText, Exception innerException, ref string resultFailureMessage)
	{
		if (failureKind < ContractFailureKind.Precondition || failureKind > ContractFailureKind.Assume)
		{
			throw new ArgumentException($"Invalid enum value: {failureKind}", "failureKind");
		}
		string text = "contract failed.";
		ContractFailedEventArgs e = null;
		RuntimeHelpers.PrepareConstrainedRegions();
		string text2;
		try
		{
			text = GetDisplayMessage(failureKind, userMessage, conditionText);
			EventHandler<ContractFailedEventArgs> contractFailedEvent = _contractFailedEvent;
			if (contractFailedEvent != null)
			{
				e = new ContractFailedEventArgs(failureKind, text, conditionText, innerException);
				Delegate[] invocationList = contractFailedEvent.GetInvocationList();
				foreach (Delegate obj in invocationList)
				{
					EventHandler<ContractFailedEventArgs> eventHandler = (EventHandler<ContractFailedEventArgs>)obj;
					try
					{
						eventHandler(null, e);
					}
					catch (Exception obj2)
					{
						GC.KeepAlive(obj2);
						e.SetUnwind();
					}
				}
				if (e.Unwind)
				{
					throw new System.Diagnostics.Contracts.ContractException(failureKind, text, userMessage, conditionText, innerException);
				}
			}
		}
		finally
		{
			text2 = ((e == null || !e.Handled) ? text : null);
		}
		resultFailureMessage = text2;
	}

	[DebuggerNonUserCode]
	[SecuritySafeCritical]
	private static void TriggerFailureImplementation(ContractFailureKind kind, string displayMessage, string userMessage, string conditionText, Exception innerException)
	{
		if (!Environment.UserInteractive)
		{
			throw new System.Diagnostics.Contracts.ContractException(kind, displayMessage, userMessage, conditionText, innerException);
		}
		ContractHelperEx.Fail(displayMessage);
	}
}
