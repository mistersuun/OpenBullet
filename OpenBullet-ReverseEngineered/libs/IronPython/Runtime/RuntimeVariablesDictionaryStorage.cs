// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.RuntimeVariablesDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

internal class RuntimeVariablesDictionaryStorage : CustomDictionaryStorage
{
  private readonly MutableTuple _boxes;
  private readonly string[] _args;

  public RuntimeVariablesDictionaryStorage(MutableTuple boxes, string[] args)
  {
    this._boxes = boxes;
    this._args = args;
  }

  internal MutableTuple Tuple => this._boxes;

  internal string[] Names => this._args;

  protected override IEnumerable<KeyValuePair<string, object>> GetExtraItems()
  {
    for (int i = 0; i < this._args.Length; ++i)
    {
      if (this.GetCell(i).Value != Uninitialized.Instance && this._args[i] != null)
        yield return new KeyValuePair<string, object>(this._args[i], this.GetCell(i).Value);
    }
  }

  protected override bool TrySetExtraValue(string key, object value)
  {
    for (int i = 0; i < this._args.Length; ++i)
    {
      if (this._args[i] == key)
      {
        this.GetCell(i).Value = value;
        return true;
      }
    }
    return false;
  }

  protected override bool TryGetExtraValue(string key, out object value)
  {
    for (int i = 0; i < this._args.Length; ++i)
    {
      if (this._args[i] == key)
      {
        value = this.GetCell(i).Value;
        return true;
      }
    }
    value = (object) null;
    return false;
  }

  protected override bool? TryRemoveExtraValue(string key)
  {
    for (int i = 0; i < this._args.Length; ++i)
    {
      if (this._args[i] == key)
      {
        ClosureCell cell = this.GetCell(i);
        if (cell.Value == Uninitialized.Instance)
          return new bool?(false);
        cell.Value = (object) Uninitialized.Instance;
        return new bool?(true);
      }
    }
    return new bool?();
  }

  internal ClosureCell GetCell(int i)
  {
    return (ClosureCell) this._boxes.GetNestedValue(this._args.Length, i);
  }
}
