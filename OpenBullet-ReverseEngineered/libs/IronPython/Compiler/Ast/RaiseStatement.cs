// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.RaiseStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Compiler.Ast;

public class RaiseStatement : Statement
{
  private readonly Expression _type;
  private readonly Expression _value;
  private readonly Expression _traceback;
  private bool _inFinally;

  public RaiseStatement(Expression exceptionType, Expression exceptionValue, Expression traceBack)
  {
    this._type = exceptionType;
    this._value = exceptionValue;
    this._traceback = traceBack;
  }

  [Obsolete("Type is obsolete due to direct inheritance from DLR Expression.  Use ExceptType instead")]
  public Expression Type => this._type;

  public Expression ExceptType => this._type;

  public Expression Value => this._value;

  public Expression Traceback => this._traceback;

  public override System.Linq.Expressions.Expression Reduce()
  {
    System.Linq.Expressions.Expression expression;
    if (this._type == null && this._value == null && this._traceback == null)
    {
      expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeRethrownException, this.Parent.LocalContext);
      if (!this.InFinally)
        expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(Node.UpdateLineUpdated(true), expression);
    }
    else
      expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeException, this.Parent.LocalContext, Node.TransformOrConstantNull(this._type, typeof (object)), Node.TransformOrConstantNull(this._value, typeof (object)), Node.TransformOrConstantNull(this._traceback, typeof (object)));
    return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Throw(expression), this.Span);
  }

  internal bool InFinally
  {
    get => this._inFinally;
    set => this._inFinally = value;
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._type != null)
        this._type.Walk(walker);
      if (this._value != null)
        this._value.Walk(walker);
      if (this._traceback != null)
        this._traceback.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
