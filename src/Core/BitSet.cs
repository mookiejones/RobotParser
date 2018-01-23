using System;
using System.Collections.Generic;
using System.Text;
namespace RobotParser.Core
{
   [Serializable]
public sealed class BitSet : ICloneable
{
	private const int BITS = 64;

	private const int LOG_BITS = 6;

	private const int MOD_MASK = 63;

	private ulong[] _bits;

	public BitSet()
		: this(64)
	{
	}

	[CLSCompliant(false)]
	public BitSet(ulong[] bits)
	{
		this._bits = bits;
	}

	public BitSet(IEnumerable<int> items)
		: this()
	{
		foreach (int item in items)
		{
			this.Add(item);
		}
	}

	public BitSet(int nbits)
	{
		this._bits = new ulong[(nbits - 1 >> 6) + 1];
	}

	public static BitSet Of(int el)
	{
		BitSet bitSet = new BitSet(el + 1);
		bitSet.Add(el);
		return bitSet;
	}

	public static BitSet Of(int a, int b)
	{
		BitSet bitSet = new BitSet(Math.Max(a, b) + 1);
		bitSet.Add(a);
		bitSet.Add(b);
		return bitSet;
	}

	public static BitSet Of(int a, int b, int c)
	{
		BitSet bitSet = new BitSet();
		bitSet.Add(a);
		bitSet.Add(b);
		bitSet.Add(c);
		return bitSet;
	}

	public static BitSet Of(int a, int b, int c, int d)
	{
		BitSet bitSet = new BitSet();
		bitSet.Add(a);
		bitSet.Add(b);
		bitSet.Add(c);
		bitSet.Add(d);
		return bitSet;
	}

	public BitSet Or(BitSet a)
	{
		if (a == null)
		{
			return this;
		}
		BitSet bitSet = (BitSet)this.Clone();
		bitSet.OrInPlace(a);
		return bitSet;
	}

	public void Add(int el)
	{
		int num = BitSet.WordNumber(el);
		if (num >= this._bits.Length)
		{
			this.GrowToInclude(el);
		}
		this._bits[num] |= BitSet.BitMask(el);
	}

	public void GrowToInclude(int bit)
	{
		int size = Math.Max(this._bits.Length << 1, BitSet.NumWordsToHold(bit));
		this.SetSize(size);
	}

	public void OrInPlace(BitSet a)
	{
		if (a != null)
		{
			if (a._bits.Length > this._bits.Length)
			{
				this.SetSize(a._bits.Length);
			}
			int num = Math.Min(this._bits.Length, a._bits.Length);
			for (int num2 = num - 1; num2 >= 0; num2--)
			{
				this._bits[num2] |= a._bits[num2];
			}
		}
	}

	private void SetSize(int nwords)
	{
		Array.Resize<ulong>(ref this._bits, nwords);
	}

	private static ulong BitMask(int bitNumber)
	{
		int num = bitNumber & 0x3F;
		return (ulong)(1L << num);
	}

	public object Clone()
	{
		return new BitSet((ulong[])this._bits.Clone());
	}

	public int Size()
	{
		int num = 0;
		for (int num2 = this._bits.Length - 1; num2 >= 0; num2--)
		{
			ulong num3 = this._bits[num2];
			if (num3 != 0)
			{
				for (int num4 = 63; num4 >= 0; num4--)
				{
					if (((long)num3 & 1L << num4) != 0)
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	public override int GetHashCode()
	{
		throw new NotImplementedException();
	}

	public override bool Equals(object other)
	{
		if (other != null && other is BitSet)
		{
			BitSet bitSet = (BitSet)other;
			int num = Math.Min(this._bits.Length, bitSet._bits.Length);
			for (int i = 0; i < num; i++)
			{
				if (this._bits[i] != bitSet._bits[i])
				{
					return false;
				}
			}
			if (this._bits.Length > num)
			{
				for (int j = num + 1; j < this._bits.Length; j++)
				{
					if (this._bits[j] != 0)
					{
						return false;
					}
				}
			}
			else if (bitSet._bits.Length > num)
			{
				for (int k = num + 1; k < bitSet._bits.Length; k++)
				{
					if (bitSet._bits[k] != 0)
					{
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}

	public bool Member(int el)
	{
		if (el < 0)
		{
			return false;
		}
		int num = BitSet.WordNumber(el);
		if (num >= this._bits.Length)
		{
			return false;
		}
		return (this._bits[num] & BitSet.BitMask(el)) != 0;
	}

	public void Remove(int el)
	{
		int num = BitSet.WordNumber(el);
		if (num < this._bits.Length)
		{
			this._bits[num] &= ~BitSet.BitMask(el);
		}
	}

	public bool IsNil()
	{
		for (int num = this._bits.Length - 1; num >= 0; num--)
		{
			if (this._bits[num] != 0)
			{
				return false;
			}
		}
		return true;
	}

	private static int NumWordsToHold(int el)
	{
		return (el >> 6) + 1;
	}

	public int NumBits()
	{
		return this._bits.Length << 6;
	}

	public int LengthInLongWords()
	{
		return this._bits.Length;
	}

	public int[] ToArray()
	{
		int[] array = new int[this.Size()];
		int num = 0;
		for (int i = 0; i < this._bits.Length << 6; i++)
		{
			if (this.Member(i))
			{
				array[num++] = i;
			}
		}
		return array;
	}

	private static int WordNumber(int bit)
	{
		return bit >> 6;
	}

	public override string ToString()
	{
		return this.ToString(null);
	}

	public string ToString(string[] tokenNames)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string value = ",";
		bool flag = false;
		stringBuilder.Append('{');
		for (int i = 0; i < this._bits.Length << 6; i++)
		{
			if (this.Member(i))
			{
				if (i > 0 && flag)
				{
					stringBuilder.Append(value);
				}
				if (tokenNames != null)
				{
					stringBuilder.Append(tokenNames[i]);
				}
				else
				{
					stringBuilder.Append(i);
				}
				flag = true;
			}
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}
}

}