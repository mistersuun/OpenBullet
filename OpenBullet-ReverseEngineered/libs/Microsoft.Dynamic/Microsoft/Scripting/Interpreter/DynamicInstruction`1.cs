// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.DynamicInstruction`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal class DynamicInstruction<TRet> : Instruction
{
  private CallSite<Func<CallSite, TRet>> _site;

  public static Instruction Factory(CallSiteBinder binder)
  {
    return (Instruction) new DynamicInstruction<TRet>(CallSite<Func<CallSite, TRet>>.Create(binder));
  }

  private DynamicInstruction(CallSite<Func<CallSite, TRet>> site) => this._site = site;

  public override int ProducedStack => 1;

  public override int ConsumedStack => 0;

  public override int Run(InterpretedFrame frame)
  {
    frame.Data[frame.StackIndex] = (object) this._site.Target((CallSite) this._site);
    frame.StackIndex -= -1;
    return 1;
  }

  public override string ToString() => $"Dynamic({this._site.Binder.ToString()})";
}
