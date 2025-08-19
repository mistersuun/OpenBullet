// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.DictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal abstract class DictionaryStorage
{
  public abstract void Add(ref DictionaryStorage storage, object key, object value);

  public virtual void AddNoLock(ref DictionaryStorage storage, object key, object value)
  {
    this.Add(ref storage, key, value);
  }

  public abstract bool Remove(ref DictionaryStorage storage, object key);

  public virtual bool TryRemoveValue(ref DictionaryStorage storage, object key, out object value)
  {
    return this.TryGetValue(key, out value) && this.Remove(ref storage, key);
  }

  public abstract void Clear(ref DictionaryStorage storage);

  public virtual void CopyTo(ref DictionaryStorage into)
  {
    foreach (KeyValuePair<object, object> keyValuePair in this.GetItems())
      into.Add(ref into, keyValuePair.Key, keyValuePair.Value);
  }

  public abstract bool Contains(object key);

  public abstract bool TryGetValue(object key, out object value);

  public abstract int Count { get; }

  public virtual bool HasNonStringAttributes()
  {
    foreach (KeyValuePair<object, object> keyValuePair in this.GetItems())
    {
      if (!(keyValuePair.Key is string))
        return true;
    }
    return false;
  }

  public abstract List<KeyValuePair<object, object>> GetItems();

  public virtual IEnumerable<object> GetKeys()
  {
    foreach (KeyValuePair<object, object> keyValuePair in this.GetItems())
      yield return keyValuePair.Key;
  }

  public virtual DictionaryStorage Clone()
  {
    CommonDictionaryStorage dictionaryStorage = new CommonDictionaryStorage();
    foreach (KeyValuePair<object, object> keyValuePair in this.GetItems())
      dictionaryStorage.Add(keyValuePair.Key, keyValuePair.Value);
    return (DictionaryStorage) dictionaryStorage;
  }

  public virtual void EnsureCapacityNoLock(int size)
  {
  }

  public virtual IEnumerator<KeyValuePair<object, object>> GetEnumerator()
  {
    return (IEnumerator<KeyValuePair<object, object>>) this.GetItems().GetEnumerator();
  }

  public virtual bool TryGetPath(out object value)
  {
    return this.TryGetValue((object) "__path__", out value);
  }

  public virtual bool TryGetPackage(out object value)
  {
    return this.TryGetValue((object) "__package__", out value);
  }

  public virtual bool TryGetBuiltins(out object value)
  {
    return this.TryGetValue((object) "__builtins__", out value);
  }

  public virtual bool TryGetName(out object value)
  {
    return this.TryGetValue((object) "__name__", out value);
  }

  public virtual bool TryGetImport(out object value)
  {
    return this.TryGetValue((object) "__import__", out value);
  }
}
