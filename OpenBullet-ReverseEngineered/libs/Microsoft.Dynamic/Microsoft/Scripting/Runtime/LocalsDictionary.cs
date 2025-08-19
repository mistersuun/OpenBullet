// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.LocalsDictionary
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class LocalsDictionary : CustomStringDictionary
{
  private readonly IRuntimeVariables _locals;
  private readonly string[] _symbols;
  private Dictionary<string, int> _boxes;

  public LocalsDictionary(IRuntimeVariables locals, string[] symbols)
  {
    this._locals = locals;
    this._symbols = symbols;
  }

  private void EnsureBoxes()
  {
    if (this._boxes != null)
      return;
    int length = this._symbols.Length;
    Dictionary<string, int> dictionary = new Dictionary<string, int>(length);
    for (int index = 0; index < length; ++index)
      dictionary[this._symbols[index]] = index;
    this._boxes = dictionary;
  }

  public override string[] GetExtraKeys() => this._symbols;

  protected internal override bool TrySetExtraValue(string key, object value)
  {
    this.EnsureBoxes();
    int index;
    if (!this._boxes.TryGetValue(key, out index))
      return false;
    this._locals[index] = value;
    return true;
  }

  protected internal override bool TryGetExtraValue(string key, out object value)
  {
    this.EnsureBoxes();
    int index;
    if (this._boxes.TryGetValue(key, out index))
    {
      value = this._locals[index];
      return true;
    }
    value = (object) null;
    return false;
  }
}
