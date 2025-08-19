// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.GeneratorRewriter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler;

internal sealed class GeneratorRewriter : DynamicExpressionVisitor
{
  private readonly Expression _body;
  private readonly string _name;
  private readonly StrongBox<Type> _tupleType = new StrongBox<Type>((Type) null);
  private readonly StrongBox<ParameterExpression> _tupleExpr = new StrongBox<ParameterExpression>((ParameterExpression) null);
  private readonly Stack<LabelTarget> _returnLabels = new Stack<LabelTarget>();
  private ParameterExpression _gotoRouter;
  private bool _inTryWithFinally;
  private readonly List<GeneratorRewriter.YieldMarker> _yields = new List<GeneratorRewriter.YieldMarker>();
  private readonly Dictionary<ParameterExpression, DelayedTupleExpression> _vars = new Dictionary<ParameterExpression, DelayedTupleExpression>();
  private readonly List<KeyValuePair<ParameterExpression, DelayedTupleExpression>> _orderedVars = new List<KeyValuePair<ParameterExpression, DelayedTupleExpression>>();
  private readonly List<ParameterExpression> _temps = new List<ParameterExpression>();
  private Expression _state;
  private Expression _current;
  private const int GotoRouterYielding = 0;
  private const int GotoRouterNone = -1;
  internal const int NotStarted = -1;
  internal const int Finished = 0;
  internal static ParameterExpression _generatorParam = Expression.Parameter(typeof (PythonGenerator), "$generator");

  internal GeneratorRewriter(string name, Expression body)
  {
    this._body = body;
    this._name = name;
    this._returnLabels.Push(Expression.Label("retLabel"));
    this._gotoRouter = Expression.Variable(typeof (int), "$gotoRouter");
  }

  internal Expression Reduce(
    bool shouldInterpret,
    bool emitDebugSymbols,
    int compilationThreshold,
    IList<ParameterExpression> parameters,
    Func<Expression<Func<MutableTuple, object>>, Expression<Func<MutableTuple, object>>> bodyConverter)
  {
    this._state = (Expression) this.LiftVariable(Expression.Parameter(typeof (int), "state"));
    this._current = (Expression) this.LiftVariable(Expression.Parameter(typeof (object), "current"));
    foreach (ParameterExpression parameter in (IEnumerable<ParameterExpression>) parameters)
      this.LiftVariable(parameter);
    DelayedTupleExpression delayedTupleExpression = this.LiftVariable(GeneratorRewriter._generatorParam);
    Expression expression1 = ((ExpressionVisitor) this).Visit(this._body);
    int count = this._yields.Count;
    SwitchCase[] switchCaseArray = new SwitchCase[count + 1];
    for (int index = 0; index < count; ++index)
      switchCaseArray[index] = Expression.SwitchCase((Expression) Expression.Goto(this._yields[index].Label), Microsoft.Scripting.Ast.Utils.Constant((object) this._yields[index].State));
    switchCaseArray[count] = Expression.SwitchCase((Expression) Expression.Goto(this._returnLabels.Peek()), Microsoft.Scripting.Ast.Utils.Constant((object) 0));
    Expression[] expressionArray = new Expression[this._vars.Count];
    foreach (KeyValuePair<ParameterExpression, DelayedTupleExpression> orderedVar in this._orderedVars)
      expressionArray[orderedVar.Value.Index] = orderedVar.Value.Index < 2 || orderedVar.Value.Index >= parameters.Count + 2 ? (Expression) Expression.Default(orderedVar.Key.Type) : (Expression) parameters[orderedVar.Value.Index - 2];
    Expression right = MutableTuple.Create(expressionArray);
    Type type = this._tupleType.Value = right.Type;
    ParameterExpression left1 = this._tupleExpr.Value = Expression.Parameter(type, "tuple");
    ParameterExpression parameterExpression1 = Expression.Parameter(typeof (MutableTuple), "tupleArg");
    this._temps.Add(this._gotoRouter);
    this._temps.Add(left1);
    ParameterExpression left2 = Expression.Parameter(type, "tuple");
    ParameterExpression parameterExpression2 = Expression.Parameter(typeof (PythonGenerator), "ret");
    Expression<Func<MutableTuple, object>> expression2 = Expression.Lambda<Func<MutableTuple, object>>((Expression) Expression.Block((IEnumerable<ParameterExpression>) this._temps.ToArray(), (Expression) Expression.Assign((Expression) left1, (Expression) Expression.Convert((Expression) parameterExpression1, type)), (Expression) Expression.Switch((Expression) Expression.Assign((Expression) this._gotoRouter, this._state), switchCaseArray), expression1, this.MakeAssign(this._state, Microsoft.Scripting.Ast.Utils.Constant((object) 0)), (Expression) Expression.Label(this._returnLabels.Peek()), this._current), this._name, (IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression1
    });
    return (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
    {
      left2,
      parameterExpression2
    }, (Expression) Expression.Assign((Expression) parameterExpression2, (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeGenerator"), (Expression) parameters[0], (Expression) Expression.Assign((Expression) left2, right), emitDebugSymbols ? (Expression) bodyConverter(expression2) : (Expression) Expression.Constant((object) new LazyCode<Func<MutableTuple, object>>(bodyConverter(expression2), shouldInterpret, compilationThreshold), typeof (object)))), (Expression) new DelayedTupleAssign((Expression) new DelayedTupleExpression(delayedTupleExpression.Index, new StrongBox<ParameterExpression>(left2), this._tupleType, typeof (PythonGenerator)), (Expression) parameterExpression2), (Expression) parameterExpression2);
  }

  private GeneratorRewriter.YieldMarker GetYieldMarker(YieldExpression node)
  {
    GeneratorRewriter.YieldMarker yieldMarker = new GeneratorRewriter.YieldMarker(this._yields.Count + 1);
    this._yields.Add(yieldMarker);
    return yieldMarker;
  }

  private Expression ToTemp(ref Expression e)
  {
    DelayedTupleExpression variable = this.LiftVariable(Expression.Variable(e.Type, "generatorTemp" + (object) this._temps.Count));
    Expression temp = this.MakeAssign((Expression) variable, e);
    e = (Expression) variable;
    return temp;
  }

  private Expression MakeAssign(Expression variable, Expression value)
  {
    switch (value.NodeType)
    {
      case ExpressionType.Conditional:
        return this.MakeAssignConditional(variable, value);
      case ExpressionType.Block:
        return this.MakeAssignBlock(variable, value);
      case ExpressionType.Label:
        return this.MakeAssignLabel(variable, (LabelExpression) value);
      default:
        return GeneratorRewriter.DelayedAssign(variable, value);
    }
  }

  private Expression MakeAssignLabel(Expression variable, LabelExpression value)
  {
    GeneratorRewriter.GotoRewriteInfo gotoRewriteInfo = new GeneratorRewriter.GotoRewriteInfo(variable, Expression.Label(value.Target.Name + "_voided"));
    Expression defaultValue = new GeneratorRewriter.GotoRewriter(this, gotoRewriteInfo, value.Target).Visit(value.DefaultValue);
    return this.MakeAssignLabel(variable, gotoRewriteInfo, value.Target, defaultValue);
  }

  private Expression MakeAssignLabel(
    Expression variable,
    GeneratorRewriter.GotoRewriteInfo curVariable,
    LabelTarget target,
    Expression defaultValue)
  {
    return (Expression) Expression.Label(curVariable.VoidTarget, this.MakeAssign(variable, defaultValue));
  }

  private Expression MakeAssignBlock(Expression variable, Expression value)
  {
    BlockExpression blockExpression = (BlockExpression) value;
    ReadOnlyCollectionBuilder<Expression> collectionBuilder = new ReadOnlyCollectionBuilder<Expression>((IEnumerable<Expression>) blockExpression.Expressions);
    Expression expression = collectionBuilder[collectionBuilder.Count - 1];
    if (expression.NodeType == ExpressionType.Label)
    {
      LabelExpression labelExpression = (LabelExpression) expression;
      GeneratorRewriter.GotoRewriteInfo gotoRewriteInfo = new GeneratorRewriter.GotoRewriteInfo(variable, Expression.Label(labelExpression.Target.Name + "_voided"));
      GeneratorRewriter.GotoRewriter gotoRewriter = new GeneratorRewriter.GotoRewriter(this, gotoRewriteInfo, labelExpression.Target);
      for (int index = 0; index < collectionBuilder.Count - 1; ++index)
        collectionBuilder[index] = gotoRewriter.Visit(collectionBuilder[index]);
      collectionBuilder[collectionBuilder.Count - 1] = this.MakeAssignLabel(variable, gotoRewriteInfo, labelExpression.Target, gotoRewriter.Visit(labelExpression.DefaultValue));
    }
    else
      collectionBuilder[collectionBuilder.Count - 1] = this.MakeAssign(variable, collectionBuilder[collectionBuilder.Count - 1]);
    return (Expression) Expression.Block((IEnumerable<ParameterExpression>) blockExpression.Variables, (IEnumerable<Expression>) collectionBuilder);
  }

  private Expression MakeAssignConditional(Expression variable, Expression value)
  {
    ConditionalExpression conditionalExpression = (ConditionalExpression) value;
    return (Expression) Expression.Condition(conditionalExpression.Test, this.MakeAssign(variable, conditionalExpression.IfTrue), this.MakeAssign(variable, conditionalExpression.IfFalse));
  }

  private BlockExpression ToTemp(ref ReadOnlyCollection<Expression> args)
  {
    int count = args.Count;
    Expression[] expressionArray1 = new Expression[count];
    Expression[] expressionArray2 = new Expression[count];
    args.CopyTo(expressionArray2, 0);
    for (int index = 0; index < count; ++index)
      expressionArray1[index] = this.ToTemp(ref expressionArray2[index]);
    args = new ReadOnlyCollection<Expression>((IList<Expression>) expressionArray2);
    return Expression.Block(expressionArray1);
  }

  protected virtual Expression VisitTry(TryExpression node)
  {
    int count1 = this._yields.Count;
    bool inTryWithFinally = this._inTryWithFinally;
    if (node.Finally != null || node.Fault != null)
      this._inTryWithFinally = true;
    Expression body = ((ExpressionVisitor) this).Visit(node.Body);
    int count2 = this._yields.Count;
    IList<CatchBlock> catchBlockList = (IList<CatchBlock>) ExpressionVisitor.Visit<CatchBlock>(node.Handlers, new Func<CatchBlock, CatchBlock>(((ExpressionVisitor) this).VisitCatchBlock));
    int count3 = this._yields.Count;
    this._returnLabels.Push(Expression.Label("tryLabel"));
    Expression @finally = ((ExpressionVisitor) this).Visit(node.Finally);
    Expression fault = ((ExpressionVisitor) this).Visit(node.Fault);
    LabelTarget target = this._returnLabels.Pop();
    int count4 = this._yields.Count;
    this._inTryWithFinally = inTryWithFinally;
    if (body == node.Body && catchBlockList == node.Handlers && @finally == node.Finally && fault == node.Fault)
      return (Expression) node;
    if (count1 == this._yields.Count)
      return (Expression) Expression.MakeTry((Type) null, body, @finally, fault, (IEnumerable<CatchBlock>) catchBlockList);
    if (fault != null && count4 != count3)
      throw new NotSupportedException("yield in fault block is not supported");
    LabelTarget labelTarget1 = Expression.Label("tryStart");
    if (count2 != count1)
      body = (Expression) Expression.Block((Expression) this.MakeYieldRouter(node.Body.Type, count1, count2, labelTarget1), body);
    if (count3 != count2)
    {
      List<Expression> expressionList = new List<Expression>();
      expressionList.Add((Expression) this.MakeYieldRouter(node.Body.Type, count2, count3, labelTarget1));
      expressionList.Add((Expression) null);
      int index = 0;
      for (int count5 = catchBlockList.Count; index < count5; ++index)
      {
        CatchBlock catchBlock = catchBlockList[index];
        if (catchBlock != node.Handlers[index])
        {
          if (catchBlockList.IsReadOnly)
            catchBlockList = (IList<CatchBlock>) ArrayUtils.ToArray<CatchBlock>((ICollection<CatchBlock>) catchBlockList);
          ParameterExpression parameterExpression = Expression.Variable(catchBlock.Test, (string) null);
          ParameterExpression node1 = catchBlock.Variable ?? Expression.Variable(catchBlock.Test, (string) null);
          this.LiftVariable(node1);
          Expression filter = catchBlock.Filter;
          if (filter != null && catchBlock.Variable != null)
            filter = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
            {
              catchBlock.Variable
            }, (Expression) Expression.Assign((Expression) catchBlock.Variable, (Expression) parameterExpression), filter);
          catchBlockList[index] = Expression.Catch(parameterExpression, (Expression) Expression.Block(GeneratorRewriter.DelayedAssign(((ExpressionVisitor) this).Visit((Expression) node1), (Expression) parameterExpression), (Expression) Expression.Default(node.Body.Type)), filter);
          Expression ifTrue = new GeneratorRewriter.RethrowRewriter()
          {
            Exception = ((Expression) node1)
          }.Visit(catchBlock.Body);
          expressionList.Add((Expression) Expression.Condition((Expression) Expression.NotEqual(((ExpressionVisitor) this).Visit((Expression) node1), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, node1.Type)), ifTrue, (Expression) Expression.Default(node.Body.Type)));
        }
      }
      expressionList[1] = (Expression) Expression.MakeTry((Type) null, body, (Expression) null, (Expression) null, (IEnumerable<CatchBlock>) new ReadOnlyCollection<CatchBlock>(catchBlockList));
      body = (Expression) Expression.Block((IEnumerable<Expression>) expressionList);
      catchBlockList = (IList<CatchBlock>) new CatchBlock[0];
    }
    if (count4 != count3)
    {
      if (catchBlockList.Count > 0)
      {
        body = (Expression) Expression.MakeTry((Type) null, body, (Expression) null, (Expression) null, (IEnumerable<CatchBlock>) catchBlockList);
        catchBlockList = (IList<CatchBlock>) new CatchBlock[0];
      }
      LabelTarget labelTarget2 = Expression.Label("tryEnd");
      Expression expression1 = (Expression) this.MakeYieldRouter(node.Body.Type, count3, count4, labelTarget2);
      SwitchExpression switchExpression = this.MakeYieldRouter(node.Body.Type, count3, count4, labelTarget1);
      ParameterExpression parameterExpression = Expression.Variable(typeof (Exception), "e");
      ParameterExpression node2 = Expression.Variable(typeof (Exception), "$saved$" + (object) this._temps.Count);
      this.LiftVariable(node2);
      Expression expression2 = body;
      Expression expression3 = GeneratorRewriter.DelayedAssign(((ExpressionVisitor) this).Visit((Expression) node2), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, node2.Type));
      LabelExpression labelExpression = Expression.Label(labelTarget2);
      body = (Expression) Expression.Block((Expression) Expression.TryCatchFinally((Expression) Expression.Block((Expression) switchExpression, expression2, expression3, (Expression) labelExpression), (Expression) Expression.Block(this.MakeSkipFinallyBlock(target), expression1, @finally, (Expression) Expression.Condition((Expression) Expression.NotEqual(((ExpressionVisitor) this).Visit((Expression) node2), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, node2.Type)), (Expression) Expression.Throw(((ExpressionVisitor) this).Visit((Expression) node2)), (Expression) Microsoft.Scripting.Ast.Utils.Empty()), (Expression) Expression.Label(target)), Expression.Catch(parameterExpression, Microsoft.Scripting.Ast.Utils.Void(GeneratorRewriter.DelayedAssign(((ExpressionVisitor) this).Visit((Expression) node2), (Expression) parameterExpression)))), (Expression) Expression.Condition((Expression) Expression.Equal((Expression) this._gotoRouter, Microsoft.Scripting.Ast.Utils.Constant((object) 0)), (Expression) Expression.Goto(this._returnLabels.Peek()), (Expression) Microsoft.Scripting.Ast.Utils.Empty()));
      @finally = (Expression) null;
    }
    else if (@finally != null)
      @finally = (Expression) Expression.Block(this.MakeSkipFinallyBlock(target), @finally, (Expression) Expression.Label(target));
    if (catchBlockList.Count > 0 || @finally != null || fault != null)
      body = (Expression) Expression.MakeTry((Type) null, body, @finally, fault, (IEnumerable<CatchBlock>) catchBlockList);
    return (Expression) Expression.Block((Expression) Expression.Label(labelTarget1), body);
  }

  private Expression MakeSkipFinallyBlock(LabelTarget target)
  {
    return (Expression) Expression.Condition((Expression) Expression.AndAlso((Expression) Expression.Equal((Expression) this._gotoRouter, Microsoft.Scripting.Ast.Utils.Constant((object) 0)), (Expression) Expression.NotEqual(this._state, Microsoft.Scripting.Ast.Utils.Constant((object) 0))), (Expression) Expression.Goto(target), (Expression) Microsoft.Scripting.Ast.Utils.Empty());
  }

  protected virtual CatchBlock VisitCatchBlock(CatchBlock node)
  {
    if (node.Variable != null)
      this.LiftVariable(node.Variable);
    Expression lhs = ((ExpressionVisitor) this).Visit((Expression) node.Variable);
    int count = this._yields.Count;
    Expression filter = ((ExpressionVisitor) this).Visit(node.Filter);
    if (count != this._yields.Count)
      throw new NotSupportedException("yield in filter is not allowed");
    Expression body = ((ExpressionVisitor) this).Visit(node.Body);
    if (lhs == node.Variable && body == node.Body && filter == node.Filter)
      return node;
    return lhs != node.Variable && count == this._yields.Count ? Expression.MakeCatchBlock(node.Test, node.Variable, (Expression) Expression.Block((Expression) new DelayedTupleAssign(lhs, (Expression) node.Variable), body), filter) : Expression.MakeCatchBlock(node.Test, node.Variable, body, filter);
  }

  private SwitchExpression MakeYieldRouter(Type type, int start, int end, LabelTarget newTarget)
  {
    SwitchCase[] switchCaseArray = new SwitchCase[end - start];
    for (int index = start; index < end; ++index)
    {
      GeneratorRewriter.YieldMarker yield = this._yields[index];
      switchCaseArray[index - start] = Expression.SwitchCase((Expression) Expression.Goto(yield.Label, type), Microsoft.Scripting.Ast.Utils.Constant((object) yield.State));
      yield.Label = newTarget;
    }
    return Expression.Switch((Expression) this._gotoRouter, (Expression) Expression.Default(type), switchCaseArray);
  }

  protected virtual Expression VisitExtension(Expression node)
  {
    if (node is YieldExpression node1)
      return this.VisitYield(node1);
    FinallyFlowControlExpression controlExpression = node as FinallyFlowControlExpression;
    return ((ExpressionVisitor) this).Visit(node.ReduceExtensions());
  }

  private Expression VisitYield(YieldExpression node)
  {
    Expression expression = ((ExpressionVisitor) this).Visit(node.Value);
    List<Expression> expressionList = new List<Expression>();
    if (expression == null)
    {
      expressionList.Add(this.MakeAssign(this._state, Microsoft.Scripting.Ast.Utils.Constant((object) 0)));
      if (this._inTryWithFinally)
        expressionList.Add((Expression) Expression.Assign((Expression) this._gotoRouter, Microsoft.Scripting.Ast.Utils.Constant((object) 0)));
      expressionList.Add((Expression) Expression.Goto(this._returnLabels.Peek()));
      return (Expression) Expression.Block((IEnumerable<Expression>) expressionList);
    }
    expressionList.Add(this.MakeAssign(this._current, expression));
    GeneratorRewriter.YieldMarker yieldMarker = this.GetYieldMarker(node);
    expressionList.Add(this.MakeAssign(this._state, Microsoft.Scripting.Ast.Utils.Constant((object) yieldMarker.State)));
    if (this._inTryWithFinally)
      expressionList.Add((Expression) Expression.Assign((Expression) this._gotoRouter, Microsoft.Scripting.Ast.Utils.Constant((object) 0)));
    expressionList.Add((Expression) Expression.Goto(this._returnLabels.Peek()));
    expressionList.Add((Expression) Expression.Label(yieldMarker.Label));
    expressionList.Add((Expression) Expression.Assign((Expression) this._gotoRouter, Microsoft.Scripting.Ast.Utils.Constant((object) -1)));
    expressionList.Add((Expression) Microsoft.Scripting.Ast.Utils.Empty());
    return (Expression) Expression.Block((IEnumerable<Expression>) expressionList);
  }

  protected virtual Expression VisitBlock(BlockExpression node)
  {
    foreach (ParameterExpression variable in node.Variables)
      this.LiftVariable(variable);
    int count = this._yields.Count;
    ReadOnlyCollection<Expression> readOnlyCollection = ((ExpressionVisitor) this).Visit(node.Expressions);
    if (readOnlyCollection == node.Expressions)
      return (Expression) node;
    return count == this._yields.Count ? (Expression) Expression.Block(node.Type, (IEnumerable<ParameterExpression>) node.Variables, (IEnumerable<Expression>) readOnlyCollection) : (Expression) Expression.Block(node.Type, (IEnumerable<Expression>) readOnlyCollection);
  }

  private DelayedTupleExpression LiftVariable(ParameterExpression param)
  {
    DelayedTupleExpression delayedTupleExpression;
    if (!this._vars.TryGetValue(param, out delayedTupleExpression))
    {
      this._vars[param] = delayedTupleExpression = new DelayedTupleExpression(this._vars.Count, this._tupleExpr, this._tupleType, param.Type);
      this._orderedVars.Add(new KeyValuePair<ParameterExpression, DelayedTupleExpression>(param, delayedTupleExpression));
    }
    return delayedTupleExpression;
  }

  protected virtual Expression VisitParameter(ParameterExpression node)
  {
    return (Expression) this._vars[node];
  }

  protected virtual Expression VisitLambda<T>(Expression<T> node) => (Expression) node;

  private Expression VisitAssign(BinaryExpression node)
  {
    int count = this._yields.Count;
    Expression expression = ((ExpressionVisitor) this).Visit(node.Left);
    Expression e1 = ((ExpressionVisitor) this).Visit(node.Right);
    if (expression == node.Left && e1 == node.Right)
      return (Expression) node;
    if (count == this._yields.Count)
      return expression is DelayedTupleExpression ? (Expression) new DelayedTupleAssign(expression, e1) : (Expression) Expression.Assign(expression, e1);
    List<Expression> expressionList = new List<Expression>();
    if (expression == node.Left)
    {
      switch (expression.NodeType)
      {
        case ExpressionType.MemberAccess:
          MemberExpression left1 = (MemberExpression) node.Left;
          Expression e2 = ((ExpressionVisitor) this).Visit(left1.Expression);
          expressionList.Add(this.ToTemp(ref e2));
          expression = (Expression) Expression.MakeMemberAccess(e2, left1.Member);
          break;
        case ExpressionType.Parameter:
          break;
        case ExpressionType.Index:
          IndexExpression left2 = (IndexExpression) node.Left;
          Expression e3 = ((ExpressionVisitor) this).Visit(left2.Object);
          ReadOnlyCollection<Expression> args = ((ExpressionVisitor) this).Visit(left2.Arguments);
          if (e3 == left2.Object && args == left2.Arguments)
            return (Expression) left2;
          expressionList.Add(this.ToTemp(ref e3));
          expressionList.Add((Expression) this.ToTemp(ref args));
          expression = (Expression) Expression.MakeIndex(e3, left2.Indexer, (IEnumerable<Expression>) args);
          break;
        default:
          throw Assert.Unreachable;
      }
    }
    else if (expression is BlockExpression)
    {
      BlockExpression blockExpression = (BlockExpression) expression;
      expression = blockExpression.Expressions[blockExpression.Expressions.Count - 1];
      expressionList.AddRange((IEnumerable<Expression>) blockExpression.Expressions);
      expressionList.RemoveAt(expressionList.Count - 1);
    }
    if (e1 != node.Right)
      expressionList.Add(this.ToTemp(ref e1));
    if (expression is DelayedTupleExpression)
      expressionList.Add(GeneratorRewriter.DelayedAssign(expression, e1));
    else
      expressionList.Add((Expression) Expression.Assign(expression, e1));
    return (Expression) Expression.Block((IEnumerable<Expression>) expressionList);
  }

  protected virtual Expression VisitDynamic(DynamicExpression node)
  {
    int count = this._yields.Count;
    ReadOnlyCollection<Expression> args = ((ExpressionVisitor) this).Visit(node.Arguments);
    if (args == node.Arguments)
      return (Expression) node;
    return count == this._yields.Count ? (Expression) Expression.MakeDynamic(node.DelegateType, node.Binder, (IEnumerable<Expression>) args) : (Expression) Expression.Block((Expression) this.ToTemp(ref args), (Expression) Expression.MakeDynamic(node.DelegateType, node.Binder, (IEnumerable<Expression>) args));
  }

  protected virtual Expression VisitIndex(IndexExpression node)
  {
    int count = this._yields.Count;
    Expression e = ((ExpressionVisitor) this).Visit(node.Object);
    ReadOnlyCollection<Expression> args = ((ExpressionVisitor) this).Visit(node.Arguments);
    if (e == node.Object && args == node.Arguments)
      return (Expression) node;
    return count == this._yields.Count ? (Expression) Expression.MakeIndex(e, node.Indexer, (IEnumerable<Expression>) args) : (Expression) Expression.Block(this.ToTemp(ref e), (Expression) this.ToTemp(ref args), (Expression) Expression.MakeIndex(e, node.Indexer, (IEnumerable<Expression>) args));
  }

  protected virtual Expression VisitInvocation(InvocationExpression node)
  {
    int count = this._yields.Count;
    Expression e = ((ExpressionVisitor) this).Visit(node.Expression);
    ReadOnlyCollection<Expression> args = ((ExpressionVisitor) this).Visit(node.Arguments);
    if (e == node.Expression && args == node.Arguments)
      return (Expression) node;
    return count == this._yields.Count ? (Expression) Expression.Invoke(e, (IEnumerable<Expression>) args) : (Expression) Expression.Block(this.ToTemp(ref e), (Expression) this.ToTemp(ref args), (Expression) Expression.Invoke(e, (IEnumerable<Expression>) args));
  }

  protected virtual Expression VisitMethodCall(MethodCallExpression node)
  {
    int count = this._yields.Count;
    Expression e = ((ExpressionVisitor) this).Visit(node.Object);
    ReadOnlyCollection<Expression> args = ((ExpressionVisitor) this).Visit(node.Arguments);
    if (e == node.Object && args == node.Arguments)
      return (Expression) node;
    if (count == this._yields.Count)
      return (Expression) Expression.Call(e, node.Method, (IEnumerable<Expression>) args);
    return e == null ? (Expression) Expression.Block((Expression) this.ToTemp(ref args), (Expression) Expression.Call((Expression) null, node.Method, (IEnumerable<Expression>) args)) : (Expression) Expression.Block(this.ToTemp(ref e), (Expression) this.ToTemp(ref args), (Expression) Expression.Call(e, node.Method, (IEnumerable<Expression>) args));
  }

  protected virtual Expression VisitNew(NewExpression node)
  {
    int count = this._yields.Count;
    ReadOnlyCollection<Expression> args = ((ExpressionVisitor) this).Visit(node.Arguments);
    if (args == node.Arguments)
      return (Expression) node;
    if (count != this._yields.Count)
      return (Expression) Expression.Block((Expression) this.ToTemp(ref args), node.Members != null ? (Expression) Expression.New(node.Constructor, (IEnumerable<Expression>) args, (IEnumerable<MemberInfo>) node.Members) : (Expression) Expression.New(node.Constructor, (IEnumerable<Expression>) args));
    return node.Members == null ? (Expression) Expression.New(node.Constructor, (IEnumerable<Expression>) args) : (Expression) Expression.New(node.Constructor, (IEnumerable<Expression>) args, (IEnumerable<MemberInfo>) node.Members);
  }

  protected virtual Expression VisitNewArray(NewArrayExpression node)
  {
    int count = this._yields.Count;
    ReadOnlyCollection<Expression> args = ((ExpressionVisitor) this).Visit(node.Expressions);
    if (args == node.Expressions)
      return (Expression) node;
    if (count != this._yields.Count)
      return (Expression) Expression.Block((Expression) this.ToTemp(ref args), node.NodeType == ExpressionType.NewArrayInit ? (Expression) Expression.NewArrayInit(node.Type.GetElementType(), (IEnumerable<Expression>) args) : (Expression) Expression.NewArrayBounds(node.Type.GetElementType(), (IEnumerable<Expression>) args));
    return node.NodeType != ExpressionType.NewArrayInit ? (Expression) Expression.NewArrayBounds(node.Type.GetElementType(), (IEnumerable<Expression>) args) : (Expression) Expression.NewArrayInit(node.Type.GetElementType(), (IEnumerable<Expression>) args);
  }

  protected virtual Expression VisitMember(MemberExpression node)
  {
    int count = this._yields.Count;
    Expression e = ((ExpressionVisitor) this).Visit(node.Expression);
    if (e == node.Expression)
      return (Expression) node;
    return count == this._yields.Count ? (Expression) Expression.MakeMemberAccess(e, node.Member) : (Expression) Expression.Block(this.ToTemp(ref e), (Expression) Expression.MakeMemberAccess(e, node.Member));
  }

  protected virtual Expression VisitBinary(BinaryExpression node)
  {
    if (node.NodeType == ExpressionType.Assign)
      return this.VisitAssign(node);
    if (node.CanReduce)
      return ((ExpressionVisitor) this).Visit(node.Reduce());
    int count = this._yields.Count;
    Expression e1 = ((ExpressionVisitor) this).Visit(node.Left);
    Expression e2 = ((ExpressionVisitor) this).Visit(node.Right);
    if (e1 == node.Left && e2 == node.Right)
      return (Expression) node;
    return count == this._yields.Count ? (Expression) Expression.MakeBinary(node.NodeType, e1, e2, node.IsLiftedToNull, node.Method, node.Conversion) : (Expression) Expression.Block(this.ToTemp(ref e1), this.ToTemp(ref e2), (Expression) Expression.MakeBinary(node.NodeType, e1, e2, node.IsLiftedToNull, node.Method, node.Conversion));
  }

  protected virtual Expression VisitTypeBinary(TypeBinaryExpression node)
  {
    int count = this._yields.Count;
    Expression e = ((ExpressionVisitor) this).Visit(node.Expression);
    if (e == node.Expression)
      return (Expression) node;
    if (count != this._yields.Count)
      return (Expression) Expression.Block(this.ToTemp(ref e), node.NodeType == ExpressionType.TypeIs ? (Expression) Expression.TypeIs(e, node.TypeOperand) : (Expression) Expression.TypeEqual(e, node.TypeOperand));
    return node.NodeType != ExpressionType.TypeIs ? (Expression) Expression.TypeEqual(e, node.TypeOperand) : (Expression) Expression.TypeIs(e, node.TypeOperand);
  }

  protected virtual Expression VisitUnary(UnaryExpression node)
  {
    if (node.CanReduce)
      return ((ExpressionVisitor) this).Visit(node.Reduce());
    int count = this._yields.Count;
    Expression e = ((ExpressionVisitor) this).Visit(node.Operand);
    if (e == node.Operand)
      return (Expression) node;
    return count == this._yields.Count || node.NodeType == ExpressionType.Convert && node.Type == typeof (void) ? (Expression) Expression.MakeUnary(node.NodeType, e, node.Type, node.Method) : (Expression) Expression.Block(this.ToTemp(ref e), (Expression) Expression.MakeUnary(node.NodeType, e, node.Type, node.Method));
  }

  protected virtual Expression VisitMemberInit(MemberInitExpression node)
  {
    int count1 = this._yields.Count;
    // ISSUE: explicit non-virtual call
    Expression expression = __nonvirtual (((ExpressionVisitor) this).VisitMemberInit(node));
    int count2 = this._yields.Count;
    return count1 == count2 ? expression : expression.Reduce();
  }

  protected virtual Expression VisitListInit(ListInitExpression node)
  {
    int count1 = this._yields.Count;
    // ISSUE: explicit non-virtual call
    Expression expression = __nonvirtual (((ExpressionVisitor) this).VisitListInit(node));
    int count2 = this._yields.Count;
    return count1 == count2 ? expression : expression.Reduce();
  }

  private static Expression DelayedAssign(Expression lhs, Expression rhs)
  {
    return (Expression) new DelayedTupleAssign(lhs, rhs);
  }

  private struct GotoRewriteInfo(Expression variable, LabelTarget voidTarget)
  {
    public readonly Expression Variable = variable;
    public readonly LabelTarget VoidTarget = voidTarget;
  }

  private class GotoRewriter : ExpressionVisitor
  {
    private readonly GeneratorRewriter.GotoRewriteInfo _gotoInfo;
    private readonly LabelTarget _target;
    private readonly GeneratorRewriter _rewriter;

    public GotoRewriter(
      GeneratorRewriter rewriter,
      GeneratorRewriter.GotoRewriteInfo gotoInfo,
      LabelTarget target)
    {
      this._gotoInfo = gotoInfo;
      this._target = target;
      this._rewriter = rewriter;
    }

    protected override Expression VisitGoto(GotoExpression node)
    {
      return node.Target == this._target ? (Expression) Expression.Goto(this._gotoInfo.VoidTarget, (Expression) Expression.Block(this._rewriter.MakeAssign(this._gotoInfo.Variable, node.Value), (Expression) Expression.Default(typeof (void))), node.Type) : base.VisitGoto(node);
    }
  }

  private class RethrowRewriter : ExpressionVisitor
  {
    internal Expression Exception;

    protected override Expression VisitUnary(UnaryExpression node)
    {
      return node.NodeType == ExpressionType.Throw && node.Operand == null ? (Expression) Expression.Throw(this.Exception, node.Type) : base.VisitUnary(node);
    }

    protected override Expression VisitLambda<T>(Expression<T> node) => (Expression) node;

    protected override Expression VisitTry(TryExpression node) => (Expression) node;

    protected override Expression VisitExtension(Expression node)
    {
      return node is DelayedTupleExpression ? node : base.VisitExtension(node);
    }
  }

  private sealed class YieldMarker
  {
    internal LabelTarget Label = Expression.Label("yieldMarker");
    internal readonly int State;

    internal YieldMarker(int state) => this.State = state;
  }
}
