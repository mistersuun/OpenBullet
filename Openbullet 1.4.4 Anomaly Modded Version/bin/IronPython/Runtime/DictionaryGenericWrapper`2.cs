// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.DictionaryGenericWrapper`2
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

public class DictionaryGenericWrapper<K, V> : 
  IDictionary<K, V>,
  ICollection<KeyValuePair<K, V>>,
  IEnumerable<KeyValuePair<K, V>>,
  IEnumerable
{
  private IDictionary<object, object> self;

  public DictionaryGenericWrapper(IDictionary<object, object> self) => this.self = self;

  public void Add(K key, V value) => this.self.Add((object) key, (object) value);

  public bool ContainsKey(K key) => this.self.ContainsKey((object) key);

  public ICollection<K> Keys
  {
    get
    {
      List<K> keys = new List<K>();
      foreach (object key in (IEnumerable<object>) this.self.Keys)
        keys.Add((K) key);
      return (ICollection<K>) keys;
    }
  }

  public bool Remove(K key) => this.self.Remove((object) key);

  public bool TryGetValue(K key, out V value)
  {
    object obj;
    if (this.self.TryGetValue((object) key, out obj))
    {
      value = (V) obj;
      return true;
    }
    value = default (V);
    return false;
  }

  public ICollection<V> Values
  {
    get
    {
      List<V> values = new List<V>();
      foreach (object obj in (IEnumerable<object>) this.self.Values)
        values.Add((V) obj);
      return (ICollection<V>) values;
    }
  }

  public V this[K key]
  {
    get => (V) this.self[(object) key];
    set => this.self[(object) key] = (object) value;
  }

  public void Add(KeyValuePair<K, V> item)
  {
    this.self.Add(new KeyValuePair<object, object>((object) item.Key, (object) item.Value));
  }

  public void Clear() => this.self.Clear();

  public bool Contains(KeyValuePair<K, V> item)
  {
    return this.self.Contains(new KeyValuePair<object, object>((object) item.Key, (object) item.Value));
  }

  public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
  {
    foreach (KeyValuePair<K, V> keyValuePair in this)
      array[arrayIndex++] = keyValuePair;
  }

  public int Count => this.self.Count;

  public bool IsReadOnly => this.self.IsReadOnly;

  public bool Remove(KeyValuePair<K, V> item)
  {
    return this.self.Remove((object) new KeyValuePair<object, object>((object) item.Key, (object) item.Value));
  }

  public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
  {
    foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) this.self)
      yield return new KeyValuePair<K, V>((K) keyValuePair.Key, (V) keyValuePair.Value);
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.self.GetEnumerator();
}
