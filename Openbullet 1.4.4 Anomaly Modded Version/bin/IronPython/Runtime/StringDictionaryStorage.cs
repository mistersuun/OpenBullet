// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.StringDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal class StringDictionaryStorage : DictionaryStorage
{
  private Dictionary<string, object> _data;

  public StringDictionaryStorage()
  {
  }

  public StringDictionaryStorage(int count)
  {
    this._data = new Dictionary<string, object>(count, (IEqualityComparer<string>) StringComparer.Ordinal);
  }

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    this.Add(key, value);
  }

  public void Add(object key, object value)
  {
    lock (this)
      this.AddNoLock(key, value);
  }

  public override void AddNoLock(ref DictionaryStorage storage, object key, object value)
  {
    this.AddNoLock(key, value);
  }

  public void AddNoLock(object key, object value)
  {
    this.EnsureData();
    if (key is string key1)
      this._data[key1] = value;
    else
      this.GetObjectDictionary()[key] = value;
  }

  public override bool Contains(object key)
  {
    if (this._data == null)
      return false;
    lock (this)
    {
      if (key is string key1)
        return this._data.ContainsKey(key1);
      Dictionary<object, object> objectDictionary = this.TryGetObjectDictionary();
      return objectDictionary != null && objectDictionary.ContainsKey(key);
    }
  }

  public override bool Remove(ref DictionaryStorage storage, object key) => this.Remove(key);

  public bool Remove(object key)
  {
    if (this._data == null)
      return false;
    lock (this)
    {
      if (key is string key1)
        return this._data.Remove(key1);
      Dictionary<object, object> objectDictionary = this.TryGetObjectDictionary();
      return objectDictionary != null && objectDictionary.Remove(key);
    }
  }

  public override bool TryGetValue(object key, out object value)
  {
    if (this._data != null)
    {
      lock (this)
      {
        if (key is string key1)
          return this._data.TryGetValue(key1, out value);
        Dictionary<object, object> objectDictionary = this.TryGetObjectDictionary();
        if (objectDictionary != null)
          return objectDictionary.TryGetValue(key, out value);
      }
    }
    value = (object) null;
    return false;
  }

  public override int Count
  {
    get
    {
      if (this._data == null)
        return 0;
      lock (this)
      {
        if (this._data == null)
          return 0;
        int count = this._data.Count;
        Dictionary<object, object> objectDictionary = this.TryGetObjectDictionary();
        if (objectDictionary != null)
          count += objectDictionary.Count - 1;
        return count;
      }
    }
  }

  public override void Clear(ref DictionaryStorage storage)
  {
    this._data = (Dictionary<string, object>) null;
  }

  public override List<KeyValuePair<object, object>> GetItems()
  {
    List<KeyValuePair<object, object>> items = new List<KeyValuePair<object, object>>();
    if (this._data != null)
    {
      lock (this)
      {
        foreach (KeyValuePair<string, object> keyValuePair in this._data)
        {
          if (!string.IsNullOrEmpty(keyValuePair.Key))
            items.Add(new KeyValuePair<object, object>((object) keyValuePair.Key, keyValuePair.Value));
        }
        if (this.TryGetObjectDictionary() != null)
        {
          foreach (KeyValuePair<object, object> keyValuePair in this.GetObjectDictionary())
            items.Add(keyValuePair);
        }
      }
    }
    return items;
  }

  public override bool HasNonStringAttributes()
  {
    if (this._data != null)
    {
      lock (this)
      {
        if (this.TryGetObjectDictionary() != null)
          return true;
      }
    }
    return false;
  }

  private Dictionary<object, object> TryGetObjectDictionary()
  {
    object obj;
    return this._data != null && this._data.TryGetValue(string.Empty, out obj) ? (Dictionary<object, object>) obj : (Dictionary<object, object>) null;
  }

  private Dictionary<object, object> GetObjectDictionary()
  {
    lock (this)
    {
      this.EnsureData();
      object objectDictionary1;
      if (this._data.TryGetValue(string.Empty, out objectDictionary1))
        return (Dictionary<object, object>) objectDictionary1;
      Dictionary<object, object> objectDictionary2 = new Dictionary<object, object>();
      this._data[string.Empty] = (object) objectDictionary2;
      return objectDictionary2;
    }
  }

  private void EnsureData()
  {
    if (this._data != null)
      return;
    this._data = new Dictionary<string, object>();
  }
}
