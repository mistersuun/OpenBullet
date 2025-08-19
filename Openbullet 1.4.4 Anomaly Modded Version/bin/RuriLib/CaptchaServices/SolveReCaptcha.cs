// Decompiled with JetBrains decompiler
// Type: RuriLib.CaptchaServices.SolveReCaptcha
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;

#nullable disable
namespace RuriLib.CaptchaServices;

public class SolveReCaptcha(string userId, string apiKey, int timeout) : CaptchaService(userId, apiKey, timeout)
{
  public override string SolveRecaptcha(string siteKey, string siteUrl)
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient.Timeout = this.Timeout;
      string message = cwebClient.DownloadString($"https://api.solverecaptcha.com/?userid={this.User}&key={this.Pass}&sitekey={siteKey}&pageurl={siteUrl}");
      if (message.Contains("ERROR"))
        throw new Exception(message);
      this.Status = CaptchaService.CaptchaStatus.Completed;
      return message.Split('|')[1];
    }
  }

  public override double GetBalance()
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      cwebClient.Timeout = 3;
      try
      {
        if (cwebClient.DownloadString($"https://api.solverecaptcha.com/?userid={this.User}&key={this.Pass}&sitekey=test&pageurl=test").Contains("ERROR_ACCESS_DENIED"))
          return 0.0;
      }
      catch
      {
      }
      return 999.0;
    }
  }
}
