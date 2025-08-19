// Decompiled with JetBrains decompiler
// Type: IronPython.Hosting.PythonService
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Threading;

#nullable disable
namespace IronPython.Hosting;

public sealed class PythonService : MarshalByRefObject
{
  private readonly ScriptEngine _engine;
  private readonly PythonContext _context;
  private ScriptScope _sys;
  private ScriptScope _builtins;
  private ScriptScope _clr;

  public PythonService(PythonContext context, ScriptEngine engine)
  {
    this._context = context;
    this._engine = engine;
  }

  public ScriptScope GetSystemState()
  {
    if (this._sys == null)
      Interlocked.CompareExchange<ScriptScope>(ref this._sys, HostingHelpers.CreateScriptScope(this._engine, this._context.SystemState.Scope), (ScriptScope) null);
    return this._sys;
  }

  public ScriptScope GetBuiltins()
  {
    if (this._builtins == null)
      Interlocked.CompareExchange<ScriptScope>(ref this._builtins, HostingHelpers.CreateScriptScope(this._engine, this._context.BuiltinModuleInstance.Scope), (ScriptScope) null);
    return this._builtins;
  }

  public ScriptScope GetClr()
  {
    if (this._clr == null)
      Interlocked.CompareExchange<ScriptScope>(ref this._clr, HostingHelpers.CreateScriptScope(this._engine, this._context.ClrModule.Scope), (ScriptScope) null);
    return this._clr;
  }

  public ScriptScope CreateModule(string name, string filename, string docString)
  {
    PythonModule module = new PythonModule();
    this._context.PublishModule(name, module);
    module.__init__(name, docString);
    module.__dict__[(object) "__file__"] = (object) filename;
    return HostingHelpers.CreateScriptScope(this._engine, module.Scope);
  }

  public ScriptScope ImportModule(ScriptEngine engine, string name)
  {
    if (Importer.ImportModule(this._context.SharedClsContext, (object) this._context.SharedClsContext.GlobalDict, name, false, -1) is PythonModule pythonModule)
      return HostingHelpers.CreateScriptScope(engine, pythonModule.Scope);
    throw PythonOps.ImportError("no module named {0}", (object) name);
  }

  public string[] GetModuleFilenames()
  {
    List<string> stringList = new List<string>();
    if (this._engine.GetSysModule().GetVariable("modules") is PythonDictionary variable)
    {
      foreach (KeyValuePair<object, object> keyValuePair in variable)
      {
        string key = keyValuePair.Key as string;
        PythonModule pythonModule = keyValuePair.Value as PythonModule;
        object obj;
        if (key != null && pythonModule != null && pythonModule.Get__dict__().TryGetValue((object) "__file__", out obj) && obj != null)
          stringList.Add(key);
      }
    }
    return stringList.ToArray();
  }

  public void DispatchCommand(Action command) => this._context.DispatchCommand(command);

  public ObjectHandle GetSetCommandDispatcher(ObjectHandle dispatcher)
  {
    Action<Action> commandDispatcher = this._context.GetSetCommandDispatcher((Action<Action>) dispatcher.Unwrap());
    return commandDispatcher != null ? new ObjectHandle((object) commandDispatcher) : (ObjectHandle) null;
  }

  public ObjectHandle GetLocalCommandDispatcher()
  {
    return new ObjectHandle((object) (Action<Action>) (action => this._context.DispatchCommand(action)));
  }

  public override object InitializeLifetimeService() => this._engine.InitializeLifetimeService();
}
