// Decompiled with JetBrains decompiler
// Type: RuriLib.CaptchaServices.DeCaptcher
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Drawing;
using System.Globalization;
using System.Net.Http;

#nullable disable
namespace RuriLib.CaptchaServices;

public class DeCaptcher(string user, string pass, int timeout) : CaptchaService(user, pass, timeout)
{
  public override double GetBalance()
  {
    using (HttpClient client = new HttpClient())
    {
      using (MultipartFormDataContent content = new MultipartFormDataContent())
      {
        client.Timeout = TimeSpan.FromSeconds((double) this.Timeout);
        content.Add((HttpContent) new StringContent(this.User), "username");
        content.Add((HttpContent) new StringContent(this.Pass), "password");
        content.Add((HttpContent) new StringContent("balance"), "function");
        return double.Parse(this.PostSync(client, "http://poster.de-captcher.com/", (HttpContent) content), (IFormatProvider) CultureInfo.InvariantCulture);
      }
    }
  }

  public override string SolveRecaptcha(string siteKey, string siteUrl)
  {
    using (HttpClient client = new HttpClient())
    {
      using (MultipartFormDataContent content = new MultipartFormDataContent())
      {
        client.Timeout = TimeSpan.FromSeconds((double) this.Timeout);
        content.Add((HttpContent) new StringContent(this.User), "username");
        content.Add((HttpContent) new StringContent(this.Pass), "password");
        content.Add((HttpContent) new StringContent("proxyurl"), "function");
        content.Add((HttpContent) new StringContent(siteKey), "key");
        content.Add((HttpContent) new StringContent(siteUrl), "url");
        string message = this.PostSync(client, "http://poster.de-captcher.com/", (HttpContent) content);
        try
        {
          return message.Split('|')[5];
        }
        catch
        {
          throw new Exception(message);
        }
      }
    }
  }

  public override string SolveCaptcha(Bitmap bitmap)
  {
    using (HttpClient client = new HttpClient())
    {
      using (MultipartFormDataContent content = new MultipartFormDataContent())
      {
        client.Timeout = TimeSpan.FromSeconds((double) this.Timeout);
        content.Add((HttpContent) new StringContent(this.User), "username");
        content.Add((HttpContent) new StringContent(this.Pass), "password");
        content.Add((HttpContent) new StringContent("picture2"), "function");
        content.Add((HttpContent) new ByteArrayContent(this.GetBytes(bitmap)), "pict");
        content.Add((HttpContent) new StringContent("0"), "picttype");
        string message = this.PostSync(client, "http://poster.de-captcher.com/", (HttpContent) content);
        try
        {
          return message.Split('|')[5];
        }
        catch
        {
          throw new Exception(message);
        }
      }
    }
  }
}
