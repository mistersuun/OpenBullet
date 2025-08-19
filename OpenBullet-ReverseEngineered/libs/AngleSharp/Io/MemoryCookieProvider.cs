// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.MemoryCookieProvider
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Globalization;
using System.Net;

#nullable disable
namespace AngleSharp.Io;

public class MemoryCookieProvider : ICookieProvider
{
  private readonly CookieContainer _container;

  public MemoryCookieProvider() => this._container = new CookieContainer();

  public CookieContainer Container => this._container;

  public string GetCookie(Url url) => this._container.GetCookieHeader((Uri) url);

  public void SetCookie(Url url, string value)
  {
    string cookieHeader = MemoryCookieProvider.Sanatize(url.HostName, value);
    try
    {
      this._container.SetCookies((Uri) url, cookieHeader);
    }
    catch (CookieException ex)
    {
    }
  }

  private static string Sanatize(string host, string cookie)
  {
    string str1 = "expires=";
    string oldValue = $"Domain={host};";
    int startIndex1;
    for (int startIndex2 = 0; startIndex2 < cookie.Length; startIndex2 = startIndex1)
    {
      int num1 = cookie.IndexOf(str1, startIndex2, StringComparison.OrdinalIgnoreCase);
      if (num1 != -1)
      {
        int num2 = num1 + str1.Length;
        startIndex1 = cookie.IndexOfAny(new char[2]
        {
          ';',
          ','
        }, num2 + 4);
        if (startIndex1 == -1)
          startIndex1 = cookie.Length;
        string str2 = cookie.Substring(0, num2);
        string str3 = cookie.Substring(num2, startIndex1 - num2);
        string str4 = cookie.Substring(startIndex1);
        DateTime result;
        if (DateTime.TryParse(str3.Replace("UTC", "GMT"), out result))
        {
          string str5 = result.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture);
          cookie = str2 + str5 + str4;
        }
      }
      else
        break;
    }
    return cookie.Replace(oldValue, string.Empty);
  }
}
