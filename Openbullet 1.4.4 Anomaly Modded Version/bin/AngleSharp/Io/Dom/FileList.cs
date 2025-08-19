// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Dom.FileList
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Io.Dom;

internal sealed class FileList : IFileList, IEnumerable<IFile>, IEnumerable
{
  private readonly List<IFile> _entries;

  internal FileList() => this._entries = new List<IFile>();

  public IFile this[int index] => this._entries[index];

  public int Length => this._entries.Count;

  public void Add(IFile item) => this._entries.Add(item);

  public void Clear() => this._entries.Clear();

  public bool Remove(IFile item) => this._entries.Remove(item);

  public IEnumerator<IFile> GetEnumerator() => (IEnumerator<IFile>) this._entries.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
