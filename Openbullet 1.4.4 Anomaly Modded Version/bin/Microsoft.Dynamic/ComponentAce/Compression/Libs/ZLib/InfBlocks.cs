// Decompiled with JetBrains decompiler
// Type: ComponentAce.Compression.Libs.ZLib.InfBlocks
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace ComponentAce.Compression.Libs.ZLib;

internal sealed class InfBlocks
{
  private const int MANY = 1440;
  private InflateBlockMode mode;
  private int left;
  private int table;
  private int index;
  private int[] blens;
  private int[] bb = new int[1];
  private int[] tb = new int[1];
  private InfCodes codes;
  private int last;
  private int[] hufts;
  internal bool needCheck;
  private long check;

  public byte[] Window { get; set; }

  public int End { get; set; }

  public int ReadPos { get; set; }

  public int WritePos { get; set; }

  public int BitK { get; set; }

  public int BitB { get; set; }

  internal InfBlocks(ZStream z, bool needCheck, int w)
  {
    this.hufts = new int[4320];
    this.Window = new byte[w];
    this.End = w;
    this.needCheck = needCheck;
    this.mode = InflateBlockMode.TYPE;
    this.reset(z, (long[]) null);
  }

  internal void reset(ZStream z, long[] c)
  {
    if (c != null)
      c[0] = this.check;
    if (this.mode == InflateBlockMode.BTREE || this.mode == InflateBlockMode.DTREE)
      this.blens = (int[]) null;
    if (this.mode == InflateBlockMode.CODES)
      this.codes.free(z);
    this.mode = InflateBlockMode.TYPE;
    this.BitK = 0;
    this.BitB = 0;
    this.ReadPos = this.WritePos = 0;
    if (!this.needCheck)
      return;
    z.adler = this.check = Adler32.GetAdler32Checksum(0L, (byte[]) null, 0, 0);
  }

  internal int proc(ZStream z, int r)
  {
    int nextInIndex = z.next_in_index;
    int availIn = z.avail_in;
    int number1 = this.BitB;
    int num1 = this.BitK;
    int destinationIndex = this.WritePos;
    int num2 = destinationIndex < this.ReadPos ? this.ReadPos - destinationIndex - 1 : this.End - destinationIndex;
    int num3;
    int num4;
    while (true)
    {
      int length;
      do
      {
        switch (this.mode)
        {
          case InflateBlockMode.TYPE:
            for (; num1 < 3; num1 += 8)
            {
              if (availIn != 0)
              {
                r = 0;
                --availIn;
                number1 |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << num1;
              }
              else
              {
                this.BitB = number1;
                this.BitK = num1;
                z.avail_in = availIn;
                z.total_in += (long) (nextInIndex - z.next_in_index);
                z.next_in_index = nextInIndex;
                this.WritePos = destinationIndex;
                return this.inflate_flush(z, r);
              }
            }
            int number2 = number1 & 7;
            this.last = number2 & 1;
            switch (ZLibUtil.URShift(number2, 1))
            {
              case 0:
                int number3 = ZLibUtil.URShift(number1, 3);
                int num5 = num1 - 3;
                int bits1 = num5 & 7;
                number1 = ZLibUtil.URShift(number3, bits1);
                num1 = num5 - bits1;
                this.mode = InflateBlockMode.LENS;
                continue;
              case 1:
                int[] bl1 = new int[1];
                int[] bd1 = new int[1];
                int[][] tl1 = new int[1][];
                int[][] td1 = new int[1][];
                InfTree.inflate_trees_fixed(bl1, bd1, tl1, td1, z);
                this.codes = new InfCodes(bl1[0], bd1[0], tl1[0], td1[0], z);
                number1 = ZLibUtil.URShift(number1, 3);
                num1 -= 3;
                this.mode = InflateBlockMode.CODES;
                continue;
              case 2:
                number1 = ZLibUtil.URShift(number1, 3);
                num1 -= 3;
                this.mode = InflateBlockMode.TABLE;
                continue;
              case 3:
                int num6 = ZLibUtil.URShift(number1, 3);
                int num7 = num1 - 3;
                this.mode = InflateBlockMode.BAD;
                z.msg = "invalid block type";
                r = -3;
                this.BitB = num6;
                this.BitK = num7;
                z.avail_in = availIn;
                z.total_in += (long) (nextInIndex - z.next_in_index);
                z.next_in_index = nextInIndex;
                this.WritePos = destinationIndex;
                return this.inflate_flush(z, r);
              default:
                continue;
            }
          case InflateBlockMode.LENS:
            for (; num1 < 32 /*0x20*/; num1 += 8)
            {
              if (availIn != 0)
              {
                r = 0;
                --availIn;
                number1 |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << num1;
              }
              else
              {
                this.BitB = number1;
                this.BitK = num1;
                z.avail_in = availIn;
                z.total_in += (long) (nextInIndex - z.next_in_index);
                z.next_in_index = nextInIndex;
                this.WritePos = destinationIndex;
                return this.inflate_flush(z, r);
              }
            }
            if ((ZLibUtil.URShift(~number1, 16 /*0x10*/) & (int) ushort.MaxValue) != (number1 & (int) ushort.MaxValue))
            {
              this.mode = InflateBlockMode.BAD;
              z.msg = "invalid stored block lengths";
              r = -3;
              this.BitB = number1;
              this.BitK = num1;
              z.avail_in = availIn;
              z.total_in += (long) (nextInIndex - z.next_in_index);
              z.next_in_index = nextInIndex;
              this.WritePos = destinationIndex;
              return this.inflate_flush(z, r);
            }
            this.left = number1 & (int) ushort.MaxValue;
            number1 = num1 = 0;
            this.mode = this.left != 0 ? InflateBlockMode.STORED : (this.last != 0 ? InflateBlockMode.DRY : InflateBlockMode.TYPE);
            continue;
          case InflateBlockMode.STORED:
            if (availIn == 0)
            {
              this.BitB = number1;
              this.BitK = num1;
              z.avail_in = availIn;
              z.total_in += (long) (nextInIndex - z.next_in_index);
              z.next_in_index = nextInIndex;
              this.WritePos = destinationIndex;
              return this.inflate_flush(z, r);
            }
            if (num2 == 0)
            {
              if (destinationIndex == this.End && this.ReadPos != 0)
              {
                destinationIndex = 0;
                num2 = destinationIndex < this.ReadPos ? this.ReadPos - destinationIndex - 1 : this.End - destinationIndex;
              }
              if (num2 == 0)
              {
                this.WritePos = destinationIndex;
                r = this.inflate_flush(z, r);
                destinationIndex = this.WritePos;
                num2 = destinationIndex < this.ReadPos ? this.ReadPos - destinationIndex - 1 : this.End - destinationIndex;
                if (destinationIndex == this.End && this.ReadPos != 0)
                {
                  destinationIndex = 0;
                  num2 = destinationIndex < this.ReadPos ? this.ReadPos - destinationIndex - 1 : this.End - destinationIndex;
                }
                if (num2 == 0)
                {
                  this.BitB = number1;
                  this.BitK = num1;
                  z.avail_in = availIn;
                  z.total_in += (long) (nextInIndex - z.next_in_index);
                  z.next_in_index = nextInIndex;
                  this.WritePos = destinationIndex;
                  return this.inflate_flush(z, r);
                }
              }
            }
            r = 0;
            length = this.left;
            if (length > availIn)
              length = availIn;
            if (length > num2)
              length = num2;
            Array.Copy((Array) z.next_in, nextInIndex, (Array) this.Window, destinationIndex, length);
            nextInIndex += length;
            availIn -= length;
            destinationIndex += length;
            num2 -= length;
            continue;
          case InflateBlockMode.TABLE:
            goto label_37;
          case InflateBlockMode.BTREE:
            goto label_46;
          case InflateBlockMode.DTREE:
            goto label_54;
          case InflateBlockMode.CODES:
            goto label_76;
          case InflateBlockMode.DRY:
            goto label_81;
          case InflateBlockMode.DONE:
            goto label_84;
          case InflateBlockMode.BAD:
            goto label_85;
          default:
            goto label_86;
        }
      }
      while ((this.left -= length) != 0);
      this.mode = this.last != 0 ? InflateBlockMode.DRY : InflateBlockMode.TYPE;
      continue;
label_37:
      for (; num1 < 14; num1 += 8)
      {
        if (availIn != 0)
        {
          r = 0;
          --availIn;
          number1 |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << num1;
        }
        else
        {
          this.BitB = number1;
          this.BitK = num1;
          z.avail_in = availIn;
          z.total_in += (long) (nextInIndex - z.next_in_index);
          z.next_in_index = nextInIndex;
          this.WritePos = destinationIndex;
          return this.inflate_flush(z, r);
        }
      }
      int num8;
      this.table = num8 = number1 & 16383 /*0x3FFF*/;
      if ((num8 & 31 /*0x1F*/) <= 29 && (num8 >> 5 & 31 /*0x1F*/) <= 29)
      {
        this.blens = new int[258 + (num8 & 31 /*0x1F*/) + (num8 >> 5 & 31 /*0x1F*/)];
        number1 = ZLibUtil.URShift(number1, 14);
        num1 -= 14;
        this.index = 0;
        this.mode = InflateBlockMode.BTREE;
      }
      else
        break;
label_46:
      while (this.index < 4 + ZLibUtil.URShift(this.table, 10))
      {
        for (; num1 < 3; num1 += 8)
        {
          if (availIn != 0)
          {
            r = 0;
            --availIn;
            number1 |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << num1;
          }
          else
          {
            this.BitB = number1;
            this.BitK = num1;
            z.avail_in = availIn;
            z.total_in += (long) (nextInIndex - z.next_in_index);
            z.next_in_index = nextInIndex;
            this.WritePos = destinationIndex;
            return this.inflate_flush(z, r);
          }
        }
        this.blens[ZLibUtil.border[this.index++]] = number1 & 7;
        number1 = ZLibUtil.URShift(number1, 3);
        num1 -= 3;
      }
      while (this.index < 19)
        this.blens[ZLibUtil.border[this.index++]] = 0;
      this.bb[0] = 7;
      num3 = InfTree.inflate_trees_bits(this.blens, this.bb, this.tb, this.hufts, z);
      if (num3 == 0)
      {
        this.index = 0;
        this.mode = InflateBlockMode.DTREE;
      }
      else
        goto label_50;
label_54:
      while (true)
      {
        int table1 = this.table;
        if (this.index < 258 + (table1 & 31 /*0x1F*/) + (table1 >> 5 & 31 /*0x1F*/))
        {
          int index1;
          for (index1 = this.bb[0]; num1 < index1; num1 += 8)
          {
            if (availIn != 0)
            {
              r = 0;
              --availIn;
              number1 |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << num1;
            }
            else
            {
              this.BitB = number1;
              this.BitK = num1;
              z.avail_in = availIn;
              z.total_in += (long) (nextInIndex - z.next_in_index);
              z.next_in_index = nextInIndex;
              this.WritePos = destinationIndex;
              return this.inflate_flush(z, r);
            }
          }
          int huft1 = this.hufts[(this.tb[0] + (number1 & ZLibUtil.inflate_mask[index1])) * 3 + 1];
          int huft2 = this.hufts[(this.tb[0] + (number1 & ZLibUtil.inflate_mask[huft1])) * 3 + 2];
          if (huft2 < 16 /*0x10*/)
          {
            number1 = ZLibUtil.URShift(number1, huft1);
            num1 -= huft1;
            this.blens[this.index++] = huft2;
          }
          else
          {
            int bits2 = huft2 == 18 ? 7 : huft2 - 14;
            int num9 = huft2 == 18 ? 11 : 3;
            for (; num1 < huft1 + bits2; num1 += 8)
            {
              if (availIn != 0)
              {
                r = 0;
                --availIn;
                number1 |= ((int) z.next_in[nextInIndex++] & (int) byte.MaxValue) << num1;
              }
              else
              {
                this.BitB = number1;
                this.BitK = num1;
                z.avail_in = availIn;
                z.total_in += (long) (nextInIndex - z.next_in_index);
                z.next_in_index = nextInIndex;
                this.WritePos = destinationIndex;
                return this.inflate_flush(z, r);
              }
            }
            int number4 = ZLibUtil.URShift(number1, huft1);
            int num10 = num1 - huft1;
            int num11 = num9 + (number4 & ZLibUtil.inflate_mask[bits2]);
            number1 = ZLibUtil.URShift(number4, bits2);
            num1 = num10 - bits2;
            int index2 = this.index;
            int table2 = this.table;
            if (index2 + num11 <= 258 + (table2 & 31 /*0x1F*/) + (table2 >> 5 & 31 /*0x1F*/) && (huft2 != 16 /*0x10*/ || index2 >= 1))
            {
              int blen = huft2 == 16 /*0x10*/ ? this.blens[index2 - 1] : 0;
              do
              {
                this.blens[index2++] = blen;
              }
              while (--num11 != 0);
              this.index = index2;
            }
            else
              goto label_68;
          }
        }
        else
          break;
      }
      this.tb[0] = -1;
      int[] bl2 = new int[1];
      int[] bd2 = new int[1];
      int[] tl2 = new int[1];
      int[] td2 = new int[1];
      bl2[0] = 9;
      bd2[0] = 6;
      int table = this.table;
      num4 = InfTree.inflate_trees_dynamic(257 + (table & 31 /*0x1F*/), 1 + (table >> 5 & 31 /*0x1F*/), this.blens, bl2, bd2, tl2, td2, this.hufts, z);
      switch (num4)
      {
        case -3:
          goto label_73;
        case 0:
          this.codes = new InfCodes(bl2[0], bd2[0], this.hufts, tl2[0], this.hufts, td2[0], z);
          this.blens = (int[]) null;
          this.mode = InflateBlockMode.CODES;
          break;
        default:
          goto label_74;
      }
label_76:
      this.BitB = number1;
      this.BitK = num1;
      z.avail_in = availIn;
      z.total_in += (long) (nextInIndex - z.next_in_index);
      z.next_in_index = nextInIndex;
      this.WritePos = destinationIndex;
      if ((r = this.codes.proc(this, z, r)) == 1)
      {
        r = 0;
        this.codes.free(z);
        nextInIndex = z.next_in_index;
        availIn = z.avail_in;
        number1 = this.BitB;
        num1 = this.BitK;
        destinationIndex = this.WritePos;
        num2 = destinationIndex < this.ReadPos ? this.ReadPos - destinationIndex - 1 : this.End - destinationIndex;
        if (this.last == 0)
          this.mode = InflateBlockMode.TYPE;
        else
          goto label_80;
      }
      else
        goto label_77;
    }
    this.mode = InflateBlockMode.BAD;
    z.msg = "too many length or distance symbols";
    r = -3;
    this.BitB = number1;
    this.BitK = num1;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    this.WritePos = destinationIndex;
    return this.inflate_flush(z, r);
label_50:
    r = num3;
    if (r == -3)
    {
      this.blens = (int[]) null;
      this.mode = InflateBlockMode.BAD;
    }
    this.BitB = number1;
    this.BitK = num1;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    this.WritePos = destinationIndex;
    return this.inflate_flush(z, r);
label_68:
    this.blens = (int[]) null;
    this.mode = InflateBlockMode.BAD;
    z.msg = "invalid bit length repeat";
    r = -3;
    this.BitB = number1;
    this.BitK = num1;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    this.WritePos = destinationIndex;
    return this.inflate_flush(z, r);
label_73:
    this.blens = (int[]) null;
    this.mode = InflateBlockMode.BAD;
label_74:
    r = num4;
    this.BitB = number1;
    this.BitK = num1;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    this.WritePos = destinationIndex;
    return this.inflate_flush(z, r);
label_77:
    return this.inflate_flush(z, r);
label_80:
    this.mode = InflateBlockMode.DRY;
label_81:
    this.WritePos = destinationIndex;
    r = this.inflate_flush(z, r);
    destinationIndex = this.WritePos;
    int num12 = destinationIndex < this.ReadPos ? this.ReadPos - destinationIndex - 1 : this.End - destinationIndex;
    if (this.ReadPos != this.WritePos)
    {
      this.BitB = number1;
      this.BitK = num1;
      z.avail_in = availIn;
      z.total_in += (long) (nextInIndex - z.next_in_index);
      z.next_in_index = nextInIndex;
      this.WritePos = destinationIndex;
      return this.inflate_flush(z, r);
    }
    this.mode = InflateBlockMode.DONE;
label_84:
    r = 1;
    this.BitB = number1;
    this.BitK = num1;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    this.WritePos = destinationIndex;
    return this.inflate_flush(z, r);
label_85:
    r = -3;
    this.BitB = number1;
    this.BitK = num1;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    this.WritePos = destinationIndex;
    return this.inflate_flush(z, r);
label_86:
    r = -2;
    this.BitB = number1;
    this.BitK = num1;
    z.avail_in = availIn;
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    this.WritePos = destinationIndex;
    return this.inflate_flush(z, r);
  }

  internal void free(ZStream z)
  {
    this.reset(z, (long[]) null);
    this.Window = (byte[]) null;
    this.hufts = (int[]) null;
  }

  internal void set_dictionary(byte[] d, int start, int n)
  {
    Array.Copy((Array) d, start, (Array) this.Window, 0, n);
    this.ReadPos = this.WritePos = n;
  }

  internal int sync_point() => this.mode != InflateBlockMode.LENS ? 0 : 1;

  internal int inflate_flush(ZStream z, int r)
  {
    int nextOutIndex = z.next_out_index;
    int readPos = this.ReadPos;
    int num1 = (readPos <= this.WritePos ? this.WritePos : this.End) - readPos;
    if (num1 > z.avail_out)
      num1 = z.avail_out;
    if (num1 != 0 && r == -5)
      r = 0;
    z.avail_out -= num1;
    z.total_out += (long) num1;
    if (this.needCheck)
      z.adler = this.check = Adler32.GetAdler32Checksum(this.check, this.Window, readPos, num1);
    Array.Copy((Array) this.Window, readPos, (Array) z.next_out, nextOutIndex, num1);
    int destinationIndex = nextOutIndex + num1;
    int num2 = readPos + num1;
    if (num2 == this.End)
    {
      int num3 = 0;
      if (this.WritePos == this.End)
        this.WritePos = 0;
      int num4 = this.WritePos - num3;
      if (num4 > z.avail_out)
        num4 = z.avail_out;
      if (num4 != 0 && r == -5)
        r = 0;
      z.avail_out -= num4;
      z.total_out += (long) num4;
      if (this.needCheck)
        z.adler = this.check = Adler32.GetAdler32Checksum(this.check, this.Window, num3, num4);
      Array.Copy((Array) this.Window, num3, (Array) z.next_out, destinationIndex, num4);
      destinationIndex += num4;
      num2 = num3 + num4;
    }
    z.next_out_index = destinationIndex;
    this.ReadPos = num2;
    return r;
  }
}
