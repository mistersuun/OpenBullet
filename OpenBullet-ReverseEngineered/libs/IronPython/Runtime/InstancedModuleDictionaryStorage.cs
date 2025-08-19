// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.InstancedModuleDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

internal class InstancedModuleDictionaryStorage : ModuleDictionaryStorage
{
  private BuiltinPythonModule _module;

  public InstancedModuleDictionaryStorage(
    BuiltinPythonModule moduleInstance,
    Dictionary<string, PythonGlobal> globalsDict)
    : base(moduleInstance.GetType(), globalsDict)
  {
    this._module = moduleInstance;
  }

  public override BuiltinPythonModule Instance => this._module;
}
