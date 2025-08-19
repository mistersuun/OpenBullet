// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ForStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ForStatement : Statement, ILoopStatement
{
  private int _headerIndex;
  private readonly Expression _left;
  private Expression _list;
  private Statement _body;
  private readonly Statement _else;
  private LabelTarget _break;
  private LabelTarget _continue;

  public ForStatement(Expression left, Expression list, Statement body, Statement else_)
  {
    this._left = left;
    this._list = list;
    this._body = body;
    this._else = else_;
  }

  public int HeaderIndex
  {
    set => this._headerIndex = value;
  }

  public Expression Left => this._left;

  public Statement Body
  {
    get => this._body;
    set => this._body = value;
  }

  public Expression List
  {
    get => this._list;
    set => this._list = value;
  }

  public Statement Else => this._else;

  LabelTarget ILoopStatement.BreakLabel
  {
    get => this._break;
    set => this._break = value;
  }

  LabelTarget ILoopStatement.ContinueLabel
  {
    get => this._continue;
    set => this._continue = value;
  }

  public override System.Linq.Expressions.Expression Reduce()
  {
    ParameterExpression enumerator = System.Linq.Expressions.Expression.Variable(typeof (KeyValuePair<IEnumerator, IDisposable>), "foreach_enumerator");
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      enumerator
    }, ForStatement.TransformFor(this.Parent, enumerator, this._list, this._left, (System.Linq.Expressions.Expression) this._body, this._else, this.Span, this.GlobalParent.IndexToLocation(this._headerIndex), this._break, this._continue, true));
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._left != null)
        this._left.Walk(walker);
      if (this._list != null)
        this._list.Walk(walker);
      if (this._body != null)
        this._body.Walk(walker);
      if (this._else != null)
        this._else.Walk(walker);
    }
    walker.PostWalk(this);
  }

  internal static System.Linq.Expressions.Expression TransformFor(
    ScopeStatement parent,
    ParameterExpression enumerator,
    Expression list,
    Expression left,
    System.Linq.Expressions.Expression body,
    Statement else_,
    SourceSpan span,
    SourceLocation header,
    LabelTarget breakLabel,
    LabelTarget continueLabel,
    bool isStatement)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) enumerator, (System.Linq.Expressions.Expression) new PythonDynamicExpression1<KeyValuePair<IEnumerator, IDisposable>>((CallSiteBinder) Binders.UnaryOperationBinder(parent.GlobalParent.PyContext, PythonOperationKind.GetEnumeratorForIteration), parent.GlobalParent.CompilationMode, Utils.Convert((System.Linq.Expressions.Expression) list, typeof (object)))), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.TryFinally((System.Linq.Expressions.Expression) Utils.Loop(parent.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Property((System.Linq.Expressions.Expression) enumerator, typeof (KeyValuePair<IEnumerator, IDisposable>).GetProperty("Key")), typeof (IEnumerator).GetMethod("MoveNext")), left.Span), (System.Linq.Expressions.Expression) null, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(left.TransformSet(SourceSpan.None, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Property((System.Linq.Expressions.Expression) enumerator, typeof (KeyValuePair<IEnumerator, IDisposable>).GetProperty("Key")), typeof (IEnumerator).GetProperty("Current").GetGetMethod()), PythonOperationKind.None), body, isStatement ? Node.UpdateLineNumber(parent.GlobalParent.IndexToLocation(list.StartIndex).Line) : (System.Linq.Expressions.Expression) Utils.Empty(), (System.Linq.Expressions.Expression) Utils.Empty()), (System.Linq.Expressions.Expression) else_, breakLabel, continueLabel), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.ForLoopDispose, (System.Linq.Expressions.Expression) enumerator), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) enumerator, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.New(typeof (KeyValuePair<IEnumerator, IDisposable>))))));
  }

  internal override bool CanThrow
  {
    get
    {
      return this._left.CanThrow || this._list.CanThrow || this._list is ConstantExpression list && !(list.Value is string);
    }
  }
}
