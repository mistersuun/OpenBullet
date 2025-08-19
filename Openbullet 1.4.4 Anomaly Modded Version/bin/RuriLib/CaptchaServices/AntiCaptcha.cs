// Decompiled with JetBrains decompiler
// Type: RuriLib.CaptchaServices.AntiCaptcha
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace RuriLib.CaptchaServices;

public class AntiCaptcha(string apiKey, int timeout) : CaptchaService(apiKey, timeout)
{
  public override double GetBalance()
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient.Timeout = this.Timeout;
      cwebClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
      AntiCaptcha.GetBalanceResponse getBalanceResponse = JsonConvert.DeserializeObject<AntiCaptcha.GetBalanceResponse>(cwebClient.UploadString("https://api.anti-captcha.com/getBalance", JsonConvert.SerializeObject((object) new AntiCaptcha.GetBalanceRequest(this.ApiKey))));
      return getBalanceResponse.errorId <= 0 ? getBalanceResponse.balance : throw new Exception(getBalanceResponse.errorCode);
    }
  }

  public override string SolveRecaptcha(string siteKey, string siteUrl)
  {
    using (CWebClient cwebClient1 = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient1.Timeout = this.Timeout;
      cwebClient1.Headers.Add(HttpRequestHeader.ContentType, "application/json");
      AntiCaptcha.CreateTaskResponse createTaskResponse = JsonConvert.DeserializeObject<AntiCaptcha.CreateTaskResponse>(cwebClient1.UploadString("https://api.anti-captcha.com/createTask", JsonConvert.SerializeObject((object) new AntiCaptcha.CreateRecaptchaTaskRequest(this.ApiKey, siteUrl, siteKey))));
      this.TaskId = createTaskResponse.errorId <= 0 ? (object) createTaskResponse.taskId : throw new Exception(createTaskResponse.errorCode);
      this.Status = CaptchaService.CaptchaStatus.Processing;
      DateTime now = DateTime.Now;
      while (this.Status == CaptchaService.CaptchaStatus.Processing && (DateTime.Now - now).TotalSeconds < (double) this.Timeout)
      {
        Thread.Sleep(5000);
        cwebClient1.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        CWebClient cwebClient2 = cwebClient1;
        // ISSUE: reference to a compiler-generated field
        if (AntiCaptcha.\u003C\u003Eo__2.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          AntiCaptcha.\u003C\u003Eo__2.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, string, object, AntiCaptcha.GetTaskResultRequest>>.Create(Binder.InvokeConstructor(CSharpBinderFlags.None, typeof (AntiCaptcha), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        string data = JsonConvert.SerializeObject((object) AntiCaptcha.\u003C\u003Eo__2.\u003C\u003Ep__0.Target((CallSite) AntiCaptcha.\u003C\u003Eo__2.\u003C\u003Ep__0, typeof (AntiCaptcha.GetTaskResultRequest), this.ApiKey, this.TaskId));
        AntiCaptcha.GetTaskResultRecaptchaResponse recaptchaResponse = JsonConvert.DeserializeObject<AntiCaptcha.GetTaskResultRecaptchaResponse>(cwebClient2.UploadString("https://api.anti-captcha.com/getTaskResult", data));
        if (recaptchaResponse.errorId > 0)
          throw new Exception(recaptchaResponse.errorCode);
        if (recaptchaResponse.status == "ready")
        {
          this.Status = CaptchaService.CaptchaStatus.Completed;
          return recaptchaResponse.solution.gRecaptchaResponse;
        }
      }
      throw new TimeoutException();
    }
  }

  public override string SolveCaptcha(Bitmap bitmap)
  {
    using (CWebClient cwebClient1 = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient1.Timeout = this.Timeout;
      cwebClient1.Headers.Add(HttpRequestHeader.ContentType, "application/json");
      AntiCaptcha.CreateTaskResponse createTaskResponse = JsonConvert.DeserializeObject<AntiCaptcha.CreateTaskResponse>(cwebClient1.UploadString("https://api.anti-captcha.com/createTask", JsonConvert.SerializeObject((object) new AntiCaptcha.CreateCaptchaTaskRequest(this.ApiKey, this.GetBase64(bitmap, ImageFormat.Png)))));
      this.TaskId = createTaskResponse.errorId <= 0 ? (object) createTaskResponse.taskId : throw new Exception(createTaskResponse.errorCode);
      this.Status = CaptchaService.CaptchaStatus.Processing;
      DateTime now = DateTime.Now;
      while (this.Status == CaptchaService.CaptchaStatus.Processing && (DateTime.Now - now).TotalSeconds < (double) this.Timeout)
      {
        Thread.Sleep(5000);
        cwebClient1.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        CWebClient cwebClient2 = cwebClient1;
        // ISSUE: reference to a compiler-generated field
        if (AntiCaptcha.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          AntiCaptcha.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, string, object, AntiCaptcha.GetTaskResultRequest>>.Create(Binder.InvokeConstructor(CSharpBinderFlags.None, typeof (AntiCaptcha), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        string data = JsonConvert.SerializeObject((object) AntiCaptcha.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) AntiCaptcha.\u003C\u003Eo__3.\u003C\u003Ep__0, typeof (AntiCaptcha.GetTaskResultRequest), this.ApiKey, this.TaskId));
        AntiCaptcha.GetTaskResultCaptchaResponse resultCaptchaResponse = JsonConvert.DeserializeObject<AntiCaptcha.GetTaskResultCaptchaResponse>(cwebClient2.UploadString("https://api.anti-captcha.com/getTaskResult", data));
        if (resultCaptchaResponse.errorId > 0)
          throw new Exception(resultCaptchaResponse.errorCode);
        if (resultCaptchaResponse.status == "ready")
        {
          this.Status = CaptchaService.CaptchaStatus.Completed;
          return resultCaptchaResponse.solution.text;
        }
      }
      throw new TimeoutException();
    }
  }

  private class GetBalanceRequest
  {
    public string clientKey;

    public GetBalanceRequest(string apiKey) => this.clientKey = apiKey;
  }

  private class GetBalanceResponse
  {
    public int errorId;
    public string errorCode;
    public double balance;
  }

  private class CreateRecaptchaTaskRequest
  {
    public string clientKey;
    public AntiCaptcha.RecaptchaTask task;

    public CreateRecaptchaTaskRequest(string apiKey, string url, string key)
    {
      this.clientKey = apiKey;
      this.task = new AntiCaptcha.RecaptchaTask(url, key);
    }
  }

  private class RecaptchaTask
  {
    public string type = "NoCaptchaTaskProxyless";
    public string websiteURL;
    public string websiteKey;

    public RecaptchaTask(string url, string key)
    {
      this.websiteURL = url;
      this.websiteKey = key;
    }
  }

  private class CreateCaptchaTaskRequest
  {
    public string clientKey;
    public AntiCaptcha.CaptchaTask task;

    public CreateCaptchaTaskRequest(string apiKey, string base64)
    {
      this.clientKey = apiKey;
      this.task = new AntiCaptcha.CaptchaTask(base64);
    }
  }

  private class CaptchaTask
  {
    public string type = "ImageToTextTask";
    public string body;

    public CaptchaTask(string base64) => this.body = base64;
  }

  private class CreateTaskResponse
  {
    public int errorId;
    public int taskId;
    public string errorCode;
  }

  private class GetTaskResultRequest
  {
    public string clientKey;
    public int taskId;

    public GetTaskResultRequest(string apiKey, int id)
    {
      this.clientKey = apiKey;
      this.taskId = id;
    }
  }

  private class GetTaskResultCaptchaResponse
  {
    public int errorId;
    public string errorCode;
    public string status;
    public AntiCaptcha.CaptchaSolution solution;
  }

  private class GetTaskResultRecaptchaResponse
  {
    public int errorId;
    public string errorCode;
    public string status;
    public AntiCaptcha.RecaptchaSolution solution;
  }

  private class RecaptchaSolution
  {
    public string gRecaptchaResponse;
  }

  private class CaptchaSolution
  {
    public string text;
  }
}
