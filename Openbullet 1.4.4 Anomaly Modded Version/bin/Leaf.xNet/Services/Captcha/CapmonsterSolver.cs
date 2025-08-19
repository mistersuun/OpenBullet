// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.Captcha.CapmonsterSolver
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

#nullable disable
namespace Leaf.xNet.Services.Captcha;

public class CapmonsterSolver : RucaptchaSolver
{
  public CapmonsterSolver(string host = "127.0.0.3:80")
  {
    this.Host = host;
    this.IsApiKeyRequired = false;
  }
}
