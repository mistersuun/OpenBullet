// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.DynamicInstruction`8
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal class DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, TRet> : Instruction
{
  private CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>> _site;

  public static Instruction Factory(CallSiteBinder binder)
  {
    return (Instruction) new DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, TRet>(CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>>.Create(binder));
  }

  private DynamicInstruction(
    CallSite<Func<CallSite, T0, T1, T2, T3, T4, T5, T6, TRet>> site)
  {
    this._site = site;
  }

  public override int ProducedStack => 1;

  public override int ConsumedStack => 7;

  public override int Run(InterpretedFrame frame)
  {
    frame.Data[frame.StackIndex - 7] = (object) this._site.Target((CallSite) this._site, (T0) frame.Data[frame.StackIndex - 7], (T1) frame.Data[frame.StackIndex - 6], (T2) frame.Data[frame.StackIndex - 5], (T3) frame.Data[frame.StackIndex - 4], (T4) frame.Data[frame.StackIndex - 3], (T5) frame.Data[frame.StackIndex - 2], (T6) frame.Data[frame.StackIndex - 1]);
    frame.StackIndex -= 6;
    return 1;
  }

  public override string ToString() => $"Dynamic({this._site.Binder.ToString()})";
}
