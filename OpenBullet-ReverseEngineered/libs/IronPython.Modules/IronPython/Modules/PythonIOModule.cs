// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonIOModule
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonIOModule
{
  public const int DEFAULT_BUFFER_SIZE = 8192 /*0x2000*/;
  private static readonly object _blockingIOErrorKey = new object();
  private static readonly object _unsupportedOperationKey = new object();
  private static HashSet<char> _validModes = PythonIOModule.MakeSet("abrtwU+");

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.EnsureModuleException(PythonIOModule._blockingIOErrorKey, PythonExceptions.IOError, typeof (PythonIOModule.BlockingIOError), dict, "BlockingIOError", "__builtin__", (Func<string, Exception>) (msg => (Exception) new PythonIOModule._BlockingIOErrorException(msg))).IsSystemType = true;
    context.EnsureModuleException(PythonIOModule._unsupportedOperationKey, new PythonType[2]
    {
      PythonExceptions.ValueError,
      PythonExceptions.IOError
    }, typeof (PythonExceptions.BaseException), dict, "UnsupportedOperation", "io");
  }

  public static PythonIOModule._IOBase open(
    CodeContext context,
    object file,
    string mode = "r",
    int buffering = -1,
    string encoding = null,
    string errors = null,
    string newline = null,
    bool closefd = true)
  {
    int fd = -1;
    switch (file)
    {
      case string name:
label_3:
        HashSet<char> other = PythonIOModule.MakeSet(mode);
        if (other.Count < mode.Length || !PythonIOModule._validModes.IsSupersetOf((IEnumerable<char>) other))
          throw PythonOps.ValueError("invalid mode: {0}", (object) mode);
        bool flag1 = other.Contains('r');
        bool flag2 = other.Contains('w');
        bool flag3 = other.Contains('a');
        bool flag4 = other.Contains('+');
        bool flag5 = other.Contains('t');
        bool flag6 = other.Contains('b');
        if (other.Contains('U'))
        {
          if (flag2 | flag3)
            throw PythonOps.ValueError("can't use U and writing mode at once");
          flag1 = true;
        }
        if (flag5 & flag6)
          throw PythonOps.ValueError("can't have text and binary mode at once");
        if (flag1 & flag2 || flag1 & flag3 || flag2 & flag3)
          throw PythonOps.ValueError("can't have read/write/append mode at once");
        if (!(flag1 | flag2 | flag3))
          throw PythonOps.ValueError("must have exactly one of read/write/append mode");
        if (flag6 && encoding != null)
          throw PythonOps.ValueError("binary mode doesn't take an encoding argument");
        if (flag6 && newline != null)
          throw PythonOps.ValueError("binary mode doesn't take a newline argument");
        mode = flag1 ? "r" : "";
        if (flag2)
          mode += "w";
        if (flag3)
          mode += "a";
        if (flag4)
          mode += "+";
        PythonIOModule.FileIO raw = name == null ? new PythonIOModule.FileIO(context, fd, mode, closefd) : new PythonIOModule.FileIO(context, name, mode, closefd);
        bool line_buffering = false;
        if (buffering == 1 || buffering < 0 && raw.isatty(context))
        {
          buffering = -1;
          line_buffering = true;
        }
        if (buffering < 0)
          buffering = 8192 /*0x2000*/;
        if (buffering == 0)
        {
          if (flag6)
            return (PythonIOModule._IOBase) raw;
          throw PythonOps.ValueError("can't have unbuffered text I/O");
        }
        PythonIOModule._BufferedIOBase buffer;
        if (flag4)
          buffer = (PythonIOModule._BufferedIOBase) PythonIOModule.BufferedRandom.Create(context, (PythonIOModule._IOBase) raw, buffering, (object) null);
        else if (flag2 | flag3)
          buffer = (PythonIOModule._BufferedIOBase) PythonIOModule.BufferedWriter.Create(context, (object) raw, buffering, (object) null);
        else if (flag1)
          buffer = (PythonIOModule._BufferedIOBase) PythonIOModule.BufferedReader.Create(context, (object) raw, buffering);
        else
          throw PythonOps.ValueError("unknown mode: {0}", (object) mode);
        if (flag6)
          return (PythonIOModule._IOBase) buffer;
        PythonIOModule.TextIOWrapper textIoWrapper = PythonIOModule.TextIOWrapper.Create(context, (object) buffer, encoding, errors, newline, line_buffering);
        ((IPythonExpandable) textIoWrapper).EnsureCustomAttributes()[nameof (mode)] = (object) mode;
        return (PythonIOModule._IOBase) textIoWrapper;
      case Extensible<string> _:
        name = ((Extensible<string>) file).Value;
        goto label_3;
      default:
        fd = PythonIOModule.GetInt(file, 0);
        goto label_3;
    }
  }

  private static HashSet<char> MakeSet(string chars)
  {
    HashSet<char> charSet = new HashSet<char>();
    for (int index = 0; index < chars.Length; ++index)
      charSet.Add(chars[index]);
    return charSet;
  }

  private static BigInteger GetBigInt(object i, string msg)
  {
    BigInteger res;
    if (PythonIOModule.TryGetBigInt(i, out res))
      return res;
    throw PythonOps.TypeError(msg);
  }

  private static bool TryGetBigInt(object i, out BigInteger res)
  {
    switch (i)
    {
      case BigInteger bigInteger:
        res = bigInteger;
        return true;
      case int num1:
        res = (BigInteger) num1;
        return true;
      case long num2:
        res = (BigInteger) num2;
        return true;
      case Extensible<int> extensible1:
        res = (BigInteger) extensible1.Value;
        return true;
      case Extensible<BigInteger> extensible2:
        res = extensible2.Value;
        return true;
      default:
        res = BigInteger.Zero;
        return false;
    }
  }

  private static int GetInt(object i) => PythonIOModule.GetInt(i, (string) null, (object[]) null);

  private static int GetInt(object i, int defaultValue)
  {
    return PythonIOModule.GetInt(i, defaultValue, (string) null, (object[]) null);
  }

  private static int GetInt(object i, string msg, params object[] args)
  {
    int num;
    if (PythonIOModule.TryGetInt(i, out num))
      return num;
    if (msg == null)
      throw PythonOps.TypeError("integer argument expected, got '{0}'", (object) PythonTypeOps.GetName(i));
    throw PythonOps.TypeError(msg, args);
  }

  private static int GetInt(object i, int defaultValue, string msg, params object[] args)
  {
    return i == null ? defaultValue : PythonIOModule.GetInt(i, msg, args);
  }

  private static bool TryGetInt(object i, out int value)
  {
    switch (i)
    {
      case null:
        value = int.MinValue;
        return false;
      case int num:
        value = num;
        return true;
      case BigInteger self:
        return self.AsInt32(out value);
      case Extensible<int> extensible1:
        value = extensible1.Value;
        return true;
      case Extensible<BigInteger> extensible2:
        return extensible2.Value.AsInt32(out value);
      default:
        value = int.MinValue;
        return false;
    }
  }

  private static Bytes GetBytes(object o, string name)
  {
    switch (o)
    {
      case null:
        return (Bytes) null;
      case Bytes bytes:
        return bytes;
      case Extensible<string> extensible:
        pattern_1 = extensible.Value;
        break;
    }
    return pattern_1 != null ? PythonOps.MakeBytes(pattern_1.MakeByteArray()) : throw PythonOps.TypeError($"'{name}' should have returned bytes");
  }

  private static IList<byte> GetBytes(object buf)
  {
    switch (buf)
    {
      case IList<byte> bytes:
        return bytes;
      case Extensible<string> _:
        pattern_1 = ((Extensible<string>) buf).Value;
        break;
    }
    if (pattern_1 != null)
      return (IList<byte>) pattern_1.MakeByteArray();
    return buf is ArrayModule.array array ? (IList<byte>) array.ToByteArray() : throw PythonOps.TypeError("must be bytes or buffer, not {0}", (object) PythonTypeOps.GetName(buf));
  }

  [PythonType]
  [DontMapIDisposableToContextManager]
  public class BytesIO : 
    PythonIOModule._BufferedIOBase,
    IEnumerator,
    IDisposable,
    IDynamicMetaObjectProvider
  {
    private static readonly int DEFAULT_BUF_SIZE = 20;
    private byte[] _data;
    private int _pos;
    private int _length;
    private object _current;

    internal BytesIO(CodeContext context)
      : base(context)
    {
    }

    public BytesIO(CodeContext context, object initial_bytes = null)
      : base(context)
    {
    }

    public void __init__(object initial_bytes = null)
    {
      if (this._data == null)
        this._data = new byte[PythonIOModule.BytesIO.DEFAULT_BUF_SIZE];
      this._pos = this._length = 0;
      if (initial_bytes == null)
        return;
      this.DoWrite(initial_bytes);
      this._pos = 0;
    }

    public override void close(CodeContext context) => this._data = (byte[]) null;

    public override bool closed => this._data == null;

    public Bytes getvalue()
    {
      this._checkClosed();
      if (this._length == 0)
        return Bytes.Empty;
      byte[] numArray = new byte[this._length];
      Array.Copy((Array) this._data, (Array) numArray, this._length);
      return Bytes.Make(numArray);
    }

    [Documentation("isatty() -> False\n\nAlways returns False since BytesIO objects are not connected\nto a TTY-like device.")]
    public override bool isatty(CodeContext context)
    {
      this._checkClosed();
      return false;
    }

    [Documentation("read([size]) -> read at most size bytes, returned as a bytes object.\n\nIf the size argument is negative, read until EOF is reached.\nReturn an empty string at EOF.")]
    public override object read(CodeContext context, object size = null)
    {
      this._checkClosed();
      int val2 = PythonIOModule.GetInt(size, -1);
      int length = Math.Max(0, this._length - this._pos);
      if (val2 >= 0)
        length = Math.Min(length, val2);
      if (length == 0)
        return (object) Bytes.Empty;
      byte[] numArray = new byte[length];
      Array.Copy((Array) this._data, this._pos, (Array) numArray, 0, length);
      this._pos += length;
      return (object) Bytes.Make(numArray);
    }

    [Documentation("read1(size) -> read at most size bytes, returned as a bytes object.\n\nIf the size argument is negative or omitted, read until EOF is reached.\nReturn an empty string at EOF.")]
    public override Bytes read1(CodeContext context, int size)
    {
      return (Bytes) this.read(context, (object) size);
    }

    public override bool readable(CodeContext context) => true;

    [Documentation("readinto(array_or_bytearray) -> int.  Read up to len(b) bytes into b.\n\nReturns number of bytes read (0 for EOF).")]
    public BigInteger readinto([NotNull] ByteArray buffer)
    {
      this._checkClosed();
      int num = Math.Min(this._length - this._pos, buffer.Count);
      for (int index = 0; index < num; ++index)
        buffer[index] = (object) this._data[this._pos++];
      return (BigInteger) num;
    }

    public BigInteger readinto([NotNull] ArrayModule.array buffer)
    {
      this._checkClosed();
      int num1 = Math.Min(this._length - this._pos, buffer.__len__() * buffer.itemsize);
      int num2 = num1 % buffer.itemsize;
      buffer.FromStream((Stream) new MemoryStream(this._data, this._pos, num1 - num2, false, false), 0);
      this._pos += num1 - num2;
      if (num2 != 0)
      {
        byte[] buffer1 = buffer.RawGetItem(num1 / buffer.itemsize);
        for (int index = 0; index < num2; ++index)
          buffer1[index] = this._data[this._pos++];
        buffer.FromStream((Stream) new MemoryStream(buffer1), num1 / buffer.itemsize);
      }
      return (BigInteger) num1;
    }

    public override BigInteger readinto(CodeContext context, object buf)
    {
      switch (buf)
      {
        case ByteArray buffer1:
          return this.readinto(buffer1);
        case ArrayModule.array buffer2:
          return this.readinto(buffer2);
        default:
          this._checkClosed();
          throw PythonOps.TypeError("must be read-write buffer, not {0}", (object) PythonTypeOps.GetName(buf));
      }
    }

    [Documentation("readline([size]) -> next line from the file, as bytes.\n\nRetain newline.  A non-negative size argument limits the maximum\nnumber of bytes to return (an incomplete line may be returned then).\nReturn an empty string at EOF.")]
    public override object readline(CodeContext context, int limit = -1)
    {
      return (object) this.readline(limit);
    }

    private Bytes readline(int size = -1)
    {
      this._checkClosed();
      if (this._pos >= this._length || size == 0)
        return Bytes.Empty;
      int pos;
      for (pos = this._pos; (size < 0 || this._pos - pos < size) && this._pos < this._length; ++this._pos)
      {
        if (this._data[this._pos] == (byte) 10)
        {
          ++this._pos;
          break;
        }
      }
      byte[] numArray = new byte[this._pos - pos];
      Array.Copy((Array) this._data, pos, (Array) numArray, 0, this._pos - pos);
      return Bytes.Make(numArray);
    }

    public Bytes readline(object size)
    {
      if (size == null)
        return this.readline(-1);
      this._checkClosed();
      throw PythonOps.TypeError("integer argument expected, got '{0}'", (object) PythonTypeOps.GetName(size));
    }

    [Documentation("readlines([size]) -> list of bytes objects, each a line from the file.\n\nCall readline() repeatedly and return a list of the lines so read.\nThe optional size argument, if given, is an approximate bound on the\ntotal number of bytes in the lines returned.")]
    public override IronPython.Runtime.List readlines(object hint = null)
    {
      this._checkClosed();
      int num = PythonIOModule.GetInt(hint, -1);
      IronPython.Runtime.List list = new IronPython.Runtime.List();
      for (Bytes bytes = this.readline(-1); bytes.Count > 0; bytes = this.readline(-1))
      {
        list.append((object) bytes);
        if (num > 0)
        {
          num -= bytes.Count;
          if (num <= 0)
            break;
        }
      }
      return list;
    }

    [Documentation("seek(pos, whence=0) -> int.  Change stream position.\n\nSeek to byte offset pos relative to position indicated by whence:\n     0  Start of stream (the default).  pos should be >= 0;\n     1  Current position - pos may be negative;\n     2  End of stream - pos usually negative.\nReturns the new absolute position.")]
    public BigInteger seek(int pos, int whence = 0)
    {
      this._checkClosed();
      switch (whence)
      {
        case 0:
          this._pos = pos >= 0 ? pos : throw PythonOps.ValueError("negative seek value {0}", (object) pos);
          return (BigInteger) this._pos;
        case 1:
          this._pos = Math.Max(0, this._pos + pos);
          return (BigInteger) this._pos;
        case 2:
          this._pos = Math.Max(0, this._length + pos);
          return (BigInteger) this._pos;
        default:
          throw PythonOps.ValueError("invalid whence ({0}, should be 0, 1 or 2)", (object) whence);
      }
    }

    public BigInteger seek(int pos, BigInteger whence) => this.seek(pos, (int) whence);

    public BigInteger seek(int pos, double whence)
    {
      throw PythonOps.TypeError("integer argument expected, got float");
    }

    public BigInteger seek(double pos, object whence = 0)
    {
      throw PythonOps.TypeError("'float' object cannot be interpreted as an index");
    }

    public override BigInteger seek(CodeContext context, BigInteger pos, object whence = 0)
    {
      this._checkClosed();
      int pos1 = (int) pos;
      switch (whence)
      {
        case int whence1:
          return this.seek(pos1, whence1);
        case Extensible<int> whence2:
          return this.seek(pos1, (int) whence2);
        case BigInteger whence3:
          return this.seek(pos1, whence3);
        case Extensible<BigInteger> whence4:
          return this.seek(pos1, (BigInteger) whence4);
        case double _:
        case Extensible<double> _:
          throw PythonOps.TypeError("integer argument expected, got float");
        default:
          return this.seek(pos1, PythonIOModule.GetInt(whence));
      }
    }

    public override bool seekable(CodeContext context) => true;

    [Documentation("tell() -> current file position, an integer")]
    public override BigInteger tell(CodeContext context)
    {
      this._checkClosed();
      return (BigInteger) this._pos;
    }

    [Documentation("truncate([size]) -> int.  Truncate the file to at most size bytes.\n\nSize defaults to the current file position, as returned by tell().\nReturns the new size.  Imply an absolute seek to the position size.")]
    public BigInteger truncate() => this.truncate(this._pos);

    public BigInteger truncate(int size)
    {
      this._checkClosed();
      this._length = size >= 0 ? Math.Min(this._length, size) : throw PythonOps.ValueError("negative size value {0}", (object) size);
      return (BigInteger) size;
    }

    public override BigInteger truncate(CodeContext context, object size = null)
    {
      if (size == null)
        return this.truncate();
      int size1;
      if (PythonIOModule.TryGetInt(size, out size1))
        return this.truncate(size1);
      this._checkClosed();
      throw PythonOps.TypeError("integer argument expected, got '{0}'", (object) PythonTypeOps.GetName(size));
    }

    public override bool writable(CodeContext context) => true;

    [Documentation("write(bytes) -> int.  Write bytes to file.\n\nReturn the number of bytes written.")]
    public override BigInteger write(CodeContext context, object bytes)
    {
      this._checkClosed();
      return (BigInteger) this.DoWrite(bytes);
    }

    [Documentation("writelines(sequence_of_strings) -> None.  Write strings to the file.\n\nNote that newlines are not added.  The sequence can be any iterable\nobject producing strings. This is equivalent to calling write() for\neach string.")]
    public void writelines([NotNull] IEnumerable lines)
    {
      this._checkClosed();
      foreach (object line in lines)
        this.DoWrite(line);
    }

    void IDisposable.Dispose()
    {
    }

    object IEnumerator.Current
    {
      get
      {
        this._checkClosed();
        return this._current;
      }
    }

    bool IEnumerator.MoveNext()
    {
      Bytes bytes = this.readline(-1);
      if (bytes.Count == 0)
        return false;
      this._current = (object) bytes;
      return true;
    }

    void IEnumerator.Reset()
    {
      this.seek(0);
      this._current = (object) null;
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule.BytesIO>(parameter, (IPythonExpandable) this);
    }

    private int DoWrite(byte[] bytes)
    {
      if (bytes.Length == 0)
        return 0;
      this.EnsureSizeSetLength(this._pos + bytes.Length);
      Array.Copy((Array) bytes, 0, (Array) this._data, this._pos, bytes.Length);
      this._pos += bytes.Length;
      return bytes.Length;
    }

    private int DoWrite(ICollection<byte> bytes)
    {
      int count = bytes.Count;
      if (count == 0)
        return 0;
      this.EnsureSizeSetLength(this._pos + count);
      bytes.CopyTo(this._data, this._pos);
      this._pos += count;
      return count;
    }

    private int DoWrite(string bytes)
    {
      int length = bytes.Length;
      if (length == 0)
        return 0;
      byte[] bytes1 = new byte[length];
      for (int index = 0; index < length; ++index)
      {
        int num = (int) bytes[index];
        bytes1[index] = num < 256 /*0x0100*/ ? (byte) num : throw PythonOps.TypeError("'unicode' does not have the buffer interface");
      }
      return this.DoWrite(bytes1);
    }

    private int DoWrite(object bytes)
    {
      switch (bytes)
      {
        case byte[] _:
          return this.DoWrite((byte[]) bytes);
        case Bytes _:
          return this.DoWrite(((Bytes) bytes)._bytes);
        case ArrayModule.array _:
          return this.DoWrite(((ArrayModule.array) bytes).ToByteArray());
        case ICollection<byte> _:
          return this.DoWrite((ICollection<byte>) bytes);
        case string _:
          return this.DoWrite((string) bytes);
        default:
          throw PythonOps.TypeError("expected a readable buffer object");
      }
    }

    private void EnsureSize(int size)
    {
      if (this._data.Length >= size)
        return;
      size = size > PythonIOModule.BytesIO.DEFAULT_BUF_SIZE ? Math.Max(size, this._data.Length * 2) : PythonIOModule.BytesIO.DEFAULT_BUF_SIZE;
      byte[] data1 = this._data;
      this._data = new byte[size];
      byte[] data2 = this._data;
      int length = this._length;
      Array.Copy((Array) data1, (Array) data2, length);
    }

    private void EnsureSizeSetLength(int size)
    {
      if (this._data.Length < size)
      {
        this.EnsureSize(size);
        this._length = size;
      }
      else
      {
        while (this._length < this._pos)
          this._data[this._length++] = (byte) 0;
        this._length = Math.Max(this._length, size);
      }
    }
  }

  [Documentation("file(name: str[, mode: str]) -> file IO object\n\nOpen a file.  The mode can be 'r', 'w' or 'a' for reading (default),\nwriting or appending.   The file will be created if it doesn't exist\nwhen opened for writing or appending; it will be truncated when\nopened for writing.  Add a '+' to the mode to allow simultaneous\nreading and writing.")]
  [PythonType]
  [DontMapIDisposableToContextManager]
  public class FileIO : 
    PythonIOModule._RawIOBase,
    IDisposable,
    IWeakReferenceable,
    ICodeFormattable,
    IDynamicMetaObjectProvider
  {
    private static readonly int DEFAULT_BUF_SIZE = 32 /*0x20*/;
    internal Stream _readStream;
    private Stream _writeStream;
    private bool _closed;
    private bool _closefd;
    private WeakRefTracker _tracker;
    private PythonContext _context;
    public object name;

    public FileIO(CodeContext context, int fd, string mode = "r", bool closefd = true)
      : base(context)
    {
      if (fd < 0)
        throw PythonOps.ValueError("fd must be >= 0");
      PythonContext languageContext = context.LanguageContext;
      this._context = languageContext;
      switch (PythonIOModule.FileIO.StandardizeMode(mode))
      {
        case "+a":
        case "a+":
          this.mode = "r+";
          break;
        case "+r":
        case "r+":
          this.mode = "rb+";
          break;
        case "+w":
        case "w+":
          this.mode = "rb+";
          break;
        case "a":
          this.mode = "w";
          break;
        case "r":
          this.mode = "rb";
          break;
        case "w":
          this.mode = "wb";
          break;
        default:
          PythonIOModule.FileIO.BadMode(mode);
          break;
      }
      PythonFile pf;
      if (languageContext.FileManager.TryGetFileFromId(languageContext, fd, out pf))
      {
        this.name = (object) pf.name ?? (object) fd;
        this._readStream = pf._stream;
        this._writeStream = pf._stream;
      }
      else
      {
        object o;
        if (languageContext.FileManager.TryGetObjectFromId(languageContext, fd, out o))
        {
          switch (o)
          {
            case PythonIOModule.FileIO fileIo:
              this.name = fileIo.name ?? (object) fd;
              this._readStream = fileIo._readStream;
              this._writeStream = fileIo._writeStream;
              break;
            case Stream stream:
              this.name = (object) fd;
              this._readStream = stream;
              this._writeStream = stream;
              break;
          }
        }
      }
      this._closefd = closefd;
    }

    public FileIO(CodeContext context, string name, string mode = "r", bool closefd = true)
      : base(context)
    {
      if (!closefd)
        throw PythonOps.ValueError("Cannot use closefd=False with file name");
      this._closefd = true;
      this.name = (object) name;
      PlatformAdaptationLayer platform = context.LanguageContext.DomainManager.Platform;
      switch (PythonIOModule.FileIO.StandardizeMode(mode))
      {
        case "+a":
        case "a+":
          this._readStream = PythonIOModule.FileIO.OpenFile(context, platform, name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
          this._writeStream = PythonIOModule.FileIO.OpenFile(context, platform, name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
          this._readStream.Seek(0L, SeekOrigin.End);
          this._writeStream.Seek(0L, SeekOrigin.End);
          this.mode = "ab+";
          break;
        case "+r":
        case "r+":
          this._readStream = this._writeStream = PythonIOModule.FileIO.OpenFile(context, platform, name, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
          this.mode = "rb+";
          break;
        case "+w":
        case "w+":
          this._readStream = this._writeStream = PythonIOModule.FileIO.OpenFile(context, platform, name, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
          this.mode = "rb+";
          break;
        case "a":
          this._readStream = this._writeStream = PythonIOModule.FileIO.OpenFile(context, platform, name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
          this._readStream.Seek(0L, SeekOrigin.End);
          this.mode = "ab";
          break;
        case "r":
          this._readStream = this._writeStream = PythonIOModule.FileIO.OpenFile(context, platform, name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
          this.mode = "rb";
          break;
        case "w":
          this._readStream = this._writeStream = PythonIOModule.FileIO.OpenFile(context, platform, name, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
          this.mode = "wb";
          break;
        default:
          PythonIOModule.FileIO.BadMode(mode);
          break;
      }
      this._context = context.LanguageContext;
    }

    private static string StandardizeMode(string mode)
    {
      int length = mode.IndexOf('b');
      if (length == mode.Length - 1)
        mode = mode.Substring(0, length);
      else if (length >= 0)
      {
        StringBuilder stringBuilder = new StringBuilder(mode.Substring(0, length), mode.Length - 1);
        for (int index = length + 1; index < mode.Length; ++index)
        {
          if (mode[index] != 'b')
            stringBuilder.Append(mode[index]);
        }
        mode = stringBuilder.ToString();
      }
      return mode;
    }

    private static void BadMode(string mode)
    {
      bool flag1 = false;
      bool flag2 = false;
      foreach (char ch in mode)
      {
        switch (ch)
        {
          case '+':
            flag2 = !flag2 ? true : throw PythonOps.ValueError("Must have exactly one of read/write/append mode");
            continue;
          case 'a':
          case 'r':
          case 'w':
            flag1 = !flag1 ? true : throw PythonOps.ValueError("Must have exactly one of read/write/append mode");
            continue;
          case 'b':
            continue;
          default:
            throw PythonOps.ValueError("invalid mode: {0}", (object) mode);
        }
      }
      throw PythonOps.ValueError("Must have exactly one of read/write/append mode");
    }

    [Documentation("close() -> None.  Close the file.\n\nA closed file cannot be used for further I/O operations.  close() may becalled more than once without error.  Changes the fileno to -1.")]
    public override void close(CodeContext context)
    {
      if (this._closed)
        return;
      this.flush(context);
      this._closed = true;
      if (this._closefd)
      {
        this._readStream.Dispose();
        if (this._readStream != this._writeStream)
          this._writeStream.Dispose();
      }
      this._context.RawFileManager?.Remove((object) this);
    }

    [Documentation("True if the file is closed")]
    public override bool closed => this._closed;

    public bool closefd => this._closefd;

    [Documentation("fileno() -> int. \"file descriptor\".\n\nThis is needed for lower-level file interfaces, such as the fcntl module.")]
    public override int fileno(CodeContext context)
    {
      this._checkClosed();
      return this._context.FileManager.GetOrAssignIdForObject((object) this);
    }

    [Documentation("Flush write buffers, if applicable.\n\nThis is not implemented for read-only and non-blocking streams.\n")]
    public override void flush(CodeContext context)
    {
      this._checkClosed();
      if (this._writeStream == null)
        return;
      this._writeStream.Flush();
    }

    [Documentation("isatty() -> bool.  True if the file is connected to a tty device.")]
    public override bool isatty(CodeContext context)
    {
      this._checkClosed();
      return false;
    }

    [Documentation("String giving the file mode")]
    public string mode { get; }

    [Documentation("read(size: int) -> bytes.  read at most size bytes, returned as bytes.\n\nOnly makes one system call, so less data may be returned than requested\nIn non-blocking mode, returns None if no data is available.\nOn end-of-file, returns ''.")]
    public override object read(CodeContext context, object size = null)
    {
      int count = PythonIOModule.GetInt(size, -1);
      if (count < 0)
        return (object) this.readall();
      this.EnsureReadable();
      byte[] array = new byte[count];
      int newSize = this._readStream.Read(array, 0, count);
      Array.Resize<byte>(ref array, newSize);
      return (object) Bytes.Make(array);
    }

    [Documentation("readable() -> bool.  True if file was opened in a read mode.")]
    public override bool readable(CodeContext context)
    {
      this._checkClosed();
      return this._readStream.CanRead;
    }

    [Documentation("readall() -> bytes.  read all data from the file, returned as bytes.\n\nIn non-blocking mode, returns as much as is immediately available,\nor None if no data is available.  On end-of-file, returns ''.")]
    public Bytes readall()
    {
      this.EnsureReadable();
      int defaultBufSize = PythonIOModule.FileIO.DEFAULT_BUF_SIZE;
      byte[] array = new byte[defaultBufSize];
      int newSize;
      for (newSize = this._readStream.Read(array, 0, defaultBufSize); newSize == defaultBufSize; defaultBufSize *= 2)
      {
        Array.Resize<byte>(ref array, defaultBufSize * 2);
        newSize += this._readStream.Read(array, defaultBufSize, defaultBufSize);
      }
      Array.Resize<byte>(ref array, newSize);
      return Bytes.Make(array);
    }

    [Documentation("readinto() -> Same as RawIOBase.readinto().")]
    public BigInteger readinto([NotNull] ArrayModule.array buffer)
    {
      this.EnsureReadable();
      return (BigInteger) (int) buffer.FromStream(this._readStream, 0, buffer.__len__() * buffer.itemsize);
    }

    public BigInteger readinto([NotNull] ByteArray buffer)
    {
      this.EnsureReadable();
      for (int index = 0; index < buffer.Count; ++index)
      {
        int num = this._readStream.ReadByte();
        if (num == -1)
          return (BigInteger) (index - 1);
        buffer[index] = (object) (byte) num;
      }
      return (BigInteger) buffer.Count;
    }

    public BigInteger readinto([NotNull] PythonBuffer buffer)
    {
      this.EnsureReadable();
      throw PythonOps.TypeError("buffer is read-only");
    }

    public override BigInteger readinto(CodeContext context, object buf)
    {
      switch (buf)
      {
        case ByteArray buffer:
          return this.readinto(buffer);
        case ArrayModule.array _:
          return this.readinto(buffer);
        default:
          this.EnsureReadable();
          throw PythonOps.TypeError("argument 1 must be read/write buffer, not {0}", (object) DynamicHelpers.GetPythonType(buf).Name);
      }
    }

    [Documentation("seek(offset: int[, whence: int]) -> None.  Move to new file position.\n\nArgument offset is a byte count.  Optional argument whence defaults to\n0 (offset from start of file, offset should be >= 0); other values are 1\n(move relative to current position, positive or negative), and 2 (move\nrelative to end of file, usually negative, although many platforms allow\nseeking beyond the end of a file).\nNote that not all file objects are seekable.")]
    public override BigInteger seek(CodeContext context, BigInteger offset, object whence = 0)
    {
      this._checkClosed();
      return (BigInteger) this._readStream.Seek((long) offset, (SeekOrigin) PythonIOModule.GetInt(whence));
    }

    public BigInteger seek(double offset, object whence = 0)
    {
      this._checkClosed();
      throw PythonOps.TypeError("an integer is required");
    }

    [Documentation("seekable() -> bool.  True if file supports random-access.")]
    public override bool seekable(CodeContext context)
    {
      this._checkClosed();
      return this._readStream.CanSeek;
    }

    [Documentation("tell() -> int.  Current file position")]
    public override BigInteger tell(CodeContext context)
    {
      this._checkClosed();
      return (BigInteger) this._readStream.Position;
    }

    public BigInteger truncate(BigInteger size)
    {
      this.EnsureWritable();
      long position = this._readStream.Position;
      this._writeStream.SetLength((long) size);
      this._readStream.Seek(position, SeekOrigin.Begin);
      return size;
    }

    public BigInteger truncate(double size)
    {
      this.EnsureWritable();
      throw PythonOps.TypeError("an integer is required");
    }

    [Documentation("truncate([size: int]) -> None.  Truncate the file to at most size bytes.\n\nSize defaults to the current file position, as returned by tell().The current file position is changed to the value of size.")]
    public override BigInteger truncate(CodeContext context, object pos = null)
    {
      if (pos == null)
        return this.truncate(this.tell(context));
      BigInteger res;
      if (PythonIOModule.TryGetBigInt(pos, out res))
        return this.truncate(res);
      this.EnsureWritable();
      throw PythonOps.TypeError("an integer is required");
    }

    [Documentation("writable() -> bool.  True if file was opened in a write mode.")]
    public override bool writable(CodeContext context)
    {
      this._checkClosed();
      return this._writeStream.CanWrite;
    }

    private BigInteger write([NotNull] byte[] b)
    {
      this.EnsureWritable();
      this._writeStream.Write(b, 0, b.Length);
      this.SeekToEnd();
      return (BigInteger) b.Length;
    }

    private BigInteger write([NotNull] Bytes b) => this.write(b._bytes);

    private BigInteger write([NotNull] ICollection<byte> b)
    {
      this.EnsureWritable();
      int count = b.Count;
      byte[] numArray = new byte[count];
      b.CopyTo(numArray, 0);
      this._writeStream.Write(numArray, 0, count);
      this.SeekToEnd();
      return (BigInteger) count;
    }

    private BigInteger write([NotNull] string s) => this.write(s.MakeByteArray());

    [Documentation("write(b: bytes) -> int.  Write bytes b to file, return number written.\n\nOnly makes one system call, so not all the data may be written.\nThe number of bytes actually written is returned.")]
    public override BigInteger write(CodeContext context, object b)
    {
      switch (b)
      {
        case byte[] b1:
          return this.write(b1);
        case Bytes b2:
          return this.write(b2);
        case ArrayModule.array array:
          return this.write(array.ToByteArray());
        case ICollection<byte> b3:
          return this.write(b3);
        default:
          this.EnsureWritable();
          throw PythonOps.TypeError("expected a readable buffer object");
      }
    }

    public string __repr__(CodeContext context)
    {
      return $"<_io.FileIO name={PythonOps.Repr(context, this.name)} mode='{this.mode}'>";
    }

    void IDisposable.Dispose()
    {
    }

    WeakRefTracker IWeakReferenceable.GetWeakRef() => this._tracker;

    bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
    {
      this._tracker = value;
      return true;
    }

    void IWeakReferenceable.SetFinalizer(WeakRefTracker value)
    {
      ((IWeakReferenceable) this).SetWeakRef(value);
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule.FileIO>(parameter, (IPythonExpandable) this);
    }

    private static Stream OpenFile(
      CodeContext context,
      PlatformAdaptationLayer pal,
      string name,
      FileMode fileMode,
      FileAccess fileAccess,
      FileShare fileShare)
    {
      try
      {
        return pal.OpenInputFileStream(name, fileMode, fileAccess, fileShare);
      }
      catch (UnauthorizedAccessException ex)
      {
        throw PythonFile.ToIoException(context, name, ex);
      }
      catch (IOException ex)
      {
        PythonFile.AddFilename(context, name, (Exception) ex);
        throw;
      }
    }

    private void EnsureReadable()
    {
      this._checkClosed();
      this._checkReadable("File not open for reading");
    }

    private void EnsureWritable()
    {
      this._checkClosed();
      this._checkWritable("File not open for writing");
    }

    private void SeekToEnd()
    {
      if (this._readStream == this._writeStream)
        return;
      this._readStream.Seek(this._writeStream.Position, SeekOrigin.Begin);
    }
  }

  [PythonType]
  [DontMapGetMemberNamesToDir]
  public class _IOBase : 
    IDisposable,
    IEnumerator<object>,
    IEnumerator,
    IEnumerable<object>,
    IEnumerable,
    IWeakReferenceable,
    IDynamicMetaObjectProvider,
    IPythonExpandable
  {
    private bool _closed;
    internal CodeContext context;
    private object _current;
    private WeakRefTracker _weakref;
    private IDictionary<string, object> _customAttributes;

    public _IOBase(CodeContext context) => this.context = context;

    public void __del__(CodeContext context) => this.close(context);

    public PythonIOModule._IOBase __enter__()
    {
      this._checkClosed();
      return this;
    }

    public void __exit__(CodeContext context, params object[] excinfo) => this.close(context);

    public void _checkClosed() => this._checkClosed((string) null);

    public void _checkClosed(string msg)
    {
      if (this.closed)
        throw PythonOps.ValueError(msg ?? "I/O operation on closed file.");
    }

    public void _checkReadable() => this._checkReadable((string) null);

    public void _checkReadable(string msg)
    {
      if (!this.readable(this.context))
        throw PythonOps.ValueError(msg ?? "File or stream is not readable.");
    }

    public void _checkSeekable() => this._checkSeekable((string) null);

    public void _checkSeekable(string msg)
    {
      if (!this.seekable(this.context))
        throw PythonOps.ValueError(msg ?? "File or stream is not seekable.");
    }

    public void _checkWritable() => this._checkWritable((string) null);

    public void _checkWritable(string msg)
    {
      if (!this.writable(this.context))
        throw PythonOps.ValueError(msg ?? "File or stream is not writable.");
    }

    public virtual void close(CodeContext context)
    {
      try
      {
        if (this._closed)
          return;
        this.flush(context);
      }
      finally
      {
        this._closed = true;
      }
    }

    public virtual bool closed => this._closed;

    public virtual int fileno(CodeContext context)
    {
      throw this.UnsupportedOperation(context, nameof (fileno));
    }

    public virtual void flush(CodeContext context) => this._checkClosed();

    public virtual bool isatty(CodeContext context)
    {
      this._checkClosed();
      return false;
    }

    [PythonHidden(new PlatformID[] {})]
    public virtual Bytes peek(CodeContext context, int length = 0)
    {
      this._checkClosed();
      throw this.AttributeError(nameof (peek));
    }

    [PythonHidden(new PlatformID[] {})]
    public virtual object read(CodeContext context, object length = null)
    {
      throw this.AttributeError(nameof (read));
    }

    [PythonHidden(new PlatformID[] {})]
    public virtual Bytes read1(CodeContext context, int length = 0)
    {
      throw this.AttributeError(nameof (read1));
    }

    public virtual bool readable(CodeContext context) => false;

    public virtual object readline(CodeContext context, int limit)
    {
      this._checkClosed();
      List<Bytes> list = new List<Bytes>();
      int length = 0;
      while (limit < 0 || list.Count < limit)
      {
        object o = this.read(context, (object) 1);
        if (o != null)
        {
          Bytes bytes = PythonIOModule.GetBytes(o, "read()");
          if (bytes.Count != 0)
          {
            list.Add(bytes);
            length += bytes.Count;
            if (bytes._bytes[bytes.Count - 1] == (byte) 10)
              break;
          }
          else
            break;
        }
        else
          break;
      }
      return (object) Bytes.Concat((IList<Bytes>) list, length);
    }

    public object readline(CodeContext context, object limit = null)
    {
      return this.readline(context, PythonIOModule.GetInt(limit, -1));
    }

    public virtual IronPython.Runtime.List readlines() => this.readlines((object) null);

    public virtual IronPython.Runtime.List readlines(object hint = null)
    {
      int num1 = PythonIOModule.GetInt(hint, -1);
      IronPython.Runtime.List list = new IronPython.Runtime.List();
      if (num1 <= 0)
      {
        foreach (object obj in this)
          list.AddNoLock(obj);
        return list;
      }
      int num2 = 0;
      foreach (object obj in this)
      {
        switch (obj)
        {
          case Bytes bytes:
            list.AddNoLock(obj);
            num2 += bytes.Count;
            if (num2 < num1)
              continue;
            goto label_19;
          case string str:
            list.AddNoLock(obj);
            num2 += str.Length;
            if (num2 < num1)
              continue;
            goto label_19;
          default:
            throw PythonOps.TypeError("next() should return string or bytes");
        }
      }
label_19:
      return list;
    }

    public virtual BigInteger seek(CodeContext context, BigInteger pos, object whence = 0)
    {
      throw this.UnsupportedOperation(context, nameof (seek));
    }

    public virtual bool seekable(CodeContext context) => false;

    public virtual BigInteger tell(CodeContext context)
    {
      return this.seek(context, (BigInteger) 0, (object) 1);
    }

    public virtual BigInteger truncate(CodeContext context, object pos = null)
    {
      throw this.UnsupportedOperation(context, nameof (truncate));
    }

    public virtual bool writable(CodeContext context) => false;

    [PythonHidden(new PlatformID[] {})]
    public virtual BigInteger write(CodeContext context, object buf)
    {
      throw this.AttributeError(nameof (write));
    }

    public virtual void writelines(CodeContext context, object lines)
    {
      this._checkClosed();
      IEnumerator enumerator = PythonOps.GetEnumerator(context, lines);
      while (enumerator.MoveNext())
        this.write(context, enumerator.Current);
    }

    ~_IOBase()
    {
      try
      {
        this.close(this.context);
      }
      catch
      {
      }
      finally
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
      }
    }

    object IEnumerator<object>.Current => this._current;

    object IEnumerator.Current => this._current;

    bool IEnumerator.MoveNext()
    {
      this._current = this.readline(this.context, -1);
      if (this._current == null)
        return false;
      if (this._current is Bytes current1)
        return current1.Count > 0;
      return this._current is string current2 ? current2.Length > 0 : PythonOps.IsTrue(this._current);
    }

    void IEnumerator.Reset()
    {
      this._current = (object) null;
      this.seek(this.context, (BigInteger) 0, (object) 0);
    }

    [PythonHidden(new PlatformID[] {})]
    public IEnumerator<object> GetEnumerator()
    {
      this._checkClosed();
      return (IEnumerator<object>) this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      this._checkClosed();
      return (IEnumerator) this;
    }

    void IDisposable.Dispose()
    {
    }

    WeakRefTracker IWeakReferenceable.GetWeakRef() => this._weakref;

    bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
    {
      this._weakref = value;
      return true;
    }

    void IWeakReferenceable.SetFinalizer(WeakRefTracker value)
    {
      ((IWeakReferenceable) this).SetWeakRef(value);
    }

    IDictionary<string, object> IPythonExpandable.EnsureCustomAttributes()
    {
      return this._customAttributes = (IDictionary<string, object>) new Dictionary<string, object>();
    }

    IDictionary<string, object> IPythonExpandable.CustomAttributes => this._customAttributes;

    CodeContext IPythonExpandable.Context => this.context;

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule._IOBase>(parameter, (IPythonExpandable) this);
    }

    internal Exception UnsupportedOperation(CodeContext context, string attr)
    {
      throw PythonExceptions.CreateThrowable((PythonType) context.LanguageContext.GetModuleState(PythonIOModule._unsupportedOperationKey), (object) $"{PythonTypeOps.GetName((object) this)}.{attr} not supported");
    }

    internal Exception AttributeError(string attrName)
    {
      throw PythonOps.AttributeError("'{0}' object has no attribute '{1}'", (object) PythonTypeOps.GetName((object) this), (object) attrName);
    }

    internal Exception InvalidPosition(BigInteger pos)
    {
      return PythonOps.IOError("Raw stream returned invalid position {0}", (object) pos);
    }
  }

  [PythonType]
  public class _RawIOBase(CodeContext context) : PythonIOModule._IOBase(context), IDynamicMetaObjectProvider
  {
    public override object read(CodeContext context, object size = null)
    {
      int capacity = PythonIOModule.GetInt(size, -1);
      if (capacity < 0)
        return (object) this.readall(context);
      ByteArray buf = new ByteArray(new List<byte>(capacity));
      int index = (int) this.readinto(context, (object) buf);
      List<byte> bytes = buf._bytes;
      if (index < bytes.Count)
        bytes.RemoveRange(index, bytes.Count - index);
      return (object) new Bytes((IList<byte>) bytes);
    }

    public Bytes readall(CodeContext context)
    {
      List<Bytes> list = new List<Bytes>();
      int length = 0;
      while (true)
      {
        object o = this.read(context, (object) 8192 /*0x2000*/);
        if (o != null)
        {
          Bytes bytes = PythonIOModule.GetBytes(o, "read()");
          if (bytes.Count != 0)
          {
            length += bytes.Count;
            list.Add(bytes);
          }
          else
            break;
        }
        else
          break;
      }
      return Bytes.Concat((IList<Bytes>) list, length);
    }

    public virtual BigInteger readinto(CodeContext context, object buf)
    {
      throw this.UnsupportedOperation(context, nameof (readinto));
    }

    public override BigInteger write(CodeContext context, object buf)
    {
      throw this.UnsupportedOperation(context, nameof (write));
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule._RawIOBase>(parameter, (IPythonExpandable) this);
    }
  }

  [PythonType]
  public class _BufferedIOBase(CodeContext context) : 
    PythonIOModule._IOBase(context),
    IDynamicMetaObjectProvider
  {
    public virtual object detach(CodeContext context)
    {
      throw this.UnsupportedOperation(context, nameof (detach));
    }

    public override object read(CodeContext context, object length = null)
    {
      throw this.UnsupportedOperation(context, nameof (read));
    }

    public virtual BigInteger readinto(CodeContext context, object buf)
    {
      int length = -1;
      if (PythonOps.HasAttr(context, buf, "__len__"))
        length = PythonOps.Length(buf);
      object o = this.read(context, (object) length);
      if (o == null)
        return BigInteger.Zero;
      Bytes bytes = PythonIOModule.GetBytes(o, "read()");
      if (buf is IList<byte> byteList)
      {
        for (int index = 0; index < bytes.Count; ++index)
          byteList[index] = bytes._bytes[index];
        GC.KeepAlive((object) this);
        return (BigInteger) bytes.Count;
      }
      object ret;
      if (PythonOps.TryGetBoundAttr(buf, "__setslice__", out ret))
      {
        PythonOps.CallWithContext(context, ret, (object) new IronPython.Runtime.Slice((object) bytes.Count), (object) bytes);
        GC.KeepAlive((object) this);
        return (BigInteger) bytes.Count;
      }
      if (!PythonOps.TryGetBoundAttr(buf, "__setitem__", out ret))
        throw PythonOps.TypeError("must be read-write buffer, not " + PythonTypeOps.GetName(buf));
      for (int index = 0; index < bytes.Count; ++index)
        PythonOps.CallWithContext(context, ret, (object) index, bytes[context, index]);
      GC.KeepAlive((object) this);
      return (BigInteger) bytes.Count;
    }

    public override BigInteger write(CodeContext context, object buf)
    {
      throw this.UnsupportedOperation(context, nameof (write));
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule._BufferedIOBase>(parameter, (IPythonExpandable) this);
    }
  }

  [PythonType]
  public class _TextIOBase(CodeContext context) : PythonIOModule._IOBase(context), IDynamicMetaObjectProvider
  {
    public virtual object detach(CodeContext context)
    {
      throw this.UnsupportedOperation(context, nameof (detach));
    }

    public virtual string encoding => (string) null;

    public virtual string errors => (string) null;

    public virtual object newlines => (object) null;

    public override object read(CodeContext context, object length = -1)
    {
      throw this.UnsupportedOperation(context, nameof (read));
    }

    public override object readline(CodeContext context, int limit = -1)
    {
      throw this.UnsupportedOperation(context, nameof (readline));
    }

    public override BigInteger truncate(CodeContext context, object pos = null)
    {
      throw this.UnsupportedOperation(context, nameof (truncate));
    }

    public override BigInteger write(CodeContext context, object str)
    {
      throw this.UnsupportedOperation(context, nameof (write));
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule._TextIOBase>(parameter, (IPythonExpandable) this);
    }
  }

  [PythonType]
  public class BufferedReader(CodeContext context, object raw, int buffer_size = 8192 /*0x2000*/) : 
    PythonIOModule._BufferedIOBase(context),
    IDynamicMetaObjectProvider
  {
    private PythonIOModule._IOBase _rawIO;
    private object _raw;
    private int _bufSize;
    private Bytes _readBuf;
    private int _readBufPos;

    internal static PythonIOModule.BufferedReader Create(
      CodeContext context,
      object raw,
      int buffer_size = 8192 /*0x2000*/)
    {
      PythonIOModule.BufferedReader bufferedReader = new PythonIOModule.BufferedReader(context, raw, buffer_size);
      bufferedReader.__init__(context, raw, buffer_size);
      return bufferedReader;
    }

    public void __init__(CodeContext context, object raw, int buffer_size = 8192 /*0x2000*/)
    {
      this.raw = raw;
      if (this._rawIO != null)
      {
        if (!this._rawIO.readable(context))
          throw PythonOps.IOError("\"raw\" argument must be readable.");
      }
      else if (PythonOps.Not(PythonOps.Invoke(context, this._raw, "readable")))
        throw PythonOps.IOError("\"raw\" argument must be readable.");
      this._bufSize = buffer_size > 0 ? buffer_size : throw PythonOps.ValueError("invalid buffer size (must be positive)");
      this._readBuf = Bytes.Empty;
    }

    public object raw
    {
      get => this._raw;
      set
      {
        this._rawIO = value as PythonIOModule._IOBase;
        this._raw = value;
      }
    }

    public override BigInteger truncate(CodeContext context, object pos = null)
    {
      if (this._rawIO != null)
        return this._rawIO.truncate(context, pos);
      return PythonIOModule.GetBigInt(PythonOps.Invoke(context, this._raw, nameof (truncate), pos), "truncate() should return integer");
    }

    public override void close(CodeContext context)
    {
      if (this.closed)
        return;
      try
      {
        this.flush(context);
      }
      finally
      {
        if (this._rawIO != null)
          this._rawIO.close(context);
        else
          PythonOps.Invoke(context, this._raw, nameof (close));
      }
    }

    public override object detach(CodeContext context)
    {
      if (this._raw == null)
        throw PythonOps.ValueError("raw stream already detached");
      this.flush(context);
      object raw = this._raw;
      this.raw = (object) null;
      return raw;
    }

    public override bool seekable(CodeContext context)
    {
      return this._rawIO != null ? this._rawIO.seekable(context) : PythonOps.IsTrue(PythonOps.Invoke(context, this._raw, nameof (seekable)));
    }

    public override bool readable(CodeContext context)
    {
      return this._rawIO != null ? this._rawIO.readable(context) : PythonOps.IsTrue(PythonOps.Invoke(context, this._raw, nameof (readable)));
    }

    public override bool writable(CodeContext context)
    {
      return this._rawIO != null ? this._rawIO.writable(context) : PythonOps.IsTrue(PythonOps.Invoke(context, this._raw, nameof (writable)));
    }

    public override bool closed
    {
      get
      {
        return this._rawIO != null ? this._rawIO.closed : PythonOps.IsTrue(PythonOps.GetBoundAttr(this.context, this._raw, nameof (closed)));
      }
    }

    public object name => PythonOps.GetBoundAttr(this.context, this._raw, nameof (name));

    public object mode => PythonOps.GetBoundAttr(this.context, this._raw, nameof (mode));

    public override int fileno(CodeContext context)
    {
      return this._rawIO != null ? this._rawIO.fileno(context) : PythonIOModule.GetInt(PythonOps.Invoke(context, this._raw, nameof (fileno)), "fileno() should return integer");
    }

    public override bool isatty(CodeContext context)
    {
      return this._rawIO != null ? this._rawIO.isatty(context) : PythonOps.IsTrue(PythonOps.Invoke(context, this._raw, nameof (isatty)));
    }

    public override object read(CodeContext context, object length = null)
    {
      int length1 = PythonIOModule.GetInt(length, -1);
      if (length1 < -1)
        throw PythonOps.ValueError("invalid number of bytes to read");
      lock (this)
        return (object) this.ReadNoLock(context, length1);
    }

    private Bytes ReadNoLock(CodeContext context, int length)
    {
      if (length == 0)
        return Bytes.Empty;
      if (length < 0)
      {
        List<Bytes> list = new List<Bytes>();
        int length1 = 0;
        if (this._readBuf.Count > 0)
        {
          list.Add(this.ResetReadBuf());
          length1 += list[0].Count;
        }
        Bytes bytes;
        while (true)
        {
          object o;
          if (this._rawIO != null)
            o = this._rawIO.read(context, (object) -1);
          else
            o = PythonOps.Invoke(context, this._raw, "read", (object) -1);
          bytes = PythonIOModule.GetBytes(o, "read()");
          if (bytes != null && bytes.Count != 0)
          {
            list.Add(bytes);
            length1 += bytes.Count;
          }
          else
            break;
        }
        if (length1 == 0)
          return bytes;
        GC.KeepAlive((object) this);
        return Bytes.Concat((IList<Bytes>) list, length1);
      }
      if (length < this._readBuf.Count - this._readBufPos)
      {
        byte[] numArray = new byte[length];
        Array.Copy((Array) this._readBuf._bytes, this._readBufPos, (Array) numArray, 0, length);
        this._readBufPos += length;
        if (this._readBufPos == this._readBuf.Count)
          this.ResetReadBuf();
        GC.KeepAlive((object) this);
        return Bytes.Make(numArray);
      }
      List<Bytes> list1 = new List<Bytes>();
      int length2 = length;
      if (this._readBuf.Count > 0)
      {
        list1.Add(this.ResetReadBuf());
        length2 -= list1[0].Count;
      }
      while (length2 > 0)
      {
        object o;
        if (this._rawIO != null)
          o = this._rawIO.read(context, (object) this._bufSize);
        else
          o = PythonOps.Invoke(context, this._raw, "read", (object) this._bufSize);
        this._readBuf = o != null ? PythonIOModule.GetBytes(o, "read()") : Bytes.Empty;
        if (this._readBuf.Count != 0)
        {
          if (length2 >= this._readBuf.Count - this._readBufPos)
          {
            length2 -= this._readBuf.Count - this._readBufPos;
            list1.Add(this.ResetReadBuf());
          }
          else
          {
            byte[] numArray = new byte[length2];
            Array.Copy((Array) this._readBuf._bytes, 0, (Array) numArray, 0, length2);
            list1.Add(Bytes.Make(numArray));
            this._readBufPos = length2;
            length2 = 0;
            break;
          }
        }
        else
          break;
      }
      GC.KeepAlive((object) this);
      return Bytes.Concat((IList<Bytes>) list1, length - length2);
    }

    public override Bytes peek(CodeContext context, int length = 0)
    {
      this._checkClosed();
      if (length <= 0 || length > this._bufSize)
        length = this._bufSize;
      lock (this)
        return this.PeekNoLock(context, length);
    }

    private Bytes PeekNoLock(CodeContext context, int length)
    {
      int num = this._readBuf.Count - this._readBufPos;
      byte[] numArray = new byte[length];
      if (length <= num)
      {
        Array.Copy((Array) this._readBuf._bytes, this._readBufPos, (Array) numArray, 0, length);
        return Bytes.Make(numArray);
      }
      object o;
      if (this._rawIO != null)
        o = this._rawIO.read(context, (object) (length - this._readBuf.Count + this._readBufPos));
      else
        o = PythonOps.Invoke(context, this._raw, "read", (object) (length - this._readBuf.Count + this._readBufPos));
      Bytes bytes = o != null ? PythonIOModule.GetBytes(o, "read()") : Bytes.Empty;
      this._readBuf = this.ResetReadBuf() + bytes;
      return this._readBuf;
    }

    public override Bytes read1(CodeContext context, int length = 0)
    {
      if (length == 0)
        return Bytes.Empty;
      if (length < 0)
        throw PythonOps.ValueError("number of bytes to read must be positive");
      lock (this)
      {
        this.PeekNoLock(context, 1);
        return this.ReadNoLock(context, Math.Min(length, this._readBuf.Count - this._readBufPos));
      }
    }

    public override BigInteger tell(CodeContext context)
    {
      BigInteger pos = this._rawIO != null ? this._rawIO.tell(context) : PythonIOModule.GetBigInt(PythonOps.Invoke(context, this._raw, nameof (tell)), "tell() should return integer");
      if (pos < 0L)
        throw this.InvalidPosition(pos);
      return pos - (BigInteger) this._readBuf.Count + (BigInteger) this._readBufPos;
    }

    public BigInteger seek(double offset, object whence = 0)
    {
      this._checkClosed();
      throw PythonOps.TypeError("an integer is required");
    }

    public override BigInteger seek(CodeContext context, BigInteger pos, object whence = 0)
    {
      int whence1 = PythonIOModule.GetInt(whence);
      if (whence1 < 0 || whence1 > 2)
        throw PythonOps.ValueError("invalid whence ({0}, should be 0, 1, or 2)", (object) whence1);
      lock (this)
      {
        if (whence1 == 1)
          pos -= (BigInteger) (this._readBuf.Count - this._readBufPos);
        object i;
        if (this._rawIO != null)
          i = (object) this._rawIO.seek(context, pos, (object) whence1);
        else
          i = PythonOps.Invoke(context, this._raw, nameof (seek), (object) whence1);
        pos = PythonIOModule.GetBigInt(i, "seek() should return integer");
        this.ResetReadBuf();
        if (pos < 0L)
          throw this.InvalidPosition(pos);
        GC.KeepAlive((object) this);
        return pos;
      }
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule.BufferedReader>(parameter, (IPythonExpandable) this);
    }

    private Bytes ResetReadBuf()
    {
      Bytes bytes;
      if (this._readBufPos == 0)
      {
        bytes = this._readBuf;
      }
      else
      {
        byte[] numArray = new byte[this._readBuf.Count - this._readBufPos];
        Array.Copy((Array) this._readBuf._bytes, this._readBufPos, (Array) numArray, 0, numArray.Length);
        bytes = Bytes.Make(numArray);
        this._readBufPos = 0;
      }
      this._readBuf = Bytes.Empty;
      return bytes;
    }
  }

  [PythonType]
  public class BufferedWriter(
    CodeContext context,
    object raw,
    int buffer_size = 8192 /*0x2000*/,
    object max_buffer_size = null) : PythonIOModule._BufferedIOBase(context), IDynamicMetaObjectProvider
  {
    private PythonIOModule._IOBase _rawIO;
    private object _raw;
    private int _bufSize;
    private List<byte> _writeBuf;

    internal static PythonIOModule.BufferedWriter Create(
      CodeContext context,
      object raw,
      int buffer_size = 8192 /*0x2000*/,
      object max_buffer_size = null)
    {
      PythonIOModule.BufferedWriter bufferedWriter = new PythonIOModule.BufferedWriter(context, raw, buffer_size, max_buffer_size);
      bufferedWriter.__init__(context, raw, buffer_size, max_buffer_size);
      return bufferedWriter;
    }

    public void __init__(CodeContext context, object raw, int buffer_size = 8192 /*0x2000*/, object max_buffer_size = null)
    {
      if (max_buffer_size != null)
        PythonOps.Warn(context, PythonExceptions.DeprecationWarning, "max_buffer_size is deprecated");
      this.raw = raw;
      if (this._rawIO != null)
      {
        if (!this._rawIO.writable(context))
          throw PythonOps.IOError("\"raw\" argument must be writable.");
      }
      else if (PythonOps.Not(PythonOps.Invoke(context, this._raw, "writable")))
        throw PythonOps.IOError("\"raw\" argument must be writable.");
      this._bufSize = buffer_size > 0 ? buffer_size : throw PythonOps.ValueError("invalid buffer size (must be positive)");
      this._writeBuf = new List<byte>();
    }

    public object raw
    {
      get => this._raw;
      set
      {
        this._rawIO = value as PythonIOModule._IOBase;
        this._raw = value;
      }
    }

    public override void close(CodeContext context)
    {
      if (this.closed)
        return;
      try
      {
        this.flush(context);
      }
      finally
      {
        if (this._rawIO != null)
          this._rawIO.close(context);
        else
          PythonOps.Invoke(context, this._raw, nameof (close));
      }
    }

    public override object detach(CodeContext context)
    {
      if (this._raw == null)
        throw PythonOps.ValueError("raw stream already detached");
      this.flush(context);
      object raw = this._raw;
      this.raw = (object) null;
      return raw;
    }

    public override bool seekable(CodeContext context)
    {
      if (this._rawIO == null)
        return PythonOps.IsTrue(PythonOps.Invoke(context, this._raw, nameof (seekable)));
      int num = this._rawIO.seekable(context) ? 1 : 0;
      GC.KeepAlive((object) this);
      return num != 0;
    }

    public override bool readable(CodeContext context)
    {
      if (this._rawIO == null)
        return PythonOps.IsTrue(PythonOps.Invoke(context, this._raw, nameof (readable)));
      int num = this._rawIO.readable(context) ? 1 : 0;
      GC.KeepAlive((object) this);
      return num != 0;
    }

    public override bool writable(CodeContext context)
    {
      if (this._rawIO == null)
        return PythonOps.IsTrue(PythonOps.Invoke(context, this._raw, nameof (writable)));
      int num = this._rawIO.writable(context) ? 1 : 0;
      GC.KeepAlive((object) this);
      return num != 0;
    }

    public override bool closed
    {
      get
      {
        return this._rawIO != null ? this._rawIO.closed : PythonOps.IsTrue(PythonOps.GetBoundAttr(this.context, this._raw, nameof (closed)));
      }
    }

    public object name => PythonOps.GetBoundAttr(this.context, this._raw, nameof (name));

    public object mode => PythonOps.GetBoundAttr(this.context, this._raw, nameof (mode));

    public override int fileno(CodeContext context)
    {
      if (this._rawIO == null)
        return PythonIOModule.GetInt(PythonOps.Invoke(context, this._raw, nameof (fileno)), "fileno() should return integer");
      int num = this._rawIO.fileno(context);
      GC.KeepAlive((object) this);
      return num;
    }

    public override bool isatty(CodeContext context)
    {
      if (this._rawIO == null)
        return PythonOps.IsTrue(PythonOps.Invoke(context, this._raw, nameof (isatty)));
      int num = this._rawIO.isatty(context) ? 1 : 0;
      GC.KeepAlive((object) this);
      return num != 0;
    }

    public override BigInteger write(CodeContext context, object buf)
    {
      this._checkClosed("write to closed file");
      IList<byte> bytes = PythonIOModule.GetBytes(buf);
      lock (this)
      {
        if (this._writeBuf.Count > this._bufSize)
          this.FlushNoLock(context);
        int count1 = this._writeBuf.Count;
        this._writeBuf.AddRange((IEnumerable<byte>) bytes);
        int num1 = this._writeBuf.Count - count1;
        if (this._writeBuf.Count > this._bufSize)
        {
          try
          {
            this.FlushNoLock(context);
          }
          catch (PythonIOModule._BlockingIOErrorException ex)
          {
            if (this._writeBuf.Count > this._bufSize)
            {
              int count2 = this._writeBuf.Count - this._bufSize;
              int num2 = num1 - count2;
              this._writeBuf.RemoveRange(this._bufSize, count2);
            }
            throw;
          }
        }
        return (BigInteger) num1;
      }
    }

    public override BigInteger truncate(CodeContext context, object pos = null)
    {
      lock (this)
      {
        this.FlushNoLock(context);
        if (pos == null)
          pos = this._rawIO == null ? (object) PythonIOModule.GetBigInt(PythonOps.Invoke(context, this._raw, "tell"), "tell() should return integer") : (object) this._rawIO.tell(context);
        if (this._rawIO != null)
          return this._rawIO.truncate(context, pos);
        BigInteger bigInt = PythonIOModule.GetBigInt(PythonOps.Invoke(context, this._raw, nameof (truncate), pos), "truncate() should return integer");
        GC.KeepAlive((object) this);
        return bigInt;
      }
    }

    public override void flush(CodeContext context)
    {
      lock (this)
        this.FlushNoLock(context);
    }

    private void FlushNoLock(CodeContext context)
    {
      this._checkClosed("flush of closed file");
      int num1 = 0;
      try
      {
        while (this._writeBuf.Count > 0)
        {
          object i;
          if (this._rawIO != null)
            i = (object) this._rawIO.write(context, (object) this._writeBuf);
          else
            i = PythonOps.Invoke(context, this._raw, "write", (object) this._writeBuf);
          int count = PythonIOModule.GetInt(i, "write() should return integer");
          if (count > this._writeBuf.Count || count < 0)
            throw PythonOps.IOError("write() returned incorrect number of bytes");
          this._writeBuf.RemoveRange(0, count);
          num1 += count;
        }
      }
      catch (PythonIOModule._BlockingIOErrorException ex)
      {
        object i;
        ref object local = ref i;
        int count;
        if (!PythonOps.TryGetBoundAttr((object) ex, "characters_written", out local) || !PythonIOModule.TryGetInt(i, out count))
          throw;
        this._writeBuf.RemoveRange(0, count);
        int num2 = num1 + count;
        throw;
      }
    }

    public override BigInteger tell(CodeContext context)
    {
      BigInteger pos = this._rawIO != null ? this._rawIO.tell(context) : PythonIOModule.GetBigInt(PythonOps.Invoke(context, this._raw, nameof (tell)), "tell() should return integer");
      if (pos < 0L)
        throw this.InvalidPosition(pos);
      GC.KeepAlive((object) this);
      return pos + (BigInteger) this._writeBuf.Count;
    }

    public BigInteger seek(double offset, object whence = 0)
    {
      this._checkClosed();
      throw PythonOps.TypeError("an integer is required");
    }

    public override BigInteger seek(CodeContext context, BigInteger pos, object whence = 0)
    {
      int whence1 = PythonIOModule.GetInt(whence);
      if (whence1 < 0 || whence1 > 2)
        throw PythonOps.ValueError("invalid whence ({0}, should be 0, 1, or 2)", (object) whence1);
      lock (this)
      {
        this.FlushNoLock(context);
        BigInteger bigInteger;
        if (this._rawIO == null)
          bigInteger = PythonIOModule.GetBigInt(PythonOps.Invoke(context, this._raw, nameof (seek), (object) pos, (object) whence1), "seek() should return integer");
        else
          bigInteger = this._rawIO.seek(context, pos, (object) whence1);
        if (bigInteger < 0L)
          throw this.InvalidPosition(pos);
        GC.KeepAlive((object) this);
        return bigInteger;
      }
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule.BufferedWriter>(parameter, (IPythonExpandable) this);
    }
  }

  [PythonType]
  public class BufferedRandom(
    CodeContext context,
    PythonIOModule._IOBase raw,
    int buffer_size = 8192 /*0x2000*/,
    object max_buffer_size = null) : PythonIOModule._BufferedIOBase(context), IDynamicMetaObjectProvider
  {
    private PythonIOModule._IOBase _inner;
    private int _bufSize;
    private Bytes _readBuf;
    private int _readBufPos;
    private List<byte> _writeBuf;

    internal static PythonIOModule.BufferedRandom Create(
      CodeContext context,
      PythonIOModule._IOBase raw,
      int buffer_size = 8192 /*0x2000*/,
      object max_buffer_size = null)
    {
      PythonIOModule.BufferedRandom bufferedRandom = new PythonIOModule.BufferedRandom(context, raw, buffer_size, max_buffer_size);
      bufferedRandom.__init__(context, raw, buffer_size, max_buffer_size);
      return bufferedRandom;
    }

    public void __init__(
      CodeContext context,
      PythonIOModule._IOBase raw,
      int buffer_size = 8192 /*0x2000*/,
      object max_buffer_size = null)
    {
      if (max_buffer_size != null)
        PythonOps.Warn(context, PythonExceptions.DeprecationWarning, "max_buffer_size is deprecated");
      raw._checkSeekable();
      if (buffer_size <= 0)
        throw PythonOps.ValueError("invalid buffer size (must be positive)");
      if (!raw.readable(context))
        throw PythonOps.IOError("\"raw\" argument must be readable.");
      if (!raw.writable(context))
        throw PythonOps.IOError("\"raw\" argument must be writable.");
      this._bufSize = buffer_size;
      this._inner = raw;
      this._readBuf = Bytes.Empty;
      this._writeBuf = new List<byte>();
    }

    public PythonIOModule._IOBase raw
    {
      get => this._inner;
      set => this._inner = value;
    }

    public override void close(CodeContext context)
    {
      if (this.closed)
        return;
      try
      {
        this.flush(context);
      }
      finally
      {
        this._inner.close(context);
      }
    }

    public override object detach(CodeContext context)
    {
      if (this._inner == null)
        throw PythonOps.ValueError("raw stream already detached");
      this.flush(context);
      PythonIOModule._IOBase inner = this._inner;
      this._inner = (PythonIOModule._IOBase) null;
      return (object) inner;
    }

    public override bool seekable(CodeContext context)
    {
      int num = this._inner.seekable(context) ? 1 : 0;
      GC.KeepAlive((object) this);
      return num != 0;
    }

    public override bool readable(CodeContext context)
    {
      int num = this._inner.readable(context) ? 1 : 0;
      GC.KeepAlive((object) this);
      return num != 0;
    }

    public override bool writable(CodeContext context)
    {
      int num = this._inner.writable(context) ? 1 : 0;
      GC.KeepAlive((object) this);
      return num != 0;
    }

    public override bool closed => this._inner.closed;

    public object name => PythonOps.GetBoundAttr(this.context, (object) this._inner, nameof (name));

    public object mode => PythonOps.GetBoundAttr(this.context, (object) this._inner, nameof (mode));

    public override int fileno(CodeContext context)
    {
      int num = this._inner.fileno(context);
      GC.KeepAlive((object) this);
      return num;
    }

    public override bool isatty(CodeContext context) => this._inner.isatty(context);

    public override object read(CodeContext context, object length = null)
    {
      this.flush(context);
      int length1 = PythonIOModule.GetInt(length, -1);
      if (length1 < -1)
        throw PythonOps.ValueError("invalid number of bytes to read");
      lock (this)
        return (object) this.ReadNoLock(context, length1);
    }

    private Bytes ReadNoLock(CodeContext context, int length)
    {
      if (length == 0)
        return Bytes.Empty;
      if (length < 0)
      {
        List<Bytes> list = new List<Bytes>();
        int length1 = 0;
        if (this._readBuf.Count > 0)
        {
          list.Add(this.ResetReadBuf());
          length1 += list[0].Count;
        }
        Bytes bytes;
        while (true)
        {
          bytes = (Bytes) this._inner.read(context, (object) -1);
          if (bytes != null && bytes.Count != 0)
          {
            list.Add(bytes);
            length1 += bytes.Count;
          }
          else
            break;
        }
        if (length1 == 0)
          return bytes;
        GC.KeepAlive((object) this);
        return Bytes.Concat((IList<Bytes>) list, length1);
      }
      if (length < this._readBuf.Count - this._readBufPos)
      {
        byte[] numArray = new byte[length];
        Array.Copy((Array) this._readBuf._bytes, this._readBufPos, (Array) numArray, 0, length);
        this._readBufPos += length;
        if (this._readBufPos == this._readBuf.Count)
          this.ResetReadBuf();
        GC.KeepAlive((object) this);
        return Bytes.Make(numArray);
      }
      List<Bytes> list1 = new List<Bytes>();
      int length2 = length;
      if (this._readBuf.Count > 0)
      {
        list1.Add(this.ResetReadBuf());
        length2 -= list1[0].Count;
      }
      while (length2 > 0)
      {
        this._readBuf = (Bytes) this._inner.read(context, (object) this._bufSize) ?? Bytes.Empty;
        if (this._readBuf.Count != 0)
        {
          if (length2 >= this._readBuf.Count - this._readBufPos)
          {
            length2 -= this._readBuf.Count - this._readBufPos;
            list1.Add(this.ResetReadBuf());
          }
          else
          {
            byte[] numArray = new byte[length2];
            Array.Copy((Array) this._readBuf._bytes, 0, (Array) numArray, 0, length2);
            list1.Add(Bytes.Make(numArray));
            this._readBufPos = length2;
            length2 = 0;
            break;
          }
        }
        else
          break;
      }
      GC.KeepAlive((object) this);
      return Bytes.Concat((IList<Bytes>) list1, length - length2);
    }

    public override Bytes peek(CodeContext context, int length = 0)
    {
      this._checkClosed();
      this.flush(context);
      if (length <= 0 || length > this._bufSize)
        length = this._bufSize;
      lock (this)
        return this.PeekNoLock(context, length);
    }

    private Bytes PeekNoLock(CodeContext context, int length)
    {
      int num = this._readBuf.Count - this._readBufPos;
      byte[] numArray = new byte[length];
      if (length <= num)
      {
        Array.Copy((Array) this._readBuf._bytes, this._readBufPos, (Array) numArray, 0, length);
        return Bytes.Make(numArray);
      }
      Bytes bytes = (Bytes) this._inner.read(context, (object) (length - this._readBuf.Count + this._readBufPos)) ?? Bytes.Empty;
      this._readBuf = this.ResetReadBuf() + bytes;
      GC.KeepAlive((object) this);
      return this._readBuf;
    }

    public override Bytes read1(CodeContext context, int length = 0)
    {
      this.flush(context);
      if (length == 0)
        return Bytes.Empty;
      if (length < 0)
        throw PythonOps.ValueError("number of bytes to read must be positive");
      lock (this)
      {
        this.PeekNoLock(context, 1);
        return this.ReadNoLock(context, Math.Min(length, this._readBuf.Count - this._readBufPos));
      }
    }

    private Bytes ResetReadBuf()
    {
      Bytes bytes;
      if (this._readBufPos == 0)
      {
        bytes = this._readBuf;
      }
      else
      {
        byte[] numArray = new byte[this._readBuf.Count - this._readBufPos];
        Array.Copy((Array) this._readBuf._bytes, this._readBufPos, (Array) numArray, 0, numArray.Length);
        bytes = Bytes.Make(numArray);
        this._readBufPos = 0;
      }
      this._readBuf = Bytes.Empty;
      return bytes;
    }

    public override BigInteger write(CodeContext context, object buf)
    {
      this._checkClosed("write to closed file");
      if (this._readBuf.Count > 0)
      {
        lock (this)
        {
          this._inner.seek(context, (BigInteger) (this._readBufPos - this._readBuf.Count), (object) 1);
          this.ResetReadBuf();
        }
      }
      IList<byte> bytes = PythonIOModule.GetBytes(buf);
      lock (this)
      {
        if (this._writeBuf.Count > this._bufSize)
          this.FlushNoLock(context);
        int count1 = this._writeBuf.Count;
        this._writeBuf.AddRange((IEnumerable<byte>) bytes);
        int num1 = this._writeBuf.Count - count1;
        if (this._writeBuf.Count > this._bufSize)
        {
          try
          {
            this.FlushNoLock(context);
          }
          catch (PythonIOModule._BlockingIOErrorException ex)
          {
            if (this._writeBuf.Count > this._bufSize)
            {
              int count2 = this._writeBuf.Count - this._bufSize;
              int num2 = num1 - count2;
              this._writeBuf.RemoveRange(this._bufSize, count2);
            }
            throw;
          }
        }
        return (BigInteger) num1;
      }
    }

    public override void flush(CodeContext context)
    {
      lock (this)
        this.FlushNoLock(context);
    }

    private void FlushNoLock(CodeContext context)
    {
      this._checkClosed("flush of closed file");
      int num1 = 0;
      try
      {
        while (this._writeBuf.Count > 0)
        {
          int count = (int) this._inner.write(context, (object) this._writeBuf);
          if (count > this._writeBuf.Count || count < 0)
            throw PythonOps.IOError("write() returned incorrect number of bytes");
          this._writeBuf.RemoveRange(0, count);
          num1 += count;
        }
      }
      catch (PythonIOModule._BlockingIOErrorException ex)
      {
        object i;
        ref object local = ref i;
        int count;
        if (!PythonOps.TryGetBoundAttr((object) ex, "characters_written", out local) || !PythonIOModule.TryGetInt(i, out count))
          throw;
        this._writeBuf.RemoveRange(0, count);
        int num2 = num1 + count;
        throw;
      }
    }

    public override BigInteger readinto(CodeContext context, object buf)
    {
      this.flush(context);
      return base.readinto(context, buf);
    }

    public BigInteger seek(double offset, object whence = 0)
    {
      this._checkClosed();
      throw PythonOps.TypeError("an integer is required");
    }

    public override BigInteger seek(CodeContext context, BigInteger pos, object whence = 0)
    {
      int num = PythonIOModule.GetInt(whence);
      if (num < 0 || num > 2)
        throw PythonOps.ValueError("invalid whence ({0}, should be 0, 1, or 2)", (object) num);
      lock (this)
      {
        this.FlushNoLock(context);
        if (this._readBuf.Count > 0)
          this._inner.seek(context, (BigInteger) (this._readBufPos - this._readBuf.Count), (object) 1);
        pos = this._inner.seek(context, pos, whence);
        this.ResetReadBuf();
        if (pos < 0L)
          throw PythonOps.IOError("seek() returned invalid position");
        GC.KeepAlive((object) this);
        return pos;
      }
    }

    public override BigInteger truncate(CodeContext context, object pos = null)
    {
      lock (this)
      {
        this.FlushNoLock(context);
        if (pos == null)
          pos = (object) this.tell(context);
        BigInteger bigInteger = this._inner.truncate(context, pos);
        GC.KeepAlive((object) this);
        return bigInteger;
      }
    }

    public override BigInteger tell(CodeContext context)
    {
      BigInteger pos = this._inner.tell(context);
      if (pos < 0L)
        throw this.InvalidPosition(pos);
      return this._writeBuf.Count > 0 ? pos + (BigInteger) this._writeBuf.Count : pos - (BigInteger) this._readBuf.Count + (BigInteger) this._readBufPos;
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule.BufferedRandom>(parameter, (IPythonExpandable) this);
    }
  }

  [PythonType]
  public class BufferedRWPair(
    CodeContext context,
    object reader,
    object writer,
    int buffer_size = 8192 /*0x2000*/,
    object max_buffer_size = null) : PythonIOModule._BufferedIOBase(context), IDynamicMetaObjectProvider
  {
    private PythonIOModule.BufferedReader _reader;
    private PythonIOModule.BufferedWriter _writer;

    public void __init__(
      CodeContext context,
      object reader,
      object writer,
      int buffer_size = 8192 /*0x2000*/,
      object max_buffer_size = null)
    {
      if (max_buffer_size != null)
        PythonOps.Warn(context, PythonExceptions.DeprecationWarning, "max_buffer_size is deprecated");
      this.reader = reader;
      this.writer = writer;
      if (!this._reader.readable(context))
        throw PythonOps.IOError("\"reader\" object must be readable.");
      if (!this._writer.writable(context))
        throw PythonOps.IOError("\"writer\" object must be writable.");
    }

    public object reader
    {
      get => (object) this._reader;
      set
      {
        if (!(value is PythonIOModule.BufferedReader bufferedReader))
          bufferedReader = PythonIOModule.BufferedReader.Create(this.context, value, 8192 /*0x2000*/);
        this._reader = bufferedReader;
      }
    }

    public object writer
    {
      get => (object) this._writer;
      set
      {
        if (!(value is PythonIOModule.BufferedWriter bufferedWriter))
          bufferedWriter = PythonIOModule.BufferedWriter.Create(this.context, value, 8192 /*0x2000*/, (object) null);
        this._writer = bufferedWriter;
      }
    }

    public override object read(CodeContext context, object length = null)
    {
      object obj = this._reader.read(context, length);
      GC.KeepAlive((object) this);
      return obj;
    }

    public override BigInteger readinto(CodeContext context, object buf)
    {
      BigInteger bigInteger = this._reader.readinto(context, buf);
      GC.KeepAlive((object) this);
      return bigInteger;
    }

    public override BigInteger write(CodeContext context, object buf)
    {
      BigInteger bigInteger = this._writer.write(context, buf);
      GC.KeepAlive((object) this);
      return bigInteger;
    }

    public override Bytes peek(CodeContext context, int length = 0)
    {
      Bytes bytes = this._reader.peek(context, length);
      GC.KeepAlive((object) this);
      return bytes;
    }

    public override Bytes read1(CodeContext context, int length)
    {
      Bytes bytes = this._reader.read1(context, length);
      GC.KeepAlive((object) this);
      return bytes;
    }

    public override bool readable(CodeContext context)
    {
      int num = this._reader.readable(context) ? 1 : 0;
      GC.KeepAlive((object) this);
      return num != 0;
    }

    public override bool writable(CodeContext context)
    {
      int num = this._writer.writable(context) ? 1 : 0;
      GC.KeepAlive((object) this);
      return num != 0;
    }

    public override void flush(CodeContext context)
    {
      this._writer.flush(context);
      GC.KeepAlive((object) this);
    }

    public override void close(CodeContext context)
    {
      try
      {
        this._writer.close(context);
      }
      finally
      {
        this._reader.close(context);
      }
      GC.KeepAlive((object) this);
    }

    public override bool isatty(CodeContext context)
    {
      int num = this._reader.isatty(context) ? 1 : (this._writer.isatty(context) ? 1 : 0);
      GC.KeepAlive((object) this);
      return num != 0;
    }

    public override bool closed => this._writer.closed;

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule.BufferedRWPair>(parameter, (IPythonExpandable) this);
    }
  }

  [PythonType]
  public class StringIO(CodeContext context, string initial_value = "", string newline = "\n") : 
    PythonIOModule.TextIOWrapper(context),
    IDynamicMetaObjectProvider
  {
    public void __init__(CodeContext context, string initial_value = "", string newline = "\n")
    {
      PythonIOModule.BytesIO buffer = new PythonIOModule.BytesIO(context);
      buffer.__init__((object) null);
      this.__init__(context, (object) buffer, "utf-8", (string) null, newline, false);
      if (newline == null)
        this._writeTranslate = false;
      if (string.IsNullOrEmpty(initial_value))
        return;
      this.write(context, (object) initial_value);
      this.seek(context, (BigInteger) 0, (object) 0);
    }

    public override object detach(CodeContext context)
    {
      throw this.UnsupportedOperation(context, nameof (detach));
    }

    public string getvalue(CodeContext context)
    {
      this.flush(context);
      return ((PythonIOModule.BytesIO) this._bufferTyped).getvalue().decode(context, (object) this._encoding, this._errors);
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule.StringIO>(parameter, (IPythonExpandable) this);
    }
  }

  [PythonType]
  public class TextIOWrapper : 
    PythonIOModule._TextIOBase,
    IEnumerator<object>,
    IDisposable,
    IEnumerator,
    IEnumerable<object>,
    IEnumerable,
    ICodeFormattable,
    IDynamicMetaObjectProvider
  {
    public int _CHUNK_SIZE = 128 /*0x80*/;
    internal PythonIOModule._BufferedIOBase _bufferTyped;
    private object _buffer;
    internal string _encoding;
    internal string _errors;
    private bool _seekable;
    private bool _telling;
    private object _encoder;
    private object _decoder;
    private bool _line_buffering;
    private bool _readUniversal;
    private bool _readTranslate;
    internal bool _writeTranslate;
    private string _readNL;
    private string _writeNL;
    private int _decodedCharsUsed;
    private string _decodedChars;
    private Bytes _nextInput;
    private int _decodeFlags;
    private object _current;

    internal TextIOWrapper(CodeContext context)
      : base(context)
    {
    }

    internal static PythonIOModule.TextIOWrapper Create(
      CodeContext context,
      object buffer,
      string encoding = null,
      string errors = null,
      string newline = null,
      bool line_buffering = false)
    {
      PythonIOModule.TextIOWrapper textIoWrapper = new PythonIOModule.TextIOWrapper(context, buffer, encoding, errors, newline, line_buffering);
      textIoWrapper.__init__(context, buffer, encoding, errors, newline, line_buffering);
      return textIoWrapper;
    }

    public TextIOWrapper(
      CodeContext context,
      object buffer,
      string encoding = null,
      string errors = null,
      string newline = null,
      bool line_buffering = false)
      : base(context)
    {
    }

    public void __init__(
      CodeContext context,
      object buffer,
      string encoding = null,
      string errors = null,
      string newline = null,
      bool line_buffering = false)
    {
      if (newline != null && (newline == null || newline.Length != 0) && !(newline == "\n") && !(newline == "\r") && !(newline == "\r\n"))
        throw PythonOps.ValueError(string.Format("illegal newline value: " + newline));
      if (encoding == null)
      {
        encoding = PythonLocale.PreferredEncoding;
        if (encoding == "")
          encoding = "ascii";
      }
      if (errors == null)
        errors = "strict";
      this._bufferTyped = buffer as PythonIOModule._BufferedIOBase;
      this._buffer = buffer;
      this._encoding = encoding;
      this._errors = errors;
      this._seekable = this._telling = this._bufferTyped != null ? this._bufferTyped.seekable(context) : PythonOps.IsTrue(PythonOps.Invoke(context, this._buffer, "seekable"));
      this._line_buffering = line_buffering;
      this._readUniversal = string.IsNullOrEmpty(newline);
      this._readTranslate = newline == null;
      this._readNL = newline;
      this._writeTranslate = newline != "";
      this._writeNL = string.IsNullOrEmpty(newline) ? Environment.NewLine : newline;
      this._decodedChars = "";
      this._decodedCharsUsed = 0;
    }

    public object buffer => this._buffer;

    public override string encoding => this._encoding;

    public override string errors => this._errors;

    public bool line_buffering => this._line_buffering;

    public override object newlines
    {
      get
      {
        if (!this._readUniversal || this._decoder == null)
          return (object) null;
        return this._decoder is PythonIOModule.IncrementalNewlineDecoder decoder ? decoder.newlines : PythonOps.GetBoundAttr(this.context, this._decoder, nameof (newlines));
      }
    }

    public override bool seekable(CodeContext context) => this._seekable;

    public override bool readable(CodeContext context)
    {
      return this._bufferTyped == null ? PythonOps.IsTrue(PythonOps.Invoke(context, this._buffer, nameof (readable))) : this._bufferTyped.readable(context);
    }

    public override bool writable(CodeContext context)
    {
      return this._bufferTyped == null ? PythonOps.IsTrue(PythonOps.Invoke(context, this._buffer, nameof (writable))) : this._bufferTyped.writable(context);
    }

    public override void flush(CodeContext context)
    {
      if (this._bufferTyped != null)
        this._bufferTyped.flush(context);
      else
        PythonOps.Invoke(context, this._buffer, nameof (flush));
      this._telling = this._seekable;
    }

    public override void close(CodeContext context)
    {
      if (this.closed)
        return;
      try
      {
        this.flush(context);
      }
      finally
      {
        if (this._bufferTyped != null)
          this._bufferTyped.close(context);
        else
          PythonOps.Invoke(context, this._buffer, nameof (close));
      }
    }

    public override bool closed
    {
      get
      {
        return this._bufferTyped == null ? PythonOps.IsTrue(PythonOps.GetBoundAttr(this.context, this._buffer, nameof (closed))) : this._bufferTyped.closed;
      }
    }

    public object name => PythonOps.GetBoundAttr(this.context, this._buffer, nameof (name));

    public override int fileno(CodeContext context)
    {
      return this._bufferTyped == null ? PythonIOModule.GetInt(PythonOps.Invoke(context, this._buffer, nameof (fileno)), "fileno() should return an int") : this._bufferTyped.fileno(context);
    }

    public override bool isatty(CodeContext context)
    {
      return this._bufferTyped == null ? PythonOps.IsTrue(PythonOps.Invoke(context, this._buffer, nameof (isatty))) : this._bufferTyped.isatty(context);
    }

    public override BigInteger write(CodeContext context, object s)
    {
      switch (s)
      {
        case string s1:
label_3:
          if (this.closed)
            throw PythonOps.ValueError("write to closed file");
          int length = s1.Length;
          bool flag = (this._writeTranslate || this._line_buffering) && s1.Contains("\n");
          if (flag && this._writeTranslate && this._writeNL != "\n")
            s1 = s1.Replace("\n", this._writeNL);
          string buf = StringOps.encode(context, s1, (object) this._encoding, this._errors);
          if (this._bufferTyped != null)
            this._bufferTyped.write(context, (object) buf);
          else
            PythonOps.Invoke(context, this._buffer, nameof (write), (object) buf);
          if (this._line_buffering && (flag || buf.Contains("\r")))
            this.flush(context);
          this._nextInput = (Bytes) null;
          if (this._decoder != null)
            PythonOps.Invoke(context, this._decoder, "reset");
          GC.KeepAlive((object) this);
          return (BigInteger) length;
        case Extensible<string> extensible:
          s1 = extensible.Value;
          goto label_3;
        default:
          throw PythonOps.TypeError("must be unicode, not {0}", (object) PythonTypeOps.GetName(s));
      }
    }

    public override BigInteger tell(CodeContext context)
    {
      if (!this._seekable)
        throw PythonOps.IOError("underlying stream is not seekable");
      if (!this._telling)
        throw PythonOps.IOError("telling position disabled by next() call");
      this.flush(context);
      BigInteger pos = this._bufferTyped != null ? this._bufferTyped.tell(context) : PythonIOModule.GetBigInt(PythonOps.Invoke(context, this._buffer, nameof (tell)), "tell() should return an integer");
      if (pos < 0L)
        throw this.InvalidPosition(pos);
      object decoder = this._decoder;
      if (decoder == null || this._nextInput == null)
      {
        if (!string.IsNullOrEmpty(this._decodedChars))
          throw PythonOps.AssertionError("pending decoded text");
        return pos;
      }
      PythonIOModule.IncrementalNewlineDecoder incrementalNewlineDecoder = decoder as PythonIOModule.IncrementalNewlineDecoder;
      BigInteger bigInteger1 = pos - (BigInteger) this._nextInput.Count;
      int decodedCharsUsed = this._decodedCharsUsed;
      if (decodedCharsUsed == 0)
        return bigInteger1;
      PythonTuple state = incrementalNewlineDecoder == null ? (PythonTuple) PythonOps.Invoke(context, decoder, "getstate") : incrementalNewlineDecoder.getstate(context);
      try
      {
        if (incrementalNewlineDecoder != null)
          incrementalNewlineDecoder.SetState(context, Bytes.Empty, this._decodeFlags);
        else
          PythonOps.Invoke(context, decoder, "setstate", (object) PythonTuple.MakeTuple((object) Bytes.Empty, (object) this._decodeFlags));
        BigInteger bigInteger2 = bigInteger1;
        int num1 = 0;
        int num2 = 0;
        foreach (byte num3 in this._nextInput._bytes)
        {
          Bytes input = new Bytes((IList<byte>) new byte[1]
          {
            num3
          });
          ++num1;
          if (incrementalNewlineDecoder != null)
            num2 += incrementalNewlineDecoder.decode(context, (IList<byte>) input, false).Length;
          else
            num2 += ((string) PythonOps.Invoke(context, decoder, "decode", (object) input)).Length;
          Bytes buf;
          if (incrementalNewlineDecoder != null)
          {
            incrementalNewlineDecoder.GetState(context, out buf, out this._decodeFlags);
          }
          else
          {
            PythonTuple pythonTuple = (PythonTuple) PythonOps.Invoke(context, decoder, "getstate");
            buf = PythonIOModule.GetBytes(pythonTuple[0], "getstate");
            this._decodeFlags = Converter.ConvertToInt32(pythonTuple[1]);
          }
          if ((buf == null || buf.Count == 0) && num2 <= decodedCharsUsed)
          {
            bigInteger2 += (BigInteger) num1;
            decodedCharsUsed -= num2;
            num1 = 0;
            num2 = 0;
          }
          if (num2 >= decodedCharsUsed)
          {
            int num4;
            if (incrementalNewlineDecoder != null)
              num4 = num2 + incrementalNewlineDecoder.decode(context, (IList<byte>) Bytes.Empty, true).Length;
            else
              num4 = num2 + ((string) PythonOps.Invoke(context, decoder, "decode", (object) Bytes.Empty, (object) true)).Length;
            if (num4 < decodedCharsUsed)
              throw PythonOps.IOError("can't reconstruct logical file position");
            break;
          }
        }
        return bigInteger2;
      }
      finally
      {
        if (incrementalNewlineDecoder != null)
          incrementalNewlineDecoder.setstate(context, state);
        else
          PythonOps.Invoke(context, decoder, "setstate", (object) state);
      }
    }

    public override BigInteger truncate(CodeContext context, object pos = null)
    {
      this.flush(context);
      if (pos == null)
        pos = (object) this.tell(context);
      if (pos is int num)
        result = (BigInteger) num;
      else if (!(pos is BigInteger result) && !Converter.TryConvertToBigInteger(pos, out result))
        throw PythonOps.TypeError("an integer is required");
      BigInteger pos1 = this.tell(context);
      this.seek(context, result, (object) 0);
      BigInteger bigInteger = this._bufferTyped != null ? this._bufferTyped.truncate(context, (object) null) : PythonIOModule.GetBigInt(PythonOps.Invoke(context, this._buffer, nameof (truncate)), "truncate() should return an integer");
      this.seek(context, pos1, (object) 0);
      return bigInteger;
    }

    public override object detach(CodeContext context)
    {
      if (this._buffer == null)
        throw PythonOps.ValueError("buffer is already detached");
      this.flush(context);
      object obj = (object) this._bufferTyped ?? this._buffer;
      this._buffer = (object) (this._bufferTyped = (PythonIOModule._BufferedIOBase) null);
      return obj;
    }

    public BigInteger seek(double offset, object whence = 0)
    {
      this._checkClosed();
      throw PythonOps.TypeError("an integer is required");
    }

    public override BigInteger seek(CodeContext context, BigInteger cookie, object whence = 0)
    {
      int num = PythonIOModule.GetInt(whence);
      if (this.closed)
        throw PythonOps.ValueError("tell on closed file");
      if (!this._seekable)
        throw PythonOps.IOError("underlying stream is not seekable");
      if (num == 1)
      {
        if (cookie != 0L)
          throw PythonOps.IOError("can't do nonzero cur-relative seeks");
        num = 0;
        cookie = this.tell(context);
      }
      else if (num == 2)
      {
        if (cookie != 0L)
          throw PythonOps.IOError("can't do nonzero end-relative seeks");
        this.flush(context);
        BigInteger bigInteger;
        if (this._bufferTyped == null)
          bigInteger = PythonIOModule.GetBigInt(PythonOps.Invoke(context, this._buffer, nameof (seek), (object) BigInteger.Zero, (object) 2), "seek() should return an integer");
        else
          bigInteger = this._bufferTyped.seek(context, BigInteger.Zero, (object) 2);
        BigInteger pos = bigInteger;
        if (pos < 0L)
          throw this.InvalidPosition(pos);
        this.SetDecodedChars(string.Empty);
        this._nextInput = (Bytes) null;
        if (this._decoder != null)
        {
          if (this._decoder is PythonIOModule.IncrementalNewlineDecoder decoder)
            decoder.reset(context);
          else
            PythonOps.Invoke(context, this._decoder, "reset");
        }
        GC.KeepAlive((object) this);
        return pos;
      }
      if (num != 0)
        throw PythonOps.ValueError("invalid whence ({0}, should be 0, 1, or 2)", (object) num);
      if (cookie < 0L)
        throw PythonOps.ValueError("negative seek position {0}", (object) cookie);
      this.flush(context);
      BigInteger pos1;
      int decodeFlags;
      int bytesFed;
      int skip;
      bool needEOF;
      this.UnpackCookie(cookie, out pos1, out decodeFlags, out bytesFed, out skip, out needEOF);
      if (this._bufferTyped != null)
        this._bufferTyped.seek(context, pos1, (object) 0);
      else
        PythonOps.Invoke(context, this._buffer, nameof (seek), (object) pos1, (object) 0);
      this.SetDecodedChars(string.Empty);
      this._nextInput = (Bytes) null;
      object decoder1 = this._decoder;
      PythonIOModule.IncrementalNewlineDecoder incrementalNewlineDecoder = decoder1 as PythonIOModule.IncrementalNewlineDecoder;
      if (cookie == BigInteger.Zero && decoder1 != null)
      {
        if (incrementalNewlineDecoder != null)
          incrementalNewlineDecoder.reset(context);
        else
          PythonOps.Invoke(context, decoder1, "reset");
      }
      else if (decoder1 != null || decodeFlags != 0 || skip != 0)
      {
        if (this._decoder == null)
        {
          decoder1 = this.GetDecoder(context);
          incrementalNewlineDecoder = decoder1 as PythonIOModule.IncrementalNewlineDecoder;
        }
        if (incrementalNewlineDecoder != null)
          incrementalNewlineDecoder.SetState(context, Bytes.Empty, decodeFlags);
        else
          PythonOps.Invoke(context, decoder1, "setstate", (object) PythonTuple.MakeTuple((object) Bytes.Empty, (object) decodeFlags));
        this._decodeFlags = decodeFlags;
        this._nextInput = Bytes.Empty;
      }
      if (skip > 0)
      {
        object obj;
        if (this._bufferTyped == null)
          obj = PythonOps.Invoke(context, this._buffer, "read", (object) bytesFed);
        else
          obj = this._bufferTyped.read(context, (object) bytesFed);
        object o = obj;
        Bytes input = o != null ? PythonIOModule.GetBytes(o, "read()") : Bytes.Empty;
        if (incrementalNewlineDecoder != null)
          this.SetDecodedChars(incrementalNewlineDecoder.decode(context, (IList<byte>) input, needEOF));
        else
          this.SetDecodedChars((string) PythonOps.Invoke(context, decoder1, "decode", (object) input, (object) needEOF));
        if (this._decodedChars.Length < skip)
          throw PythonOps.IOError("can't restore logical file position");
        this._decodedCharsUsed = skip;
      }
      try
      {
        object target = this._encoder ?? this.GetEncoder(context);
        if (cookie == 0L)
          PythonOps.Invoke(context, target, "reset");
        else
          PythonOps.Invoke(context, target, "setstate", (object) 0);
      }
      catch (LookupException ex)
      {
      }
      GC.KeepAlive((object) this);
      return cookie;
    }

    public override object read(CodeContext context, object length = null)
    {
      this._checkClosed();
      if (!this.readable(context))
        throw PythonOps.IOError("not readable");
      int length1 = PythonIOModule.GetInt(length, -1);
      object o = this._decoder ?? this.GetDecoder(context);
      if (length1 < 0)
      {
        string decodedChars = this.GetDecodedChars();
        object obj1;
        if (this._bufferTyped == null)
          obj1 = PythonOps.Invoke(context, this._buffer, nameof (read), (object) -1);
        else
          obj1 = this._bufferTyped.read(context, (object) -1);
        object obj2 = obj1;
        object boundAttr = PythonOps.GetBoundAttr(context, o, "decode");
        string str = (string) PythonOps.CallWithKeywordArgs(context, boundAttr, new object[2]
        {
          obj2,
          (object) true
        }, new string[1]{ "final" });
        this.SetDecodedChars(string.Empty);
        this._nextInput = (Bytes) null;
        return decodedChars != null ? (object) (decodedChars + str) : (object) str;
      }
      StringBuilder stringBuilder = new StringBuilder(this.GetDecodedChars(length1));
      bool flag = true;
      while (stringBuilder.Length < length1 & flag)
      {
        flag = this.ReadChunk(context);
        stringBuilder.Append(this.GetDecodedChars(length1 - stringBuilder.Length));
      }
      return (object) stringBuilder.ToString();
    }

    public override object readline(CodeContext context, int limit = -1)
    {
      this._checkClosed("read from closed file");
      string decodedChars = this.GetDecodedChars();
      int startIndex = 0;
      if (this._decoder == null)
        this.GetDecoder(context);
      int num1;
      int index;
      int num2;
      while (true)
      {
        if (this._readTranslate)
        {
          num1 = decodedChars.IndexOf('\n', startIndex);
          if (num1 < 0)
            startIndex = decodedChars.Length;
          else
            break;
        }
        else if (this._readUniversal)
        {
          index = decodedChars.IndexOfAny(new char[2]
          {
            '\r',
            '\n'
          }, startIndex);
          if (index == -1)
            startIndex = decodedChars.Length;
          else
            goto label_9;
        }
        else
        {
          num2 = decodedChars.IndexOf(this._readNL);
          if (num2 >= 0)
            goto label_11;
        }
        if (limit < 0 || decodedChars.Length < limit)
        {
          do
            ;
          while (this.ReadChunk(context) && string.IsNullOrEmpty(this._decodedChars));
          if (!string.IsNullOrEmpty(this._decodedChars))
            decodedChars += this.GetDecodedChars();
          else
            goto label_17;
        }
        else
          goto label_13;
      }
      int length = num1 + 1;
      goto label_18;
label_9:
      length = decodedChars[index] != '\n' ? (decodedChars.Length <= index + 1 || decodedChars[index + 1] != '\n' ? index + 1 : index + 2) : index + 1;
      goto label_18;
label_11:
      length = num2 + this._readNL.Length;
      goto label_18;
label_13:
      length = limit;
      goto label_18;
label_17:
      this.SetDecodedChars(string.Empty);
      this._nextInput = (Bytes) null;
      return (object) decodedChars;
label_18:
      if (limit >= 0 && length > limit)
        length = limit;
      this.RewindDecodedChars(decodedChars.Length - length);
      GC.KeepAlive((object) this);
      return (object) decodedChars.Substring(0, length);
    }

    object IEnumerator<object>.Current => this._current;

    object IEnumerator.Current => this._current;

    bool IEnumerator.MoveNext()
    {
      this._telling = false;
      this._current = this.readline(this.context, -1);
      int num = this._current == null ? 0 : (this._current is Bytes current1 && current1.Count > 0 || this._current is string current2 && current2.Length > 0 ? 1 : (PythonOps.IsTrue(this._current) ? 1 : 0));
      if (num != 0)
        return num != 0;
      this._nextInput = (Bytes) null;
      this._telling = this._seekable;
      return num != 0;
    }

    void IEnumerator.Reset()
    {
      this._current = (object) null;
      this.seek(this.context, (BigInteger) 0, (object) 0);
    }

    IEnumerator<object> IEnumerable<object>.GetEnumerator()
    {
      this._checkClosed();
      return (IEnumerator<object>) this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      this._checkClosed();
      return (IEnumerator) this;
    }

    public string __repr__(CodeContext context)
    {
      return $"<_io.TextIOWrapper encoding='{this._encoding}'>";
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new MetaExpandable<PythonIOModule.TextIOWrapper>(parameter, (IPythonExpandable) this);
    }

    private void UnpackCookie(
      BigInteger cookie,
      out BigInteger pos,
      out int decodeFlags,
      out int bytesFed,
      out int skip,
      out bool needEOF)
    {
      BigInteger bigInteger = (BigInteger.One << 64 /*0x40*/) - (BigInteger) 1;
      pos = cookie & bigInteger;
      cookie >>= 64 /*0x40*/;
      decodeFlags = (int) (cookie & bigInteger);
      cookie >>= 64 /*0x40*/;
      bytesFed = (int) (cookie & bigInteger);
      cookie >>= 64 /*0x40*/;
      skip = (int) (cookie & bigInteger);
      needEOF = cookie > bigInteger;
    }

    private object GetEncoder(CodeContext context)
    {
      object o = (object) PythonOps.LookupEncoding(context, this._encoding);
      object ret;
      if (o == null || !PythonOps.TryGetBoundAttr(context, o, "incrementalencoder", out ret))
        throw PythonOps.LookupError(this._encoding);
      this._encoder = PythonOps.CallWithContext(context, ret, (object) this._errors);
      return this._encoder;
    }

    private object GetDecoder(CodeContext context)
    {
      object o = (object) PythonOps.LookupEncoding(context, this._encoding);
      object ret;
      if (o == null || !PythonOps.TryGetBoundAttr(context, o, "incrementaldecoder", out ret))
        throw PythonOps.LookupError(this._encoding);
      this._decoder = PythonOps.CallWithContext(context, ret, (object) this._errors);
      if (this._readUniversal)
        this._decoder = (object) new PythonIOModule.IncrementalNewlineDecoder(this._decoder, this._readTranslate, "strict");
      return this._decoder;
    }

    private void SetDecodedChars(string chars)
    {
      this._decodedChars = chars;
      this._decodedCharsUsed = 0;
    }

    private string GetDecodedChars()
    {
      string decodedChars = this._decodedChars.Substring(this._decodedCharsUsed);
      this._decodedCharsUsed += decodedChars.Length;
      return decodedChars;
    }

    private string GetDecodedChars(int length)
    {
      length = Math.Min(length, this._decodedChars.Length - this._decodedCharsUsed);
      string decodedChars = this._decodedChars.Substring(this._decodedCharsUsed, length);
      this._decodedCharsUsed += length;
      return decodedChars;
    }

    private void RewindDecodedChars(int length)
    {
      if (this._decodedCharsUsed < length)
        throw PythonOps.AssertionError("rewind decoded_chars out of bounds");
      this._decodedCharsUsed -= length;
    }

    private bool ReadChunk(CodeContext context)
    {
      PythonIOModule.IncrementalNewlineDecoder incrementalNewlineDecoder = this._decoder != null ? this._decoder as PythonIOModule.IncrementalNewlineDecoder : throw PythonOps.ValueError("no decoder");
      Bytes buf = (Bytes) null;
      int flags = 0;
      if (this._telling)
      {
        if (incrementalNewlineDecoder != null)
        {
          incrementalNewlineDecoder.GetState(context, out buf, out flags);
        }
        else
        {
          PythonTuple pythonTuple = (PythonTuple) PythonOps.Invoke(context, this._decoder, "getstate");
          buf = PythonIOModule.GetBytes(pythonTuple[0], "getstate");
          flags = (int) pythonTuple[1];
        }
      }
      object obj;
      if (this._bufferTyped == null)
        obj = PythonOps.Invoke(context, this._buffer, "read", (object) this._CHUNK_SIZE);
      else
        obj = this._bufferTyped.read(context, (object) this._CHUNK_SIZE);
      object o = obj;
      Bytes input = o != null ? PythonIOModule.GetBytes(o, "read()") : Bytes.Empty;
      bool final = o == null || input.Count == 0;
      string chars;
      if (incrementalNewlineDecoder != null)
        chars = incrementalNewlineDecoder.decode(context, (IList<byte>) input, final);
      else
        chars = (string) PythonOps.Invoke(context, this._decoder, "decode", (object) input, (object) final);
      this.SetDecodedChars(chars);
      if (this._telling)
      {
        this._decodeFlags = flags;
        this._nextInput = buf + input;
      }
      return !final;
    }
  }

  [PythonType]
  public class IncrementalNewlineDecoder
  {
    private object _decoder;
    private bool _translate;
    private PythonIOModule.IncrementalNewlineDecoder.LineEnding _seenNL;
    private bool _pendingCR;
    private string _errors;

    public IncrementalNewlineDecoder(object decoder, bool translate, string errors = "strict")
    {
      this._decoder = decoder;
      this._translate = translate;
      this._errors = errors;
    }

    public string decode(CodeContext context, [NotNull] IList<byte> input, bool final = false)
    {
      object o;
      if (this._decoder == null)
        o = (object) input.MakeString();
      else
        o = PythonOps.CallWithKeywordArgs(context, PythonOps.GetBoundAttr(context, this._decoder, nameof (decode)), new object[2]
        {
          (object) input,
          (object) true
        }, new string[1]{ nameof (final) });
      switch (o)
      {
        case string decoded:
label_6:
          return this.DecodeWorker(context, decoded, final);
        case Extensible<string> _:
          decoded = ((Extensible<string>) o).Value;
          goto label_6;
        default:
          throw PythonOps.TypeError("decoder produced {0}, expected str", (object) PythonTypeOps.GetName(o));
      }
    }

    public string decode(CodeContext context, [NotNull] string input, bool final = false)
    {
      return this._decoder == null ? this.DecodeWorker(context, input, final) : this.decode(context, (IList<byte>) new Bytes((IList<byte>) input.MakeByteArray()), final);
    }

    private string DecodeWorker(CodeContext context, string decoded, bool final)
    {
      if (this._pendingCR && (final || decoded.Length > 0))
      {
        decoded = "\r" + decoded;
        this._pendingCR = false;
      }
      if (decoded.Length == 0)
        return decoded;
      if (!final && decoded.Length > 0 && decoded[decoded.Length - 1] == '\r')
      {
        decoded = decoded.Substring(0, decoded.Length - 1);
        this._pendingCR = true;
      }
      if (this._translate || this._seenNL != PythonIOModule.IncrementalNewlineDecoder.LineEnding.All)
      {
        int num1 = decoded.count("\r\n");
        int num2 = decoded.count("\r") - num1;
        if (this._seenNL != PythonIOModule.IncrementalNewlineDecoder.LineEnding.All)
        {
          int num3 = decoded.count("\n") - num1;
          this._seenNL |= (PythonIOModule.IncrementalNewlineDecoder.LineEnding) ((num1 > 0 ? 4 : 0) | (num3 > 0 ? 2 : 0) | (num2 > 0 ? 1 : 0));
        }
        if (this._translate)
        {
          if (num1 > 0)
            decoded = decoded.Replace("\r\n", "\n");
          if (num2 > 0)
            decoded = decoded.Replace('\r', '\n');
        }
      }
      return decoded;
    }

    public PythonTuple getstate(CodeContext context)
    {
      object empty = (object) Bytes.Empty;
      int num = 0;
      if (this._decoder != null)
      {
        PythonTuple pythonTuple = (PythonTuple) PythonOps.Invoke(context, this._decoder, nameof (getstate));
        empty = pythonTuple[0];
        num = Converter.ConvertToInt32(pythonTuple[1]) << 1;
      }
      if (this._pendingCR)
        num |= 1;
      return PythonTuple.MakeTuple(empty, (object) num);
    }

    internal void GetState(CodeContext context, out Bytes buf, out int flags)
    {
      PythonTuple pythonTuple = (PythonTuple) PythonOps.Invoke(context, this._decoder, "getstate");
      buf = PythonIOModule.GetBytes(pythonTuple[0], "getstate");
      flags = Converter.ConvertToInt32(pythonTuple[1]) << 1;
      if (!this._pendingCR)
        return;
      flags |= 1;
    }

    public void setstate(CodeContext context, [NotNull] PythonTuple state)
    {
      object obj = state[0];
      int int32 = Converter.ConvertToInt32(state[1]);
      this._pendingCR = (int32 & 1) != 0;
      if (this._decoder == null)
        return;
      PythonOps.Invoke(context, this._decoder, nameof (setstate), (object) PythonTuple.MakeTuple(obj, (object) (int32 >> 1)));
    }

    internal void SetState(CodeContext context, Bytes buffer, int flags)
    {
      this._pendingCR = (flags & 1) != 0;
      if (this._decoder == null)
        return;
      PythonOps.Invoke(context, this._decoder, "setstate", (object) PythonTuple.MakeTuple((object) buffer, (object) (flags >> 1)));
    }

    public void reset(CodeContext context)
    {
      this._seenNL = PythonIOModule.IncrementalNewlineDecoder.LineEnding.None;
      this._pendingCR = false;
      if (this._decoder == null)
        return;
      PythonOps.Invoke(context, this._decoder, nameof (reset));
    }

    public object newlines
    {
      get
      {
        switch (this._seenNL)
        {
          case PythonIOModule.IncrementalNewlineDecoder.LineEnding.None:
            return (object) null;
          case PythonIOModule.IncrementalNewlineDecoder.LineEnding.CR:
            return (object) "\r";
          case PythonIOModule.IncrementalNewlineDecoder.LineEnding.LF:
            return (object) "\n";
          case PythonIOModule.IncrementalNewlineDecoder.LineEnding.CR | PythonIOModule.IncrementalNewlineDecoder.LineEnding.LF:
            return (object) PythonTuple.MakeTuple((object) "\r", (object) "\n");
          case PythonIOModule.IncrementalNewlineDecoder.LineEnding.CRLF:
            return (object) "\r\n";
          case PythonIOModule.IncrementalNewlineDecoder.LineEnding.CR | PythonIOModule.IncrementalNewlineDecoder.LineEnding.CRLF:
            return (object) PythonTuple.MakeTuple((object) "\r", (object) "\r\n");
          case PythonIOModule.IncrementalNewlineDecoder.LineEnding.LF | PythonIOModule.IncrementalNewlineDecoder.LineEnding.CRLF:
            return (object) PythonTuple.MakeTuple((object) "\n", (object) "\r\n");
          default:
            return (object) PythonTuple.MakeTuple((object) "\r", (object) "\n", (object) "\r\n");
        }
      }
    }

    [Flags]
    private enum LineEnding
    {
      None = 0,
      CR = 1,
      LF = 2,
      CRLF = 4,
      All = CRLF | LF | CR, // 0x00000007
    }
  }

  [PythonType]
  [DynamicBaseType]
  private class BlockingIOError(PythonType cls) : PythonExceptions._EnvironmentError(cls)
  {
    private int _characters_written;

    public override void __init__(params object[] args)
    {
      switch (args.Length)
      {
        case 2:
          base.__init__(args);
          break;
        case 3:
          this._characters_written = PythonIOModule.GetInt(args[2], "an integer is required");
          base.__init__(new object[2]{ args[0], args[1] });
          break;
        default:
          if (args.Length < 2)
            throw PythonOps.TypeError("BlockingIOError() takes at least 2 arguments ({0} given)", (object) args.Length);
          throw PythonOps.TypeError("BlockingIOError() takes at most 3 arguments ({0} given)", (object) args.Length);
      }
    }

    public int characters_written
    {
      get => this._characters_written;
      set => this._characters_written = value;
    }
  }

  private class _BlockingIOErrorException(string msg) : IOException(msg)
  {
  }
}
