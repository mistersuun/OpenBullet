// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.Captcha.CaptchaException
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet.Services.Captcha;

public class CaptchaException : Exception
{
  public readonly CaptchaError Error;
  private new readonly string _message;

  public override string Message
  {
    get => this.Error != CaptchaError.CustomMessage ? this.Error.ToString() : this._message;
  }

  public CaptchaException(string message)
  {
    if (string.IsNullOrEmpty(message))
    {
      this.Error = CaptchaError.Unknown;
    }
    else
    {
      this._message = message;
      this.Error = CaptchaError.CustomMessage;
    }
  }

  public CaptchaException(CaptchaError error) => this.Error = error;
}
