// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LoadObjectInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class LoadObjectInstruction : Instruction
{
  private readonly object _value;

  internal LoadObjectInstruction(object value) => this._value = value;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    frame.Data[frame.StackIndex++] = this._value;
    return 1;
  }

  public override string ToString() => $"LoadObject({this._value ?? (object) "null"})";
}
