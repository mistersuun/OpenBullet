// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.TypeGroupOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class TypeGroupOps
{
  public static string __repr__(TypeGroup self)
  {
    StringBuilder stringBuilder = new StringBuilder("<types ");
    bool flag = false;
    foreach (Type type in self.Types)
    {
      if (flag)
        stringBuilder.Append(", ");
      PythonType pythonTypeFromType = DynamicHelpers.GetPythonTypeFromType(type);
      stringBuilder.Append('\'');
      stringBuilder.Append(pythonTypeFromType.Name);
      stringBuilder.Append('\'');
      flag = true;
    }
    stringBuilder.Append(">");
    return stringBuilder.ToString();
  }

  [SpecialName]
  public static PythonType GetItem(TypeGroup self, params PythonType[] types)
  {
    return TypeGroupOps.GetItemHelper(self, types);
  }

  [SpecialName]
  public static object Call(CodeContext context, TypeGroup self, params object[] args)
  {
    return PythonCalls.Call(context, (object) DynamicHelpers.GetPythonTypeFromType(self.GetNonGenericType()), args ?? ArrayUtils.EmptyObjects);
  }

  [SpecialName]
  public static object Call(
    CodeContext context,
    TypeGroup self,
    [ParamDictionary] PythonDictionary kwArgs,
    params object[] args)
  {
    return PythonCalls.CallWithKeywordArgs(context, (object) DynamicHelpers.GetPythonTypeFromType(self.GetNonGenericType()), args ?? ArrayUtils.EmptyObjects, (IDictionary<object, object>) (kwArgs ?? new PythonDictionary()));
  }

  [SpecialName]
  public static PythonType GetItem(TypeGroup self, params object[] types)
  {
    PythonType[] types1 = new PythonType[types.Length];
    for (int index = 0; index < types.Length; ++index)
    {
      object type = types[index];
      switch (type)
      {
        case PythonType _:
          types1[index] = (PythonType) type;
          break;
        case TypeGroup _:
          TypeGroup typeGroup = type as TypeGroup;
          Type nonGenericType;
          if (!typeGroup.TryGetNonGenericType(out nonGenericType))
            throw PythonOps.TypeError("cannot use open generic type {0} as type argument", (object) typeGroup.Name);
          types1[index] = DynamicHelpers.GetPythonTypeFromType(nonGenericType);
          break;
        default:
          throw PythonOps.TypeErrorForTypeMismatch("type", type);
      }
    }
    return TypeGroupOps.GetItemHelper(self, types1);
  }

  [SpecialName]
  public static PythonType GetItem(TypeGroup self, PythonTuple tuple)
  {
    return tuple.__len__() == 0 ? DynamicHelpers.GetPythonTypeFromType(self.GetNonGenericType()) : TypeGroupOps.GetItem(self, tuple._data);
  }

  private static PythonType GetItemHelper(TypeGroup self, PythonType[] types)
  {
    Type type = (self.GetTypeForArity(types.Length) ?? throw new ValueErrorException($"could not find compatible generic type for {types.Length} type arguments")).Type;
    if (types.Length != 0)
      type = type.MakeGenericType(PythonTypeOps.ConvertToTypes(types));
    return DynamicHelpers.GetPythonTypeFromType(type);
  }
}
