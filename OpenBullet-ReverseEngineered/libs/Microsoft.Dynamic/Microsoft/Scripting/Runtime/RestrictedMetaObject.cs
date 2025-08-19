// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.RestrictedMetaObject
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using System;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public class RestrictedMetaObject : DynamicMetaObject, IRestrictedMetaObject
{
  public RestrictedMetaObject(Expression expression, BindingRestrictions restriction, object value)
    : base(expression, restriction, value)
  {
  }

  public RestrictedMetaObject(Expression expression, BindingRestrictions restriction)
    : base(expression, restriction)
  {
  }

  public DynamicMetaObject Restrict(Type type)
  {
    if (type == this.LimitType)
      return (DynamicMetaObject) this;
    return this.HasValue ? (DynamicMetaObject) new RestrictedMetaObject(Utils.Convert(this.Expression, type), BindingRestrictionsHelpers.GetRuntimeTypeRestriction(this.Expression, type), this.Value) : (DynamicMetaObject) new RestrictedMetaObject(Utils.Convert(this.Expression, type), BindingRestrictionsHelpers.GetRuntimeTypeRestriction(this.Expression, type));
  }
}
