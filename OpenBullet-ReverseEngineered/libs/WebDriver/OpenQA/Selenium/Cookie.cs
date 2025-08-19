// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Cookie
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
[Serializable]
public class Cookie
{
  private string cookieName;
  private string cookieValue;
  private string cookiePath;
  private string cookieDomain;
  private DateTime? cookieExpiry;

  public Cookie(string name, string value, string domain, string path, DateTime? expiry)
  {
    if (string.IsNullOrEmpty(name))
      throw new ArgumentException("Cookie name cannot be null or empty string", nameof (name));
    if (value == null)
      throw new ArgumentNullException(nameof (value), "Cookie value cannot be null");
    this.cookieName = name.IndexOf(';') == -1 ? name : throw new ArgumentException("Cookie names cannot contain a ';': " + name, nameof (name));
    this.cookieValue = value;
    if (!string.IsNullOrEmpty(path))
      this.cookiePath = path;
    this.cookieDomain = Cookie.StripPort(domain);
    if (!expiry.HasValue)
      return;
    this.cookieExpiry = expiry;
  }

  public Cookie(string name, string value, string path, DateTime? expiry)
    : this(name, value, (string) null, path, expiry)
  {
  }

  public Cookie(string name, string value, string path)
    : this(name, value, path, new DateTime?())
  {
  }

  public Cookie(string name, string value)
    : this(name, value, (string) null, new DateTime?())
  {
  }

  [JsonProperty("name")]
  public string Name => this.cookieName;

  [JsonProperty("value")]
  public string Value => this.cookieValue;

  [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
  public string Domain => this.cookieDomain;

  [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
  public virtual string Path => this.cookiePath;

  [JsonProperty("secure")]
  public virtual bool Secure => false;

  [JsonProperty("httpOnly")]
  public virtual bool IsHttpOnly => false;

  public DateTime? Expiry => this.cookieExpiry;

  [JsonProperty("expiry", NullValueHandling = NullValueHandling.Ignore)]
  internal long? ExpirySeconds
  {
    get
    {
      if (!this.cookieExpiry.HasValue)
        return new long?();
      DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      DateTime universalTime = this.cookieExpiry.Value;
      universalTime = universalTime.ToUniversalTime();
      return new long?(Convert.ToInt64(universalTime.Subtract(dateTime).TotalSeconds));
    }
  }

  public static Cookie FromDictionary(Dictionary<string, object> rawCookie)
  {
    string name = rawCookie != null ? rawCookie["name"].ToString() : throw new ArgumentNullException(nameof (rawCookie), "Dictionary cannot be null");
    string empty1 = string.Empty;
    if (rawCookie["value"] != null)
      empty1 = rawCookie["value"].ToString();
    string path = "/";
    if (rawCookie.ContainsKey("path") && rawCookie["path"] != null)
      path = rawCookie["path"].ToString();
    string empty2 = string.Empty;
    if (rawCookie.ContainsKey("domain") && rawCookie["domain"] != null)
      empty2 = rawCookie["domain"].ToString();
    DateTime? expiry = new DateTime?();
    if (rawCookie.ContainsKey("expiry") && rawCookie["expiry"] != null)
    {
      double result = 0.0;
      if (double.TryParse(rawCookie["expiry"].ToString(), NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        try
        {
          expiry = new DateTime?(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(result).ToLocalTime());
        }
        catch (ArgumentOutOfRangeException ex)
        {
          expiry = new DateTime?(DateTime.MaxValue.ToLocalTime());
        }
      }
    }
    bool isSecure = false;
    if (rawCookie.ContainsKey("secure") && rawCookie["secure"] != null)
      isSecure = bool.Parse(rawCookie["secure"].ToString());
    bool isHttpOnly = false;
    if (rawCookie.ContainsKey("httpOnly") && rawCookie["httpOnly"] != null)
      isHttpOnly = bool.Parse(rawCookie["httpOnly"].ToString());
    return (Cookie) new ReturnedCookie(name, empty1, empty2, path, expiry, isSecure, isHttpOnly);
  }

  public override string ToString()
  {
    return $"{this.cookieName}={this.cookieValue}{(!this.cookieExpiry.HasValue ? string.Empty : "; expires=" + this.cookieExpiry.Value.ToUniversalTime().ToString("ddd MM dd yyyy hh:mm:ss UTC", (IFormatProvider) CultureInfo.InvariantCulture))}{(string.IsNullOrEmpty(this.cookiePath) ? string.Empty : "; path=" + this.cookiePath)}{(string.IsNullOrEmpty(this.cookieDomain) ? string.Empty : "; domain=" + this.cookieDomain)}";
  }

  public override bool Equals(object obj)
  {
    Cookie cookie = obj as Cookie;
    if (this == obj)
      return true;
    return cookie != null && this.cookieName.Equals(cookie.cookieName) && (this.cookieValue != null ? (!this.cookieValue.Equals(cookie.cookieValue) ? 1 : 0) : (cookie.Value != null ? 1 : 0)) == 0;
  }

  public override int GetHashCode() => this.cookieName.GetHashCode();

  private static string StripPort(string domain)
  {
    if (string.IsNullOrEmpty(domain))
      return (string) null;
    return domain.Split(':')[0];
  }
}
