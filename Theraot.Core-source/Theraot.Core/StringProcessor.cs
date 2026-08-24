using System;
using System.Collections.Generic;
using Theraot.Collections;

namespace Theraot.Core;

[Serializable]
public class StringProcessor
{
	private readonly int _length;

	private readonly string _string;

	private int _position;

	public int Count => _length - _position;

	public bool EndOfString => _position == _length;

	public bool Greedy { get; set; }

	public int Position
	{
		get
		{
			return _position;
		}
		set
		{
			if (value > 0 && value <= _length)
			{
				_position = value;
				return;
			}
			throw new ArgumentOutOfRangeException("value", "The position must be greater than zero and less or equal to the length of the underlying string.");
		}
	}

	public string String => _string;

	public StringProcessor(string str)
	{
		if (str == null)
		{
			throw new ArgumentNullException("str", "The string is null.");
		}
		_string = str;
		_length = str.Length;
		_position = 0;
	}

	public bool ExtractUntil(out string found, string target)
	{
		return ExtractUntil(out found, target, StringComparison.Ordinal);
	}

	public bool ExtractUntil(out string found, string target, StringComparison stringComparison)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "The target string is null.");
		}
		if (target.Length != 0)
		{
			int num = _string.IndexOf(target, _position, stringComparison);
			if (num != -1)
			{
				found = PrivateReadToPosition(num);
				return true;
			}
		}
		found = null;
		return false;
	}

	public bool ExtractUntil(out string found, char target)
	{
		int num = _string.IndexOf(target, _position);
		if (num != -1)
		{
			found = PrivateReadToPosition(num);
			return true;
		}
		found = null;
		return false;
	}

	public bool ExtractUntilAfter(out string found, string target)
	{
		return ExtractUntilAfter(out found, target, StringComparison.Ordinal);
	}

	public bool ExtractUntilAfter(out string found, string target, StringComparison stringComparison)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "The target string is null.");
		}
		if (target.Length != 0)
		{
			int num = _string.IndexOf(target, _position, stringComparison);
			if (num != -1)
			{
				found = PrivateReadToPosition(num + target.Length);
				return true;
			}
		}
		found = null;
		return false;
	}

	public bool ExtractUntilAfter(out string found, char target)
	{
		int num = _string.IndexOf(target, _position);
		if (num != -1)
		{
			found = PrivateReadToPosition(num + 1);
			return true;
		}
		found = null;
		return false;
	}

	public int Peek()
	{
		if (_position == _length)
		{
			return -1;
		}
		return _string[_position];
	}

	public bool Peek(string target)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "The target string is null.");
		}
		int length = target.Length;
		if (_position + length <= _length)
		{
			string text = _string.Substring(_position, length);
			if (text == target)
			{
				return true;
			}
		}
		return false;
	}

	public bool Peek(char target)
	{
		if (_position == _length)
		{
			return false;
		}
		char c = _string[_position];
		if (c == target)
		{
			return true;
		}
		return false;
	}

	public bool Peek(Func<char, bool> predicate)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate", "The predicate is null.");
		}
		if (_position == _length)
		{
			return false;
		}
		char arg = _string[_position];
		if (predicate(arg))
		{
			return true;
		}
		return false;
	}

	public char PeekChar()
	{
		if (_position == _length)
		{
			throw new IndexOutOfRangeException("Reached the end of the string.");
		}
		return _string[_position];
	}

	public int Read()
	{
		if (_position == _length)
		{
			return -1;
		}
		char result = _string[_position];
		_position++;
		return result;
	}

	public int Read(char[] destination, int destinationIndex, int count)
	{
		if (destination == null)
		{
			throw new ArgumentNullException("destination", "Buffer cannot be null.");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
		}
		int num = _length - _position;
		if (num > 0)
		{
			if (num > count)
			{
				num = count;
			}
			_string.CopyTo(_position, destination, destinationIndex, num);
			_position += num;
		}
		return num;
	}

	public bool Read(string target)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "The target string is null.");
		}
		int length = target.Length;
		if (_position + length <= _length)
		{
			string text = _string.Substring(_position, length);
			if (text == target)
			{
				_position += length;
				return true;
			}
		}
		return false;
	}

	public bool Read(char target)
	{
		if (_position == _length)
		{
			return false;
		}
		char c = _string[_position];
		if (c == target)
		{
			_position++;
			return true;
		}
		return false;
	}

	public string Read(int length)
	{
		if (_position + length <= _length)
		{
			string result = _string.Substring(_position, length);
			_position += length;
			return result;
		}
		return null;
	}

	public string Read(IEnumerable<string> targets)
	{
		if (targets == null)
		{
			throw new ArgumentNullException("targets", "The targets collection is null.");
		}
		foreach (string target in targets)
		{
			if (target == null)
			{
				throw new ArgumentException("Found nulls in the targets collection.", "targets");
			}
			int length = target.Length;
			if (_position + length <= _length)
			{
				string text = _string.Substring(_position, length);
				if (text == target)
				{
					_position += length;
					return text;
				}
			}
		}
		return null;
	}

	public string Read(Func<char, bool> predicate)
	{
		int position = _position;
		Skip(predicate);
		return _string.Substring(position, _position - position);
	}

	public char ReadChar()
	{
		if (_position == _length)
		{
			throw new IndexOutOfRangeException("Reached the end of the string.");
		}
		char result = _string[_position];
		_position++;
		return result;
	}

	public string ReadLine()
	{
		bool greedy = Greedy;
		Greedy = true;
		string result = PrivateReadUntil(new char[2] { '\r', '\n' });
		Read('\r');
		Read('\n');
		Greedy = greedy;
		return result;
	}

	public string ReadToEnd()
	{
		string result = ((_position != 0) ? _string.Substring(_position, _length - _position) : _string);
		_position = _length;
		return result;
	}

	public string ReadToPosition(int position)
	{
		if (position == _position)
		{
			return string.Empty;
		}
		if (position < _position)
		{
			throw new ArgumentOutOfRangeException("position", "The new position must be greater than the current position.");
		}
		return PrivateReadToPosition(position);
	}

	public string ReadUntil(string target)
	{
		return ReadUntil(target, StringComparison.Ordinal);
	}

	public string ReadUntil(string target, StringComparison stringComparison)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "The target string is null.");
		}
		if (target.Length != 0)
		{
			int num = _string.IndexOf(target, _position, stringComparison);
			if (num != -1)
			{
				return PrivateReadToPosition(num);
			}
		}
		if (Greedy)
		{
			return ReadToEnd();
		}
		return null;
	}

	public string ReadUntil(char target)
	{
		int num = _string.IndexOf(target, _position);
		if (num != -1)
		{
			return PrivateReadToPosition(num);
		}
		if (Greedy)
		{
			return ReadToEnd();
		}
		return null;
	}

	public string ReadUntil(IEnumerable<string> targets)
	{
		return ReadUntil(targets, StringComparison.Ordinal);
	}

	public string ReadUntil(IEnumerable<string> targets, StringComparison stringComparison)
	{
		int position = _position;
		if (SkipUntil(targets, stringComparison))
		{
			return _string.Substring(position, _position - position);
		}
		return null;
	}

	public string ReadUntil(IEnumerable<string> targets, out string found)
	{
		return ReadUntil(targets, out found, StringComparison.Ordinal);
	}

	public string ReadUntil(IEnumerable<string> targets, out string found, StringComparison stringComparison)
	{
		int position = _position;
		if (SkipUntil(targets, out found, stringComparison))
		{
			return _string.Substring(position, _position - position);
		}
		return null;
	}

	public string ReadUntil(IEnumerable<char> targets)
	{
		int position = _position;
		if (SkipUntil(targets))
		{
			return _string.Substring(position, _position - position);
		}
		return null;
	}

	public string ReadUntil(char[] targets)
	{
		if (targets == null)
		{
			throw new ArgumentNullException("targets");
		}
		return PrivateReadUntil(targets);
	}

	public string ReadUntil(Func<char, bool> predicate)
	{
		int position = _position;
		if (SkipUntil(predicate))
		{
			return _string.Substring(position, _position - position);
		}
		return null;
	}

	public string ReadUntilAfter(string target)
	{
		return ReadUntilAfter(target, StringComparison.Ordinal);
	}

	public string ReadUntilAfter(string target, StringComparison stringComparison)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "The target string is null.");
		}
		if (target.Length != 0)
		{
			int num = _string.IndexOf(target, _position, stringComparison);
			if (num != -1)
			{
				return PrivateReadToPosition(num + target.Length);
			}
		}
		if (Greedy)
		{
			return ReadToEnd();
		}
		return null;
	}

	public string ReadUntilAfter(char target)
	{
		int num = _string.IndexOf(target, _position);
		if (num != -1)
		{
			return PrivateReadToPosition(num + 1);
		}
		if (Greedy)
		{
			return ReadToEnd();
		}
		return null;
	}

	public string ReadWhile(char target)
	{
		int position = _position;
		SkipWhile(target);
		return _string.Substring(position, _position - position);
	}

	public string ReadWhile(IEnumerable<char> targets)
	{
		int position = _position;
		SkipWhile(targets);
		return _string.Substring(position, _position - position);
	}

	public string ReadWhile(Func<char, bool> predicate)
	{
		int position = _position;
		SkipWhile(predicate);
		return _string.Substring(position, _position - position);
	}

	public bool Skip(Func<char, bool> predicate)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate", "The predicate is null.");
		}
		if (_position == _length)
		{
			return false;
		}
		char arg = _string[_position];
		if (predicate(arg))
		{
			_position++;
			return true;
		}
		return false;
	}

	public bool SkipBackBefore(string target)
	{
		return SkipBackBefore(target, StringComparison.Ordinal);
	}

	public bool SkipBackBefore(string target, StringComparison stringComparison)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "The target string is null.");
		}
		if (target.Length != 0)
		{
			int num = _string.LastIndexOf(target, _position, stringComparison);
			if (num != -1)
			{
				_position = num;
				return true;
			}
		}
		if (Greedy)
		{
			_position = 0;
		}
		return false;
	}

	public bool SkipBackBefore(char target)
	{
		int num = _string.LastIndexOf(target, _position);
		if (num != -1)
		{
			_position = num;
			return true;
		}
		if (Greedy)
		{
			_position = 0;
		}
		return false;
	}

	public bool SkipBackTo(string target)
	{
		return SkipBackTo(target, StringComparison.Ordinal);
	}

	public bool SkipBackTo(string target, StringComparison stringComparison)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "The target string is null.");
		}
		if (target.Length != 0)
		{
			int num = _string.LastIndexOf(target, _position, stringComparison);
			if (num != -1)
			{
				_position = num + target.Length;
				return true;
			}
		}
		if (Greedy)
		{
			_position = 0;
		}
		return false;
	}

	public bool SkipBackTo(char target)
	{
		int num = _string.LastIndexOf(target, _position);
		if (num != -1)
		{
			_position = num + 1;
			return true;
		}
		if (Greedy)
		{
			_position = 0;
		}
		return false;
	}

	public bool SkipLine()
	{
		bool greedy = Greedy;
		Greedy = true;
		bool result = PrivateSkipUntil(new char[2] { '\r', '\n' }) || Read('\r') || Read('\n');
		Greedy = greedy;
		return result;
	}

	public bool SkipUntil(string target)
	{
		return SkipUntil(target, StringComparison.Ordinal);
	}

	public bool SkipUntil(string target, StringComparison stringComparison)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "The target string is null.");
		}
		if (target.Length != 0)
		{
			int num = _string.IndexOf(target, _position, stringComparison);
			if (num != -1)
			{
				_position = num;
				return true;
			}
		}
		if (Greedy)
		{
			_position = _length;
		}
		return false;
	}

	public bool SkipUntil(char target)
	{
		int num = _string.IndexOf(target, _position);
		bool flag = num != -1;
		if (flag)
		{
			_position = num;
		}
		else if (Greedy)
		{
			_position = _length;
		}
		return flag;
	}

	public bool SkipUntil(IEnumerable<string> targets)
	{
		return SkipUntil(targets, StringComparison.Ordinal);
	}

	public bool SkipUntil(IEnumerable<string> targets, StringComparison stringComparison)
	{
		if (targets == null)
		{
			throw new ArgumentNullException("targets", "The targets collection is null.");
		}
		int num = 0;
		bool flag = false;
		foreach (string target in targets)
		{
			if (target == null)
			{
				throw new ArgumentException("Found nulls in the targets collection.", "targets");
			}
			if (target.Length != 0)
			{
				int num2 = _string.IndexOf(target, _position, stringComparison);
				if (num2 != -1 && (!flag || num2 < num))
				{
					num = num2;
					flag = true;
				}
			}
		}
		if (flag)
		{
			_position = num;
		}
		else if (Greedy)
		{
			_position = _length;
		}
		return flag;
	}

	public bool SkipUntil(IEnumerable<string> targets, out string found)
	{
		return SkipUntil(targets, out found, StringComparison.Ordinal);
	}

	public bool SkipUntil(IEnumerable<string> targets, out string found, StringComparison stringComparison)
	{
		if (targets == null)
		{
			throw new ArgumentNullException("targets", "The targets collection is null.");
		}
		found = null;
		int num = 0;
		bool flag = false;
		foreach (string target in targets)
		{
			if (target == null)
			{
				throw new ArgumentException("Found nulls in the targets collection.", "targets");
			}
			if (target.Length != 0)
			{
				int num2 = _string.IndexOf(target, _position, stringComparison);
				if (num2 != -1 && (!flag || num2 < num))
				{
					found = target;
					num = num2;
					flag = true;
				}
			}
		}
		if (flag)
		{
			_position = num;
		}
		else if (Greedy)
		{
			_position = _length;
		}
		return flag;
	}

	public bool SkipUntil(IEnumerable<char> targets)
	{
		if (targets == null)
		{
			throw new ArgumentNullException("targets");
		}
		int num = 0;
		bool flag = false;
		foreach (char target in targets)
		{
			int num2 = _string.IndexOf(target, _position);
			if (num2 != -1 && (!flag || num2 < num))
			{
				num = num2;
				flag = true;
			}
		}
		if (flag)
		{
			_position = num;
		}
		else if (Greedy)
		{
			_position = _length;
		}
		return flag;
	}

	public bool SkipUntil(char[] targets)
	{
		if (targets == null)
		{
			throw new ArgumentNullException("targets");
		}
		return PrivateSkipUntil(targets);
	}

	public bool SkipUntil(Func<char, bool> predicate)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate", "The predicate is null.");
		}
		if (_position == _length)
		{
			return false;
		}
		bool result = false;
		while (true)
		{
			if (_position == _length)
			{
				return result;
			}
			char arg = _string[_position];
			if (predicate(arg))
			{
				break;
			}
			_position++;
			result = true;
		}
		return result;
	}

	public bool SkipUntilAfter(string target)
	{
		return SkipUntilAfter(target, StringComparison.Ordinal);
	}

	public bool SkipUntilAfter(string target, StringComparison stringComparison)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "The target string is null.");
		}
		if (target.Length != 0)
		{
			int num = _string.IndexOf(target, _position, stringComparison);
			if (num != -1)
			{
				_position = num + target.Length;
				return true;
			}
		}
		if (Greedy)
		{
			_position = _length;
		}
		return false;
	}

	public bool SkipUntilAfter(char target)
	{
		int num = _string.IndexOf(target, _position);
		if (num != -1)
		{
			_position = num + 1;
			return true;
		}
		if (Greedy)
		{
			_position = _length;
		}
		return false;
	}

	public bool SkipWhile(char target)
	{
		if (_position == _length)
		{
			return false;
		}
		bool result = false;
		while (true)
		{
			if (_position == _length)
			{
				return result;
			}
			char c = _string[_position];
			if (c != target)
			{
				break;
			}
			_position++;
			result = true;
		}
		return result;
	}

	public bool SkipWhile(IEnumerable<char> targets)
	{
		if (targets == null)
		{
			throw new ArgumentNullException("targets");
		}
		if (_position == _length)
		{
			return false;
		}
		ICollection<char> collection = Extensions.AsCollection(targets);
		bool result = false;
		while (true)
		{
			if (_position == _length)
			{
				return result;
			}
			char item = _string[_position];
			if (!collection.Contains(item))
			{
				break;
			}
			_position++;
			result = true;
		}
		return result;
	}

	public bool SkipWhile(Func<char, bool> predicate)
	{
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		if (_position == _length)
		{
			return false;
		}
		bool result = false;
		while (true)
		{
			if (_position == _length)
			{
				return result;
			}
			char arg = _string[_position];
			if (!predicate(arg))
			{
				break;
			}
			_position++;
			result = true;
		}
		return result;
	}

	public bool TryPeek(out char character)
	{
		if (_position == _length)
		{
			character = '\0';
			return false;
		}
		character = _string[_position];
		return true;
	}

	public bool TryTake(out char character)
	{
		if (_position == _length)
		{
			character = '\0';
			return false;
		}
		character = _string[_position];
		_position++;
		return true;
	}

	private string PrivateReadToPosition(int position)
	{
		string result = _string.Substring(_position, position - _position);
		_position = position;
		return result;
	}

	private string PrivateReadUntil(char[] targets)
	{
		int num = _string.IndexOfAny(targets, _position);
		if (num != -1)
		{
			return PrivateReadToPosition(num);
		}
		if (Greedy)
		{
			return ReadToEnd();
		}
		return null;
	}

	private bool PrivateSkipUntil(char[] targets)
	{
		int num = _string.IndexOfAny(targets, _position);
		bool flag = num != -1;
		if (flag)
		{
			_position = num;
		}
		else if (Greedy)
		{
			_position = _length;
		}
		return flag;
	}
}
