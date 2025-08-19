// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.LookupGlobalInstruction
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Interpreter;

#nullable disable
namespace IronPython.Compiler;

internal class LookupGlobalInstruction : Instruction
{
  private readonly string _name;
  private readonly bool _isLocal;
  private readonly bool _lightThrow;

  public LookupGlobalInstruction(string name, bool isLocal, bool lightThrow)
  {
    this._name = name;
    this._isLocal = isLocal;
    this._lightThrow = lightThrow;
  }

  public override int ConsumedStack => 1;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    frame.Push(PythonOps.GetVariable((CodeContext) frame.Pop(), this._name, !this._isLocal, this._lightThrow));
    return 1;
  }

  public override string ToString()
  {
    return $"LookupGlobal({this._name}, isLocal={this._isLocal.ToString()})";
  }
}
