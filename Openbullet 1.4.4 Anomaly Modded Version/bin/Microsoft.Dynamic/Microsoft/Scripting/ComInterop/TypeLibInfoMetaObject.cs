// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.TypeLibInfoMetaObject
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal sealed class TypeLibInfoMetaObject : DynamicMetaObject
{
  private readonly ComTypeLibInfo _info;

  internal TypeLibInfoMetaObject(Expression expression, ComTypeLibInfo info)
    : base(expression, BindingRestrictions.Empty, (object) info)
  {
    this._info = info;
  }

  public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    string name = binder.Name;
    if (name == this._info.Name)
      name = "TypeLibDesc";
    else if (name != "Guid" && name != "Name" && name != "VersionMajor" && name != "VersionMinor")
      return binder.FallbackGetMember((DynamicMetaObject) this);
    return new DynamicMetaObject((Expression) Expression.Convert((Expression) Expression.Property(Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (ComTypeLibInfo)), typeof (ComTypeLibInfo).GetProperty(name)), typeof (object)), this.ComTypeLibInfoRestrictions((DynamicMetaObject) this));
  }

  public override IEnumerable<string> GetDynamicMemberNames()
  {
    return (IEnumerable<string>) this._info.GetMemberNames();
  }

  private BindingRestrictions ComTypeLibInfoRestrictions(params DynamicMetaObject[] args)
  {
    return BindingRestrictions.Combine((IList<DynamicMetaObject>) args).Merge(BindingRestrictions.GetTypeRestriction(this.Expression, typeof (ComTypeLibInfo)));
  }
}
