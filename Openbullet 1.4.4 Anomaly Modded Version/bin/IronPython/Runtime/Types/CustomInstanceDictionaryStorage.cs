// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.CustomInstanceDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Types;

[Serializable]
internal sealed class CustomInstanceDictionaryStorage : StringDictionaryStorage
{
  private readonly int _keyVersion;
  private readonly string[] _extraKeys;
  private readonly object[] _values;
  private static int _namesVersion;

  internal static int AllocateVersion()
  {
    return Interlocked.Increment(ref CustomInstanceDictionaryStorage._namesVersion);
  }

  public CustomInstanceDictionaryStorage(string[] extraKeys, int keyVersion)
  {
    this._extraKeys = extraKeys;
    this._keyVersion = keyVersion;
    this._values = new object[extraKeys.Length];
    for (int index = 0; index < this._values.Length; ++index)
      this._values[index] = (object) Uninitialized.Instance;
  }

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    int key1 = this.FindKey(key);
    if (key1 != -1)
      this._values[key1] = value;
    else
      base.Add(ref storage, key, value);
  }

  public override void AddNoLock(ref DictionaryStorage storage, object key, object value)
  {
    int key1 = this.FindKey(key);
    if (key1 != -1)
      this._values[key1] = value;
    else
      base.AddNoLock(ref storage, key, value);
  }

  public override bool Contains(object key)
  {
    int key1 = this.FindKey(key);
    return key1 != -1 ? this._values[key1] != Uninitialized.Instance : base.Contains(key);
  }

  public override bool Remove(ref DictionaryStorage storage, object key)
  {
    int key1 = this.FindKey(key);
    if (key1 == -1)
      return base.Remove(ref storage, key);
    return Interlocked.Exchange<object>(ref this._values[key1], (object) Uninitialized.Instance) != Uninitialized.Instance;
  }

  public override bool TryGetValue(object key, out object value)
  {
    int key1 = this.FindKey(key);
    if (key1 == -1)
      return base.TryGetValue(key, out value);
    value = this._values[key1];
    if (value != Uninitialized.Instance)
      return true;
    value = (object) null;
    return false;
  }

  public override int Count
  {
    get
    {
      int count = base.Count;
      foreach (object obj in this._values)
      {
        if (obj != Uninitialized.Instance)
          ++count;
      }
      return count;
    }
  }

  public override void Clear(ref DictionaryStorage storage)
  {
    for (int index = 0; index < this._values.Length; ++index)
      this._values[index] = (object) Uninitialized.Instance;
    base.Clear(ref storage);
  }

  public override List<KeyValuePair<object, object>> GetItems()
  {
    List<KeyValuePair<object, object>> items = base.GetItems();
    for (int index = 0; index < this._extraKeys.Length; ++index)
    {
      if (!string.IsNullOrEmpty(this._extraKeys[index]) && this._values[index] != Uninitialized.Instance)
        items.Add(new KeyValuePair<object, object>((object) this._extraKeys[index], this._values[index]));
    }
    return items;
  }

  public int KeyVersion => this._keyVersion;

  public int FindKey(object key) => key is string key1 ? this.FindKey(key1) : -1;

  public int FindKey(string key)
  {
    for (int key1 = 0; key1 < this._extraKeys.Length; ++key1)
    {
      if (this._extraKeys[key1] == key)
        return key1;
    }
    return -1;
  }

  public bool TryGetValue(int index, out object value)
  {
    value = this._values[index];
    return value != Uninitialized.Instance;
  }

  public object GetValueHelper(int index, object oldInstance)
  {
    object obj = this._values[index];
    return obj != Uninitialized.Instance ? obj : ((OldInstance) oldInstance).GetBoundMember((CodeContext) null, this._extraKeys[index]);
  }

  public bool TryGetValueHelper(int index, object oldInstance, out object res)
  {
    res = this._values[index];
    return res != Uninitialized.Instance || ((OldInstance) oldInstance).TryGetBoundCustomMember((CodeContext) null, this._extraKeys[index], out res);
  }

  public void SetExtraValue(int index, object value) => this._values[index] = value;
}
