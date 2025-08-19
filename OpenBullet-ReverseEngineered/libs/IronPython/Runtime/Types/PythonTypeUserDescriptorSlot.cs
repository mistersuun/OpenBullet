// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.PythonTypeUserDescriptorSlot
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;

#nullable disable
namespace IronPython.Runtime.Types;

public sealed class PythonTypeUserDescriptorSlot : PythonTypeSlot
{
  private object _value;
  private int _descVersion;
  private PythonTypeSlot _desc;
  private const int UserDescriptorFalse = -1;

  internal PythonTypeUserDescriptorSlot(object value) => this._value = value;

  internal PythonTypeUserDescriptorSlot(object value, bool isntDescriptor)
  {
    this._value = value;
    if (!isntDescriptor)
      return;
    this._descVersion = -1;
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    try
    {
      value = PythonOps.GetUserDescriptor(this.Value, instance, (object) owner);
      return true;
    }
    catch (MissingMemberException ex)
    {
      value = (object) null;
      return false;
    }
  }

  internal object GetValue(CodeContext context, object instance, PythonType owner)
  {
    if (this._descVersion == -1)
      return this._value;
    if (this._descVersion != DynamicHelpers.GetPythonType(this._value).Version)
    {
      this.CalculateDescriptorInfo();
      if (this._descVersion == -1)
        return this._value;
    }
    object func;
    this._desc.TryGetValue(context, this._value, DynamicHelpers.GetPythonType(this._value), out func);
    return context.LanguageContext.Call(context, func, instance, (object) owner);
  }

  private void CalculateDescriptorInfo()
  {
    PythonType pythonType = DynamicHelpers.GetPythonType(this._value);
    if (!pythonType.IsSystemType)
    {
      this._descVersion = pythonType.Version;
      if (pythonType.TryResolveSlot(pythonType.Context.SharedClsContext, "__get__", out this._desc))
        return;
      this._descVersion = -1;
    }
    else
      this._descVersion = -1;
  }

  internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    return PythonOps.TryDeleteUserDescriptor(this.Value, instance);
  }

  internal override bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    return PythonOps.TrySetUserDescriptor(this.Value, instance, value);
  }

  internal override bool IsSetDescriptor(CodeContext context, PythonType owner)
  {
    return PythonOps.TryGetBoundAttr(context, this.Value, "__set__", out object _);
  }

  internal object Value
  {
    get => this._value;
    set => this._value = value;
  }
}
