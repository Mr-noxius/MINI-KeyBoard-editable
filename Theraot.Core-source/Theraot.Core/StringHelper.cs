using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Theraot.Core;

[DebuggerNonUserCode]
public static class StringHelper
{
	public static string Append(this string text, string value)
	{
		return text + value;
	}

	public static string Append(this string text, string value1, string value2)
	{
		return text + value1 + value2;
	}

	public static string Append(this string text, string value1, string value2, string value3)
	{
		return text + value1 + value2 + value3;
	}

	public static string Append(this string text, params string[] values)
	{
		return text + values;
	}

	public static string Concat(params string[] value)
	{
		return string.Concat(value);
	}

	public static string Concat(string[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		return ConcatExtracted(array, arrayIndex, array.Length - arrayIndex);
	}

	public static string Concat(string[] array, int arrayIndex, int countLimit)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (countLimit < 0)
		{
			throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
		}
		if (countLimit > array.Length - arrayIndex)
		{
			throw new ArgumentException("startIndex plus countLimit is greater than the number of elements in array.", "array");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		return ConcatExtracted(array, arrayIndex, countLimit);
	}

	public static string Concat(params object[] values)
	{
		return string.Concat(values);
	}

	public static string Concat(object[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		return ConcatExtracted(array, arrayIndex, array.Length - arrayIndex);
	}

	public static string Concat(object[] array, int arrayIndex, int countLimit)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (countLimit < 0)
		{
			throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
		}
		if (countLimit > array.Length - arrayIndex)
		{
			throw new ArgumentException("startIndex plus countLimit is greater than the number of elements in array.", "array");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		return ConcatExtracted(array, arrayIndex, countLimit);
	}

	public static string Concat<T>(IEnumerable<T> values, Func<T, string> converter)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		List<string> list = new List<string>();
		int num = 0;
		foreach (T value in values)
		{
			string text = converter(value);
			list.Add(text);
			num += text.Length;
		}
		return ConcatExtractedExtracted(list.ToArray(), 0, list.Count, num);
	}

	public static string End(this string text, int characterCount)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		int length = text.Length;
		if (length < characterCount)
		{
			return text;
		}
		return text.Substring(length - characterCount);
	}

	public static string EnsureEnd(this string text, string end, StringComparison comparisonType)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (!text.EndsWith(end, comparisonType))
		{
			return text.Append(end);
		}
		return text;
	}

	public static string EnsureStart(this string text, string start)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (!text.StartsWith(start, StringComparison.CurrentCulture))
		{
			return start.Append(text);
		}
		return text;
	}

	public static string EnsureStart(this string text, string start, StringComparison comparisonType)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (!text.StartsWith(start, comparisonType))
		{
			return start.Append(text);
		}
		return text;
	}

	public static string ExceptEnd(this string text, int characterCount)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		int length = text.Length;
		if (length < characterCount)
		{
			return string.Empty;
		}
		return text.Substring(0, length - characterCount);
	}

	public static string ExceptStart(this string text, int characterCount)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		int length = text.Length;
		if (length < characterCount)
		{
			return string.Empty;
		}
		return text.Substring(characterCount);
	}

	public static string Implode(string separator, params object[] values)
	{
		if (separator == null)
		{
			return string.Concat(values);
		}
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		string[] array = new string[values.Length];
		int num = 0;
		foreach (object obj in values)
		{
			array[num++] = obj.ToString();
		}
		return ImplodeExtracted(separator, array, 0, array.Length);
	}

	public static string Implode(string separator, object[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		if (object.ReferenceEquals(separator, null))
		{
			separator = string.Empty;
		}
		return ImplodeExtracted(separator, array, arrayIndex, array.Length - arrayIndex);
	}

	public static string Implode(string separator, object[] array, int arrayIndex, int countLimit)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (countLimit < 0)
		{
			throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
		}
		if (countLimit > array.Length - arrayIndex)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		if (object.ReferenceEquals(separator, null))
		{
			separator = string.Empty;
		}
		return ImplodeExtracted(separator, array, arrayIndex, countLimit);
	}

	public static string Implode(string separator, params string[] value)
	{
		if (object.ReferenceEquals(value, null))
		{
			throw new ArgumentNullException("value");
		}
		if (object.ReferenceEquals(separator, null))
		{
			separator = string.Empty;
		}
		return ImplodeExtracted(separator, value, 0, value.Length);
	}

	public static string Implode(string separator, string[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		if (object.ReferenceEquals(separator, null))
		{
			separator = string.Empty;
		}
		return ImplodeExtracted(separator, array, arrayIndex, array.Length - arrayIndex);
	}

	public static string Implode(string separator, string[] array, int arrayIndex, int countLimit)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
		}
		if (countLimit < 0)
		{
			throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
		}
		if (countLimit > array.Length - arrayIndex)
		{
			throw new ArgumentException("The array can not contain the number of elements.", "array");
		}
		if (arrayIndex == array.Length)
		{
			return string.Empty;
		}
		if (object.ReferenceEquals(separator, null))
		{
			separator = string.Empty;
		}
		return ImplodeExtracted(separator, array, arrayIndex, countLimit);
	}

	public static string Implode(string separator, IEnumerable<string> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values);
		}
		List<string> list = new List<string>();
		foreach (string value in values)
		{
			list.Add(value);
		}
		return ImplodeExtracted(separator, list.ToArray(), 0, list.Count);
	}

	public static string Implode<T>(string separator, IEnumerable<T> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values);
		}
		List<string> list = new List<string>();
		foreach (T value in values)
		{
			list.Add(value.ToString());
		}
		return ImplodeExtracted(separator, list.ToArray(), 0, list.Count);
	}

	public static string Implode<T>(string separator, IEnumerable<T> values, Func<T, string> converter)
	{
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values, converter);
		}
		List<string> list = new List<string>();
		foreach (T value in values)
		{
			list.Add(value.ToString());
		}
		return ImplodeExtracted(separator, list.ToArray(), 0, list.Count);
	}

	public static string Implode(string separator, IEnumerable<string> values, string start, string end)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values);
		}
		List<string> list = new List<string>();
		foreach (string value in values)
		{
			list.Add(value);
		}
		if (list.Count > 0)
		{
			start = start ?? string.Empty;
			end = end ?? string.Empty;
			return start + ImplodeExtracted(separator, list.ToArray(), 0, list.Count) + end;
		}
		return string.Empty;
	}

	public static string Implode<T>(string separator, IEnumerable<T> values, string start, string end)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values);
		}
		List<string> list = new List<string>();
		foreach (T value in values)
		{
			list.Add(value.ToString());
		}
		if (list.Count > 0)
		{
			start = start ?? string.Empty;
			end = end ?? string.Empty;
			return start + ImplodeExtracted(separator, list.ToArray(), 0, list.Count) + end;
		}
		return string.Empty;
	}

	public static string Implode<T>(string separator, IEnumerable<T> values, Func<T, string> converter, string start, string end)
	{
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (separator == null)
		{
			return Concat(values, converter);
		}
		List<string> list = new List<string>();
		foreach (T value in values)
		{
			list.Add(value.ToString());
		}
		if (list.Count > 0)
		{
			start = start ?? string.Empty;
			end = end ?? string.Empty;
			return start + ImplodeExtracted(separator, list.ToArray(), 0, list.Count) + end;
		}
		return string.Empty;
	}

	public static bool Like(this string text, Regex regex, int startAt)
	{
		return regex.IsMatch(text, startAt);
	}

	public static bool Like(this string text, Regex regex)
	{
		return text.Like(regex, 0);
	}

	public static bool Like(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
	{
		Regex regex = new Regex(regexPattern, regexOptions);
		return regex.IsMatch(text, startAt);
	}

	public static bool Like(this string text, string regexPattern, RegexOptions regexOptions)
	{
		return text.Like(regexPattern, regexOptions, 0);
	}

	public static bool Like(this string text, string regexPattern, bool ignoreCase)
	{
		return text.Like(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
	}

	public static bool Like(this string text, string regexPattern)
	{
		return text.Like(regexPattern, RegexOptions.IgnoreCase, 0);
	}

	public static bool Like(this string text, string regexPattern, bool ignoreCase, int startAt)
	{
		return text.Like(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt);
	}

	public static bool Like(this string text, string regexPattern, int startAt)
	{
		return text.Like(regexPattern, RegexOptions.IgnoreCase, startAt);
	}

	public static Match Match(this string text, Regex regex, int startAt, int length)
	{
		return regex.Match(text, startAt, length);
	}

	public static Match Match(this string text, Regex regex, int startAt)
	{
		return regex.Match(text, startAt);
	}

	public static Match Match(this string text, Regex regex)
	{
		return text.Match(regex, 0);
	}

	public static Match Match(this string text, string regexPattern, RegexOptions regexOptions, int startAt, int length)
	{
		Regex regex = new Regex(regexPattern, regexOptions);
		return regex.Match(text, startAt, length);
	}

	public static Match Match(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
	{
		Regex regex = new Regex(regexPattern, regexOptions);
		return regex.Match(text, startAt);
	}

	public static Match Match(this string text, string regexPattern, RegexOptions regexOptions)
	{
		return text.Match(regexPattern, regexOptions, 0);
	}

	public static Match Match(this string text, string regexPattern, bool ignoreCase, int startAt, int length)
	{
		return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt, length);
	}

	public static Match Match(this string text, string regexPattern, bool ignoreCase, int startAt)
	{
		return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt);
	}

	public static Match Match(this string text, string regexPattern, bool ignoreCase)
	{
		return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
	}

	public static Match Match(this string text, string regexPattern, int startAt, int length)
	{
		return text.Match(regexPattern, RegexOptions.IgnoreCase, startAt, length);
	}

	public static Match Match(this string text, string regexPattern, int startAt)
	{
		return text.Match(regexPattern, RegexOptions.IgnoreCase, startAt);
	}

	public static Match Match(this string text, string regexPattern)
	{
		return text.Match(regexPattern, RegexOptions.IgnoreCase, 0);
	}

	public static MatchCollection Matches(this string text, Regex regex, int startAt)
	{
		return regex.Matches(text, startAt);
	}

	public static MatchCollection Matches(this string text, Regex regex)
	{
		return text.Matches(regex, 0);
	}

	public static MatchCollection Matches(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
	{
		Regex regex = new Regex(regexPattern, regexOptions);
		return regex.Matches(text, startAt);
	}

	public static MatchCollection Matches(this string text, string regexPattern, RegexOptions regexOptions)
	{
		return text.Matches(regexPattern, regexOptions, 0);
	}

	public static MatchCollection Matches(this string text, string regexPattern, bool ignoreCase, int startAt)
	{
		Regex regex = new Regex(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
		return regex.Matches(text, startAt);
	}

	public static MatchCollection Matches(this string text, string regexPattern, bool ignoreCase)
	{
		return text.Matches(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
	}

	public static MatchCollection Matches(this string text, string regexPattern, int startAt)
	{
		Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
		return regex.Matches(text, startAt);
	}

	public static MatchCollection Matches(this string text, string regexPattern)
	{
		return text.Matches(regexPattern, RegexOptions.IgnoreCase, 0);
	}

	public static string NeglectEnd(this string text, string end, StringComparison comparisonType)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (end == null)
		{
			throw new ArgumentNullException("end");
		}
		if (text.EndsWith(end, comparisonType))
		{
			return text.ExceptEnd(end.Length);
		}
		return text;
	}

	public static string NeglectStart(this string text, string start)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (start == null)
		{
			throw new ArgumentNullException("start");
		}
		if (text.StartsWith(start, StringComparison.CurrentCulture))
		{
			return text.ExceptStart(start.Length);
		}
		return text;
	}

	public static string NeglectStart(this string text, string start, StringComparison comparisonType)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (start == null)
		{
			throw new ArgumentNullException("start");
		}
		if (text.StartsWith(start, comparisonType))
		{
			return text.ExceptStart(start.Length);
		}
		return text;
	}

	public static string Safe(this string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		return text;
	}

	public static string Start(this string text, int characterCount)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		int length = text.Length;
		if (length < characterCount)
		{
			return text;
		}
		return text.Substring(0, characterCount);
	}

	private static string ConcatExtracted(object[] array, int startIndex, int count)
	{
		int num = 0;
		int num2 = startIndex + count;
		string[] array2 = new string[count];
		for (int i = startIndex; i < num2; i++)
		{
			object obj = array[i];
			if (!object.ReferenceEquals(obj, null))
			{
				num += (array2[i - startIndex] = obj.ToString()).Length;
			}
		}
		return ConcatExtractedExtracted(array2, 0, count, num);
	}

	private static string ConcatExtracted(string[] array, int startIndex, int count)
	{
		int num = 0;
		int num2 = startIndex + count;
		for (int i = startIndex; i < num2; i++)
		{
			string text = array[i];
			if (!object.ReferenceEquals(text, null))
			{
				num += text.Length;
			}
		}
		return ConcatExtractedExtracted(array, startIndex, num2, num);
	}

	private static string ConcatExtractedExtracted(string[] array, int startIndex, int maxIndex, int length)
	{
		if (length <= 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(length);
		for (int i = startIndex; i < maxIndex; i++)
		{
			string value = array[i];
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	private static string ImplodeExtracted(string separator, object[] array, int startIndex, int count)
	{
		int num = 0;
		int num2 = startIndex + count;
		string[] array2 = new string[count];
		for (int i = startIndex; i < num2; i++)
		{
			object obj = array[i];
			if (!object.ReferenceEquals(obj, null))
			{
				num += (array2[i - startIndex] = obj.ToString()).Length;
			}
		}
		num += separator.Length * (count - 1);
		return ImplodeExtractedExtracted(separator, array2, 0, count, num);
	}

	private static string ImplodeExtracted(string separator, string[] array, int startIndex, int count)
	{
		int num = 0;
		int num2 = startIndex + count;
		for (int i = startIndex; i < num2; i++)
		{
			string text = array[i];
			if (!object.ReferenceEquals(text, null))
			{
				num += text.Length;
			}
		}
		num += separator.Length * (count - 1);
		return ImplodeExtractedExtracted(separator, array, startIndex, num2, num);
	}

	private static string ImplodeExtractedExtracted(string separator, string[] array, int startIndex, int maxIndex, int length)
	{
		if (length <= 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(length);
		bool flag = true;
		for (int i = startIndex; i < maxIndex; i++)
		{
			string value = array[i];
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(separator);
			}
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	public static bool IsNullOrWhiteSpace(string value)
	{
		return string.IsNullOrWhiteSpace(value);
	}

	public static string Concat(IEnumerable<string> values)
	{
		return string.Concat(values);
	}

	public static string Concat<T>(IEnumerable<T> values)
	{
		return string.Concat(values);
	}

	public static string Join(string separator, IEnumerable<string> values)
	{
		return string.Join(separator, values);
	}

	public static string Join<T>(string separator, IEnumerable<T> values)
	{
		return string.Join(separator, values);
	}

	public static string Join(string separator, params object[] values)
	{
		return string.Join(separator, values);
	}

	public static string Join(string separator, params string[] values)
	{
		return string.Join(separator, values);
	}

	public static string Join(string separator, string[] values, int startIndex, int count)
	{
		return string.Join(separator, values, startIndex, count);
	}

	public static string EnsureEnd(this string text, string end)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (!text.EndsWith(end, ignoreCase: false, CultureInfo.CurrentCulture))
		{
			return text.Append(end);
		}
		return text;
	}

	public static string EnsureEnd(this string text, string end, bool ignoreCase, CultureInfo culture)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (!text.EndsWith(end, ignoreCase, culture))
		{
			return text.Append(end);
		}
		return text;
	}

	public static string EnsureStart(this string text, string start, bool ignoreCase, CultureInfo culture)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (!text.StartsWith(start, ignoreCase, culture))
		{
			return start.Append(text);
		}
		return text;
	}

	public static string NeglectEnd(this string text, string end)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (end == null)
		{
			throw new ArgumentNullException("end");
		}
		if (text.EndsWith(end, ignoreCase: false, CultureInfo.CurrentCulture))
		{
			return text.ExceptEnd(end.Length);
		}
		return text;
	}

	public static string NeglectEnd(this string text, string end, bool ignoreCase, CultureInfo culture)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (end == null)
		{
			throw new ArgumentNullException("end");
		}
		if (text.EndsWith(end, ignoreCase, culture))
		{
			return text.ExceptEnd(end.Length);
		}
		return text;
	}

	public static string NeglectStart(this string text, string start, bool ignoreCase, CultureInfo culture)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (start == null)
		{
			throw new ArgumentNullException("start");
		}
		if (text.StartsWith(start, ignoreCase, culture))
		{
			return text.ExceptStart(start.Length);
		}
		return text;
	}
}
