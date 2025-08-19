// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Super
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

[PythonType("super")]
public class Super : PythonTypeSlot, ICodeFormattable
{
  private PythonType _thisClass;
  private object _self;
  private object _selfClass;

  public void __init__(PythonType type) => this.__init__(type, (object) null);

  public void __init__(PythonType type, object obj)
  {
    if (obj != null)
    {
      PythonType pythonType = obj as PythonType;
      if (PythonOps.IsInstance(obj, type))
      {
        this._thisClass = type;
        this._self = obj;
        this._selfClass = (object) DynamicHelpers.GetPythonType(obj);
      }
      else
      {
        this._thisClass = pythonType != null && pythonType.IsSubclassOf(type) ? type : throw PythonOps.TypeError("super(type, obj): obj must be an instance or subtype of type {1}, not {0}", (object) PythonTypeOps.GetName(obj), (object) type.Name);
        this._selfClass = obj;
        this._self = obj;
      }
    }
    else
    {
      this._thisClass = type;
      this._self = (object) null;
      this._selfClass = (object) null;
    }
  }

  public PythonType __thisclass__ => this._thisClass;

  public object __self__ => this._self;

  public object __self_class__ => this._selfClass;

  public new object __get__(CodeContext context, object instance, object owner)
  {
    PythonType pythonType = this.PythonType;
    if (pythonType != TypeCache.Super)
      return PythonCalls.Call(context, (object) pythonType, (object) this._thisClass, instance);
    Super super = new Super();
    super.__init__(this._thisClass, instance);
    return (object) super;
  }

  [SpecialName]
  public object GetCustomMember(CodeContext context, string name)
  {
    object customMember;
    if (this._selfClass is PythonType selfClass)
    {
      IList<PythonType> resolutionOrder = selfClass.ResolutionOrder;
      bool flag = false;
      int index1;
      for (index1 = 0; index1 < resolutionOrder.Count; ++index1)
      {
        if (resolutionOrder[index1] == this._thisClass)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        index1 = 0;
        resolutionOrder = this._thisClass.ResolutionOrder;
      }
      object self = this._self == this._selfClass ? (object) null : this._self;
      for (int index2 = index1 + 1; index2 < resolutionOrder.Count; ++index2)
      {
        if (this.TryLookupInBase(context, resolutionOrder[index2], name, self, out customMember))
          return customMember;
      }
    }
    return this.PythonType.TryGetBoundMember(context, (object) this, name, out customMember) ? customMember : (object) OperationFailed.Value;
  }

  [SpecialName]
  public void SetMember(CodeContext context, string name, object value)
  {
    this.PythonType.SetMember(context, (object) this, name, value);
  }

  [SpecialName]
  public void DeleteCustomMember(CodeContext context, string name)
  {
    this.PythonType.DeleteMember(context, (object) this, name);
  }

  private bool TryLookupInBase(
    CodeContext context,
    PythonType pt,
    string name,
    object self,
    out object value)
  {
    if (pt.OldClass == null)
    {
      PythonTypeSlot slot;
      if (pt.TryLookupSlot(context, name, out slot) && slot.TryGetValue(context, self, this.DescriptorContext, out value))
        return true;
    }
    else if (pt.OldClass.TryLookupOneSlot(this.DescriptorContext, name, out value))
    {
      value = OldClass.GetOldStyleDescriptor(context, value, self, this.DescriptorContext);
      return true;
    }
    value = (object) null;
    return false;
  }

  private PythonType DescriptorContext
  {
    get
    {
      return !DynamicHelpers.GetPythonType(this._self).IsSubclassOf(this._thisClass) ? (this._self == this._selfClass && this._selfClass is PythonType selfClass1 ? selfClass1 : this._thisClass) : (this._selfClass is PythonType selfClass2 ? selfClass2 : ((OldClass) this._selfClass).TypeObject);
    }
  }

  private PythonType PythonType
  {
    get => this.GetType() == typeof (Super) ? TypeCache.Super : (this as IPythonObject).PythonType;
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = this.__get__(context, instance, (object) owner);
    return true;
  }

  internal override bool GetAlwaysSucceeds => true;

  public string __repr__(CodeContext context)
  {
    string str = this._self != this ? PythonOps.Repr(context, this._self) : "<super object>";
    return $"<{PythonTypeOps.GetName((object) this)}: {PythonOps.Repr(context, (object) this._thisClass)}, {str}>";
  }
}
