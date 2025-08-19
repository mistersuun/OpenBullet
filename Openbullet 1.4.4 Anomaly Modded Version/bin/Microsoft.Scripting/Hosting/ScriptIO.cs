// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ScriptIO
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Hosting;

public sealed class ScriptIO : MarshalByRefObject
{
  private readonly SharedIO _io;

  public Stream InputStream => this._io.InputStream;

  public Stream OutputStream => this._io.OutputStream;

  public Stream ErrorStream => this._io.ErrorStream;

  public TextReader InputReader => this._io.InputReader;

  public TextWriter OutputWriter => this._io.OutputWriter;

  public TextWriter ErrorWriter => this._io.ErrorWriter;

  public Encoding InputEncoding => this._io.InputEncoding;

  public Encoding OutputEncoding => this._io.OutputEncoding;

  public Encoding ErrorEncoding => this._io.ErrorEncoding;

  internal SharedIO SharedIO => this._io;

  internal ScriptIO(SharedIO io) => this._io = io;

  public void SetOutput(Stream stream, Encoding encoding)
  {
    ContractUtils.RequiresNotNull((object) stream, nameof (stream));
    ContractUtils.RequiresNotNull((object) encoding, nameof (encoding));
    this._io.SetOutput(stream, (TextWriter) new StreamWriter(stream, encoding));
  }

  public void SetOutput(Stream stream, TextWriter writer)
  {
    ContractUtils.RequiresNotNull((object) stream, nameof (stream));
    ContractUtils.RequiresNotNull((object) writer, nameof (writer));
    this._io.SetOutput(stream, writer);
  }

  public void SetErrorOutput(Stream stream, Encoding encoding)
  {
    ContractUtils.RequiresNotNull((object) stream, nameof (stream));
    ContractUtils.RequiresNotNull((object) encoding, nameof (encoding));
    this._io.SetErrorOutput(stream, (TextWriter) new StreamWriter(stream, encoding));
  }

  public void SetErrorOutput(Stream stream, TextWriter writer)
  {
    ContractUtils.RequiresNotNull((object) stream, nameof (stream));
    ContractUtils.RequiresNotNull((object) writer, nameof (writer));
    this._io.SetErrorOutput(stream, writer);
  }

  public void SetInput(Stream stream, Encoding encoding)
  {
    ContractUtils.RequiresNotNull((object) stream, nameof (stream));
    ContractUtils.RequiresNotNull((object) encoding, nameof (encoding));
    this._io.SetInput(stream, (TextReader) new StreamReader(stream, encoding), encoding);
  }

  public void SetInput(Stream stream, TextReader reader, Encoding encoding)
  {
    ContractUtils.RequiresNotNull((object) stream, nameof (stream));
    ContractUtils.RequiresNotNull((object) reader, nameof (reader));
    ContractUtils.RequiresNotNull((object) encoding, nameof (encoding));
    this._io.SetInput(stream, reader, encoding);
  }

  public void RedirectToConsole() => this._io.RedirectToConsole();

  public override object InitializeLifetimeService() => (object) null;
}
