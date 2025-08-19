// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.PythonReference
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

internal class PythonReference
{
  private readonly string _name;
  private PythonVariable _variable;

  public PythonReference(string name) => this._name = name;

  public string Name => this._name;

  internal PythonVariable PythonVariable
  {
    get => this._variable;
    set => this._variable = value;
  }
}
