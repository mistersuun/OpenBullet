// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.AugmentedAssignStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;

#nullable disable
namespace IronPython.Compiler.Ast;

public class AugmentedAssignStatement : Statement
{
  private readonly PythonOperator _op;
  private readonly Expression _left;
  private readonly Expression _right;

  public AugmentedAssignStatement(PythonOperator op, Expression left, Expression right)
  {
    this._op = op;
    this._left = left;
    this._right = right;
  }

  public PythonOperator Operator => this._op;

  public Expression Left => this._left;

  public Expression Right => this._right;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this._left.TransformSet(this.Span, (System.Linq.Expressions.Expression) this._right, AugmentedAssignStatement.PythonOperatorToAction(this._op));
  }

  private static PythonOperationKind PythonOperatorToAction(PythonOperator op)
  {
    switch (op)
    {
      case PythonOperator.Add:
        return PythonOperationKind.InPlaceAdd;
      case PythonOperator.Subtract:
        return PythonOperationKind.InPlaceSubtract;
      case PythonOperator.Multiply:
        return PythonOperationKind.InPlaceMultiply;
      case PythonOperator.Divide:
        return PythonOperationKind.InPlaceDivide;
      case PythonOperator.TrueDivide:
        return PythonOperationKind.InPlaceTrueDivide;
      case PythonOperator.Mod:
        return PythonOperationKind.InPlaceMod;
      case PythonOperator.BitwiseAnd:
        return PythonOperationKind.InPlaceBitwiseAnd;
      case PythonOperator.BitwiseOr:
        return PythonOperationKind.InPlaceBitwiseOr;
      case PythonOperator.Xor:
        return PythonOperationKind.InPlaceExclusiveOr;
      case PythonOperator.LeftShift:
        return PythonOperationKind.InPlaceLeftShift;
      case PythonOperator.RightShift:
        return PythonOperationKind.InPlaceRightShift;
      case PythonOperator.Power:
        return PythonOperationKind.InPlacePower;
      case PythonOperator.FloorDivide:
        return PythonOperationKind.InPlaceFloorDivide;
      default:
        return PythonOperationKind.None;
    }
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._left != null)
        this._left.Walk(walker);
      if (this._right != null)
        this._right.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
