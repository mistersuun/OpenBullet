// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.VariantArray
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal static class VariantArray
{
  private static readonly List<Type> _generatedTypes = new List<Type>(0);

  internal static MemberExpression GetStructField(ParameterExpression variantArray, int field)
  {
    return Expression.Field((Expression) variantArray, "Element" + (object) field);
  }

  internal static Type GetStructType(int args)
  {
    if (args <= 1)
      return typeof (VariantArray1);
    if (args <= 2)
      return typeof (VariantArray2);
    if (args <= 4)
      return typeof (VariantArray4);
    if (args <= 8)
      return typeof (VariantArray8);
    int size = 1;
    while (args > size)
      size *= 2;
    lock (VariantArray._generatedTypes)
    {
      foreach (Type generatedType in VariantArray._generatedTypes)
      {
        int num = int.Parse(generatedType.Name.Substring(nameof (VariantArray).Length), (IFormatProvider) CultureInfo.InvariantCulture);
        if (size == num)
          return generatedType;
      }
      Type structType = VariantArray.CreateCustomType(size).MakeGenericType(typeof (Variant));
      VariantArray._generatedTypes.Add(structType);
      return structType;
    }
  }

  private static Type CreateCustomType(int size)
  {
    TypeAttributes attr = TypeAttributes.SequentialLayout;
    TypeBuilder typeBuilder = UnsafeMethods.DynamicModule.DefineType(nameof (VariantArray) + (object) size, attr, typeof (ValueType));
    GenericTypeParameterBuilder genericParameter = typeBuilder.DefineGenericParameters("T")[0];
    for (int index = 0; index < size; ++index)
      typeBuilder.DefineField("Element" + (object) index, (Type) genericParameter, FieldAttributes.Public);
    return typeBuilder.CreateType();
  }
}
