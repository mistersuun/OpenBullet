// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.NewTypeInfo
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime.Types;

internal class NewTypeInfo
{
  private readonly Type _baseType;
  private readonly IList<Type> _interfaceTypes;
  private int? _hash;

  public NewTypeInfo(Type baseType, IList<Type> interfaceTypes)
  {
    this._baseType = baseType;
    this._interfaceTypes = interfaceTypes;
  }

  public static NewTypeInfo GetTypeInfo(string typeName, PythonTuple bases)
  {
    List<Type> typeList = new List<Type>();
    Type type1 = typeof (object);
    PythonType pythonType1 = (PythonType) null;
    foreach (PythonType pythonType2 in NewTypeInfo.GetPythonTypes(typeName, (ICollection<object>) bases))
    {
      IList<Type> baseInterfaces = (IList<Type>) ReflectionUtils.EmptyTypes;
      Type type2 = pythonType2.ExtensionType;
      if (pythonType2.ExtensionType.IsInterface)
      {
        baseInterfaces = (IList<Type>) new Type[1]{ type2 };
        type2 = typeof (object);
      }
      else if (NewTypeMaker.IsInstanceType(type2))
      {
        baseInterfaces = (IList<Type>) new List<Type>();
        type2 = NewTypeInfo.GetBaseTypeFromUserType(pythonType2, baseInterfaces, type2.BaseType);
      }
      if (type2 == (Type) null || typeof (BuiltinFunction).IsAssignableFrom(type2) || typeof (PythonFunction).IsAssignableFrom(type2))
        throw PythonOps.TypeError(typeName + ": {0} is not an acceptable base type", (object) pythonType2.Name);
      if (type2.ContainsGenericParameters())
        throw PythonOps.TypeError(typeName + ": cannot inhert from open generic instantiation {0}. Only closed instantiations are supported.", (object) pythonType2);
      foreach (Type type3 in (IEnumerable<Type>) baseInterfaces)
      {
        if (type3.ContainsGenericParameters())
          throw PythonOps.TypeError(typeName + ": cannot inhert from open generic instantiation {0}. Only closed instantiations are supported.", (object) type3);
        typeList.Add(type3);
      }
      if (!type1.IsSubclassOf(type2))
      {
        type1 = !(type1 != typeof (object)) || !(type1 != type2) || type1.IsDefined(typeof (DynamicBaseTypeAttribute), false) || type2.IsSubclassOf(type1) ? type2 : throw PythonOps.TypeError(typeName + ": can only extend one CLI or builtin type, not both {0} (for {1}) and {2} (for {3})", (object) type1.FullName, (object) pythonType1, (object) type2.FullName, (object) pythonType2);
        pythonType1 = pythonType2;
      }
    }
    return new NewTypeInfo(type1, typeList.Count == 0 ? (IList<Type>) ReflectionUtils.EmptyTypes : (IList<Type>) typeList.ToArray());
  }

  private static IEnumerable<PythonType> GetPythonTypes(string typeName, ICollection<object> bases)
  {
    foreach (object obj in (IEnumerable<object>) bases)
    {
      switch (obj)
      {
        case PythonType pythonType:
          yield return pythonType;
          continue;
        case OldClass _:
          continue;
        default:
          throw PythonOps.TypeError($"{typeName}: unsupported base type for new-style class {obj}");
      }
    }
  }

  private static Type GetBaseTypeFromUserType(
    PythonType curBasePythonType,
    IList<Type> baseInterfaces,
    Type curTypeToExtend)
  {
    Queue<PythonType> pythonTypeQueue = new Queue<PythonType>();
    pythonTypeQueue.Enqueue(curBasePythonType);
    do
    {
      foreach (PythonType baseType in (IEnumerable<PythonType>) pythonTypeQueue.Dequeue().BaseTypes)
      {
        if (!(baseType.ExtensionType == curTypeToExtend) && !curTypeToExtend.IsSubclassOf(baseType.ExtensionType))
        {
          if (baseType.ExtensionType.IsInterface)
            baseInterfaces.Add(baseType.ExtensionType);
          else if (NewTypeMaker.IsInstanceType(baseType.ExtensionType))
            pythonTypeQueue.Enqueue(baseType);
          else if (!baseType.IsOldClass)
          {
            curTypeToExtend = (Type) null;
            break;
          }
        }
      }
    }
    while (pythonTypeQueue.Count > 0);
    return curTypeToExtend;
  }

  public Type BaseType => this._baseType;

  public IList<Type> InterfaceTypes => this._interfaceTypes;

  public override int GetHashCode()
  {
    if (!this._hash.HasValue)
    {
      int hashCode = this._baseType.GetHashCode();
      for (int index = 0; index < this._interfaceTypes.Count; ++index)
        hashCode ^= this._interfaceTypes[index].GetHashCode();
      this._hash = new int?(hashCode);
    }
    return this._hash.Value;
  }

  public override bool Equals(object obj)
  {
    if (!(obj is NewTypeInfo newTypeInfo) || !this._baseType.Equals(newTypeInfo._baseType) || this._interfaceTypes.Count != newTypeInfo._interfaceTypes.Count)
      return false;
    for (int index = 0; index < this._interfaceTypes.Count; ++index)
    {
      if (!this._interfaceTypes[index].Equals(newTypeInfo._interfaceTypes[index]))
        return false;
    }
    return true;
  }
}
