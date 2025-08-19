// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.Captcha.CaptchaProxy
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet.Services.Captcha;

public struct CaptchaProxy
{
  public readonly CaptchaProxyType Type;
  public readonly string Address;

  public bool IsValid
  {
    get
    {
      return !object.Equals((object) this, (object) new CaptchaProxy()) && !string.IsNullOrEmpty(this.Address);
    }
  }

  public CaptchaProxy(CaptchaProxyType type, string address)
  {
    CaptchaProxy.Validate(type, address);
    this.Type = type;
    this.Address = address;
  }

  public CaptchaProxy(string type, string address)
  {
    CaptchaProxyType result;
    if (!Enum.TryParse<CaptchaProxyType>(type.Trim().ToUpper(), out result))
      throw new ArgumentException("Proxy type is invalid. Available: HTTP, HTTPS, SOCKS4, SOCKS5", nameof (address));
    CaptchaProxy.Validate(result, address);
    this.Type = result;
    this.Address = address;
  }

  private static void Validate(CaptchaProxyType type, string address)
  {
    int num = !string.IsNullOrEmpty(address) ? address.IndexOf(':') : throw new ArgumentException("CaptchaProxy should contain address", nameof (address));
    if (num == -1 || address.Length - 1 - num < 2)
      throw new ArgumentException("address should contain port", nameof (address));
  }
}
