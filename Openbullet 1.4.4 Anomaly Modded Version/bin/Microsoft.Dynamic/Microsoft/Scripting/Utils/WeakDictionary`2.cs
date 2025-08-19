// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.WeakDictionary`2
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Utils;

public class WeakDictionary<TKey, TValue> : 
  IDictionary<TKey, TValue>,
  ICollection<KeyValuePair<TKey, TValue>>,
  IEnumerable<KeyValuePair<TKey, TValue>>,
  IEnumerable
{
  private static readonly IEqualityComparer<object> comparer = (IEqualityComparer<object>) new WeakComparer<object>();
  private static readonly ConstructorInfo valueConstructor = typeof (TValue).GetConstructor(new Type[0]);
  private IDictionary<object, TValue> dict = (IDictionary<object, TValue>) new Dictionary<object, TValue>(WeakDictionary<TKey, TValue>.comparer);
  private int version;
  private int cleanupVersion;
  private int cleanupGC;

  public void Add(TKey key, TValue value)
  {
    this.CheckCleanup();
    this.dict.Add((object) new WeakObject((object) key), value);
  }

  public bool ContainsKey(TKey key) => this.dict.ContainsKey((object) key);

  public ICollection<TKey> Keys => throw new NotImplementedException();

  public bool Remove(TKey key) => this.dict.Remove((object) key);

  public bool TryGetValue(TKey key, out TValue value)
  {
    return this.dict.TryGetValue((object) key, out value);
  }

  public TValue GetOrCreateValue(TKey key)
  {
    TValue obj;
    if (!this.TryGetValue(key, out obj))
    {
      obj = !(WeakDictionary<TKey, TValue>.valueConstructor == (ConstructorInfo) null) ? (TValue) WeakDictionary<TKey, TValue>.valueConstructor.Invoke(new object[0]) : throw new InvalidOperationException(typeof (TValue).Name + " does not have a default constructor.");
      this.Add(key, obj);
    }
    return obj;
  }

  public ICollection<TValue> Values => throw new NotImplementedException();

  public TValue this[TKey key]
  {
    get => this.dict[(object) key];
    set
    {
      this.CheckCleanup();
      this.dict[(object) new WeakObject((object) key)] = value;
    }
  }

  private void CheckCleanup()
  {
    ++this.version;
    if ((long) (this.version - this.cleanupVersion) <= (long) (1234 + this.dict.Count / 2))
      return;
    int num1 = GC.CollectionCount(0);
    int num2 = num1 != this.cleanupGC ? 1 : 0;
    if (num2 != 0)
      this.cleanupGC = num1;
    if (num2 != 0)
    {
      this.Cleanup();
      this.cleanupVersion = this.version;
    }
    else
      this.cleanupVersion += 1234;
  }

  private void Cleanup()
  {
    int num1 = 0;
    int num2 = 0;
    foreach (WeakObject key in (IEnumerable<object>) this.dict.Keys)
    {
      if (key.Target != null)
        ++num1;
      else
        ++num2;
    }
    if (num2 <= num1 / 4)
      return;
    Dictionary<object, TValue> dictionary = new Dictionary<object, TValue>(num1 + num1 / 4, WeakDictionary<TKey, TValue>.comparer);
    foreach (WeakObject key in (IEnumerable<object>) this.dict.Keys)
    {
      object target = key.Target;
      if (target != null)
        dictionary[(object) key] = this.dict[(object) key];
      GC.KeepAlive(target);
    }
    this.dict = (IDictionary<object, TValue>) dictionary;
  }

  public void Add(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

  public void Clear() => throw new NotImplementedException();

  public bool Contains(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

  public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
  {
    throw new NotImplementedException();
  }

  public int Count => throw new NotImplementedException();

  public bool IsReadOnly => throw new NotImplementedException();

  public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
  {
    throw new NotImplementedException();
  }

  IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}
