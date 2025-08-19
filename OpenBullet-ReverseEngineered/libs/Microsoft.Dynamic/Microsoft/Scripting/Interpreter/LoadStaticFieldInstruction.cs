// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LoadStaticFieldInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class LoadStaticFieldInstruction : Instruction
{
  private readonly FieldInfo _field;

  public LoadStaticFieldInstruction(FieldInfo field) => this._field = field;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    frame.Push(this._field.GetValue((object) null));
    return 1;
  }
}
