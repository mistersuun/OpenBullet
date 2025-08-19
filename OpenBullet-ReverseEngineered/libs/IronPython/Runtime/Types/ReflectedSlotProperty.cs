// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ReflectedSlotProperty
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("member_descriptor")]
internal class ReflectedSlotProperty : PythonTypeDataSlot, ICodeFormattable
{
  private readonly string _name;
  private readonly string _typeName;
  private readonly int _index;
  private static readonly Dictionary<int, ReflectedSlotProperty.SlotValue> _methods = new Dictionary<int, ReflectedSlotProperty.SlotValue>();

  public ReflectedSlotProperty(string name, string typeName, int index)
  {
    this._index = index;
    this._name = name;
    this._typeName = typeName;
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    if (instance != null)
    {
      value = this.Getter(instance);
      PythonOps.CheckInitializedAttribute(value, instance, this._name);
      return true;
    }
    value = (object) this;
    return true;
  }

  internal override bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    if (instance == null)
      return false;
    this.Setter(instance, value);
    return true;
  }

  internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    return this.TrySetValue(context, instance, owner, (object) Uninitialized.Instance);
  }

  public string __repr__(CodeContext context)
  {
    return $"<member '{this._name}' of '{this._typeName}' objects>";
  }

  private ReflectedSlotProperty.SlotValue Value
  {
    get
    {
      ReflectedSlotProperty.SlotValue slotValue;
      lock (ReflectedSlotProperty._methods)
      {
        if (!ReflectedSlotProperty._methods.TryGetValue(this._index, out slotValue))
          slotValue = ReflectedSlotProperty._methods[this._index] = new ReflectedSlotProperty.SlotValue();
      }
      return slotValue;
    }
  }

  internal SlotGetValue Getter
  {
    get
    {
      ReflectedSlotProperty.SlotValue slotValue = this.Value;
      lock (slotValue)
      {
        this.EnsureGetter(slotValue);
        return slotValue.Getter;
      }
    }
  }

  internal SlotSetValue Setter
  {
    get
    {
      ReflectedSlotProperty.SlotValue slotValue = this.Value;
      lock (slotValue)
      {
        this.EnsureSetter(slotValue);
        return slotValue.Setter;
      }
    }
  }

  internal int Index => this._index;

  private void EnsureGetter(ReflectedSlotProperty.SlotValue value)
  {
    if (value.Getter != null)
      return;
    value.Getter = (SlotGetValue) (instance => ((IPythonObject) instance).GetSlots()[this._index]);
  }

  private void EnsureSetter(ReflectedSlotProperty.SlotValue value)
  {
    if (value.Setter != null)
      return;
    value.Setter = (SlotSetValue) ((instance, setvalue) => ((IPythonObject) instance).GetSlots()[this._index] = setvalue);
  }

  private class SlotValue
  {
    public SlotGetValue Getter;
    public SlotSetValue Setter;
  }
}
