// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ConditionalBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Actions;

internal class ConditionalBuilder
{
  private readonly List<Expression> _conditions = new List<Expression>();
  private readonly List<Expression> _bodies = new List<Expression>();
  private readonly List<ParameterExpression> _variables = new List<ParameterExpression>();
  private Expression _body;
  private bool _isError;
  private BindingRestrictions _restrictions = BindingRestrictions.Empty;

  public void AddCondition(Expression condition, Expression body)
  {
    this._conditions.Add(condition);
    this._bodies.Add(body);
  }

  public void FinishCondition(DynamicMetaObject body)
  {
    this._restrictions = this._restrictions.Merge(body.Restrictions);
    this.FinishCondition(body.Expression);
  }

  public void FinishCondition(Expression body)
  {
    if (this._body != null)
      throw new InvalidOperationException();
    for (int index = this._bodies.Count - 1; index >= 0; --index)
    {
      Type type = this._bodies[index].Type;
      if (type != body.Type)
      {
        if (type.IsSubclassOf(body.Type))
          type = body.Type;
        else if (!body.Type.IsSubclassOf(type))
          type = typeof (object);
      }
      body = (Expression) Expression.Condition(this._conditions[index], Microsoft.Scripting.Ast.Utils.Convert(this._bodies[index], type), Microsoft.Scripting.Ast.Utils.Convert(body, type));
    }
    this._body = (Expression) Expression.Block((IEnumerable<ParameterExpression>) this._variables, body);
  }

  public void FinishError(DynamicMetaObject body)
  {
    if (this._conditions.Count == 0)
      this._isError = true;
    this.FinishCondition(body);
  }

  public void FinishError(Expression body)
  {
    if (this._conditions.Count == 0)
      this._isError = true;
    this.FinishCondition(body);
  }

  public BindingRestrictions Restrictions
  {
    get => this._restrictions;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this._restrictions = value;
    }
  }

  public DynamicMetaObject GetMetaObject(params DynamicMetaObject[] types)
  {
    if (this._body == null)
      throw new InvalidOperationException("FinishCondition should have been called");
    return this._isError ? (DynamicMetaObject) new ErrorMetaObject(this._body, BindingRestrictions.Combine((IList<DynamicMetaObject>) types).Merge(this.Restrictions)) : new DynamicMetaObject(this._body, BindingRestrictions.Combine((IList<DynamicMetaObject>) types).Merge(this.Restrictions));
  }

  public void AddVariable(ParameterExpression var) => this._variables.Add(var);
}
