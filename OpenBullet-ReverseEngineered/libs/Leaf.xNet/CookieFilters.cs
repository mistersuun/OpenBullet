// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.CookieFilters
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet;

public static class CookieFilters
{
  public static bool Enabled = true;
  public static bool Trim = true;
  public static bool Path = true;
  public static bool CommaEndingValue = true;

  public static string Filter(string rawCookie)
  {
    return CookieFilters.Enabled ? rawCookie.TrimWhitespace().FilterPath().FilterInvalidExpireYear().FilterCommaEndingValue() : rawCookie;
  }

  public static string FilterDomain(string domain)
  {
    if (string.IsNullOrWhiteSpace(domain))
      return (string) null;
    domain = domain.Trim('\t', '\n', '\r', ' ');
    return ((domain.Length <= 1 ? 0 : (domain[0] == '.' ? 1 : 0)) & (domain.IndexOf('.', 1) == -1 ? 1 : 0)) == 0 ? domain : domain.Substring(1);
  }

  private static string TrimWhitespace(this string rawCookie)
  {
    return CookieFilters.Trim ? rawCookie.Trim() : rawCookie;
  }

  private static string FilterPath(this string rawCookie)
  {
    if (!CookieFilters.Path)
      return rawCookie;
    int num1 = rawCookie.IndexOf("path=/", 0, StringComparison.OrdinalIgnoreCase);
    if (num1 == -1)
      return rawCookie;
    int num2 = num1 + "path=/".Length;
    if (num2 >= rawCookie.Length - 1 || rawCookie[num2] == ';')
      return rawCookie;
    int num3 = rawCookie.IndexOf(';', num2);
    if (num3 == -1)
      num3 = rawCookie.Length;
    return rawCookie.Remove(num2, num3 - num2);
  }

  private static string FilterCommaEndingValue(this string rawCookie)
  {
    if (!CookieFilters.CommaEndingValue)
      return rawCookie;
    int num1 = rawCookie.IndexOf('=');
    if (num1 == -1 || num1 >= rawCookie.Length - 1)
      return rawCookie;
    int num2 = rawCookie.IndexOf(';', num1 + 1);
    if (num2 == -1)
      num2 = rawCookie.Length - 1;
    int num3 = num2 - 1;
    return rawCookie[num3] == ',' ? rawCookie.Remove(num3, 1).Insert(num3, "%2C") : rawCookie;
  }

  private static string FilterInvalidExpireYear(this string rawCookie)
  {
    int num1 = rawCookie.IndexOf("expires=", StringComparison.OrdinalIgnoreCase);
    if (num1 == -1)
      return rawCookie;
    int startIndex1 = num1 + "expires=".Length;
    int num2 = rawCookie.IndexOf(';', startIndex1);
    if (num2 == -1)
      num2 = rawCookie.Length;
    int num3 = rawCookie.Substring(startIndex1, num2 - startIndex1).IndexOf("9999", StringComparison.Ordinal);
    if (num3 == -1)
      return rawCookie;
    int startIndex2 = num3 + (startIndex1 + "9999".Length - 1);
    return rawCookie.Remove(startIndex2, 1).Insert(startIndex2, "8");
  }
}
