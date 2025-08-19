// Decompiled with JetBrains decompiler
// Type: ComponentAce.Compression.Libs.ZLib.InfCodes
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace ComponentAce.Compression.Libs.ZLib;

internal sealed class InfCodes
{
  private InflateCodesMode mode;
  private int count;
  private int[] tree;
  internal int tree_index;
  internal int need;
  internal int lit;
  internal int get_Renamed;
  internal int dist;
  private byte lbits;
  private byte dbits;
  private int[] ltree;
  private int ltree_index;
  private int[] dtree;
  private int dtree_index;

  internal InfCodes(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, ZStream z)
  {
    this.mode = InflateCodesMode.START;
    this.lbits = (byte) bl;
    this.dbits = (byte) bd;
    this.ltree = tl;
    this.ltree_index = tl_index;
    this.dtree = td;
    this.dtree_index = td_index;
  }

  internal InfCodes(int bl, int bd, int[] tl, int[] td, ZStream z)
  {
    this.mode = InflateCodesMode.START;
    this.lbits = (byte) bl;
    this.dbits = (byte) bd;
    this.ltree = tl;
    this.ltree_index = 0;
    this.dtree = td;
    this.dtree_index = 0;
  }

  internal int proc(InfBlocks s, ZStream z, int r)
  {
    int nextInIndex = z.next_in_index;
    int availIn = z.avail_in;
    int number = s.BitB;
    int bitK = s.BitK;
    int num1 = s.WritePos;
    int num2 = num1 < s.ReadPos ? s.ReadPos - num1 - 1 : s.End - num1;
    while (true)
    {
      switch (this.mode)
      {
        case InflateCodesMode.START:
          if (num2 >= 258 && availIn >= 10)
          {
            s.BitB = number;
            s.BitK = bitK;
            z.avail_in = availIn;
            z.total_in += (long) (nextInIndex - z.next_in_index);
            z.next_in_index = nextInIndex;
            s.WritePos = num1;
            r = this.inflate_fast((int) this.lbits, (int) this.dbits, this.ltree, this.ltree_index, this.dtree, this.dtree_index, s, z);
            nextInIndex = z.next_in_index;
            availIn = z.avail_in;
            number = s.BitB;
            bitK = s.BitK;
            num1 = s.WritePos;
            num2 = num1 < s.ReadPos ? s.ReadPos - num1 - 1 : s.End - num1;
            int num3;
            switch (r)
            {
              case 0:
                goto label_7;
              case 1:
                num3 = 7;
                break;
              default:
                num3 = 9;
                break;
            }
            this.mode = (InflateCodesMode) num3;
            continue;
          }
label_7:
          this.need = (int) this.lbits;
          this.tree = this.ltree;
          this.tree_index = this.ltree_index;
          this.mode = InflateCodesMode.LEN;
          goto case InflateCodesMode.LEN;
        case InflateCodesMode.LEN:
          int need1;
          for (need1 = this.need; bitK < need1; bitK += 8)
          {
            if (availIn != 0)
            {
              r = 0;
              --availIn;
              number |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << bitK;
            }
            else
            {
              s.BitB = number;
              s.BitK = bitK;
              z.avail_in = availIn;
              z.total_in += (long) (nextInIndex - z.next_in_index);
              z.next_in_index = nextInIndex;
              s.WritePos = num1;
              return s.inflate_flush(z, r);
            }
          }
          int index1 = (this.tree_index + (number & ZLibUtil.inflate_mask[need1])) * 3;
          number = ZLibUtil.URShift(number, this.tree[index1 + 1]);
          bitK -= this.tree[index1 + 1];
          int num4 = this.tree[index1];
          if (num4 == 0)
          {
            this.lit = this.tree[index1 + 2];
            this.mode = InflateCodesMode.LIT;
            continue;
          }
          if ((num4 & 16 /*0x10*/) != 0)
          {
            this.get_Renamed = num4 & 15;
            this.count = this.tree[index1 + 2];
            this.mode = InflateCodesMode.LENEXT;
            continue;
          }
          if ((num4 & 64 /*0x40*/) == 0)
          {
            this.need = num4;
            this.tree_index = index1 / 3 + this.tree[index1 + 2];
            continue;
          }
          if ((num4 & 32 /*0x20*/) != 0)
          {
            this.mode = InflateCodesMode.WASH;
            continue;
          }
          goto label_21;
        case InflateCodesMode.LENEXT:
          int getRenamed1;
          for (getRenamed1 = this.get_Renamed; bitK < getRenamed1; bitK += 8)
          {
            if (availIn != 0)
            {
              r = 0;
              --availIn;
              number |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << bitK;
            }
            else
            {
              s.BitB = number;
              s.BitK = bitK;
              z.avail_in = availIn;
              z.total_in += (long) (nextInIndex - z.next_in_index);
              z.next_in_index = nextInIndex;
              s.WritePos = num1;
              return s.inflate_flush(z, r);
            }
          }
          this.count += number & ZLibUtil.inflate_mask[getRenamed1];
          number >>= getRenamed1;
          bitK -= getRenamed1;
          this.need = (int) this.dbits;
          this.tree = this.dtree;
          this.tree_index = this.dtree_index;
          this.mode = InflateCodesMode.DIST;
          goto case InflateCodesMode.DIST;
        case InflateCodesMode.DIST:
          int need2;
          for (need2 = this.need; bitK < need2; bitK += 8)
          {
            if (availIn != 0)
            {
              r = 0;
              --availIn;
              number |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << bitK;
            }
            else
            {
              s.BitB = number;
              s.BitK = bitK;
              z.avail_in = availIn;
              z.total_in += (long) (nextInIndex - z.next_in_index);
              z.next_in_index = nextInIndex;
              s.WritePos = num1;
              return s.inflate_flush(z, r);
            }
          }
          int index2 = (this.tree_index + (number & ZLibUtil.inflate_mask[need2])) * 3;
          number >>= this.tree[index2 + 1];
          bitK -= this.tree[index2 + 1];
          int num5 = this.tree[index2];
          if ((num5 & 16 /*0x10*/) != 0)
          {
            this.get_Renamed = num5 & 15;
            this.dist = this.tree[index2 + 2];
            this.mode = InflateCodesMode.DISTEXT;
            continue;
          }
          if ((num5 & 64 /*0x40*/) == 0)
          {
            this.need = num5;
            this.tree_index = index2 / 3 + this.tree[index2 + 2];
            continue;
          }
          goto label_37;
        case InflateCodesMode.DISTEXT:
          int getRenamed2;
          for (getRenamed2 = this.get_Renamed; bitK < getRenamed2; bitK += 8)
          {
            if (availIn != 0)
            {
              r = 0;
              --availIn;
              number |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << bitK;
            }
            else
            {
              s.BitB = number;
              s.BitK = bitK;
              z.avail_in = availIn;
              z.total_in += (long) (nextInIndex - z.next_in_index);
              z.next_in_index = nextInIndex;
              s.WritePos = num1;
              return s.inflate_flush(z, r);
            }
          }
          this.dist += number & ZLibUtil.inflate_mask[getRenamed2];
          number >>= getRenamed2;
          bitK -= getRenamed2;
          this.mode = InflateCodesMode.COPY;
          goto case InflateCodesMode.COPY;
        case InflateCodesMode.COPY:
          int num6 = num1 - this.dist;
          while (num6 < 0)
            num6 += s.End;
          for (; this.count != 0; --this.count)
          {
            if (num2 == 0)
            {
              if (num1 == s.End && s.ReadPos != 0)
              {
                num1 = 0;
                num2 = num1 < s.ReadPos ? s.ReadPos - num1 - 1 : s.End - num1;
              }
              if (num2 == 0)
              {
                s.WritePos = num1;
                r = s.inflate_flush(z, r);
                num1 = s.WritePos;
                num2 = num1 < s.ReadPos ? s.ReadPos - num1 - 1 : s.End - num1;
                if (num1 == s.End && s.ReadPos != 0)
                {
                  num1 = 0;
                  num2 = num1 < s.ReadPos ? s.ReadPos - num1 - 1 : s.End - num1;
                }
                if (num2 == 0)
                {
                  s.BitB = number;
                  s.BitK = bitK;
                  z.avail_in = availIn;
                  z.total_in += (long) (nextInIndex - z.next_in_index);
                  z.next_in_index = nextInIndex;
                  s.WritePos = num1;
                  return s.inflate_flush(z, r);
                }
              }
            }
            s.Window[num1++] = s.Window[num6++];
            --num2;
            if (num6 == s.End)
              num6 = 0;
          }
          this.mode = InflateCodesMode.START;
          continue;
        case InflateCodesMode.LIT:
          if (num2 == 0)
          {
            if (num1 == s.End && s.ReadPos != 0)
            {
              num1 = 0;
              num2 = num1 < s.ReadPos ? s.ReadPos - num1 - 1 : s.End - num1;
            }
            if (num2 == 0)
            {
              s.WritePos = num1;
              r = s.inflate_flush(z, r);
              num1 = s.WritePos;
              num2 = num1 < s.ReadPos ? s.ReadPos - num1 - 1 : s.End - num1;
              if (num1 == s.End && s.ReadPos != 0)
              {
                num1 = 0;
                num2 = num1 < s.ReadPos ? s.ReadPos - num1 - 1 : s.End - num1;
              }
              if (num2 == 0)
                goto label_67;
            }
          }
          r = 0;
          s.Window[num1++] = (byte) this.lit;
          --num2;
          this.mode = InflateCodesMode.START;
          continue;
        case InflateCodesMode.WASH:
          goto label_69;
        case InflateCodesMode.END:
          goto label_74;
        case InflateCodesMode.BADCODE:
          goto label_75;
        default:
          goto label_76;
      }
    }
label_21:
    this.mode = InflateCodesMode.BADCODE;
    z.msg = "invalid literal/length code";
    r = -3;
    s.BitB = number;
    s.BitK = bitK;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    s.WritePos = num1;
    return s.inflate_flush(z, r);
label_37:
    this.mode = InflateCodesMode.BADCODE;
    z.msg = "invalid distance code";
    r = -3;
    s.BitB = number;
    s.BitK = bitK;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    s.WritePos = num1;
    return s.inflate_flush(z, r);
label_67:
    s.BitB = number;
    s.BitK = bitK;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    s.WritePos = num1;
    return s.inflate_flush(z, r);
label_69:
    if (bitK > 7)
    {
      bitK -= 8;
      ++availIn;
      --nextInIndex;
    }
    s.WritePos = num1;
    r = s.inflate_flush(z, r);
    num1 = s.WritePos;
    int num7 = num1 < s.ReadPos ? s.ReadPos - num1 - 1 : s.End - num1;
    if (s.ReadPos != s.WritePos)
    {
      s.BitB = number;
      s.BitK = bitK;
      z.avail_in = availIn;
      z.total_in += (long) (nextInIndex - z.next_in_index);
      z.next_in_index = nextInIndex;
      s.WritePos = num1;
      return s.inflate_flush(z, r);
    }
    this.mode = InflateCodesMode.END;
label_74:
    r = 1;
    s.BitB = number;
    s.BitK = bitK;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    s.WritePos = num1;
    return s.inflate_flush(z, r);
label_75:
    r = -3;
    s.BitB = number;
    s.BitK = bitK;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    s.WritePos = num1;
    return s.inflate_flush(z, r);
label_76:
    r = -2;
    s.BitB = number;
    s.BitK = bitK;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    s.WritePos = num1;
    return s.inflate_flush(z, r);
  }

  internal void free(ZStream z)
  {
  }

  internal int inflate_fast(
    int bl,
    int bd,
    int[] tl,
    int tl_index,
    int[] td,
    int td_index,
    InfBlocks s,
    ZStream z)
  {
    int nextInIndex = z.next_in_index;
    int availIn = z.avail_in;
    int num1 = s.BitB;
    int num2 = s.BitK;
    int destinationIndex = s.WritePos;
    int num3 = destinationIndex < s.ReadPos ? s.ReadPos - destinationIndex - 1 : s.End - destinationIndex;
    int num4 = ZLibUtil.inflate_mask[bl];
    int num5 = ZLibUtil.inflate_mask[bd];
    do
    {
      for (; num2 < 20; num2 += 8)
      {
        --availIn;
        num1 |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << num2;
      }
      int num6 = num1 & num4;
      int[] numArray1 = tl;
      int num7 = tl_index;
      int index1;
      if ((index1 = numArray1[(num7 + num6) * 3]) == 0)
      {
        num1 >>= numArray1[(num7 + num6) * 3 + 1];
        num2 -= numArray1[(num7 + num6) * 3 + 1];
        s.Window[destinationIndex++] = (byte) numArray1[(num7 + num6) * 3 + 2];
        --num3;
      }
      else
      {
        do
        {
          num1 >>= numArray1[(num7 + num6) * 3 + 1];
          num2 -= numArray1[(num7 + num6) * 3 + 1];
          if ((index1 & 16 /*0x10*/) != 0)
          {
            int index2 = index1 & 15;
            int length1 = numArray1[(num7 + num6) * 3 + 2] + (num1 & ZLibUtil.inflate_mask[index2]);
            int num8 = num1 >> index2;
            int num9;
            for (num9 = num2 - index2; num9 < 15; num9 += 8)
            {
              --availIn;
              num8 |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << num9;
            }
            int num10 = num8 & num5;
            int[] numArray2 = td;
            int num11 = td_index;
            int index3 = numArray2[(num11 + num10) * 3];
            while (true)
            {
              num8 >>= numArray2[(num11 + num10) * 3 + 1];
              num9 -= numArray2[(num11 + num10) * 3 + 1];
              if ((index3 & 16 /*0x10*/) == 0)
              {
                if ((index3 & 64 /*0x40*/) == 0)
                {
                  num10 = num10 + numArray2[(num11 + num10) * 3 + 2] + (num8 & ZLibUtil.inflate_mask[index3]);
                  index3 = numArray2[(num11 + num10) * 3];
                }
                else
                  goto label_30;
              }
              else
                break;
            }
            int index4;
            for (index4 = index3 & 15; num9 < index4; num9 += 8)
            {
              --availIn;
              num8 |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << num9;
            }
            int num12 = numArray2[(num11 + num10) * 3 + 2] + (num8 & ZLibUtil.inflate_mask[index4]);
            num1 = num8 >> index4;
            num2 = num9 - index4;
            num3 -= length1;
            int sourceIndex1;
            int num13;
            if (destinationIndex >= num12)
            {
              int sourceIndex2 = destinationIndex - num12;
              if (destinationIndex - sourceIndex2 > 0 && 2 > destinationIndex - sourceIndex2)
              {
                byte[] window1 = s.Window;
                int index5 = destinationIndex;
                int num14 = index5 + 1;
                byte[] window2 = s.Window;
                int index6 = sourceIndex2;
                int num15 = index6 + 1;
                int num16 = (int) window2[index6];
                window1[index5] = (byte) num16;
                int num17 = length1 - 1;
                byte[] window3 = s.Window;
                int index7 = num14;
                destinationIndex = index7 + 1;
                byte[] window4 = s.Window;
                int index8 = num15;
                sourceIndex1 = index8 + 1;
                int num18 = (int) window4[index8];
                window3[index7] = (byte) num18;
                length1 = num17 - 1;
              }
              else
              {
                Array.Copy((Array) s.Window, sourceIndex2, (Array) s.Window, destinationIndex, 2);
                destinationIndex += 2;
                sourceIndex1 = sourceIndex2 + 2;
                length1 -= 2;
              }
            }
            else
            {
              sourceIndex1 = destinationIndex - num12;
              do
              {
                sourceIndex1 += s.End;
              }
              while (sourceIndex1 < 0);
              int length2 = s.End - sourceIndex1;
              if (length1 > length2)
              {
                length1 -= length2;
                if (destinationIndex - sourceIndex1 > 0 && length2 > destinationIndex - sourceIndex1)
                {
                  do
                  {
                    s.Window[destinationIndex++] = s.Window[sourceIndex1++];
                  }
                  while (--length2 != 0);
                }
                else
                {
                  Array.Copy((Array) s.Window, sourceIndex1, (Array) s.Window, destinationIndex, length2);
                  destinationIndex += length2;
                  num13 = sourceIndex1 + length2;
                }
                sourceIndex1 = 0;
              }
            }
            if (destinationIndex - sourceIndex1 > 0 && length1 > destinationIndex - sourceIndex1)
            {
              do
              {
                s.Window[destinationIndex++] = s.Window[sourceIndex1++];
              }
              while (--length1 != 0);
              goto label_37;
            }
            Array.Copy((Array) s.Window, sourceIndex1, (Array) s.Window, destinationIndex, length1);
            destinationIndex += length1;
            num13 = sourceIndex1 + length1;
            goto label_37;
label_30:
            z.msg = "invalid distance code";
            int num19 = z.avail_in - availIn;
            int num20 = num9 >> 3 < num19 ? num9 >> 3 : num19;
            int num21 = availIn + num20;
            int num22 = nextInIndex - num20;
            int num23 = num9 - (num20 << 3);
            s.BitB = num8;
            s.BitK = num23;
            z.avail_in = num21;
            z.total_in += (long) (num22 - z.next_in_index);
            z.next_in_index = num22;
            s.WritePos = destinationIndex;
            return -3;
          }
          if ((index1 & 64 /*0x40*/) == 0)
            num6 = num6 + numArray1[(num7 + num6) * 3 + 2] + (num1 & ZLibUtil.inflate_mask[index1]);
          else
            goto label_34;
        }
        while ((index1 = numArray1[(num7 + num6) * 3]) != 0);
        num1 >>= numArray1[(num7 + num6) * 3 + 1];
        num2 -= numArray1[(num7 + num6) * 3 + 1];
        s.Window[destinationIndex++] = (byte) numArray1[(num7 + num6) * 3 + 2];
        --num3;
        goto label_37;
label_34:
        if ((index1 & 32 /*0x20*/) != 0)
        {
          int num24 = z.avail_in - availIn;
          int num25 = num2 >> 3 < num24 ? num2 >> 3 : num24;
          int num26 = availIn + num25;
          int num27 = nextInIndex - num25;
          int num28 = num2 - (num25 << 3);
          s.BitB = num1;
          s.BitK = num28;
          z.avail_in = num26;
          z.total_in += (long) (num27 - z.next_in_index);
          z.next_in_index = num27;
          s.WritePos = destinationIndex;
          return 1;
        }
        z.msg = "invalid literal/length code";
        int num29 = z.avail_in - availIn;
        int num30 = num2 >> 3 < num29 ? num2 >> 3 : num29;
        int num31 = availIn + num30;
        int num32 = nextInIndex - num30;
        int num33 = num2 - (num30 << 3);
        s.BitB = num1;
        s.BitK = num33;
        z.avail_in = num31;
        z.total_in += (long) (num32 - z.next_in_index);
        z.next_in_index = num32;
        s.WritePos = destinationIndex;
        return -3;
      }
label_37:;
    }
    while (num3 >= 258 && availIn >= 10);
    int num34 = z.avail_in - availIn;
    int num35 = num2 >> 3 < num34 ? num2 >> 3 : num34;
    int num36 = availIn + num35;
    int num37 = nextInIndex - num35;
    int num38 = num2 - (num35 << 3);
    s.BitB = num1;
    s.BitK = num38;
    z.avail_in = num36;
    z.total_in += (long) (num37 - z.next_in_index);
    z.next_in_index = num37;
    s.WritePos = destinationIndex;
    return 0;
  }
}
