// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Internal.ReturnedCookie
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Internal;

public class ReturnedCookie : Cookie
{
  private bool isSecure;
  private bool isHttpOnly;

  public ReturnedCookie(
    string name,
    string value,
    string domain,
    string path,
    DateTime? expiry,
    bool isSecure,
    bool isHttpOnly)
    : base(name, value, domain, path, expiry)
  {
    this.isSecure = isSecure;
    this.isHttpOnly = isHttpOnly;
  }

  public override bool Secure => this.isSecure;

  public override bool IsHttpOnly => this.isHttpOnly;

  public override string ToString()
  {
    string[] strArray = new string[8];
    strArray[0] = this.Name;
    strArray[1] = "=";
    strArray[2] = this.Value;
    string str;
    if (this.Expiry.HasValue)
    {
      DateTime universalTime = this.Expiry.Value;
      universalTime = universalTime.ToUniversalTime();
      str = "; expires=" + universalTime.ToString("ddd MM/dd/yyyy HH:mm:ss UTC", (IFormatProvider) CultureInfo.InvariantCulture);
    }
    else
      str = string.Empty;
    strArray[3] = str;
    strArray[4] = string.IsNullOrEmpty(this.Path) ? string.Empty : "; path=" + this.Path;
    strArray[5] = string.IsNullOrEmpty(this.Domain) ? string.Empty : "; domain=" + this.Domain;
    strArray[6] = this.isSecure ? "; secure" : string.Empty;
    strArray[7] = this.isHttpOnly ? "; httpOnly" : string.Empty;
    return string.Concat(strArray);
  }
}
