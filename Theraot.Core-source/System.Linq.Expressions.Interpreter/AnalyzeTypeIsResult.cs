namespace System.Linq.Expressions.Interpreter;

internal enum AnalyzeTypeIsResult
{
	KnownFalse,
	KnownTrue,
	KnownAssignable,
	Unknown
}
