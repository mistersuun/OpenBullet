// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComClassMetaObject
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class ComClassMetaObject : DynamicMetaObject
{
  internal ComClassMetaObject(Expression expression, ComTypeClassDesc cls)
    : base(expression, BindingRestrictions.Empty, (object) cls)
  {
  }

  public override DynamicMetaObject BindCreateInstance(
    CreateInstanceBinder binder,
    DynamicMetaObject[] args)
  {
    return new DynamicMetaObject((Expression) Expression.Call(Utils.Convert(this.Expression, typeof (ComTypeClassDesc)), typeof (ComTypeClassDesc).GetMethod("CreateInstance")), BindingRestrictions.Combine((IList<DynamicMetaObject>) args).Merge(BindingRestrictions.GetTypeRestriction(this.Expression, typeof (ComTypeClassDesc))));
  }
}
