// Decompiled with JetBrains decompiler
// Type: RuriLib.CaptchaServices.CaptchasIO
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net;
using System.Threading;

#nullable disable
namespace RuriLib.CaptchaServices;

public class CaptchasIO(string apiKey, int timeout) : CaptchaService(apiKey, timeout)
{
  public override double GetBalance()
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient.Timeout = this.Timeout;
      CaptchasIO.GenericResponse genericResponse = JsonConvert.DeserializeObject<CaptchasIO.GenericResponse>(cwebClient.DownloadString($"http://api.captchas.io/res.php?key={this.ApiKey}&action=getbalance&json=1"));
      int? status = genericResponse.status;
      int num = 0;
      if (status.GetValueOrDefault() == num & status.HasValue)
        throw new Exception(genericResponse.request);
      return double.Parse(genericResponse.request, (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }

  public override string SolveRecaptcha(string siteKey, string siteUrl)
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient.Timeout = this.Timeout;
      CaptchasIO.GenericResponse genericResponse1 = JsonConvert.DeserializeObject<CaptchasIO.GenericResponse>(cwebClient.DownloadString($"http://api.captchas.io/in.php?key={this.ApiKey}&method=userrecaptcha&googlekey={siteKey}&pageurl={siteUrl}&json=1"));
      int? status = genericResponse1.status;
      int num1 = 0;
      if (status.GetValueOrDefault() == num1 & status.HasValue)
        throw new Exception(genericResponse1.request);
      this.TaskId = (object) genericResponse1.request;
      this.Status = CaptchaService.CaptchaStatus.Processing;
      DateTime now = DateTime.Now;
      while (this.Status == CaptchaService.CaptchaStatus.Processing && (DateTime.Now - now).TotalSeconds < (double) this.Timeout)
      {
        Thread.Sleep(5000);
        CaptchasIO.GenericResponse genericResponse2 = JsonConvert.DeserializeObject<CaptchasIO.GenericResponse>(cwebClient.DownloadString($"http://api.captchas.io/res.php?key={this.ApiKey}&action=get&id={this.TaskId}&json=1"));
        if (!genericResponse2.request.Contains("NOT_READY"))
        {
          status = genericResponse2.status;
          int num2 = 0;
          if (status.GetValueOrDefault() == num2 & status.HasValue)
            throw new Exception(genericResponse2.request);
          this.Status = CaptchaService.CaptchaStatus.Completed;
          return genericResponse2.request;
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
      CaptchasIO.GenericResponse genericResponse1 = JsonConvert.DeserializeObject<CaptchasIO.GenericResponse>(cwebClient.UploadString("http://api.captchas.io.com/in.php", $"key={this.ApiKey}&method=base64&regsense=1&body={this.GetBase64(bitmap, ImageFormat.Bmp)}&json=1"));
      int? status = genericResponse1.status;
      int num1 = 0;
      if (status.GetValueOrDefault() == num1 & status.HasValue)
        throw new Exception(genericResponse1.request);
      this.TaskId = (object) genericResponse1.request;
      this.Status = CaptchaService.CaptchaStatus.Processing;
      DateTime now = DateTime.Now;
      while (this.Status == CaptchaService.CaptchaStatus.Processing && (DateTime.Now - now).TotalSeconds < (double) this.Timeout)
      {
        Thread.Sleep(5000);
        CaptchasIO.GenericResponse genericResponse2 = JsonConvert.DeserializeObject<CaptchasIO.GenericResponse>(cwebClient.DownloadString($"http://api.captchas.io/res.php?key={this.ApiKey}&action=get&id={this.TaskId}&json=1"));
        if (!genericResponse2.request.Contains("NOT_READY"))
        {
          status = genericResponse2.status;
          int num2 = 0;
          if (status.GetValueOrDefault() == num2 & status.HasValue)
            throw new Exception(genericResponse2.request);
          this.Status = CaptchaService.CaptchaStatus.Completed;
          return genericResponse2.request;
        }
      }
      throw new TimeoutException();
    }
  }

  private struct GenericResponse
  {
    public int? status;
    public string request;
  }
}
