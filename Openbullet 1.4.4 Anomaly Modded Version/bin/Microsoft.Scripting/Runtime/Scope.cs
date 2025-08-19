// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.Scope
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class Scope : IDynamicMetaObjectProvider
{
  private ScopeExtension[] _extensions;
  private readonly object _extensionsLock = new object();
  private readonly IDynamicMetaObjectProvider _storage;

  public Scope()
  {
    this._extensions = ScopeExtension.EmptyArray;
    this._storage = (IDynamicMetaObjectProvider) new ScopeStorage();
  }

  public Scope(IDictionary<string, object> dictionary)
  {
    this._extensions = ScopeExtension.EmptyArray;
    this._storage = (IDynamicMetaObjectProvider) new StringDictionaryExpando(dictionary);
  }

  public Scope(IDynamicMetaObjectProvider storage)
  {
    this._extensions = ScopeExtension.EmptyArray;
    this._storage = storage;
  }

  public ScopeExtension GetExtension(ContextId languageContextId)
  {
    return languageContextId.Id >= this._extensions.Length ? (ScopeExtension) null : this._extensions[languageContextId.Id];
  }

  public ScopeExtension SetExtension(ContextId languageContextId, ScopeExtension extension)
  {
    ContractUtils.RequiresNotNull((object) extension, nameof (extension));
    lock (this._extensionsLock)
    {
      if (languageContextId.Id >= this._extensions.Length)
        Array.Resize<ScopeExtension>(ref this._extensions, languageContextId.Id + 1);
      return this._extensions[languageContextId.Id] ?? (this._extensions[languageContextId.Id] = extension);
    }
  }

  public object Storage => (object) this._storage;

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new Scope.MetaScope(parameter, this);
  }

  internal sealed class MetaScope(Expression parameter, Scope scope) : DynamicMetaObject(parameter, BindingRestrictions.Empty, (object) scope)
  {
    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    {
      return this.Restrict(this.StorageMetaObject.BindGetMember(binder));
    }

    public override DynamicMetaObject BindInvokeMember(
      InvokeMemberBinder binder,
      DynamicMetaObject[] args)
    {
      return this.Restrict(this.StorageMetaObject.BindInvokeMember(binder, args));
    }

    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    {
      return this.Restrict(this.StorageMetaObject.BindSetMember(binder, value));
    }

    public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
    {
      return this.Restrict(this.StorageMetaObject.BindDeleteMember(binder));
    }

    private DynamicMetaObject Restrict(DynamicMetaObject result)
    {
      return this.Expression.Type == typeof (Scope) ? result : new DynamicMetaObject(result.Expression, BindingRestrictions.GetTypeRestriction(this.Expression, typeof (Scope)).Merge(result.Restrictions));
    }

    private DynamicMetaObject StorageMetaObject
    {
      get
      {
        return DynamicMetaObject.Create((object) this.Value._storage, (Expression) this.StorageExpression);
      }
    }

    private MemberExpression StorageExpression
    {
      get
      {
        return Expression.Property((Expression) Expression.Convert(this.Expression, typeof (Scope)), typeof (Scope).GetProperty("Storage"));
      }
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
      return this.StorageMetaObject.GetDynamicMemberNames();
    }

    public Scope Value => (Scope) base.Value;
  }
}
