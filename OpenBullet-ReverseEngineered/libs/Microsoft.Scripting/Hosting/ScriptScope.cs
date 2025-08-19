// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ScriptScope
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.Remoting;

#nullable disable
namespace Microsoft.Scripting.Hosting;

[DebuggerTypeProxy(typeof (ScriptScope.DebugView))]
public sealed class ScriptScope : MarshalByRefObject, IDynamicMetaObjectProvider
{
  private readonly Scope _scope;
  private readonly ScriptEngine _engine;

  internal ScriptScope(ScriptEngine engine, Scope scope)
  {
    this._scope = scope;
    this._engine = engine;
  }

  internal Scope Scope => this._scope;

  public ScriptEngine Engine => this._engine;

  public object GetVariable(string name)
  {
    return this._engine.LanguageContext.ScopeGetVariable(this.Scope, name);
  }

  public T GetVariable<T>(string name)
  {
    return this._engine.LanguageContext.ScopeGetVariable<T>(this.Scope, name);
  }

  public bool TryGetVariable(string name, out object value)
  {
    return this._engine.LanguageContext.ScopeTryGetVariable(this.Scope, name, out value);
  }

  public bool TryGetVariable<T>(string name, out T value)
  {
    object obj;
    if (this._engine.LanguageContext.ScopeTryGetVariable(this.Scope, name, out obj))
    {
      value = this._engine.Operations.ConvertTo<T>(obj);
      return true;
    }
    value = default (T);
    return false;
  }

  public void SetVariable(string name, object value)
  {
    this._engine.LanguageContext.ScopeSetVariable(this.Scope, name, value);
  }

  public ObjectHandle GetVariableHandle(string name) => new ObjectHandle(this.GetVariable(name));

  public bool TryGetVariableHandle(string name, out ObjectHandle handle)
  {
    object o;
    if (this.TryGetVariable(name, out o))
    {
      handle = new ObjectHandle(o);
      return true;
    }
    handle = (ObjectHandle) null;
    return false;
  }

  public void SetVariable(string name, ObjectHandle handle)
  {
    ContractUtils.RequiresNotNull((object) handle, nameof (handle));
    this.SetVariable(name, handle.Unwrap());
  }

  public bool ContainsVariable(string name) => this.TryGetVariable(name, out object _);

  public bool RemoveVariable(string name)
  {
    if (!this._engine.Operations.ContainsMember((object) this._scope, name))
      return false;
    this._engine.Operations.RemoveMember((object) this._scope, name);
    return true;
  }

  public IEnumerable<string> GetVariableNames()
  {
    return (IEnumerable<string>) this._engine.Operations.GetMemberNames(this._scope.Storage);
  }

  public IEnumerable<KeyValuePair<string, object>> GetItems()
  {
    List<KeyValuePair<string, object>> items = new List<KeyValuePair<string, object>>();
    foreach (string variableName in this.GetVariableNames())
      items.Add(new KeyValuePair<string, object>(variableName, this._engine.Operations.GetMember(this._scope.Storage, variableName)));
    items.TrimExcess();
    return (IEnumerable<KeyValuePair<string, object>>) items;
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new ScriptScope.Meta(parameter, this);
  }

  public override object InitializeLifetimeService() => (object) null;

  internal sealed class DebugView
  {
    private readonly ScriptScope _scope;

    public DebugView(ScriptScope scope) => this._scope = scope;

    public ScriptEngine Language => this._scope._engine;

    public Hashtable Variables
    {
      get
      {
        Hashtable variables = new Hashtable();
        foreach (KeyValuePair<string, object> keyValuePair in this._scope.GetItems())
          variables[(object) keyValuePair.Key] = keyValuePair.Value;
        return variables;
      }
    }
  }

  private sealed class Meta : DynamicMetaObject
  {
    internal Meta(Expression parameter, ScriptScope scope)
      : base(parameter, BindingRestrictions.Empty, (object) scope)
    {
    }

    public override DynamicMetaObject BindGetMember(GetMemberBinder action)
    {
      ParameterExpression ifTrue = Expression.Variable(typeof (object), "result");
      DynamicMetaObject member = action.FallbackGetMember((DynamicMetaObject) this);
      return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        ifTrue
      }, (Expression) Expression.Condition((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, typeof (ScriptScope)), typeof (ScriptScope).GetMethod("TryGetVariable", new Type[2]
      {
        typeof (string),
        typeof (object).MakeByRefType()
      }), (Expression) Expression.Constant((object) action.Name), (Expression) ifTrue), (Expression) ifTrue, (Expression) Expression.Convert(member.Expression, typeof (object)))), BindingRestrictions.GetTypeRestriction(this.Expression, typeof (ScriptScope)).Merge(member.Restrictions));
    }

    public override DynamicMetaObject BindSetMember(SetMemberBinder action, DynamicMetaObject value)
    {
      Expression expression = (Expression) Expression.Convert(value.Expression, typeof (object));
      return new DynamicMetaObject((Expression) Expression.Block((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, typeof (ScriptScope)), typeof (ScriptScope).GetMethod("SetVariable", new Type[2]
      {
        typeof (string),
        typeof (object)
      }), (Expression) Expression.Constant((object) action.Name), expression), expression), this.Restrictions.Merge(value.Restrictions).Merge(BindingRestrictions.GetTypeRestriction(this.Expression, typeof (ScriptScope))));
    }

    public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder action)
    {
      DynamicMetaObject dynamicMetaObject = action.FallbackDeleteMember((DynamicMetaObject) this);
      return new DynamicMetaObject((Expression) Expression.IfThenElse((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, typeof (ScriptScope)), typeof (ScriptScope).GetMethod("RemoveVariable"), (Expression) Expression.Constant((object) action.Name)), (Expression) Expression.Empty(), dynamicMetaObject.Expression), this.Restrictions.Merge(BindingRestrictions.GetTypeRestriction(this.Expression, typeof (ScriptScope))).Merge(dynamicMetaObject.Restrictions));
    }

    public override DynamicMetaObject BindInvokeMember(
      InvokeMemberBinder action,
      DynamicMetaObject[] args)
    {
      DynamicMetaObject dynamicMetaObject1 = action.FallbackInvokeMember((DynamicMetaObject) this, args);
      ParameterExpression parameterExpression = Expression.Variable(typeof (object), "result");
      DynamicMetaObject dynamicMetaObject2 = action.FallbackInvoke(new DynamicMetaObject((Expression) parameterExpression, BindingRestrictions.Empty), args, (DynamicMetaObject) null);
      return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        parameterExpression
      }, (Expression) Expression.Condition((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, typeof (ScriptScope)), typeof (ScriptScope).GetMethod("TryGetVariable", new Type[2]
      {
        typeof (string),
        typeof (object).MakeByRefType()
      }), (Expression) Expression.Constant((object) action.Name), (Expression) parameterExpression), (Expression) Expression.Convert(dynamicMetaObject2.Expression, typeof (object)), (Expression) Expression.Convert(dynamicMetaObject1.Expression, typeof (object)))), BindingRestrictions.Combine((IList<DynamicMetaObject>) args).Merge(BindingRestrictions.GetTypeRestriction(this.Expression, typeof (ScriptScope))).Merge(dynamicMetaObject1.Restrictions));
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
      return ((ScriptScope) this.Value).GetVariableNames();
    }
  }
}
