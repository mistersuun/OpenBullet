// Decompiled with JetBrains decompiler
// Type: Extreme.Net.HttpContent
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System.IO;

#nullable disable
namespace Extreme.Net;

public abstract class HttpContent
{
  protected string _contentType = string.Empty;

  public string ContentType
  {
    get => this._contentType;
    set => this._contentType = value ?? string.Empty;
  }

  public abstract long CalculateContentLength();

  public abstract void WriteTo(Stream stream);

  public void Dispose() => this.Dispose(true);

  protected virtual void Dispose(bool disposing)
  {
  }
}
