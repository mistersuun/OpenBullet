// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.TryStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public class TryStatement : Statement
{
  private int _headerIndex;
  private Statement _body;
  private readonly TryStatementHandler[] _handlers;
  private Statement _else;
  private Statement _finally;

  public TryStatement(
    Statement body,
    TryStatementHandler[] handlers,
    Statement else_,
    Statement finally_)
  {
    this._body = body;
    this._handlers = handlers;
    this._else = else_;
    this._finally = finally_;
  }

  public int HeaderIndex
  {
    set => this._headerIndex = value;
  }

  public Statement Body => this._body;

  public Statement Else => this._else;

  public Statement Finally => this._finally;

  public IList<TryStatementHandler> Handlers => (IList<TryStatementHandler>) this._handlers;

  public override System.Linq.Expressions.Expression Reduce()
  {
    ParameterExpression parameterExpression1 = (ParameterExpression) null;
    ParameterExpression parameterExpression2 = (ParameterExpression) null;
    if (this._else != null || this._handlers != null && this._handlers.Length != 0)
    {
      parameterExpression1 = System.Linq.Expressions.Expression.Variable(typeof (bool), "$lineUpdated_try");
      if (this._else != null)
        parameterExpression2 = System.Linq.Expressions.Expression.Variable(typeof (bool), "run_else");
    }
    System.Linq.Expressions.Expression body1 = (System.Linq.Expressions.Expression) this._body;
    System.Linq.Expressions.Expression body2 = (System.Linq.Expressions.Expression) this._else;
    ParameterExpression parameterExpression3;
    System.Linq.Expressions.Expression expression;
    if (this._handlers != null && this._handlers.Length != 0)
    {
      parameterExpression3 = System.Linq.Expressions.Expression.Variable(typeof (Exception), "$exception");
      expression = this.TransformHandlers(parameterExpression3);
    }
    else
    {
      parameterExpression3 = (ParameterExpression) null;
      expression = (System.Linq.Expressions.Expression) null;
    }
    System.Linq.Expressions.Expression body3;
    if (body2 != null)
      body3 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression2, Utils.Constant((object) true)), Node.PushLineUpdated(false, parameterExpression1), LightExceptions.RewriteExternal((System.Linq.Expressions.Expression) Utils.Try(this.Parent.AddDebugInfo((System.Linq.Expressions.Expression) Utils.Empty(), new SourceSpan(this.Span.Start, this.GlobalParent.IndexToLocation(this._headerIndex))), body1, Utils.Constant((object) null)).Catch(parameterExpression3, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression2, Utils.Constant((object) false)), expression, Node.PopLineUpdated(parameterExpression1), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression3, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) null, typeof (Exception))), Utils.Constant((object) null))), Utils.IfThen((System.Linq.Expressions.Expression) parameterExpression2, body2), (System.Linq.Expressions.Expression) Utils.Empty());
    else if (expression != null)
      body3 = LightExceptions.RewriteExternal((System.Linq.Expressions.Expression) Utils.Try(this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) Utils.Empty(), new SourceSpan(this.Span.Start, this.GlobalParent.IndexToLocation(this._headerIndex))), Node.PushLineUpdated(false, parameterExpression1), body1, Utils.Constant((object) null)).Catch(parameterExpression3, expression, Node.PopLineUpdated(parameterExpression1), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.ExceptionHandled, this.Parent.LocalContext), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression3, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) null, typeof (Exception))), Utils.Constant((object) null)));
    else
      body3 = body1;
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) TryStatement.GetVariables(parameterExpression1, parameterExpression2), this.AddFinally(body3), (System.Linq.Expressions.Expression) Utils.Default(typeof (void)));
  }

  private static ReadOnlyCollectionBuilder<ParameterExpression> GetVariables(
    ParameterExpression lineUpdated,
    ParameterExpression runElse)
  {
    ReadOnlyCollectionBuilder<ParameterExpression> variables = new ReadOnlyCollectionBuilder<ParameterExpression>();
    if (lineUpdated != null)
      variables.Add(lineUpdated);
    if (runElse != null)
      variables.Add(runElse);
    return variables;
  }

  private System.Linq.Expressions.Expression AddFinally(System.Linq.Expressions.Expression body)
  {
    if (this._finally != null)
    {
      ParameterExpression parameterExpression1 = System.Linq.Expressions.Expression.Variable(typeof (Exception), "$tryThrows");
      ParameterExpression parameterExpression2 = System.Linq.Expressions.Expression.Variable(typeof (Exception), "$localException");
      System.Linq.Expressions.Expression expression = (System.Linq.Expressions.Expression) this._finally;
      body = (System.Linq.Expressions.Expression) Utils.Try((System.Linq.Expressions.Expression) Utils.Try(this.Parent.AddDebugInfo((System.Linq.Expressions.Expression) Utils.Empty(), new SourceSpan(this.Span.Start, this.GlobalParent.IndexToLocation(this._headerIndex))), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression1, (System.Linq.Expressions.Expression) Utils.Constant((object) null, typeof (Exception))), body, (System.Linq.Expressions.Expression) Utils.Empty()).Catch(parameterExpression2, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression1, (System.Linq.Expressions.Expression) parameterExpression2), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Rethrow()))).FinallyWithJumps((System.Linq.Expressions.Expression) Utils.If((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NotEqual((System.Linq.Expressions.Expression) parameterExpression1, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Default(typeof (Exception))), this.Parent.GetSaveLineNumberExpression(parameterExpression1, false)), Node.UpdateLineUpdated(false), expression, (System.Linq.Expressions.Expression) Utils.If((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NotEqual((System.Linq.Expressions.Expression) parameterExpression1, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Default(typeof (Exception))), Node.UpdateLineUpdated(true)));
      body = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        parameterExpression1
      }, body);
    }
    return body;
  }

  private System.Linq.Expressions.Expression TransformHandlers(ParameterExpression exception)
  {
    ParameterExpression left = System.Linq.Expressions.Expression.Variable(typeof (object), "$extracted");
    List<Microsoft.Scripting.Ast.IfStatementTest> ifStatementTestList = new List<Microsoft.Scripting.Ast.IfStatementTest>(this._handlers.Length);
    ParameterExpression parameterExpression = (ParameterExpression) null;
    System.Linq.Expressions.Expression @else = (System.Linq.Expressions.Expression) null;
    for (int index = 0; index < this._handlers.Length; ++index)
    {
      TryStatementHandler handler = this._handlers[index];
      if (handler.Test != null)
      {
        System.Linq.Expressions.Expression expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.CheckException, this.Parent.LocalContext, (System.Linq.Expressions.Expression) left, Utils.Convert((System.Linq.Expressions.Expression) handler.Test, typeof (object)));
        Microsoft.Scripting.Ast.IfStatementTest ifStatementTest;
        if (handler.Target != null)
        {
          if (parameterExpression == null)
            parameterExpression = System.Linq.Expressions.Expression.Variable(typeof (object), "$converted");
          ifStatementTest = Utils.IfCondition((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NotEqual((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression, expression), Utils.Constant((object) null)), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(handler.Target.TransformSet(SourceSpan.None, (System.Linq.Expressions.Expression) parameterExpression, PythonOperationKind.None), this.GlobalParent.AddDebugInfo(TryStatement.GetTracebackHeader((Statement) this, exception, (System.Linq.Expressions.Expression) handler.Body), new SourceSpan(this.GlobalParent.IndexToLocation(handler.StartIndex), this.GlobalParent.IndexToLocation(handler.HeaderIndex))), (System.Linq.Expressions.Expression) Utils.Empty()));
        }
        else
          ifStatementTest = Utils.IfCondition((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NotEqual(expression, Utils.Constant((object) null)), this.GlobalParent.AddDebugInfo(TryStatement.GetTracebackHeader((Statement) this, exception, (System.Linq.Expressions.Expression) handler.Body), new SourceSpan(this.GlobalParent.IndexToLocation(handler.StartIndex), this.GlobalParent.IndexToLocation(handler.HeaderIndex))));
        ifStatementTestList.Add(ifStatementTest);
      }
      else
        @else = this.GlobalParent.AddDebugInfo(TryStatement.GetTracebackHeader((Statement) this, exception, (System.Linq.Expressions.Expression) handler.Body), new SourceSpan(this.GlobalParent.IndexToLocation(handler.StartIndex), this.GlobalParent.IndexToLocation(handler.HeaderIndex)));
    }
    System.Linq.Expressions.Expression expression1;
    if (ifStatementTestList.Count > 0)
    {
      if (@else == null)
        @else = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(this.Parent.GetSaveLineNumberExpression(exception, true), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Throw((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (ExceptionHelpers).GetMethod("UpdateForRethrow"), (System.Linq.Expressions.Expression) exception)));
      expression1 = Utils.If(ifStatementTestList.ToArray(), @else);
    }
    else
      expression1 = @else;
    IList<ParameterExpression> variables;
    if (parameterExpression != null)
      variables = (IList<ParameterExpression>) new ReadOnlyCollectionBuilder<ParameterExpression>()
      {
        parameterExpression,
        left
      };
    else
      variables = (IList<ParameterExpression>) new ReadOnlyCollectionBuilder<ParameterExpression>()
      {
        left
      };
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) variables, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) left, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.SetCurrentException, this.Parent.LocalContext, (System.Linq.Expressions.Expression) exception)), expression1, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) left, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) null)), (System.Linq.Expressions.Expression) Utils.Empty());
  }

  internal static System.Linq.Expressions.Expression GetTracebackHeader(
    Statement node,
    ParameterExpression exception,
    System.Linq.Expressions.Expression body)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(node.Parent.GetSaveLineNumberExpression(exception, false), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.BuildExceptionInfo, node.Parent.LocalContext, (System.Linq.Expressions.Expression) exception), body, (System.Linq.Expressions.Expression) Utils.Empty());
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._body != null)
        this._body.Walk(walker);
      if (this._handlers != null)
      {
        foreach (Node handler in this._handlers)
          handler.Walk(walker);
      }
      if (this._else != null)
        this._else.Walk(walker);
      if (this._finally != null)
        this._finally.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
