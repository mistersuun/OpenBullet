// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.PerfTrack
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#nullable disable
namespace Microsoft.Scripting;

public static class PerfTrack
{
  private static int totalEvents;
  private static readonly Dictionary<PerfTrack.Categories, Dictionary<string, int>> _events = PerfTrack.MakeEventsDictionary();
  private static readonly Dictionary<PerfTrack.Categories, int> summaryStats = new Dictionary<PerfTrack.Categories, int>();

  private static Dictionary<PerfTrack.Categories, Dictionary<string, int>> MakeEventsDictionary()
  {
    Dictionary<PerfTrack.Categories, Dictionary<string, int>> dictionary = new Dictionary<PerfTrack.Categories, Dictionary<string, int>>();
    for (int key = 0; key <= 17; ++key)
      dictionary[(PerfTrack.Categories) key] = new Dictionary<string, int>();
    return dictionary;
  }

  public static void DumpHistogram<TKey>(IDictionary<TKey, int> histogram)
  {
    PerfTrack.DumpHistogram<TKey>(histogram, Console.Out);
  }

  public static void DumpStats() => PerfTrack.DumpStats(Console.Out);

  public static void DumpHistogram<TKey>(IDictionary<TKey, int> histogram, TextWriter output)
  {
    TKey[] items = ArrayUtils.MakeArray<TKey>(histogram.Keys);
    int[] keys = ArrayUtils.MakeArray<int>(histogram.Values);
    Array.Sort<int, TKey>(keys, items);
    for (int index = 0; index < items.Length; ++index)
      output.WriteLine("{0} {1}", (object) items[index], (object) keys[index]);
  }

  public static void AddHistograms<TKey>(
    IDictionary<TKey, int> result,
    IDictionary<TKey, int> addend)
  {
    foreach (KeyValuePair<TKey, int> keyValuePair in (IEnumerable<KeyValuePair<TKey, int>>) addend)
    {
      int num;
      result[keyValuePair.Key] = keyValuePair.Value + (result.TryGetValue(keyValuePair.Key, out num) ? num : 0);
    }
  }

  public static void IncrementEntry<TKey>(IDictionary<TKey, int> histogram, TKey key)
  {
    int num;
    histogram.TryGetValue(key, out num);
    histogram[key] = num + 1;
  }

  public static void DumpStats(TextWriter output)
  {
    if (PerfTrack.totalEvents == 0)
      return;
    output.WriteLine();
    output.WriteLine("---- Performance Details ----");
    output.WriteLine();
    foreach (KeyValuePair<PerfTrack.Categories, Dictionary<string, int>> keyValuePair in PerfTrack._events)
    {
      if (keyValuePair.Value.Count > 0)
      {
        output.WriteLine("Category : " + (object) keyValuePair.Key);
        PerfTrack.DumpHistogram<string>((IDictionary<string, int>) keyValuePair.Value, output);
        output.WriteLine();
      }
    }
    output.WriteLine();
    output.WriteLine("---- Performance Summary ----");
    output.WriteLine();
    double num = 0.0;
    foreach (KeyValuePair<PerfTrack.Categories, int> summaryStat in PerfTrack.summaryStats)
    {
      switch (summaryStat.Key)
      {
        case PerfTrack.Categories.Exceptions:
          output.WriteLine("Total Exception ({0}) = {1}  (throwtime = ~{2} secs)", (object) summaryStat.Key, (object) summaryStat.Value, (object) ((double) summaryStat.Value * 2.5365656E-05));
          num += (double) summaryStat.Value * 2.5365656E-05;
          continue;
        case PerfTrack.Categories.Fields:
          output.WriteLine("Total field = {0} (time = ~{1} secs)", (object) summaryStat.Value, (object) ((double) summaryStat.Value * 1.8080093E-06));
          num += (double) summaryStat.Value * 1.8080093E-06;
          continue;
        case PerfTrack.Categories.Methods:
          output.WriteLine("Total calls = {0} (calltime = ~{1} secs)", (object) summaryStat.Value, (object) ((double) summaryStat.Value * 5.1442355E-06));
          num += (double) summaryStat.Value * 5.1442355E-06;
          continue;
        default:
          output.WriteLine("Total {1} = {0}", (object) summaryStat.Value, (object) summaryStat.Key);
          continue;
      }
    }
    output.WriteLine();
    output.WriteLine("Total Known Times: {0}", (object) num);
  }

  [Conditional("DEBUG")]
  public static void NoteEvent(PerfTrack.Categories category, object key)
  {
    if (!DebugOptions.TrackPerformance)
      return;
    Dictionary<string, int> dictionary = PerfTrack._events[category];
    ++PerfTrack.totalEvents;
    lock (dictionary)
    {
      string key1 = key.ToString();
      if (key is Exception exception)
        key1 = exception.GetType().ToString();
      int num;
      dictionary[key1] = dictionary.TryGetValue(key1, out num) ? num + 1 : 1;
      if (!PerfTrack.summaryStats.TryGetValue(category, out num))
        PerfTrack.summaryStats[category] = 1;
      else
        PerfTrack.summaryStats[category] = num + 1;
    }
  }

  public enum Categories
  {
    Temporary,
    ReflectedTypes,
    Exceptions,
    Properties,
    Fields,
    Methods,
    Compiler,
    DelegateCreate,
    DictInvoke,
    OperatorInvoke,
    OverAllocate,
    Rules,
    RuleEvaluation,
    Binding,
    BindingSlow,
    BindingFast,
    BindingTarget,
    Count,
  }
}
