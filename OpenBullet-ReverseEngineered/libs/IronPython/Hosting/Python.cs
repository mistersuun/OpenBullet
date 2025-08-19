// Decompiled with JetBrains decompiler
// Type: IronPython.Hosting.Python
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Modules;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Hosting;

public static class Python
{
  public static ScriptRuntime CreateRuntime()
  {
    return new ScriptRuntime(Python.CreateRuntimeSetup((IDictionary<string, object>) null));
  }

  public static ScriptRuntime CreateRuntime(IDictionary<string, object> options)
  {
    return new ScriptRuntime(Python.CreateRuntimeSetup(options));
  }

  public static ScriptRuntime CreateRuntime(AppDomain domain)
  {
    ContractUtils.RequiresNotNull((object) domain, nameof (domain));
    return ScriptRuntime.CreateRemote(domain, Python.CreateRuntimeSetup((IDictionary<string, object>) null));
  }

  public static ScriptRuntime CreateRuntime(AppDomain domain, IDictionary<string, object> options)
  {
    ContractUtils.RequiresNotNull((object) domain, nameof (domain));
    return ScriptRuntime.CreateRemote(domain, Python.CreateRuntimeSetup(options));
  }

  public static ScriptEngine CreateEngine() => Python.GetEngine(Python.CreateRuntime());

  public static ScriptEngine CreateEngine(IDictionary<string, object> options)
  {
    return Python.GetEngine(Python.CreateRuntime(options));
  }

  public static ScriptEngine CreateEngine(AppDomain domain)
  {
    return Python.GetEngine(Python.CreateRuntime(domain));
  }

  public static ScriptEngine CreateEngine(AppDomain domain, IDictionary<string, object> options)
  {
    return Python.GetEngine(Python.CreateRuntime(domain, options));
  }

  public static ScriptEngine GetEngine(ScriptRuntime runtime)
  {
    return runtime.GetEngineByTypeName(typeof (PythonContext).AssemblyQualifiedName);
  }

  public static ScriptScope GetSysModule(this ScriptRuntime runtime)
  {
    ContractUtils.RequiresNotNull((object) runtime, nameof (runtime));
    return Python.GetEngine(runtime).GetSysModule();
  }

  public static ScriptScope GetSysModule(this ScriptEngine engine)
  {
    ContractUtils.RequiresNotNull((object) engine, nameof (engine));
    return Python.GetPythonService(engine).GetSystemState();
  }

  public static ScriptScope GetBuiltinModule(this ScriptRuntime runtime)
  {
    ContractUtils.RequiresNotNull((object) runtime, nameof (runtime));
    return Python.GetEngine(runtime).GetBuiltinModule();
  }

  public static ScriptScope GetBuiltinModule(this ScriptEngine engine)
  {
    ContractUtils.RequiresNotNull((object) engine, nameof (engine));
    return Python.GetPythonService(engine).GetBuiltins();
  }

  public static ScriptScope GetClrModule(this ScriptRuntime runtime)
  {
    ContractUtils.RequiresNotNull((object) runtime, nameof (runtime));
    return Python.GetEngine(runtime).GetClrModule();
  }

  public static ScriptScope GetClrModule(this ScriptEngine engine)
  {
    ContractUtils.RequiresNotNull((object) engine, nameof (engine));
    return Python.GetPythonService(engine).GetClr();
  }

  public static ScriptScope ImportModule(this ScriptRuntime runtime, string moduleName)
  {
    ContractUtils.RequiresNotNull((object) runtime, nameof (runtime));
    ContractUtils.RequiresNotNull((object) moduleName, nameof (moduleName));
    return Python.GetEngine(runtime).ImportModule(moduleName);
  }

  public static ScriptScope ImportModule(this ScriptEngine engine, string moduleName)
  {
    ContractUtils.RequiresNotNull((object) engine, nameof (engine));
    ContractUtils.RequiresNotNull((object) moduleName, nameof (moduleName));
    return Python.GetPythonService(engine).ImportModule(engine, moduleName);
  }

  public static void ImportModule(this ScriptScope scope, string moduleName)
  {
    ContractUtils.RequiresNotNull((object) scope, nameof (scope));
    ContractUtils.RequiresNotNull((object) moduleName, nameof (moduleName));
    scope.SetVariable(moduleName, (object) scope.Engine.ImportModule(moduleName));
  }

  public static void SetHostVariables(
    this ScriptRuntime runtime,
    string prefix,
    string executable,
    string version)
  {
    ContractUtils.RequiresNotNull((object) runtime, nameof (runtime));
    ContractUtils.RequiresNotNull((object) prefix, nameof (prefix));
    ContractUtils.RequiresNotNull((object) executable, nameof (executable));
    ContractUtils.RequiresNotNull((object) version, nameof (version));
    Python.GetPythonContext(Python.GetEngine(runtime)).SetHostVariables(prefix, executable, version);
  }

  public static void SetHostVariables(
    this ScriptEngine engine,
    string prefix,
    string executable,
    string version)
  {
    ContractUtils.RequiresNotNull((object) engine, nameof (engine));
    ContractUtils.RequiresNotNull((object) prefix, nameof (prefix));
    ContractUtils.RequiresNotNull((object) executable, nameof (executable));
    ContractUtils.RequiresNotNull((object) version, nameof (version));
    Python.GetPythonContext(engine).SetHostVariables(prefix, executable, version);
  }

  public static void SetTrace(this ScriptEngine engine, TracebackDelegate traceFunc)
  {
    SysModule.settrace(Python.GetPythonContext(engine).SharedContext, (object) traceFunc);
  }

  public static void SetTrace(this ScriptRuntime runtime, TracebackDelegate traceFunc)
  {
    Python.GetEngine(runtime).SetTrace(traceFunc);
  }

  public static void CallTracing(
    this ScriptRuntime runtime,
    object traceFunc,
    params object[] args)
  {
    Python.GetEngine(runtime).CallTracing(traceFunc, args);
  }

  public static void CallTracing(this ScriptEngine engine, object traceFunc, params object[] args)
  {
    SysModule.call_tracing(Python.GetPythonContext(engine).SharedContext, traceFunc, PythonTuple.MakeTuple(args));
  }

  public static ScriptRuntimeSetup CreateRuntimeSetup(IDictionary<string, object> options)
  {
    ScriptRuntimeSetup runtimeSetup = new ScriptRuntimeSetup();
    runtimeSetup.LanguageSetups.Add(Python.CreateLanguageSetup(options));
    if (options != null)
    {
      object obj;
      if (options.TryGetValue("Debug", out obj) && obj is bool flag1 && flag1)
        runtimeSetup.DebugMode = true;
      if (options.TryGetValue("PrivateBinding", out obj) && obj is bool flag2 && flag2)
        runtimeSetup.PrivateBinding = true;
    }
    return runtimeSetup;
  }

  public static LanguageSetup CreateLanguageSetup(IDictionary<string, object> options)
  {
    LanguageSetup languageSetup = new LanguageSetup(typeof (PythonContext).AssemblyQualifiedName, "IronPython 2.7.9", (IEnumerable<string>) "IronPython;Python;py".Split(';'), (IEnumerable<string>) ".py".Split(';'));
    if (options != null)
    {
      foreach (KeyValuePair<string, object> option in (IEnumerable<KeyValuePair<string, object>>) options)
        languageSetup.Options.Add(option.Key, option.Value);
    }
    return languageSetup;
  }

  public static ScriptScope CreateModule(this ScriptEngine engine, string name)
  {
    return Python.GetPythonService(engine).CreateModule(name, string.Empty, string.Empty);
  }

  public static ScriptScope CreateModule(this ScriptEngine engine, string name, string filename)
  {
    return Python.GetPythonService(engine).CreateModule(name, filename, string.Empty);
  }

  public static ScriptScope CreateModule(
    this ScriptEngine engine,
    string name,
    string filename,
    string docString)
  {
    return Python.GetPythonService(engine).CreateModule(name, filename, docString);
  }

  public static string[] GetModuleFilenames(this ScriptEngine engine)
  {
    return Python.GetPythonService(engine).GetModuleFilenames();
  }

  private static PythonService GetPythonService(ScriptEngine engine)
  {
    return engine.GetService<PythonService>((object) engine);
  }

  private static PythonContext GetPythonContext(ScriptEngine engine)
  {
    return HostingHelpers.GetLanguageContext(engine) as PythonContext;
  }
}
