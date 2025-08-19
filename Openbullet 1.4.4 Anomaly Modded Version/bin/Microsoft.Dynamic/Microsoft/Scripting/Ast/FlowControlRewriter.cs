// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.FlowControlRewriter
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Ast;

internal sealed class FlowControlRewriter : ExpressionVisitor
{
  private readonly Dictionary<LabelTarget, FlowControlRewriter.LabelInfo> _labels = new Dictionary<LabelTarget, FlowControlRewriter.LabelInfo>();
  private readonly Stack<FlowControlRewriter.BlockInfo> _blocks = new Stack<FlowControlRewriter.BlockInfo>();
  private ParameterExpression _flowVariable;

  internal Expression Reduce(Expression node)
  {
    this._blocks.Push(new FlowControlRewriter.BlockInfo());
    node = this.Visit(node);
    if (this._flowVariable != null)
    {
      List<ParameterExpression> variables = new List<ParameterExpression>();
      variables.Add(this._flowVariable);
      foreach (FlowControlRewriter.LabelInfo labelInfo in this._labels.Values)
      {
        if (labelInfo.Variable != null)
          variables.Add(labelInfo.Variable);
      }
      node = (Expression) Expression.Block((IEnumerable<ParameterExpression>) variables, node);
    }
    this._blocks.Pop();
    return node;
  }

  private void EnsureFlow(FlowControlRewriter.BlockInfo block)
  {
    if (this._flowVariable == null)
      this._flowVariable = Expression.Variable(typeof (int), "$flow");
    if (block.HasFlow)
      return;
    block.FlowLabel = Expression.Label();
    block.NeedFlowLabels = new HashSet<LabelTarget>();
  }

  private FlowControlRewriter.LabelInfo EnsureLabelInfo(LabelTarget target)
  {
    FlowControlRewriter.LabelInfo labelInfo1;
    if (!this._labels.TryGetValue(target, out labelInfo1))
    {
      Dictionary<LabelTarget, FlowControlRewriter.LabelInfo> labels = this._labels;
      LabelTarget key = target;
      labelInfo1 = new FlowControlRewriter.LabelInfo(this._labels.Count + 1, target.Type);
      FlowControlRewriter.LabelInfo labelInfo2 = labelInfo1;
      labels.Add(key, labelInfo2);
    }
    return labelInfo1;
  }

  protected override Expression VisitExtension(Expression node)
  {
    if (node is FinallyFlowControlExpression controlExpression)
      return this.Visit(controlExpression.Body);
    return node.CanReduce ? this.Visit(node.Reduce()) : base.VisitExtension(node);
  }

  protected override Expression VisitLambda<T>(Expression<T> node) => (Expression) node;

  protected override Expression VisitTry(TryExpression node)
  {
    FlowControlRewriter.BlockInfo block = new FlowControlRewriter.BlockInfo()
    {
      InFinally = true
    };
    this._blocks.Push(block);
    Expression expression1 = this.Visit(node.Finally);
    Expression fault = this.Visit(node.Fault);
    block.InFinally = false;
    LabelTarget flowLabel = block.FlowLabel;
    if (flowLabel != null)
      block.FlowLabel = Expression.Label();
    Expression body = this.Visit(node.Body);
    IList<CatchBlock> handlers1 = (IList<CatchBlock>) ExpressionVisitor.Visit<CatchBlock>(node.Handlers, new Func<CatchBlock, CatchBlock>(((ExpressionVisitor) this).VisitCatchBlock));
    this._blocks.Pop();
    if (body == node.Body && handlers1 == node.Handlers && expression1 == node.Finally && fault == node.Fault)
      return (Expression) node;
    if (!block.HasFlow)
      return (Expression) Expression.MakeTry((Type) null, body, expression1, fault, (IEnumerable<CatchBlock>) handlers1);
    if (node.Type != typeof (void))
      throw new NotSupportedException("FinallyFlowControlExpression does not support TryExpressions of non-void type.");
    if (handlers1.Count > 0)
      body = (Expression) Expression.MakeTry((Type) null, body, (Expression) null, (Expression) null, (IEnumerable<CatchBlock>) handlers1);
    ParameterExpression left = Expression.Variable(typeof (Exception), "$exception");
    ParameterExpression parameterExpression = Expression.Variable(typeof (Exception), "e");
    IList<CatchBlock> handlers2;
    if (expression1 != null)
    {
      handlers2 = (IList<CatchBlock>) new CatchBlock[1]
      {
        Expression.Catch(parameterExpression, (Expression) Expression.Block((Expression) Expression.Assign((Expression) left, (Expression) parameterExpression), (Expression) Microsoft.Scripting.Ast.Utils.Default(node.Type)))
      };
      expression1 = (Expression) Expression.Block(expression1, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) left, (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, left.Type)), (Expression) Expression.Throw((Expression) left), (Expression) Microsoft.Scripting.Ast.Utils.Empty()));
      if (flowLabel != null)
        expression1 = (Expression) Expression.Label(flowLabel, expression1);
    }
    else
    {
      Expression expression2 = (Expression) Expression.Block(fault, (Expression) Expression.Throw((Expression) parameterExpression));
      if (flowLabel != null)
        expression2 = (Expression) Expression.Label(flowLabel, expression2);
      handlers2 = (IList<CatchBlock>) new CatchBlock[1]
      {
        Expression.Catch(parameterExpression, expression2)
      };
      fault = (Expression) null;
    }
    return (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) Expression.MakeTry((Type) null, body, expression1, fault, (IEnumerable<CatchBlock>) handlers2), (Expression) Expression.Label(block.FlowLabel), this.MakeFlowControlSwitch(block));
  }

  private Expression MakeFlowControlSwitch(FlowControlRewriter.BlockInfo block)
  {
    return (Expression) Expression.Switch((Expression) this._flowVariable, (Expression) null, (MethodInfo) null, (IEnumerable<SwitchCase>) new ReadOnlyCollection<SwitchCase>((IList<SwitchCase>) block.NeedFlowLabels.Map<LabelTarget, SwitchCase>((Func<LabelTarget, SwitchCase>) (target => Expression.SwitchCase(this.MakeFlowJump(target), Microsoft.Scripting.Ast.Utils.Constant((object) this._labels[target].FlowState))))));
  }

  private Expression MakeFlowJump(LabelTarget target)
  {
    foreach (FlowControlRewriter.BlockInfo block in this._blocks)
    {
      if (!block.LabelDefs.Contains(target))
      {
        if (block.InFinally || block.HasFlow)
        {
          this.EnsureFlow(block);
          block.NeedFlowLabels.Add(target);
          return (Expression) Expression.Goto(block.FlowLabel);
        }
      }
      else
        break;
    }
    return (Expression) Expression.Block((Expression) Expression.Assign((Expression) this._flowVariable, Microsoft.Scripting.Ast.Utils.Constant((object) 0)), (Expression) Expression.Goto(target, (Expression) this._labels[target].Variable));
  }

  protected override Expression VisitGoto(GotoExpression node)
  {
    foreach (FlowControlRewriter.BlockInfo block in this._blocks)
    {
      if (!block.LabelDefs.Contains(node.Target))
      {
        if (block.InFinally || block.HasFlow)
        {
          this.EnsureFlow(block);
          block.NeedFlowLabels.Add(node.Target);
          FlowControlRewriter.LabelInfo labelInfo = this.EnsureLabelInfo(node.Target);
          BinaryExpression binaryExpression = Expression.Assign((Expression) this._flowVariable, Microsoft.Scripting.Ast.Utils.Constant((object) labelInfo.FlowState));
          GotoExpression gotoExpression = Expression.Goto(block.FlowLabel, node.Type);
          return (Expression) Expression.Block(labelInfo.Variable != null ? (Expression) Expression.Assign((Expression) labelInfo.Variable, node.Value) : node.Value ?? (Expression) Microsoft.Scripting.Ast.Utils.Empty(), (Expression) binaryExpression, (Expression) gotoExpression);
        }
      }
      else
        break;
    }
    return base.VisitGoto(node);
  }

  protected override Expression VisitBlock(BlockExpression node)
  {
    foreach (Expression expression in node.Expressions)
    {
      if (expression is LabelExpression labelExpression)
      {
        this.VisitLabelTarget(labelExpression.Target);
        if (labelExpression.DefaultValue is BlockExpression defaultValue)
          this.VisitBlock(defaultValue);
      }
    }
    return base.VisitBlock(node);
  }

  protected override LabelTarget VisitLabelTarget(LabelTarget node)
  {
    if (node != null)
    {
      this.EnsureLabelInfo(node);
      this._blocks.Peek().LabelDefs.Add(node);
    }
    return node;
  }

  private sealed class BlockInfo
  {
    internal bool InFinally;
    internal readonly HashSet<LabelTarget> LabelDefs = new HashSet<LabelTarget>();
    internal HashSet<LabelTarget> NeedFlowLabels;
    internal LabelTarget FlowLabel;

    internal bool HasFlow => this.FlowLabel != null;
  }

  private struct LabelInfo
  {
    internal readonly int FlowState;
    internal readonly ParameterExpression Variable;

    internal LabelInfo(int index, Type varType)
    {
      this.FlowState = index;
      if (varType != typeof (void))
        this.Variable = Expression.Variable(varType, (string) null);
      else
        this.Variable = (ParameterExpression) null;
    }
  }
}
