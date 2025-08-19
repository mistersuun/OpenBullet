// Decompiled with JetBrains decompiler
// Type: AngleSharp.Common.ObjectExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace AngleSharp.Common;

public static class ObjectExtensions
{
  public static Dictionary<string, string> ToDictionary(this object values)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    if (values != null)
    {
      foreach (PropertyInfo property in values.GetType().GetProperties())
      {
        object obj = property.GetValue(values, (object[]) null) ?? (object) string.Empty;
        dictionary.Add(property.Name, obj.ToString());
      }
    }
    return dictionary;
  }

  public static T GetItemByIndex<T>(this IEnumerable<T> items, int index)
  {
    if (index >= 0)
    {
      int num = 0;
      foreach (T itemByIndex in items)
      {
        if (num++ == index)
          return itemByIndex;
      }
    }
    throw new ArgumentOutOfRangeException(nameof (index));
  }

  public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T element)
  {
    yield return element;
    foreach (T obj in items)
      yield return obj;
  }

  public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T element)
  {
    foreach (T obj in items)
    {
      if ((object) obj != (object) (T) element)
        yield return obj;
    }
  }

  public static T? TryGet<T>(this IDictionary<string, object> values, string key) where T : struct
  {
    object obj1;
    return values.TryGetValue(key, out obj1) && obj1 is T obj2 ? new T?(obj2) : new T?();
  }

  public static object TryGet(this IDictionary<string, object> values, string key)
  {
    object obj;
    values.TryGetValue(key, out obj);
    return obj;
  }

  public static U GetOrDefault<T, U>(this IDictionary<T, U> values, T key, U defaultValue)
  {
    U u;
    return !values.TryGetValue(key, out u) ? defaultValue : u;
  }

  public static double Constraint(this double value, double min, double max)
  {
    if (value < min)
      return min;
    return value <= max ? value : max;
  }

  public static string GetMessage<T>(this T code) where T : struct
  {
    return CustomAttributeExtensions.GetCustomAttribute<DomDescriptionAttribute>((MemberInfo) IntrospectionExtensions.GetTypeInfo(typeof (T)).GetDeclaredField(code.ToString()))?.Description ?? "An unknown error occurred.";
  }
}
