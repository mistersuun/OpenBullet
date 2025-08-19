// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonGlobalVariableExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Interpreter;
using System;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler;

internal class PythonGlobalVariableExpression : 
  System.Linq.Expressions.Expression,
  IInstructionProvider,
  IPythonGlobalExpression,
  IPythonVariableExpression,
  ILightExceptionAwareExpression
{
  private readonly System.Linq.Expressions.Expression _target;
  private readonly PythonGlobal _global;
  private readonly PythonVariable _variable;
  private readonly bool _lightEh;
  internal static System.Linq.Expressions.Expression Uninitialized = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, typeof (Microsoft.Scripting.Runtime.Uninitialized).GetField("Instance"));

  public PythonGlobalVariableExpression(
    System.Linq.Expressions.Expression globalExpr,
    PythonVariable variable,
    PythonGlobal global)
    : this(globalExpr, variable, global, false)
  {
  }

  internal PythonGlobalVariableExpression(
    System.Linq.Expressions.Expression globalExpr,
    PythonVariable variable,
    PythonGlobal global,
    bool lightEh)
  {
    this._target = globalExpr;
    this._global = global;
    this._variable = variable;
    this._lightEh = lightEh;
  }

  public System.Linq.Expressions.Expression Target => this._target;

  public PythonVariable Variable => this._variable;

  public PythonGlobal Global => this._global;

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public sealed override Type Type => typeof (object);

  public override bool CanReduce => true;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Property(this._target, PythonGlobal.CurrentValueProperty);
  }

  public System.Linq.Expressions.Expression RawValue()
  {
    return (System.Linq.Expressions.Expression) new PythonRawGlobalValueExpression(this);
  }

  public System.Linq.Expressions.Expression Assign(System.Linq.Expressions.Expression value)
  {
    return (System.Linq.Expressions.Expression) new PythonSetGlobalVariableExpression(this, value);
  }

  public System.Linq.Expressions.Expression Delete()
  {
    return (System.Linq.Expressions.Expression) new PythonSetGlobalVariableExpression(this, PythonGlobalVariableExpression.Uninitialized);
  }

  public System.Linq.Expressions.Expression Create() => (System.Linq.Expressions.Expression) null;

  protected override System.Linq.Expressions.Expression VisitChildren(ExpressionVisitor visitor)
  {
    System.Linq.Expressions.Expression globalExpr = visitor.Visit(this._target);
    return globalExpr == this._target ? (System.Linq.Expressions.Expression) this : (System.Linq.Expressions.Expression) new PythonGlobalVariableExpression(globalExpr, this._variable, this._global, this._lightEh);
  }

  public void AddInstructions(LightCompiler compiler)
  {
    if (this._lightEh)
      compiler.Instructions.Emit((Instruction) new PythonLightThrowGlobalInstruction(this._global));
    else
      compiler.Instructions.Emit((Instruction) new PythonGlobalInstruction(this._global));
  }

  System.Linq.Expressions.Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    return this._lightEh ? (System.Linq.Expressions.Expression) this : (System.Linq.Expressions.Expression) new PythonGlobalVariableExpression(this._target, this._variable, this._global, true);
  }
}
