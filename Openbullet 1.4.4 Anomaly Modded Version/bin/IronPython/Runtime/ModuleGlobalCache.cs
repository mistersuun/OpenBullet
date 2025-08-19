// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ModuleGlobalCache
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;

#nullable disable
namespace IronPython.Runtime;

public sealed class ModuleGlobalCache
{
  private object _value;
  internal static readonly object NotCaching = new object();
  internal static readonly ModuleGlobalCache NoCache = new ModuleGlobalCache(ModuleGlobalCache.NotCaching);

  public ModuleGlobalCache(object value) => this._value = value;

  public bool IsCaching => this._value != ModuleGlobalCache.NotCaching;

  public bool HasValue => this._value != Uninitialized.Instance;

  public object Value
  {
    get => this._value;
    set
    {
      this._value = this._value != ModuleGlobalCache.NotCaching ? value : throw new ValueErrorException("Cannot change non-caching value.");
    }
  }

  public void Changed(object sender, ModuleChangeEventArgs e)
  {
    ContractUtils.RequiresNotNull((object) e, nameof (e));
    switch (e.ChangeType)
    {
      case ModuleChangeType.Set:
        this.Value = e.Value;
        break;
      case ModuleChangeType.Delete:
        this.Value = (object) Uninitialized.Instance;
        break;
    }
  }
}
