// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.StringList
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class StringList : IStringList, IEnumerable<string>, IEnumerable
{
  private readonly IEnumerable<string> _list;

  internal StringList(IEnumerable<string> list) => this._list = list;

  public string this[int index] => this._list.GetItemByIndex<string>(index);

  public int Length => this._list.Count<string>();

  public bool Contains(string entry) => this._list.Contains<string>(entry);

  public IEnumerator<string> GetEnumerator() => this._list.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._list.GetEnumerator();
}
