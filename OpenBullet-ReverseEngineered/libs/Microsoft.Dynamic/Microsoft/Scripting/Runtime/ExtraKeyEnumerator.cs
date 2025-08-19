// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ExtraKeyEnumerator
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;

#nullable disable
namespace Microsoft.Scripting.Runtime;

internal class ExtraKeyEnumerator : CheckedDictionaryEnumerator
{
  private CustomStringDictionary _idDict;
  private int _curIndex = -1;

  public ExtraKeyEnumerator(CustomStringDictionary idDict) => this._idDict = idDict;

  protected override object GetKey() => (object) this._idDict.GetExtraKeys()[this._curIndex];

  protected override object GetValue()
  {
    object obj;
    this._idDict.TryGetExtraValue(this._idDict.GetExtraKeys()[this._curIndex], out obj);
    return obj;
  }

  protected override bool DoMoveNext()
  {
    if (this._idDict.GetExtraKeys().Length == 0)
      return false;
    while (this._curIndex < this._idDict.GetExtraKeys().Length - 1)
    {
      ++this._curIndex;
      object obj;
      if (this._idDict.TryGetExtraValue(this._idDict.GetExtraKeys()[this._curIndex], out obj) && obj != Uninitialized.Instance)
        return true;
    }
    return false;
  }

  protected override void DoReset() => this._curIndex = -1;
}
