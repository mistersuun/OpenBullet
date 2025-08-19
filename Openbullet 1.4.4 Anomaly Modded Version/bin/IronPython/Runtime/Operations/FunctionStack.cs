// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.FunctionStack
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using System.Diagnostics;

#nullable disable
namespace IronPython.Runtime.Operations;

[DebuggerDisplay("Code = {Code.co_name}, Line = {Frame.f_lineno}")]
public struct FunctionStack
{
  public readonly CodeContext Context;
  public readonly FunctionCode Code;
  public TraceBackFrame Frame;

  internal FunctionStack(CodeContext context, FunctionCode code)
  {
    this.Context = context;
    this.Code = code;
    this.Frame = (TraceBackFrame) null;
  }

  internal FunctionStack(CodeContext context, FunctionCode code, TraceBackFrame frame)
  {
    this.Context = context;
    this.Code = code;
    this.Frame = frame;
  }

  internal FunctionStack(TraceBackFrame frame)
  {
    this.Context = frame.Context;
    this.Code = frame.f_code;
    this.Frame = frame;
  }
}
