// Decompiled with JetBrains decompiler
// Type: RuriLib.CaptchaServices.CWebClient
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Net;

#nullable disable
namespace RuriLib.CaptchaServices;

public class CWebClient : WebClient
{
  public int Timeout { get; set; } = 100;

  protected override WebRequest GetWebRequest(Uri uri)
  {
    WebRequest webRequest = base.GetWebRequest(uri);
    webRequest.Timeout = this.Timeout * 1000;
    return webRequest;
  }
}
