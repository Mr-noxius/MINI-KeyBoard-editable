using System;
using System.IO;

namespace Theraot.Core;

public static class StreamExtensions
{
	private const int _defaultBufferSize = 4096;

	public static void CopyTo(Stream input, Stream output)
	{
		input.CopyTo(output);
	}

	public static void CopyTo(Stream input, Stream output, int bufferSize)
	{
		input.CopyTo(output, bufferSize);
	}

	public static bool IsDisposed(this Stream stream)
	{
		try
		{
			stream.Seek(0L, SeekOrigin.Current);
			return false;
		}
		catch (ObjectDisposedException)
		{
			return true;
		}
	}

	public static void ReadComplete(this Stream stream, byte[] buffer, int offset, int length)
	{
		if (object.ReferenceEquals(stream, null))
		{
			throw new ArgumentNullException("stream");
		}
		while (length > 0)
		{
			int num = stream.Read(buffer, offset, length);
			if (num <= 0)
			{
				throw new EndOfStreamException();
			}
			length -= num;
			offset += num;
		}
	}

	public static byte[] ToArray(this Stream stream)
	{
		if (object.ReferenceEquals(stream, null))
		{
			throw new ArgumentNullException("stream");
		}
		if (stream is MemoryStream memoryStream)
		{
			return memoryStream.ToArray();
		}
		using MemoryStream memoryStream2 = new MemoryStream();
		stream.CopyTo(memoryStream2);
		return memoryStream2.ToArray();
	}
}
