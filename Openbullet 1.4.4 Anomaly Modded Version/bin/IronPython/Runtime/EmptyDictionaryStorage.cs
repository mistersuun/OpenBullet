// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.EmptyDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal class EmptyDictionaryStorage : DictionaryStorage
{
  public static EmptyDictionaryStorage Instance = new EmptyDictionaryStorage();

  private EmptyDictionaryStorage()
  {
  }

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    lock (this)
    {
      if (storage == this)
      {
        CommonDictionaryStorage dictionaryStorage = new CommonDictionaryStorage();
        dictionaryStorage.AddNoLock(key, value);
        storage = (DictionaryStorage) dictionaryStorage;
        return;
      }
    }
    storage.Add(ref storage, key, value);
  }

  public override bool Remove(ref DictionaryStorage storage, object key) => false;

  public override void Clear(ref DictionaryStorage storage)
  {
  }

  public override bool Contains(object key)
  {
    if (PythonContext.IsHashable(key))
      return false;
    throw PythonOps.TypeErrorForUnhashableObject(key);
  }

  public override bool TryGetValue(object key, out object value)
  {
    value = (object) null;
    return false;
  }

  public override int Count => 0;

  public override List<KeyValuePair<object, object>> GetItems()
  {
    return new List<KeyValuePair<object, object>>();
  }

  public override DictionaryStorage Clone() => (DictionaryStorage) this;

  public override bool HasNonStringAttributes() => false;
}
