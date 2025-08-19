// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.Captcha.RucaptchaSolver
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading;

#nullable disable
namespace Leaf.xNet.Services.Captcha;

public class RucaptchaSolver : BaseCaptchaSolver
{
  public string Host { get; protected set; } = "rucaptcha.com";

  public CaptchaProxy Proxy { get; set; }

  public override string SolveRecaptcha(
    string pageUrl,
    string siteKey,
    CancellationToken cancelToken = default (CancellationToken))
  {
    this.ThrowIfApiKeyRequiredAndInvalid();
    if (string.IsNullOrEmpty(pageUrl))
      throw new ArgumentException($"Invalid argument: \"pageUrl\" = {pageUrl ?? "null"} when called \"SolveRecaptcha\"", nameof (pageUrl));
    if (string.IsNullOrEmpty(siteKey))
      throw new ArgumentException($"Invalid argument: \"siteKey\" = {siteKey ?? "null"} when called \"SolveRecaptcha\"", nameof (siteKey));
    NameValueCollection data = new NameValueCollection()
    {
      {
        "key",
        this.ApiKey
      },
      {
        "method",
        "userrecaptcha"
      },
      {
        "googlekey",
        siteKey
      },
      {
        "pageurl",
        pageUrl
      }
    };
    if (this.Proxy.IsValid)
    {
      data.Add("proxy", this.Proxy.Address);
      data.Add("proxytype", this.Proxy.Type.ToString());
    }
    string message = "unknown";
    bool flag1 = true;
    for (int index = 0; (long) index < (long) this.UploadRetries; ++index)
    {
      cancelToken.ThrowIfCancellationRequested();
      message = Encoding.UTF8.GetString(this.Http.UploadValues($"http://{this.Host}/in.php", data));
      if (!message.Contains("ERROR_NO_SLOT_AVAILABLE"))
      {
        flag1 = !message.Contains("OK|");
        break;
      }
      this.Delay(this.UploadDelayOnNoSlotAvailable, cancelToken);
    }
    if (flag1)
      throw new CaptchaException(message);
    string str1 = message.Replace("OK|", "").Trim();
    bool flag2 = true;
    this.Delay(this.BeforeStatusCheckingDelay, cancelToken);
    for (int index = 0; (long) index < (long) this.StatusRetries; ++index)
    {
      message = this.Http.DownloadString($"http://{this.Host}/res.php?key={this.ApiKey}&action=get&id={str1}");
      if (!message.Contains("CAPCHA_NOT_READY"))
      {
        flag2 = !message.Contains("OK|");
        break;
      }
      this.Delay(this.StatusDelayOnNotReady, cancelToken);
    }
    cancelToken.ThrowIfCancellationRequested();
    if (flag2)
      throw new CaptchaException(message);
    string str2 = message.Replace("OK|", "");
    return !string.IsNullOrEmpty(str2) ? str2 : throw new CaptchaException(CaptchaError.EmptyResponse);
  }
}
