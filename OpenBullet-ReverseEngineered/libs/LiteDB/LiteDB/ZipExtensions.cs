// Decompiled with JetBrains decompiler
// Type: LiteDB.ZipExtensions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal static class ZipExtensions
{
  public static IEnumerable<LiteDB.ZipValues> ZipValues(
    this IEnumerable<BsonValue> first,
    IEnumerable<BsonValue> second,
    IEnumerable<BsonValue> thrid = null)
  {
    IEnumerator<BsonValue> firstEnumerator = first.GetEnumerator();
    IEnumerator<BsonValue> secondEnumerator = second.GetEnumerator();
    IEnumerator<BsonValue> thridEnumerator = thrid?.GetEnumerator();
    BsonValue firstCurrent = BsonValue.Null;
    BsonValue secondCurrent = BsonValue.Null;
    BsonValue thridCurrent = BsonValue.Null;
    while (firstEnumerator.MoveNext())
    {
      firstCurrent = firstEnumerator.Current;
      if (secondEnumerator.MoveNext())
        secondCurrent = secondEnumerator.Current;
      if (thrid != null && thridEnumerator.MoveNext())
        thridCurrent = thridEnumerator.Current;
      yield return new LiteDB.ZipValues(firstCurrent, secondCurrent, thridCurrent);
    }
    while (secondEnumerator.MoveNext())
    {
      secondCurrent = secondEnumerator.Current;
      if (thrid != null && thridEnumerator.MoveNext())
        thridCurrent = thridEnumerator.Current;
      yield return new LiteDB.ZipValues(firstCurrent, secondCurrent, thridCurrent);
    }
    while (thrid != null && thridEnumerator.MoveNext())
    {
      thridCurrent = thridEnumerator.Current;
      yield return new LiteDB.ZipValues(firstCurrent, secondCurrent, thridCurrent);
    }
  }
}
