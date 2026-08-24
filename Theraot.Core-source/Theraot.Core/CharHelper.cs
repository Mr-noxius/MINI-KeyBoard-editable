namespace Theraot.Core;

public static class CharHelper
{
	private static readonly string _classicWhitespace = "\t\n\v\f\r ";

	public static bool IsClassicWhitespace(char character)
	{
		return _classicWhitespace.IndexOf(character) >= 0;
	}
}
