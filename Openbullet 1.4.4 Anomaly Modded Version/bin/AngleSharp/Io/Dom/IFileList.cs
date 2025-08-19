// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Dom.IFileList
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Io.Dom;

[DomName("FileList")]
public interface IFileList : IEnumerable<IFile>, IEnumerable
{
  [DomName("item")]
  [DomAccessor(Accessors.Getter)]
  IFile this[int index] { get; }

  [DomName("length")]
  int Length { get; }

  void Add(IFile file);

  bool Remove(IFile file);

  void Clear();
}
