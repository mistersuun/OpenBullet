// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.SendingRemoteHttpRequestEventArgs
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Net;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class SendingRemoteHttpRequestEventArgs : EventArgs
{
  private HttpWebRequest request;
  private string requestBody;

  public SendingRemoteHttpRequestEventArgs(HttpWebRequest request, string requestBody)
  {
    this.request = request;
    this.requestBody = requestBody;
  }

  public HttpWebRequest Request => this.request;

  public string RequestBody => this.requestBody;
}
