// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Dom.IFile
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System;

#nullable disable
namespace AngleSharp.Io.Dom;

[DomName("File")]
public interface IFile : IBlob, IDisposable
{
  [DomName("name")]
  string Name { get; }

  [DomName("lastModified")]
  DateTime LastModified { get; }
}
