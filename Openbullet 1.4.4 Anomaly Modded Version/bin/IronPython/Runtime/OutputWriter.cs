// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.OutputWriter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal sealed class OutputWriter : TextWriter
{
  private readonly PythonContext _context;
  private readonly bool _isErrorOutput;

  public OutputWriter(PythonContext context, bool isErrorOutput)
  {
    this._context = context;
    this._isErrorOutput = isErrorOutput;
  }

  public object Sink
  {
    get
    {
      return !this._isErrorOutput ? this._context.SystemStandardOut : this._context.SystemStandardError;
    }
  }

  public override Encoding Encoding
  {
    get => !(this.Sink is PythonFile sink) ? (Encoding) null : sink.Encoding;
  }

  public override void Write(string value)
  {
    try
    {
      PythonOps.PrintWithDestNoNewline(DefaultContext.Default, this.Sink, (object) value);
    }
    catch (Exception ex)
    {
      PythonOps.PrintWithDest(DefaultContext.Default, this._context.SystemStandardOut, (object) this._context.FormatException(ex));
    }
  }

  public override void Write(char value) => this.Write(value.ToString());

  public override void Write(char[] value) => this.Write(new string(value));

  public override void Flush()
  {
    if (this.Sink is PythonFile sink)
    {
      sink.flush();
    }
    else
    {
      if (!PythonOps.HasAttr(this._context.SharedContext, this.Sink, "flush"))
        return;
      PythonOps.Invoke(this._context.SharedContext, this.Sink, "flush");
    }
  }
}
