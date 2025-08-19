// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.DefaultResourceLoader
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace AngleSharp.Io;

public class DefaultResourceLoader(IBrowsingContext context, Predicate<Request> filter = null) : 
  BaseLoader(context, filter),
  IResourceLoader,
  ILoader
{
  public virtual IDownload FetchAsync(ResourceRequest request)
  {
    Request request1 = new Request()
    {
      Address = request.Target,
      Content = Stream.Null,
      Method = HttpMethod.Get,
      Headers = (IDictionary<string, string>) new Dictionary<string, string>()
      {
        [HeaderNames.Referer] = (request?.Source.Owner.DocumentUri ?? string.Empty)
      }
    };
    string cookie = this.GetCookie(request.Target);
    if (cookie != null)
      request1.Headers[HeaderNames.Cookie] = cookie;
    return this.DownloadAsync(request1, (INode) request.Source);
  }
}
