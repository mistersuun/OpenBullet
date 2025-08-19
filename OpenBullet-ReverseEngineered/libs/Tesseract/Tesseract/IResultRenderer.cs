// Decompiled with JetBrains decompiler
// Type: Tesseract.IResultRenderer
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;

#nullable disable
namespace Tesseract;

public interface IResultRenderer : IDisposable
{
  IDisposable BeginDocument(string title);

  bool AddPage(Page page);

  int PageNumber { get; }
}
