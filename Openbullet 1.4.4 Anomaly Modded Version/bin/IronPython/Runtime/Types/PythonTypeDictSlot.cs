// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.PythonTypeDictSlot
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("getset_descriptor")]
public sealed class PythonTypeDictSlot : PythonTypeSlot, ICodeFormattable
{
  private PythonType _type;

  public PythonTypeDictSlot(PythonType type) => this._type = type;

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    switch (instance)
    {
      case null:
        value = (object) new DictProxy(owner);
        return true;
      case PythonType dt:
        value = (object) new DictProxy(dt);
        return true;
      case IPythonObject pythonObject:
        if (pythonObject.PythonType.HasDictionary)
        {
          PythonDictionary pythonDictionary = pythonObject.Dict;
          if (pythonDictionary != null || (pythonDictionary = pythonObject.SetDict(pythonObject.PythonType.MakeDictionary())) != null)
          {
            value = (object) pythonDictionary;
            return true;
          }
          break;
        }
        break;
    }
    value = (object) null;
    return false;
  }

  internal override bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    if (instance is IPythonObject pythonObject)
    {
      if (!(value is PythonDictionary))
        throw PythonOps.TypeError("__dict__ must be set to a dictionary, not '{0}'", (object) owner.Name);
      if (!pythonObject.PythonType.HasDictionary)
        return false;
      pythonObject.ReplaceDict((PythonDictionary) value);
      return true;
    }
    if (instance == null)
      throw PythonOps.AttributeError("'__dict__' of '{0}' objects is not writable", (object) owner.Name);
    return false;
  }

  internal override bool IsSetDescriptor(CodeContext context, PythonType owner) => true;

  internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    if (instance is IPythonObject pythonObject)
    {
      if (!pythonObject.PythonType.HasDictionary)
        return false;
      pythonObject.ReplaceDict((PythonDictionary) null);
      return true;
    }
    if (instance == null)
      throw PythonOps.TypeError("'__dict__' of '{0}' objects is not writable", (object) owner.Name);
    return false;
  }

  public void __set__(CodeContext context, object instance, object value)
  {
    this.TrySetValue(context, instance, DynamicHelpers.GetPythonType(instance), value);
  }

  public string __repr__(CodeContext context)
  {
    return $"<attribute '__dict__' of '{this._type.Name}' objects>";
  }
}
