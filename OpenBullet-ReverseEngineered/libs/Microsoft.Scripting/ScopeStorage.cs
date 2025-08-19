// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ScopeStorage
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting;

public sealed class ScopeStorage : IDynamicMetaObjectProvider
{
  private readonly Dictionary<string, ScopeVariableIgnoreCase> _storage = new Dictionary<string, ScopeVariableIgnoreCase>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

  public object GetValue(string name, bool ignoreCase)
  {
    object obj;
    if (this.GetScopeVariable(name, ignoreCase).TryGetValue(out obj))
      return obj;
    throw new KeyNotFoundException("no value");
  }

  public bool TryGetValue(string name, bool ignoreCase, out object value)
  {
    object obj;
    if (this.HasVariable(name) && this.GetScopeVariable(name, ignoreCase).TryGetValue(out obj))
    {
      value = obj;
      return true;
    }
    value = (object) null;
    return false;
  }

  public void SetValue(string name, bool ignoreCase, object value)
  {
    this.GetScopeVariable(name, ignoreCase).SetValue(value);
  }

  public bool DeleteValue(string name, bool ignoreCase)
  {
    return this.HasVariable(name) && this.GetScopeVariable(name, ignoreCase).DeleteValue();
  }

  public bool HasValue(string name, bool ignoreCase)
  {
    return this.HasVariable(name) && this.GetScopeVariable(name, ignoreCase).HasValue;
  }

  public IScopeVariable GetScopeVariable(string name, bool ignoreCase)
  {
    return ignoreCase ? (IScopeVariable) this.GetScopeVariableIgnoreCase(name) : (IScopeVariable) this.GetScopeVariable(name);
  }

  public ScopeVariable GetScopeVariable(string name)
  {
    return this.GetScopeVariableIgnoreCase(name).GetCaseSensitiveStorage(name);
  }

  public ScopeVariableIgnoreCase GetScopeVariableIgnoreCase(string name)
  {
    lock (this._storage)
    {
      ScopeVariableIgnoreCase variableIgnoreCase;
      if (!this._storage.TryGetValue(name, out variableIgnoreCase))
        this._storage[name] = variableIgnoreCase = new ScopeVariableIgnoreCase(name);
      return variableIgnoreCase;
    }
  }

  public object this[string index]
  {
    get => this.GetValue(index, false);
    set => this.SetValue(index, false, value);
  }

  public IList<string> GetMemberNames()
  {
    List<string> list = new List<string>();
    lock (this._storage)
    {
      foreach (ScopeVariableIgnoreCase variableIgnoreCase in this._storage.Values)
        variableIgnoreCase.AddNames(list);
    }
    return (IList<string>) list;
  }

  public IList<KeyValuePair<string, object>> GetItems()
  {
    List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
    lock (this._storage)
    {
      foreach (ScopeVariableIgnoreCase variableIgnoreCase in this._storage.Values)
        variableIgnoreCase.AddItems(list);
    }
    return (IList<KeyValuePair<string, object>>) list;
  }

  private bool HasVariable(string name)
  {
    lock (this._storage)
      return this._storage.ContainsKey(name);
  }

  public DynamicMetaObject GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new ScopeStorage.Meta(parameter, this);
  }

  private class Meta(Expression parameter, ScopeStorage storage) : DynamicMetaObject(parameter, BindingRestrictions.Empty, (object) storage)
  {
    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    {
      return this.DynamicTryGetValue(binder.Name, binder.IgnoreCase, binder.FallbackGetMember((DynamicMetaObject) this).Expression, (Func<Expression, Expression>) (tmp => tmp));
    }

    public override DynamicMetaObject BindInvokeMember(
      InvokeMemberBinder binder,
      DynamicMetaObject[] args)
    {
      return this.DynamicTryGetValue(binder.Name, binder.IgnoreCase, binder.FallbackInvokeMember((DynamicMetaObject) this, args).Expression, (Func<Expression, Expression>) (tmp => binder.FallbackInvoke(new DynamicMetaObject(tmp, BindingRestrictions.Empty), args, (DynamicMetaObject) null).Expression));
    }

    private DynamicMetaObject DynamicTryGetValue(
      string name,
      bool ignoreCase,
      Expression fallback,
      Func<Expression, Expression> resultOp)
    {
      IScopeVariable scopeVariable = this.Value.GetScopeVariable(name, ignoreCase);
      ParameterExpression parameterExpression = Expression.Parameter(typeof (object));
      return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        parameterExpression
      }, (Expression) Expression.Condition((Expression) Expression.Call(ScopeStorage.Meta.Variable(scopeVariable), scopeVariable.GetType().GetMethod("TryGetValue"), (Expression) parameterExpression), ExpressionUtils.Convert(resultOp((Expression) parameterExpression), typeof (object)), ExpressionUtils.Convert(fallback, typeof (object)))), BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value));
    }

    private static Expression Variable(IScopeVariable variable)
    {
      return (Expression) Expression.Convert((Expression) Expression.Property((Expression) Expression.Constant((object) ((IWeakReferencable) variable).WeakReference), typeof (WeakReference).GetProperty("Target")), variable.GetType());
    }

    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    {
      IScopeVariable scopeVariable = this.Value.GetScopeVariable(binder.Name, binder.IgnoreCase);
      Expression expression = ExpressionUtils.Convert(value.Expression, typeof (object));
      return new DynamicMetaObject((Expression) Expression.Block((Expression) Expression.Call(ScopeStorage.Meta.Variable(scopeVariable), scopeVariable.GetType().GetMethod("SetValue"), expression), expression), BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value));
    }

    public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
    {
      IScopeVariable scopeVariable = this.Value.GetScopeVariable(binder.Name, binder.IgnoreCase);
      return new DynamicMetaObject((Expression) Expression.Condition((Expression) Expression.Call(ScopeStorage.Meta.Variable(scopeVariable), scopeVariable.GetType().GetMethod("DeleteValue")), (Expression) Expression.Default(binder.ReturnType), binder.FallbackDeleteMember((DynamicMetaObject) this).Expression), BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value));
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
      return (IEnumerable<string>) this.Value.GetMemberNames();
    }

    public ScopeStorage Value => (ScopeStorage) base.Value;
  }
}
