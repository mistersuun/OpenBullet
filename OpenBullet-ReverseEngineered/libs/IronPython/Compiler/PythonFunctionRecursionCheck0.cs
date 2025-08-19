// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonFunctionRecursionCheck0
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using System;

#nullable disable
namespace IronPython.Compiler;

internal class PythonFunctionRecursionCheck0
{
  private readonly Func<PythonFunction, object> _target;

  public PythonFunctionRecursionCheck0(Func<PythonFunction, object> target)
  {
    this._target = target;
  }

  public object CallTarget(PythonFunction function)
  {
    try
    {
      PythonOps.FunctionPushFrame(function.Context.LanguageContext);
      return this._target(function);
    }
    finally
    {
      PythonOps.FunctionPopFrame();
    }
  }
}
