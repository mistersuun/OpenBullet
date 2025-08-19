// Decompiled with JetBrains decompiler
// Type: RuriLib.CaptchaServices.CaptchaService
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

#nullable disable
namespace RuriLib.CaptchaServices;

public abstract class CaptchaService
{
  public string ApiKey { get; set; }

  public string User { get; set; }

  public string Pass { get; set; }

  public string Domain { get; set; }

  public int Port { get; set; }

  public int Timeout { get; set; }

  public object TaskId { get; set; }

  public CaptchaService.CaptchaStatus Status { get; set; }

  public CaptchaService(string apiKey, int timeout)
  {
    this.ApiKey = apiKey;
    this.Timeout = timeout;
    this.Status = CaptchaService.CaptchaStatus.Idle;
  }

  public CaptchaService(string user, string pass, int timeout)
  {
    this.User = user;
    this.Pass = pass;
    this.Timeout = timeout;
    this.Status = CaptchaService.CaptchaStatus.Idle;
  }

  public CaptchaService(string apiKey, string domain, int port, int timeout)
  {
    this.ApiKey = apiKey;
    this.Domain = domain;
    this.Port = port;
    this.Timeout = timeout;
  }

  public virtual double GetBalance() => 0.0;

  public virtual string SolveRecaptcha(string siteKey, string siteUrl) => string.Empty;

  public virtual string SolveCaptcha(Bitmap image) => string.Empty;

  protected string GetBase64(Bitmap image, ImageFormat format)
  {
    MemoryStream memoryStream = new MemoryStream();
    image.Save((Stream) memoryStream, format);
    return Convert.ToBase64String(memoryStream.ToArray());
  }

  protected Stream GetStream(Bitmap image, ImageFormat format)
  {
    Stream stream = (Stream) new MemoryStream();
    image.Save(stream, format);
    return stream;
  }

  protected byte[] GetBytes(Bitmap image)
  {
    return (byte[]) new ImageConverter().ConvertTo((object) image, typeof (byte[]));
  }

  protected string PostSync(HttpClient client, string url, HttpContent content)
  {
    return Task.Run<string>((Func<Task<string>>) (() => Task.Run<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (() => client.PostAsync(url, content))).Result.Content.ReadAsStringAsync())).Result;
  }

  protected byte[] PostSync2(HttpClient client, string url, HttpContent content)
  {
    return Task.Run<byte[]>((Func<Task<byte[]>>) (() => Task.Run<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (() => client.PostAsync(url, content))).Result.Content.ReadAsByteArrayAsync())).Result;
  }

  protected Stream PostSync3(HttpClient client, string url, HttpContent content)
  {
    return Task.Run<Stream>((Func<Task<Stream>>) (() => Task.Run<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (() => client.PostAsync(url, content))).Result.Content.ReadAsStreamAsync())).Result;
  }

  protected string EscapeLongString(string longString)
  {
    string str = "";
    int val1 = 200;
    for (int startIndex = 0; startIndex < longString.Length; startIndex += val1)
      str += Uri.EscapeDataString(longString.Substring(startIndex, Math.Min(val1, longString.Length - startIndex)));
    return str;
  }

  public enum CaptchaStatus
  {
    Idle,
    Processing,
    Completed,
  }
}
