// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonFunctionRecursionCheck3
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using System;

#nullable disable
namespace IronPython.Compiler;

internal class PythonFunctionRecursionCheck3
{
  private readonly Func<PythonFunction, object, object, object, object> _target;

  public PythonFunctionRecursionCheck3(
    Func<PythonFunction, object, object, object, object> target)
  {
    this._target = target;
  }

  public object CallTarget(PythonFunction function, object arg0, object arg1, object arg2)
  {
    try
    {
      PythonOps.FunctionPushFrame(function.Context.LanguageContext);
      return this._target(function, arg0, arg1, arg2);
    }
    finally
    {
      PythonOps.FunctionPopFrame();
    }
  }
}
