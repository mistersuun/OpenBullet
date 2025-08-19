// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.EnumerableFunctions
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace System.Windows.Controls;

internal static class EnumerableFunctions
{
  public static IEnumerable<R> Scan<T, R>(
    this IEnumerable<T> that,
    Func<R, T, R> func,
    R initialValue)
  {
    R acc = initialValue;
    yield return acc;
    IEnumerator<T> enumerator = that.GetEnumerator();
    while (enumerator.MoveNext())
    {
      acc = func(acc, enumerator.Current);
      yield return acc;
    }
  }

  public static IEnumerable<R> Zip<T0, T1, R>(
    IEnumerable<T0> enumerable0,
    IEnumerable<T1> enumerable1,
    Func<T0, T1, R> func)
  {
    IEnumerator<T0> enumerator0 = enumerable0.GetEnumerator();
    IEnumerator<T1> enumerator1 = enumerable1.GetEnumerator();
    while (enumerator0.MoveNext() && enumerator1.MoveNext())
      yield return func(enumerator0.Current, enumerator1.Current);
  }

  public static int? IndexOf<T>(this IEnumerable<T> that, T item)
  {
    IEnumerator<T> enumerator = that.GetEnumerator();
    int num = 0;
    while (enumerator.MoveNext())
    {
      if (object.ReferenceEquals((object) enumerator.Current, (object) item))
        return new int?(num);
      ++num;
    }
    return new int?();
  }

  public static IEnumerable<double> GetWeightedValues(
    this IEnumerable<double> values,
    double percent)
  {
    double total = values.Sum();
    return total == 0.0 ? values.Select<double, double>((Func<double, double>) (_ => 0.0)) : EnumerableFunctions.Zip<double, double, Tuple<double, double>>(values.Scan<double, double>((Func<double, double, double>) ((acc, current) => acc + current), 0.0), values, (Func<double, double, Tuple<double, double>>) ((acc, current) => Tuple.Create<double, double>(acc, current))).Select<Tuple<double, double>, Tuple<double, double>>((Func<Tuple<double, double>, Tuple<double, double>>) (tuple => Tuple.Create<double, double>(tuple.First / total, tuple.Second / total))).Select<Tuple<double, double>, double>((Func<Tuple<double, double>, double>) (tuple =>
    {
      double first = tuple.First;
      double second = tuple.Second;
      if (percent > first && first + second > percent)
        return (percent - first) * total;
      return percent <= first ? 0.0 : 1.0;
    }));
  }
}
