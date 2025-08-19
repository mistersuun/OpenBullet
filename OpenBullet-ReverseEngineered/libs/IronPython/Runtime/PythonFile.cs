// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonFile
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using Microsoft.Win32.SafeHandles;
using Mono.Unix;
using Mono.Unix.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

[PythonType("file")]
[DontMapIEnumerableToContains]
public class PythonFile : 
  IDisposable,
  ICodeFormattable,
  IEnumerator<string>,
  IEnumerator,
  IWeakReferenceable
{
  private ConsoleStreamType _consoleStreamType;
  private SharedIO _io;
  internal Stream _stream;
  private string _mode;
  private string _name;
  private string _encoding;
  private PythonFile.PythonFileMode _fileMode;
  private PythonStreamReader _reader;
  private PythonStreamWriter _writer;
  [PythonHidden(new PlatformID[] {})]
  protected bool _isOpen;
  private long? _reseekPosition;
  private WeakRefTracker _weakref;
  private string _enumValue;
  internal readonly PythonContext _context;
  private bool _softspace;

  internal bool IsConsole => this._stream == null;

  internal bool IsOutput => this._writer != null;

  internal PythonFile(PythonContext context) => this._context = context;

  public PythonFile(CodeContext context)
    : this(context.LanguageContext)
  {
  }

  internal static PythonFile Create(CodeContext context, Stream stream, string name, string mode)
  {
    return PythonFile.Create(context, stream, context.LanguageContext.DefaultEncoding, name, mode);
  }

  internal static PythonFile Create(
    CodeContext context,
    Stream stream,
    Encoding encoding,
    string name,
    string mode)
  {
    PythonFile pythonFile = new PythonFile(context.LanguageContext);
    pythonFile.__init__(stream, encoding, name, mode);
    return pythonFile;
  }

  internal static PythonFile CreateConsole(
    PythonContext context,
    SharedIO io,
    ConsoleStreamType type,
    string name)
  {
    PythonFile console = new PythonFile(context);
    console.InitializeConsole(io, type, name);
    return console;
  }

  internal static PythonFile[] CreatePipe(CodeContext context)
  {
    Encoding encoding = context.LanguageContext.DefaultEncoding;
    PythonFile inPipeFile = new PythonFile(context);
    PythonFile outPipeFile = new PythonFile(context);
    if (Environment.OSVersion.Platform == PlatformID.Unix)
    {
      InitializePipesUnix();
    }
    else
    {
      AnonymousPipeServerStream pipeServerStream = new AnonymousPipeServerStream(PipeDirection.In);
      inPipeFile.InitializePipe((Stream) pipeServerStream, "r", encoding);
      AnonymousPipeClientStream pipeClientStream = new AnonymousPipeClientStream(PipeDirection.Out, pipeServerStream.ClientSafePipeHandle);
      outPipeFile.InitializePipe((Stream) pipeClientStream, "w", encoding);
    }
    return new PythonFile[2]{ inPipeFile, outPipeFile };

    void InitializePipesUnix()
    {
      UnixPipes pipes = UnixPipes.CreatePipes();
      inPipeFile.InitializePipe((Stream) pipes.Reading, "r", encoding);
      outPipeFile.InitializePipe((Stream) pipes.Writing, "w", encoding);
    }
  }

  [PythonHidden(new PlatformID[] {})]
  public static PythonTuple CreatePipeAsFd(CodeContext context)
  {
    PythonFile[] pipe = PythonFile.CreatePipe(context);
    return PythonTuple.MakeTuple((object) context.LanguageContext.FileManager.AddToStrongMapping(pipe[0]), (object) context.LanguageContext.FileManager.AddToStrongMapping(pipe[1]));
  }

  ~PythonFile()
  {
    try
    {
      this.Dispose(false);
    }
    catch (ObjectDisposedException ex)
    {
    }
    catch (EncoderFallbackException ex)
    {
    }
    catch (IOException ex)
    {
    }
    finally
    {
      // ISSUE: explicit finalizer call
      base.Finalize();
    }
  }

  public void __init__(CodeContext context, string name, string mode = "r", double buffering)
  {
    throw PythonOps.TypeError("integer argument expected, got float");
  }

  public void __init__(CodeContext context, string name, string mode = "r", BigInteger buffering)
  {
    this.__init__(context, name, mode, (int) buffering);
  }

  public void __init__(CodeContext context, string name, string mode = "r", int buffering = -1)
  {
    FileShare share = FileShare.ReadWrite;
    if (name == null)
      throw PythonOps.TypeError("file name must be string, found NoneType");
    switch (mode)
    {
      case null:
        throw PythonOps.TypeError("mode must be string, not None");
      case "":
        throw PythonOps.ValueError("empty mode string");
      default:
        FileMode fmode;
        FileAccess faccess;
        bool seekEnd;
        PythonFile.TranslateAndValidateMode(mode, out fmode, out faccess, out seekEnd);
        try
        {
          Stream stream;
          try
          {
            stream = Environment.OSVersion.Platform != PlatformID.Win32NT || !(name == "nul") ? (buffering > 0 ? context.LanguageContext.DomainManager.Platform.OpenInputFileStream(name, fmode, faccess, share, buffering) : context.LanguageContext.DomainManager.Platform.OpenInputFileStream(name, fmode, faccess, share)) : Stream.Null;
          }
          catch (IOException ex)
          {
            PythonFile.AddFilename(context, name, (Exception) ex);
            throw;
          }
          GC.SuppressFinalize((object) stream);
          if (seekEnd)
            stream.Seek(0L, SeekOrigin.End);
          this.__init__(stream, context.LanguageContext.DefaultEncoding, name, mode);
          this._isOpen = true;
          break;
        }
        catch (UnauthorizedAccessException ex)
        {
          throw PythonFile.ToIoException(context, name, ex);
        }
    }
  }

  internal static Exception ToIoException(
    CodeContext context,
    string name,
    UnauthorizedAccessException e)
  {
    Exception ioe = (Exception) new IOException(e.Message, (Exception) e);
    PythonFile.AddFilename(context, name, ioe);
    return ioe;
  }

  internal static void AddFilename(CodeContext context, string name, Exception ioe)
  {
    object python = PythonExceptions.ToPython(ioe);
    PythonOps.SetAttr(context, python, "filename", (object) name);
  }

  internal static void ValidateMode(string mode)
  {
    PythonFile.TranslateAndValidateMode(mode, out FileMode _, out FileAccess _, out bool _);
  }

  private static void TranslateAndValidateMode(
    string mode,
    out FileMode fmode,
    out FileAccess faccess,
    out bool seekEnd)
  {
    string str = mode.Length != 0 ? mode : throw PythonOps.ValueError("empty mode string");
    if (mode.IndexOf('U') != -1)
    {
      mode = mode.Replace("U", string.Empty);
      switch (mode)
      {
        case "":
          mode = "r";
          break;
        case "+":
          mode = "r+";
          break;
        default:
          if (mode[0] == 'w' || mode[0] == 'a')
            throw PythonOps.ValueError("universal newline mode can only be used with modes starting with 'r'");
          if (mode[0] != 'r')
          {
            mode = "r" + mode;
            break;
          }
          break;
      }
    }
    seekEnd = false;
    switch (mode[0])
    {
      case 'a':
        fmode = FileMode.Append;
        mode = mode.Remove(0, 1);
        break;
      case 'r':
        fmode = FileMode.Open;
        mode = mode.Remove(0, 1);
        break;
      case 'w':
        fmode = FileMode.Create;
        mode = mode.Remove(0, 1);
        break;
      default:
        throw PythonOps.ValueError("mode string must begin with one of 'r', 'w', 'a' or 'U', not '{0}'", (object) str);
    }
    if (mode.IndexOf('+') != -1)
    {
      mode = mode.Remove(mode.IndexOf('+'), 1);
      faccess = FileAccess.ReadWrite;
      if (fmode == FileMode.Append)
      {
        fmode = FileMode.OpenOrCreate;
        seekEnd = true;
      }
    }
    else
    {
      switch (fmode)
      {
        case FileMode.Create:
          faccess = FileAccess.Write;
          break;
        case FileMode.Open:
          faccess = FileAccess.Read;
          break;
        case FileMode.Append:
          faccess = FileAccess.Write;
          break;
        default:
          throw new InvalidOperationException();
      }
    }
    if (mode.Length > 0 && mode[0] != 'U' && mode[0] != 'b' && mode[0] != 't')
      throw PythonOps.ValueError("Invalid mode ('{0}')", (object) str);
  }

  public void __init__(CodeContext context, [NotNull] Stream stream)
  {
    ContractUtils.RequiresNotNull((object) stream, nameof (stream));
    string mode = !stream.CanRead || !stream.CanWrite ? (!stream.CanWrite ? "r" : "w") : "w+";
    this.__init__(stream, context.LanguageContext.DefaultEncoding, mode);
  }

  public void __init__(CodeContext context, [NotNull] Stream stream, string mode)
  {
    this.__init__(stream, context.LanguageContext.DefaultEncoding, mode);
  }

  public void __init__([NotNull] Stream stream, Encoding encoding, string mode)
  {
    this.InternalInitialize(stream, encoding, mode);
  }

  public void __init__([NotNull] Stream stream, [NotNull] Encoding encoding, string name, string mode)
  {
    ContractUtils.RequiresNotNull((object) stream, nameof (stream));
    ContractUtils.RequiresNotNull((object) encoding, nameof (encoding));
    this.InternalInitialize(stream, encoding, name, mode);
  }

  private PythonTextReader CreateTextReader(
    TextReader reader,
    Encoding encoding,
    long initPosition)
  {
    switch (this._fileMode)
    {
      case PythonFile.PythonFileMode.TextCrLf:
        return (PythonTextReader) new PythonTextCRLFReader(reader, encoding, initPosition);
      case PythonFile.PythonFileMode.TextCr:
        return (PythonTextReader) new PythonTextCRReader(reader, encoding, initPosition);
      case PythonFile.PythonFileMode.TextLf:
        return (PythonTextReader) new PythonTextLFReader(reader, encoding, initPosition);
      case PythonFile.PythonFileMode.UniversalNewline:
        return (PythonTextReader) new PythonUniversalReader(reader, encoding, initPosition);
      default:
        throw Assert.Unreachable;
    }
  }

  private PythonTextReader CreateConsoleReader()
  {
    Encoding encoding;
    return this.CreateTextReader(this._io.GetReader(out encoding), encoding, 0L);
  }

  private PythonTextWriter CreateTextWriter(TextWriter writer)
  {
    PythonFile.PythonFileMode pythonFileMode = this._fileMode;
    if (this._fileMode == PythonFile.PythonFileMode.UniversalNewline)
      pythonFileMode = Environment.OSVersion.Platform != PlatformID.Unix ? PythonFile.PythonFileMode.TextCrLf : PythonFile.PythonFileMode.TextLf;
    switch (pythonFileMode)
    {
      case PythonFile.PythonFileMode.TextCrLf:
        return new PythonTextWriter(writer, "\r\n");
      case PythonFile.PythonFileMode.TextCr:
        return new PythonTextWriter(writer, "\r");
      case PythonFile.PythonFileMode.TextLf:
        return new PythonTextWriter(writer, "\n");
      default:
        throw Assert.Unreachable;
    }
  }

  internal bool SetMode(CodeContext context, bool text)
  {
    lock (this)
    {
      PythonFile.PythonFileMode pythonFileMode = PythonFile.MapFileMode(this._mode);
      this._fileMode = !text ? PythonFile.PythonFileMode.Binary : (pythonFileMode != PythonFile.PythonFileMode.Binary ? pythonFileMode : PythonFile.PythonFileMode.UniversalNewline);
      if (this._stream != null)
      {
        Encoding encoding;
        if (!StringOps.TryGetEncoding(this._encoding, out encoding))
          encoding = context.LanguageContext.DefaultEncoding;
        this.InitializeReaderAndWriter(this._stream, encoding);
      }
      else if (text)
        this.InitializeConsole(this._io, this._consoleStreamType, this._name);
      else
        this.InitializeReaderAndWriter(this._stream = this._io.GetStream(this._consoleStreamType), this._io.GetEncoding(this._consoleStreamType));
      return this._fileMode != PythonFile.PythonFileMode.Binary;
    }
  }

  internal void InternalInitialize(Stream stream, Encoding encoding, string mode)
  {
    this._stream = stream;
    this._mode = mode;
    this._isOpen = true;
    this._io = (SharedIO) null;
    this._fileMode = PythonFile.MapFileMode(mode);
    this._encoding = StringOps.GetEncodingName(encoding);
    this.InitializeReaderAndWriter(stream, encoding);
    if (stream is FileStream fileStream)
      this._name = fileStream.Name;
    else
      this._name = "nul";
  }

  private void InitializeReaderAndWriter(Stream stream, Encoding encoding)
  {
    if (stream.CanRead)
    {
      if (this._fileMode == PythonFile.PythonFileMode.Binary)
      {
        this._reader = (PythonStreamReader) new PythonBinaryReader(stream);
      }
      else
      {
        long position = stream.CanSeek ? stream.Position : 0L;
        this._reader = (PythonStreamReader) this.CreateTextReader((TextReader) new StreamReader(stream, encoding, false), encoding, position);
      }
    }
    if (!stream.CanWrite)
      return;
    if (this._fileMode == PythonFile.PythonFileMode.Binary)
      this._writer = (PythonStreamWriter) new PythonBinaryWriter(stream);
    else
      this._writer = (PythonStreamWriter) this.CreateTextWriter((TextWriter) new StreamWriter(stream, encoding));
  }

  internal void InitializeConsole(SharedIO io, ConsoleStreamType type, string name)
  {
    this._consoleStreamType = type;
    this._io = io;
    this._mode = type == ConsoleStreamType.Input ? "r" : "w";
    this._isOpen = true;
    this._fileMode = PythonFile.MapFileMode(this._mode);
    this._name = name;
    this._encoding = StringOps.GetEncodingName(io.OutputEncoding);
    if (type == ConsoleStreamType.Input)
      this._reader = (PythonStreamReader) this.CreateConsoleReader();
    else
      this._writer = (PythonStreamWriter) this.CreateTextWriter(this._io.GetWriter(type));
  }

  internal void InitializePipe(Stream stream, string mode, Encoding encoding)
  {
    this._stream = stream;
    this._io = (SharedIO) null;
    this._name = "<pipe>";
    this._mode = mode;
    this._fileMode = PythonFile.PythonFileMode.Binary;
    this._encoding = StringOps.GetEncodingName(encoding);
    this._isOpen = true;
    this.InitializeReaderAndWriter(stream, encoding);
  }

  internal void InternalInitialize(Stream stream, Encoding encoding, string name, string mode)
  {
    this.InternalInitialize(stream, encoding, mode);
    this._name = name;
  }

  internal bool TryGetFileHandle(out object handle)
  {
    Stream stream = this._stream;
    SharedIO io = this._io;
    if (stream == null && io != null)
      stream = io.GetStream(this._consoleStreamType);
    if (stream is FileStream)
    {
      handle = ((FileStream) stream).SafeFileHandle.DangerousGetHandle().ToPython();
      return true;
    }
    if (stream is PipeStream)
    {
      handle = ((PipeStream) stream).SafePipeHandle.DangerousGetHandle().ToPython();
      return true;
    }
    if (Environment.OSVersion.Platform == PlatformID.Unix)
    {
      handle = GetFileHandleUnix();
      if (handle != null)
        return true;
    }
    object obj = stream.GetType().GetField("_handle", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue((object) stream);
    if (obj is SafeFileHandle)
    {
      handle = ((SafeHandle) obj).DangerousGetHandle().ToPython();
      return true;
    }
    handle = (object) null;
    return false;

    object GetFileHandleUnix()
    {
      return stream is UnixStream ? (object) ((UnixStream) stream).Handle : (object) null;
    }
  }

  private static PythonFile.PythonFileMode MapFileMode(string mode)
  {
    if (mode.Contains("b"))
      return PythonFile.PythonFileMode.Binary;
    if (mode.Contains("U"))
      return PythonFile.PythonFileMode.UniversalNewline;
    switch (Environment.NewLine)
    {
      case "\r\n":
        return PythonFile.PythonFileMode.TextCrLf;
      case "\r":
        return PythonFile.PythonFileMode.TextCr;
      case "\n":
        return PythonFile.PythonFileMode.TextLf;
      default:
        throw new NotImplementedException("Unsupported Environment.NewLine value");
    }
  }

  internal Encoding Encoding
  {
    get
    {
      if (this._reader != null)
        return this._reader.Encoding;
      return this._writer == null ? (Encoding) null : this._writer.Encoding;
    }
  }

  internal PythonContext Context => this._context;

  void IDisposable.Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  [PythonHidden(new PlatformID[] {})]
  protected virtual void Dispose(bool disposing)
  {
    lock (this)
    {
      if (!this._isOpen)
        return;
      try
      {
        this.FlushNoLock();
      }
      catch (IOException ex)
      {
      }
      this._isOpen = false;
      if (!this.IsConsole)
        this._stream.Dispose();
      PythonFileManager rawFileManager = this._context.RawFileManager;
      if (rawFileManager == null)
        return;
      rawFileManager.Remove(this);
      rawFileManager.Remove((object) this._stream);
    }
  }

  public virtual object close()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
    return (object) null;
  }

  [Documentation("True if the file is closed, False if the file is still open")]
  public bool closed => !this._isOpen;

  [PythonHidden(new PlatformID[] {})]
  protected void ThrowIfClosed()
  {
    if (!this._isOpen)
      throw PythonOps.ValueError("I/O operation on closed file");
  }

  public virtual void flush()
  {
    lock (this)
      this.FlushNoLock();
  }

  private void FlushNoLock()
  {
    this.ThrowIfClosed();
    if (this._writer == null)
      return;
    this._writer.Flush();
    if (this.IsConsole)
      return;
    this._stream.Flush();
  }

  internal void FlushToDisk()
  {
    lock (this)
    {
      this.ThrowIfClosed();
      if (this._writer == null)
        return;
      this._writer.Flush();
      this._writer.FlushToDisk();
    }
  }

  public int fileno()
  {
    this.ThrowIfClosed();
    return this._context.FileManager.GetOrAssignIdForFile(this);
  }

  [Documentation("gets the mode of the file")]
  public string mode => this._mode;

  [Documentation("gets the name of the file")]
  public string name => this._name;

  [Documentation("gets the encoding used when reading/writing text")]
  public string encoding => this._encoding;

  public string read() => this.read(-1);

  public string read(int size)
  {
    PythonStreamReader reader = this.GetReader();
    return size < 0 ? reader.ReadToEnd() : reader.Read(size);
  }

  public string readline() => this.GetReader().ReadLine();

  public string readline(int size) => this.GetReader().ReadLine(size);

  public List readlines()
  {
    List list = new List();
    while (true)
    {
      string str = this.readline();
      if (!string.IsNullOrEmpty(str))
        list.AddNoLock((object) str);
      else
        break;
    }
    return list;
  }

  public List readlines(int sizehint)
  {
    List list = new List();
    while (true)
    {
      string str = this.readline();
      if (!string.IsNullOrEmpty(str))
      {
        list.AddNoLock((object) str);
        if (str.Length < sizehint)
          sizehint -= str.Length;
        else
          break;
      }
      else
        break;
    }
    return list;
  }

  public void seek(long offset) => this.seek(offset, 0);

  public void seek(long offset, int whence)
  {
    if (this._mode == "a")
      return;
    this.ThrowIfClosed();
    if (this.IsConsole || !this._stream.CanSeek)
      throw PythonOps.IOError("Can not seek on file " + this._name);
    lock (this)
    {
      this.FlushNoLock();
      this.SavePositionPreSeek();
      SeekOrigin origin = (SeekOrigin) whence;
      long num = this._stream.Seek(offset, origin);
      if (this._reader == null)
        return;
      this._reader.DiscardBufferedData();
      this._reader.Position = num;
    }
  }

  public bool softspace
  {
    [Python3Warning("file.softspace not supported in 3.x")] get => this._softspace;
    [Python3Warning("file.softspace not supported in 3.x")] set => this._softspace = value;
  }

  public object tell()
  {
    long currentPosition = this.GetCurrentPosition();
    return currentPosition <= (long) int.MaxValue ? (object) (int) currentPosition : (object) (BigInteger) currentPosition;
  }

  private long GetCurrentPosition()
  {
    if (this._reader != null)
      return this._reader.Position;
    return this._stream != null ? this._stream.Position : throw PythonExceptions.CreateThrowable(PythonExceptions.IOError, (object) 9, (object) "Bad file descriptor");
  }

  public void truncate()
  {
    lock (this)
    {
      this.FlushNoLock();
      this.TruncateNoLock(this.GetCurrentPosition());
    }
  }

  public void truncate(long size)
  {
    lock (this)
    {
      this.FlushNoLock();
      this.TruncateNoLock(size);
    }
  }

  private void TruncateNoLock(long size)
  {
    if (size < 0L)
      throw PythonExceptions.CreateThrowable(PythonExceptions.IOError, (object) 22, (object) "Invalid argument");
    lock (this)
    {
      if (this._stream is FileStream stream)
      {
        if (stream.CanWrite)
          stream.SetLength(size);
        else
          throw PythonExceptions.CreateThrowable(PythonExceptions.IOError, (object) 13, (object) "Permission denied");
      }
      else
        throw PythonExceptions.CreateThrowable(PythonExceptions.IOError, (object) 9, (object) "Bad file descriptor");
    }
  }

  public void write(string s)
  {
    if (s == null)
      throw PythonOps.TypeError("must be string or read-only character buffer, not None");
    lock (this)
      this.WriteNoLock(s);
  }

  public void write([NotNull] IList<byte> bytes)
  {
    lock (this)
      this.WriteNoLock(bytes);
  }

  private void WriteNoLock(string s)
  {
    int num = this.GetWriter().Write(s);
    if (!this.IsConsole && this._reader != null && this._stream.CanSeek)
      this._reader.Position += (long) num;
    if (!this.IsConsole)
      return;
    this.FlushNoLock();
  }

  private void WriteNoLock([NotNull] IList<byte> b)
  {
    int num = this.GetWriter().WriteBytes(b);
    if (!this.IsConsole && this._reader != null && this._stream.CanSeek)
      this._reader.Position += (long) num;
    if (!this.IsConsole)
      return;
    this.FlushNoLock();
  }

  public void write([NotNull] PythonBuffer buf) => this.write((IList<byte>) buf);

  public void write([NotNull] object arr) => this.WriteWorker(arr, true);

  private void WriteWorker(object arr, bool locking)
  {
    if (!(arr is IPythonArray pythonArray))
      throw PythonOps.TypeError("file.write() argument must be string or read-only character buffer, not {0}", (object) DynamicHelpers.GetPythonType(arr).Name);
    if (this._fileMode != PythonFile.PythonFileMode.Binary)
      throw PythonOps.TypeError("file.write() argument must be string or buffer, not {0}", (object) DynamicHelpers.GetPythonType(arr).Name);
    if (locking)
      this.write(pythonArray.tostring());
    else
      this.WriteNoLock(pythonArray.tostring());
  }

  public void writelines(object o)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(o);
    if (!enumerator.MoveNext())
      return;
    lock (this)
    {
      do
      {
        if (!(enumerator.Current is string current3))
        {
          if (enumerator.Current is Bytes current2)
            this.WriteWorker((object) current2, false);
          else if (enumerator.Current is PythonBuffer current1)
          {
            this.WriteNoLock((IList<byte>) current1);
          }
          else
          {
            if (!(enumerator.Current is IPythonArray current))
              throw PythonOps.TypeError("writelines() argument must be a sequence of strings");
            this.WriteWorker((object) current, false);
          }
        }
        else
          this.WriteNoLock(current3);
      }
      while (enumerator.MoveNext());
    }
  }

  [Python3Warning("f.xreadlines() not supported in 3.x, try 'for line in f' instead")]
  public PythonFile xreadlines() => this;

  public object newlines
  {
    get
    {
      if (this._reader == null || !(this._reader is PythonUniversalReader))
        return (object) null;
      PythonUniversalReader.TerminatorStyles terminators = ((PythonUniversalReader) this._reader).Terminators;
      switch (terminators)
      {
        case PythonUniversalReader.TerminatorStyles.None:
          return (object) null;
        case PythonUniversalReader.TerminatorStyles.CrLf:
          return (object) "\r\n";
        case PythonUniversalReader.TerminatorStyles.Cr:
          return (object) "\r";
        case PythonUniversalReader.TerminatorStyles.Lf:
          return (object) "\n";
        default:
          List<string> stringList = new List<string>();
          if ((terminators & PythonUniversalReader.TerminatorStyles.CrLf) != PythonUniversalReader.TerminatorStyles.None)
            stringList.Add("\r\n");
          if ((terminators & PythonUniversalReader.TerminatorStyles.Cr) != PythonUniversalReader.TerminatorStyles.None)
            stringList.Add("\r");
          if ((terminators & PythonUniversalReader.TerminatorStyles.Lf) != PythonUniversalReader.TerminatorStyles.None)
            stringList.Add("\n");
          return (object) new PythonTuple((object) stringList.ToArray());
      }
    }
  }

  private void SavePositionPreSeek()
  {
    if (!(this._mode == "a+"))
      return;
    this._reseekPosition = new long?(this._stream.Position);
  }

  private PythonStreamReader GetReader()
  {
    this.ThrowIfClosed();
    if (this._reader == null)
      throw PythonOps.IOError("Can not read from " + this._name);
    if (this.IsConsole)
    {
      lock (this)
      {
        if (this._io.InputReader != this._reader.TextReader)
          this._reader = (PythonStreamReader) this.CreateConsoleReader();
      }
    }
    return this._reader;
  }

  private PythonStreamWriter GetWriter()
  {
    this.ThrowIfClosed();
    if (this._writer == null)
      throw PythonOps.IOError("Can not write to " + this._name);
    lock (this)
    {
      if (this.IsConsole)
      {
        if (this._stream != this._io.GetStream(this._consoleStreamType))
        {
          TextWriter writer = this._io.GetWriter(this._consoleStreamType);
          if (writer != this._writer.TextWriter)
          {
            try
            {
              this._writer.Flush();
            }
            catch (ObjectDisposedException ex)
            {
            }
            this._writer = (PythonStreamWriter) this.CreateTextWriter(writer);
          }
        }
      }
      else if (this._reseekPosition.HasValue)
      {
        this._stream.Seek(this._reseekPosition.Value, SeekOrigin.Begin);
        this._reader.Position = this._reseekPosition.Value;
        this._reseekPosition = new long?();
      }
    }
    return this._writer;
  }

  public object next()
  {
    string str = this.readline();
    return !string.IsNullOrEmpty(str) ? (object) str : throw PythonOps.StopIteration();
  }

  public object __iter__()
  {
    this.ThrowIfClosed();
    return (object) this;
  }

  public bool isatty()
  {
    if (Environment.OSVersion.Platform == PlatformID.Unix)
      return isattyUnix();
    return this.IsConsole && !isRedirected();

    static bool isattyUnix() => Syscall.isatty(0) || Syscall.isatty(1) || Syscall.isatty(2);

    bool isRedirected()
    {
      if (this._consoleStreamType == ConsoleStreamType.Output)
        return Console.IsOutputRedirected;
      return this._consoleStreamType == ConsoleStreamType.Input ? Console.IsInputRedirected : Console.IsErrorRedirected;
    }
  }

  public object __enter__()
  {
    this.ThrowIfClosed();
    return (object) this;
  }

  public void __exit__(params object[] excinfo) => this.close();

  public virtual string __repr__(CodeContext context)
  {
    return $"<{(this._isOpen ? (object) "open" : (object) "closed")} file '{this._name ?? "<uninitialized file>"}', mode '{this._mode ?? "<uninitialized file>"}' at 0x{this.GetHashCode():X8}>";
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

  string IEnumerator<string>.Current => this._enumValue;

  object IEnumerator.Current => (object) this._enumValue;

  bool IEnumerator.MoveNext()
  {
    this._enumValue = this.readline();
    return !string.IsNullOrEmpty(this._enumValue);
  }

  void IEnumerator.Reset() => throw new NotImplementedException();

  private enum PythonFileMode
  {
    Binary,
    TextCrLf,
    TextCr,
    TextLf,
    UniversalNewline,
  }
}
