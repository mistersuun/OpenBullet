// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.CustomStringDictionary
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Scripting.Runtime;

[DebuggerDisplay("Count = {Count}")]
public abstract class CustomStringDictionary : 
  IDictionary,
  ICollection,
  IEnumerable,
  IDictionary<object, object>,
  ICollection<KeyValuePair<object, object>>,
  IEnumerable<KeyValuePair<object, object>>
{
  private Dictionary<object, object> _data;
  private static readonly object _nullObject = new object();

  public abstract string[] GetExtraKeys();

  protected internal abstract bool TrySetExtraValue(string key, object value);

  protected internal abstract bool TryGetExtraValue(string key, out object value);

  private void InitializeData() => this._data = new Dictionary<object, object>();

  void IDictionary<object, object>.Add(object key, object value)
  {
    if (key is string key1)
    {
      lock (this)
      {
        if (this._data == null)
          this.InitializeData();
        if (this.TrySetExtraValue(key1, value))
          return;
        this._data.Add((object) key1, value);
      }
    }
    else
      this.AddObjectKey(key, value);
  }

  private void AddObjectKey(object key, object value)
  {
    if (this._data == null)
      this.InitializeData();
    this._data[key] = value;
  }

  bool IDictionary<object, object>.ContainsKey(object key)
  {
    lock (this)
      return this._data != null && this._data.TryGetValue(key, out object _);
  }

  public ICollection<object> Keys
  {
    get
    {
      List<object> keys = new List<object>();
      lock (this)
      {
        if (this._data != null)
          keys.AddRange((IEnumerable<object>) this._data.Keys);
      }
      foreach (string extraKey in this.GetExtraKeys())
      {
        object obj;
        if (this.TryGetExtraValue(extraKey, out obj) && obj != Uninitialized.Instance)
          keys.Add((object) extraKey);
      }
      return (ICollection<object>) keys;
    }
  }

  bool IDictionary<object, object>.Remove(object key)
  {
    if (!(key is string key1))
      return this.RemoveObjectKey(key);
    lock (this)
    {
      if (this.TrySetExtraValue(key1, (object) Uninitialized.Instance))
        return true;
      return this._data != null && this._data.Remove((object) key1);
    }
  }

  private bool RemoveObjectKey(object key) => this._data.Remove(key);

  public bool TryGetValue(object key, out object value)
  {
    if (!(key is string key1))
      return this.TryGetObjectValue(key, out value);
    lock (this)
    {
      if (this.TryGetExtraValue(key1, out value) && value != Uninitialized.Instance)
        return true;
      return this._data != null && this._data.TryGetValue((object) key1, out value);
    }
  }

  private bool TryGetObjectValue(object key, out object value)
  {
    if (this._data != null)
      return this._data.TryGetValue(key, out value);
    value = (object) null;
    return false;
  }

  ICollection<object> IDictionary<object, object>.Values
  {
    get
    {
      List<object> values = new List<object>();
      lock (this)
      {
        if (this._data != null)
          values.AddRange((IEnumerable<object>) this._data.Values);
      }
      foreach (string extraKey in this.GetExtraKeys())
      {
        object obj;
        if (this.TryGetExtraValue(extraKey, out obj) && obj != Uninitialized.Instance)
          values.Add(obj);
      }
      return (ICollection<object>) values;
    }
  }

  public object this[object key]
  {
    get
    {
      if (key is string str)
      {
        lock (this)
        {
          object obj;
          if (this.TryGetExtraValue(str, out obj) && !(obj is Uninitialized))
            return obj;
          return this._data != null ? this._data[(object) str] : throw new KeyNotFoundException(str);
        }
      }
      object obj1;
      if (this.TryGetObjectValue(key, out obj1))
        return obj1;
      throw new KeyNotFoundException(key.ToString());
    }
    set
    {
      if (key is string key1)
      {
        lock (this)
        {
          if (this.TrySetExtraValue(key1, value))
            return;
          if (this._data == null)
            this.InitializeData();
          this._data[(object) key1] = value;
        }
      }
      else
        this.AddObjectKey(key, value);
    }
  }

  public void Add(KeyValuePair<object, object> item) => throw new NotImplementedException();

  public void Clear()
  {
    lock (this)
    {
      foreach (string extraKey in this.GetExtraKeys())
        this.TrySetExtraValue(extraKey, (object) Uninitialized.Instance);
      this._data = (Dictionary<object, object>) null;
    }
  }

  public bool Contains(KeyValuePair<object, object> item) => throw new NotImplementedException();

  public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
  {
    ContractUtils.RequiresNotNull((object) array, nameof (array));
    ContractUtils.RequiresArrayRange<KeyValuePair<object, object>>((IList<KeyValuePair<object, object>>) array, arrayIndex, this.Count, nameof (arrayIndex), "Count");
    foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) this)
      array[arrayIndex++] = keyValuePair;
  }

  public int Count
  {
    get
    {
      Dictionary<object, object> data = this._data;
      int count = data != null ? __nonvirtual (data.Count) : 0;
      lock (this)
      {
        foreach (string extraKey in this.GetExtraKeys())
        {
          object obj;
          if (this.TryGetExtraValue(extraKey, out obj) && obj != Uninitialized.Instance)
            ++count;
        }
      }
      return count;
    }
  }

  public bool IsReadOnly => false;

  public bool Remove(KeyValuePair<object, object> item) => throw new NotImplementedException();

  public bool Remove(object key)
  {
    if (!(key is string key1))
      return this.RemoveObjectKey(key);
    if (this.TrySetExtraValue(key1, (object) Uninitialized.Instance))
      return true;
    lock (this)
      return this._data != null && this._data.Remove((object) key1);
  }

  IEnumerator<KeyValuePair<object, object>> IEnumerable<KeyValuePair<object, object>>.GetEnumerator()
  {
    if (this._data != null)
    {
      foreach (KeyValuePair<object, object> keyValuePair in this._data)
        yield return keyValuePair;
    }
    string[] strArray = this.GetExtraKeys();
    for (int index = 0; index < strArray.Length; ++index)
    {
      string key = strArray[index];
      object obj;
      if (this.TryGetExtraValue(key, out obj) && obj != Uninitialized.Instance)
        yield return new KeyValuePair<object, object>((object) key, obj);
    }
    strArray = (string[]) null;
  }

  public IEnumerator GetEnumerator()
  {
    List<object> objectList = new List<object>((IEnumerable<object>) this.Keys);
    int index = 0;
    while (index < objectList.Count && objectList[index] == (objectList[index] = CustomStringDictionary.ObjToNull(objectList[index])))
      ++index;
    return (IEnumerator) objectList.GetEnumerator();
  }

  void IDictionary.Add(object key, object value)
  {
    ((IDictionary<object, object>) this).Add(key, value);
  }

  public bool Contains(object key) => this.TryGetValue(key, out object _);

  IDictionaryEnumerator IDictionary.GetEnumerator()
  {
    List<IDictionaryEnumerator> enums = new List<IDictionaryEnumerator>();
    enums.Add((IDictionaryEnumerator) new ExtraKeyEnumerator(this));
    if (this._data != null)
      enums.Add(((IDictionary) this._data).GetEnumerator());
    return (IDictionaryEnumerator) new DictionaryUnionEnumerator((IList<IDictionaryEnumerator>) enums);
  }

  public bool IsFixedSize => false;

  ICollection IDictionary.Keys => (ICollection) new List<object>((IEnumerable<object>) this.Keys);

  void IDictionary.Remove(object key) => this.Remove(key);

  ICollection IDictionary.Values
  {
    get
    {
      return (ICollection) new List<object>((IEnumerable<object>) ((IDictionary<object, object>) this).Values);
    }
  }

  public int GetValueHashCode() => throw Error.DictionaryNotHashable();

  public virtual bool ValueEquals(object other)
  {
    if (this == other)
      return true;
    IDictionary<object, object> dictionary1 = other as IDictionary<object, object>;
    IDictionary<object, object> dictionary2 = (IDictionary<object, object>) this;
    int? count1 = dictionary1?.Count;
    int count2 = dictionary2.Count;
    if (!(count1.GetValueOrDefault() == count2 & count1.HasValue))
      return false;
    foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) dictionary2)
    {
      object obj;
      if (!dictionary1.TryGetValue(keyValuePair.Key, out obj))
        return false;
      if (obj != null)
      {
        if (!obj.Equals(keyValuePair.Value))
          return false;
      }
      else if (keyValuePair.Value != null && !keyValuePair.Value.Equals(obj))
        return false;
    }
    return true;
  }

  public void CopyTo(Array array, int index) => throw Error.MethodOrOperatorNotImplemented();

  public bool IsSynchronized => true;

  public object SyncRoot => (object) this;

  public static object NullToObj(object o) => o == null ? CustomStringDictionary._nullObject : o;

  public static object ObjToNull(object o)
  {
    return o == CustomStringDictionary._nullObject ? (object) null : o;
  }

  public static bool IsNullObject(object o) => o == CustomStringDictionary._nullObject;
}
