namespace System.Numerics;

internal static class NumericsHelpers
{
	private const int _uintBitCount = 32;

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

	public static void DangerousMakeTwosComplement(uint[] d)
	{
		if (d != null && d.Length > 0)
		{
			d[0] = ~d[0] + 1;
			int i;
			for (i = 1; d[i - 1] == 0 && i < d.Length; i++)
			{
				d[i] = ~d[i] + 1;
			}
			for (; i < d.Length; i++)
			{
				d[i] = ~d[i];
			}
		}
	}

	public static ulong MakeUlong(uint uHi, uint uLo)
	{
		return ((ulong)uHi << 32) | uLo;
	}

	public static uint Abs(int a)
	{
		uint num = (uint)(a >> 31);
		return ((uint)a ^ num) - num;
	}

	public static uint CombineHash(uint u1, uint u2)
	{
		return ((u1 << 7) | (u1 >> 25)) ^ u2;
	}

	public static int CombineHash(int n1, int n2)
	{
		return (int)CombineHash((uint)n1, (uint)n2);
	}

	public static int CbitHighZero(uint u)
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

	public static int CbitHighZero(ulong uu)
	{
		if ((uu & 0xFFFFFFFF00000000uL) == 0)
		{
			return 32 + CbitHighZero((uint)uu);
		}
		return CbitHighZero((uint)(uu >> 32));
	}
}
