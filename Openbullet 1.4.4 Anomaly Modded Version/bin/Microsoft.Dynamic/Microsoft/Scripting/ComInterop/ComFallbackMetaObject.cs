// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComFallbackMetaObject
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class ComFallbackMetaObject : DynamicMetaObject
{
  internal ComFallbackMetaObject(
    Expression expression,
    BindingRestrictions restrictions,
    object arg)
    : base(expression, restrictions, arg)
  {
  }

  public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.FallbackGetIndex((DynamicMetaObject) this.UnwrapSelf(), indexes);
  }

  public override DynamicMetaObject BindSetIndex(
    SetIndexBinder binder,
    DynamicMetaObject[] indexes,
    DynamicMetaObject value)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.FallbackSetIndex((DynamicMetaObject) this.UnwrapSelf(), indexes, value);
  }

  public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.FallbackGetMember((DynamicMetaObject) this.UnwrapSelf());
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder binder,
    DynamicMetaObject[] args)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.FallbackInvokeMember((DynamicMetaObject) this.UnwrapSelf(), args);
  }

  public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.FallbackSetMember((DynamicMetaObject) this.UnwrapSelf(), value);
  }

  protected virtual ComUnwrappedMetaObject UnwrapSelf()
  {
    return new ComUnwrappedMetaObject((Expression) ComObject.RcwFromComObject(this.Expression), this.Restrictions.Merge(ComBinderHelpers.GetTypeRestrictionForDynamicMetaObject((DynamicMetaObject) this)), ((ComObject) this.Value).RuntimeCallableWrapper);
  }
}
