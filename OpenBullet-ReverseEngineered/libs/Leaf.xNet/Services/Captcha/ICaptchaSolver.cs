// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.Captcha.ICaptchaSolver
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.IO;
using System.Threading;

#nullable disable
namespace Leaf.xNet.Services.Captcha;

public interface ICaptchaSolver
{
  uint UploadRetries { get; set; }

  uint StatusRetries { get; set; }

  TimeSpan UploadDelayOnNoSlotAvailable { get; set; }

  TimeSpan StatusDelayOnNotReady { get; set; }

  TimeSpan BeforeStatusCheckingDelay { get; set; }

  string SolveImage(string imageUrl, CancellationToken cancelToken = default (CancellationToken));

  string SolveImage(byte[] imageBytes, CancellationToken cancelToken = default (CancellationToken));

  string SolveImage(Stream imageStream, CancellationToken cancelToken = default (CancellationToken));

  string SolveImageFromBase64(string imageBase64, CancellationToken cancelToken = default (CancellationToken));

  string SolveRecaptcha(string pageUrl, string siteKey, CancellationToken cancelToken = default (CancellationToken));
}
