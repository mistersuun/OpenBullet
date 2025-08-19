// Decompiled with JetBrains decompiler
// Type: LiteDB.DictionaryExtensions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#nullable disable
namespace LiteDB;

internal static class DictionaryExtensions
{
  public static ushort NextIndex<T>(this Dictionary<ushort, T> dict)
  {
    if (dict.Count == 0)
      return 0;
    ushort count = (ushort) dict.Count;
    while (dict.ContainsKey(count))
      ++count;
    return count;
  }

  public static T GetOrDefault<K, T>(this IDictionary<K, T> dict, K key, T defaultValue = null)
  {
    T obj;
    return dict.TryGetValue(key, out obj) ? obj : defaultValue;
  }

  public static void ParseKeyValue(this IDictionary<string, string> dict, string connectionString)
  {
    StringScanner stringScanner = new StringScanner(connectionString);
    while (!stringScanner.HasTerminated)
    {
      string key = stringScanner.Scan("(.*?)=", 1).Trim();
      stringScanner.Scan("\\s*");
      string str;
      if (stringScanner.Match("\""))
      {
        str = stringScanner.Scan("\"((?:\\\\\"|.)*?)\"", 1).Replace("\\\"", "\"");
        stringScanner.Scan("\\s*;?\\s*");
      }
      else
      {
        str = stringScanner.Scan("(.*?);\\s*", 1).Trim();
        if (str.Length == 0)
          str = stringScanner.Scan(".*").Trim();
      }
      dict[key] = str;
    }
  }

  public static T GetValue<T>(this Dictionary<string, string> dict, string key, T defaultValue)
  {
    try
    {
      string s;
      if (!dict.TryGetValue(key, out s))
        return defaultValue;
      if (typeof (T) == typeof (TimeSpan))
        return (T) (ValueType) TimeSpan.Parse(s);
      return typeof (T).GetTypeInfo().IsEnum ? (T) Enum.Parse(typeof (T), s, true) : (T) Convert.ChangeType((object) s, typeof (T));
    }
    catch (Exception ex)
    {
      throw new LiteException($"Invalid connection string value type for [{key}]");
    }
  }

  public static long GetFileSize(
    this Dictionary<string, string> dict,
    string key,
    long defaultValue)
  {
    string input = dict.GetValue<string>(key, (string) null);
    if (input == null)
      return defaultValue;
    Match match = Regex.Match(input, "^(\\d+)\\s*([tgmk])?(b|byte|bytes)?$", RegexOptions.IgnoreCase);
    if (!match.Success)
      return 0;
    long int64 = Convert.ToInt64(match.Groups[1].Value);
    switch (match.Groups[2].Value.ToLower())
    {
      case "t":
        return int64 * 1024L /*0x0400*/ * 1024L /*0x0400*/ * 1024L /*0x0400*/ * 1024L /*0x0400*/;
      case "g":
        return int64 * 1024L /*0x0400*/ * 1024L /*0x0400*/ * 1024L /*0x0400*/;
      case "m":
        return int64 * 1024L /*0x0400*/ * 1024L /*0x0400*/;
      case "k":
        return int64 * 1024L /*0x0400*/;
      case "":
        return int64;
      default:
        return 0;
    }
  }
}
