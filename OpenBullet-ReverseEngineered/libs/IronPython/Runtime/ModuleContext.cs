// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ModuleContext
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;

#nullable disable
namespace IronPython.Runtime;

public sealed class ModuleContext
{
  private readonly PythonContext _pyContext;
  private readonly PythonDictionary _globals;
  private readonly CodeContext _globalContext;
  private readonly PythonModule _module;
  private ExtensionMethodSet _extensionMethods = ExtensionMethodSet.Empty;
  private ModuleOptions _features;

  public ModuleContext(PythonDictionary globals, PythonContext creatingContext)
  {
    ContractUtils.RequiresNotNull((object) globals, nameof (globals));
    ContractUtils.RequiresNotNull((object) creatingContext, nameof (creatingContext));
    this._globals = globals;
    this._pyContext = creatingContext;
    this._globalContext = new CodeContext(globals, this);
    this._module = new PythonModule(globals);
    this._module.Scope.SetExtension(this._pyContext.ContextId, (ScopeExtension) new PythonScopeExtension(this._pyContext, this._module, this));
  }

  public ModuleContext(PythonModule module, PythonContext creatingContext)
  {
    ContractUtils.RequiresNotNull((object) module, nameof (module));
    ContractUtils.RequiresNotNull((object) creatingContext, nameof (creatingContext));
    this._globals = module.__dict__;
    this._pyContext = creatingContext;
    this._globalContext = new CodeContext(this._globals, this);
    this._module = module;
  }

  public PythonDictionary Globals => this._globals;

  public PythonContext Context => this._pyContext;

  public Scope GlobalScope => this._module.Scope;

  public CodeContext GlobalContext => this._globalContext;

  public PythonModule Module => this._module;

  public ModuleOptions Features
  {
    get => this._features;
    set => this._features = value;
  }

  public bool ShowCls
  {
    get => (this._features & ModuleOptions.ShowClsMethods) != 0;
    set
    {
      if (value)
        this._features |= ModuleOptions.ShowClsMethods;
      else
        this._features &= ~ModuleOptions.ShowClsMethods;
    }
  }

  internal ExtensionMethodSet ExtensionMethods
  {
    get => this._extensionMethods;
    set => this._extensionMethods = value;
  }

  internal void InitializeBuiltins(bool moduleBuiltins)
  {
    if (this.Globals.ContainsKey((object) "__builtins__"))
      return;
    if (moduleBuiltins)
      this.Globals[(object) "__builtins__"] = (object) this.Context.BuiltinModuleInstance;
    else
      this.Globals[(object) "__builtins__"] = (object) this.Context.BuiltinModuleDict;
  }
}
