// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ModuleLoader
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using Microsoft.Scripting;

#nullable disable
namespace IronPython.Runtime;

public sealed class ModuleLoader
{
  private readonly OnDiskScriptCode _sc;
  private readonly string _parentName;
  private readonly string _name;

  internal ModuleLoader(OnDiskScriptCode sc, string parentName, string name)
  {
    this._sc = sc;
    this._parentName = parentName;
    this._name = name;
  }

  public PythonModule load_module(CodeContext context, string fullName)
  {
    PythonContext languageContext = context.LanguageContext;
    CodeContext context1 = this._sc.CreateContext();
    context1.ModuleContext.InitializeBuiltins(false);
    languageContext.InitializeModule(this._sc.SourceUnit.Path, context1.ModuleContext, (ScriptCode) this._sc, ModuleOptions.Initialize);
    object obj;
    if (this._parentName != null && languageContext.SystemStateModules.TryGetValue((object) this._parentName, out obj) && obj is PythonModule pythonModule)
      pythonModule.__dict__[(object) this._name] = (object) context1.ModuleContext.Module;
    return context1.ModuleContext.Module;
  }
}
