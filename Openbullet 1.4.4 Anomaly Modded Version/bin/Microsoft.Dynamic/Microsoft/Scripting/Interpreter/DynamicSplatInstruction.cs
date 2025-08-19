// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.DynamicSplatInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class DynamicSplatInstruction : Instruction
{
  private readonly CallSite<Func<CallSite, ArgumentArray, object>> _site;
  private readonly int _argumentCount;

  internal DynamicSplatInstruction(
    int argumentCount,
    CallSite<Func<CallSite, ArgumentArray, object>> site)
  {
    this._site = site;
    this._argumentCount = argumentCount;
  }

  public override int ProducedStack => 1;

  public override int ConsumedStack => this._argumentCount;

  public override int Run(InterpretedFrame frame)
  {
    int first = frame.StackIndex - this._argumentCount;
    object obj = this._site.Target((CallSite) this._site, new ArgumentArray(frame.Data, first, this._argumentCount));
    frame.Data[first] = obj;
    frame.StackIndex = first + 1;
    return 1;
  }

  public override string ToString() => $"DynamicSplatInstruction({(object) this._site})";
}
