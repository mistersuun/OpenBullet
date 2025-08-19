// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.LookupGlobalVariable
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Interpreter;
using System;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler;

internal class LookupGlobalVariable : 
  Expression,
  IInstructionProvider,
  IPythonGlobalExpression,
  IPythonVariableExpression,
  ILightExceptionAwareExpression
{
  private readonly string _name;
  private readonly bool _isLocal;
  private readonly bool _lightThrow;
  private readonly Expression _codeContextExpr;

  public LookupGlobalVariable(Expression codeContextExpr, string name, bool isLocal)
    : this(codeContextExpr, name, isLocal, false)
  {
  }

  public LookupGlobalVariable(
    Expression codeContextExpr,
    string name,
    bool isLocal,
    bool lightThrow)
  {
    this._name = name;
    this._isLocal = isLocal;
    this._codeContextExpr = codeContextExpr;
    this._lightThrow = lightThrow;
  }

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public sealed override Type Type => typeof (object);

  public override bool CanReduce => true;

  protected override Expression VisitChildren(ExpressionVisitor visitor) => (Expression) this;

  public Expression RawValue()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod(this._isLocal ? "RawGetLocal" : "RawGetGlobal"), this._codeContextExpr, Utils.Constant((object) this._name));
  }

  public override Expression Reduce()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod(this._isLocal ? "GetLocal" : "GetGlobal"), this._codeContextExpr, Utils.Constant((object) this._name));
  }

  public Expression Assign(Expression value)
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod(this._isLocal ? "SetLocal" : "SetGlobal"), this._codeContextExpr, Utils.Constant((object) this._name), value);
  }

  public Expression Create() => (Expression) null;

  public bool IsLocal => this._isLocal;

  public Expression CodeContext => this._codeContextExpr;

  public string Name => this._name;

  public Expression Delete()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod(this._isLocal ? "DeleteLocal" : "DeleteGlobal"), this._codeContextExpr, Utils.Constant((object) this._name));
  }

  void IInstructionProvider.AddInstructions(LightCompiler compiler)
  {
    compiler.Compile(this._codeContextExpr);
    compiler.Instructions.Emit((Instruction) new LookupGlobalInstruction(this._name, this._isLocal, this._lightThrow));
  }

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    return this._lightThrow ? (Expression) this : (Expression) new LookupGlobalVariable(this._codeContextExpr, this._name, this._isLocal, true);
  }
}
