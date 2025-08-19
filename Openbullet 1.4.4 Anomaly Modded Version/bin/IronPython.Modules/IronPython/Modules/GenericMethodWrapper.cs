// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.GenericMethodWrapper
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Modules;

[PythonType("method-wrapper")]
public class GenericMethodWrapper
{
  private readonly string name;
  private readonly IProxyObject target;

  public GenericMethodWrapper(string methodName, IProxyObject proxyTarget)
  {
    this.name = methodName;
    this.target = proxyTarget;
  }

  [SpecialName]
  public object Call(CodeContext context, params object[] args)
  {
    return PythonOps.Invoke(context, this.target.Target, this.name, args);
  }

  [SpecialName]
  public object Call(CodeContext context, [ParamDictionary] IDictionary<object, object> dict, params object[] args)
  {
    object func;
    if (!DynamicHelpers.GetPythonType(this.target.Target).TryGetBoundMember(context, this.target.Target, this.name, out func))
      throw PythonOps.AttributeError("type {0} has no attribute {1}", (object) DynamicHelpers.GetPythonType(this.target.Target), (object) this.name);
    return PythonCalls.CallWithKeywordArgs(context, func, args, dict);
  }
}
