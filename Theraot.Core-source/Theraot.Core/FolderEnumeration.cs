using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Core;

[DebuggerNonUserCode]
public static class FolderEnumeration
{
	public static IEnumerable<string> GetFiles(string folder, string pattern)
	{
		IEnumerable<string> enumerable = null;
		try
		{
			enumerable = Directory.EnumerateFiles(folder, pattern, SearchOption.TopDirectoryOnly);
		}
		catch (DirectoryNotFoundException)
		{
		}
		catch (UnauthorizedAccessException)
		{
		}
		return enumerable ?? ((IEnumerable<string>)ArrayReservoir<string>.EmptyArray);
	}

	public static IEnumerable<string> GetFilesAndFoldersRecursive(string folder, string pattern)
	{
		IEnumerable<IEnumerable<string>> source = GraphHelper.ExploreBreadthFirstTree(folder, GetFolders, (string current) => current.AsUnaryEnumerable().Concat(GetFiles(current, pattern)));
		return GetFiles(folder, pattern).Concat(source.Flatten());
	}

	public static IEnumerable<string> GetFilesRecursive(string folder, string pattern)
	{
		IEnumerable<IEnumerable<string>> source = GraphHelper.ExploreBreadthFirstTree(folder, GetFolders, (string current) => GetFiles(current, pattern));
		return GetFiles(folder, pattern).Concat(source.Flatten());
	}

	public static IEnumerable<string> GetFolders(string folder)
	{
		try
		{
			IEnumerable<string> source = Directory.EnumerateDirectories(folder);
			return source.Where((string subFolder) => (File.GetAttributes(subFolder) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint);
		}
		catch
		{
			return ArrayReservoir<string>.EmptyArray;
		}
	}

	public static IEnumerable<string> GetFoldersRecursive(string folder)
	{
		return GraphHelper.ExploreBreadthFirstTree(folder, GetFolders);
	}
}
