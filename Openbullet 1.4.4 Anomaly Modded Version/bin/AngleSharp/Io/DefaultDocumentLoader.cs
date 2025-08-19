// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.DefaultDocumentLoader
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Io;

public class DefaultDocumentLoader(IBrowsingContext context, Predicate<Request> filter = null) : 
  BaseLoader(context, filter),
  IDocumentLoader,
  ILoader
{
  public virtual IDownload FetchAsync(DocumentRequest request)
  {
    Request request1 = new Request()
    {
      Address = request.Target,
      Content = request.Body,
      Method = request.Method
    };
    foreach (KeyValuePair<string, string> header in request.Headers)
      request1.Headers[header.Key] = header.Value;
    return this.DownloadAsync(request1, request.Source);
  }
}
