// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.ConditionalBuilder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class ConditionalBuilder
{
  private readonly DynamicMetaObjectBinder _action;
  private readonly List<Expression> _conditions = new List<Expression>();
  private readonly List<Expression> _bodies = new List<Expression>();
  private readonly List<ParameterExpression> _variables = new List<ParameterExpression>();
  private Expression _body;
  private BindingRestrictions _restrictions = BindingRestrictions.Empty;
  private ParameterExpression _compareRetBool;
  private Type _retType;

  public ConditionalBuilder(DynamicMetaObjectBinder action) => this._action = action;

  public ConditionalBuilder()
  {
  }

  public void AddCondition(Expression condition, Expression body)
  {
    this._conditions.Add(condition);
    this._bodies.Add(body);
  }

  public void ExtendLastCondition(Expression condition)
  {
    if (this._body != null)
    {
      this.AddCondition(condition, this._body);
      this._body = (Expression) null;
    }
    else
      this._conditions[this._conditions.Count - 1] = (Expression) Expression.AndAlso(this._conditions[this._conditions.Count - 1], condition);
  }

  public void FinishCondition(Expression body) => this.FinishCondition(body, typeof (object));

  public void FinishCondition(Expression body, Type retType)
  {
    this._body = this._body == null ? body : throw new InvalidOperationException();
    this._retType = retType;
  }

  public ParameterExpression CompareRetBool
  {
    get
    {
      if (this._compareRetBool == null)
      {
        this._compareRetBool = Expression.Variable(typeof (bool), "compareRetBool");
        this.AddVariable(this._compareRetBool);
      }
      return this._compareRetBool;
    }
  }

  public BindingRestrictions Restrictions
  {
    get => this._restrictions;
    set => this._restrictions = value;
  }

  public DynamicMetaObjectBinder Action => this._action;

  public bool NoConditions => this._conditions.Count == 0;

  public bool IsFinal => this._body != null;

  public DynamicMetaObject GetMetaObject(params DynamicMetaObject[] types)
  {
    Expression expression = this._body != null ? this._body : throw new InvalidOperationException("FinishCondition not called before GetMetaObject");
    for (int index = this._bodies.Count - 1; index >= 0; --index)
      expression = (Expression) Expression.Condition(this._conditions[index], Utils.Convert(this._bodies[index], this._retType), Utils.Convert(expression, this._retType));
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) this._variables, expression), BindingRestrictions.Combine((IList<DynamicMetaObject>) types));
  }

  public void AddVariable(ParameterExpression var)
  {
    if (this._body != null)
      throw new InvalidOperationException("Variables must be added before calling FinishCondition");
    this._variables.Add(var);
  }
}
