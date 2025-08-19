// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ParameterBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class ParameterBinder : PythonWalkerNonRecursive
{
  private PythonNameBinder _binder;

  public ParameterBinder(PythonNameBinder binder) => this._binder = binder;

  public override bool Walk(Parameter node)
  {
    node.Parent = this._binder._currentScope;
    node.PythonVariable = this._binder.DefineParameter(node.Name);
    return false;
  }

  public override bool Walk(SublistParameter node)
  {
    node.PythonVariable = this._binder.DefineParameter(node.Name);
    node.Parent = this._binder._currentScope;
    this.WalkTuple(node.Tuple);
    return false;
  }

  private void WalkTuple(TupleExpression tuple)
  {
    tuple.Parent = this._binder._currentScope;
    foreach (Expression tuple1 in (IEnumerable<Expression>) tuple.Items)
    {
      if (tuple1 is NameExpression nameExpression)
      {
        this._binder.DefineName(nameExpression.Name);
        nameExpression.Parent = this._binder._currentScope;
        nameExpression.Reference = this._binder.Reference(nameExpression.Name);
      }
      else
        this.WalkTuple((TupleExpression) tuple1);
    }
  }

  public override bool Walk(TupleExpression node)
  {
    node.Parent = this._binder._currentScope;
    return true;
  }
}
