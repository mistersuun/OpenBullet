// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.ContractUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class ContractUtils
{
  [Conditional("DEBUG")]
  public static void Assert(bool precondition)
  {
  }

  public static void Requires(bool precondition)
  {
    if (!precondition)
      throw new ArgumentException(Strings.MethodPreconditionViolated);
  }

  public static void Requires(bool precondition, string paramName)
  {
    if (!precondition)
      throw new ArgumentException(Strings.InvalidArgumentValue, paramName);
  }

  public static void Requires(bool precondition, string paramName, string message)
  {
    if (!precondition)
      throw new ArgumentException(message, paramName);
  }

  public static void RequiresNotNull(object value, string paramName)
  {
    if (value == null)
      throw new ArgumentNullException(paramName);
  }

  public static void RequiresNotEmpty(string str, string paramName)
  {
    ContractUtils.RequiresNotNull((object) str, paramName);
    if (str.Length == 0)
      throw new ArgumentException(Strings.NonEmptyStringRequired, paramName);
  }

  public static void RequiresNotEmpty<T>(ICollection<T> collection, string paramName)
  {
    ContractUtils.RequiresNotNull((object) collection, paramName);
    if (collection.Count == 0)
      throw new ArgumentException(Strings.NonEmptyCollectionRequired, paramName);
  }

  public static void RequiresArrayIndex<T>(IList<T> array, int index, string indexName)
  {
    ContractUtils.RequiresArrayIndex(array.Count, index, indexName);
  }

  public static void RequiresArrayIndex(int arraySize, int index, string indexName)
  {
    if (index < 0 || index >= arraySize)
      throw new ArgumentOutOfRangeException(indexName);
  }

  public static void RequiresArrayInsertIndex<T>(IList<T> array, int index, string indexName)
  {
    ContractUtils.RequiresArrayInsertIndex(array.Count, index, indexName);
  }

  public static void RequiresArrayInsertIndex(int arraySize, int index, string indexName)
  {
    if (index < 0 || index > arraySize)
      throw new ArgumentOutOfRangeException(indexName);
  }

  public static void RequiresArrayRange<T>(
    IList<T> array,
    int offset,
    int count,
    string offsetName,
    string countName)
  {
    ContractUtils.RequiresArrayRange(array.Count, offset, count, offsetName, countName);
  }

  public static void RequiresArrayRange(
    int arraySize,
    int offset,
    int count,
    string offsetName,
    string countName)
  {
    if (count < 0)
      throw new ArgumentOutOfRangeException(countName);
    if (offset < 0 || arraySize - offset < count)
      throw new ArgumentOutOfRangeException(offsetName);
  }

  public static void RequiresListRange(
    IList array,
    int offset,
    int count,
    string offsetName,
    string countName)
  {
    if (count < 0)
      throw new ArgumentOutOfRangeException(countName);
    if (offset < 0 || array.Count - offset < count)
      throw new ArgumentOutOfRangeException(offsetName);
  }

  public static void RequiresArrayRange(
    string str,
    int offset,
    int count,
    string offsetName,
    string countName)
  {
    if (count < 0)
      throw new ArgumentOutOfRangeException(countName);
    if (offset < 0 || str.Length - offset < count)
      throw new ArgumentOutOfRangeException(offsetName);
  }

  public static void RequiresNotNullItems<T>(IList<T> array, string arrayName)
  {
    ContractUtils.RequiresNotNull((object) array, arrayName);
    for (int index = 0; index < array.Count; ++index)
    {
      if ((object) array[index] == null)
        throw ExceptionUtils.MakeArgumentItemNullException(index, arrayName);
    }
  }

  public static void RequiresNotNullItems<T>(IEnumerable<T> collection, string collectionName)
  {
    ContractUtils.RequiresNotNull((object) collection, collectionName);
    int index = 0;
    foreach (T obj in collection)
    {
      if ((object) obj == null)
        throw ExceptionUtils.MakeArgumentItemNullException(index, collectionName);
      ++index;
    }
  }

  [Conditional("FALSE")]
  public static void Invariant(bool condition)
  {
  }

  [Conditional("FALSE")]
  public static void Invariant(bool condition, string message)
  {
  }

  [Conditional("FALSE")]
  public static void Ensures(bool condition)
  {
  }

  [Conditional("FALSE")]
  public static void Ensures(bool condition, string message)
  {
  }

  public static T Result<T>() => default (T);

  public static T Parameter<T>(out T value)
  {
    value = default (T);
    return value;
  }

  public static T Old<T>(T value) => value;
}
