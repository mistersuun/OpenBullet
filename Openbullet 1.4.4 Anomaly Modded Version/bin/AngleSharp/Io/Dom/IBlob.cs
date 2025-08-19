// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Dom.IBlob
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System;
using System.IO;

#nullable disable
namespace AngleSharp.Io.Dom;

[DomName("Blob")]
public interface IBlob : IDisposable
{
  [DomName("size")]
  int Length { get; }

  [DomName("type")]
  string Type { get; }

  [DomName("isClosed")]
  bool IsClosed { get; }

  Stream Body { get; }

  [DomName("slice")]
  IBlob Slice(int start = 0, int end = 2147483647 /*0x7FFFFFFF*/, string contentType = null);

  [DomName("close")]
  void Close();
}
