// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.DynamicHelpers
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace IronPython.Runtime.Types;

public static class DynamicHelpers
{
  public static PythonType GetPythonTypeFromType(Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    return PythonType.GetPythonType(type);
  }

  public static PythonType GetPythonType(object o)
  {
    return o is IPythonObject pythonObject ? pythonObject.PythonType : DynamicHelpers.GetPythonTypeFromType(CompilerHelpers.GetType(o));
  }

  public static ReflectedEvent.BoundEvent MakeBoundEvent(
    ReflectedEvent eventObj,
    object instance,
    Type type)
  {
    return new ReflectedEvent.BoundEvent(eventObj, instance, DynamicHelpers.GetPythonTypeFromType(type));
  }
}
