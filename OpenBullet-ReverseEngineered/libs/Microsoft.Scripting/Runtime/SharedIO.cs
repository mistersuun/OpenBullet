// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.SharedIO
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class SharedIO
{
  private readonly object _mutex = new object();
  private Stream _inputStream;
  private Stream _outputStream;
  private Stream _errorStream;
  private TextReader _inputReader;
  private TextWriter _outputWriter;
  private TextWriter _errorWriter;
  private Encoding _inputEncoding;

  public Stream InputStream
  {
    get
    {
      this.InitializeInput();
      return this._inputStream;
    }
  }

  public Stream OutputStream
  {
    get
    {
      this.InitializeOutput();
      return this._outputStream;
    }
  }

  public Stream ErrorStream
  {
    get
    {
      this.InitializeErrorOutput();
      return this._errorStream;
    }
  }

  public TextReader InputReader
  {
    get
    {
      this.InitializeInput();
      return this._inputReader;
    }
  }

  public TextWriter OutputWriter
  {
    get
    {
      this.InitializeOutput();
      return this._outputWriter;
    }
  }

  public TextWriter ErrorWriter
  {
    get
    {
      this.InitializeErrorOutput();
      return this._errorWriter;
    }
  }

  public Encoding InputEncoding
  {
    get
    {
      this.InitializeInput();
      return this._inputEncoding;
    }
  }

  public Encoding OutputEncoding
  {
    get
    {
      this.InitializeOutput();
      return this._outputWriter.Encoding;
    }
  }

  public Encoding ErrorEncoding
  {
    get
    {
      this.InitializeErrorOutput();
      return this._errorWriter.Encoding;
    }
  }

  internal SharedIO()
  {
  }

  private void InitializeInput()
  {
    if (this._inputStream != null)
      return;
    lock (this._mutex)
    {
      if (this._inputStream != null)
        return;
      this._inputStream = (Stream) ConsoleInputStream.Instance;
      this._inputEncoding = Console.InputEncoding;
      this._inputReader = Console.In;
    }
  }

  private void InitializeOutput()
  {
    if (this._outputStream != null)
      return;
    lock (this._mutex)
    {
      if (this._outputStream != null)
        return;
      this._outputStream = Console.OpenStandardOutput();
      this._outputWriter = Console.Out;
    }
  }

  private void InitializeErrorOutput()
  {
    if (this._errorStream != null)
      return;
    lock (this._mutex)
    {
      if (this._errorStream != null)
        return;
      this._errorStream = Console.OpenStandardError();
      this._errorWriter = Console.Error;
    }
  }

  public void SetOutput(Stream stream, TextWriter writer)
  {
    lock (this._mutex)
    {
      this._outputStream = stream;
      this._outputWriter = writer;
    }
  }

  public void SetErrorOutput(Stream stream, TextWriter writer)
  {
    lock (this._mutex)
    {
      this._errorStream = stream;
      this._errorWriter = writer;
    }
  }

  public void SetInput(Stream stream, TextReader reader, Encoding encoding)
  {
    lock (this._mutex)
    {
      this._inputStream = stream;
      this._inputReader = reader;
      this._inputEncoding = encoding;
    }
  }

  public void RedirectToConsole()
  {
    lock (this._mutex)
    {
      this._inputEncoding = (Encoding) null;
      this._inputStream = (Stream) null;
      this._outputStream = (Stream) null;
      this._errorStream = (Stream) null;
      this._inputReader = (TextReader) null;
      this._outputWriter = (TextWriter) null;
      this._errorWriter = (TextWriter) null;
    }
  }

  public Stream GetStream(ConsoleStreamType type)
  {
    switch (type)
    {
      case ConsoleStreamType.Input:
        return this.InputStream;
      case ConsoleStreamType.Output:
        return this.OutputStream;
      case ConsoleStreamType.ErrorOutput:
        return this.ErrorStream;
      default:
        throw Error.InvalidStreamType((object) type);
    }
  }

  public TextWriter GetWriter(ConsoleStreamType type)
  {
    if (type == ConsoleStreamType.Output)
      return this.OutputWriter;
    if (type == ConsoleStreamType.ErrorOutput)
      return this.ErrorWriter;
    throw Error.InvalidStreamType((object) type);
  }

  public Encoding GetEncoding(ConsoleStreamType type)
  {
    switch (type)
    {
      case ConsoleStreamType.Input:
        return this.InputEncoding;
      case ConsoleStreamType.Output:
        return this.OutputEncoding;
      case ConsoleStreamType.ErrorOutput:
        return this.ErrorEncoding;
      default:
        throw Error.InvalidStreamType((object) type);
    }
  }

  public TextReader GetReader(out Encoding encoding)
  {
    TextReader inputReader;
    lock (this._mutex)
    {
      inputReader = this.InputReader;
      encoding = this.InputEncoding;
    }
    return inputReader;
  }

  public Stream GetStreamProxy(ConsoleStreamType type)
  {
    return (Stream) new SharedIO.StreamProxy(this, type);
  }

  private sealed class StreamProxy : Stream
  {
    private readonly ConsoleStreamType _type;
    private readonly SharedIO _io;

    public StreamProxy(SharedIO io, ConsoleStreamType type)
    {
      this._io = io;
      this._type = type;
    }

    public override bool CanRead => this._type == ConsoleStreamType.Input;

    public override bool CanSeek => false;

    public override bool CanWrite => !this.CanRead;

    public override void Flush() => this._io.GetStream(this._type).Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
      return this._io.GetStream(this._type).Read(buffer, offset, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this._io.GetStream(this._type).Write(buffer, offset, count);
    }

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();
  }
}
