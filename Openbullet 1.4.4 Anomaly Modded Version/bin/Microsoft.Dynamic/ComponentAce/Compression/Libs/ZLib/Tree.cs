// Decompiled with JetBrains decompiler
// Type: ComponentAce.Compression.Libs.ZLib.Tree
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace ComponentAce.Compression.Libs.ZLib;

internal sealed class Tree
{
  private short[] dyn_tree;
  private int max_code;
  private StaticTree stat_desc;

  public short[] DynTree
  {
    get => this.dyn_tree;
    set => this.dyn_tree = value;
  }

  public int MaxCode
  {
    get => this.max_code;
    set => this.max_code = value;
  }

  internal StaticTree StatDesc
  {
    get => this.stat_desc;
    set => this.stat_desc = value;
  }

  internal static int d_code(int dist)
  {
    return dist >= 256 /*0x0100*/ ? (int) ZLibUtil._dist_code[256 /*0x0100*/ + ZLibUtil.URShift(dist, 7)] : (int) ZLibUtil._dist_code[dist];
  }

  private void gen_bitlen(Deflate s)
  {
    short[] dynTree = this.dyn_tree;
    short[] staticTree = this.stat_desc.static_tree;
    int[] extraBits = this.stat_desc.extra_bits;
    int extraBase = this.stat_desc.extra_base;
    int maxLength = this.stat_desc.max_length;
    int num1 = 0;
    for (int index = 0; index <= 15; ++index)
      s.bl_count[index] = (short) 0;
    dynTree[s.heap[s.heap_max] * 2 + 1] = (short) 0;
    int index1;
    for (index1 = s.heap_max + 1; index1 < 573; ++index1)
    {
      int num2 = s.heap[index1];
      int index2 = (int) dynTree[(int) dynTree[num2 * 2 + 1] * 2 + 1] + 1;
      if (index2 > maxLength)
      {
        index2 = maxLength;
        ++num1;
      }
      dynTree[num2 * 2 + 1] = (short) index2;
      if (num2 <= this.max_code)
      {
        ++s.bl_count[index2];
        int num3 = 0;
        if (num2 >= extraBase)
          num3 = extraBits[num2 - extraBase];
        short num4 = dynTree[num2 * 2];
        s.opt_len += (int) num4 * (index2 + num3);
        if (staticTree != null)
          s.static_len += (int) num4 * ((int) staticTree[num2 * 2 + 1] + num3);
      }
    }
    if (num1 == 0)
      return;
    do
    {
      int index3 = maxLength - 1;
      while (s.bl_count[index3] == (short) 0)
        --index3;
      --s.bl_count[index3];
      s.bl_count[index3 + 1] = (short) ((int) s.bl_count[index3 + 1] + 2);
      --s.bl_count[maxLength];
      num1 -= 2;
    }
    while (num1 > 0);
    for (int index4 = maxLength; index4 != 0; --index4)
    {
      int num5 = (int) s.bl_count[index4];
      while (num5 != 0)
      {
        int num6 = s.heap[--index1];
        if (num6 <= this.max_code)
        {
          if ((int) dynTree[num6 * 2 + 1] != index4)
          {
            s.opt_len = (int) ((long) s.opt_len + ((long) index4 - (long) dynTree[num6 * 2 + 1]) * (long) dynTree[num6 * 2]);
            dynTree[num6 * 2 + 1] = (short) index4;
          }
          --num5;
        }
      }
    }
  }

  internal void build_tree(Deflate s)
  {
    short[] dynTree = this.dyn_tree;
    short[] staticTree = this.stat_desc.static_tree;
    int elems = this.stat_desc.elems;
    int max_code = -1;
    s.heap_len = 0;
    s.heap_max = 573;
    for (int index = 0; index < elems; ++index)
    {
      if (dynTree[index * 2] != (short) 0)
      {
        s.heap[++s.heap_len] = max_code = index;
        s.depth[index] = (byte) 0;
      }
      else
        dynTree[index * 2 + 1] = (short) 0;
    }
    while (s.heap_len < 2)
    {
      int[] heap = s.heap;
      int index1 = ++s.heap_len;
      int num1;
      if (max_code >= 2)
        num1 = 0;
      else
        max_code = num1 = max_code + 1;
      int num2 = num1;
      heap[index1] = num1;
      int index2 = num2;
      dynTree[index2 * 2] = (short) 1;
      s.depth[index2] = (byte) 0;
      --s.opt_len;
      if (staticTree != null)
        s.static_len -= (int) staticTree[index2 * 2 + 1];
    }
    this.max_code = max_code;
    for (int k = s.heap_len / 2; k >= 1; --k)
      s.pqdownheap(dynTree, k);
    int index3 = elems;
    do
    {
      int index4 = s.heap[1];
      s.heap[1] = s.heap[s.heap_len--];
      s.pqdownheap(dynTree, 1);
      int index5 = s.heap[1];
      s.heap[--s.heap_max] = index4;
      s.heap[--s.heap_max] = index5;
      dynTree[index3 * 2] = (short) ((int) dynTree[index4 * 2] + (int) dynTree[index5 * 2]);
      s.depth[index3] = (byte) ((uint) Math.Max(s.depth[index4], s.depth[index5]) + 1U);
      dynTree[index4 * 2 + 1] = dynTree[index5 * 2 + 1] = (short) index3;
      s.heap[1] = index3++;
      s.pqdownheap(dynTree, 1);
    }
    while (s.heap_len >= 2);
    s.heap[--s.heap_max] = s.heap[1];
    this.gen_bitlen(s);
    Tree.gen_codes(dynTree, max_code, s.bl_count);
  }

  private static void gen_codes(short[] tree, int max_code, short[] bl_count)
  {
    short[] numArray = new short[16 /*0x10*/];
    short num = 0;
    for (int index = 1; index <= 15; ++index)
      numArray[index] = num = (short) ((int) num + (int) bl_count[index - 1] << 1);
    for (int index = 0; index <= max_code; ++index)
    {
      int len = (int) tree[index * 2 + 1];
      if (len != 0)
        tree[index * 2] = (short) Tree.bi_reverse((int) numArray[len]++, len);
    }
  }

  private static int bi_reverse(int code, int len)
  {
    int number = 0;
    do
    {
      int num = number | code & 1;
      code = ZLibUtil.URShift(code, 1);
      number = num << 1;
    }
    while (--len > 0);
    return ZLibUtil.URShift(number, 1);
  }
}
