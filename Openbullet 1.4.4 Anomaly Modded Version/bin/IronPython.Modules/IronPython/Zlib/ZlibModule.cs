// Decompiled with JetBrains decompiler
// Type: IronPython.Zlib.ZlibModule
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using ComponentAce.Compression.Libs.ZLib;
using IronPython.Modules;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Zlib;

public static class ZlibModule
{
  public const string __doc__ = "The functions in this module allow compression and decompression using the\r\nzlib library, which is based on GNU zip.\r\n\r\nadler32(string[, start]) -- Compute an Adler-32 checksum.\r\ncompress(string[, level]) -- Compress string, with compression level in 1-9.\r\ncompressobj([level]) -- Return a compressor object.\r\ncrc32(string[, start]) -- Compute a CRC-32 checksum.\r\ndecompress(string,[wbits],[bufsize]) -- Decompresses a compressed string.\r\ndecompressobj([wbits]) -- Return a decompressor object.\r\n\r\n'wbits' is window buffer size.\r\nCompressor objects support compress() and flush() methods; decompressor\r\nobjects support decompress() and flush().";
  public const string ZLIB_VERSION = "1.2.3";
  internal const int Z_OK = 0;
  internal const int Z_STREAM_END = 1;
  internal const int Z_NEED_DICT = 2;
  internal const int Z_ERRNO = -1;
  internal const int Z_STREAM_ERROR = -2;
  internal const int Z_DATA_ERROR = -3;
  internal const int Z_MEM_ERROR = -4;
  internal const int Z_BUF_ERROR = -5;
  internal const int Z_VERSION_ERROR = -6;
  public const int Z_NO_FLUSH = 0;
  public const int Z_SYNC_FLUSH = 2;
  public const int Z_FULL_FLUSH = 3;
  public const int Z_FINISH = 4;
  public const int Z_BEST_SPEED = 1;
  public const int Z_BEST_COMPRESSION = 9;
  public const int Z_DEFAULT_COMPRESSION = -1;
  public const int Z_FILTERED = 1;
  public const int Z_HUFFMAN_ONLY = 2;
  public const int Z_DEFAULT_STRATEGY = 0;
  public const int DEFLATED = 8;
  public const int DEF_MEM_LEVEL = 8;
  public const int MAX_WBITS = 15;
  internal const int DEFAULTALLOC = 16384 /*0x4000*/;
  public static PythonType error;

  [Documentation("adler32(string[, start]) -- Compute an Adler-32 checksum of string.\r\n\r\nAn optional starting value can be specified.  The returned checksum is\r\na signed integer.")]
  public static int adler32([BytesConversion] IList<byte> data, long baseValue = 1)
  {
    return (int) Adler32.GetAdler32Checksum(baseValue, data.ToArray<byte>(), 0, data.Count<byte>());
  }

  [Documentation("crc32(string[, start]) -- Compute a CRC-32 checksum of string.\r\n\r\nAn optional starting value can be specified.  The returned checksum is\r\na signed integer.")]
  public static int crc32([BytesConversion] IList<byte> data, long baseValue = 0)
  {
    if (baseValue < (long) int.MinValue || baseValue > (long) uint.MaxValue)
      throw new ArgumentOutOfRangeException(nameof (baseValue));
    return baseValue >= 0L && baseValue <= (long) uint.MaxValue ? PythonBinaryAscii.crc32(data.ToArray<byte>(), (uint) baseValue) : PythonBinaryAscii.crc32(data.ToArray<byte>(), (int) baseValue);
  }

  [Documentation("compress(string[, level]) -- Returned compressed string.\r\n\r\nOptional arg level is the compression level, in 1-9.")]
  public static string compress([BytesConversion] IList<byte> data, int level = -1)
  {
    byte[] array = data.ToArray<byte>();
    byte[] bytes = new byte[array.Length + array.Length / 1000 + 12 + 1];
    ZStream zst = new ZStream();
    zst.next_in = array;
    zst.avail_in = array.Length;
    zst.next_out = bytes;
    zst.avail_out = bytes.Length;
    int err1 = zst.DeflateInit(level);
    switch (err1)
    {
      case -2:
        throw PythonOps.CreateThrowable(ZlibModule.error, (object) "Bad compression level");
      case 0:
        int err2 = zst.deflate(FlushStrategy.Z_FINISH);
        if (err2 != 1)
        {
          zst.deflateEnd();
          throw ZlibModule.zlib_error(zst, err2, "while compressing data");
        }
        int err3 = zst.deflateEnd();
        if (err3 == 0)
          return PythonAsciiEncoding.Instance.GetString(bytes, 0, (int) zst.total_out);
        throw ZlibModule.zlib_error(zst, err3, "while finishing compression");
      default:
        zst.deflateEnd();
        ZlibModule.zlib_error(zst, err1, "while compressing data");
        return (string) null;
    }
  }

  [Documentation("compressobj([level]) -- Return a compressor object.\r\n\r\nOptional arg level is the compression level, in 1-9.")]
  public static Compress compressobj(
    int level = -1,
    int method = 8,
    int wbits = 15,
    int memlevel = 8,
    int strategy = 0)
  {
    return new Compress(level, method, wbits, memlevel, strategy);
  }

  [Documentation("decompress(string[, wbits[, bufsize]]) -- Return decompressed string.\r\n\r\nOptional arg wbits is the window buffer size.  Optional arg bufsize is\r\nthe initial output buffer size.")]
  public static string decompress([BytesConversion] IList<byte> data, int wbits = 15, int bufsize = 16384 /*0x4000*/)
  {
    byte[] bytes = ZlibModule.Decompress(data.ToArray<byte>(), wbits, bufsize);
    return PythonAsciiEncoding.Instance.GetString(bytes, 0, bytes.Length);
  }

  [Documentation("decompressobj([wbits]) -- Return a decompressor object.\r\n\r\nOptional arg wbits is the window buffer size.")]
  public static IronPython.Zlib.Decompress decompressobj(int wbits = 15) => new IronPython.Zlib.Decompress(wbits);

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    ZlibModule.error = context.EnsureModuleException((object) "zlib.error", PythonExceptions.Exception, dict, "error", "zlib");
  }

  internal static Exception MakeError(params object[] args)
  {
    return PythonOps.CreateThrowable(ZlibModule.error, args);
  }

  internal static Exception zlib_error(ZStream zst, int err, string msg)
  {
    string str = zst.msg;
    if (str == null)
    {
      switch (err)
      {
        case -5:
          str = "incomplete or truncated stream";
          break;
        case -3:
          str = "invalid input data";
          break;
        case -2:
          str = "inconsistent stream state";
          break;
      }
    }
    return str == null ? ZlibModule.MakeError((object) $"Error {err} {msg}") : ZlibModule.MakeError((object) $"Error {err} {msg}: {str}");
  }

  [PythonHidden(new PlatformID[] {})]
  internal static byte[] Decompress(byte[] input, int wbits = 15, int bufsize = 16384 /*0x4000*/)
  {
    byte[] sourceArray = new byte[bufsize];
    byte[] array = new byte[bufsize];
    int destinationIndex = 0;
    ZStream zst = new ZStream();
    zst.next_in = input;
    zst.avail_in = input.Length;
    zst.next_out = sourceArray;
    zst.avail_out = sourceArray.Length;
    int err1 = zst.inflateInit(wbits);
    if (err1 != 0)
    {
      zst.inflateEnd();
      throw ZlibModule.zlib_error(zst, err1, "while preparing to decompress data");
    }
    int err2;
    do
    {
      err2 = zst.inflate(FlushStrategy.Z_FINISH);
      if (err2 != 1)
      {
        if (err2 == -5 && zst.avail_out > 0)
        {
          zst.inflateEnd();
          throw ZlibModule.zlib_error(zst, err2, "while decompressing data");
        }
        if (err2 == 0 || err2 == -5 && zst.avail_out == 0)
        {
          if (destinationIndex + sourceArray.Length > array.Length)
            Array.Resize<byte>(ref array, array.Length * 2);
          Array.Copy((Array) sourceArray, 0, (Array) array, destinationIndex, sourceArray.Length);
          destinationIndex += sourceArray.Length;
          zst.next_out = sourceArray;
          zst.avail_out = sourceArray.Length;
          zst.next_out_index = 0;
        }
        else
        {
          zst.inflateEnd();
          throw ZlibModule.zlib_error(zst, err2, "while decompressing data");
        }
      }
    }
    while (err2 != 1);
    int err3 = zst.inflateEnd();
    if (err3 != 0)
      throw ZlibModule.zlib_error(zst, err3, "while finishing data decompression");
    if (destinationIndex + sourceArray.Length - zst.avail_out > array.Length)
      Array.Resize<byte>(ref array, array.Length * 2);
    Array.Copy((Array) sourceArray, 0, (Array) array, destinationIndex, sourceArray.Length - zst.avail_out);
    int count = destinationIndex + (sourceArray.Length - zst.avail_out);
    return ((IEnumerable<byte>) array).Take<byte>(count).ToArray<byte>();
  }
}
