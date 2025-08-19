// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LightLambdaClosureVisitor
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class LightLambdaClosureVisitor : ExpressionVisitor
{
  private readonly Dictionary<ParameterExpression, LocalVariable> _closureVars;
  private readonly ParameterExpression _closureArray;
  private readonly Stack<HashSet<ParameterExpression>> _shadowedVars = new Stack<HashSet<ParameterExpression>>();

  private LightLambdaClosureVisitor(
    Dictionary<ParameterExpression, LocalVariable> closureVariables,
    ParameterExpression closureArray)
  {
    this._closureArray = closureArray;
    this._closureVars = closureVariables;
  }

  internal static Func<StrongBox<object>[], Delegate> BindLambda(
    LambdaExpression lambda,
    Dictionary<ParameterExpression, LocalVariable> closureVariables)
  {
    ParameterExpression closureArray = Expression.Parameter(typeof (StrongBox<object>[]), "closure");
    lambda = (LambdaExpression) new LightLambdaClosureVisitor(closureVariables, closureArray).Visit((Expression) lambda);
    return Expression.Lambda<Func<StrongBox<object>[], Delegate>>((Expression) lambda, closureArray).Compile();
  }

  protected override Expression VisitLambda<T>(Expression<T> node)
  {
    this._shadowedVars.Push(new HashSet<ParameterExpression>((IEnumerable<ParameterExpression>) node.Parameters));
    Expression body = this.Visit(node.Body);
    this._shadowedVars.Pop();
    return body == node.Body ? (Expression) node : (Expression) Expression.Lambda<T>(body, node.Name, node.TailCall, (IEnumerable<ParameterExpression>) node.Parameters);
  }

  protected override Expression VisitBlock(BlockExpression node)
  {
    if (node.Variables.Count > 0)
      this._shadowedVars.Push(new HashSet<ParameterExpression>((IEnumerable<ParameterExpression>) node.Variables));
    ReadOnlyCollection<Expression> readOnlyCollection = this.Visit(node.Expressions);
    if (node.Variables.Count > 0)
      this._shadowedVars.Pop();
    return readOnlyCollection == node.Expressions ? (Expression) node : (Expression) Expression.Block((IEnumerable<ParameterExpression>) node.Variables, (IEnumerable<Expression>) readOnlyCollection);
  }

  protected override CatchBlock VisitCatchBlock(CatchBlock node)
  {
    if (node.Variable != null)
      this._shadowedVars.Push(new HashSet<ParameterExpression>((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        node.Variable
      }));
    Expression body = this.Visit(node.Body);
    Expression filter = this.Visit(node.Filter);
    if (node.Variable != null)
      this._shadowedVars.Pop();
    return body == node.Body && filter == node.Filter ? node : Expression.MakeCatchBlock(node.Test, node.Variable, body, filter);
  }

  protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
  {
    int count = node.Variables.Count;
    List<Expression> initializers = new List<Expression>();
    List<ParameterExpression> variables = new List<ParameterExpression>();
    int[] numArray = new int[count];
    for (int index = 0; index < count; ++index)
    {
      Expression closureItem = this.GetClosureItem(node.Variables[index], false);
      if (closureItem == null)
      {
        numArray[index] = variables.Count;
        variables.Add(node.Variables[index]);
      }
      else
      {
        numArray[index] = -1 - initializers.Count;
        initializers.Add(closureItem);
      }
    }
    if (initializers.Count == 0)
      return (Expression) node;
    NewArrayExpression newArrayExpression = Expression.NewArrayInit(typeof (IStrongBox), (IEnumerable<Expression>) initializers);
    return variables.Count == 0 ? (Expression) Expression.Invoke((Expression) Expression.Constant((object) new Func<IStrongBox[], IRuntimeVariables>(RuntimeVariables.Create)), (Expression) newArrayExpression) : (Expression) Expression.Invoke(Utils.Constant((object) new Func<IRuntimeVariables, IRuntimeVariables, int[], IRuntimeVariables>(LightLambdaClosureVisitor.MergedRuntimeVariables.Create)), (Expression) Expression.RuntimeVariables((IEnumerable<ParameterExpression>) variables), (Expression) newArrayExpression, Utils.Constant((object) numArray));
  }

  protected override Expression VisitParameter(ParameterExpression node)
  {
    Expression closureItem = this.GetClosureItem(node, true);
    return closureItem == null ? (Expression) node : Utils.Convert(closureItem, node.Type);
  }

  protected override Expression VisitBinary(BinaryExpression node)
  {
    if (node.NodeType == ExpressionType.Assign && node.Left.NodeType == ExpressionType.Parameter)
    {
      ParameterExpression left = (ParameterExpression) node.Left;
      Expression closureItem = this.GetClosureItem(left, true);
      if (closureItem != null)
        return (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          left
        }, (Expression) Expression.Assign((Expression) left, this.Visit(node.Right)), (Expression) Expression.Assign(closureItem, Utils.Convert((Expression) left, typeof (object))), (Expression) left);
    }
    return base.VisitBinary(node);
  }

  private Expression GetClosureItem(ParameterExpression variable, bool unbox)
  {
    foreach (HashSet<ParameterExpression> shadowedVar in this._shadowedVars)
    {
      if (shadowedVar.Contains(variable))
        return (Expression) null;
    }
    LocalVariable localVariable;
    if (!this._closureVars.TryGetValue(variable, out localVariable))
      throw new InvalidOperationException("unbound variable: " + variable.Name);
    Expression strongBoxExpression = localVariable.LoadFromArray((Expression) null, (Expression) this._closureArray);
    return !unbox ? strongBoxExpression : LightCompiler.Unbox(strongBoxExpression);
  }

  protected override Expression VisitExtension(Expression node)
  {
    return this.Visit(node.ReduceExtensions());
  }

  private sealed class MergedRuntimeVariables : IRuntimeVariables
  {
    private readonly IRuntimeVariables _first;
    private readonly IRuntimeVariables _second;
    private readonly int[] _indexes;

    private MergedRuntimeVariables(
      IRuntimeVariables first,
      IRuntimeVariables second,
      int[] indexes)
    {
      this._first = first;
      this._second = second;
      this._indexes = indexes;
    }

    internal static IRuntimeVariables Create(
      IRuntimeVariables first,
      IRuntimeVariables second,
      int[] indexes)
    {
      return (IRuntimeVariables) new LightLambdaClosureVisitor.MergedRuntimeVariables(first, second, indexes);
    }

    int IRuntimeVariables.Count => this._indexes.Length;

    object IRuntimeVariables.this[int index]
    {
      get
      {
        index = this._indexes[index];
        return index < 0 ? this._second[-1 - index] : this._first[index];
      }
      set
      {
        index = this._indexes[index];
        if (index >= 0)
          this._first[index] = value;
        else
          this._second[-1 - index] = value;
      }
    }
  }
}
