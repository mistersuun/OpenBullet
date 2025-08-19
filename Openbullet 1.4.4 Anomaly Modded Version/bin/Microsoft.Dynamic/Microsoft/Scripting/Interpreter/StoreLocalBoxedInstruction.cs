// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.StoreLocalBoxedInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class StoreLocalBoxedInstruction : LocalAccessInstruction
{
  internal StoreLocalBoxedInstruction(int index)
    : base(index)
  {
  }

  public override int ConsumedStack => 1;

  public override int ProducedStack => 0;

  public override int Run(InterpretedFrame frame)
  {
    ((StrongBox<object>) frame.Data[this._index]).Value = frame.Data[--frame.StackIndex];
    return 1;
  }
}
