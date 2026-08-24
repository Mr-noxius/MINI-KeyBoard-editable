using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.Diagnostics.Contracts;

[Serializable]
internal sealed class ContractException : Exception
{
	private readonly ContractFailureKind _kind;

	private readonly string _userMessage;

	private readonly string _condition;

	public ContractFailureKind Kind => _kind;

	public string Failure => Message;

	public string UserMessage => _userMessage;

	public string Condition => _condition;

	private ContractException()
	{
		base.HResult = -2146233022;
	}

	public ContractException(ContractFailureKind kind, string failure, string userMessage, string condition, Exception innerException)
		: base(failure, innerException)
	{
		base.HResult = -2146233022;
		_kind = kind;
		_userMessage = userMessage;
		_condition = condition;
	}

	private ContractException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		_kind = (ContractFailureKind)info.GetInt32("Kind");
		_userMessage = info.GetString("UserMessage");
		_condition = info.GetString("Condition");
	}

	[SecurityCritical]
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("Kind", _kind);
		info.AddValue("UserMessage", _userMessage);
		info.AddValue("Condition", _condition);
	}
}
