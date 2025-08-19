// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.Instruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public abstract class Instruction
{
  public virtual int ConsumedStack => 0;

  public virtual int ProducedStack => 0;

  public virtual int ConsumedContinuations => 0;

  public virtual int ProducedContinuations => 0;

  public int StackBalance => this.ProducedStack - this.ConsumedStack;

  public int ContinuationsBalance => this.ProducedContinuations - this.ConsumedContinuations;

  public abstract int Run(InterpretedFrame frame);

  public virtual string InstructionName
  {
    get => this.GetType().Name.Replace(nameof (Instruction), string.Empty);
  }

  public override string ToString() => this.InstructionName + "()";

  public virtual string ToDebugString(
    int instructionIndex,
    object cookie,
    Func<int, int> labelIndexer,
    IList<object> objects)
  {
    return this.ToString();
  }

  public virtual object GetDebugCookie(LightCompiler compiler) => (object) null;
}
