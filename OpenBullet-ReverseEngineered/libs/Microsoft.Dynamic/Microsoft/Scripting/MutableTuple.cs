// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.MutableTuple
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting;

public abstract class MutableTuple
{
  public const int MaxSize = 128 /*0x80*/;
  private static readonly Dictionary<Type, int> _sizeDict = new Dictionary<Type, int>();

  public abstract object GetValue(int index);

  public abstract void SetValue(int index, object value);

  public void SetNestedValue(int size, int index, object value)
  {
    if (size < 128 /*0x80*/)
    {
      this.SetValue(index, value);
    }
    else
    {
      MutableTuple mutableTuple = this;
      int index1 = -1;
      foreach (int num in MutableTuple.GetAccessPath(size, index))
      {
        if (index1 != -1)
          mutableTuple = (MutableTuple) mutableTuple.GetValue(index1);
        index1 = num;
      }
      mutableTuple.SetValue(index1, value);
    }
  }

  public object GetNestedValue(int size, int index)
  {
    if (size < 128 /*0x80*/)
      return this.GetValue(index);
    object nestedValue = (object) this;
    foreach (int index1 in MutableTuple.GetAccessPath(size, index))
      nestedValue = ((MutableTuple) nestedValue).GetValue(index1);
    return nestedValue;
  }

  public static Type GetTupleType(int size)
  {
    if (size > 128 /*0x80*/)
      return (Type) null;
    if (size <= 1)
      return typeof (MutableTuple<>);
    if (size <= 2)
      return typeof (MutableTuple<,>);
    if (size <= 4)
      return typeof (MutableTuple<,,,>);
    if (size <= 8)
      return typeof (MutableTuple<,,,,,,,>);
    if (size <= 16 /*0x10*/)
      return typeof (MutableTuple<,,,,,,,,,,,,,,,>);
    if (size <= 32 /*0x20*/)
      return typeof (MutableTuple<,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,>);
    return size <= 64 /*0x40*/ ? typeof (MutableTuple<,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,>) : typeof (MutableTuple<,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,>);
  }

  public static Type MakeTupleType(params Type[] types)
  {
    ContractUtils.RequiresNotNull((object) types, nameof (types));
    return MutableTuple.MakeTupleType(types, 0, types.Length);
  }

  public static int GetSize(Type tupleType)
  {
    ContractUtils.RequiresNotNull((object) tupleType, nameof (tupleType));
    int size = 0;
    lock (MutableTuple._sizeDict)
    {
      if (MutableTuple._sizeDict.TryGetValue(tupleType, out size))
        return size;
    }
    Stack<Type> typeStack = new Stack<Type>((IEnumerable<Type>) tupleType.GetGenericArguments());
    while (typeStack.Count != 0)
    {
      Type c = typeStack.Pop();
      if (typeof (MutableTuple).IsAssignableFrom(c))
      {
        foreach (Type genericArgument in c.GetGenericArguments())
          typeStack.Push(genericArgument);
      }
      else if (!(c == typeof (DynamicNull)))
        ++size;
    }
    lock (MutableTuple._sizeDict)
      MutableTuple._sizeDict[tupleType] = size;
    return size;
  }

  public static MutableTuple MakeTuple(Type tupleType, params object[] args)
  {
    ContractUtils.RequiresNotNull((object) tupleType, nameof (tupleType));
    ContractUtils.RequiresNotNull((object) args, nameof (args));
    return MutableTuple.MakeTuple(tupleType, 0, args.Length, args);
  }

  public static object[] GetTupleValues(MutableTuple tuple)
  {
    ContractUtils.RequiresNotNull((object) tuple, nameof (tuple));
    List<object> args = new List<object>();
    MutableTuple.GetTupleValues(tuple, args);
    return args.ToArray();
  }

  public static IEnumerable<PropertyInfo> GetAccessPath(Type tupleType, int index)
  {
    return MutableTuple.GetAccessProperties(tupleType, MutableTuple.GetSize(tupleType), index);
  }

  internal static IEnumerable<PropertyInfo> GetAccessProperties(
    Type tupleType,
    int size,
    int index)
  {
    ContractUtils.RequiresNotNull((object) tupleType, nameof (tupleType));
    if (index < 0 || index >= size)
      throw new ArgumentException(nameof (index));
    foreach (int num in MutableTuple.GetAccessPath(size, index))
    {
      PropertyInfo pi = tupleType.GetInheritedProperties("Item" + $"{num:D3}").First<PropertyInfo>();
      yield return pi;
      tupleType = pi.PropertyType;
      pi = (PropertyInfo) null;
    }
  }

  internal static IEnumerable<int> GetAccessPath(int size, int index)
  {
    int depth = 0;
    int mask = (int) sbyte.MaxValue;
    int adjust = 1;
    int num = size;
    while (num > 128 /*0x80*/)
    {
      ++depth;
      num /= 128 /*0x80*/;
      mask *= 128 /*0x80*/;
      adjust *= 128 /*0x80*/;
    }
    while (depth-- >= 0)
    {
      yield return (index & mask) / adjust;
      mask /= 128 /*0x80*/;
      adjust /= 128 /*0x80*/;
    }
  }

  private static void GetTupleValues(MutableTuple tuple, List<object> args)
  {
    Type[] genericArguments = tuple.GetType().GetGenericArguments();
    for (int index = 0; index < genericArguments.Length; ++index)
    {
      if (typeof (MutableTuple).IsAssignableFrom(genericArguments[index]))
        MutableTuple.GetTupleValues((MutableTuple) tuple.GetValue(index), args);
      else if (genericArguments[index] != typeof (DynamicNull))
        args.Add(tuple.GetValue(index));
    }
  }

  private static MutableTuple MakeTuple(Type tupleType, int start, int end, object[] args)
  {
    int num1 = end - start;
    MutableTuple instance = (MutableTuple) Activator.CreateInstance(tupleType);
    if (num1 > 128 /*0x80*/)
    {
      int num2 = 1;
      while (num1 > 128 /*0x80*/)
      {
        num1 = (num1 + 128 /*0x80*/ - 1) / 128 /*0x80*/;
        num2 *= 128 /*0x80*/;
      }
      for (int index = 0; index < num1; ++index)
      {
        int start1 = start + index * num2;
        int end1 = Math.Min(end, start + (index + 1) * num2);
        PropertyInfo propertyInfo = tupleType.GetInheritedProperties("Item" + $"{index:D3}").First<PropertyInfo>();
        instance.SetValue(index, (object) MutableTuple.CreateTupleInstance(propertyInfo.PropertyType, start1, end1, args));
      }
    }
    else
    {
      for (int index = start; index < end; ++index)
        instance.SetValue(index - start, args[index]);
    }
    return instance;
  }

  private static MutableTuple CreateTupleInstance(
    Type tupleType,
    int start,
    int end,
    object[] args)
  {
    if (args == null)
      return (MutableTuple) Activator.CreateInstance(tupleType);
    object[] destinationArray = new object[tupleType.GetGenericArguments().Length];
    Array.Copy((Array) args, start, (Array) destinationArray, 0, end - start);
    return (MutableTuple) Activator.CreateInstance(tupleType, destinationArray);
  }

  private static Type MakeTupleType(Type[] types, int start, int end)
  {
    int size = end - start;
    Type tupleType1 = MutableTuple.GetTupleType(size);
    if (tupleType1 != (Type) null)
    {
      Type[] typeArray = new Type[tupleType1.GetGenericArguments().Length];
      int num = 0;
      for (int index = start; index < end; ++index)
        typeArray[num++] = types[index];
      while (num < typeArray.Length)
        typeArray[num++] = typeof (DynamicNull);
      return tupleType1.MakeGenericType(typeArray);
    }
    int num1 = 1;
    while (size > 128 /*0x80*/)
    {
      size = (size + 128 /*0x80*/ - 1) / 128 /*0x80*/;
      num1 *= 128 /*0x80*/;
    }
    Type tupleType2 = MutableTuple.GetTupleType(size);
    Type[] typeArray1 = new Type[tupleType2.GetGenericArguments().Length];
    for (int index = 0; index < size; ++index)
    {
      int start1 = start + index * num1;
      int end1 = Math.Min(end, start + (index + 1) * num1);
      typeArray1[index] = MutableTuple.MakeTupleType(types, start1, end1);
    }
    for (int index = size; index < typeArray1.Length; ++index)
      typeArray1[index] = typeof (DynamicNull);
    return tupleType2.MakeGenericType(typeArray1);
  }

  public abstract int Capacity { get; }

  public static Expression Create(params Expression[] values)
  {
    return MutableTuple.CreateNew(MutableTuple.MakeTupleType(ArrayUtils.ConvertAll<Expression, Type>(values, (Func<Expression, Type>) (x => x.Type))), 0, values.Length, values);
  }

  private static int PowerOfTwoRound(int value)
  {
    int num = 1;
    while (value > num)
      num <<= 1;
    return num;
  }

  internal static Expression CreateNew(Type tupleType, int start, int end, Expression[] values)
  {
    int num1 = end - start;
    Expression[] input;
    if (num1 > 128 /*0x80*/)
    {
      int num2 = 1;
      while (num1 > 128 /*0x80*/)
      {
        num1 = (num1 + 128 /*0x80*/ - 1) / 128 /*0x80*/;
        num2 *= 128 /*0x80*/;
      }
      input = new Expression[MutableTuple.PowerOfTwoRound(num1)];
      for (int index = 0; index < num1; ++index)
      {
        int start1 = start + index * num2;
        int end1 = Math.Min(end, start + (index + 1) * num2);
        PropertyInfo propertyInfo = tupleType.GetInheritedProperties("Item" + $"{index:D3}").First<PropertyInfo>();
        input[index] = MutableTuple.CreateNew(propertyInfo.PropertyType, start1, end1, values);
      }
      for (int index = num1; index < input.Length; ++index)
        input[index] = (Expression) Expression.Constant((object) null, typeof (DynamicNull));
    }
    else
    {
      input = new Expression[MutableTuple.PowerOfTwoRound(num1)];
      for (int index = 0; index < num1; ++index)
        input[index] = values[index + start];
      for (int index = num1; index < input.Length; ++index)
        input[index] = (Expression) Expression.Constant((object) null, typeof (DynamicNull));
    }
    return (Expression) Expression.New(tupleType.GetConstructor(ArrayUtils.ConvertAll<Expression, Type>(input, (Func<Expression, Type>) (x => x.Type))), input);
  }
}
