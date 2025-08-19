// Decompiled with JetBrains decompiler
// Type: LiteDB.ExpressionOperators
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class ExpressionOperators
{
  public static IEnumerable<BsonValue> ADD(
    IEnumerable<BsonValue> left,
    IEnumerable<BsonValue> right)
  {
    foreach (ZipValues value in left.ZipValues(right))
    {
      if (value.First.IsString || value.Second.IsString)
        yield return (BsonValue) (value.First.RawValue?.ToString() + value.Second.RawValue?.ToString());
      else if (value.First.IsDateTime && value.Second.IsNumber)
        yield return (BsonValue) value.First.AsDateTime.AddDays(value.Second.AsDouble);
      else if (value.First.IsNumber && value.Second.IsDateTime)
        yield return (BsonValue) value.Second.AsDateTime.AddDays(value.First.AsDouble);
      else if (value.First.IsNumber && value.Second.IsNumber)
        yield return value.First + value.Second;
    }
  }

  public static IEnumerable<BsonValue> MINUS(
    IEnumerable<BsonValue> left,
    IEnumerable<BsonValue> right)
  {
    foreach (ZipValues value in left.ZipValues(right))
    {
      if (value.First.IsDateTime && value.Second.IsNumber)
        yield return (BsonValue) value.First.AsDateTime.AddDays(-value.Second.AsDouble);
      else if (value.First.IsNumber && value.Second.IsDateTime)
        yield return (BsonValue) value.Second.AsDateTime.AddDays(-value.First.AsDouble);
      else if (value.First.IsNumber && value.Second.IsNumber)
        yield return value.First - value.Second;
    }
  }

  public static IEnumerable<BsonValue> MULTIPLY(
    IEnumerable<BsonValue> left,
    IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
    {
      if (zipValue.First.IsNumber && zipValue.Second.IsNumber)
        yield return zipValue.First * zipValue.Second;
    }
  }

  public static IEnumerable<BsonValue> DIVIDE(
    IEnumerable<BsonValue> left,
    IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
    {
      if (zipValue.First.IsNumber && zipValue.Second.IsNumber)
        yield return zipValue.First / zipValue.Second;
    }
  }

  public static IEnumerable<BsonValue> MOD(
    IEnumerable<BsonValue> left,
    IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
    {
      if (zipValue.First.IsNumber && zipValue.Second.IsNumber)
        yield return (BsonValue) ((int) zipValue.First % (int) zipValue.Second);
    }
  }

  public static IEnumerable<BsonValue> EQ(IEnumerable<BsonValue> left, IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
      yield return (BsonValue) (zipValue.First == zipValue.Second);
  }

  public static IEnumerable<BsonValue> NEQ(
    IEnumerable<BsonValue> left,
    IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
      yield return (BsonValue) (zipValue.First != zipValue.Second);
  }

  public static IEnumerable<BsonValue> GT(IEnumerable<BsonValue> left, IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
      yield return (BsonValue) (zipValue.First > zipValue.Second);
  }

  public static IEnumerable<BsonValue> GTE(
    IEnumerable<BsonValue> left,
    IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
      yield return (BsonValue) (zipValue.First >= zipValue.Second);
  }

  public static IEnumerable<BsonValue> LT(IEnumerable<BsonValue> left, IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
      yield return (BsonValue) (zipValue.First < zipValue.Second);
  }

  public static IEnumerable<BsonValue> LTE(
    IEnumerable<BsonValue> left,
    IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
      yield return (BsonValue) (zipValue.First <= zipValue.Second);
  }

  public static IEnumerable<BsonValue> AND(
    IEnumerable<BsonValue> left,
    IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
      yield return (BsonValue) ((bool) zipValue.First && (bool) zipValue.Second);
  }

  public static IEnumerable<BsonValue> OR(IEnumerable<BsonValue> left, IEnumerable<BsonValue> right)
  {
    foreach (ZipValues zipValue in left.ZipValues(right))
      yield return (BsonValue) ((bool) zipValue.First || (bool) zipValue.Second);
  }

  public static IEnumerable<BsonValue> DOCUMENT(
    IEnumerable<BsonValue> keys,
    IEnumerable<IEnumerable<BsonValue>> values)
  {
    BsonDocument bsonDocument = new BsonDocument();
    foreach (ZipValues zipValue in keys.ZipValues(values.Select<IEnumerable<BsonValue>, BsonValue>((Func<IEnumerable<BsonValue>, BsonValue>) (x => x.FirstOrDefault<BsonValue>()))))
    {
      BsonValue first = zipValue.First;
      BsonValue second = zipValue.Second;
      if (second != (BsonValue) null)
        bsonDocument[(string) first] = second;
    }
    yield return (BsonValue) bsonDocument;
  }

  public static IEnumerable<BsonValue> ARRAY(IEnumerable<IEnumerable<BsonValue>> values)
  {
    yield return (BsonValue) new BsonArray(values.SelectMany<IEnumerable<BsonValue>, BsonValue>((Func<IEnumerable<BsonValue>, IEnumerable<BsonValue>>) (x => x)));
  }
}
