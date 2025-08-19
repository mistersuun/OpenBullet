// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonLightThrowGlobalInstruction
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Interpreter;

#nullable disable
namespace IronPython.Compiler;

internal class PythonLightThrowGlobalInstruction(PythonGlobal global) : PythonGlobalInstruction(global)
{
  public override int Run(InterpretedFrame frame)
  {
    frame.Push(this._global.CurrentValueLightThrow);
    return 1;
  }
}
