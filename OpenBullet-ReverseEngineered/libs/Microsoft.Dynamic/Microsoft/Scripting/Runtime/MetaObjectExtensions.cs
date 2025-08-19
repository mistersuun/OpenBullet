// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.MetaObjectExtensions
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public static class MetaObjectExtensions
{
  public static bool NeedsDeferral(this DynamicMetaObject self)
  {
    if (self.HasValue)
      return false;
    return !self.Expression.Type.IsSealed() || typeof (IDynamicMetaObjectProvider).IsAssignableFrom(self.Expression.Type);
  }

  public static DynamicMetaObject Restrict(this DynamicMetaObject self, Type type)
  {
    ContractUtils.RequiresNotNull((object) self, nameof (self));
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    if (self is IRestrictedMetaObject restrictedMetaObject)
      return restrictedMetaObject.Restrict(type);
    if (type == self.Expression.Type && (type.IsSealed() || self.Expression.NodeType == ExpressionType.New || self.Expression.NodeType == ExpressionType.NewArrayBounds || self.Expression.NodeType == ExpressionType.NewArrayInit))
      return self.Clone(self.Restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, type)));
    if (type == typeof (DynamicNull))
      return self.Clone(Microsoft.Scripting.Ast.Utils.Constant((object) null), self.Restrictions.Merge(BindingRestrictions.GetInstanceRestriction(self.Expression, (object) null)));
    Expression newExpression = !type.IsValueType() || !(self.Expression.Type != typeof (Enum)) ? Microsoft.Scripting.Ast.Utils.Convert(self.Expression, CompilerHelpers.GetVisibleType(type)) : (Expression) Expression.Unbox(self.Expression, CompilerHelpers.GetVisibleType(type));
    return self.Clone(newExpression, self.Restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, type)));
  }

  public static DynamicMetaObject Clone(this DynamicMetaObject self, Expression newExpression)
  {
    return self.Clone(newExpression, self.Restrictions);
  }

  public static DynamicMetaObject Clone(
    this DynamicMetaObject self,
    BindingRestrictions newRestrictions)
  {
    return self.Clone(self.Expression, newRestrictions);
  }

  public static DynamicMetaObject Clone(
    this DynamicMetaObject self,
    Expression newExpression,
    BindingRestrictions newRestrictions)
  {
    return !self.HasValue ? new DynamicMetaObject(newExpression, newRestrictions) : new DynamicMetaObject(newExpression, newRestrictions, self.Value);
  }

  public static Type GetLimitType(this DynamicMetaObject self)
  {
    return self.Value == null && self.HasValue ? typeof (DynamicNull) : self.LimitType;
  }

  public static Type GetRuntimeType(this DynamicMetaObject self)
  {
    return self.Value == null && self.HasValue ? typeof (DynamicNull) : self.RuntimeType;
  }
}
