using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Theraot.Core;

[DebuggerStepThrough]
public static class NumericHelper
{
	[StructLayout(LayoutKind.Explicit)]
	private struct DoubleUlong
	{
		[FieldOffset(0)]
		public double Dbl;

		[FieldOffset(0)]
		public ulong Uu;
	}

	public static bool AreBinaryEquals(double left, double right)
	{
		ulong num = (ulong)BitConverter.DoubleToInt64Bits(left);
		ulong num2 = (ulong)BitConverter.DoubleToInt64Bits(right);
		return num == num2;
	}

	public static bool AreBinaryEquals(float left, float right)
	{
		uint num = SingleAsUInt32(left);
		uint num2 = SingleAsUInt32(right);
		return num == num2;
	}

	public static bool IsOne(double value)
	{
		ulong num = (ulong)BitConverter.DoubleToInt64Bits(value);
		return num == 4607182418800017408L;
	}

	public static bool IsOne(float value)
	{
		uint num = SingleAsUInt32(value);
		return num == 1065353216;
	}

	public static bool IsZero(double value)
	{
		ulong num = (ulong)BitConverter.DoubleToInt64Bits(value);
		if (num != 0)
		{
			return num == 9223372036854775808uL;
		}
		return true;
	}

	public static bool IsZero(float value)
	{
		uint num = SingleAsUInt32(value);
		if (num != 0)
		{
			return num == 2147483648u;
		}
		return true;
	}

	public static int BinaryReverse(this int value)
	{
		return (int)((uint)value).BinaryReverse();
	}

	public static long BinaryReverse(this long value)
	{
		return (long)((ulong)value).BinaryReverse();
	}

	[CLSCompliant(false)]
	public static uint BinaryReverse(this uint value)
	{
		value = ((value & 0xAAAAAAAAu) >> 1) | ((value & 0x55555555) << 1);
		value = ((value & 0xCCCCCCCCu) >> 2) | ((value & 0x33333333) << 2);
		value = ((value & 0xF0F0F0F0u) >> 4) | ((value & 0xF0F0F0F) << 4);
		value = ((value & 0xFF00FF00u) >> 8) | ((value & 0xFF00FF) << 8);
		return (value >> 16) | (value << 16);
	}

	[CLSCompliant(false)]
	public static ulong BinaryReverse(this ulong value)
	{
		GetParts(value, out var lo, out var hi);
		return BuildUInt64(lo.BinaryReverse(), hi.BinaryReverse());
	}

	[CLSCompliant(false)]
	public static IEnumerable<sbyte> Bits(this sbyte value)
	{
		byte check = 128;
		int log2 = 8;
		byte tmp = (byte)value;
		do
		{
			if ((tmp & check) != 0)
			{
				yield return (sbyte)check;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<short> Bits(this short value)
	{
		ushort check = 32768;
		int log2 = 16;
		ushort tmp = (ushort)value;
		do
		{
			if ((tmp & check) != 0)
			{
				yield return (short)check;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<int> Bits(this int value)
	{
		uint check = 2147483648u;
		int log2 = 32;
		do
		{
			if (((uint)value & check) != 0)
			{
				yield return (int)check;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<long> Bits(this long value)
	{
		ulong check = 9223372036854775808uL;
		int log2 = 64;
		do
		{
			if (((ulong)value & check) != 0)
			{
				yield return (long)check;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<byte> Bits(this byte value)
	{
		byte check = 128;
		int log2 = 8;
		do
		{
			if ((value & check) != 0)
			{
				yield return check;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<ushort> Bits(this ushort value)
	{
		ushort check = 32768;
		int log2 = 16;
		do
		{
			if ((value & check) != 0)
			{
				yield return check;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<uint> Bits(this uint value)
	{
		uint check = 2147483648u;
		int log2 = 32;
		do
		{
			if ((value & check) != 0)
			{
				yield return check;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<ulong> Bits(this ulong value)
	{
		ulong check = 9223372036854775808uL;
		int log2 = 64;
		do
		{
			if ((value & check) != 0)
			{
				yield return check;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<int> BitsBinary(this sbyte value)
	{
		byte check = 128;
		int log2 = 8;
		byte tmp = (byte)value;
		do
		{
			if ((tmp & check) != 0)
			{
				yield return 1;
			}
			else
			{
				yield return 0;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<int> BitsBinary(this short value)
	{
		ushort check = 32768;
		int log2 = 16;
		ushort tmp = (ushort)value;
		do
		{
			if ((tmp & check) != 0)
			{
				yield return 1;
			}
			else
			{
				yield return 0;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<int> BitsBinary(this int value)
	{
		uint check = 2147483648u;
		int log2 = 32;
		do
		{
			if (((uint)value & check) != 0)
			{
				yield return 1;
			}
			else
			{
				yield return 0;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<int> BitsBinary(this long value)
	{
		ulong check = 9223372036854775808uL;
		int log2 = 64;
		do
		{
			if (((ulong)value & check) != 0)
			{
				yield return 1;
			}
			else
			{
				yield return 0;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<int> BitsBinary(this byte value)
	{
		byte check = 128;
		int log2 = 8;
		do
		{
			if ((value & check) != 0)
			{
				yield return 1;
			}
			else
			{
				yield return 0;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<int> BitsBinary(this ushort value)
	{
		ushort check = 32768;
		int log2 = 16;
		do
		{
			if ((value & check) != 0)
			{
				yield return 1;
			}
			else
			{
				yield return 0;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<int> BitsBinary(this uint value)
	{
		uint check = 2147483648u;
		int log2 = 32;
		do
		{
			if ((value & check) != 0)
			{
				yield return 1;
			}
			else
			{
				yield return 0;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<int> BitsBinary(this ulong value)
	{
		ulong check = 9223372036854775808uL;
		int log2 = 64;
		do
		{
			if ((value & check) != 0)
			{
				yield return 1;
			}
			else
			{
				yield return 0;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<int> BitsLog2(this sbyte value)
	{
		byte check = 128;
		int log2 = 8;
		byte tmp = (byte)value;
		do
		{
			if ((tmp & check) != 0)
			{
				yield return log2 - 1;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<int> BitsLog2(this short value)
	{
		ushort check = 32768;
		int log2 = 16;
		ushort tmp = (ushort)value;
		do
		{
			if ((tmp & check) != 0)
			{
				yield return log2 - 1;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<int> BitsLog2(this int value)
	{
		uint check = 2147483648u;
		int log2 = 32;
		do
		{
			if (((uint)value & check) != 0)
			{
				yield return log2 - 1;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<int> BitsLog2(this long value)
	{
		ulong check = 9223372036854775808uL;
		int log2 = 64;
		do
		{
			if (((ulong)value & check) != 0)
			{
				yield return log2 - 1;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static IEnumerable<int> BitsLog2(this byte value)
	{
		byte check = 128;
		int log2 = 8;
		do
		{
			if ((value & check) != 0)
			{
				yield return log2 - 1;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<int> BitsLog2(this ushort value)
	{
		ushort check = 32768;
		int log2 = 16;
		do
		{
			if ((value & check) != 0)
			{
				yield return log2 - 1;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<int> BitsLog2(this uint value)
	{
		uint check = 2147483648u;
		int log2 = 32;
		do
		{
			if ((value & check) != 0)
			{
				yield return log2 - 1;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	[CLSCompliant(false)]
	public static IEnumerable<int> BitsLog2(this ulong value)
	{
		ulong check = 9223372036854775808uL;
		int log2 = 64;
		do
		{
			if ((value & check) != 0)
			{
				yield return log2 - 1;
			}
			check >>= 1;
			log2--;
		}
		while (log2 > 0);
	}

	public static long DoubleAsInt64(double value)
	{
		return BitConverter.DoubleToInt64Bits(value);
	}

	[CLSCompliant(false)]
	public static ulong DoubleAsUInt64(double value)
	{
		return (ulong)BitConverter.DoubleToInt64Bits(value);
	}

	public static float Int32AsSingle(int value)
	{
		return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
	}

	public static double Int64AsDouble(long value)
	{
		return BitConverter.Int64BitsToDouble(value);
	}

	public static int LeadingZeroCount(this int value)
	{
		return ((uint)value).LeadingZeroCount();
	}

	public static int LeadingZeroCount(this long value)
	{
		return ((ulong)value).LeadingZeroCount();
	}

	[CLSCompliant(false)]
	public static int LeadingZeroCount(this uint value)
	{
		value |= value >> 1;
		value |= value >> 2;
		value |= value >> 4;
		value |= value >> 8;
		value |= value >> 16;
		return 32 - PopulationCount(value);
	}

	[CLSCompliant(false)]
	public static int LeadingZeroCount(this ulong value)
	{
		value |= value >> 1;
		value |= value >> 2;
		value |= value >> 4;
		value |= value >> 8;
		value |= value >> 16;
		value |= value >> 32;
		return 64 - PopulationCount(value);
	}

	[CLSCompliant(false)]
	public static int PopulationCount(uint value)
	{
		value -= (value >> 1) & 0x55555555;
		value = (value & 0x33333333) + ((value >> 2) & 0x33333333);
		value = (value + (value >> 4)) & 0xF0F0F0F;
		value += value >> 8;
		value += value >> 16;
		return (int)(value & 0x3F);
	}

	[CLSCompliant(false)]
	public static int PopulationCount(ulong value)
	{
		value -= (value >> 1) & 0x5555555555555555L;
		value = (value & 0x3333333333333333L) + ((value >> 2) & 0x3333333333333333L);
		value = (value + (value >> 4)) & 0xF0F0F0F0F0F0F0FL;
		return (int)(value * 72340172838076673L >> 56);
	}

	public static int PopulationCount(int value)
	{
		return PopulationCount((uint)value);
	}

	public static int PopulationCount(long value)
	{
		return PopulationCount((ulong)value);
	}

	public static int SingleAsInt32(float value)
	{
		return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
	}

	[CLSCompliant(false)]
	public static uint SingleAsUInt32(float value)
	{
		return (uint)BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
	}

	public static string ToStringBinary(this double value)
	{
		return BitConverter.DoubleToInt64Bits(value).ToStringBinary();
	}

	public static string ToStringBinary(this float value)
	{
		return BitConverter.ToInt32(BitConverter.GetBytes(value), 0).ToStringBinary();
	}

	[CLSCompliant(false)]
	public static string ToStringBinary(this sbyte value)
	{
		return StringHelper.Concat(value.BitsBinary(), (int input) => input.ToString(CultureInfo.InvariantCulture));
	}

	public static string ToStringBinary(this short value)
	{
		return StringHelper.Concat(value.BitsBinary(), (int input) => input.ToString(CultureInfo.InvariantCulture));
	}

	public static string ToStringBinary(this int value)
	{
		return StringHelper.Concat(value.BitsBinary(), (int input) => input.ToString(CultureInfo.InvariantCulture));
	}

	public static string ToStringBinary(this long value)
	{
		return StringHelper.Concat(value.BitsBinary(), (int input) => input.ToString(CultureInfo.InvariantCulture));
	}

	public static string ToStringBinary(this byte value)
	{
		return StringHelper.Concat(value.BitsBinary(), (int input) => input.ToString(CultureInfo.InvariantCulture));
	}

	[CLSCompliant(false)]
	public static string ToStringBinary(this ushort value)
	{
		return StringHelper.Concat(value.BitsBinary(), (int input) => input.ToString(CultureInfo.InvariantCulture));
	}

	[CLSCompliant(false)]
	public static string ToStringBinary(this uint value)
	{
		return StringHelper.Concat(value.BitsBinary(), (int input) => input.ToString(CultureInfo.InvariantCulture));
	}

	[CLSCompliant(false)]
	public static string ToStringBinary(this ulong value)
	{
		return StringHelper.Concat(value.BitsBinary(), (int input) => input.ToString(CultureInfo.InvariantCulture));
	}

	public static int TrailingZeroCount(this int value)
	{
		return value.BinaryReverse().LeadingZeroCount();
	}

	public static int TrailingZeroCount(this long value)
	{
		return value.BinaryReverse().LeadingZeroCount();
	}

	[CLSCompliant(false)]
	public static int TrailingZeroCount(this uint value)
	{
		return value.BinaryReverse().LeadingZeroCount();
	}

	[CLSCompliant(false)]
	public static int TrailingZeroCount(this ulong value)
	{
		return value.BinaryReverse().LeadingZeroCount();
	}

	[CLSCompliant(false)]
	public static float UInt32AsSingle(uint value)
	{
		return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
	}

	[CLSCompliant(false)]
	public static double UInt64AsDouble(ulong value)
	{
		return BitConverter.Int64BitsToDouble((long)value);
	}

	public static double BuildDouble(int sign, long mantissa, int exponent)
	{
		if (sign == 0 || mantissa == 0)
		{
			return 0.0;
		}
		if (mantissa < 0)
		{
			mantissa = -mantissa;
			sign = -sign;
		}
		ulong man = (ulong)mantissa;
		return GetDoubleFromParts(sign, exponent, man);
	}

	[CLSCompliant(false)]
	public static double BuildDouble(int sign, ulong mantissa, int exponent)
	{
		return GetDoubleFromParts(sign, exponent, mantissa);
	}

	public static long BuildInt64(int hi, int lo)
	{
		return (long)(((ulong)(uint)hi << 32) | (uint)lo);
	}

	[CLSCompliant(false)]
	public static long BuildInt64(uint hi, uint lo)
	{
		return (long)(((ulong)hi << 32) | lo);
	}

	public static float BuildSingle(int sign, int mantissa, int exponent)
	{
		return (float)BuildDouble(sign, mantissa, exponent);
	}

	[CLSCompliant(false)]
	public static float BuildSingle(int sign, uint mantissa, int exponent)
	{
		return (float)BuildDouble(sign, mantissa, exponent);
	}

	[CLSCompliant(false)]
	public static ulong BuildUInt64(uint hi, uint lo)
	{
		return ((ulong)hi << 32) | lo;
	}

	[CLSCompliant(false)]
	public static double GetDoubleFromParts(int sign, int exp, ulong man)
	{
		DoubleUlong doubleUlong = default(DoubleUlong);
		doubleUlong.Dbl = 0.0;
		if (man == 0)
		{
			doubleUlong.Uu = 0uL;
		}
		else
		{
			int num = CbitHighZero(man) - 11;
			man = ((num >= 0) ? (man << num) : (man >> -num));
			exp -= num;
			exp += 1075;
			if (exp >= 2047)
			{
				doubleUlong.Uu = 9218868437227405312uL;
			}
			else if (exp <= 0)
			{
				exp--;
				if (exp < -52)
				{
					doubleUlong.Uu = 0uL;
				}
				else
				{
					doubleUlong.Uu = man >> -exp;
				}
			}
			else
			{
				doubleUlong.Uu = (man & 0xFFFFFFFFFFFFFL) | (ulong)((long)exp << 52);
			}
		}
		if (sign < 0)
		{
			doubleUlong.Uu |= 9223372036854775808uL;
		}
		return doubleUlong.Dbl;
	}

	[CLSCompliant(false)]
	public static void GetDoubleParts(double dbl, out int sign, out int exp, out ulong man, out bool fFinite)
	{
		DoubleUlong doubleUlong = default(DoubleUlong);
		doubleUlong.Uu = 0uL;
		doubleUlong.Dbl = dbl;
		sign = 1 - ((int)(doubleUlong.Uu >> 62) & 2);
		man = doubleUlong.Uu & 0xFFFFFFFFFFFFFL;
		exp = (int)(doubleUlong.Uu >> 52) & 0x7FF;
		if (exp == 0)
		{
			fFinite = true;
			if (man != 0)
			{
				exp = -1074;
			}
		}
		else if (exp == 2047)
		{
			fFinite = false;
			exp = int.MaxValue;
		}
		else
		{
			fFinite = true;
			man |= 4503599627370496uL;
			exp -= 1075;
		}
	}

	public static void GetParts(long value, out int lo, out int hi)
	{
		lo = (int)value;
		hi = (int)(value >>> 32);
	}

	public static void GetParts(int value, out short lo, out short hi)
	{
		lo = (short)value;
		hi = (short)(value >> 16);
	}

	[CLSCompliant(false)]
	public static void GetParts(ulong value, out uint lo, out uint hi)
	{
		lo = (uint)value;
		hi = (uint)(value >> 32);
	}

	[CLSCompliant(false)]
	public static void GetParts(uint value, out ushort lo, out ushort hi)
	{
		lo = (ushort)value;
		hi = (ushort)(value >> 16);
	}

	public static void GetParts(float value, out int sign, out int mantissa, out int exponent, out bool finite)
	{
		if (value.CompareTo(0f) == 0)
		{
			sign = 0;
			mantissa = 0;
			exponent = 1;
			finite = true;
			return;
		}
		int num = SingleAsInt32(value);
		sign = ((num >= 0) ? 1 : (-1));
		exponent = (num >> 23) & 0xFF;
		if (exponent == 2047)
		{
			finite = false;
			mantissa = 0;
			return;
		}
		finite = true;
		mantissa = num & 0xFFFFFF;
		if (exponent == 0)
		{
			exponent = 1;
		}
		else
		{
			mantissa |= 8388608;
		}
		exponent -= 150;
		if (mantissa != 0)
		{
			while ((mantissa & 1) == 0)
			{
				mantissa >>= 1;
				exponent++;
			}
		}
	}

	public static void GetParts(double value, out int sign, out long mantissa, out int exponent, out bool finite)
	{
		GetDoubleParts(value, out sign, out exponent, out var man, out finite);
		mantissa = (long)man;
	}

	internal static int CbitHighZero(uint u)
	{
		if (u == 0)
		{
			return 32;
		}
		int num = 0;
		if ((u & 0xFFFF0000u) == 0)
		{
			num += 16;
			u <<= 16;
		}
		if ((u & 0xFF000000u) == 0)
		{
			num += 8;
			u <<= 8;
		}
		if ((u & 0xF0000000u) == 0)
		{
			num += 4;
			u <<= 4;
		}
		if ((u & 0xC0000000u) == 0)
		{
			num += 2;
			u <<= 2;
		}
		if ((u & 0x80000000u) == 0)
		{
			num++;
		}
		return num;
	}

	internal static int CbitHighZero(ulong uu)
	{
		if ((uu & 0xFFFFFFFF00000000uL) == 0)
		{
			return 32 + CbitHighZero((uint)uu);
		}
		return CbitHighZero((uint)(uu >> 32));
	}

	internal static int CbitLowZero(uint u)
	{
		if (u == 0)
		{
			return 32;
		}
		int num = 0;
		if ((u & 0xFFFF) == 0)
		{
			num += 16;
			u >>= 16;
		}
		if ((u & 0xFF) == 0)
		{
			num += 8;
			u >>= 8;
		}
		if ((u & 0xF) == 0)
		{
			num += 4;
			u >>= 4;
		}
		if ((u & 3) == 0)
		{
			num += 2;
			u >>= 2;
		}
		if ((u & 1) == 0)
		{
			num++;
		}
		return num;
	}

	public static int GCD(int left, int right)
	{
		if (left < 0)
		{
			left = -left;
		}
		if (right < 0)
		{
			right = -right;
		}
		return (int)GCD((uint)left, (uint)right);
	}

	[CLSCompliant(false)]
	public static uint GCD(uint left, uint right)
	{
		if (left < right)
		{
			Swap(ref left, ref right);
		}
		while (right != 0)
		{
			int num = 32;
			while (true)
			{
				left -= right;
				if (left < right)
				{
					break;
				}
				if (--num == 0)
				{
					left %= right;
					break;
				}
			}
			Swap(ref left, ref right);
		}
		return left;
	}

	public static long GCD(long left, long right)
	{
		if (left < 0)
		{
			left = -left;
		}
		if (right < 0)
		{
			right = -right;
		}
		return (long)GCD((ulong)left, (ulong)right);
	}

	[CLSCompliant(false)]
	public static ulong GCD(ulong uu1, ulong uu2)
	{
		if (uu1 < uu2)
		{
			Swap(ref uu1, ref uu2);
		}
		while (uu1 > uint.MaxValue)
		{
			if (uu2 == 0)
			{
				return uu1;
			}
			int num = 32;
			while (true)
			{
				uu1 -= uu2;
				if (uu1 < uu2)
				{
					break;
				}
				if (--num == 0)
				{
					uu1 %= uu2;
					break;
				}
			}
			Swap(ref uu1, ref uu2);
		}
		return GCD((uint)uu1, (uint)uu2);
	}

	[DebuggerNonUserCode]
	public static int Log2(int number)
	{
		if (number < 0)
		{
			throw new ArgumentOutOfRangeException("number", "The logarithm of a negative number is imaginary.");
		}
		return Log2((uint)number);
	}

	[CLSCompliant(false)]
	[DebuggerNonUserCode]
	public static int Log2(uint number)
	{
		if (number == 0)
		{
			throw new ArgumentOutOfRangeException("number", "The logarithm of zero is not defined.");
		}
		number |= number >> 1;
		number |= number >> 2;
		number |= number >> 4;
		number |= number >> 8;
		number |= number >> 16;
		return PopulationCount(number >> 1);
	}

	[DebuggerNonUserCode]
	public static int Log2(long number)
	{
		if (number < 0)
		{
			throw new ArgumentOutOfRangeException("number", "The logarithm of a negative number is imaginary.");
		}
		return Log2((ulong)number);
	}

	[CLSCompliant(false)]
	[DebuggerNonUserCode]
	public static int Log2(ulong number)
	{
		if (number == 0)
		{
			throw new ArgumentOutOfRangeException("number", "The logarithm of zero is not defined.");
		}
		number |= number >> 1;
		number |= number >> 2;
		number |= number >> 4;
		number |= number >> 8;
		number |= number >> 16;
		number |= number >> 32;
		return PopulationCount(number >> 1);
	}

	[DebuggerNonUserCode]
	public static int NextPowerOf2(int number)
	{
		if (number < 0)
		{
			return 1;
		}
		return (int)NextPowerOf2((uint)number);
	}

	[CLSCompliant(false)]
	public static uint NextPowerOf2(uint number)
	{
		number |= number >> 1;
		number |= number >> 2;
		number |= number >> 4;
		number |= number >> 8;
		number |= number >> 16;
		return number + 1;
	}

	public static float Round(this float number, int decimals)
	{
		return (float)Math.Round(number, decimals);
	}

	public static double Round(this double number, int decimals)
	{
		return Math.Round(number, decimals);
	}

	public static decimal Round(this decimal number, int decimals)
	{
		return Math.Round(number, decimals);
	}

	public static float Round(this float number, int decimals, MidpointRounding mode)
	{
		return (float)Math.Round(number, decimals, mode);
	}

	public static double Round(this double number, int decimals, MidpointRounding mode)
	{
		return Math.Round(number, decimals, mode);
	}

	public static decimal Round(this decimal number, int decimals, MidpointRounding mode)
	{
		return Math.Round(number, decimals, mode);
	}

	public static float Round(this float number)
	{
		return (float)Math.Round(number);
	}

	public static double Round(this double number)
	{
		return Math.Round(number);
	}

	public static decimal Round(this decimal number)
	{
		return Math.Round(number);
	}

	public static float Round(this float number, MidpointRounding mode)
	{
		return (float)Math.Round(number, mode);
	}

	public static double Round(this double number, MidpointRounding mode)
	{
		return Math.Round(number, mode);
	}

	public static decimal Round(this decimal number, MidpointRounding mode)
	{
		return Math.Round(number, mode);
	}

	[DebuggerNonUserCode]
	public static int Sqrt(int number)
	{
		int num = number >> 1;
		while (true)
		{
			int num2 = num + number / num >> 1;
			if (num2 >= num)
			{
				break;
			}
			num = num2;
		}
		return num;
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		T val = a;
		a = b;
		b = val;
	}

	internal static uint GetHi(ulong uu)
	{
		return (uint)(uu >> 32);
	}

	internal static uint GetLo(ulong uu)
	{
		return (uint)uu;
	}
}
