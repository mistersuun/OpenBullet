// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.HybridReferenceDictionary`2
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal class HybridReferenceDictionary<TKey, TValue> where TKey : class
{
  private KeyValuePair<TKey, TValue>[] _keysAndValues;
  private Dictionary<TKey, TValue> _dict;
  private int _count;
  private const int _arraySize = 10;

  public HybridReferenceDictionary()
  {
  }

  public HybridReferenceDictionary(int initialCapicity)
  {
    if (initialCapicity > 10)
      this._dict = new Dictionary<TKey, TValue>(initialCapicity);
    else
      this._keysAndValues = new KeyValuePair<TKey, TValue>[initialCapicity];
  }

  public bool TryGetValue(TKey key, out TValue value)
  {
    if (this._dict != null)
      return this._dict.TryGetValue(key, out value);
    if (this._keysAndValues != null)
    {
      for (int index = 0; index < this._keysAndValues.Length; ++index)
      {
        if ((object) this._keysAndValues[index].Key == (object) key)
        {
          value = this._keysAndValues[index].Value;
          return true;
        }
      }
    }
    value = default (TValue);
    return false;
  }

  public bool Remove(TKey key)
  {
    if (this._dict != null)
      return this._dict.Remove(key);
    if (this._keysAndValues != null)
    {
      for (int index = 0; index < this._keysAndValues.Length; ++index)
      {
        if ((object) this._keysAndValues[index].Key == (object) key)
        {
          this._keysAndValues[index] = new KeyValuePair<TKey, TValue>();
          --this._count;
          return true;
        }
      }
    }
    return false;
  }

  public bool ContainsKey(TKey key)
  {
    if (this._dict != null)
      return this._dict.ContainsKey(key);
    if (this._keysAndValues != null)
    {
      for (int index = 0; index < this._keysAndValues.Length; ++index)
      {
        if ((object) this._keysAndValues[index].Key == (object) key)
          return true;
      }
    }
    return false;
  }

  public int Count => this._dict != null ? this._dict.Count : this._count;

  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
  {
    return this._dict != null ? (IEnumerator<KeyValuePair<TKey, TValue>>) this._dict.GetEnumerator() : this.GetEnumeratorWorker();
  }

  private IEnumerator<KeyValuePair<TKey, TValue>> GetEnumeratorWorker()
  {
    if (this._keysAndValues != null)
    {
      for (int i = 0; i < this._keysAndValues.Length; ++i)
      {
        if ((object) this._keysAndValues[i].Key != null)
          yield return this._keysAndValues[i];
      }
    }
  }

  public TValue this[TKey key]
  {
    get
    {
      TValue obj;
      if (this.TryGetValue(key, out obj))
        return obj;
      throw new KeyNotFoundException();
    }
    set
    {
      if (this._dict != null)
      {
        this._dict[key] = value;
      }
      else
      {
        int index1;
        if (this._keysAndValues != null)
        {
          index1 = -1;
          for (int index2 = 0; index2 < this._keysAndValues.Length; ++index2)
          {
            if ((object) this._keysAndValues[index2].Key == (object) key)
            {
              this._keysAndValues[index2] = new KeyValuePair<TKey, TValue>(key, value);
              return;
            }
            if ((object) this._keysAndValues[index2].Key == null)
              index1 = index2;
          }
        }
        else
        {
          this._keysAndValues = new KeyValuePair<TKey, TValue>[10];
          index1 = 0;
        }
        if (index1 != -1)
        {
          ++this._count;
          this._keysAndValues[index1] = new KeyValuePair<TKey, TValue>(key, value);
        }
        else
        {
          this._dict = new Dictionary<TKey, TValue>();
          for (int index3 = 0; index3 < this._keysAndValues.Length; ++index3)
            this._dict[this._keysAndValues[index3].Key] = this._keysAndValues[index3].Value;
          this._keysAndValues = (KeyValuePair<TKey, TValue>[]) null;
          this._dict[key] = value;
        }
      }
    }
  }
}
