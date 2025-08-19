// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.Bz2.Bz2Module
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using Ionic.BZip2;
using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace IronPython.Modules.Bz2;

public static class Bz2Module
{
  public const string __doc__ = "The python bz2 module provides a comprehensive interface for\r\nthe bz2 compression library. It implements a complete file\r\ninterface, one shot (de)compression functions, and types for\r\nsequential (de)compression.";
  internal const int DEFAULT_COMPRESSLEVEL = 9;
  private const int PARALLEL_THRESHOLD = 10485760 /*0xA00000*/;

  [Documentation("compress(data [, compresslevel=9]) -> string\r\n\r\nCompress data in one shot. If you want to compress data sequentially,\r\nuse an instance of BZ2Compressor instead. The compresslevel parameter, if\r\ngiven, must be a number between 1 and 9.\r\n")]
  public static Bytes compress([BytesConversion] IList<byte> data, int compresslevel = 9)
  {
    using (MemoryStream output = new MemoryStream())
    {
      using (Stream stream = data.Count > 10485760 /*0xA00000*/ ? (Stream) new ParallelBZip2OutputStream((Stream) output, true) : (Stream) new BZip2OutputStream((Stream) output, true))
      {
        byte[] arrayNoCopy = data.ToArrayNoCopy();
        stream.Write(arrayNoCopy, 0, data.Count);
      }
      return Bytes.Make(output.ToArray());
    }
  }

  [Documentation("decompress(data) -> decompressed data\r\n\r\nDecompress data in one shot. If you want to decompress data sequentially,\r\nuse an instance of BZ2Decompressor instead.\r\n")]
  public static Bytes decompress([BytesConversion] IList<byte> data)
  {
    if (data.Count == 0)
      return new Bytes();
    byte[] buffer = new byte[1024 /*0x0400*/];
    using (MemoryStream memoryStream = new MemoryStream())
    {
      using (MemoryStream input = new MemoryStream(data.ToArrayNoCopy(), false))
      {
        using (BZip2InputStream bzip2InputStream = new BZip2InputStream((Stream) input))
        {
          while (true)
          {
            int count;
            try
            {
              count = bzip2InputStream.Read(buffer, 0, buffer.Length);
            }
            catch (IOException ex)
            {
              throw PythonOps.ValueError(ex.Message);
            }
            if (count > 0)
              memoryStream.Write(buffer, 0, count);
            else
              break;
          }
        }
      }
      return Bytes.Make(memoryStream.ToArray());
    }
  }

  private static byte[] ToArrayNoCopy(this IList<byte> bytes)
  {
    switch (bytes)
    {
      case byte[] arrayNoCopy:
        return arrayNoCopy;
      case Bytes bytes1:
        return bytes1.GetUnsafeByteArray();
      default:
        return bytes.ToArray<byte>();
    }
  }

  [PythonType]
  public class BZ2Compressor
  {
    public const string __doc__ = "BZ2Compressor([compresslevel=9]) -> compressor object\r\n\r\nCreate a new compressor object. This object may be used to compress\r\ndata sequentially. If you want to compress data in one shot, use the\r\ncompress() function instead. The compresslevel parameter, if given,\r\nmust be a number between 1 and 9.\r\n";
    private int compresslevel;
    private MemoryStream output;
    private BZip2OutputStream bz2Output;
    private long lastPosition;

    public BZ2Compressor(int compresslevel = 9)
    {
      this.compresslevel = compresslevel;
      this.output = new MemoryStream();
      this.bz2Output = new BZip2OutputStream((Stream) this.output, true);
    }

    [Documentation("compress(data) -> string\r\n\r\nProvide more data to the compressor object. It will return chunks of\r\ncompressed data whenever possible. When you've finished providing data\r\nto compress, call the flush() method to finish the compression process,\r\nand return what is left in the internal buffers.\r\n")]
    public Bytes compress([BytesConversion] IList<byte> data)
    {
      byte[] arrayNoCopy = data.ToArrayNoCopy();
      this.bz2Output.Write(arrayNoCopy, 0, arrayNoCopy.Length);
      return new Bytes((IList<byte>) this.GetLatestData());
    }

    [Documentation("flush() -> string\r\n\r\nFinish the compression process and return what is left in internal buffers.\r\nYou must not use the compressor object after calling this method.\r\n")]
    public Bytes flush()
    {
      this.bz2Output.Dispose();
      return new Bytes((IList<byte>) this.GetLatestData());
    }

    private byte[] GetLatestData()
    {
      long length = this.output.Position - this.lastPosition;
      byte[] destinationArray = new byte[length];
      if (length > 0L)
      {
        Array.Copy((Array) this.output.GetBuffer(), this.lastPosition, (Array) destinationArray, 0L, length);
        this.lastPosition = this.output.Position;
      }
      return destinationArray;
    }
  }

  [PythonType]
  public class BZ2Decompressor
  {
    public const string __doc__ = "BZ2Decompressor() -> decompressor object\r\n\r\nCreate a new decompressor object. This object may be used to decompress\r\ndata sequentially. If you want to decompress data in one shot, use the\r\ndecompress() function instead.\r\n";
    private MemoryStream input;
    private BZip2InputStream bz2Input;
    private long lastSuccessfulPosition;
    private bool _finished;

    public Bytes unused_data
    {
      get
      {
        long length = this.input.Length - this.lastSuccessfulPosition;
        byte[] numArray = new byte[length];
        Array.Copy((Array) this.input.GetBuffer(), this.lastSuccessfulPosition, (Array) numArray, 0L, length);
        return new Bytes((IList<byte>) numArray);
      }
    }

    [Documentation("decompress(data) -> string\r\n\r\nProvide more data to the decompressor object. It will return chunks\r\nof decompressed data whenever possible. If you try to decompress data\r\nafter the end of stream is found, EOFError will be raised. If any data\r\nwas found after the end of stream, it'll be ignored and saved in\r\nunused_data attribute.\r\n")]
    public Bytes decompress([BytesConversion] IList<byte> data)
    {
      if (this._finished)
        throw PythonOps.EofError("End of stream was already found");
      byte[] arrayNoCopy = data.ToArrayNoCopy();
      if (!this.InitializeMemoryStream(arrayNoCopy))
        this.AddData(arrayNoCopy);
      List<byte> bytes = new List<byte>();
      if (this.InitializeBZ2Stream())
      {
        long position = this.input.Position;
        object o = this.bz2Input.DumpState();
        try
        {
          int num;
          while ((num = this.bz2Input.ReadByte()) != -1)
          {
            bytes.Add((byte) num);
            position = this.input.Position;
            o = this.bz2Input.DumpState();
          }
          this.lastSuccessfulPosition = this.input.Position;
          this._finished = true;
        }
        catch (IOException ex)
        {
          this.input.Position = position;
          this.bz2Input.RestoreState(o);
        }
      }
      return new Bytes((IList<byte>) bytes);
    }

    private bool InitializeMemoryStream(byte[] data)
    {
      if (this.input != null)
        return false;
      this.input = new MemoryStream();
      this.input.Write(data, 0, data.Length);
      this.input.Position = 0L;
      return true;
    }

    private bool InitializeBZ2Stream()
    {
      if (this.bz2Input != null)
        return true;
      try
      {
        this.bz2Input = new BZip2InputStream((Stream) this.input, true);
        return true;
      }
      catch (IOException ex)
      {
        this.input.Position = this.lastSuccessfulPosition;
        return false;
      }
    }

    private void AddData(byte[] bytes)
    {
      long position = this.input.Position;
      this.input.Position = this.input.Length;
      this.input.Write(((IEnumerable<byte>) bytes).ToArray<byte>(), 0, bytes.Length);
      this.input.Position = position;
    }
  }

  [PythonType]
  public class BZ2File(CodeContext context) : PythonFile(context)
  {
    public const string __doc__ = "BZ2File(name [, mode='r', buffering=0, compresslevel=9]) -> file object\r\n\r\nOpen a bz2 file. The mode can be 'r' or 'w', for reading (default) or\r\nwriting. When opened for writing, the file will be created if it doesn't\r\nexist, and truncated otherwise. If the buffering argument is given, 0 means\r\nunbuffered, and larger numbers specify the buffer size. If compresslevel\r\nis given, must be a number between 1 and 9.\r\n";
    private Stream bz2Stream;

    public int buffering { get; private set; }

    public int compresslevel { get; private set; }

    public void __init__(
      CodeContext context,
      string filename,
      string mode = "r",
      int buffering = 0,
      int compresslevel = 9)
    {
      PythonContext languageContext = context.LanguageContext;
      this.buffering = buffering;
      this.compresslevel = compresslevel;
      if (!mode.Contains("b") && !mode.Contains("U"))
        mode += "b";
      if (mode.Contains("w"))
      {
        FileStream output = File.Open(filename, FileMode.Create, FileAccess.Write);
        this.bz2Stream = !mode.Contains("p") ? (Stream) new BZip2OutputStream((Stream) output) : (Stream) new ParallelBZip2OutputStream((Stream) output);
      }
      else
        this.bz2Stream = (Stream) new BZip2InputStream((Stream) File.OpenRead(filename));
      this.__init__(this.bz2Stream, languageContext.DefaultEncoding, filename, mode);
    }

    [Documentation("close() -> None or (perhaps) an integer\r\n\r\nClose the file. Sets data attribute .closed to true. A closed file\r\ncannot be used for further I/O operations. close() may be called more\r\nthan once without error.\r\n")]
    public void close() => base.close();

    [Documentation("read([size]) -> string\r\n\r\nRead at most size uncompressed bytes, returned as a string. If the size\r\nargument is negative or omitted, read until EOF is reached.\r\n")]
    public new string read()
    {
      this.ThrowIfClosed();
      return base.read();
    }

    public new string read(int size)
    {
      this.ThrowIfClosed();
      return base.read(size);
    }

    [Documentation("readline([size]) -> string\r\n\r\nReturn the next line from the file, as a string, retaining newline.\r\nA non-negative size argument will limit the maximum number of bytes to\r\nreturn (an incomplete line may be returned then). Return an empty\r\nstring at EOF.\r\n")]
    public new string readline()
    {
      this.ThrowIfClosed();
      return base.readline();
    }

    public new string readline(int sizehint)
    {
      this.ThrowIfClosed();
      return base.readline(sizehint);
    }

    [Documentation("readlines([size]) -> list\r\n\r\nCall readline() repeatedly and return a list of lines read.\r\nThe optional size argument, if given, is an approximate bound on the\r\ntotal number of bytes in the lines returned.\r\n")]
    public new IronPython.Runtime.List readlines()
    {
      if (this.closed)
        throw PythonOps.ValueError("I/O operation on closed file");
      return base.readlines();
    }

    public new IronPython.Runtime.List readlines(int sizehint)
    {
      if (this.closed)
        throw PythonOps.ValueError("I/O operation on closed file");
      return base.readlines(sizehint);
    }

    [Documentation("xreadlines() -> self\r\n\r\nFor backward compatibility. BZ2File objects now include the performance\r\noptimizations previously implemented in the xreadlines module.\r\n")]
    public Bz2Module.BZ2File xreadlines() => this;

    [Documentation("seek(offset [, whence]) -> None\r\n\r\nMove to new file position. Argument offset is a byte count. Optional\r\nargument whence defaults to 0 (offset from start of file, offset\r\nshould be >= 0); other values are 1 (move relative to current position,\r\npositive or negative), and 2 (move relative to end of file, usually\r\nnegative, although many platforms allow seeking beyond the end of a file).\r\n\r\nNote that seeking of bz2 files is emulated, and depending on the parameters\r\nthe operation may be extremely slow.\r\n")]
    public new void seek(long offset, int whence = 0) => throw new NotImplementedException();

    [Documentation("tell() -> int\r\n\r\nReturn the current file position, an integer (may be a long integer).\r\n")]
    public new object tell() => throw new NotImplementedException();

    [Documentation("write(data) -> None\r\n\r\nWrite the 'data' string to file. Note that due to buffering, close() may\r\nbe needed before the file on disk reflects the data written.\r\n")]
    public new void write([BytesConversion] IList<byte> data)
    {
      this.ThrowIfClosed();
      base.write(data);
    }

    public new void write(object data)
    {
      this.ThrowIfClosed();
      base.write(data);
    }

    public new void write(string data)
    {
      this.ThrowIfClosed();
      base.write(data);
    }

    public new void write(PythonBuffer data)
    {
      this.ThrowIfClosed();
      base.write(data);
    }

    [Documentation("writelines(sequence_of_strings) -> None\r\n\r\nWrite the sequence of strings to the file. Note that newlines are not\r\nadded. The sequence can be any iterable object producing strings. This is\r\nequivalent to calling write() for each string.\r\n")]
    public new void writelines(object sequence_of_strings)
    {
      this.ThrowIfClosed();
      base.writelines(sequence_of_strings);
    }

    public void __del__() => this.close();

    [Documentation("__enter__() -> self.")]
    public new object __enter__()
    {
      this.ThrowIfClosed();
      return (object) this;
    }

    [Documentation("__exit__(*excinfo) -> None.  Closes the file.")]
    public new void __exit__(params object[] excinfo) => this.close();
  }
}
