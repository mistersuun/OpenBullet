// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LoopCompiler
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class LoopCompiler : ExpressionVisitor
{
  private readonly ParameterExpression _frameDataVar;
  private readonly ParameterExpression _frameClosureVar;
  private readonly ParameterExpression _frameVar;
  private readonly LabelTarget _returnLabel;
  private readonly Dictionary<ParameterExpression, LocalVariable> _outerVariables;
  private readonly Dictionary<ParameterExpression, LocalVariable> _closureVariables;
  private readonly LoopExpression _loop;
  private ReadOnlyCollectionBuilder<ParameterExpression> _temps;
  private readonly Dictionary<ParameterExpression, LoopCompiler.LoopVariable> _loopVariables;
  private HashSet<ParameterExpression> _loopLocals;
  private readonly HybridReferenceDictionary<LabelTarget, BranchLabel> _labelMapping;
  private readonly int _loopStartInstructionIndex;
  private readonly int _loopEndInstructionIndex;

  internal LoopCompiler(
    LoopExpression loop,
    HybridReferenceDictionary<LabelTarget, BranchLabel> labelMapping,
    Dictionary<ParameterExpression, LocalVariable> locals,
    Dictionary<ParameterExpression, LocalVariable> closureVariables,
    int loopStartInstructionIndex,
    int loopEndInstructionIndex)
  {
    this._loop = loop;
    this._outerVariables = locals;
    this._closureVariables = closureVariables;
    this._frameDataVar = Expression.Parameter(typeof (object[]));
    this._frameClosureVar = Expression.Parameter(typeof (StrongBox<object>[]));
    this._frameVar = Expression.Parameter(typeof (InterpretedFrame));
    this._loopVariables = new Dictionary<ParameterExpression, LoopCompiler.LoopVariable>();
    this._returnLabel = Expression.Label(typeof (int));
    this._labelMapping = labelMapping;
    this._loopStartInstructionIndex = loopStartInstructionIndex;
    this._loopEndInstructionIndex = loopEndInstructionIndex;
  }

  internal Func<object[], StrongBox<object>[], InterpretedFrame, int> CreateDelegate()
  {
    LoopExpression body = (LoopExpression) this.Visit((Expression) this._loop);
    ReadOnlyCollectionBuilder<Expression> collectionBuilder1 = new ReadOnlyCollectionBuilder<Expression>();
    ReadOnlyCollectionBuilder<Expression> collectionBuilder2 = new ReadOnlyCollectionBuilder<Expression>();
    foreach (KeyValuePair<ParameterExpression, LoopCompiler.LoopVariable> loopVariable in this._loopVariables)
    {
      LocalVariable closureVariable;
      if (!this._outerVariables.TryGetValue(loopVariable.Key, out closureVariable))
        closureVariable = this._closureVariables[loopVariable.Key];
      Expression expression = closureVariable.LoadFromArray((Expression) this._frameDataVar, (Expression) this._frameClosureVar);
      if (closureVariable.InClosureOrBoxed)
      {
        ParameterExpression boxStorage = loopVariable.Value.BoxStorage;
        collectionBuilder1.Add((Expression) Expression.Assign((Expression) boxStorage, expression));
        this.AddTemp(boxStorage);
      }
      else
      {
        collectionBuilder1.Add((Expression) Expression.Assign((Expression) loopVariable.Key, Microsoft.Scripting.Ast.Utils.Convert(expression, loopVariable.Key.Type)));
        if ((loopVariable.Value.Access & ExpressionAccess.Write) != ExpressionAccess.None)
          collectionBuilder2.Add((Expression) Expression.Assign(expression, Microsoft.Scripting.Ast.Utils.Box((Expression) loopVariable.Key)));
        this.AddTemp(loopVariable.Key);
      }
    }
    if (collectionBuilder2.Count > 0)
      collectionBuilder1.Add((Expression) Expression.TryFinally((Expression) body, (Expression) Expression.Block((IEnumerable<Expression>) collectionBuilder2)));
    else
      collectionBuilder1.Add((Expression) body);
    collectionBuilder1.Add((Expression) Expression.Label(this._returnLabel, (Expression) Expression.Constant((object) (this._loopEndInstructionIndex - this._loopStartInstructionIndex))));
    return Expression.Lambda<Func<object[], StrongBox<object>[], InterpretedFrame, int>>((Expression) (this._temps != null ? Expression.Block((IEnumerable<ParameterExpression>) this._temps.ToReadOnlyCollection(), (IEnumerable<Expression>) collectionBuilder1) : Expression.Block((IEnumerable<Expression>) collectionBuilder1)), this._frameDataVar, this._frameClosureVar, this._frameVar).Compile();
  }

  protected override Expression VisitExtension(Expression node)
  {
    return node.CanReduce ? this.Visit(node.Reduce()) : base.VisitExtension(node);
  }

  protected override Expression VisitGoto(GotoExpression node)
  {
    LabelTarget target = node.Target;
    Expression expression = this.Visit(node.Value);
    BranchLabel branchLabel;
    if (!this._labelMapping.TryGetValue(target, out branchLabel))
      return (Expression) node.Update(target, expression);
    if (branchLabel.TargetIndex >= this._loopStartInstructionIndex && branchLabel.TargetIndex < this._loopEndInstructionIndex)
      return (Expression) node.Update(target, expression);
    LabelTarget returnLabel = this._returnLabel;
    MethodCallExpression methodCallExpression;
    if (expression == null)
      methodCallExpression = Expression.Call((Expression) this._frameVar, InterpretedFrame.VoidGotoMethod, (Expression) Expression.Constant((object) branchLabel.LabelIndex));
    else
      methodCallExpression = Expression.Call((Expression) this._frameVar, InterpretedFrame.GotoMethod, (Expression) Expression.Constant((object) branchLabel.LabelIndex), Microsoft.Scripting.Ast.Utils.Box(expression));
    Type type = node.Type;
    return (Expression) Expression.Return(returnLabel, (Expression) methodCallExpression, type);
  }

  protected override Expression VisitBlock(BlockExpression node)
  {
    HashSet<ParameterExpression> prevLocals = this.EnterVariableScope((ICollection<ParameterExpression>) node.Variables);
    Expression expression = base.VisitBlock(node);
    this.ExitVariableScope(prevLocals);
    return expression;
  }

  private HashSet<ParameterExpression> EnterVariableScope(ICollection<ParameterExpression> variables)
  {
    if (this._loopLocals == null)
    {
      this._loopLocals = new HashSet<ParameterExpression>((IEnumerable<ParameterExpression>) variables);
      return (HashSet<ParameterExpression>) null;
    }
    HashSet<ParameterExpression> parameterExpressionSet = new HashSet<ParameterExpression>((IEnumerable<ParameterExpression>) this._loopLocals);
    this._loopLocals.UnionWith((IEnumerable<ParameterExpression>) variables);
    return parameterExpressionSet;
  }

  protected override CatchBlock VisitCatchBlock(CatchBlock node)
  {
    if (node.Variable == null)
      return base.VisitCatchBlock(node);
    HashSet<ParameterExpression> prevLocals = this.EnterVariableScope((ICollection<ParameterExpression>) new ParameterExpression[1]
    {
      node.Variable
    });
    CatchBlock catchBlock = base.VisitCatchBlock(node);
    this.ExitVariableScope(prevLocals);
    return catchBlock;
  }

  protected override Expression VisitLambda<T>(Expression<T> node)
  {
    HashSet<ParameterExpression> prevLocals = this.EnterVariableScope((ICollection<ParameterExpression>) node.Parameters);
    try
    {
      return base.VisitLambda<T>(node);
    }
    finally
    {
      this.ExitVariableScope(prevLocals);
    }
  }

  private void ExitVariableScope(HashSet<ParameterExpression> prevLocals)
  {
    this._loopLocals = prevLocals;
  }

  protected override Expression VisitBinary(BinaryExpression node)
  {
    if (node.CanReduce)
      return this.Visit(node.Reduce());
    if (!(node.Left is ParameterExpression left1) || node.NodeType != ExpressionType.Assign)
      return base.VisitBinary(node);
    Expression left2 = this.VisitVariable(left1, ExpressionAccess.Write);
    Expression expression = this.Visit(node.Right);
    if (!(left2.Type != left1.Type))
      return (Expression) node.Update(left2, expression);
    Expression left3;
    if (expression.NodeType != ExpressionType.Parameter)
    {
      left3 = (Expression) this.AddTemp(Expression.Parameter(expression.Type));
      expression = (Expression) Expression.Assign(left3, expression);
    }
    else
      left3 = expression;
    return (Expression) Expression.Block((Expression) node.Update(left2, (Expression) Expression.Convert(expression, left2.Type)), left3);
  }

  protected override Expression VisitUnary(UnaryExpression node)
  {
    return node.CanReduce ? this.Visit(node.Reduce()) : base.VisitUnary(node);
  }

  protected override Expression VisitParameter(ParameterExpression node)
  {
    return this.VisitVariable(node, ExpressionAccess.Read);
  }

  private Expression VisitVariable(ParameterExpression node, ExpressionAccess access)
  {
    if (this._loopLocals.Contains(node))
      return (Expression) node;
    LoopCompiler.LoopVariable loopVariable;
    ParameterExpression parameterExpression;
    if (this._loopVariables.TryGetValue(node, out loopVariable))
    {
      parameterExpression = loopVariable.BoxStorage;
      this._loopVariables[node] = new LoopCompiler.LoopVariable(loopVariable.Access | access, parameterExpression);
    }
    else
    {
      LocalVariable localVariable;
      if (!this._outerVariables.TryGetValue(node, out localVariable) && (this._closureVariables == null || !this._closureVariables.TryGetValue(node, out localVariable)))
        return (Expression) node;
      parameterExpression = localVariable.InClosureOrBoxed ? Expression.Parameter(typeof (StrongBox<object>), node.Name) : (ParameterExpression) null;
      this._loopVariables[node] = new LoopCompiler.LoopVariable(access, parameterExpression);
    }
    if (parameterExpression == null)
      return (Expression) node;
    return (access & ExpressionAccess.Write) != ExpressionAccess.None ? LightCompiler.Unbox((Expression) parameterExpression) : (Expression) Expression.Convert(LightCompiler.Unbox((Expression) parameterExpression), node.Type);
  }

  private ParameterExpression AddTemp(ParameterExpression variable)
  {
    if (this._temps == null)
      this._temps = new ReadOnlyCollectionBuilder<ParameterExpression>();
    this._temps.Add(variable);
    return variable;
  }

  private struct LoopVariable(ExpressionAccess access, ParameterExpression box)
  {
    public readonly ExpressionAccess Access = access;
    public readonly ParameterExpression BoxStorage = box;

    public override string ToString() => $"{(object) this.Access} {(object) this.BoxStorage}";
  }
}
