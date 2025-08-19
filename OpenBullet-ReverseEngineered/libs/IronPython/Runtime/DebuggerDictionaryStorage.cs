// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.DebuggerDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal class DebuggerDictionaryStorage : DictionaryStorage
{
  private IDictionary<object, object> _data;
  private readonly CommonDictionaryStorage _hidden;

  public DebuggerDictionaryStorage(IDictionary<object, object> data)
  {
    this._hidden = new CommonDictionaryStorage();
    foreach (object key1 in (IEnumerable<object>) data.Keys)
    {
      if (key1 is string key2 && key2.Length > 0 && key2[0] == '$')
        this._hidden.Add((object) key2, (object) null);
    }
    this._data = data;
  }

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    this.AddNoLock(ref storage, key, value);
  }

  public override void AddNoLock(ref DictionaryStorage storage, object key, object value)
  {
    this._hidden.Remove(key);
    this._data[key] = value;
  }

  public override bool Contains(object key)
  {
    return !this._hidden.Contains(key) && this._data.ContainsKey(key);
  }

  public override bool Remove(ref DictionaryStorage storage, object key)
  {
    return !this._hidden.Contains(key) && this._data.Remove(key);
  }

  public override bool TryGetValue(object key, out object value)
  {
    if (!this._hidden.Contains(key))
      return this._data.TryGetValue(key, out value);
    value = (object) null;
    return false;
  }

  public override int Count => this._data.Count - this._hidden.Count;

  public override void Clear(ref DictionaryStorage storage)
  {
    this._data = (IDictionary<object, object>) new Dictionary<object, object>();
    this._hidden.Clear();
  }

  public override List<KeyValuePair<object, object>> GetItems()
  {
    List<KeyValuePair<object, object>> items = new List<KeyValuePair<object, object>>(this.Count);
    foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) this._data)
    {
      if (!this._hidden.Contains(keyValuePair.Key))
        items.Add(keyValuePair);
    }
    return items;
  }

  public override bool HasNonStringAttributes() => true;
}
