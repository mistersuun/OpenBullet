// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.Captcha.BaseCaptchaSolver
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.IO;
using System.Threading;

#nullable disable
namespace Leaf.xNet.Services.Captcha;

public abstract class BaseCaptchaSolver : ICaptchaSolver, IDisposable
{
  public const string NameOfString = "string";
  protected readonly AdvancedWebClient Http = new AdvancedWebClient();

  public string ApiKey { get; set; }

  public bool IsApiKeyRequired { get; protected set; } = true;

  public virtual bool IsApiKeyValid => !string.IsNullOrEmpty(this.ApiKey);

  public uint UploadRetries { get; set; } = 40;

  public uint StatusRetries { get; set; } = 80 /*0x50*/;

  public TimeSpan UploadDelayOnNoSlotAvailable { get; set; } = TimeSpan.FromSeconds(5.0);

  public TimeSpan StatusDelayOnNotReady { get; set; } = TimeSpan.FromSeconds(3.0);

  public TimeSpan BeforeStatusCheckingDelay { get; set; } = TimeSpan.FromSeconds(3.0);

  public virtual string SolveImage(string imageUrl, CancellationToken cancelToken = default (CancellationToken))
  {
    throw this.NotImplemented(nameof (SolveImage), "string");
  }

  public virtual string SolveImage(byte[] imageBytes, CancellationToken cancelToken = default (CancellationToken))
  {
    throw this.NotImplemented(nameof (SolveImage), "byte[]");
  }

  public virtual string SolveImage(Stream imageStream, CancellationToken cancelToken = default (CancellationToken))
  {
    throw this.NotImplemented(nameof (SolveImage), "Stream");
  }

  public string SolveImageFromBase64(string imageBase64, CancellationToken cancelToken = default (CancellationToken))
  {
    throw this.NotImplemented(nameof (SolveImageFromBase64), "string");
  }

  public virtual string SolveRecaptcha(
    string pageUrl,
    string siteKey,
    CancellationToken cancelToken = default (CancellationToken))
  {
    throw this.NotImplemented(nameof (SolveRecaptcha), "string, string");
  }

  protected void ThrowIfApiKeyRequiredAndInvalid()
  {
    if (this.IsApiKeyRequired && !this.IsApiKeyValid)
      throw new CaptchaException(CaptchaError.InvalidApiKey);
  }

  protected void Delay(TimeSpan delay, CancellationToken cancelToken)
  {
    if (cancelToken != CancellationToken.None)
    {
      cancelToken.WaitHandle.WaitOne(this.UploadDelayOnNoSlotAvailable);
      cancelToken.ThrowIfCancellationRequested();
    }
    else
      Thread.Sleep(this.UploadDelayOnNoSlotAvailable);
  }

  private NotImplementedException NotImplemented(string method, string parameterType)
  {
    return new NotImplementedException($"Method \"{method}\"({parameterType}) of {this.GetType().Name} isn't implemented");
  }

  public virtual void Dispose() => this.Http?.Dispose();
}
