using System;
using System.IO;
using Theraot.Collections;

namespace Theraot.Core;

public static class PathHelper
{
	private static readonly string _directorySeparatorString = Path.DirectorySeparatorChar.ToString();

	private static readonly string _altDirectorySeparatorString = Path.AltDirectorySeparatorChar.ToString();

	private static readonly string _volumeSeparatorString = Path.VolumeSeparatorChar.ToString();

	public static string DirectorySeparatorString => _directorySeparatorString;

	public static string AltDirectorySeparatorString => _altDirectorySeparatorString;

	public static string VolumeSeparatorString => _volumeSeparatorString;

	public static string Combine(params string[] paths)
	{
		return Path.Combine(paths);
	}

	public static string Combine(string path1, string path2)
	{
		return Path.Combine(path1, path2);
	}

	public static string Combine(string path1, string path2, string path3)
	{
		return Path.Combine(path1, path2, path3);
	}

	public static string Combine(string path1, string path2, string path3, string path4)
	{
		return Path.Combine(path1, path2, path3, path4);
	}

	public static bool HasInvalidPathChars(string path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		if (path == string.Empty)
		{
			return false;
		}
		return path.ContainsAny(Path.GetInvalidPathChars());
	}

	public static bool HasInvalidFileNameChars(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		return fileName.ContainsAny(Path.GetInvalidFileNameChars());
	}
}
