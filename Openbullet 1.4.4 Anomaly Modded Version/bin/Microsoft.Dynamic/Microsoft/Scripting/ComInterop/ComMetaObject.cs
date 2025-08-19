// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComMetaObject
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class ComMetaObject : DynamicMetaObject
{
  internal ComMetaObject(Expression expression, BindingRestrictions restrictions, object arg)
    : base(expression, restrictions, arg)
  {
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder binder,
    DynamicMetaObject[] args)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.Defer(((IList<DynamicMetaObject>) args).AddFirst<DynamicMetaObject>(this.WrapSelf()));
  }

  public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.Defer(((IList<DynamicMetaObject>) args).AddFirst<DynamicMetaObject>(this.WrapSelf()));
  }

  public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.Defer(this.WrapSelf());
  }

  public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.Defer(this.WrapSelf(), value);
  }

  public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.Defer(this.WrapSelf(), indexes);
  }

  public override DynamicMetaObject BindSetIndex(
    SetIndexBinder binder,
    DynamicMetaObject[] indexes,
    DynamicMetaObject value)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.Defer(this.WrapSelf(), ((IList<DynamicMetaObject>) indexes).AddLast<DynamicMetaObject>(value));
  }

  private DynamicMetaObject WrapSelf()
  {
    return new DynamicMetaObject((Expression) ComObject.RcwToComObject(this.Expression), BindingRestrictions.GetExpressionRestriction((Expression) Expression.Call(typeof (ComObject).GetMethod("IsComObject", BindingFlags.Static | BindingFlags.NonPublic), Helpers.Convert(this.Expression, typeof (object)))));
  }
}
