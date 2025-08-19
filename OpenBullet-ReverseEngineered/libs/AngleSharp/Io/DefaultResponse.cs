// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.DefaultResponse
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

#nullable disable
namespace AngleSharp.Io;

public sealed class DefaultResponse : IResponse, IDisposable
{
  public DefaultResponse()
  {
    this.Headers = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    this.StatusCode = HttpStatusCode.Accepted;
  }

  public HttpStatusCode StatusCode { get; set; }

  public Url Address { get; set; }

  public IDictionary<string, string> Headers { get; set; }

  public Stream Content { get; set; }

  void IDisposable.Dispose()
  {
    this.Content?.Dispose();
    this.Headers.Clear();
  }
}
