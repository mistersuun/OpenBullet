// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.WithStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public class WithStatement : Statement
{
  private int _headerIndex;
  private readonly Expression _contextManager;
  private readonly Expression _var;
  private Statement _body;

  public WithStatement(Expression contextManager, Expression var, Statement body)
  {
    this._contextManager = contextManager;
    this._var = var;
    this._body = body;
  }

  public int HeaderIndex
  {
    set => this._headerIndex = value;
  }

  public Expression Variable => this._var;

  public Expression ContextManager => this._contextManager;

  public Statement Body => this._body;

  public override System.Linq.Expressions.Expression Reduce()
  {
    ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression> collectionBuilder1 = new ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression>(6);
    ReadOnlyCollectionBuilder<ParameterExpression> collectionBuilder2 = new ReadOnlyCollectionBuilder<ParameterExpression>(6);
    ParameterExpression saveCurrent = System.Linq.Expressions.Expression.Variable(typeof (bool), "$lineUpdated_with");
    collectionBuilder2.Add(saveCurrent);
    ParameterExpression parameterExpression1 = System.Linq.Expressions.Expression.Variable(typeof (object), "with_manager");
    collectionBuilder2.Add(parameterExpression1);
    collectionBuilder1.Add(this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression1, (System.Linq.Expressions.Expression) this._contextManager), new SourceSpan(this.GlobalParent.IndexToLocation(this.StartIndex), this.GlobalParent.IndexToLocation(this._headerIndex))));
    ParameterExpression parameterExpression2 = System.Linq.Expressions.Expression.Variable(typeof (object), "with_exit");
    collectionBuilder2.Add(parameterExpression2);
    collectionBuilder1.Add(Node.MakeAssignment(parameterExpression2, this.GlobalParent.Get("__exit__", (System.Linq.Expressions.Expression) parameterExpression1)));
    ParameterExpression parameterExpression3 = System.Linq.Expressions.Expression.Variable(typeof (object), "with_value");
    collectionBuilder2.Add(parameterExpression3);
    collectionBuilder1.Add(this.GlobalParent.AddDebugInfoAndVoid(Node.MakeAssignment(parameterExpression3, this.Parent.Invoke(new CallSignature(0), this.Parent.LocalContext, this.GlobalParent.Get("__enter__", (System.Linq.Expressions.Expression) parameterExpression1))), new SourceSpan(this.GlobalParent.IndexToLocation(this.StartIndex), this.GlobalParent.IndexToLocation(this._headerIndex))));
    ParameterExpression parameterExpression4 = System.Linq.Expressions.Expression.Variable(typeof (bool), "with_exc");
    collectionBuilder2.Add(parameterExpression4);
    collectionBuilder1.Add(Node.MakeAssignment(parameterExpression4, Utils.Constant((object) true)));
    ParameterExpression exception;
    collectionBuilder1.Add((System.Linq.Expressions.Expression) Utils.Try((System.Linq.Expressions.Expression) Utils.Try(Node.PushLineUpdated(false, saveCurrent), this._var != null ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(this._var.TransformSet(SourceSpan.None, (System.Linq.Expressions.Expression) parameterExpression3, PythonOperationKind.None), (System.Linq.Expressions.Expression) this._body, (System.Linq.Expressions.Expression) Utils.Empty()) : (System.Linq.Expressions.Expression) this._body).Catch(exception = System.Linq.Expressions.Expression.Variable(typeof (Exception), "exception"), TryStatement.GetTracebackHeader((Statement) this, exception, this.GlobalParent.AddDebugInfoAndVoid((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(Node.MakeAssignment(parameterExpression4, Utils.Constant((object) false)), Utils.IfThen(this.GlobalParent.Convert(typeof (bool), ConversionResultKind.ExplicitCast, this.GlobalParent.Operation(typeof (bool), PythonOperationKind.IsFalse, this.MakeExitCall(parameterExpression2, (System.Linq.Expressions.Expression) exception))), Node.UpdateLineUpdated(true), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Throw((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeRethrowExceptionWorker, (System.Linq.Expressions.Expression) exception)))), this._body.Span)), Node.PopLineUpdated(saveCurrent), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Empty())).Finally(Utils.IfThen((System.Linq.Expressions.Expression) parameterExpression4, this.GlobalParent.AddDebugInfoAndVoid((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) this.GlobalParent.PyContext.Invoke(new CallSignature(3)), typeof (object), this.Parent.LocalContext, (System.Linq.Expressions.Expression) parameterExpression2, Utils.Constant((object) null), Utils.Constant((object) null), Utils.Constant((object) null)), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Empty()), this._contextManager.Span))));
    collectionBuilder1.Add((System.Linq.Expressions.Expression) Utils.Empty());
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) collectionBuilder2.ToReadOnlyCollection(), (IEnumerable<System.Linq.Expressions.Expression>) collectionBuilder1.ToReadOnlyCollection());
  }

  private System.Linq.Expressions.Expression MakeExitCall(
    ParameterExpression exit,
    System.Linq.Expressions.Expression exception)
  {
    return this.GlobalParent.Convert(typeof (bool), ConversionResultKind.ExplicitCast, this.Parent.Invoke(new CallSignature(new ArgumentType[1]
    {
      ArgumentType.List
    }), this.Parent.LocalContext, (System.Linq.Expressions.Expression) exit, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.GetExceptionInfoLocal, this.Parent.LocalContext, exception)));
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._contextManager != null)
        this._contextManager.Walk(walker);
      if (this._var != null)
        this._var.Walk(walker);
      if (this._body != null)
        this._body.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
