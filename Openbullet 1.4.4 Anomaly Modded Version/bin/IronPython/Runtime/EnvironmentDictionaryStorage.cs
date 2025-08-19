// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.EnvironmentDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal sealed class EnvironmentDictionaryStorage : DictionaryStorage
{
  private readonly CommonDictionaryStorage _storage = new CommonDictionaryStorage();

  public EnvironmentDictionaryStorage() => this.AddEnvironmentVars();

  private void AddEnvironmentVars()
  {
    try
    {
      foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
        this._storage.Add(environmentVariable.Key, environmentVariable.Value);
    }
    catch (SecurityException ex)
    {
    }
  }

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    this._storage.Add(key, value);
    string variable = key as string;
    string str = value as string;
    if (variable == null || str == null)
      return;
    Environment.SetEnvironmentVariable(variable, str);
  }

  public override bool Remove(ref DictionaryStorage storage, object key)
  {
    int num = this._storage.Remove(key) ? 1 : 0;
    if (!(key is string variable))
      return num != 0;
    Environment.SetEnvironmentVariable(variable, string.Empty);
    return num != 0;
  }

  public override bool Contains(object key) => this._storage.Contains(key);

  public override bool TryGetValue(object key, out object value)
  {
    return this._storage.TryGetValue(key, out value);
  }

  public override int Count => this._storage.Count;

  public override void Clear(ref DictionaryStorage storage)
  {
    foreach (KeyValuePair<object, object> keyValuePair in this.GetItems())
    {
      if (keyValuePair.Key is string key)
        Environment.SetEnvironmentVariable(key, string.Empty);
    }
    this._storage.Clear(ref storage);
  }

  public override List<KeyValuePair<object, object>> GetItems() => this._storage.GetItems();
}
