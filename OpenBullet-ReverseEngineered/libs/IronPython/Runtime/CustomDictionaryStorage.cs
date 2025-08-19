// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.CustomDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

internal abstract class CustomDictionaryStorage : DictionaryStorage
{
  private readonly CommonDictionaryStorage _storage = new CommonDictionaryStorage();

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    this.Add(key, value);
  }

  public override void AddNoLock(ref DictionaryStorage storage, object key, object value)
  {
    if (key is string && this.TrySetExtraValue((string) key, value))
      return;
    this._storage.AddNoLock(ref storage, key, value);
  }

  public void Add(object key, object value)
  {
    if (key is string && this.TrySetExtraValue((string) key, value))
      return;
    this._storage.Add(key, value);
  }

  public override bool Contains(object key)
  {
    object obj;
    return key is string && this.TryGetExtraValue((string) key, out obj) ? obj != Uninitialized.Instance : this._storage.Contains(key);
  }

  public override bool Remove(ref DictionaryStorage storage, object key) => this.Remove(key);

  public bool Remove(object key)
  {
    return key is string ? this.TryRemoveExtraValue((string) key) ?? this._storage.Remove(key) : this._storage.Remove(key);
  }

  public override bool TryGetValue(object key, out object value)
  {
    return key is string && this.TryGetExtraValue((string) key, out value) ? value != Uninitialized.Instance : this._storage.TryGetValue(key, out value);
  }

  public override int Count => this.GetItems().Count;

  public override void Clear(ref DictionaryStorage storage)
  {
    this._storage.Clear(ref storage);
    foreach (KeyValuePair<string, object> extraItem in this.GetExtraItems())
      this.TryRemoveExtraValue(extraItem.Key);
  }

  public override List<KeyValuePair<object, object>> GetItems()
  {
    List<KeyValuePair<object, object>> items = this._storage.GetItems();
    foreach (KeyValuePair<string, object> extraItem in this.GetExtraItems())
      items.Add(new KeyValuePair<object, object>((object) extraItem.Key, extraItem.Value));
    return items;
  }

  protected abstract IEnumerable<KeyValuePair<string, object>> GetExtraItems();

  protected abstract bool TrySetExtraValue(string key, object value);

  protected abstract bool TryGetExtraValue(string key, out object value);

  protected abstract bool? TryRemoveExtraValue(string key);
}
