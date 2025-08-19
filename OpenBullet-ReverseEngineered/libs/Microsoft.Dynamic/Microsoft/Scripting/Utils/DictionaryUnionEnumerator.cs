// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.DictionaryUnionEnumerator
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Utils;

public class DictionaryUnionEnumerator : CheckedDictionaryEnumerator
{
  private IList<IDictionaryEnumerator> _enums;
  private int _current;

  public DictionaryUnionEnumerator(IList<IDictionaryEnumerator> enums) => this._enums = enums;

  protected override object GetKey() => this._enums[this._current].Key;

  protected override object GetValue() => this._enums[this._current].Value;

  protected override bool DoMoveNext()
  {
    if (this._current == this._enums.Count)
      return false;
    if (this._enums[this._current].MoveNext())
      return true;
    ++this._current;
    return this.DoMoveNext();
  }

  protected override void DoReset()
  {
    for (int index = 0; index < this._enums.Count; ++index)
      this._enums[index].Reset();
    this._current = 0;
  }
}
