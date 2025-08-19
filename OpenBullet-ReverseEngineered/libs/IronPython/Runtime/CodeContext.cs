// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.CodeContext
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System.Diagnostics;

#nullable disable
namespace IronPython.Runtime;

[DebuggerTypeProxy(typeof (CodeContext.DebugProxy))]
[DebuggerDisplay("ModuleName = {ModuleName}")]
public sealed class CodeContext
{
  private readonly ModuleContext _modContext;
  private readonly PythonDictionary _dict;

  public CodeContext(PythonDictionary dict, ModuleContext moduleContext)
  {
    ContractUtils.RequiresNotNull((object) dict, nameof (dict));
    ContractUtils.RequiresNotNull((object) moduleContext, nameof (moduleContext));
    this._dict = dict;
    this._modContext = moduleContext;
  }

  public ModuleContext ModuleContext => this._modContext;

  public Scope GlobalScope => this._modContext.GlobalScope;

  public PythonContext LanguageContext => this._modContext.Context;

  internal PythonDictionary GlobalDict => this._modContext.Globals;

  internal bool ShowCls
  {
    get => this.ModuleContext.ShowCls;
    set => this.ModuleContext.ShowCls = value;
  }

  internal bool TryLookupName(string name, out object value)
  {
    string key = name;
    return this._dict.TryGetValue((object) key, out value) || this._modContext.Globals.TryGetValue((object) key, out value);
  }

  internal bool TryLookupBuiltin(string name, out object value)
  {
    object obj;
    if (!this.GlobalDict.TryGetValue((object) "__builtins__", out obj))
    {
      value = (object) null;
      return false;
    }
    if (obj is PythonModule pythonModule && pythonModule.__dict__.TryGetValue((object) name, out value) || obj is PythonDictionary pythonDictionary && pythonDictionary.TryGetValue((object) name, out value))
      return true;
    value = (object) null;
    return false;
  }

  internal PythonDictionary Dict => this._dict;

  internal bool TryGetVariable(string name, out object value)
  {
    return this.Dict.TryGetValue((object) name, out value);
  }

  internal bool TryRemoveVariable(string name) => this.Dict.Remove((object) name);

  internal void SetVariable(string name, object value) => this.Dict.Add((object) name, value);

  internal bool TryGetGlobalVariable(string name, out object res)
  {
    return this.GlobalDict.TryGetValue((object) name, out res);
  }

  internal void SetGlobalVariable(string name, object value)
  {
    this.GlobalDict.Add((object) name, value);
  }

  internal bool TryRemoveGlobalVariable(string name) => this.GlobalDict.Remove((object) name);

  internal PythonGlobal[] GetGlobalArray() => ((GlobalDictionaryStorage) this._dict._storage).Data;

  internal bool IsTopLevel => this.Dict != this.ModuleContext.Globals;

  internal PythonDictionary GetBuiltinsDict()
  {
    object obj;
    if (!this.GlobalDict._storage.TryGetBuiltins(out obj))
      return (PythonDictionary) null;
    return obj is PythonModule pythonModule ? pythonModule.__dict__ : obj as PythonDictionary;
  }

  internal PythonModule Module => this._modContext.Module;

  internal string ModuleName => this.Module.GetName();

  internal class DebugProxy
  {
    private readonly CodeContext _context;

    public DebugProxy(CodeContext context) => this._context = context;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public PythonModule Members => this._context.Module;
  }
}
