// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.StringExtensions
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Leaf.xNet;

public static class StringExtensions
{
  public static string[] SubstringsOrEmpty(
    this string self,
    string left,
    string right,
    int startIndex = 0,
    StringComparison comparison = StringComparison.Ordinal,
    int limit = 0)
  {
    if (string.IsNullOrEmpty(self))
      return new string[0];
    if (string.IsNullOrEmpty(left))
      throw new ArgumentNullException(nameof (left));
    if (string.IsNullOrEmpty(right))
      throw new ArgumentNullException(nameof (right));
    if (startIndex < 0 || startIndex >= self.Length)
      throw new ArgumentOutOfRangeException(nameof (startIndex), "Invalid start index");
    int startIndex1 = startIndex;
    int num1 = limit;
    List<string> stringList = new List<string>();
    while (true)
    {
      if (limit > 0)
      {
        --num1;
        if (num1 < 0)
          break;
      }
      int num2 = self.IndexOf(left, startIndex1, comparison);
      if (num2 != -1)
      {
        int startIndex2 = num2 + left.Length;
        int num3 = self.IndexOf(right, startIndex2, comparison);
        if (num3 != -1)
        {
          int length = num3 - startIndex2;
          stringList.Add(self.Substring(startIndex2, length));
          startIndex1 = num3 + right.Length;
        }
        else
          break;
      }
      else
        break;
    }
    return stringList.ToArray();
  }

  public static string[] Substrings(
    this string self,
    string left,
    string right,
    int startIndex = 0,
    StringComparison comparison = StringComparison.Ordinal,
    int limit = 0,
    string[] fallback = null)
  {
    string[] strArray = self.SubstringsOrEmpty(left, right, startIndex, comparison, limit);
    return strArray.Length == 0 ? fallback : strArray;
  }

  public static string[] SubstringsEx(
    this string self,
    string left,
    string right,
    int startIndex = 0,
    StringComparison comparison = StringComparison.Ordinal,
    int limit = 0)
  {
    string[] strArray = self.SubstringsOrEmpty(left, right, startIndex, comparison, limit);
    return strArray.Length != 0 ? strArray : throw new SubstringException($"Substrings not found. Left: \"{left}\". Right: \"{right}\".");
  }

  public static string Substring(
    this string self,
    string left,
    string right,
    int startIndex = 0,
    StringComparison comparison = StringComparison.Ordinal,
    string fallback = null)
  {
    if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right) || startIndex < 0 || startIndex >= self.Length)
      return fallback;
    int num1 = self.IndexOf(left, startIndex, comparison);
    if (num1 == -1)
      return fallback;
    int startIndex1 = num1 + left.Length;
    int num2 = self.IndexOf(right, startIndex1, comparison);
    return num2 == -1 ? fallback : self.Substring(startIndex1, num2 - startIndex1);
  }

  public static string SubstringOrEmpty(
    this string self,
    string left,
    string right,
    int startIndex = 0,
    StringComparison comparison = StringComparison.Ordinal)
  {
    return self.Substring(left, right, startIndex, comparison, string.Empty);
  }

  public static string SubstringEx(
    this string self,
    string left,
    string right,
    int startIndex = 0,
    StringComparison comparison = StringComparison.Ordinal)
  {
    return self.Substring(left, right, startIndex, comparison) ?? throw new SubstringException($"Substring not found. Left: \"{left}\". Right: \"{right}\".");
  }

  public static string SubstringLast(
    this string self,
    string right,
    string left,
    int startIndex = -1,
    StringComparison comparison = StringComparison.Ordinal,
    string notFoundValue = null)
  {
    if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(right) || string.IsNullOrEmpty(left) || startIndex < -1 || startIndex >= self.Length)
      return notFoundValue;
    if (startIndex == -1)
      startIndex = self.Length - 1;
    int num1 = self.LastIndexOf(right, startIndex, comparison);
    switch (num1)
    {
      case -1:
      case 0:
        return notFoundValue;
      default:
        int num2 = self.LastIndexOf(left, num1 - 1, comparison);
        if (num2 == -1 || num1 - num2 == 1)
          return notFoundValue;
        int startIndex1 = num2 + left.Length;
        return self.Substring(startIndex1, num1 - startIndex1);
    }
  }

  public static string SubstringLastOrEmpty(
    this string self,
    string right,
    string left,
    int startIndex = -1,
    StringComparison comparison = StringComparison.Ordinal)
  {
    return self.SubstringLast(right, left, startIndex, comparison, string.Empty);
  }

  public static string SubstringLastEx(
    this string self,
    string right,
    string left,
    int startIndex = -1,
    StringComparison comparison = StringComparison.Ordinal)
  {
    return self.SubstringLast(right, left, startIndex, comparison) ?? throw new SubstringException($"StringBetween not found. Right: \"{right}\". Left: \"{left}\".");
  }

  public static bool ContainsInsensitive(this string self, string value)
  {
    return self.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
  }
}
