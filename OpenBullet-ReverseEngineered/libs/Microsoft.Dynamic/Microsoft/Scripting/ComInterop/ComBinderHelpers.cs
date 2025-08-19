// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComBinderHelpers
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal static class ComBinderHelpers
{
  internal static bool PreferPut(Type type, bool holdsNull)
  {
    return type.IsValueType || type.IsArray || ((type == typeof (string) ? 1 : (type == typeof (DBNull) ? 1 : 0)) | (holdsNull ? 1 : 0)) != 0 || type == typeof (Missing) || type == typeof (CurrencyWrapper);
  }

  internal static bool IsByRef(DynamicMetaObject mo)
  {
    return mo.Expression is ParameterExpression expression && expression.IsByRef;
  }

  internal static bool IsStrongBoxArg(DynamicMetaObject o)
  {
    Type limitType = o.LimitType;
    return limitType.IsGenericType && limitType.GetGenericTypeDefinition() == typeof (StrongBox<>);
  }

  internal static bool[] ProcessArgumentsForCom(ref DynamicMetaObject[] args)
  {
    DynamicMetaObject[] dynamicMetaObjectArray = new DynamicMetaObject[args.Length];
    bool[] flagArray = new bool[args.Length];
    for (int index = 0; index < args.Length; ++index)
    {
      DynamicMetaObject dynamicMetaObject = args[index];
      if (ComBinderHelpers.IsByRef(dynamicMetaObject))
      {
        dynamicMetaObjectArray[index] = dynamicMetaObject;
        flagArray[index] = true;
      }
      else if (ComBinderHelpers.IsStrongBoxArg(dynamicMetaObject))
      {
        BindingRestrictions restrictions = dynamicMetaObject.Restrictions.Merge(ComBinderHelpers.GetTypeRestrictionForDynamicMetaObject(dynamicMetaObject));
        Expression expression = (Expression) Expression.Field(Helpers.Convert(dynamicMetaObject.Expression, dynamicMetaObject.LimitType), dynamicMetaObject.LimitType.GetField("Value"));
        object obj = dynamicMetaObject.Value is IStrongBox strongBox ? strongBox.Value : (object) null;
        dynamicMetaObjectArray[index] = new DynamicMetaObject(expression, restrictions, obj);
        flagArray[index] = true;
      }
      else
      {
        dynamicMetaObjectArray[index] = dynamicMetaObject;
        flagArray[index] = false;
      }
    }
    args = dynamicMetaObjectArray;
    return flagArray;
  }

  internal static BindingRestrictions GetTypeRestrictionForDynamicMetaObject(DynamicMetaObject obj)
  {
    return obj.Value == null && obj.HasValue ? BindingRestrictions.GetInstanceRestriction(obj.Expression, (object) null) : BindingRestrictions.GetTypeRestriction(obj.Expression, obj.LimitType);
  }
}
