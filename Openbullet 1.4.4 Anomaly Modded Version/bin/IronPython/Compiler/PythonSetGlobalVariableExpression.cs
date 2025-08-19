// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonSetGlobalVariableExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Interpreter;
using System;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler;

internal class PythonSetGlobalVariableExpression : Expression, IInstructionProvider
{
  private readonly PythonGlobalVariableExpression _global;
  private readonly Expression _value;

  public PythonSetGlobalVariableExpression(PythonGlobalVariableExpression global, Expression value)
  {
    this._global = global;
    this._value = value;
  }

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public sealed override Type Type => typeof (object);

  public Expression Value => this._value;

  public override bool CanReduce => true;

  public PythonGlobalVariableExpression Global => this._global;

  public override Expression Reduce()
  {
    return (Expression) Expression.Assign((Expression) Expression.Property(this._global.Target, typeof (PythonGlobal).GetProperty("CurrentValue")), Utils.Convert(this._value, typeof (object)));
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression expression = visitor.Visit(this._value);
    return expression == this._value ? (Expression) this : (Expression) new PythonSetGlobalVariableExpression(this._global, expression);
  }

  public void AddInstructions(LightCompiler compiler)
  {
    compiler.Compile(this._value);
    compiler.Instructions.Emit((Instruction) new PythonSetGlobalInstruction(this._global.Global));
  }
}
