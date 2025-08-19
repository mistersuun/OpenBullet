// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.ClosureExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace IronPython.Compiler;

internal class ClosureExpression : System.Linq.Expressions.Expression, IPythonVariableExpression
{
  private readonly System.Linq.Expressions.Expression _closureCell;
  private readonly ParameterExpression _parameter;
  private readonly PythonVariable _variable;
  internal static readonly FieldInfo _cellField = typeof (IronPython.Runtime.ClosureCell).GetField("Value");

  public ClosureExpression(
    PythonVariable variable,
    System.Linq.Expressions.Expression closureCell,
    ParameterExpression parameter)
  {
    this._variable = variable;
    this._closureCell = closureCell;
    this._parameter = parameter;
  }

  public System.Linq.Expressions.Expression ClosureCell => this._closureCell;

  public ParameterExpression OriginalParameter => this._parameter;

  public PythonVariable PythonVariable => this._variable;

  public System.Linq.Expressions.Expression Create()
  {
    return this.OriginalParameter != null ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign(this._closureCell, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeClosureCellWithValue, (System.Linq.Expressions.Expression) this.OriginalParameter)) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign(this._closureCell, (System.Linq.Expressions.Expression) ClosureExpression.MakeClosureCellExpression.Instance);
  }

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public sealed override Type Type => typeof (object);

  public override bool CanReduce => true;

  public string Name => this._variable.Name;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field(this._closureCell, ClosureExpression._cellField);
  }

  public System.Linq.Expressions.Expression Assign(System.Linq.Expressions.Expression value)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field(this._closureCell, ClosureExpression._cellField), value);
  }

  public System.Linq.Expressions.Expression Delete()
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field(this._closureCell, ClosureExpression._cellField), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, typeof (Uninitialized).GetDeclaredField("Instance")));
  }

  private class MakeClosureCellExpression : System.Linq.Expressions.Expression, IInstructionProvider
  {
    private static readonly System.Linq.Expressions.Expression _call = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeClosureCell);
    public static readonly ClosureExpression.MakeClosureCellExpression Instance = new ClosureExpression.MakeClosureCellExpression();

    public override bool CanReduce => true;

    public override ExpressionType NodeType => ExpressionType.Extension;

    public override Type Type => typeof (IronPython.Runtime.ClosureCell);

    public override System.Linq.Expressions.Expression Reduce()
    {
      return ClosureExpression.MakeClosureCellExpression._call;
    }

    public void AddInstructions(LightCompiler compiler)
    {
      compiler.Instructions.Emit((Instruction) ClosureExpression.MakeClosureCellExpression.MakeClosureCellInstruction.Instance);
    }

    private class MakeClosureCellInstruction : Instruction
    {
      public static readonly ClosureExpression.MakeClosureCellExpression.MakeClosureCellInstruction Instance = new ClosureExpression.MakeClosureCellExpression.MakeClosureCellInstruction();

      public override int ProducedStack => 1;

      public override int ConsumedStack => 0;

      public override int Run(InterpretedFrame frame)
      {
        frame.Push((object) PythonOps.MakeClosureCell());
        return 1;
      }
    }
  }
}
