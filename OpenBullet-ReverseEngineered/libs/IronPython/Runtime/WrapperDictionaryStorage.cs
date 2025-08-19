// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.WrapperDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal class WrapperDictionaryStorage : DictionaryStorage
{
  private TopNamespaceTracker _data;

  public WrapperDictionaryStorage(TopNamespaceTracker data) => this._data = data;

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    throw WrapperDictionaryStorage.CannotModifyNamespaceDict();
  }

  private static InvalidOperationException CannotModifyNamespaceDict()
  {
    return new InvalidOperationException("cannot modify namespace dictionary");
  }

  public override bool Contains(object key) => key is string name && this._data.ContainsKey(name);

  public override bool Remove(ref DictionaryStorage storage, object key)
  {
    throw WrapperDictionaryStorage.CannotModifyNamespaceDict();
  }

  public override bool TryGetValue(object key, out object value)
  {
    if (key is string name)
      return this._data.TryGetValue(name, out value);
    value = (object) null;
    return false;
  }

  public override int Count => this._data.Count;

  public override void Clear(ref DictionaryStorage storage)
  {
    throw WrapperDictionaryStorage.CannotModifyNamespaceDict();
  }

  public override List<KeyValuePair<object, object>> GetItems()
  {
    List<KeyValuePair<object, object>> items = new List<KeyValuePair<object, object>>(this._data.Count);
    foreach (KeyValuePair<string, object> keyValuePair in (NamespaceTracker) this._data)
      items.Add(new KeyValuePair<object, object>((object) keyValuePair.Key, keyValuePair.Value));
    return items;
  }
}
