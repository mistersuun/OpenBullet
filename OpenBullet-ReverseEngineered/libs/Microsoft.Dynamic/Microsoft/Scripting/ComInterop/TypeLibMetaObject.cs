// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.TypeLibMetaObject
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class TypeLibMetaObject : DynamicMetaObject
{
  private readonly ComTypeLibDesc _lib;

  internal TypeLibMetaObject(Expression expression, ComTypeLibDesc lib)
    : base(expression, BindingRestrictions.Empty, (object) lib)
  {
    this._lib = lib;
  }

  private DynamicMetaObject TryBindGetMember(string name)
  {
    if (!this._lib.HasMember(name))
      return (DynamicMetaObject) null;
    BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(this.Expression, typeof (ComTypeLibDesc)).Merge(BindingRestrictions.GetExpressionRestriction((Expression) Expression.Equal((Expression) Expression.Property(Utils.Convert(this.Expression, typeof (ComTypeLibDesc)), typeof (ComTypeLibDesc).GetProperty("Guid")), Utils.Constant((object) this._lib.Guid))));
    return new DynamicMetaObject(Utils.Constant(((ComTypeLibDesc) this.Value).GetTypeLibObjectDesc(name)), restrictions);
  }

  public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
  {
    return this.TryBindGetMember(binder.Name) ?? base.BindGetMember(binder);
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder binder,
    DynamicMetaObject[] args)
  {
    DynamicMetaObject member = this.TryBindGetMember(binder.Name);
    return member != null ? binder.FallbackInvoke(member, args, (DynamicMetaObject) null) : base.BindInvokeMember(binder, args);
  }

  public override IEnumerable<string> GetDynamicMemberNames()
  {
    return (IEnumerable<string>) this._lib.GetMemberNames();
  }
}
