// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.CookieStorage
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

#nullable disable
namespace Leaf.xNet;

[Serializable]
public class CookieStorage
{
  private bool _unescapeValuesOnSend;
  private bool _unescapeValuesOnSendCustomized;
  private static BinaryFormatter _binaryFormatter;

  public CookieContainer Container { get; private set; }

  public int Count => this.Container.Count;

  public bool IsLocked { get; set; }

  public static bool DefaultExpireBeforeSet { get; set; } = true;

  public bool ExpireBeforeSet { get; set; } = CookieStorage.DefaultExpireBeforeSet;

  public bool EscapeValuesOnReceive { get; set; } = true;

  public bool UnescapeValuesOnSend
  {
    get
    {
      return this._unescapeValuesOnSendCustomized ? this._unescapeValuesOnSend : this.EscapeValuesOnReceive;
    }
    set
    {
      this._unescapeValuesOnSendCustomized = true;
      this._unescapeValuesOnSend = value;
    }
  }

  private static BinaryFormatter Bf
  {
    get
    {
      return CookieStorage._binaryFormatter ?? (CookieStorage._binaryFormatter = new BinaryFormatter());
    }
  }

  public CookieStorage(bool isLocked = false, CookieContainer container = null)
  {
    this.IsLocked = isLocked;
    this.Container = container ?? new CookieContainer();
  }

  public void Add(Cookie cookie) => this.Container.Add(cookie);

  public void Add(CookieCollection cookies) => this.Container.Add(cookies);

  public void Set(Cookie cookie)
  {
    cookie.Name = cookie.Name.Trim();
    cookie.Value = cookie.Value.Trim();
    if (this.ExpireBeforeSet)
      this.ExpireIfExists(cookie);
    this.Add(cookie);
  }

  public void Set(CookieCollection cookies)
  {
    if (this.ExpireBeforeSet)
    {
      foreach (Cookie cookie in cookies)
        this.ExpireIfExists(cookie);
    }
    this.Add(cookies);
  }

  public void Set(string name, string value, string domain, string path = "/")
  {
    this.Set(new Cookie(name, value, path, domain));
  }

  public void Set(Uri requestAddress, string rawCookie)
  {
    string[] strArray1 = rawCookie.Split(new char[1]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
    if (strArray1.Length == 0)
      return;
    string[] strArray2 = strArray1[0].Split(new char[1]
    {
      '='
    }, 2);
    strArray2[0] = strArray2[0].Trim();
    strArray2[1] = strArray2[1].Trim();
    Cookie cookie = new Cookie(strArray2[0], strArray2.Length < 2 ? string.Empty : (this.EscapeValuesOnReceive ? Uri.EscapeDataString(strArray2[1]) : strArray2[1]));
    bool flag = false;
    for (int index = 1; index < strArray1.Length; ++index)
    {
      string[] strArray3 = strArray1[index].Split(new char[1]
      {
        '='
      }, 2);
      string lower = strArray3[0].Trim().ToLower();
      string str1 = strArray3.Length < 2 ? (string) null : strArray3[1].Trim();
      switch (lower)
      {
        case "expires":
          DateTime result;
          if (!DateTime.TryParse(str1, out result) || result.Year >= 9999)
            result = new DateTime(9998, 12, 31 /*0x1F*/, 23, 59, 59, DateTimeKind.Local);
          cookie.Expires = result;
          break;
        case "path":
          cookie.Path = str1;
          break;
        case "domain":
          string str2 = CookieFilters.FilterDomain(str1);
          if (str2 != null)
          {
            flag = true;
            cookie.Domain = str2;
            break;
          }
          break;
        case "secure":
          cookie.Secure = true;
          break;
        case "httponly":
          cookie.HttpOnly = true;
          break;
      }
    }
    if (!flag)
    {
      if (string.IsNullOrEmpty(cookie.Path) || cookie.Path.StartsWith("/"))
        cookie.Domain = requestAddress.Host;
      else if (cookie.Path.Contains("."))
      {
        string path = cookie.Path;
        cookie.Domain = path;
        cookie.Path = (string) null;
      }
    }
    this.Set(cookie);
  }

  public void Set(string requestAddress, string rawCookie)
  {
    this.Set(new Uri(requestAddress), rawCookie);
  }

  private void ExpireIfExists(Uri uri, string cookieName)
  {
    foreach (Cookie cookie in this.Container.GetCookies(uri))
    {
      if (cookie.Name == cookieName)
        cookie.Expired = true;
    }
  }

  private void ExpireIfExists(Cookie cookie)
  {
    if (string.IsNullOrEmpty(cookie.Domain))
      return;
    string str = cookie.Domain[0] == '.' ? cookie.Domain.Substring(1) : cookie.Domain;
    this.ExpireIfExists(new Uri((cookie.Secure ? "https://" : "http://") + str), cookie.Name);
  }

  public void Clear() => this.Container = new CookieContainer();

  public void Remove(string url) => this.Remove(new Uri(url));

  public void Remove(Uri uri)
  {
    foreach (Cookie cookie in this.Container.GetCookies(uri))
      cookie.Expired = true;
  }

  public void Remove(string url, string name) => this.Remove(new Uri(url), name);

  public void Remove(Uri uri, string name)
  {
    foreach (Cookie cookie in this.Container.GetCookies(uri))
    {
      if (cookie.Name == name)
        cookie.Expired = true;
    }
  }

  public string GetCookieHeader(Uri uri)
  {
    string cookieHeader = this.Container.GetCookieHeader(uri);
    if (!this.UnescapeValuesOnSend)
      return cookieHeader;
    StringBuilder stringBuilder = new StringBuilder();
    string str1 = cookieHeader;
    char[] separator1 = new char[1]{ ';' };
    foreach (string str2 in str1.Split(separator1, StringSplitOptions.RemoveEmptyEntries))
    {
      char[] separator2 = new char[1]{ '=' };
      string[] strArray = str2.Split(separator2, 2);
      stringBuilder.Append(strArray[0].Trim());
      stringBuilder.Append('=');
      stringBuilder.Append(Uri.UnescapeDataString(strArray[1].Trim()));
      stringBuilder.Append("; ");
    }
    if (stringBuilder.Length > 0)
      stringBuilder.Remove(stringBuilder.Length - 2, 2);
    return stringBuilder.ToString();
  }

  public string GetCookieHeader(string url) => this.GetCookieHeader(new Uri(url));

  public CookieCollection GetCookies(Uri uri) => this.Container.GetCookies(uri);

  public CookieCollection GetCookies(string url) => this.GetCookies(new Uri(url));

  public bool Contains(Uri uri, string cookieName)
  {
    return this.Container.Count > 0 && this.Container.GetCookies(uri)[cookieName] != null;
  }

  public bool Contains(string url, string cookieName) => this.Contains(new Uri(url), cookieName);

  public void SaveToFile(string filePath, bool overwrite = true)
  {
    if (!overwrite && System.IO.File.Exists(filePath))
      throw new ArgumentException(string.Format(Resources.CookieStorage_SaveToFile_FileAlreadyExists, (object) filePath), nameof (filePath));
    using (FileStream serializationStream = new FileStream(filePath, FileMode.OpenOrCreate))
      CookieStorage.Bf.Serialize((Stream) serializationStream, (object) this);
  }

  public static CookieStorage LoadFromFile(string filePath)
  {
    if (!System.IO.File.Exists(filePath))
      throw new FileNotFoundException($"Файл с куками '${filePath}' не найден", nameof (filePath));
    using (FileStream serializationStream = new FileStream(filePath, FileMode.Open))
      return (CookieStorage) CookieStorage.Bf.Deserialize((Stream) serializationStream);
  }
}
