// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Providers.HostingHelpers
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Runtime.Remoting;

#nullable disable
namespace Microsoft.Scripting.Hosting.Providers;

public static class HostingHelpers
{
  public static ScriptDomainManager GetDomainManager(ScriptRuntime runtime)
  {
    ContractUtils.RequiresNotNull((object) runtime, nameof (runtime));
    return runtime.Manager;
  }

  public static LanguageContext GetLanguageContext(ScriptEngine engine)
  {
    ContractUtils.RequiresNotNull((object) engine, nameof (engine));
    return engine.LanguageContext;
  }

  public static SourceUnit GetSourceUnit(ScriptSource scriptSource)
  {
    ContractUtils.RequiresNotNull((object) scriptSource, nameof (scriptSource));
    return scriptSource.SourceUnit;
  }

  public static ScriptCode GetScriptCode(CompiledCode compiledCode)
  {
    ContractUtils.RequiresNotNull((object) compiledCode, nameof (compiledCode));
    return compiledCode.ScriptCode;
  }

  public static SharedIO GetSharedIO(ScriptIO io)
  {
    ContractUtils.RequiresNotNull((object) io, nameof (io));
    return io.SharedIO;
  }

  public static Scope GetScope(ScriptScope scriptScope)
  {
    ContractUtils.RequiresNotNull((object) scriptScope, nameof (scriptScope));
    return scriptScope.Scope;
  }

  public static ScriptScope CreateScriptScope(ScriptEngine engine, Scope scope)
  {
    ContractUtils.RequiresNotNull((object) engine, nameof (engine));
    ContractUtils.RequiresNotNull((object) scope, nameof (scope));
    ContractUtils.Requires(!RemotingServices.IsTransparentProxy((object) engine), nameof (engine), "The engine cannot be a transparent proxy");
    return new ScriptScope(engine, scope);
  }

  [Obsolete("You should implement a service via LanguageContext and call ScriptEngine.GetService")]
  public static TRet CallEngine<T, TRet>(
    ScriptEngine engine,
    Func<LanguageContext, T, TRet> f,
    T arg)
  {
    return engine.Call<T, TRet>(f, arg);
  }

  public static DocumentationOperations CreateDocumentationOperations(DocumentationProvider provider)
  {
    return new DocumentationOperations(provider);
  }
}
