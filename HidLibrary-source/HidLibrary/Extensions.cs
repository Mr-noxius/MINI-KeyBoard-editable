using System.Text;

namespace HidLibrary;

public static class Extensions
{
	public static string ToUTF8String(this byte[] buffer)
	{
		string text = Encoding.UTF8.GetString(buffer);
		return text.Remove(text.IndexOf('\0'));
	}

	public static string ToUTF16String(this byte[] buffer)
	{
		string text = Encoding.Unicode.GetString(buffer);
		return text.Remove(text.IndexOf('\0'));
	}
}
