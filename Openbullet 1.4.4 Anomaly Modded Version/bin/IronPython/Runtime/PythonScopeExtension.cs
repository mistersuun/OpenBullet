// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonScopeExtension
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

internal class PythonScopeExtension : ScopeExtension
{
  private readonly ModuleContext _modContext;
  private readonly PythonModule _module;
  private Dictionary<object, object> _objectKeys;

  public PythonScopeExtension(PythonContext context, Scope scope)
    : base(scope)
  {
    this._module = new PythonModule(context, scope);
    this._modContext = new ModuleContext(this._module, context);
  }

  public PythonScopeExtension(PythonContext context, PythonModule module, ModuleContext modContext)
    : base(module.Scope)
  {
    this._module = module;
    this._modContext = modContext;
  }

  public ModuleContext ModuleContext => this._modContext;

  public PythonModule Module => this._module;

  public Dictionary<object, object> EnsureObjectKeys()
  {
    if (this._objectKeys == null)
      Interlocked.CompareExchange<Dictionary<object, object>>(ref this._objectKeys, new Dictionary<object, object>(), (Dictionary<object, object>) null);
    return this._objectKeys;
  }

  public Dictionary<object, object> ObjectKeys => this._objectKeys;
}
