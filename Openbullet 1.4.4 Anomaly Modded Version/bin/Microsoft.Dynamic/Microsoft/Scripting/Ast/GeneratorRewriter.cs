// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.GeneratorRewriter
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

internal sealed class GeneratorRewriter : DynamicExpressionVisitor
{
  private const int GotoRouterYielding = 0;
  private const int GotoRouterNone = -1;
  internal const int NotStarted = -1;
  internal const int Finished = 0;
  private readonly GeneratorExpression _generator;
  private readonly ParameterExpression _current;
  private readonly ParameterExpression _state;
  private readonly Stack<LabelTarget> _returnLabels = new Stack<LabelTarget>();
  private ParameterExpression _gotoRouter;
  private bool _inTryWithFinally;
  private readonly List<GeneratorRewriter.YieldMarker> _yields = new List<GeneratorRewriter.YieldMarker>();
  private List<int> _debugCookies;
  private readonly HashSet<ParameterExpression> _vars = new HashSet<ParameterExpression>();
  private readonly List<ParameterExpression> _temps = new List<ParameterExpression>();
  private Dictionary<LabelTarget, GeneratorRewriter.LabelInfo> _labelTemps;

  internal GeneratorRewriter(GeneratorExpression generator)
  {
    this._generator = generator;
    this._state = Expression.Parameter(typeof (int).MakeByRefType(), "state");
    this._current = Expression.Parameter(this._generator.Target.Type.MakeByRefType(), "current");
    this._returnLabels.Push(Expression.Label());
    this._gotoRouter = Expression.Variable(typeof (int), "$gotoRouter");
  }

  internal Expression Reduce()
  {
    Expression expression1 = ((ExpressionVisitor) this).Visit(this._generator.Body);
    int count = this._yields.Count;
    SwitchCase[] switchCaseArray = new SwitchCase[count + 1];
    for (int index = 0; index < count; ++index)
      switchCaseArray[index] = Expression.SwitchCase((Expression) Expression.Goto(this._yields[index].Label), Microsoft.Scripting.Ast.Utils.Constant((object) this._yields[index].State));
    switchCaseArray[count] = Expression.SwitchCase((Expression) Expression.Goto(this._returnLabels.Peek()), Microsoft.Scripting.Ast.Utils.Constant((object) 0));
    Type delegateType = typeof (GeneratorNext<>).MakeGenericType(this._generator.Target.Type);
    List<ParameterExpression> variables1 = new List<ParameterExpression>((IEnumerable<ParameterExpression>) this._vars);
    variables1.AddRange((IEnumerable<ParameterExpression>) this._temps);
    Dictionary<LabelTarget, GeneratorRewriter.LabelInfo> labelTemps = this._labelTemps;
    // ISSUE: explicit non-virtual call
    ReadOnlyCollectionBuilder<ParameterExpression> variables2 = new ReadOnlyCollectionBuilder<ParameterExpression>(1 + (labelTemps != null ? __nonvirtual (labelTemps.Count) : 0));
    variables2.Add(this._gotoRouter);
    if (this._labelTemps != null)
    {
      foreach (GeneratorRewriter.LabelInfo labelInfo in this._labelTemps.Values)
        variables2.Add(labelInfo.Temp);
    }
    Expression body = (Expression) Expression.Block((IEnumerable<ParameterExpression>) variables1, (Expression) Expression.Lambda(delegateType, (Expression) Expression.Block((IEnumerable<ParameterExpression>) variables2, (Expression) Expression.Switch((Expression) Expression.Assign((Expression) this._gotoRouter, (Expression) this._state), switchCaseArray), expression1, (Expression) Expression.Assign((Expression) this._state, Microsoft.Scripting.Ast.Utils.Constant((object) 0)), (Expression) Expression.Label(this._returnLabels.Peek())), this._generator.Name, (IEnumerable<ParameterExpression>) new ParameterExpression[2]
    {
      this._state,
      this._current
    }));
    if (this._generator.IsEnumerable)
      body = (Expression) Expression.Lambda(body);
    Expression expression2 = (Expression) null;
    if (this._debugCookies != null)
    {
      Expression[] expressionArray = new Expression[this._debugCookies.Count];
      for (int index = 0; index < this._debugCookies.Count; ++index)
        expressionArray[index] = Microsoft.Scripting.Ast.Utils.Constant((object) this._debugCookies[index]);
      expression2 = (Expression) Expression.NewArrayInit(typeof (int), expressionArray);
    }
    Type type = typeof (ScriptingRuntimeHelpers);
    Type[] typeArguments = new Type[1]
    {
      this._generator.Target.Type
    };
    Expression[] expressionArray1;
    if (expression2 == null)
      expressionArray1 = new Expression[1]{ body };
    else
      expressionArray1 = new Expression[2]
      {
        body,
        expression2
      };
    return (Expression) Expression.Call(type, "MakeGenerator", typeArguments, expressionArray1);
  }

  private GeneratorRewriter.YieldMarker GetYieldMarker(YieldExpression node)
  {
    GeneratorRewriter.YieldMarker yieldMarker = new GeneratorRewriter.YieldMarker(this._yields.Count + 1);
    this._yields.Add(yieldMarker);
    if (node.YieldMarker != -1)
    {
      if (this._debugCookies == null)
      {
        this._debugCookies = new List<int>(1);
        this._debugCookies.Add(int.MaxValue);
      }
      this._debugCookies.Insert(yieldMarker.State, node.YieldMarker);
    }
    else
      this._debugCookies?.Insert(yieldMarker.State, int.MaxValue);
    return yieldMarker;
  }

  private Expression MakeAssign(ParameterExpression variable, Expression value)
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
        return (Expression) Expression.Assign((Expression) variable, value);
    }
  }

  private Expression MakeAssignLabel(ParameterExpression variable, LabelExpression value)
  {
    GeneratorRewriter.GotoRewriteInfo gotoRewriteInfo = new GeneratorRewriter.GotoRewriteInfo(variable, Expression.Label(value.Target.Name + "_voided"));
    Expression defaultValue = new GeneratorRewriter.GotoRewriter(this, gotoRewriteInfo, value.Target).Visit(value.DefaultValue);
    return this.MakeAssignLabel(variable, gotoRewriteInfo, defaultValue);
  }

  private Expression MakeAssignLabel(
    ParameterExpression variable,
    GeneratorRewriter.GotoRewriteInfo curVariable,
    Expression defaultValue)
  {
    return (Expression) Expression.Label(curVariable.VoidTarget, this.MakeAssign(variable, defaultValue));
  }

  private Expression MakeAssignBlock(ParameterExpression variable, Expression value)
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
      collectionBuilder[collectionBuilder.Count - 1] = this.MakeAssignLabel(variable, gotoRewriteInfo, gotoRewriter.Visit(labelExpression.DefaultValue));
    }
    else
      collectionBuilder[collectionBuilder.Count - 1] = this.MakeAssign(variable, collectionBuilder[collectionBuilder.Count - 1]);
    return (Expression) Expression.Block((IEnumerable<ParameterExpression>) blockExpression.Variables, (IEnumerable<Expression>) collectionBuilder);
  }

  private Expression MakeAssignConditional(ParameterExpression variable, Expression value)
  {
    ConditionalExpression conditionalExpression = (ConditionalExpression) value;
    return (Expression) Expression.Condition(conditionalExpression.Test, this.MakeAssign(variable, conditionalExpression.IfTrue), this.MakeAssign(variable, conditionalExpression.IfFalse));
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
    this._returnLabels.Push(Expression.Label());
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
    LabelTarget labelTarget1 = Expression.Label();
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
            catchBlockList = (IList<CatchBlock>) catchBlockList.ToArray<CatchBlock>();
          ParameterExpression parameterExpression = Expression.Variable(catchBlock.Test, (string) null);
          ParameterExpression left = catchBlock.Variable ?? Expression.Variable(catchBlock.Test, (string) null);
          this._vars.Add(left);
          Expression filter = catchBlock.Filter;
          if (filter != null && catchBlock.Variable != null)
            filter = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
            {
              catchBlock.Variable
            }, (Expression) Expression.Assign((Expression) catchBlock.Variable, (Expression) parameterExpression), filter);
          catchBlockList[index] = Expression.Catch(parameterExpression, (Expression) Expression.Block((Expression) Expression.Assign((Expression) left, (Expression) parameterExpression), (Expression) Expression.Default(node.Body.Type)), filter);
          Expression ifTrue = new GeneratorRewriter.RethrowRewriter()
          {
            Exception = left
          }.Visit(catchBlock.Body);
          expressionList.Add((Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) left, (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, left.Type)), ifTrue, (Expression) Expression.Default(node.Body.Type)));
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
      LabelTarget labelTarget2 = Expression.Label();
      Expression expression1 = (Expression) this.MakeYieldRouter(node.Body.Type, count3, count4, labelTarget2);
      SwitchExpression switchExpression = this.MakeYieldRouter(node.Body.Type, count3, count4, labelTarget1);
      ParameterExpression parameterExpression = Expression.Variable(typeof (Exception), "e");
      ParameterExpression left = Expression.Variable(typeof (Exception), "$saved$" + (object) this._temps.Count);
      this._temps.Add(left);
      Expression expression2 = body;
      BinaryExpression binaryExpression = Expression.Assign((Expression) left, (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, left.Type));
      LabelExpression labelExpression = Expression.Label(labelTarget2);
      body = (Expression) Expression.Block((Expression) Expression.TryCatchFinally((Expression) Expression.Block((Expression) switchExpression, expression2, (Expression) binaryExpression, (Expression) labelExpression), (Expression) Expression.Block(this.MakeSkipFinallyBlock(target), expression1, @finally, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) left, (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, left.Type)), (Expression) Expression.Throw((Expression) left), (Expression) Microsoft.Scripting.Ast.Utils.Empty()), (Expression) Expression.Label(target)), Expression.Catch(parameterExpression, Microsoft.Scripting.Ast.Utils.Void((Expression) Expression.Assign((Expression) left, (Expression) parameterExpression)))), (Expression) Expression.Condition((Expression) Expression.Equal((Expression) this._gotoRouter, Microsoft.Scripting.Ast.Utils.Constant((object) 0)), (Expression) Expression.Goto(this._returnLabels.Peek()), (Expression) Microsoft.Scripting.Ast.Utils.Empty()));
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
    return (Expression) Expression.Condition((Expression) Expression.AndAlso((Expression) Expression.Equal((Expression) this._gotoRouter, Microsoft.Scripting.Ast.Utils.Constant((object) 0)), (Expression) Expression.NotEqual((Expression) this._state, Microsoft.Scripting.Ast.Utils.Constant((object) 0))), (Expression) Expression.Goto(target), (Expression) Microsoft.Scripting.Ast.Utils.Empty());
  }

  protected virtual CatchBlock VisitCatchBlock(CatchBlock node)
  {
    ParameterExpression variable = ((ExpressionVisitor) this).VisitAndConvert<ParameterExpression>(node.Variable, nameof (VisitCatchBlock));
    int count1 = this._yields.Count;
    Expression filter = ((ExpressionVisitor) this).Visit(node.Filter);
    int count2 = this._yields.Count;
    if (count1 != count2)
      throw new NotSupportedException("yield in filter is not allowed");
    Expression body = ((ExpressionVisitor) this).Visit(node.Body);
    return variable == node.Variable && body == node.Body && filter == node.Filter ? node : Expression.MakeCatchBlock(node.Test, variable, body, filter);
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
    if (node.Target != this._generator.Target)
      throw new InvalidOperationException("yield and generator must have the same LabelTarget object");
    Expression expression = ((ExpressionVisitor) this).Visit(node.Value);
    ReadOnlyCollectionBuilder<Expression> collectionBuilder = new ReadOnlyCollectionBuilder<Expression>();
    if (expression == null)
    {
      collectionBuilder.Add((Expression) Expression.Assign((Expression) this._state, Microsoft.Scripting.Ast.Utils.Constant((object) 0)));
      if (this._inTryWithFinally)
        collectionBuilder.Add((Expression) Expression.Assign((Expression) this._gotoRouter, Microsoft.Scripting.Ast.Utils.Constant((object) 0)));
      collectionBuilder.Add((Expression) Expression.Goto(this._returnLabels.Peek()));
      return (Expression) Expression.Block((IEnumerable<Expression>) collectionBuilder);
    }
    collectionBuilder.Add(this.MakeAssign(this._current, expression));
    GeneratorRewriter.YieldMarker yieldMarker = this.GetYieldMarker(node);
    collectionBuilder.Add((Expression) Expression.Assign((Expression) this._state, Microsoft.Scripting.Ast.Utils.Constant((object) yieldMarker.State)));
    if (this._inTryWithFinally)
      collectionBuilder.Add((Expression) Expression.Assign((Expression) this._gotoRouter, Microsoft.Scripting.Ast.Utils.Constant((object) 0)));
    collectionBuilder.Add((Expression) Expression.Goto(this._returnLabels.Peek()));
    collectionBuilder.Add((Expression) Expression.Label(yieldMarker.Label));
    collectionBuilder.Add((Expression) Expression.Assign((Expression) this._gotoRouter, Microsoft.Scripting.Ast.Utils.Constant((object) -1)));
    collectionBuilder.Add((Expression) Microsoft.Scripting.Ast.Utils.Empty());
    return (Expression) Expression.Block((IEnumerable<Expression>) collectionBuilder);
  }

  protected virtual Expression VisitBlock(BlockExpression node)
  {
    int count = this._yields.Count;
    ReadOnlyCollection<Expression> readOnlyCollection = ((ExpressionVisitor) this).Visit(node.Expressions);
    if (readOnlyCollection == node.Expressions)
      return (Expression) node;
    if (count == this._yields.Count)
      return (Expression) Expression.Block(node.Type, (IEnumerable<ParameterExpression>) node.Variables, (IEnumerable<Expression>) readOnlyCollection);
    this._vars.UnionWith((IEnumerable<ParameterExpression>) node.Variables);
    return (Expression) Expression.Block(node.Type, (IEnumerable<Expression>) readOnlyCollection);
  }

  protected virtual Expression VisitLambda<T>(Expression<T> node) => (Expression) node;

  protected virtual Expression VisitLabel(LabelExpression node)
  {
    if (node.Target.Type == typeof (void))
    {
      // ISSUE: explicit non-virtual call
      return __nonvirtual (((ExpressionVisitor) this).VisitLabel(node));
    }
    GeneratorRewriter.LabelInfo labelInfo = this.GetLabelInfo(node.Target);
    return (Expression) Expression.Block(this.MakeAssign(labelInfo.Temp, ((ExpressionVisitor) this).Visit(node.DefaultValue)), (Expression) Expression.Label(labelInfo.NewLabel), (Expression) labelInfo.Temp);
  }

  protected virtual Expression VisitGoto(GotoExpression node)
  {
    if (node.Target.Type == typeof (void))
    {
      // ISSUE: explicit non-virtual call
      return __nonvirtual (((ExpressionVisitor) this).VisitGoto(node));
    }
    GeneratorRewriter.LabelInfo labelInfo = this.GetLabelInfo(node.Target);
    return (Expression) Expression.Block(this.MakeAssign(labelInfo.Temp, ((ExpressionVisitor) this).Visit(node.Value)), (Expression) Expression.MakeGoto(node.Kind, labelInfo.NewLabel, (Expression) null, node.Type));
  }

  private GeneratorRewriter.LabelInfo GetLabelInfo(LabelTarget label)
  {
    if (this._labelTemps == null)
      this._labelTemps = new Dictionary<LabelTarget, GeneratorRewriter.LabelInfo>();
    GeneratorRewriter.LabelInfo labelInfo;
    if (!this._labelTemps.TryGetValue(label, out labelInfo))
      this._labelTemps[label] = labelInfo = new GeneratorRewriter.LabelInfo(label);
    return labelInfo;
  }

  private static bool IsConstant(Expression e) => e is ConstantExpression;

  private Expression ToTemp(ReadOnlyCollectionBuilder<Expression> block, Expression e)
  {
    if (GeneratorRewriter.IsConstant(e))
      return e;
    ParameterExpression variable = Expression.Variable(e.Type, "generatorTemp" + (object) this._temps.Count);
    this._temps.Add(variable);
    block.Add(this.MakeAssign(variable, e));
    return (Expression) variable;
  }

  private ReadOnlyCollection<Expression> ToTemp(
    ReadOnlyCollectionBuilder<Expression> block,
    ICollection<Expression> args)
  {
    ReadOnlyCollectionBuilder<Expression> collectionBuilder = new ReadOnlyCollectionBuilder<Expression>(args.Count);
    foreach (Expression e in (IEnumerable<Expression>) args)
      collectionBuilder.Add(this.ToTemp(block, e));
    return collectionBuilder.ToReadOnlyCollection();
  }

  private Expression Rewrite(
    Expression node,
    ReadOnlyCollection<Expression> arguments,
    Func<ReadOnlyCollection<Expression>, Expression> factory)
  {
    return this.Rewrite(node, (Expression) null, arguments, (Func<Expression, ReadOnlyCollection<Expression>, Expression>) ((e, args) => factory(args)));
  }

  private Expression Rewrite(
    Expression node,
    Expression expr,
    ReadOnlyCollection<Expression> arguments,
    Func<Expression, ReadOnlyCollection<Expression>, Expression> factory)
  {
    int count = this._yields.Count;
    Expression e = expr != null ? ((ExpressionVisitor) this).Visit(expr) : (Expression) null;
    ReadOnlyCollection<Expression> args = ((ExpressionVisitor) this).Visit(arguments);
    if (e == expr && args == arguments)
      return node;
    if (count == this._yields.Count)
      return factory(e, args);
    ReadOnlyCollectionBuilder<Expression> block = new ReadOnlyCollectionBuilder<Expression>(args.Count + 1);
    if (e != null)
      e = this.ToTemp(block, e);
    ReadOnlyCollection<Expression> temp = this.ToTemp(block, (ICollection<Expression>) args);
    block.Add(factory(e, temp));
    return (Expression) Expression.Block((IEnumerable<Expression>) block);
  }

  private Expression Rewrite(
    Expression node,
    Expression expr,
    Func<Expression, Expression> factory)
  {
    int count = this._yields.Count;
    Expression e = ((ExpressionVisitor) this).Visit(expr);
    if (e == expr)
      return node;
    if (count == this._yields.Count || GeneratorRewriter.IsConstant(e))
      return factory(e);
    ReadOnlyCollectionBuilder<Expression> block = new ReadOnlyCollectionBuilder<Expression>(2);
    Expression temp = this.ToTemp(block, e);
    block.Add(factory(temp));
    return (Expression) Expression.Block((IEnumerable<Expression>) block);
  }

  private Expression Rewrite(
    Expression node,
    Expression expr1,
    Expression expr2,
    Func<Expression, Expression, Expression> factory)
  {
    int count1 = this._yields.Count;
    Expression e1 = ((ExpressionVisitor) this).Visit(expr1);
    int count2 = this._yields.Count;
    Expression e2 = ((ExpressionVisitor) this).Visit(expr2);
    if (e1 == expr1 && e2 == expr2)
      return node;
    if (count1 == this._yields.Count)
      return factory(e1, e2);
    ReadOnlyCollectionBuilder<Expression> block = new ReadOnlyCollectionBuilder<Expression>(3);
    Expression temp = this.ToTemp(block, e1);
    if (count2 != this._yields.Count)
      e2 = this.ToTemp(block, e2);
    block.Add(factory(temp, e2));
    return (Expression) Expression.Block((IEnumerable<Expression>) block);
  }

  private Expression VisitAssign(BinaryExpression node)
  {
    int count = this._yields.Count;
    Expression left1 = ((ExpressionVisitor) this).Visit(node.Left);
    Expression expression = ((ExpressionVisitor) this).Visit(node.Right);
    if (left1 == node.Left && expression == node.Right)
      return (Expression) node;
    if (count == this._yields.Count)
      return (Expression) Expression.Assign(left1, expression);
    ReadOnlyCollectionBuilder<Expression> collectionBuilder = new ReadOnlyCollectionBuilder<Expression>();
    if (this._generator.RewriteAssignments)
    {
      if (left1 == node.Left)
      {
        switch (left1.NodeType)
        {
          case ExpressionType.MemberAccess:
            MemberExpression left2 = (MemberExpression) node.Left;
            if (left2.Expression != null)
            {
              left1 = (Expression) left2.Update(this.ToTemp(collectionBuilder, left2.Expression));
              break;
            }
            break;
          case ExpressionType.Parameter:
            break;
          case ExpressionType.Index:
            IndexExpression left3 = (IndexExpression) node.Left;
            left1 = (Expression) left3.Update(left3.Object != null ? this.ToTemp(collectionBuilder, left3.Object) : (Expression) null, (IEnumerable<Expression>) this.ToTemp(collectionBuilder, (ICollection<Expression>) left3.Arguments));
            break;
          default:
            throw Assert.Unreachable;
        }
      }
      else
      {
        BlockExpression blockExpression = (BlockExpression) left1;
        collectionBuilder.AddRange<Expression>((IEnumerable<Expression>) blockExpression.Expressions);
        collectionBuilder.RemoveAt(collectionBuilder.Count - 1);
        left1 = blockExpression.Expressions[blockExpression.Expressions.Count - 1];
      }
    }
    if (expression != node.Right)
      expression = this.ToTemp(collectionBuilder, expression);
    collectionBuilder.Add((Expression) Expression.Assign(left1, expression));
    return (Expression) Expression.Block((IEnumerable<Expression>) collectionBuilder);
  }

  protected virtual Expression VisitDynamic(DynamicExpression node)
  {
    return this.Rewrite((Expression) node, node.Arguments, new Func<ReadOnlyCollection<Expression>, Expression>(node.Update));
  }

  protected virtual Expression VisitIndex(IndexExpression node)
  {
    return this.Rewrite((Expression) node, node.Object, node.Arguments, new Func<Expression, ReadOnlyCollection<Expression>, Expression>(node.Update));
  }

  protected virtual Expression VisitInvocation(InvocationExpression node)
  {
    return this.Rewrite((Expression) node, node.Expression, node.Arguments, new Func<Expression, ReadOnlyCollection<Expression>, Expression>(node.Update));
  }

  protected virtual Expression VisitMethodCall(MethodCallExpression node)
  {
    return this.Rewrite((Expression) node, node.Object, node.Arguments, new Func<Expression, ReadOnlyCollection<Expression>, Expression>(node.Update));
  }

  protected virtual Expression VisitNew(NewExpression node)
  {
    return this.Rewrite((Expression) node, node.Arguments, new Func<ReadOnlyCollection<Expression>, Expression>(node.Update));
  }

  protected virtual Expression VisitNewArray(NewArrayExpression node)
  {
    return this.Rewrite((Expression) node, node.Expressions, new Func<ReadOnlyCollection<Expression>, Expression>(node.Update));
  }

  protected virtual Expression VisitMember(MemberExpression node)
  {
    return this.Rewrite((Expression) node, node.Expression, new Func<Expression, Expression>(node.Update));
  }

  protected virtual Expression VisitBinary(BinaryExpression node)
  {
    if (node.NodeType == ExpressionType.Assign)
      return this.VisitAssign(node);
    return node.CanReduce ? ((ExpressionVisitor) this).Visit(node.Reduce()) : this.Rewrite((Expression) node, node.Left, node.Right, new Func<Expression, Expression, Expression>(((Microsoft.Scripting.Ast.Utils) node).Update));
  }

  protected virtual Expression VisitTypeBinary(TypeBinaryExpression node)
  {
    return this.Rewrite((Expression) node, node.Expression, new Func<Expression, Expression>(node.Update));
  }

  protected virtual Expression VisitUnary(UnaryExpression node)
  {
    return node.CanReduce ? ((ExpressionVisitor) this).Visit(node.Reduce()) : this.Rewrite((Expression) node, node.Operand, new Func<Expression, Expression>(node.Update));
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

  private sealed class YieldMarker
  {
    internal LabelTarget Label = Expression.Label();
    internal readonly int State;

    internal YieldMarker(int state) => this.State = state;
  }

  private sealed class LabelInfo
  {
    internal readonly LabelTarget NewLabel;
    internal readonly ParameterExpression Temp;

    internal LabelInfo(LabelTarget old)
    {
      this.NewLabel = Expression.Label(old.Name);
      this.Temp = Expression.Parameter(old.Type, old.Name);
    }
  }

  private struct GotoRewriteInfo(ParameterExpression variable, LabelTarget voidTarget)
  {
    public readonly ParameterExpression Variable = variable;
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
    internal ParameterExpression Exception;

    protected override Expression VisitUnary(UnaryExpression node)
    {
      return node.NodeType == ExpressionType.Throw && node.Operand == null ? (Expression) Expression.Throw((Expression) this.Exception, node.Type) : base.VisitUnary(node);
    }

    protected override Expression VisitLambda<T>(Expression<T> node) => (Expression) node;

    protected override Expression VisitTry(TryExpression node) => (Expression) node;
  }
}
