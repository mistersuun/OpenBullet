// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.CreateDelegateInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class CreateDelegateInstruction : Instruction
{
  private readonly LightDelegateCreator _creator;

  internal CreateDelegateInstruction(LightDelegateCreator delegateCreator)
  {
    this._creator = delegateCreator;
  }

  public override int ConsumedStack => this._creator.Interpreter.ClosureSize;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    StrongBox<object>[] closure;
    if (this.ConsumedStack > 0)
    {
      closure = new StrongBox<object>[this.ConsumedStack];
      for (int index = closure.Length - 1; index >= 0; --index)
        closure[index] = (StrongBox<object>) frame.Pop();
    }
    else
      closure = (StrongBox<object>[]) null;
    Delegate @delegate = this._creator.CreateDelegate(closure);
    frame.Push((object) @delegate);
    return 1;
  }
}
