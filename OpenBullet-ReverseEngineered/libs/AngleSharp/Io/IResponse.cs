// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.IResponse
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

#nullable disable
namespace AngleSharp.Io;

public interface IResponse : IDisposable
{
  HttpStatusCode StatusCode { get; }

  Url Address { get; }

  IDictionary<string, string> Headers { get; }

  Stream Content { get; }
}
