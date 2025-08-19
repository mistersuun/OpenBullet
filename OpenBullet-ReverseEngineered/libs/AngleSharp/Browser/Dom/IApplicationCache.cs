// Decompiled with JetBrains decompiler
// Type: AngleSharp.Browser.Dom.IApplicationCache
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Browser.Dom;

[DomName("ApplicationCache")]
public interface IApplicationCache : IEventTarget
{
  [DomName("status")]
  CacheStatus Status { get; }

  [DomName("update")]
  void Update();

  [DomName("abort")]
  void Abort();

  [DomName("swapCache")]
  void Swap();

  [DomName("onchecking")]
  event DomEventHandler Checking;

  [DomName("onerror")]
  event DomEventHandler Error;

  [DomName("onnoupdate")]
  event DomEventHandler NoUpdate;

  [DomName("ondownloading")]
  event DomEventHandler Downloading;

  [DomName("onprogress")]
  event DomEventHandler Progress;

  [DomName("onupdateready")]
  event DomEventHandler UpdateReady;

  [DomName("oncached")]
  event DomEventHandler Cached;

  [DomName("onobsolete")]
  event DomEventHandler Obsolete;
}
