// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.IDownload
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;

#nullable disable
namespace AngleSharp.Io;

public interface IDownload : ICancellable<IResponse>, ICancellable
{
  Url Target { get; }

  object Source { get; }
}
