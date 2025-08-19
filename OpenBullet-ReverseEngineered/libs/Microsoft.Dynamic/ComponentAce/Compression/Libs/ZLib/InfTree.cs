// Decompiled with JetBrains decompiler
// Type: ComponentAce.Compression.Libs.ZLib.InfTree
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace ComponentAce.Compression.Libs.ZLib;

internal sealed class InfTree
{
  private static int huft_build(
    int[] b,
    int bindex,
    int n,
    int s,
    int[] d,
    int[] e,
    int[] t,
    int[] m,
    int[] hp,
    int[] hn,
    int[] v)
  {
    int[] numArray1 = new int[16 /*0x10*/];
    int[] sourceArray = new int[3];
    int[] numArray2 = new int[15];
    int[] numArray3 = new int[16 /*0x10*/];
    int num1 = 0;
    int num2 = n;
    do
    {
      ++numArray1[b[bindex + num1]];
      ++num1;
      --num2;
    }
    while (num2 != 0);
    if (numArray1[0] == n)
    {
      t[0] = -1;
      m[0] = 0;
      return 0;
    }
    int num3 = m[0];
    int index1 = 1;
    while (index1 <= 15 && numArray1[index1] == 0)
      ++index1;
    int index2 = index1;
    if (num3 < index1)
      num3 = index1;
    int index3 = 15;
    while (index3 != 0 && numArray1[index3] == 0)
      --index3;
    int index4 = index3;
    if (num3 > index3)
      num3 = index3;
    m[0] = num3;
    int num4 = 1 << index1;
    while (index1 < index3)
    {
      int num5;
      if ((num5 = num4 - numArray1[index1]) < 0)
        return -3;
      ++index1;
      num4 = num5 << 1;
    }
    int num6;
    if ((num6 = num4 - numArray1[index3]) < 0)
      return -3;
    numArray1[index3] += num6;
    int num7;
    numArray3[1] = num7 = 0;
    int index5 = 1;
    int index6 = 2;
    while (--index3 != 0)
    {
      numArray3[index6] = (num7 += numArray1[index5]);
      ++index6;
      ++index5;
    }
    int num8 = 0;
    int num9 = 0;
    do
    {
      int index7;
      if ((index7 = b[bindex + num9]) != 0)
        v[numArray3[index7]++] = num8;
      ++num9;
    }
    while (++num8 < n);
    n = numArray3[index4];
    int number1;
    numArray3[0] = number1 = 0;
    int index8 = 0;
    int index9 = -1;
    int bits = -num3;
    numArray2[0] = 0;
    int num10 = 0;
    int num11 = 0;
    for (; index2 <= index4; ++index2)
    {
      int num12 = numArray1[index2];
      while (num12-- != 0)
      {
        while (index2 > bits + num3)
        {
          ++index9;
          bits += num3;
          int num13 = index4 - bits;
          int num14 = num13 > num3 ? num3 : num13;
          int num15;
          int num16;
          if ((num16 = 1 << (num15 = index2 - bits)) > num12 + 1)
          {
            int num17 = num16 - (num12 + 1);
            int index10 = index2;
            if (num15 < num14)
            {
              int num18;
              while (++num15 < num14 && (num18 = num17 << 1) > numArray1[++index10])
                num17 = num18 - numArray1[index10];
            }
          }
          num11 = 1 << num15;
          if (hn[0] + num11 > 1440)
            return -3;
          numArray2[index9] = num10 = hn[0];
          hn[0] += num11;
          if (index9 != 0)
          {
            numArray3[index9] = number1;
            sourceArray[0] = (int) (byte) num15;
            sourceArray[1] = (int) (byte) num3;
            int num19 = ZLibUtil.URShift(number1, bits - num3);
            sourceArray[2] = num10 - numArray2[index9 - 1] - num19;
            Array.Copy((Array) sourceArray, 0, (Array) hp, (numArray2[index9 - 1] + num19) * 3, 3);
          }
          else
            t[0] = num10;
        }
        sourceArray[1] = (int) (byte) (index2 - bits);
        if (index8 >= n)
          sourceArray[0] = 192 /*0xC0*/;
        else if (v[index8] < s)
        {
          sourceArray[0] = v[index8] < 256 /*0x0100*/ ? 0 : 96 /*0x60*/;
          sourceArray[2] = v[index8++];
        }
        else
        {
          sourceArray[0] = (int) (byte) (e[v[index8] - s] + 16 /*0x10*/ + 64 /*0x40*/);
          sourceArray[2] = d[v[index8++] - s];
        }
        int num20 = 1 << index2 - bits;
        for (int index11 = ZLibUtil.URShift(number1, bits); index11 < num11; index11 += num20)
          Array.Copy((Array) sourceArray, 0, (Array) hp, (num10 + index11) * 3, 3);
        int number2;
        for (number2 = 1 << index2 - 1; (number1 & number2) != 0; number2 = ZLibUtil.URShift(number2, 1))
          number1 ^= number2;
        number1 ^= number2;
        for (int index12 = (1 << bits) - 1; (number1 & index12) != numArray3[index9]; index12 = (1 << bits) - 1)
        {
          --index9;
          bits -= num3;
        }
      }
    }
    return num6 == 0 || index4 == 1 ? 0 : -5;
  }

  internal static int inflate_trees_bits(int[] c, int[] bb, int[] tb, int[] hp, ZStream z)
  {
    int[] hn = new int[1];
    int[] v = new int[19];
    int num = InfTree.huft_build(c, 0, 19, 19, (int[]) null, (int[]) null, tb, bb, hp, hn, v);
    if (num == -3)
      z.msg = "oversubscribed dynamic bit lengths tree";
    else if (num == -3 || bb[0] == 0)
    {
      z.msg = "incomplete dynamic bit lengths tree";
      num = -3;
    }
    return num;
  }

  internal static int inflate_trees_dynamic(
    int nl,
    int nd,
    int[] c,
    int[] bl,
    int[] bd,
    int[] tl,
    int[] td,
    int[] hp,
    ZStream z)
  {
    int[] hn = new int[1];
    int[] v = new int[288];
    int num1 = InfTree.huft_build(c, 0, nl, 257, InfTreeUtil.cplens, InfTreeUtil.cplext, tl, bl, hp, hn, v);
    if (num1 != 0 || bl[0] == 0)
    {
      if (num1 == -3)
        z.msg = "oversubscribed literal/length tree";
      else if (num1 != -3)
      {
        z.msg = "incomplete literal/length tree";
        num1 = -3;
      }
      return num1;
    }
    int num2 = InfTree.huft_build(c, nl, nd, 0, InfTreeUtil.cpdist, InfTreeUtil.cpdext, td, bd, hp, hn, v);
    if (num2 == 0 && (bd[0] != 0 || nl <= 257))
      return 0;
    if (num2 == -3)
      z.msg = "oversubscribed distance tree";
    else if (num2 == -3)
    {
      z.msg = "incomplete distance tree";
      num2 = -3;
    }
    else if (num2 != -3)
    {
      z.msg = "empty distance tree with lengths";
      num2 = -3;
    }
    return num2;
  }

  internal static int inflate_trees_fixed(int[] bl, int[] bd, int[][] tl, int[][] td, ZStream z)
  {
    bl[0] = 9;
    bd[0] = 5;
    tl[0] = InfTreeUtil.fixed_tl;
    td[0] = InfTreeUtil.fixed_td;
    return 0;
  }
}
