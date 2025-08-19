// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonSetGlobalInstruction
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Interpreter;

#nullable disable
namespace IronPython.Compiler;

internal class PythonSetGlobalInstruction : Instruction
{
  private readonly PythonGlobal _global;

  public PythonSetGlobalInstruction(PythonGlobal global) => this._global = global;

  public override int ProducedStack => 1;

  public override int ConsumedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    this._global.CurrentValue = frame.Peek();
    return 1;
  }

  public override string ToString() => $"SetGlobal({(object) this._global})";
}
