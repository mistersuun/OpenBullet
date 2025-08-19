// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ScopeVariableIgnoreCase
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting;

public sealed class ScopeVariableIgnoreCase : IScopeVariable, IWeakReferencable
{
  private readonly string _firstCasing;
  private readonly ScopeVariable _firstVariable;
  private WeakReference _weakref;
  private Dictionary<string, ScopeVariable> _overflow;

  internal ScopeVariableIgnoreCase(string casing)
  {
    this._firstCasing = casing;
    this._firstVariable = new ScopeVariable();
  }

  public bool HasValue
  {
    get
    {
      if (this._firstVariable.HasValue)
        return true;
      if (this._overflow == null)
        return false;
      lock (this._overflow)
      {
        foreach (KeyValuePair<string, ScopeVariable> keyValuePair in this._overflow)
        {
          if (keyValuePair.Value.HasValue)
            return true;
        }
      }
      return false;
    }
  }

  public bool TryGetValue(out object value)
  {
    object obj;
    if (this._firstVariable.TryGetValue(out obj))
    {
      value = obj;
      return true;
    }
    if (this._overflow != null)
    {
      lock (this._overflow)
      {
        foreach (KeyValuePair<string, ScopeVariable> keyValuePair in this._overflow)
        {
          if (keyValuePair.Value.TryGetValue(out obj))
          {
            value = obj;
            return true;
          }
        }
      }
    }
    value = (object) null;
    return false;
  }

  public void SetValue(object value) => this._firstVariable.SetValue(value);

  public bool DeleteValue()
  {
    bool flag = this._firstVariable.DeleteValue();
    if (this._overflow != null)
    {
      lock (this._overflow)
      {
        foreach (KeyValuePair<string, ScopeVariable> keyValuePair in this._overflow)
          flag = keyValuePair.Value.DeleteValue() | flag;
      }
    }
    return flag;
  }

  internal ScopeVariable GetCaseSensitiveStorage(string name)
  {
    return name == this._firstCasing ? this._firstVariable : this.GetStorageSlow(name);
  }

  internal void AddNames(List<string> list)
  {
    if (this._firstVariable.HasValue)
      list.Add(this._firstCasing);
    if (this._overflow == null)
      return;
    lock (this._overflow)
    {
      foreach (KeyValuePair<string, ScopeVariable> keyValuePair in this._overflow)
      {
        if (keyValuePair.Value.HasValue)
          list.Add(keyValuePair.Key);
      }
    }
  }

  internal void AddItems(List<KeyValuePair<string, object>> list)
  {
    object obj;
    if (this._firstVariable.TryGetValue(out obj))
      list.Add(new KeyValuePair<string, object>(this._firstCasing, obj));
    if (this._overflow == null)
      return;
    lock (this._overflow)
    {
      foreach (KeyValuePair<string, ScopeVariable> keyValuePair in this._overflow)
      {
        if (keyValuePair.Value.TryGetValue(out obj))
          list.Add(new KeyValuePair<string, object>(keyValuePair.Key, obj));
      }
    }
  }

  private ScopeVariable GetStorageSlow(string name)
  {
    if (this._overflow == null)
      Interlocked.CompareExchange<Dictionary<string, ScopeVariable>>(ref this._overflow, new Dictionary<string, ScopeVariable>(), (Dictionary<string, ScopeVariable>) null);
    lock (this._overflow)
    {
      ScopeVariable storageSlow;
      if (!this._overflow.TryGetValue(name, out storageSlow))
        this._overflow[name] = storageSlow = new ScopeVariable();
      return storageSlow;
    }
  }

  public WeakReference WeakReference
  {
    get => this._weakref ?? (this._weakref = new WeakReference((object) this));
  }
}
