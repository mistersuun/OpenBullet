// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.ContextArgBuilder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions.Calls;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Binding;

public sealed class ContextArgBuilder(ParameterInfo info) : ArgBuilder(info)
{
  public override int Priority => -1;

  public override int ConsumedArgumentCount => 0;

  protected override Expression ToExpression(
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed)
  {
    return ((PythonOverloadResolver) resolver).ContextExpression;
  }
}
