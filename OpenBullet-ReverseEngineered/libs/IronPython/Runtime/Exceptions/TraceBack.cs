// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Exceptions.TraceBack
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Exceptions;

[PythonType("traceback")]
[Serializable]
public class TraceBack
{
  private readonly TraceBack _next;
  private readonly TraceBackFrame _frame;
  private int _line;

  public TraceBack(TraceBack nextTraceBack, TraceBackFrame fromFrame)
  {
    this._next = nextTraceBack;
    this._frame = fromFrame;
  }

  public TraceBack tb_next => this._next;

  public TraceBackFrame tb_frame => this._frame;

  public int tb_lineno => this._line;

  public int tb_lasti => 0;

  internal void SetLine(int lineNumber) => this._line = lineNumber;

  internal string Extract()
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (TraceBack traceBack = this; traceBack != null; traceBack = traceBack._next)
    {
      TraceBackFrame frame = traceBack._frame;
      int line = traceBack._line;
      FunctionCode fCode = frame.f_code;
      string coFilename = fCode.co_filename;
      string coName = fCode.co_name;
      stringBuilder.AppendFormat("  File \"{0}\", line {1}, in {2}{3}", (object) coFilename, (object) line, (object) coName, (object) Environment.NewLine);
    }
    return stringBuilder.ToString();
  }
}
