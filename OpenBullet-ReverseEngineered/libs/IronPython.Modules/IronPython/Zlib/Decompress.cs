// Decompiled with JetBrains decompiler
// Type: IronPython.Zlib.Decompress
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
public class Decompress
{
  private const int Z_OK = 0;
  private const int Z_STREAM_END = 1;
  private const int Z_BUF_ERROR = -5;
  private const int Z_SYNC_FLUSH = 2;
  private const int Z_FINISH = 4;
  private string _unused_data;
  private string _unconsumed_tail;
  private ZStream zst;

  internal Decompress(int wbits)
  {
    this.zst = new ZStream();
    int err = this.zst.inflateInit(wbits);
    switch (err)
    {
      case -2:
        throw PythonOps.ValueError("Invalid initialization option");
      case 0:
        this._unused_data = string.Empty;
        this._unconsumed_tail = string.Empty;
        break;
      default:
        throw ZlibModule.zlib_error(this.zst, err, "while creating decompression object");
    }
  }

  public string unused_data => this._unused_data;

  public string unconsumed_tail => this._unconsumed_tail;

  [Documentation("decompress(data, max_length) -- Return a string containing the decompressed\r\nversion of the data.\r\n\r\nAfter calling this function, some of the input data may still be stored in\r\ninternal buffers for later processing.\r\nCall the flush() method to clear these buffers.\r\nIf the max_length parameter is specified then the return value will be\r\nno longer than max_length.  Unconsumed input data will be stored in\r\nthe unconsumed_tail attribute.")]
  public string decompress([BytesConversion] IList<byte> value, int max_length = 0)
  {
    if (max_length < 0)
      throw new ArgumentException("max_length must be greater than zero");
    byte[] array1 = value.ToArray<byte>();
    byte[] array2 = new byte[max_length <= 0 || 16384 /*0x4000*/ <= max_length ? 16384 /*0x4000*/ : max_length];
    long totalOut = this.zst.total_out;
    this.zst.next_in = array1;
    this.zst.next_in_index = 0;
    this.zst.avail_in = array1.Length;
    this.zst.next_out = array2;
    this.zst.next_out_index = 0;
    this.zst.avail_out = array2.Length;
    int err;
    for (err = this.zst.inflate(FlushStrategy.Z_SYNC_FLUSH); err == 0 && this.zst.avail_out == 0 && (max_length <= 0 || array2.Length < max_length); err = this.zst.inflate(FlushStrategy.Z_SYNC_FLUSH))
    {
      int length = array2.Length;
      Array.Resize<byte>(ref array2, array2.Length * 2);
      this.zst.next_out = array2;
      this.zst.avail_out = length;
    }
    if (max_length > 0)
      this._unconsumed_tail = PythonAsciiEncoding.Instance.GetString(this.zst.next_in, this.zst.next_in_index, this.zst.avail_in);
    if (err == 1)
      this._unused_data += PythonAsciiEncoding.Instance.GetString(this.zst.next_in, this.zst.next_in_index, this.zst.avail_in);
    else if (err != 0 && err != -5)
      throw ZlibModule.zlib_error(this.zst, err, "while decompressing");
    return PythonAsciiEncoding.Instance.GetString(array2, 0, (int) (this.zst.total_out - totalOut));
  }

  [Documentation("flush( [length] ) -- Return a string containing any remaining\r\ndecompressed data. length, if given, is the initial size of the\r\noutput buffer.\r\n\r\nThe decompressor object can no longer be used after this call.")]
  public string flush(int length = 16384 /*0x4000*/)
  {
    byte[] array = length >= 1 ? new byte[length] : throw PythonOps.ValueError("length must be greater than 0.");
    long totalOut = this.zst.total_out;
    this.zst.next_out = array;
    this.zst.next_out_index = 0;
    this.zst.avail_out = array.Length;
    int num;
    for (num = this.zst.inflate(FlushStrategy.Z_FINISH); (num == 0 || num == -5) && this.zst.avail_out == 0; num = this.zst.inflate(FlushStrategy.Z_FINISH))
    {
      int length1 = array.Length;
      Array.Resize<byte>(ref array, array.Length * 2);
      this.zst.next_out = array;
      this.zst.avail_out = length1;
    }
    if (num == 1)
    {
      int err = this.zst.inflateEnd();
      if (err != 0)
        throw ZlibModule.zlib_error(this.zst, err, "from inflateEnd()");
    }
    return PythonAsciiEncoding.Instance.GetString(array, 0, (int) (this.zst.total_out - totalOut));
  }
}
