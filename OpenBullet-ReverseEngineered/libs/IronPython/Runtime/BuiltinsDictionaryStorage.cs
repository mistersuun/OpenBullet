// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.BuiltinsDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Modules;
using Microsoft.Scripting.Runtime;
using System;

#nullable disable
namespace IronPython.Runtime;

internal class BuiltinsDictionaryStorage : ModuleDictionaryStorage
{
  private readonly EventHandler<ModuleChangeEventArgs> _change;
  private object _import;

  public BuiltinsDictionaryStorage(EventHandler<ModuleChangeEventArgs> change)
    : base(typeof (Builtin))
  {
    this._change = change;
  }

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    if (key is string name)
    {
      if (name == "__import__")
        this._import = value;
      this._change((object) this, new ModuleChangeEventArgs(name, ModuleChangeType.Set, value));
    }
    base.Add(ref storage, key, value);
  }

  protected override void LazyAdd(object name, object value) => this.Add(name, value);

  public override bool Remove(ref DictionaryStorage storage, object key)
  {
    if (key is string name)
    {
      if (name == "__import__")
        this._import = (object) null;
      this._change((object) this, new ModuleChangeEventArgs(name, ModuleChangeType.Delete));
    }
    return base.Remove(ref storage, key);
  }

  public override void Clear(ref DictionaryStorage storage)
  {
    this._import = (object) null;
    base.Clear(ref storage);
  }

  public override bool TryGetImport(out object value)
  {
    if (this._import == null)
    {
      if (!base.TryGetImport(out value))
        return false;
      this._import = value;
      return true;
    }
    value = this._import;
    return true;
  }

  public override void Reload()
  {
    this._import = (object) null;
    base.Reload();
  }
}
