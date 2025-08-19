// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.TypeAsInstruction`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class TypeAsInstruction<T> : Instruction
{
  public override int ConsumedStack => 1;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    object obj = frame.Pop();
    if (obj is T)
      frame.Push(obj);
    else
      frame.Push((object) null);
    return 1;
  }

  public override string ToString() => "TypeAs " + typeof (T).Name;
}
