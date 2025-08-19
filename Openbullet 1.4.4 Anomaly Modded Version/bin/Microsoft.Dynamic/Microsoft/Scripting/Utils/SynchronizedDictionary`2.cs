// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.SynchronizedDictionary`2
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Utils;

public class SynchronizedDictionary<TKey, TValue> : 
  IDictionary<TKey, TValue>,
  ICollection<KeyValuePair<TKey, TValue>>,
  IEnumerable<KeyValuePair<TKey, TValue>>,
  IEnumerable
{
  private Dictionary<TKey, TValue> _dictionary;

  public Dictionary<TKey, TValue> UnderlyingDictionary => this._dictionary;

  public SynchronizedDictionary()
    : this(new Dictionary<TKey, TValue>())
  {
  }

  public SynchronizedDictionary(Dictionary<TKey, TValue> dictionary)
  {
    this._dictionary = dictionary;
  }

  public void Add(TKey key, TValue value)
  {
    lock (this._dictionary)
      this._dictionary.Add(key, value);
  }

  public bool ContainsKey(TKey key)
  {
    lock (this._dictionary)
      return this._dictionary.ContainsKey(key);
  }

  public ICollection<TKey> Keys
  {
    get
    {
      lock (this._dictionary)
        return (ICollection<TKey>) this._dictionary.Keys;
    }
  }

  public bool Remove(TKey key)
  {
    lock (this._dictionary)
      return this._dictionary.Remove(key);
  }

  public bool TryGetValue(TKey key, out TValue value)
  {
    lock (this._dictionary)
      return this._dictionary.TryGetValue(key, out value);
  }

  public ICollection<TValue> Values
  {
    get
    {
      lock (this._dictionary)
        return (ICollection<TValue>) this._dictionary.Values;
    }
  }

  public TValue this[TKey key]
  {
    get
    {
      lock (this._dictionary)
        return this._dictionary[key];
    }
    set
    {
      lock (this._dictionary)
        this._dictionary[key] = value;
    }
  }

  private ICollection<KeyValuePair<TKey, TValue>> AsICollection()
  {
    return (ICollection<KeyValuePair<TKey, TValue>>) this._dictionary;
  }

  public void Add(KeyValuePair<TKey, TValue> item)
  {
    lock (this._dictionary)
      this.AsICollection().Add(item);
  }

  public void Clear()
  {
    lock (this._dictionary)
      this.AsICollection().Clear();
  }

  public bool Contains(KeyValuePair<TKey, TValue> item)
  {
    lock (this._dictionary)
      return this.AsICollection().Contains(item);
  }

  public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
  {
    lock (this._dictionary)
      this.AsICollection().CopyTo(array, arrayIndex);
  }

  public int Count
  {
    get
    {
      lock (this._dictionary)
        return this.AsICollection().Count;
    }
  }

  public bool IsReadOnly
  {
    get
    {
      lock (this._dictionary)
        return this.AsICollection().IsReadOnly;
    }
  }

  public bool Remove(KeyValuePair<TKey, TValue> item)
  {
    lock (this._dictionary)
      return this.AsICollection().Remove(item);
  }

  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
  {
    lock (this._dictionary)
      return (IEnumerator<KeyValuePair<TKey, TValue>>) this._dictionary.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    lock (this._dictionary)
      return (IEnumerator) this._dictionary.GetEnumerator();
  }
}
