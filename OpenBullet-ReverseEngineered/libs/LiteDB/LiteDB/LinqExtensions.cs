// Decompiled with JetBrains decompiler
// Type: LiteDB.LinqExtensions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal static class LinqExtensions
{
  public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
  {
    using (IEnumerator<T> enumerator = source.GetEnumerator())
    {
      while (enumerator.MoveNext())
        yield return LinqExtensions.YieldBatchElements<T>(enumerator, batchSize - 1);
    }
  }

  private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize)
  {
    yield return source.Current;
    for (int i = 0; i < batchSize && source.MoveNext(); ++i)
      yield return source.Current;
  }

  public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector,
    IEqualityComparer<TKey> comparer)
  {
    if (source == null)
      throw new ArgumentNullException(nameof (source));
    if (keySelector == null)
      throw new ArgumentNullException(nameof (keySelector));
    return _();

    IEnumerable<TSource> _()
    {
      HashSet<TKey> knownKeys = new HashSet<TKey>(comparer);
      foreach (TSource source in source)
      {
        if (knownKeys.Add(keySelector(source)))
          yield return source;
      }
    }
  }
}
