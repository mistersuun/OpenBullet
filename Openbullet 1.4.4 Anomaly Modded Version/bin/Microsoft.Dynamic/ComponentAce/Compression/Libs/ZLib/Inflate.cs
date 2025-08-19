// Decompiled with JetBrains decompiler
// Type: ComponentAce.Compression.Libs.ZLib.Inflate
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;

#nullable disable
namespace ComponentAce.Compression.Libs.ZLib;

internal sealed class Inflate
{
  public InflateMode mode;
  private int method;
  private long[] was = new long[1];
  private long need;
  private int marker;
  private int nowrap;
  private int wbits;
  private IEnumerator<object> gzipHeaderRemover;
  private bool detectHeader;
  private InfBlocks blocks;

  internal int inflateReset(ZStream z)
  {
    if (z?.istate == null)
      return -2;
    z.total_in = z.total_out = 0L;
    z.msg = (string) null;
    z.istate.mode = z.istate.nowrap != 0 ? InflateMode.BLOCKS : InflateMode.METHOD;
    z.istate.blocks.reset(z, (long[]) null);
    return 0;
  }

  internal int inflateEnd(ZStream z)
  {
    this.blocks?.free(z);
    this.blocks = (InfBlocks) null;
    return 0;
  }

  internal int inflateInit(ZStream z, int windowBits)
  {
    z.msg = (string) null;
    this.blocks = (InfBlocks) null;
    this.nowrap = 0;
    this.detectHeader = false;
    if (windowBits < 0)
    {
      windowBits = -windowBits;
      this.nowrap = 1;
    }
    else if ((windowBits & 16 /*0x10*/) != 0)
    {
      this.gzipHeaderRemover = GzipHeader.CreateRemover(z);
      windowBits &= -17;
    }
    else if ((windowBits & 32 /*0x20*/) != 0)
    {
      this.detectHeader = true;
      windowBits &= -33;
    }
    if (windowBits < 8 || windowBits > 15)
    {
      this.inflateEnd(z);
      return -2;
    }
    this.wbits = windowBits;
    z.istate.blocks = new InfBlocks(z, z.istate.nowrap == 0, 1 << windowBits);
    this.inflateReset(z);
    return 0;
  }

  internal int inflate(ZStream z, FlushStrategy flush)
  {
    int num1 = (int) flush;
    if (z == null || z.istate == null || z.next_in == null)
      return -2;
    int num2 = num1 == 4 ? -5 : 0;
    int r = -5;
    if (this.detectHeader)
    {
      if (z.avail_in == 0)
        return r;
      if (z.next_in[z.next_in_index] == (byte) 31 /*0x1F*/)
        this.gzipHeaderRemover = GzipHeader.CreateRemover(z);
      this.detectHeader = false;
    }
    if (this.gzipHeaderRemover != null)
    {
      if (z.avail_in == 0 || this.gzipHeaderRemover.MoveNext())
        return r;
      this.gzipHeaderRemover = (IEnumerator<object>) null;
      z.istate.mode = InflateMode.BLOCKS;
      z.istate.blocks.needCheck = false;
      this.nowrap = 1;
    }
    while (true)
    {
      switch (z.istate.mode)
      {
        case InflateMode.METHOD:
          if (z.avail_in != 0)
          {
            r = num2;
            --z.avail_in;
            ++z.total_in;
            Inflate istate = z.istate;
            byte[] nextIn = z.next_in;
            int index = z.next_in_index++;
            int num3;
            int num4 = num3 = (int) nextIn[index];
            istate.method = num3;
            if ((num4 & 15) != 8)
            {
              z.istate.mode = InflateMode.BAD;
              z.msg = "unknown compression method";
              z.istate.marker = 5;
              continue;
            }
            if ((z.istate.method >> 4) + 8 > z.istate.wbits)
            {
              z.istate.mode = InflateMode.BAD;
              z.msg = "invalid Window size";
              z.istate.marker = 5;
              continue;
            }
            z.istate.mode = InflateMode.FLAG;
            goto case InflateMode.FLAG;
          }
          goto label_14;
        case InflateMode.FLAG:
          if (z.avail_in != 0)
          {
            r = num2;
            --z.avail_in;
            ++z.total_in;
            int num5 = (int) z.next_in[z.next_in_index++] & (int) byte.MaxValue;
            if (((z.istate.method << 8) + num5) % 31 /*0x1F*/ != 0)
            {
              z.istate.mode = InflateMode.BAD;
              z.msg = "incorrect header check";
              z.istate.marker = 5;
              continue;
            }
            if ((num5 & 32 /*0x20*/) == 0)
            {
              z.istate.mode = InflateMode.BLOCKS;
              continue;
            }
            goto label_26;
          }
          goto label_21;
        case InflateMode.DICT4:
          goto label_27;
        case InflateMode.DICT3:
          goto label_30;
        case InflateMode.DICT2:
          goto label_33;
        case InflateMode.DICT1:
          goto label_36;
        case InflateMode.DICT0:
          goto label_39;
        case InflateMode.BLOCKS:
          r = z.istate.blocks.proc(z, r);
          if (r == -3)
          {
            z.istate.mode = InflateMode.BAD;
            z.istate.marker = 0;
            continue;
          }
          if (r == 0)
            r = num2;
          if (r == 1)
          {
            r = num2;
            z.istate.blocks.reset(z, z.istate.was);
            if (z.istate.nowrap != 0)
            {
              z.istate.mode = InflateMode.DONE;
              continue;
            }
            z.istate.mode = InflateMode.CHECK4;
            goto case InflateMode.CHECK4;
          }
          goto label_45;
        case InflateMode.CHECK4:
          if (z.avail_in != 0)
          {
            r = num2;
            --z.avail_in;
            ++z.total_in;
            z.istate.need = (long) (((int) z.next_in[z.next_in_index++] & (int) byte.MaxValue) << 24 & -16777216 /*0xFF000000*/);
            z.istate.mode = InflateMode.CHECK3;
            goto case InflateMode.CHECK3;
          }
          goto label_50;
        case InflateMode.CHECK3:
          if (z.avail_in != 0)
          {
            r = num2;
            --z.avail_in;
            ++z.total_in;
            z.istate.need += (long) (((int) z.next_in[z.next_in_index++] & (int) byte.MaxValue) << 16 /*0x10*/) & 16711680L /*0xFF0000*/;
            z.istate.mode = InflateMode.CHECK2;
            goto case InflateMode.CHECK2;
          }
          goto label_53;
        case InflateMode.CHECK2:
          if (z.avail_in != 0)
          {
            r = num2;
            --z.avail_in;
            ++z.total_in;
            z.istate.need += (long) (((int) z.next_in[z.next_in_index++] & (int) byte.MaxValue) << 8) & 65280L;
            z.istate.mode = InflateMode.CHECK1;
            goto case InflateMode.CHECK1;
          }
          goto label_56;
        case InflateMode.CHECK1:
          if (z.avail_in != 0)
          {
            r = num2;
            --z.avail_in;
            ++z.total_in;
            z.istate.need += (long) z.next_in[z.next_in_index++] & (long) byte.MaxValue;
            if ((int) z.istate.was[0] != (int) z.istate.need)
            {
              z.istate.mode = InflateMode.BAD;
              z.msg = "incorrect data check";
              z.istate.marker = 5;
              continue;
            }
            goto label_62;
          }
          goto label_59;
        case InflateMode.DONE:
          goto label_63;
        case InflateMode.BAD:
          goto label_64;
        default:
          goto label_65;
      }
    }
label_14:
    return r;
label_21:
    return r;
label_26:
    z.istate.mode = InflateMode.DICT4;
label_27:
    if (z.avail_in == 0)
      return r;
    r = num2;
    --z.avail_in;
    ++z.total_in;
    z.istate.need = (long) ((int) z.next_in[z.next_in_index++] & (int) byte.MaxValue) << 24 & -16777216L;
    z.istate.mode = InflateMode.DICT3;
label_30:
    if (z.avail_in == 0)
      return r;
    r = num2;
    --z.avail_in;
    ++z.total_in;
    z.istate.need += (long) ((int) z.next_in[z.next_in_index++] & (int) byte.MaxValue) << 16 /*0x10*/ & 16711680L /*0xFF0000*/;
    z.istate.mode = InflateMode.DICT2;
label_33:
    if (z.avail_in == 0)
      return r;
    r = num2;
    --z.avail_in;
    ++z.total_in;
    z.istate.need += (long) ((int) z.next_in[z.next_in_index++] & (int) byte.MaxValue) << 8 & 65280L;
    z.istate.mode = InflateMode.DICT1;
label_36:
    if (z.avail_in == 0)
      return r;
    --z.avail_in;
    ++z.total_in;
    z.istate.need += (long) z.next_in[z.next_in_index++] & (long) byte.MaxValue;
    z.adler = z.istate.need;
    z.istate.mode = InflateMode.DICT0;
    return 2;
label_39:
    z.istate.mode = InflateMode.BAD;
    z.msg = "need dictionary";
    z.istate.marker = 0;
    return -2;
label_45:
    return r;
label_50:
    return r;
label_53:
    return r;
label_56:
    return r;
label_59:
    return r;
label_62:
    z.istate.mode = InflateMode.DONE;
label_63:
    return 1;
label_64:
    return -3;
label_65:
    return -2;
  }

  internal int inflateSetDictionary(ZStream z, byte[] dictionary, int dictLength)
  {
    int start = 0;
    int n = dictLength;
    int num;
    if (z == null)
    {
      num = 1;
    }
    else
    {
      InflateMode? mode = z.istate?.mode;
      InflateMode inflateMode = InflateMode.DICT0;
      num = !(mode.GetValueOrDefault() == inflateMode & mode.HasValue) ? 1 : 0;
    }
    if (num != 0)
      return -2;
    if (Adler32.GetAdler32Checksum(1L, dictionary, 0, dictLength) != z.adler)
      return -3;
    z.adler = Adler32.GetAdler32Checksum(0L, (byte[]) null, 0, 0);
    if (n >= 1 << z.istate.wbits)
    {
      n = (1 << z.istate.wbits) - 1;
      start = dictLength - n;
    }
    z.istate.blocks.set_dictionary(dictionary, start, n);
    z.istate.mode = InflateMode.BLOCKS;
    return 0;
  }

  internal int inflateSync(ZStream z)
  {
    if (z?.istate == null)
      return -2;
    if (z.istate.mode != InflateMode.BAD)
    {
      z.istate.mode = InflateMode.BAD;
      z.istate.marker = 0;
    }
    int availIn;
    if ((availIn = z.avail_in) == 0)
      return -5;
    int nextInIndex = z.next_in_index;
    int index;
    for (index = z.istate.marker; availIn != 0 && index < 4; --availIn)
    {
      if ((int) z.next_in[nextInIndex] == (int) ZLibUtil.mark[index])
        ++index;
      else
        index = z.next_in[nextInIndex] == (byte) 0 ? 4 - index : 0;
      ++nextInIndex;
    }
    z.total_in += (long) (nextInIndex - z.next_in_index);
    z.next_in_index = nextInIndex;
    z.avail_in = availIn;
    z.istate.marker = index;
    if (index != 4)
      return -3;
    long totalIn = z.total_in;
    long totalOut = z.total_out;
    this.inflateReset(z);
    z.total_in = totalIn;
    z.total_out = totalOut;
    z.istate.mode = InflateMode.BLOCKS;
    return 0;
  }

  internal int inflateSyncPoint(ZStream z)
  {
    return z?.istate?.blocks == null ? -2 : z.istate.blocks.sync_point();
  }
}
