namespace System.Linq.Expressions.Compiler;

internal enum AnalyzeTypeIsResult
{
	KnownFalse,
	KnownTrue,
	KnownAssignable,
	Unknown
}
