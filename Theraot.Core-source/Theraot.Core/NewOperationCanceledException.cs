using System;
using System.Runtime.Serialization;

namespace Theraot.Core;

[Serializable]
public class NewOperationCanceledException : OperationCanceledException
{
	public NewOperationCanceledException()
	{
	}

	public NewOperationCanceledException(string message)
		: base(message)
	{
	}

	public NewOperationCanceledException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected NewOperationCanceledException(SerializationInfo info, StreamingContext scheduler)
		: base(info, scheduler)
	{
	}
}
