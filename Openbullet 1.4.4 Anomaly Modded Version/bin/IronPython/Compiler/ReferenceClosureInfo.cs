// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.ReferenceClosureInfo
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;

#nullable disable
namespace IronPython.Compiler;

internal class ReferenceClosureInfo
{
  public readonly PythonVariable Variable;
  public bool IsClosedOver;

  public ReferenceClosureInfo(
    PythonVariable variable,
    int index,
    System.Linq.Expressions.Expression tupleExpr,
    bool accessedInThisScope)
  {
    this.Variable = variable;
    this.IsClosedOver = accessedInThisScope;
  }

  public PythonVariable PythonVariable => this.Variable;
}
