// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ConstructorFunction
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("builtin_function_or_method")]
public class ConstructorFunction : BuiltinFunction
{
  private MethodBase[] _ctors;

  internal ConstructorFunction(BuiltinFunction realTarget, IList<MethodBase> constructors)
    : base("__new__", ArrayUtils.ToArray<MethodBase>((ICollection<MethodBase>) ConstructorFunction.GetTargetsValidateFunction(realTarget)), realTarget.DeclaringType, FunctionType.Function | FunctionType.AlwaysVisible)
  {
    this.Name = realTarget.Name;
    this.FunctionType = realTarget.FunctionType;
    this._ctors = ArrayUtils.ToArray<MethodBase>((ICollection<MethodBase>) constructors);
  }

  internal IList<MethodBase> ConstructorTargets => (IList<MethodBase>) this._ctors;

  public override BuiltinFunctionOverloadMapper Overloads
  {
    [PythonHidden(new PlatformID[] {})] get
    {
      return (BuiltinFunctionOverloadMapper) new ConstructorOverloadMapper(this, (object) null);
    }
  }

  private static IList<MethodBase> GetTargetsValidateFunction(BuiltinFunction realTarget)
  {
    ContractUtils.RequiresNotNull((object) realTarget, nameof (realTarget));
    return realTarget.Targets;
  }

  public new string __name__ => "__new__";

  public override string __doc__
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (MethodBase constructorTarget in (IEnumerable<MethodBase>) this.ConstructorTargets)
      {
        if (constructorTarget != (MethodBase) null)
          stringBuilder.AppendLine(DocBuilder.DocOneInfo(constructorTarget, "__new__"));
      }
      return stringBuilder.ToString();
    }
  }
}
