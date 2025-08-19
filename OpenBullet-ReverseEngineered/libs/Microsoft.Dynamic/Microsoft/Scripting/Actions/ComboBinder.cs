// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ComboBinder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class ComboBinder : DynamicMetaObjectBinder
{
  private readonly BinderMappingInfo[] _metaBinders;

  public ComboBinder(params BinderMappingInfo[] binders)
    : this((ICollection<BinderMappingInfo>) binders)
  {
  }

  public ComboBinder(ICollection<BinderMappingInfo> binders)
  {
    this._metaBinders = ArrayUtils.ToArray<BinderMappingInfo>(binders);
  }

  public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    args = ArrayUtils.Insert<DynamicMetaObject>(target, args);
    List<DynamicMetaObject> results = new List<DynamicMetaObject>(this._metaBinders.Length);
    List<Expression> expressionList = new List<Expression>();
    List<ParameterExpression> parameterExpressionList = new List<ParameterExpression>();
    BindingRestrictions restrictions = BindingRestrictions.Empty;
    for (int metaBinderIndex = 0; metaBinderIndex < this._metaBinders.Length; ++metaBinderIndex)
    {
      BinderMappingInfo metaBinder = this._metaBinders[metaBinderIndex];
      DynamicMetaObject[] arguments = this.GetArguments(args, (IList<DynamicMetaObject>) results, metaBinderIndex);
      DynamicMetaObject dynamicMetaObject = metaBinder.Binder.Bind(arguments[0], ArrayUtils.RemoveFirst<DynamicMetaObject>(arguments));
      if (metaBinderIndex != 0)
        dynamicMetaObject = new DynamicMetaObject(new ComboBinder.ReplaceUpdateVisitor()
        {
          Binder = metaBinder.Binder,
          Arguments = arguments
        }.Visit(dynamicMetaObject.Expression), dynamicMetaObject.Restrictions);
      restrictions = restrictions.Merge(dynamicMetaObject.Restrictions);
      if (dynamicMetaObject.Expression.NodeType == ExpressionType.Throw)
      {
        expressionList.Add(dynamicMetaObject.Expression);
        break;
      }
      ParameterExpression left = Expression.Variable(dynamicMetaObject.Expression.Type, "comboTemp" + metaBinderIndex.ToString());
      parameterExpressionList.Add(left);
      expressionList.Add((Expression) Expression.Assign((Expression) left, dynamicMetaObject.Expression));
      results.Add(new DynamicMetaObject((Expression) left, dynamicMetaObject.Restrictions));
    }
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) parameterExpressionList.ToArray(), expressionList.ToArray()), restrictions);
  }

  public override Type ReturnType
  {
    get => this._metaBinders[this._metaBinders.Length - 1].Binder.ReturnType;
  }

  private DynamicMetaObject[] GetArguments(
    DynamicMetaObject[] args,
    IList<DynamicMetaObject> results,
    int metaBinderIndex)
  {
    BinderMappingInfo metaBinder = this._metaBinders[metaBinderIndex];
    DynamicMetaObject[] arguments = new DynamicMetaObject[metaBinder.MappingInfo.Count];
    for (int index = 0; index < arguments.Length; ++index)
    {
      ParameterMappingInfo parameterMappingInfo = metaBinder.MappingInfo[index];
      arguments[index] = !parameterMappingInfo.IsAction ? (!parameterMappingInfo.IsParameter ? new DynamicMetaObject((Expression) parameterMappingInfo.Constant, BindingRestrictions.Empty, parameterMappingInfo.Constant.Value) : args[parameterMappingInfo.ParameterIndex]) : results[parameterMappingInfo.ActionIndex];
    }
    return arguments;
  }

  public override int GetHashCode()
  {
    int hashCode = 6551;
    foreach (BinderMappingInfo metaBinder in this._metaBinders)
    {
      hashCode ^= metaBinder.Binder.GetHashCode();
      foreach (ParameterMappingInfo parameterMappingInfo in (IEnumerable<ParameterMappingInfo>) metaBinder.MappingInfo)
        hashCode ^= parameterMappingInfo.GetHashCode();
    }
    return hashCode;
  }

  public override bool Equals(object obj)
  {
    if (!(obj is ComboBinder comboBinder) || this._metaBinders.Length != comboBinder._metaBinders.Length)
      return false;
    for (int index1 = 0; index1 < this._metaBinders.Length; ++index1)
    {
      BinderMappingInfo metaBinder1 = this._metaBinders[index1];
      BinderMappingInfo metaBinder2 = comboBinder._metaBinders[index1];
      if (!metaBinder1.Binder.Equals((object) metaBinder2.Binder) || metaBinder1.MappingInfo.Count != metaBinder2.MappingInfo.Count)
        return false;
      for (int index2 = 0; index2 < metaBinder1.MappingInfo.Count; ++index2)
      {
        if (!metaBinder1.MappingInfo[index2].Equals((object) metaBinder2.MappingInfo[index2]))
          return false;
      }
    }
    return true;
  }

  private sealed class ReplaceUpdateVisitor : ExpressionVisitor
  {
    internal DynamicMetaObjectBinder Binder;
    internal DynamicMetaObject[] Arguments;

    protected override Expression VisitGoto(GotoExpression node)
    {
      return node.Target == CallSiteBinder.UpdateLabel ? this.Binder.Defer(this.Arguments).Expression : this.Visit((Expression) node);
    }
  }
}
