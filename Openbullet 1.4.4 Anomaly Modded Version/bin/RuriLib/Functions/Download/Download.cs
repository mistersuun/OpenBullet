// Decompiled with JetBrains decompiler
// Type: RuriLib.Functions.Download.Download
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using RuriLib.Models;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace RuriLib.Functions.Download;

public static class Download
{
  public static void RemoteFile(
    string fileName,
    string url,
    bool useProxies,
    CProxy proxy,
    Dictionary<string, string> cookies,
    out Dictionary<string, string> newCookies,
    int timeout,
    string userAgent = "")
  {
    HttpRequest httpRequest = new HttpRequest();
    if (userAgent != "")
      httpRequest.UserAgent = userAgent;
    httpRequest.Cookies = new CookieDictionary();
    foreach (KeyValuePair<string, string> cookie in cookies)
      httpRequest.Cookies.Add(cookie.Key, cookie.Value);
    if (useProxies)
    {
      httpRequest.Proxy = proxy.GetClient();
      httpRequest.Proxy.ReadWriteTimeout = timeout;
      httpRequest.Proxy.ConnectTimeout = timeout;
      httpRequest.Proxy.Username = proxy.Username;
      httpRequest.Proxy.Password = proxy.Password;
    }
    HttpResponse httpResponse = httpRequest.Get(url);
    using (Stream memoryStream = (Stream) httpResponse.ToMemoryStream())
    {
      using (Stream stream = (Stream) File.OpenWrite(fileName))
      {
        byte[] buffer = new byte[4096 /*0x1000*/];
        int count;
        do
        {
          count = memoryStream.Read(buffer, 0, buffer.Length);
          stream.Write(buffer, 0, count);
        }
        while (count != 0);
      }
    }
    newCookies = (Dictionary<string, string>) httpResponse.Cookies;
  }
}
