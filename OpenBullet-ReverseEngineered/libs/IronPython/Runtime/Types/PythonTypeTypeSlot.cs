// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.PythonTypeTypeSlot
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;

#nullable disable
namespace IronPython.Runtime.Types;

public class PythonTypeTypeSlot : PythonTypeDataSlot
{
  public static string __doc__ = "the object's class";

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = instance != null ? (object) DynamicHelpers.GetPythonType(instance) : (owner != TypeCache.Null ? (object) DynamicHelpers.GetPythonType((object) owner) : (object) owner);
    return true;
  }

  internal override bool GetAlwaysSucceeds => true;

  internal override bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    if (instance == null)
      return false;
    if (!(instance is IPythonObject pythonObject))
      throw PythonOps.TypeError("__class__ assignment: only for user defined types");
    if (!(value is PythonType newType))
      throw PythonOps.TypeError("__class__ must be set to new-style class, not '{0}' object", (object) DynamicHelpers.GetPythonType(value).Name);
    if (newType.UnderlyingSystemType != DynamicHelpers.GetPythonType(instance).UnderlyingSystemType)
      throw PythonOps.TypeErrorForIncompatibleObjectLayout("__class__ assignment", DynamicHelpers.GetPythonType(instance), newType.UnderlyingSystemType);
    pythonObject.SetPythonType(newType);
    return true;
  }

  internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    throw PythonOps.AttributeErrorForReadonlyAttribute(PythonTypeOps.GetName(instance), "__class__");
  }
}
