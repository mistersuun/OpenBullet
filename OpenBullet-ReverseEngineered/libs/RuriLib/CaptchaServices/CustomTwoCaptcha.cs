// Decompiled with JetBrains decompiler
// Type: RuriLib.CaptchaServices.CustomTwoCaptcha
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

public class CustomTwoCaptcha(string apiKey, string domain, int port, int timeout) : CaptchaService(apiKey, domain, port, timeout)
{
  public override double GetBalance()
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient.Timeout = this.Timeout;
      return double.Parse(cwebClient.DownloadString($"http://{this.Domain}:{this.Port}/res.php?key={this.ApiKey}&action=getbalance"), (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }

  public override string SolveRecaptcha(string siteKey, string siteUrl)
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient.Timeout = this.Timeout;
      string message1 = cwebClient.DownloadString($"http://{this.Domain}:{this.Port}/in.php?key={this.ApiKey}&method=userrecaptcha&googlekey={siteKey}&pageurl={siteUrl}");
      this.TaskId = message1.StartsWith("OK") ? (object) message1.Split('|')[1] : throw new Exception(message1);
      this.Status = CaptchaService.CaptchaStatus.Processing;
      DateTime now = DateTime.Now;
      while (this.Status == CaptchaService.CaptchaStatus.Processing && (DateTime.Now - now).TotalSeconds < (double) this.Timeout)
      {
        Thread.Sleep(5000);
        string message2 = cwebClient.DownloadString($"http://{this.Domain}:{this.Port}/res.php?key={this.ApiKey}&action=get&id={this.TaskId}");
        if (!message2.Contains("NOT_READY"))
        {
          if (message2.Contains("ERROR"))
            throw new Exception(message2);
          this.Status = CaptchaService.CaptchaStatus.Completed;
          return message2.Split('|')[1];
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
      string message1 = cwebClient.UploadString($"http://{this.Domain}{this.Port}/in.php", $"key={this.ApiKey}&method=base64&regsense=1&body={this.EscapeLongString(this.GetBase64(bitmap, ImageFormat.Bmp))}");
      this.TaskId = message1.StartsWith("OK") ? (object) message1.Split('|')[1] : throw new Exception(message1);
      this.Status = CaptchaService.CaptchaStatus.Processing;
      DateTime now = DateTime.Now;
      while (this.Status == CaptchaService.CaptchaStatus.Processing && (DateTime.Now - now).TotalSeconds < (double) this.Timeout)
      {
        Thread.Sleep(5000);
        string message2 = cwebClient.DownloadString($"http://{this.Domain}:{this.Port}/res.php?key={this.ApiKey}&action=get&id={this.TaskId}");
        if (!message2.Contains("NOT_READY"))
        {
          if (message2.Contains("ERROR"))
            throw new Exception(message2);
          this.Status = CaptchaService.CaptchaStatus.Completed;
          return message2.Split('|')[1];
        }
      }
      throw new TimeoutException();
    }
  }

  private class GenericResponse
  {
    public int status;
    public string request;
  }
}
