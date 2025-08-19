// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ConstructorOverloadMapper
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Types;

public class ConstructorOverloadMapper(ConstructorFunction builtinFunction, object instance) : 
  BuiltinFunctionOverloadMapper((BuiltinFunction) builtinFunction, instance)
{
  public override IList<MethodBase> Targets
  {
    get => ((ConstructorFunction) this.Function).ConstructorTargets;
  }

  protected override object GetTargetFunction(BuiltinFunction bf)
  {
    return bf.Targets[0].DeclaringType != typeof (InstanceOps) ? (object) new ConstructorFunction(InstanceOps.OverloadedNew, bf.Targets).BindToInstance((object) bf) : base.GetTargetFunction(bf);
  }
}
