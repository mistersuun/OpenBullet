// Decompiled with JetBrains decompiler
// Type: ComponentAce.Compression.Libs.ZLib.GzipHeader
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace ComponentAce.Compression.Libs.ZLib;

internal class GzipHeader
{
  private const int FIXED_HEADER_SIZE = 10;

  public static IEnumerator<object> CreateRemover(ZStream z)
  {
    return new GzipHeader().StartHeaderSkipping(z).GetEnumerator();
  }

  private byte GetNext(ZStream z)
  {
    --z.avail_in;
    ++z.total_in;
    return z.next_in[z.next_in_index++];
  }

  private IEnumerable<object> StartHeaderSkipping(ZStream z)
  {
    List<byte> headerCollector = new List<byte>(10);
    do
    {
      if (z.avail_in == 0)
        yield return (object) false;
      headerCollector.Add(this.GetNext(z));
    }
    while (headerCollector.Count < 10);
    byte flag = headerCollector[3];
    int outstandingSize;
    if (((int) flag & 4) != 0)
    {
      if (z.avail_in == 0)
        yield return (object) null;
      outstandingSize = (int) this.GetNext(z);
      if (z.avail_in == 0)
        yield return (object) null;
      outstandingSize += 256 /*0x0100*/ * (int) this.GetNext(z);
      do
      {
        if (z.avail_in == 0)
          yield return (object) null;
        int next = (int) this.GetNext(z);
      }
      while (--outstandingSize != 0);
    }
    if (((int) flag & 8) != 0)
    {
      do
      {
        if (z.avail_in == 0)
          yield return (object) null;
      }
      while (this.GetNext(z) != (byte) 0);
    }
    if (((int) flag & 16 /*0x10*/) != 0)
    {
      do
      {
        if (z.avail_in == 0)
          yield return (object) null;
      }
      while (this.GetNext(z) != (byte) 0);
    }
    if (((int) flag & 2) != 0)
    {
      outstandingSize = 4;
      do
      {
        if (z.avail_in == 0)
          yield return (object) null;
        int next = (int) this.GetNext(z);
      }
      while (--outstandingSize != 0);
    }
  }

  [Flags]
  private enum HEADER_FLAG
  {
    FHCRC = 2,
    FEXTRA = 4,
    FNAME = 8,
    FCOMMENT = 16, // 0x00000010
  }
}
