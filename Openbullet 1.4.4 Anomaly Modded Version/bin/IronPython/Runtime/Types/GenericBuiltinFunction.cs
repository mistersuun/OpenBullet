// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.GenericBuiltinFunction
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Types;

public class GenericBuiltinFunction : BuiltinFunction
{
  internal GenericBuiltinFunction(
    string name,
    MethodBase[] originalTargets,
    Type declaringType,
    FunctionType functionType)
    : base(name, originalTargets, declaringType, functionType)
  {
  }

  public BuiltinFunction this[PythonTuple tuple] => this[tuple._data];

  internal GenericBuiltinFunction(object instance, BuiltinFunction.BuiltinFunctionData data)
    : base(instance, data)
  {
  }

  internal override BuiltinFunction BindToInstance(object instance)
  {
    return (BuiltinFunction) new GenericBuiltinFunction(instance, this._data);
  }

  public BuiltinFunction this[params object[] key]
  {
    get
    {
      Type[] types = new Type[key.Length];
      for (int index = 0; index < types.Length; ++index)
        types[index] = Converter.ConvertToType(key[index]);
      BuiltinFunction builtinFunction = this.MakeGenericMethod(types);
      if (builtinFunction == null)
      {
        bool flag = false;
        foreach (MethodBase target in (IEnumerable<MethodBase>) this.Targets)
        {
          MethodInfo methodInfo = target as MethodInfo;
          if (methodInfo != (MethodInfo) null && methodInfo.ContainsGenericParameters)
            flag = true;
        }
        if (flag)
          throw PythonOps.TypeError($"bad type args to this generic method {this.Name}");
        throw PythonOps.TypeError($"{this.Name} is not a generic method and is unsubscriptable");
      }
      return this.IsUnbound ? builtinFunction : new BuiltinFunction(this._instance, builtinFunction._data);
    }
  }

  internal override bool IsOnlyGeneric
  {
    get
    {
      foreach (MethodBase target in (IEnumerable<MethodBase>) this.Targets)
      {
        if (!target.IsGenericMethod || !target.ContainsGenericParameters)
          return false;
      }
      return true;
    }
  }
}
