// Decompiled with JetBrains decompiler
// Type: RuriLib.CaptchaServices.DeathByCaptcha
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using Newtonsoft.Json;
using RuriLib.Functions.Requests;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Threading;

#nullable disable
namespace RuriLib.CaptchaServices;

public class DeathByCaptcha(string user, string pass, int timeout) : CaptchaService(user, pass, timeout)
{
  public override double GetBalance()
  {
    using (CWebClient cwebClient = new CWebClient())
    {
      if (this.Timeout > 0)
        cwebClient.Timeout = this.Timeout;
      cwebClient.Headers.Add(HttpRequestHeader.Accept, "application/json");
      cwebClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
      DeathByCaptcha.GetBalanceResponse getBalanceResponse = JsonConvert.DeserializeObject<DeathByCaptcha.GetBalanceResponse>(cwebClient.UploadString("http://api.dbcapi.me/api/user", $"username={this.User}&password={this.Pass}"));
      if (getBalanceResponse.status != 0)
        throw new Exception("Invalid Credentials");
      return getBalanceResponse.balance / 100.0;
    }
  }

  public override string SolveRecaptcha(string siteKey, string siteUrl)
  {
    HttpRequest httpRequest1 = new HttpRequest();
    httpRequest1.AddHeader(HttpHeader.Accept, "application/json");
    string message = httpRequest1.Post("http://api.dbcapi.me/api/captcha", (HttpContent) new Extreme.Net.MultipartContent(Request.GenerateMultipartBoundary())
    {
      {
        (HttpContent) new StringContent(this.User),
        "username"
      },
      {
        (HttpContent) new StringContent(this.Pass),
        "password"
      },
      {
        (HttpContent) new StringContent("4"),
        "type"
      },
      {
        (HttpContent) new StringContent(JsonConvert.SerializeObject((object) new DeathByCaptcha.CreateRecaptchaTaskRequest(siteUrl, siteKey))),
        "token_params"
      }
    }).ToString();
    if (message.Trim().StartsWith("{"))
    {
      DeathByCaptcha.CreateTaskResponse createTaskResponse = JsonConvert.DeserializeObject<DeathByCaptcha.CreateTaskResponse>(message);
      this.TaskId = createTaskResponse.status != (int) byte.MaxValue ? (object) createTaskResponse.captcha : throw new Exception(createTaskResponse.error);
    }
    else
    {
      string[] strArray = message.Split('&');
      int num = int.Parse(strArray[0].Split('=')[1]);
      string str = strArray[1].Split('=')[1];
      if (num == (int) byte.MaxValue)
        throw new Exception(message);
      this.TaskId = (object) str;
    }
    this.Status = CaptchaService.CaptchaStatus.Processing;
    DateTime now = DateTime.Now;
    while (this.Status == CaptchaService.CaptchaStatus.Processing && (DateTime.Now - now).TotalSeconds < (double) this.Timeout)
    {
      Thread.Sleep(5000);
      HttpRequest httpRequest2 = new HttpRequest();
      httpRequest2.AddHeader(HttpHeader.Accept, "application/json");
      DeathByCaptcha.CreateTaskResponse createTaskResponse = JsonConvert.DeserializeObject<DeathByCaptcha.CreateTaskResponse>(httpRequest2.Get($"http://api.dbcapi.me/api/captcha/{this.TaskId}").ToString());
      if (createTaskResponse != null)
      {
        if (!createTaskResponse.is_correct)
          throw new Exception("No answer could be found");
        if (createTaskResponse.text != "")
        {
          this.Status = CaptchaService.CaptchaStatus.Completed;
          return createTaskResponse.text;
        }
      }
    }
    throw new TimeoutException();
  }

  public override string SolveCaptcha(Bitmap bitmap)
  {
    HttpRequest httpRequest1 = new HttpRequest();
    httpRequest1.AddHeader(HttpHeader.Accept, "application/json");
    string message = httpRequest1.Post("http://api.dbcapi.me/api/captcha", (HttpContent) new Extreme.Net.MultipartContent(Request.GenerateMultipartBoundary())
    {
      {
        (HttpContent) new StringContent(this.User),
        "username"
      },
      {
        (HttpContent) new StringContent(this.Pass),
        "password"
      },
      {
        (HttpContent) new StringContent("base64:" + this.GetBase64(bitmap, ImageFormat.Jpeg)),
        "captchafile"
      }
    }).ToString();
    if (message.Trim().StartsWith("{"))
    {
      DeathByCaptcha.CreateTaskResponse createTaskResponse = JsonConvert.DeserializeObject<DeathByCaptcha.CreateTaskResponse>(message);
      this.TaskId = createTaskResponse.status != (int) byte.MaxValue ? (object) createTaskResponse.captcha : throw new Exception(createTaskResponse.error);
    }
    else
    {
      string[] strArray = message.Split('&');
      int num = int.Parse(strArray[0].Split('=')[1]);
      string str = strArray[1].Split('=')[1];
      if (num == (int) byte.MaxValue)
        throw new Exception(message);
      this.TaskId = (object) str;
    }
    this.Status = CaptchaService.CaptchaStatus.Processing;
    DateTime now = DateTime.Now;
    while (this.Status == CaptchaService.CaptchaStatus.Processing && (DateTime.Now - now).TotalSeconds < (double) this.Timeout)
    {
      Thread.Sleep(5000);
      HttpRequest httpRequest2 = new HttpRequest();
      httpRequest2.AddHeader(HttpHeader.Accept, "application/json");
      DeathByCaptcha.CreateTaskResponse createTaskResponse = JsonConvert.DeserializeObject<DeathByCaptcha.CreateTaskResponse>(httpRequest2.Get($"http://api.dbcapi.me/api/captcha/{this.TaskId}").ToString());
      if (createTaskResponse != null)
      {
        if (!createTaskResponse.is_correct)
          throw new Exception("No answer could be found");
        if (createTaskResponse.text != "")
        {
          this.Status = CaptchaService.CaptchaStatus.Completed;
          return createTaskResponse.text;
        }
      }
    }
    throw new TimeoutException();
  }

  private class GetBalanceResponse
  {
    public int status;
    public double balance;
  }

  private class CreateRecaptchaTaskRequest
  {
    public string googlekey;
    public string pageurl;

    public CreateRecaptchaTaskRequest(string url, string key)
    {
      this.pageurl = url;
      this.googlekey = key;
    }
  }

  private class CreateCaptchaTaskRequest
  {
    public string clientKey;
    public DeathByCaptcha.CaptchaTask task;

    public CreateCaptchaTaskRequest(string apiKey, string base64)
    {
      this.clientKey = apiKey;
      this.task = new DeathByCaptcha.CaptchaTask(base64);
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
    public int captcha;
    public int status;
    public bool is_correct;
    public string text;
    public string error;
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
    public DeathByCaptcha.CaptchaSolution solution;
  }

  private class GetTaskResultRecaptchaResponse
  {
    public int errorId;
    public string errorCode;
    public string status;
    public DeathByCaptcha.RecaptchaSolution solution;
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
