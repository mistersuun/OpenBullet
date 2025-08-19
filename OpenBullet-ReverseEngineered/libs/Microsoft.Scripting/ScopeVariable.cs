// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ScopeVariable
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting;

public sealed class ScopeVariable : IScopeVariable, IWeakReferencable
{
  private object _value;
  private WeakReference _weakref;
  private static readonly object _novalue = new object();

  internal ScopeVariable() => this._value = ScopeVariable._novalue;

  public bool HasValue => this._value != ScopeVariable._novalue;

  public bool TryGetValue(out object value)
  {
    value = this._value;
    if (value != ScopeVariable._novalue)
      return true;
    value = (object) null;
    return false;
  }

  public void SetValue(object value) => this._value = value;

  public bool DeleteValue()
  {
    return Interlocked.Exchange(ref this._value, ScopeVariable._novalue) != ScopeVariable._novalue;
  }

  public WeakReference WeakReference
  {
    get => this._weakref ?? (this._weakref = new WeakReference((object) this));
  }
}
