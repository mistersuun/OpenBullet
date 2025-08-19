// Decompiled with JetBrains decompiler
// Type: IronPython.Zlib.Compress
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using ComponentAce.Compression.Libs.ZLib;
using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace IronPython.Zlib;

[PythonType]
public class Compress
{
  private const int Z_OK = 0;
  private const int Z_BUF_ERROR = -5;
  private const int Z_STREAM_END = 1;
  private const int Z_NO_FLUSH = 0;
  private const int Z_FINISH = 4;
  private ZStream zst;

  internal Compress(int level, int method, int wbits, int memlevel, int strategy)
  {
    this.zst = new ZStream();
    int err = this.zst.DeflateInit(level, wbits);
    switch (err)
    {
      case -2:
        throw PythonOps.ValueError("Invalid initialization option");
      case 0:
        break;
      default:
        throw ZlibModule.zlib_error(this.zst, err, "while creating compression object");
    }
  }

  [Documentation("compress(data) -- Return a string containing data compressed.\r\n\r\nAfter calling this function, some of the input data may still\r\nbe stored in internal buffers for later processing.\r\nCall the flush() method to clear these buffers.")]
  public string compress([BytesConversion] IList<byte> data)
  {
    byte[] array1 = data.ToArray<byte>();
    byte[] array2 = new byte[16384 /*0x4000*/];
    long totalOut = this.zst.total_out;
    this.zst.next_in = array1;
    this.zst.next_in_index = 0;
    this.zst.avail_in = array1.Length;
    this.zst.next_out = array2;
    this.zst.next_out_index = 0;
    this.zst.avail_out = array2.Length;
    int err;
    for (err = this.zst.deflate(FlushStrategy.Z_NO_FLUSH); err == 0 && this.zst.avail_out == 0; err = this.zst.deflate(FlushStrategy.Z_NO_FLUSH))
    {
      int length = array2.Length;
      Array.Resize<byte>(ref array2, array2.Length * 2);
      this.zst.next_out = array2;
      this.zst.avail_out = length;
    }
    if (err != 0 && err != -5)
      throw ZlibModule.zlib_error(this.zst, err, "while compressing");
    return PythonAsciiEncoding.Instance.GetString(array2, 0, (int) (this.zst.total_out - totalOut));
  }

  [Documentation("flush( [mode] ) -- Return a string containing any remaining compressed data.\r\n\r\nmode can be one of the constants Z_SYNC_FLUSH, Z_FULL_FLUSH, Z_FINISH; the\r\ndefault value used when mode is not specified is Z_FINISH.\r\nIf mode == Z_FINISH, the compressor object can no longer be used after\r\ncalling the flush() method.  Otherwise, more data can still be compressed.")]
  public string flush(int mode = 4)
  {
    byte[] array = new byte[16384 /*0x4000*/];
    if (mode == 0)
      return string.Empty;
    long totalOut = this.zst.total_out;
    this.zst.avail_in = 0;
    this.zst.next_out = array;
    this.zst.next_out_index = 0;
    this.zst.avail_out = array.Length;
    int err1;
    for (err1 = this.zst.deflate((FlushStrategy) mode); err1 == 0 && this.zst.avail_out == 0; err1 = this.zst.deflate((FlushStrategy) mode))
    {
      int length = array.Length;
      Array.Resize<byte>(ref array, array.Length * 2);
      this.zst.next_out = array;
      this.zst.avail_out = length;
    }
    if (err1 == 1 && mode == 4)
    {
      int err2 = this.zst.deflateEnd();
      if (err2 != 0)
        throw ZlibModule.zlib_error(this.zst, err2, "from deflateEnd()");
    }
    else if (err1 != 0 && err1 != -5)
      throw ZlibModule.zlib_error(this.zst, err1, "while flushing");
    return PythonAsciiEncoding.Instance.GetString(array, 0, (int) (this.zst.total_out - totalOut));
  }
}
