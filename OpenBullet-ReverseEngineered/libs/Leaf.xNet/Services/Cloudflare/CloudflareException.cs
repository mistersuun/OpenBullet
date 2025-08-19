// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.Cloudflare.CloudflareException
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet.Services.Cloudflare;

[Serializable]
public class CloudflareException : Exception
{
  public CloudflareException(string message)
    : base(message)
  {
  }

  public CloudflareException(string message, Exception inner)
    : base(message, inner)
  {
  }

  public CloudflareException(int attempts)
    : this(attempts, $"Clearance failed after {attempts} attempt(s).")
  {
  }

  public CloudflareException(int attempts, string message)
    : base(message)
  {
    this.Attempts = attempts;
  }

  public CloudflareException(int attempts, string message, Exception inner)
    : base(message, inner)
  {
    this.Attempts = attempts;
  }

  public int Attempts { get; }
}
