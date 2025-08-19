// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.GlobalDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

internal class GlobalDictionaryStorage : CustomDictionaryStorage
{
  private readonly Dictionary<string, PythonGlobal> _globals;
  private readonly PythonGlobal[] _data;
  private PythonGlobal _path;
  private PythonGlobal _package;
  private PythonGlobal _builtins;
  private PythonGlobal _name;

  public GlobalDictionaryStorage(Dictionary<string, PythonGlobal> globals)
  {
    this._globals = globals;
  }

  public GlobalDictionaryStorage(Dictionary<string, PythonGlobal> globals, PythonGlobal[] data)
  {
    this._globals = globals;
    this._data = data;
  }

  protected override IEnumerable<KeyValuePair<string, object>> GetExtraItems()
  {
    foreach (KeyValuePair<string, PythonGlobal> global in this._globals)
    {
      if (global.Value.RawValue != Uninitialized.Instance)
        yield return new KeyValuePair<string, object>(global.Key, global.Value.RawValue);
    }
  }

  protected override bool? TryRemoveExtraValue(string key)
  {
    PythonGlobal pythonGlobal;
    if (!this._globals.TryGetValue(key, out pythonGlobal))
      return new bool?();
    if (pythonGlobal.RawValue == Uninitialized.Instance)
      return new bool?(false);
    pythonGlobal.RawValue = (object) Uninitialized.Instance;
    return new bool?(true);
  }

  protected override bool TrySetExtraValue(string key, object value)
  {
    PythonGlobal pythonGlobal;
    if (!this._globals.TryGetValue(key, out pythonGlobal))
      return false;
    pythonGlobal.CurrentValue = value;
    return true;
  }

  protected override bool TryGetExtraValue(string key, out object value)
  {
    PythonGlobal pythonGlobal;
    if (this._globals.TryGetValue(key, out pythonGlobal))
    {
      value = pythonGlobal.RawValue;
      return true;
    }
    value = (object) null;
    return false;
  }

  public override bool TryGetBuiltins(out object value)
  {
    return this.TryGetCachedValue(ref this._builtins, "__builtins__", out value);
  }

  public override bool TryGetPath(out object value)
  {
    return this.TryGetCachedValue(ref this._path, "__path__", out value);
  }

  public override bool TryGetPackage(out object value)
  {
    return this.TryGetCachedValue(ref this._package, "__package__", out value);
  }

  public override bool TryGetName(out object value)
  {
    return this.TryGetCachedValue(ref this._name, "__name__", out value);
  }

  private bool TryGetCachedValue(ref PythonGlobal storage, string name, out object value)
  {
    if (storage == null && !this._globals.TryGetValue(name, out storage))
      return this.TryGetValue((object) name, out value);
    value = storage.RawValue;
    return value != Uninitialized.Instance;
  }

  public PythonGlobal[] Data => this._data;
}
