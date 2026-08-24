using System.Reflection;
using System.Text;

namespace System.Runtime.ExceptionServices;

public sealed class ExceptionDispatchInfo
{
	private static FieldInfo _remoteStackTraceString;

	private readonly Exception _exception;

	private readonly object _stackTraceOriginal;

	private readonly object _stackTrace;

	public Exception SourceException => _exception;

	private ExceptionDispatchInfo(Exception exception)
	{
		_exception = exception;
		_stackTraceOriginal = _exception.StackTrace;
		_stackTrace = _exception.StackTrace;
		if (_stackTrace != null)
		{
			object stackTrace = _stackTrace;
			_stackTrace = string.Concat(stackTrace, Environment.NewLine, "---End of stack trace from previous location where exception was thrown ---", Environment.NewLine);
		}
		else
		{
			_stackTrace = string.Empty;
		}
	}

	public static ExceptionDispatchInfo Capture(Exception source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return new ExceptionDispatchInfo(source);
	}

	private static FieldInfo GetFieldInfo()
	{
		if (_remoteStackTraceString == null)
		{
			FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic) ?? typeof(Exception).GetField("remote_stack_trace", BindingFlags.Instance | BindingFlags.NonPublic);
			_remoteStackTraceString = remoteStackTraceString;
		}
		return _remoteStackTraceString;
	}

	private static void SetStackTrace(Exception exception, object value)
	{
		FieldInfo fieldInfo = GetFieldInfo();
		fieldInfo.SetValue(exception, value);
	}

	public void Throw()
	{
		try
		{
			throw _exception;
		}
		catch (Exception obj)
		{
			GC.KeepAlive(obj);
			string value = string.Concat(_stackTrace, BuildStackTrace(Environment.StackTrace));
			SetStackTrace(_exception, value);
			throw;
		}
	}

	private string BuildStackTrace(string trace)
	{
		string[] array = trace.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Contains(":"))
			{
				if (text.Contains("System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()"))
				{
					break;
				}
				if (flag)
				{
					stringBuilder.Append(Environment.NewLine);
				}
				flag = true;
				stringBuilder.Append(text);
			}
			else if (flag)
			{
				break;
			}
		}
		return stringBuilder.ToString();
	}
}
