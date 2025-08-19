// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.StyleSheetList
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class StyleSheetList : IStyleSheetList, IEnumerable<IStyleSheet>, IEnumerable
{
  private readonly IEnumerable<IStyleSheet> _sheets;

  internal StyleSheetList(IEnumerable<IStyleSheet> sheets) => this._sheets = sheets;

  public IStyleSheet this[int index]
  {
    get => this._sheets.Skip<IStyleSheet>(index).FirstOrDefault<IStyleSheet>();
  }

  public int Length => this._sheets.Count<IStyleSheet>();

  public IEnumerator<IStyleSheet> GetEnumerator() => this._sheets.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
