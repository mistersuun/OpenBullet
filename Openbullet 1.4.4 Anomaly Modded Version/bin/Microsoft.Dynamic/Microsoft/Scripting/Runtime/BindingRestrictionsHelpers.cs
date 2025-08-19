// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.BindingRestrictionsHelpers
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public static class BindingRestrictionsHelpers
{
  public static BindingRestrictions GetRuntimeTypeRestriction(Expression expr, Type type)
  {
    return type == typeof (DynamicNull) ? BindingRestrictions.GetInstanceRestriction(expr, (object) null) : BindingRestrictions.GetTypeRestriction(expr, type);
  }

  public static BindingRestrictions GetRuntimeTypeRestriction(DynamicMetaObject obj)
  {
    return obj.Restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(obj.Expression, obj.GetLimitType()));
  }
}
