using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace System;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ValueTuple : IEquatable<ValueTuple>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple>, System.ITupleInternal
{
	int System.ITupleInternal.Size => 0;

	public override bool Equals(object obj)
	{
		return obj is ValueTuple;
	}

	public bool Equals(ValueTuple other)
	{
		return true;
	}

	bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
	{
		return other is ValueTuple;
	}

	int IComparable.CompareTo(object other)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return 0;
	}

	public int CompareTo(ValueTuple other)
	{
		return 0;
	}

	int IStructuralComparable.CompareTo(object other, IComparer comparer)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return 0;
	}

	public override int GetHashCode()
	{
		return 0;
	}

	int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
	{
		return 0;
	}

	int System.ITupleInternal.GetHashCode(IEqualityComparer comparer)
	{
		return 0;
	}

	public override string ToString()
	{
		return "()";
	}

	string System.ITupleInternal.ToStringEnd()
	{
		return ")";
	}

	public static ValueTuple Create()
	{
		return default(ValueTuple);
	}

	public static ValueTuple<T1> Create<T1>(T1 item1)
	{
		return new ValueTuple<T1>(item1);
	}

	public static (T1, T2) Create<T1, T2>(T1 item1, T2 item2)
	{
		return (item1, item2);
	}

	public static (T1, T2, T3) Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
	{
		return (item1, item2, item3);
	}

	public static (T1, T2, T3, T4) Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
	{
		return (item1, item2, item3, item4);
	}

	public static (T1, T2, T3, T4, T5) Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
	{
		return (item1, item2, item3, item4, item5);
	}

	public static (T1, T2, T3, T4, T5, T6) Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
	{
		return (item1, item2, item3, item4, item5, item6);
	}

	public static (T1, T2, T3, T4, T5, T6, T7) Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
	{
		return (item1, item2, item3, item4, item5, item6, item7);
	}

	public static (T1, T2, T3, T4, T5, T6, T7, T8) Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
	{
		return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>(item1, item2, item3, item4, item5, item6, item7, Create(item8));
	}

	internal static int CombineHashCodes(int h1, int h2)
	{
		return NumericsHelpers.CombineHash(NumericsHelpers.CombineHash(Guid.NewGuid().GetHashCode(), h1), h2);
	}

	internal static int CombineHashCodes(int h1, int h2, int h3)
	{
		return NumericsHelpers.CombineHash(CombineHashCodes(h1, h2), h3);
	}

	internal static int CombineHashCodes(int h1, int h2, int h3, int h4)
	{
		return NumericsHelpers.CombineHash(CombineHashCodes(h1, h2, h3), h4);
	}

	internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
	{
		return NumericsHelpers.CombineHash(CombineHashCodes(h1, h2, h3, h4), h5);
	}

	internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
	{
		return NumericsHelpers.CombineHash(CombineHashCodes(h1, h2, h3, h4, h5), h6);
	}

	internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
	{
		return NumericsHelpers.CombineHash(CombineHashCodes(h1, h2, h3, h4, h5, h6), h7);
	}

	internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
	{
		return NumericsHelpers.CombineHash(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7), h8);
	}
}
public struct ValueTuple<T1>(T1 item1) : IEquatable<ValueTuple<T1>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1>>, System.ITupleInternal
{
	public T1 Item1 = item1;

	int System.ITupleInternal.Size => 1;

	public override bool Equals(object obj)
	{
		if (obj is ValueTuple<T1>)
		{
			return Equals((ValueTuple<T1>)obj);
		}
		return false;
	}

	public bool Equals(ValueTuple<T1> other)
	{
		return EqualityComparer<T1>.Default.Equals(Item1, other.Item1);
	}

	bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
	{
		if (other == null || !(other is ValueTuple<T1> valueTuple))
		{
			return false;
		}
		return comparer.Equals(Item1, valueTuple.Item1);
	}

	int IComparable.CompareTo(object other)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple<T1> valueTuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return Comparer<T1>.Default.Compare(Item1, valueTuple.Item1);
	}

	public int CompareTo(ValueTuple<T1> other)
	{
		return Comparer<T1>.Default.Compare(Item1, other.Item1);
	}

	int IStructuralComparable.CompareTo(object other, IComparer comparer)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple<T1> valueTuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return comparer.Compare(Item1, valueTuple.Item1);
	}

	public override int GetHashCode()
	{
		return EqualityComparer<T1>.Default.GetHashCode(Item1);
	}

	int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
	{
		return comparer.GetHashCode(Item1);
	}

	int System.ITupleInternal.GetHashCode(IEqualityComparer comparer)
	{
		return comparer.GetHashCode(Item1);
	}

	public override string ToString()
	{
		return "(" + (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ")";
	}

	string System.ITupleInternal.ToStringEnd()
	{
		return (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ")";
	}
}
[StructLayout(LayoutKind.Auto)]
public struct ValueTuple<T1, T2>(T1 item1, T2 item2) : IEquatable<(T1, T2)>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<(T1, T2)>, System.ITupleInternal
{
	public T1 Item1 = item1;

	public T2 Item2 = item2;

	int System.ITupleInternal.Size => 2;

	public override bool Equals(object obj)
	{
		if (obj is ValueTuple<T1, T2>)
		{
			return Equals(((T1, T2))obj);
		}
		return false;
	}

	public bool Equals((T1, T2) other)
	{
		if (EqualityComparer<T1>.Default.Equals(Item1, other.Item1))
		{
			return EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
		}
		return false;
	}

	bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
	{
		if (other == null || !(other is (T1, T2) tuple))
		{
			return false;
		}
		if (comparer.Equals(Item1, tuple.Item1))
		{
			return comparer.Equals(Item2, tuple.Item2);
		}
		return false;
	}

	int IComparable.CompareTo(object other)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple<T1, T2>))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return CompareTo(((T1, T2))other);
	}

	public int CompareTo((T1, T2) other)
	{
		int num = Comparer<T1>.Default.Compare(Item1, other.Item1);
		if (num != 0)
		{
			return num;
		}
		return Comparer<T2>.Default.Compare(Item2, other.Item2);
	}

	int IStructuralComparable.CompareTo(object other, IComparer comparer)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is (T1, T2) tuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		int num = comparer.Compare(Item1, tuple.Item1);
		if (num != 0)
		{
			return num;
		}
		return comparer.Compare(Item2, tuple.Item2);
	}

	public override int GetHashCode()
	{
		return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1), EqualityComparer<T2>.Default.GetHashCode(Item2));
	}

	int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	private int GetHashCodeCore(IEqualityComparer comparer)
	{
		return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2));
	}

	int System.ITupleInternal.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	public override string ToString()
	{
		return "(" + (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ")";
	}

	string System.ITupleInternal.ToStringEnd()
	{
		return (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ")";
	}
}
[StructLayout(LayoutKind.Auto)]
public struct ValueTuple<T1, T2, T3>(T1 item1, T2 item2, T3 item3) : IEquatable<(T1, T2, T3)>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<(T1, T2, T3)>, System.ITupleInternal
{
	public T1 Item1 = item1;

	public T2 Item2 = item2;

	public T3 Item3 = item3;

	int System.ITupleInternal.Size => 3;

	public override bool Equals(object obj)
	{
		if (obj is ValueTuple<T1, T2, T3>)
		{
			return Equals(((T1, T2, T3))obj);
		}
		return false;
	}

	public bool Equals((T1, T2, T3) other)
	{
		if (EqualityComparer<T1>.Default.Equals(Item1, other.Item1) && EqualityComparer<T2>.Default.Equals(Item2, other.Item2))
		{
			return EqualityComparer<T3>.Default.Equals(Item3, other.Item3);
		}
		return false;
	}

	bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
	{
		if (other == null || !(other is (T1, T2, T3) tuple))
		{
			return false;
		}
		if (comparer.Equals(Item1, tuple.Item1) && comparer.Equals(Item2, tuple.Item2))
		{
			return comparer.Equals(Item3, tuple.Item3);
		}
		return false;
	}

	int IComparable.CompareTo(object other)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple<T1, T2, T3>))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return CompareTo(((T1, T2, T3))other);
	}

	public int CompareTo((T1, T2, T3) other)
	{
		int num = Comparer<T1>.Default.Compare(Item1, other.Item1);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T2>.Default.Compare(Item2, other.Item2);
		if (num != 0)
		{
			return num;
		}
		return Comparer<T3>.Default.Compare(Item3, other.Item3);
	}

	int IStructuralComparable.CompareTo(object other, IComparer comparer)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is (T1, T2, T3) tuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		int num = comparer.Compare(Item1, tuple.Item1);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item2, tuple.Item2);
		if (num != 0)
		{
			return num;
		}
		return comparer.Compare(Item3, tuple.Item3);
	}

	public override int GetHashCode()
	{
		return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1), EqualityComparer<T2>.Default.GetHashCode(Item2), EqualityComparer<T3>.Default.GetHashCode(Item3));
	}

	int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	private int GetHashCodeCore(IEqualityComparer comparer)
	{
		return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3));
	}

	int System.ITupleInternal.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	public override string ToString()
	{
		return "(" + (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ")";
	}

	string System.ITupleInternal.ToStringEnd()
	{
		return (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ")";
	}
}
[StructLayout(LayoutKind.Auto)]
public struct ValueTuple<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) : IEquatable<(T1, T2, T3, T4)>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<(T1, T2, T3, T4)>, System.ITupleInternal
{
	public T1 Item1 = item1;

	public T2 Item2 = item2;

	public T3 Item3 = item3;

	public T4 Item4 = item4;

	int System.ITupleInternal.Size => 4;

	public override bool Equals(object obj)
	{
		if (obj is ValueTuple<T1, T2, T3, T4>)
		{
			return Equals(((T1, T2, T3, T4))obj);
		}
		return false;
	}

	public bool Equals((T1, T2, T3, T4) other)
	{
		if (EqualityComparer<T1>.Default.Equals(Item1, other.Item1) && EqualityComparer<T2>.Default.Equals(Item2, other.Item2) && EqualityComparer<T3>.Default.Equals(Item3, other.Item3))
		{
			return EqualityComparer<T4>.Default.Equals(Item4, other.Item4);
		}
		return false;
	}

	bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
	{
		if (other == null || !(other is (T1, T2, T3, T4) tuple))
		{
			return false;
		}
		if (comparer.Equals(Item1, tuple.Item1) && comparer.Equals(Item2, tuple.Item2) && comparer.Equals(Item3, tuple.Item3))
		{
			return comparer.Equals(Item4, tuple.Item4);
		}
		return false;
	}

	int IComparable.CompareTo(object other)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple<T1, T2, T3, T4>))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return CompareTo(((T1, T2, T3, T4))other);
	}

	public int CompareTo((T1, T2, T3, T4) other)
	{
		int num = Comparer<T1>.Default.Compare(Item1, other.Item1);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T2>.Default.Compare(Item2, other.Item2);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T3>.Default.Compare(Item3, other.Item3);
		if (num != 0)
		{
			return num;
		}
		return Comparer<T4>.Default.Compare(Item4, other.Item4);
	}

	int IStructuralComparable.CompareTo(object other, IComparer comparer)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is (T1, T2, T3, T4) tuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		int num = comparer.Compare(Item1, tuple.Item1);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item2, tuple.Item2);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item3, tuple.Item3);
		if (num != 0)
		{
			return num;
		}
		return comparer.Compare(Item4, tuple.Item4);
	}

	public override int GetHashCode()
	{
		return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1), EqualityComparer<T2>.Default.GetHashCode(Item2), EqualityComparer<T3>.Default.GetHashCode(Item3), EqualityComparer<T4>.Default.GetHashCode(Item4));
	}

	int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	private int GetHashCodeCore(IEqualityComparer comparer)
	{
		return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4));
	}

	int System.ITupleInternal.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	public override string ToString()
	{
		return "(" + (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ")";
	}

	string System.ITupleInternal.ToStringEnd()
	{
		return (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ")";
	}
}
[StructLayout(LayoutKind.Auto)]
public struct ValueTuple<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) : IEquatable<(T1, T2, T3, T4, T5)>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<(T1, T2, T3, T4, T5)>, System.ITupleInternal
{
	public T1 Item1 = item1;

	public T2 Item2 = item2;

	public T3 Item3 = item3;

	public T4 Item4 = item4;

	public T5 Item5 = item5;

	int System.ITupleInternal.Size => 5;

	public override bool Equals(object obj)
	{
		if (obj is ValueTuple<T1, T2, T3, T4, T5>)
		{
			return Equals(((T1, T2, T3, T4, T5))obj);
		}
		return false;
	}

	public bool Equals((T1, T2, T3, T4, T5) other)
	{
		if (EqualityComparer<T1>.Default.Equals(Item1, other.Item1) && EqualityComparer<T2>.Default.Equals(Item2, other.Item2) && EqualityComparer<T3>.Default.Equals(Item3, other.Item3) && EqualityComparer<T4>.Default.Equals(Item4, other.Item4))
		{
			return EqualityComparer<T5>.Default.Equals(Item5, other.Item5);
		}
		return false;
	}

	bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
	{
		if (other == null || !(other is (T1, T2, T3, T4, T5) tuple))
		{
			return false;
		}
		if (comparer.Equals(Item1, tuple.Item1) && comparer.Equals(Item2, tuple.Item2) && comparer.Equals(Item3, tuple.Item3) && comparer.Equals(Item4, tuple.Item4))
		{
			return comparer.Equals(Item5, tuple.Item5);
		}
		return false;
	}

	int IComparable.CompareTo(object other)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple<T1, T2, T3, T4, T5>))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return CompareTo(((T1, T2, T3, T4, T5))other);
	}

	public int CompareTo((T1, T2, T3, T4, T5) other)
	{
		int num = Comparer<T1>.Default.Compare(Item1, other.Item1);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T2>.Default.Compare(Item2, other.Item2);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T3>.Default.Compare(Item3, other.Item3);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T4>.Default.Compare(Item4, other.Item4);
		if (num != 0)
		{
			return num;
		}
		return Comparer<T5>.Default.Compare(Item5, other.Item5);
	}

	int IStructuralComparable.CompareTo(object other, IComparer comparer)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is (T1, T2, T3, T4, T5) tuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		int num = comparer.Compare(Item1, tuple.Item1);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item2, tuple.Item2);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item3, tuple.Item3);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item4, tuple.Item4);
		if (num != 0)
		{
			return num;
		}
		return comparer.Compare(Item5, tuple.Item5);
	}

	public override int GetHashCode()
	{
		return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1), EqualityComparer<T2>.Default.GetHashCode(Item2), EqualityComparer<T3>.Default.GetHashCode(Item3), EqualityComparer<T4>.Default.GetHashCode(Item4), EqualityComparer<T5>.Default.GetHashCode(Item5));
	}

	int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	private int GetHashCodeCore(IEqualityComparer comparer)
	{
		return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5));
	}

	int System.ITupleInternal.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	public override string ToString()
	{
		return "(" + (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ", " + (object.ReferenceEquals(Item5, null) ? "" : Item5.ToString()) + ")";
	}

	string System.ITupleInternal.ToStringEnd()
	{
		return (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ", " + (object.ReferenceEquals(Item5, null) ? "" : Item5.ToString()) + ")";
	}
}
[StructLayout(LayoutKind.Auto)]
public struct ValueTuple<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) : IEquatable<(T1, T2, T3, T4, T5, T6)>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<(T1, T2, T3, T4, T5, T6)>, System.ITupleInternal
{
	public T1 Item1 = item1;

	public T2 Item2 = item2;

	public T3 Item3 = item3;

	public T4 Item4 = item4;

	public T5 Item5 = item5;

	public T6 Item6 = item6;

	int System.ITupleInternal.Size => 6;

	public override bool Equals(object obj)
	{
		if (obj is ValueTuple<T1, T2, T3, T4, T5, T6>)
		{
			return Equals(((T1, T2, T3, T4, T5, T6))obj);
		}
		return false;
	}

	public bool Equals((T1, T2, T3, T4, T5, T6) other)
	{
		if (EqualityComparer<T1>.Default.Equals(Item1, other.Item1) && EqualityComparer<T2>.Default.Equals(Item2, other.Item2) && EqualityComparer<T3>.Default.Equals(Item3, other.Item3) && EqualityComparer<T4>.Default.Equals(Item4, other.Item4) && EqualityComparer<T5>.Default.Equals(Item5, other.Item5))
		{
			return EqualityComparer<T6>.Default.Equals(Item6, other.Item6);
		}
		return false;
	}

	bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
	{
		if (other == null || !(other is (T1, T2, T3, T4, T5, T6) tuple))
		{
			return false;
		}
		if (comparer.Equals(Item1, tuple.Item1) && comparer.Equals(Item2, tuple.Item2) && comparer.Equals(Item3, tuple.Item3) && comparer.Equals(Item4, tuple.Item4) && comparer.Equals(Item5, tuple.Item5))
		{
			return comparer.Equals(Item6, tuple.Item6);
		}
		return false;
	}

	int IComparable.CompareTo(object other)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6>))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return CompareTo(((T1, T2, T3, T4, T5, T6))other);
	}

	public int CompareTo((T1, T2, T3, T4, T5, T6) other)
	{
		int num = Comparer<T1>.Default.Compare(Item1, other.Item1);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T2>.Default.Compare(Item2, other.Item2);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T3>.Default.Compare(Item3, other.Item3);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T4>.Default.Compare(Item4, other.Item4);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T5>.Default.Compare(Item5, other.Item5);
		if (num != 0)
		{
			return num;
		}
		return Comparer<T6>.Default.Compare(Item6, other.Item6);
	}

	int IStructuralComparable.CompareTo(object other, IComparer comparer)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is (T1, T2, T3, T4, T5, T6) tuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		int num = comparer.Compare(Item1, tuple.Item1);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item2, tuple.Item2);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item3, tuple.Item3);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item4, tuple.Item4);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item5, tuple.Item5);
		if (num != 0)
		{
			return num;
		}
		return comparer.Compare(Item6, tuple.Item6);
	}

	public override int GetHashCode()
	{
		return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1), EqualityComparer<T2>.Default.GetHashCode(Item2), EqualityComparer<T3>.Default.GetHashCode(Item3), EqualityComparer<T4>.Default.GetHashCode(Item4), EqualityComparer<T5>.Default.GetHashCode(Item5), EqualityComparer<T6>.Default.GetHashCode(Item6));
	}

	int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	private int GetHashCodeCore(IEqualityComparer comparer)
	{
		return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6));
	}

	int System.ITupleInternal.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	public override string ToString()
	{
		return "(" + (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ", " + (object.ReferenceEquals(Item5, null) ? "" : Item5.ToString()) + ", " + (object.ReferenceEquals(Item6, null) ? "" : Item6.ToString()) + ")";
	}

	string System.ITupleInternal.ToStringEnd()
	{
		return (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ", " + (object.ReferenceEquals(Item5, null) ? "" : Item5.ToString()) + ", " + (object.ReferenceEquals(Item6, null) ? "" : Item6.ToString()) + ")";
	}
}
[StructLayout(LayoutKind.Auto)]
public struct ValueTuple<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) : IEquatable<(T1, T2, T3, T4, T5, T6, T7)>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<(T1, T2, T3, T4, T5, T6, T7)>, System.ITupleInternal
{
	public T1 Item1 = item1;

	public T2 Item2 = item2;

	public T3 Item3 = item3;

	public T4 Item4 = item4;

	public T5 Item5 = item5;

	public T6 Item6 = item6;

	public T7 Item7 = item7;

	int System.ITupleInternal.Size => 7;

	public override bool Equals(object obj)
	{
		if (obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7>)
		{
			return Equals(((T1, T2, T3, T4, T5, T6, T7))obj);
		}
		return false;
	}

	public bool Equals((T1, T2, T3, T4, T5, T6, T7) other)
	{
		if (EqualityComparer<T1>.Default.Equals(Item1, other.Item1) && EqualityComparer<T2>.Default.Equals(Item2, other.Item2) && EqualityComparer<T3>.Default.Equals(Item3, other.Item3) && EqualityComparer<T4>.Default.Equals(Item4, other.Item4) && EqualityComparer<T5>.Default.Equals(Item5, other.Item5) && EqualityComparer<T6>.Default.Equals(Item6, other.Item6))
		{
			return EqualityComparer<T7>.Default.Equals(Item7, other.Item7);
		}
		return false;
	}

	bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
	{
		if (other == null || !(other is (T1, T2, T3, T4, T5, T6, T7) tuple))
		{
			return false;
		}
		if (comparer.Equals(Item1, tuple.Item1) && comparer.Equals(Item2, tuple.Item2) && comparer.Equals(Item3, tuple.Item3) && comparer.Equals(Item4, tuple.Item4) && comparer.Equals(Item5, tuple.Item5) && comparer.Equals(Item6, tuple.Item6))
		{
			return comparer.Equals(Item7, tuple.Item7);
		}
		return false;
	}

	int IComparable.CompareTo(object other)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7>))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return CompareTo(((T1, T2, T3, T4, T5, T6, T7))other);
	}

	public int CompareTo((T1, T2, T3, T4, T5, T6, T7) other)
	{
		int num = Comparer<T1>.Default.Compare(Item1, other.Item1);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T2>.Default.Compare(Item2, other.Item2);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T3>.Default.Compare(Item3, other.Item3);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T4>.Default.Compare(Item4, other.Item4);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T5>.Default.Compare(Item5, other.Item5);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T6>.Default.Compare(Item6, other.Item6);
		if (num != 0)
		{
			return num;
		}
		return Comparer<T7>.Default.Compare(Item7, other.Item7);
	}

	int IStructuralComparable.CompareTo(object other, IComparer comparer)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is (T1, T2, T3, T4, T5, T6, T7) tuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		int num = comparer.Compare(Item1, tuple.Item1);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item2, tuple.Item2);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item3, tuple.Item3);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item4, tuple.Item4);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item5, tuple.Item5);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item6, tuple.Item6);
		if (num != 0)
		{
			return num;
		}
		return comparer.Compare(Item7, tuple.Item7);
	}

	public override int GetHashCode()
	{
		return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1), EqualityComparer<T2>.Default.GetHashCode(Item2), EqualityComparer<T3>.Default.GetHashCode(Item3), EqualityComparer<T4>.Default.GetHashCode(Item4), EqualityComparer<T5>.Default.GetHashCode(Item5), EqualityComparer<T6>.Default.GetHashCode(Item6), EqualityComparer<T7>.Default.GetHashCode(Item7));
	}

	int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	private int GetHashCodeCore(IEqualityComparer comparer)
	{
		return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7));
	}

	int System.ITupleInternal.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	public override string ToString()
	{
		return "(" + (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ", " + (object.ReferenceEquals(Item5, null) ? "" : Item5.ToString()) + ", " + (object.ReferenceEquals(Item6, null) ? "" : Item6.ToString()) + ", " + (object.ReferenceEquals(Item7, null) ? "" : Item7.ToString()) + ")";
	}

	string System.ITupleInternal.ToStringEnd()
	{
		return (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ", " + (object.ReferenceEquals(Item5, null) ? "" : Item5.ToString()) + ", " + (object.ReferenceEquals(Item6, null) ? "" : Item6.ToString()) + ", " + (object.ReferenceEquals(Item7, null) ? "" : Item7.ToString()) + ")";
	}
}
[StructLayout(LayoutKind.Auto)]
public struct ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>, System.ITupleInternal where TRest : struct
{
	public T1 Item1;

	public T2 Item2;

	public T3 Item3;

	public T4 Item4;

	public T5 Item5;

	public T6 Item6;

	public T7 Item7;

	public TRest Rest;

	int System.ITupleInternal.Size
	{
		get
		{
			if ((object)Rest is System.ITupleInternal tupleInternal)
			{
				return 7 + tupleInternal.Size;
			}
			return 8;
		}
	}

	public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
	{
		if (!(rest is System.ITupleInternal))
		{
			throw new ArgumentException("The TRest type argument of ValueTuple`8 must be a ValueTuple.");
		}
		Item1 = item1;
		Item2 = item2;
		Item3 = item3;
		Item4 = item4;
		Item5 = item5;
		Item6 = item6;
		Item7 = item7;
		Rest = rest;
	}

	public override bool Equals(object obj)
	{
		if (obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)
		{
			return Equals((ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)obj);
		}
		return false;
	}

	public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
	{
		if (EqualityComparer<T1>.Default.Equals(Item1, other.Item1) && EqualityComparer<T2>.Default.Equals(Item2, other.Item2) && EqualityComparer<T3>.Default.Equals(Item3, other.Item3) && EqualityComparer<T4>.Default.Equals(Item4, other.Item4) && EqualityComparer<T5>.Default.Equals(Item5, other.Item5) && EqualityComparer<T6>.Default.Equals(Item6, other.Item6) && EqualityComparer<T7>.Default.Equals(Item7, other.Item7))
		{
			return EqualityComparer<TRest>.Default.Equals(Rest, other.Rest);
		}
		return false;
	}

	bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
	{
		if (other == null || !(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> valueTuple))
		{
			return false;
		}
		if (comparer.Equals(Item1, valueTuple.Item1) && comparer.Equals(Item2, valueTuple.Item2) && comparer.Equals(Item3, valueTuple.Item3) && comparer.Equals(Item4, valueTuple.Item4) && comparer.Equals(Item5, valueTuple.Item5) && comparer.Equals(Item6, valueTuple.Item6) && comparer.Equals(Item7, valueTuple.Item7))
		{
			return comparer.Equals(Rest, valueTuple.Rest);
		}
		return false;
	}

	int IComparable.CompareTo(object other)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		return CompareTo((ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other);
	}

	public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
	{
		int num = Comparer<T1>.Default.Compare(Item1, other.Item1);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T2>.Default.Compare(Item2, other.Item2);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T3>.Default.Compare(Item3, other.Item3);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T4>.Default.Compare(Item4, other.Item4);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T5>.Default.Compare(Item5, other.Item5);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T6>.Default.Compare(Item6, other.Item6);
		if (num != 0)
		{
			return num;
		}
		num = Comparer<T7>.Default.Compare(Item7, other.Item7);
		if (num != 0)
		{
			return num;
		}
		return Comparer<TRest>.Default.Compare(Rest, other.Rest);
	}

	int IStructuralComparable.CompareTo(object other, IComparer comparer)
	{
		if (other == null)
		{
			return 1;
		}
		if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> valueTuple))
		{
			throw new ArgumentException("The parameter should be a ValueTuple type of appropriate arity.", "other");
		}
		int num = comparer.Compare(Item1, valueTuple.Item1);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item2, valueTuple.Item2);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item3, valueTuple.Item3);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item4, valueTuple.Item4);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item5, valueTuple.Item5);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item6, valueTuple.Item6);
		if (num != 0)
		{
			return num;
		}
		num = comparer.Compare(Item7, valueTuple.Item7);
		if (num != 0)
		{
			return num;
		}
		return comparer.Compare(Rest, valueTuple.Rest);
	}

	public override int GetHashCode()
	{
		if (!((object)Rest is System.ITupleInternal { Size: var size } tupleInternal))
		{
			return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1), EqualityComparer<T2>.Default.GetHashCode(Item2), EqualityComparer<T3>.Default.GetHashCode(Item3), EqualityComparer<T4>.Default.GetHashCode(Item4), EqualityComparer<T5>.Default.GetHashCode(Item5), EqualityComparer<T6>.Default.GetHashCode(Item6), EqualityComparer<T7>.Default.GetHashCode(Item7));
		}
		if (size >= 8)
		{
			return tupleInternal.GetHashCode();
		}
		switch (8 - size)
		{
		case 1:
			return ValueTuple.CombineHashCodes(EqualityComparer<T7>.Default.GetHashCode(Item7), tupleInternal.GetHashCode());
		case 2:
			return ValueTuple.CombineHashCodes(EqualityComparer<T6>.Default.GetHashCode(Item6), EqualityComparer<T7>.Default.GetHashCode(Item7), tupleInternal.GetHashCode());
		case 3:
			return ValueTuple.CombineHashCodes(EqualityComparer<T5>.Default.GetHashCode(Item5), EqualityComparer<T6>.Default.GetHashCode(Item6), EqualityComparer<T7>.Default.GetHashCode(Item7), tupleInternal.GetHashCode());
		case 4:
			return ValueTuple.CombineHashCodes(EqualityComparer<T4>.Default.GetHashCode(Item4), EqualityComparer<T5>.Default.GetHashCode(Item5), EqualityComparer<T6>.Default.GetHashCode(Item6), EqualityComparer<T7>.Default.GetHashCode(Item7), tupleInternal.GetHashCode());
		case 5:
			return ValueTuple.CombineHashCodes(EqualityComparer<T3>.Default.GetHashCode(Item3), EqualityComparer<T4>.Default.GetHashCode(Item4), EqualityComparer<T5>.Default.GetHashCode(Item5), EqualityComparer<T6>.Default.GetHashCode(Item6), EqualityComparer<T7>.Default.GetHashCode(Item7), tupleInternal.GetHashCode());
		case 6:
			return ValueTuple.CombineHashCodes(EqualityComparer<T2>.Default.GetHashCode(Item2), EqualityComparer<T3>.Default.GetHashCode(Item3), EqualityComparer<T4>.Default.GetHashCode(Item4), EqualityComparer<T5>.Default.GetHashCode(Item5), EqualityComparer<T6>.Default.GetHashCode(Item6), EqualityComparer<T7>.Default.GetHashCode(Item7), tupleInternal.GetHashCode());
		case 7:
		case 8:
			return ValueTuple.CombineHashCodes(EqualityComparer<T1>.Default.GetHashCode(Item1), EqualityComparer<T2>.Default.GetHashCode(Item2), EqualityComparer<T3>.Default.GetHashCode(Item3), EqualityComparer<T4>.Default.GetHashCode(Item4), EqualityComparer<T5>.Default.GetHashCode(Item5), EqualityComparer<T6>.Default.GetHashCode(Item6), EqualityComparer<T7>.Default.GetHashCode(Item7), tupleInternal.GetHashCode());
		default:
			return -1;
		}
	}

	int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	private int GetHashCodeCore(IEqualityComparer comparer)
	{
		if (!((object)Rest is System.ITupleInternal { Size: var size } tupleInternal))
		{
			return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7));
		}
		if (size >= 8)
		{
			return tupleInternal.GetHashCode(comparer);
		}
		switch (8 - size)
		{
		case 1:
			return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
		case 2:
			return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
		case 3:
			return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
		case 4:
			return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
		case 5:
			return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
		case 6:
			return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
		case 7:
		case 8:
			return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
		default:
			return -1;
		}
	}

	int System.ITupleInternal.GetHashCode(IEqualityComparer comparer)
	{
		return GetHashCodeCore(comparer);
	}

	public override string ToString()
	{
		if ((object)Rest is System.ITupleInternal tupleInternal)
		{
			return "(" + (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ", " + (object.ReferenceEquals(Item5, null) ? "" : Item5.ToString()) + ", " + (object.ReferenceEquals(Item6, null) ? "" : Item6.ToString()) + ", " + (object.ReferenceEquals(Item7, null) ? "" : Item7.ToString()) + ", " + tupleInternal.ToStringEnd();
		}
		return "(" + (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ", " + (object.ReferenceEquals(Item5, null) ? "" : Item5.ToString()) + ", " + (object.ReferenceEquals(Item6, null) ? "" : Item6.ToString()) + ", " + (object.ReferenceEquals(Item7, null) ? "" : Item7.ToString()) + ", " + Rest.ToString() + ")";
	}

	string System.ITupleInternal.ToStringEnd()
	{
		if ((object)Rest is System.ITupleInternal tupleInternal)
		{
			return (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ", " + (object.ReferenceEquals(Item5, null) ? "" : Item5.ToString()) + ", " + (object.ReferenceEquals(Item6, null) ? "" : Item6.ToString()) + ", " + (object.ReferenceEquals(Item7, null) ? "" : Item7.ToString()) + ", " + tupleInternal.ToStringEnd();
		}
		return (object.ReferenceEquals(Item1, null) ? "" : Item1.ToString()) + ", " + (object.ReferenceEquals(Item2, null) ? "" : Item2.ToString()) + ", " + (object.ReferenceEquals(Item3, null) ? "" : Item3.ToString()) + ", " + (object.ReferenceEquals(Item4, null) ? "" : Item4.ToString()) + ", " + (object.ReferenceEquals(Item5, null) ? "" : Item5.ToString()) + ", " + (object.ReferenceEquals(Item6, null) ? "" : Item6.ToString()) + ", " + (object.ReferenceEquals(Item7, null) ? "" : Item7.ToString()) + ", " + Rest.ToString() + ")";
	}
}
