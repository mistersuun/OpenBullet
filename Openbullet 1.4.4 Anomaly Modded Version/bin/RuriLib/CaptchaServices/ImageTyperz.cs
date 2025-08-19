// Decompiled with JetBrains decompiler
// Type: RuriLib.CaptchaServices.ImageTyperz
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net;
using System.Threading;

#nullable disable
namespace RuriLib.CaptchaServices;

public class ImageTyperz(string apiKey, int timeout) : CaptchaService(apiKey, timeout)
{
  public override double GetBalance()
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient.Timeout = this.Timeout;
      cwebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
      string str = cwebClient.UploadString("http://captchatypers.com/Forms/RequestBalanceToken.ashx", $"token={this.ApiKey}&action=REQUESTBALANCE");
      return !str.Contains("ERROR") ? double.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture) : throw new Exception(str);
    }
  }

  public override string SolveRecaptcha(string siteKey, string siteUrl)
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient.Timeout = this.Timeout;
      cwebClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
      string message1 = cwebClient.UploadString("http://captchatypers.com/captchaapi/UploadRecaptchaToken.ashx", $"token={this.ApiKey}&action=UPLOADCAPTCHA&pageurl={siteUrl}&googlekey={siteKey}");
      this.TaskId = !message1.Contains("ERROR") ? (object) message1 : throw new Exception(message1);
      this.Status = CaptchaService.CaptchaStatus.Processing;
      DateTime now = DateTime.Now;
      while (this.Status == CaptchaService.CaptchaStatus.Processing && (DateTime.Now - now).TotalSeconds < (double) this.Timeout)
      {
        Thread.Sleep(5000);
        cwebClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
        string message2 = cwebClient.UploadString("http://captchatypers.com/captchaapi/GetRecaptchaTextToken.ashx", $"token={this.ApiKey}&action=GETTEXT&captchaID={this.TaskId}");
        if (!message2.Contains("NOTDECODED"))
        {
          if (message2.Contains("ERROR"))
            throw new Exception(message2);
          this.Status = CaptchaService.CaptchaStatus.Completed;
          return message2;
        }
      }
      throw new TimeoutException();
    }
  }

  public override string SolveCaptcha(Bitmap bitmap)
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient.Timeout = this.Timeout;
      cwebClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
      string message = cwebClient.UploadString("http://captchatypers.com/Forms/UploadFileAndGetTextNEWToken.ashx", $"token={this.ApiKey}&action=UPLOADCAPTCHA&chkCase=1&file={WebUtility.UrlEncode(this.GetBase64(bitmap, ImageFormat.Bmp))}");
      if (message.Contains("ERROR"))
        throw new Exception(message);
      this.Status = CaptchaService.CaptchaStatus.Completed;
      return message.Split('|')[1];
    }
  }
}
