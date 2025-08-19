// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.HttpContent
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.IO;

#nullable disable
namespace Leaf.xNet;

public abstract class HttpContent : IDisposable
{
  protected string MimeContentType = string.Empty;

  public string ContentType
  {
    get => this.MimeContentType;
    set => this.MimeContentType = value ?? string.Empty;
  }

  public abstract long CalculateContentLength();

  public abstract void WriteTo(Stream stream);

  public void Dispose() => this.Dispose(true);

  protected virtual void Dispose(bool disposing)
  {
  }
}
