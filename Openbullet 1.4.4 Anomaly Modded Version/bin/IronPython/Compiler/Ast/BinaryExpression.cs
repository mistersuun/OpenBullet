// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.BinaryExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

#nullable disable
namespace IronPython.Compiler.Ast;

public class BinaryExpression : Expression, IInstructionProvider
{
  private readonly Expression _left;
  private readonly Expression _right;
  private readonly PythonOperator _op;
  private const int MaximumInlineStringLength = 1048576 /*0x100000*/;

  public BinaryExpression(PythonOperator op, Expression left, Expression right)
  {
    ContractUtils.RequiresNotNull((object) left, nameof (left));
    ContractUtils.RequiresNotNull((object) right, nameof (right));
    this._op = op != PythonOperator.None ? op : throw new ValueErrorException("bad operator");
    this._left = left;
    this._right = right;
    this.StartIndex = left.StartIndex;
    this.EndIndex = right.EndIndex;
  }

  public Expression Left => this._left;

  public Expression Right => this._right;

  public PythonOperator Operator => this._op;

  private bool IsComparison()
  {
    switch (this._op)
    {
      case PythonOperator.LessThan:
      case PythonOperator.LessThanOrEqual:
      case PythonOperator.GreaterThan:
      case PythonOperator.GreaterThanOrEqual:
      case PythonOperator.Equal:
      case PythonOperator.NotEqual:
      case PythonOperator.In:
      case PythonOperator.NotIn:
      case PythonOperator.IsNot:
      case PythonOperator.Is:
        return true;
      default:
        return false;
    }
  }

  private bool NeedComparisonTransformation()
  {
    return this.IsComparison() && BinaryExpression.IsComparison(this._right);
  }

  public static bool IsComparison(Expression expression)
  {
    return expression is BinaryExpression binaryExpression && binaryExpression.IsComparison();
  }

  private System.Linq.Expressions.Expression FinishCompare(System.Linq.Expressions.Expression left)
  {
    BinaryExpression right1 = (BinaryExpression) this._right;
    System.Linq.Expressions.Expression left1 = (System.Linq.Expressions.Expression) right1.Left;
    ParameterExpression left2 = System.Linq.Expressions.Expression.Parameter(typeof (object), "chained_comparison");
    System.Linq.Expressions.Expression left3 = this.MakeBinaryOperation(this._op, left, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) left2, Microsoft.Scripting.Ast.Utils.Convert(left1, left2.Type)), this.Span);
    System.Linq.Expressions.Expression expression1;
    if (BinaryExpression.IsComparison(right1._right))
    {
      expression1 = right1.FinishCompare((System.Linq.Expressions.Expression) left2);
    }
    else
    {
      System.Linq.Expressions.Expression right2 = (System.Linq.Expressions.Expression) right1.Right;
      expression1 = this.MakeBinaryOperation(right1.Operator, (System.Linq.Expressions.Expression) left2, right2, right1.Span);
    }
    System.Linq.Expressions.Expression right3 = expression1;
    MethodInfo isTrue = AstMethods.IsTrue;
    ParameterExpression parameterExpression;
    ref ParameterExpression local = ref parameterExpression;
    System.Linq.Expressions.Expression expression2 = Microsoft.Scripting.Ast.Utils.CoalesceTrue(left3, right3, isTrue, out local);
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
    {
      left2,
      parameterExpression
    }, expression2);
  }

  public override System.Linq.Expressions.Expression Reduce()
  {
    if (!this.CanEmitWarning(this._op))
    {
      ConstantExpression constantExpression = this.ConstantFold();
      if (constantExpression != null)
      {
        constantExpression.Parent = this.Parent;
        return Microsoft.Scripting.Ast.Utils.Convert(constantExpression.Reduce(), typeof (object));
      }
    }
    if (this._op == PythonOperator.Mod && this._left is ConstantExpression left && left.Value is string)
      return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(!left.IsUnicodeString ? AstMethods.FormatString : AstMethods.FormatUnicode, this.Parent.LocalContext, (System.Linq.Expressions.Expression) this._left, Microsoft.Scripting.Ast.Utils.Convert((System.Linq.Expressions.Expression) this._right, typeof (object)));
    return this.NeedComparisonTransformation() ? this.FinishCompare((System.Linq.Expressions.Expression) this._left) : this.MakeBinaryOperation(this._op, (System.Linq.Expressions.Expression) this._left, (System.Linq.Expressions.Expression) this._right, this.Span);
  }

  void IInstructionProvider.AddInstructions(LightCompiler compiler)
  {
    if (this.NeedComparisonTransformation())
    {
      compiler.Compile(this.Reduce());
    }
    else
    {
      switch (this._op)
      {
        case PythonOperator.IsNot:
          compiler.Compile((System.Linq.Expressions.Expression) this._left);
          compiler.Compile((System.Linq.Expressions.Expression) this._right);
          compiler.Instructions.Emit((Instruction) BinaryExpression.IsNotInstruction.Instance);
          break;
        case PythonOperator.Is:
          compiler.Compile((System.Linq.Expressions.Expression) this._left);
          compiler.Compile((System.Linq.Expressions.Expression) this._right);
          compiler.Instructions.Emit((Instruction) BinaryExpression.IsInstruction.Instance);
          break;
        default:
          compiler.Compile(this.Reduce());
          break;
      }
    }
  }

  public override string NodeName => !this.IsComparison() ? "operator" : "comparison";

  private System.Linq.Expressions.Expression MakeBinaryOperation(
    PythonOperator op,
    System.Linq.Expressions.Expression left,
    System.Linq.Expressions.Expression right,
    SourceSpan span)
  {
    if (op == PythonOperator.NotIn)
      return Microsoft.Scripting.Ast.Utils.Convert((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Not(this.GlobalParent.Operation(typeof (bool), PythonOperationKind.Contains, left, right)), typeof (object));
    if (op == PythonOperator.In)
      return Microsoft.Scripting.Ast.Utils.Convert(this.GlobalParent.Operation(typeof (bool), PythonOperationKind.Contains, left, right), typeof (object));
    PythonOperationKind action = BinaryExpression.PythonOperatorToAction(op);
    if (action == PythonOperationKind.None)
      return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(BinaryExpression.GetHelperMethod(op), Node.ConvertIfNeeded(left, typeof (object)), Node.ConvertIfNeeded(right, typeof (object)));
    if (!this.CanEmitWarning(op))
      return this.GlobalParent.Operation(typeof (object), action, left, right);
    ParameterExpression left1 = System.Linq.Expressions.Expression.Parameter(left.Type, nameof (left));
    ParameterExpression left2 = System.Linq.Expressions.Expression.Parameter(right.Type, nameof (right));
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
    {
      left1,
      left2
    }, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.WarnDivision, this.Parent.LocalContext, Microsoft.Scripting.Ast.Utils.Constant((object) this.GlobalParent.DivisionOptions), Microsoft.Scripting.Ast.Utils.Convert((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) left1, left), typeof (object)), Microsoft.Scripting.Ast.Utils.Convert((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) left2, right), typeof (object))), this.GlobalParent.Operation(typeof (object), action, (System.Linq.Expressions.Expression) left1, (System.Linq.Expressions.Expression) left2));
  }

  private bool CanEmitWarning(PythonOperator op)
  {
    if (op != PythonOperator.Divide)
      return false;
    return this.GlobalParent.DivisionOptions == PythonDivisionOptions.Warn || this.GlobalParent.DivisionOptions == PythonDivisionOptions.WarnAll;
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      this._left.Walk(walker);
      this._right.Walk(walker);
    }
    walker.PostWalk(this);
  }

  private static PythonOperationKind PythonOperatorToAction(PythonOperator op)
  {
    switch (op)
    {
      case PythonOperator.Add:
        return PythonOperationKind.Add;
      case PythonOperator.Subtract:
        return PythonOperationKind.Subtract;
      case PythonOperator.Multiply:
        return PythonOperationKind.Multiply;
      case PythonOperator.Divide:
        return PythonOperationKind.Divide;
      case PythonOperator.TrueDivide:
        return PythonOperationKind.TrueDivide;
      case PythonOperator.Mod:
        return PythonOperationKind.Mod;
      case PythonOperator.BitwiseAnd:
        return PythonOperationKind.BitwiseAnd;
      case PythonOperator.BitwiseOr:
        return PythonOperationKind.BitwiseOr;
      case PythonOperator.Xor:
        return PythonOperationKind.ExclusiveOr;
      case PythonOperator.LeftShift:
        return PythonOperationKind.LeftShift;
      case PythonOperator.RightShift:
        return PythonOperationKind.RightShift;
      case PythonOperator.Power:
        return PythonOperationKind.Power;
      case PythonOperator.FloorDivide:
        return PythonOperationKind.FloorDivide;
      case PythonOperator.LessThan:
        return PythonOperationKind.LessThan;
      case PythonOperator.LessThanOrEqual:
        return PythonOperationKind.LessThanOrEqual;
      case PythonOperator.GreaterThan:
        return PythonOperationKind.GreaterThan;
      case PythonOperator.GreaterThanOrEqual:
        return PythonOperationKind.GreaterThanOrEqual;
      case PythonOperator.Equal:
        return PythonOperationKind.Equal;
      case PythonOperator.NotEqual:
        return PythonOperationKind.NotEqual;
      case PythonOperator.In:
        return PythonOperationKind.Contains;
      case PythonOperator.NotIn:
      case PythonOperator.IsNot:
      case PythonOperator.Is:
        return PythonOperationKind.None;
      default:
        return PythonOperationKind.None;
    }
  }

  private static MethodInfo GetHelperMethod(PythonOperator op)
  {
    if (op == PythonOperator.IsNot)
      return AstMethods.IsNot;
    return op == PythonOperator.Is ? AstMethods.Is : (MethodInfo) null;
  }

  internal override bool CanThrow
  {
    get
    {
      return this._op != PythonOperator.Is && this._op != PythonOperator.IsNot || this._left.CanThrow || this._right.CanThrow;
    }
  }

  internal override ConstantExpression ConstantFold()
  {
    ConstantExpression constantExpression1 = this._left.ConstantFold();
    Expression expression = (Expression) this._right.ConstantFold();
    ConstantExpression constantExpression2 = constantExpression1 as ConstantExpression;
    ConstantExpression constantExpression3 = expression as ConstantExpression;
    try
    {
      if (constantExpression2 != null && constantExpression3 != null && constantExpression2.Value != null && constantExpression3.Value != null && constantExpression2.Value.GetType() == constantExpression3.Value.GetType())
      {
        if (constantExpression2.Value.GetType() == typeof (int))
        {
          switch (this._op)
          {
            case PythonOperator.Add:
              return new ConstantExpression(Int32Ops.Add((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.Subtract:
              return new ConstantExpression(Int32Ops.Subtract((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.Multiply:
              return new ConstantExpression(Int32Ops.Multiply((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.Divide:
              return new ConstantExpression(Int32Ops.Divide((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.TrueDivide:
              return new ConstantExpression((object) Int32Ops.TrueDivide((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.Mod:
              return new ConstantExpression((object) Int32Ops.Mod((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.BitwiseAnd:
              return new ConstantExpression((object) Int32Ops.BitwiseAnd((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.BitwiseOr:
              return new ConstantExpression((object) Int32Ops.BitwiseOr((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.Xor:
              return new ConstantExpression((object) Int32Ops.ExclusiveOr((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.LeftShift:
              return new ConstantExpression(Int32Ops.LeftShift((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.RightShift:
              return new ConstantExpression((object) Int32Ops.RightShift((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.Power:
              return new ConstantExpression(Int32Ops.Power((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.FloorDivide:
              return new ConstantExpression(Int32Ops.FloorDivide((int) constantExpression2.Value, (int) constantExpression3.Value));
            case PythonOperator.LessThan:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(Int32Ops.Compare((int) constantExpression2.Value, (int) constantExpression3.Value) < 0));
            case PythonOperator.LessThanOrEqual:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(Int32Ops.Compare((int) constantExpression2.Value, (int) constantExpression3.Value) <= 0));
            case PythonOperator.GreaterThan:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(Int32Ops.Compare((int) constantExpression2.Value, (int) constantExpression3.Value) > 0));
            case PythonOperator.GreaterThanOrEqual:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(Int32Ops.Compare((int) constantExpression2.Value, (int) constantExpression3.Value) >= 0));
            case PythonOperator.Equal:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(Int32Ops.Compare((int) constantExpression2.Value, (int) constantExpression3.Value) == 0));
            case PythonOperator.NotEqual:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(Int32Ops.Compare((int) constantExpression2.Value, (int) constantExpression3.Value) != 0));
          }
        }
        if (constantExpression2.Value.GetType() == typeof (double))
        {
          switch (this._op)
          {
            case PythonOperator.Add:
              return new ConstantExpression((object) DoubleOps.Add((double) constantExpression2.Value, (double) constantExpression3.Value));
            case PythonOperator.Subtract:
              return new ConstantExpression((object) DoubleOps.Subtract((double) constantExpression2.Value, (double) constantExpression3.Value));
            case PythonOperator.Multiply:
              return new ConstantExpression((object) DoubleOps.Multiply((double) constantExpression2.Value, (double) constantExpression3.Value));
            case PythonOperator.Divide:
              return new ConstantExpression((object) DoubleOps.Divide((double) constantExpression2.Value, (double) constantExpression3.Value));
            case PythonOperator.TrueDivide:
              return new ConstantExpression((object) DoubleOps.TrueDivide((double) constantExpression2.Value, (double) constantExpression3.Value));
            case PythonOperator.Mod:
              return new ConstantExpression((object) DoubleOps.Mod((double) constantExpression2.Value, (double) constantExpression3.Value));
            case PythonOperator.Power:
              return new ConstantExpression((object) DoubleOps.Power((double) constantExpression2.Value, (double) constantExpression3.Value));
            case PythonOperator.FloorDivide:
              return new ConstantExpression((object) DoubleOps.FloorDivide((double) constantExpression2.Value, (double) constantExpression3.Value));
            case PythonOperator.LessThan:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(DoubleOps.Compare((double) constantExpression2.Value, (double) constantExpression3.Value) < 0));
            case PythonOperator.LessThanOrEqual:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(DoubleOps.Compare((double) constantExpression2.Value, (double) constantExpression3.Value) <= 0));
            case PythonOperator.GreaterThan:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(DoubleOps.Compare((double) constantExpression2.Value, (double) constantExpression3.Value) > 0));
            case PythonOperator.GreaterThanOrEqual:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(DoubleOps.Compare((double) constantExpression2.Value, (double) constantExpression3.Value) >= 0));
            case PythonOperator.Equal:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(DoubleOps.Compare((double) constantExpression2.Value, (double) constantExpression3.Value) == 0));
            case PythonOperator.NotEqual:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(DoubleOps.Compare((double) constantExpression2.Value, (double) constantExpression3.Value) != 0));
          }
        }
        if (constantExpression2.Value.GetType() == typeof (BigInteger))
        {
          switch (this._op)
          {
            case PythonOperator.Add:
              return new ConstantExpression((object) BigIntegerOps.Add((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.Subtract:
              return new ConstantExpression((object) BigIntegerOps.Subtract((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.Multiply:
              return new ConstantExpression((object) BigIntegerOps.Multiply((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.Divide:
              return new ConstantExpression((object) BigIntegerOps.Divide((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.TrueDivide:
              return new ConstantExpression((object) BigIntegerOps.TrueDivide((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.Mod:
              return new ConstantExpression((object) BigIntegerOps.Mod((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.BitwiseAnd:
              return new ConstantExpression((object) BigIntegerOps.BitwiseAnd((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.BitwiseOr:
              return new ConstantExpression((object) BigIntegerOps.BitwiseOr((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.Xor:
              return new ConstantExpression((object) BigIntegerOps.ExclusiveOr((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.LeftShift:
              return new ConstantExpression((object) BigIntegerOps.LeftShift((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.RightShift:
              return new ConstantExpression((object) BigIntegerOps.RightShift((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.Power:
              return new ConstantExpression(BigIntegerOps.Power((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.FloorDivide:
              return new ConstantExpression((object) BigIntegerOps.FloorDivide((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value));
            case PythonOperator.LessThan:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(BigIntegerOps.Compare((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value) < 0));
            case PythonOperator.LessThanOrEqual:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(BigIntegerOps.Compare((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value) <= 0));
            case PythonOperator.GreaterThan:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(BigIntegerOps.Compare((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value) > 0));
            case PythonOperator.GreaterThanOrEqual:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(BigIntegerOps.Compare((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value) >= 0));
            case PythonOperator.Equal:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(BigIntegerOps.Compare((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value) == 0));
            case PythonOperator.NotEqual:
              return new ConstantExpression(ScriptingRuntimeHelpers.BooleanToObject(BigIntegerOps.Compare((BigInteger) constantExpression2.Value, (BigInteger) constantExpression3.Value) != 0));
          }
        }
        if (constantExpression2.Value.GetType() == typeof (Complex))
        {
          switch (this._op)
          {
            case PythonOperator.Add:
              return new ConstantExpression((object) ComplexOps.Add((Complex) constantExpression2.Value, (Complex) constantExpression3.Value));
            case PythonOperator.Subtract:
              return new ConstantExpression((object) ComplexOps.Subtract((Complex) constantExpression2.Value, (Complex) constantExpression3.Value));
            case PythonOperator.Multiply:
              return new ConstantExpression((object) ComplexOps.Multiply((Complex) constantExpression2.Value, (Complex) constantExpression3.Value));
            case PythonOperator.Divide:
              return new ConstantExpression((object) ComplexOps.Divide((Complex) constantExpression2.Value, (Complex) constantExpression3.Value));
            case PythonOperator.TrueDivide:
              return new ConstantExpression((object) ComplexOps.TrueDivide((Complex) constantExpression2.Value, (Complex) constantExpression3.Value));
            case PythonOperator.Power:
              return new ConstantExpression((object) ComplexOps.Power((Complex) constantExpression2.Value, (Complex) constantExpression3.Value));
          }
        }
        if (constantExpression2.Value.GetType() == typeof (string))
        {
          if (this._op == PythonOperator.Add)
            return new ConstantExpression((object) ((string) constantExpression2.Value + (string) constantExpression3.Value));
        }
      }
      else if (this._op == PythonOperator.Multiply)
      {
        if (constantExpression2 != null)
        {
          if (constantExpression3 != null)
          {
            if (constantExpression2.Value.GetType() == typeof (string) && constantExpression3.Value.GetType() == typeof (int))
            {
              string str = StringOps.Multiply((string) constantExpression2.Value, (int) constantExpression3.Value);
              if (str.Length < 1048576 /*0x100000*/)
                return new ConstantExpression((object) str);
            }
            else if (constantExpression2.Value.GetType() == typeof (int))
            {
              if (constantExpression3.Value.GetType() == typeof (string))
              {
                string str = StringOps.Multiply((string) constantExpression3.Value, (int) constantExpression2.Value);
                if (str.Length < 1048576 /*0x100000*/)
                  return new ConstantExpression((object) str);
              }
            }
          }
        }
      }
    }
    catch (ArithmeticException ex)
    {
    }
    return (ConstantExpression) null;
  }

  private abstract class BinaryInstruction : Instruction
  {
    public override int ConsumedStack => 2;

    public override int ProducedStack => 1;
  }

  private class IsInstruction : BinaryExpression.BinaryInstruction
  {
    public static readonly BinaryExpression.IsInstruction Instance = new BinaryExpression.IsInstruction();

    public override int Run(InterpretedFrame frame)
    {
      frame.Push(PythonOps.Is(frame.Pop(), frame.Pop()));
      return 1;
    }
  }

  private class IsNotInstruction : BinaryExpression.BinaryInstruction
  {
    public static readonly BinaryExpression.IsNotInstruction Instance = new BinaryExpression.IsNotInstruction();

    public override int Run(InterpretedFrame frame)
    {
      frame.Push(PythonOps.IsNot(frame.Pop(), frame.Pop()));
      return 1;
    }
  }
}
