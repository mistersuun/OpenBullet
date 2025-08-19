// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.TypeEnumMetaObject
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class TypeEnumMetaObject : DynamicMetaObject
{
  private readonly ComTypeEnumDesc _desc;

  internal TypeEnumMetaObject(ComTypeEnumDesc desc, Expression expression)
    : base(expression, BindingRestrictions.Empty, (object) desc)
  {
    this._desc = desc;
  }

  public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
  {
    if (this._desc.HasMember(binder.Name))
      return new DynamicMetaObject((Expression) Utils.Constant(((ComTypeEnumDesc) this.Value).GetValue(binder.Name), typeof (object)), this.EnumRestrictions());
    throw new NotImplementedException();
  }

  public override IEnumerable<string> GetDynamicMemberNames()
  {
    return (IEnumerable<string>) this._desc.GetMemberNames();
  }

  private BindingRestrictions EnumRestrictions()
  {
    return BindingRestrictionsHelpers.GetRuntimeTypeRestriction(this.Expression, typeof (ComTypeEnumDesc)).Merge(BindingRestrictions.GetExpressionRestriction((Expression) Expression.Equal((Expression) Expression.Property((Expression) Expression.Property(Utils.Convert(this.Expression, typeof (ComTypeEnumDesc)), typeof (ComTypeDesc).GetProperty("TypeLib")), typeof (ComTypeLibDesc).GetProperty("Guid")), Utils.Constant((object) this._desc.TypeLib.Guid)))).Merge(BindingRestrictions.GetExpressionRestriction((Expression) Expression.Equal((Expression) Expression.Property(Utils.Convert(this.Expression, typeof (ComTypeEnumDesc)), typeof (ComTypeEnumDesc).GetProperty("TypeName")), Utils.Constant((object) this._desc.TypeName))));
  }
}
