// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.FlowDefiner
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

internal class FlowDefiner : PythonWalkerNonRecursive
{
  private readonly FlowChecker _fc;

  public FlowDefiner(FlowChecker fc) => this._fc = fc;

  public override bool Walk(NameExpression node)
  {
    this._fc.Define(node.Name);
    return false;
  }

  public override bool Walk(MemberExpression node)
  {
    node.Walk((PythonWalker) this._fc);
    return false;
  }

  public override bool Walk(IndexExpression node)
  {
    node.Walk((PythonWalker) this._fc);
    return false;
  }

  public override bool Walk(ParenthesisExpression node) => true;

  public override bool Walk(TupleExpression node) => true;

  public override bool Walk(ListExpression node) => true;

  public override bool Walk(Parameter node)
  {
    this._fc.Define(node.Name);
    return true;
  }

  public override bool Walk(SublistParameter node) => true;
}
