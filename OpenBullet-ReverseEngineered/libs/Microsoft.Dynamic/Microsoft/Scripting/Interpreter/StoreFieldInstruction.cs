// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.StoreFieldInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class StoreFieldInstruction : Instruction
{
  private readonly FieldInfo _field;

  public StoreFieldInstruction(FieldInfo field) => this._field = field;

  public override int ConsumedStack => 2;

  public override int ProducedStack => 0;

  public override int Run(InterpretedFrame frame)
  {
    object obj = frame.Pop();
    this._field.SetValue(frame.Pop(), obj);
    return 1;
  }
}
