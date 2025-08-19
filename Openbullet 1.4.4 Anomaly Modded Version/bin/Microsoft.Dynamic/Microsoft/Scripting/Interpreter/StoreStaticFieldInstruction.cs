// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.StoreStaticFieldInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class StoreStaticFieldInstruction : Instruction
{
  private readonly FieldInfo _field;

  public StoreStaticFieldInstruction(FieldInfo field) => this._field = field;

  public override int ConsumedStack => 1;

  public override int ProducedStack => 0;

  public override int Run(InterpretedFrame frame)
  {
    this._field.SetValue((object) null, frame.Pop());
    return 1;
  }
}
