// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ScriptRuntime
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Hosting;

public sealed class ScriptRuntime : MarshalByRefObject
{
  private readonly Dictionary<LanguageContext, ScriptEngine> _engines;
  private readonly InvariantContext _invariantContext;
  private readonly object _lock = new object();
  private ScriptScope _globals;
  private Scope _scopeGlobals;
  private ScriptEngine _invariantEngine;

  public ScriptRuntime(ScriptRuntimeSetup setup)
  {
    ContractUtils.RequiresNotNull((object) setup, nameof (setup));
    DlrConfiguration configuration = setup.ToConfiguration();
    this.Setup = setup;
    try
    {
      this.Host = (ScriptHost) Activator.CreateInstance(setup.HostType, setup.HostArguments.ToArray<object>());
    }
    catch (TargetInvocationException ex)
    {
      throw new InvalidImplementationException(Strings.InvalidCtorImplementation((object) setup.HostType, (object) ex.InnerException.Message), ex.InnerException);
    }
    catch (Exception ex)
    {
      throw new InvalidImplementationException(Strings.InvalidCtorImplementation((object) setup.HostType, (object) ex.Message), ex);
    }
    this.Manager = new ScriptDomainManager((DynamicRuntimeHostingProvider) new ScriptHostProxy(this.Host), configuration);
    this._invariantContext = new InvariantContext(this.Manager);
    this.IO = new ScriptIO(this.Manager.SharedIO);
    this._engines = new Dictionary<LanguageContext, ScriptEngine>();
    this._globals = new ScriptScope(this.GetEngineNoLockNoNotification((LanguageContext) this._invariantContext, out bool _), this.Manager.Globals);
    this.Host.SetRuntime(this);
    object obj;
    if (setup.Options.TryGetValue("NoDefaultReferences", out obj) && Convert.ToBoolean(obj))
      return;
    this.LoadAssembly(typeof (string).Assembly);
    this.LoadAssembly(typeof (Debug).Assembly);
  }

  internal ScriptDomainManager Manager { get; }

  public ScriptHost Host { get; }

  public ScriptIO IO { get; }

  public static ScriptRuntime CreateFromConfiguration()
  {
    return new ScriptRuntime(ScriptRuntimeSetup.ReadConfiguration());
  }

  public static ScriptRuntime CreateRemote(AppDomain domain, ScriptRuntimeSetup setup)
  {
    ContractUtils.RequiresNotNull((object) domain, nameof (domain));
    return (ScriptRuntime) domain.CreateInstanceAndUnwrap(typeof (ScriptRuntime).Assembly.FullName, typeof (ScriptRuntime).FullName, false, BindingFlags.Default, (Binder) null, new object[1]
    {
      (object) setup
    }, (CultureInfo) null, (object[]) null);
  }

  public override object InitializeLifetimeService() => (object) null;

  public ScriptRuntimeSetup Setup { get; }

  public ScriptEngine GetEngine(string languageName)
  {
    ContractUtils.RequiresNotNull((object) languageName, nameof (languageName));
    ScriptEngine engine;
    if (!this.TryGetEngine(languageName, out engine))
      throw new ArgumentException($"Unknown language name: '{languageName}'");
    return engine;
  }

  public ScriptEngine GetEngineByTypeName(string assemblyQualifiedTypeName)
  {
    ContractUtils.RequiresNotNull((object) assemblyQualifiedTypeName, nameof (assemblyQualifiedTypeName));
    return this.GetEngine(this.Manager.GetLanguageByTypeName(assemblyQualifiedTypeName));
  }

  public ScriptEngine GetEngineByFileExtension(string fileExtension)
  {
    ContractUtils.RequiresNotNull((object) fileExtension, nameof (fileExtension));
    ScriptEngine engine;
    if (!this.TryGetEngineByFileExtension(fileExtension, out engine))
      throw new ArgumentException($"Unknown file extension: '{fileExtension}'");
    return engine;
  }

  public bool TryGetEngine(string languageName, out ScriptEngine engine)
  {
    LanguageContext language;
    if (!this.Manager.TryGetLanguage(languageName, out language))
    {
      engine = (ScriptEngine) null;
      return false;
    }
    engine = this.GetEngine(language);
    return true;
  }

  public bool TryGetEngineByFileExtension(string fileExtension, out ScriptEngine engine)
  {
    LanguageContext language;
    if (!this.Manager.TryGetLanguageByFileExtension(fileExtension, out language))
    {
      engine = (ScriptEngine) null;
      return false;
    }
    engine = this.GetEngine(language);
    return true;
  }

  internal ScriptEngine GetEngine(LanguageContext language)
  {
    bool freshEngineCreated;
    ScriptEngine lockNoNotification;
    lock (this._engines)
      lockNoNotification = this.GetEngineNoLockNoNotification(language, out freshEngineCreated);
    if (freshEngineCreated && language != this._invariantContext)
      this.Host.EngineCreated(lockNoNotification);
    return lockNoNotification;
  }

  private ScriptEngine GetEngineNoLockNoNotification(
    LanguageContext language,
    out bool freshEngineCreated)
  {
    ScriptEngine lockNoNotification;
    if (freshEngineCreated = !this._engines.TryGetValue(language, out lockNoNotification))
    {
      lockNoNotification = new ScriptEngine(this, language);
      this._engines.Add(language, lockNoNotification);
    }
    return lockNoNotification;
  }

  public ScriptScope CreateScope() => this.InvariantEngine.CreateScope();

  public ScriptScope CreateScope(string languageId) => this.GetEngine(languageId).CreateScope();

  public ScriptScope CreateScope(IDynamicMetaObjectProvider storage)
  {
    return this.InvariantEngine.CreateScope(storage);
  }

  public ScriptScope CreateScope(string languageId, IDynamicMetaObjectProvider storage)
  {
    return this.GetEngine(languageId).CreateScope(storage);
  }

  public ScriptScope CreateScope(IDictionary<string, object> dictionary)
  {
    return this.InvariantEngine.CreateScope(dictionary);
  }

  public ScriptScope CreateScope(string languageId, IDictionary<string, object> storage)
  {
    return this.GetEngine(languageId).CreateScope(storage);
  }

  public ScriptScope ExecuteFile(string path)
  {
    ContractUtils.RequiresNotEmpty(path, nameof (path));
    string extension = Path.GetExtension(path);
    ScriptEngine engine;
    if (!this.TryGetEngineByFileExtension(extension, out engine))
      throw new ArgumentException($"File extension '{extension}' is not associated with any language.");
    return engine.ExecuteFile(path);
  }

  public ScriptScope UseFile(string path)
  {
    ContractUtils.RequiresNotEmpty(path, nameof (path));
    string extension = Path.GetExtension(path);
    ScriptEngine engine;
    if (!this.TryGetEngineByFileExtension(extension, out engine))
      throw new ArgumentException($"File extension '{extension}' is not associated with any language.");
    ICollection<string> searchPaths = engine.GetSearchPaths();
    if (searchPaths.Count == 0)
      throw new InvalidOperationException($"No search paths defined for language '{engine.Setup.DisplayName}'");
    foreach (string path1 in (IEnumerable<string>) searchPaths)
    {
      string path2 = Path.Combine(path1, path);
      ScriptScope scope = engine.GetScope(path2);
      if (scope != null)
        return scope;
    }
    foreach (string path1 in (IEnumerable<string>) searchPaths)
    {
      string path3 = Path.Combine(path1, path);
      if (this.Manager.Platform.FileExists(path3))
        return this.ExecuteFile(path3);
    }
    string str = searchPaths.Aggregate<string>((Func<string, string, string>) ((x, y) => $"{x}, {y}"));
    throw new FileNotFoundException($"File '{path}' not found in language's search path: {str}");
  }

  public ScriptScope Globals
  {
    get
    {
      Scope globals = this.Manager.Globals;
      if (this._scopeGlobals == globals)
        return this._globals;
      lock (this._lock)
      {
        if (this._scopeGlobals != globals)
        {
          this._globals = new ScriptScope(this.InvariantEngine, globals);
          this._scopeGlobals = globals;
        }
        return this._globals;
      }
    }
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      lock (this._lock)
      {
        this._globals = value;
        this.Manager.Globals = value.Scope;
      }
    }
  }

  public void LoadAssembly(Assembly assembly) => this.Manager.LoadAssembly(assembly);

  public ObjectOperations Operations => this.InvariantEngine.Operations;

  public ObjectOperations CreateOperations() => this.InvariantEngine.CreateOperations();

  public void Shutdown()
  {
    List<LanguageContext> languageContextList;
    lock (this._engines)
      languageContextList = new List<LanguageContext>((IEnumerable<LanguageContext>) this._engines.Keys);
    foreach (LanguageContext languageContext in languageContextList)
      languageContext.Shutdown();
  }

  internal ScriptEngine InvariantEngine
  {
    get
    {
      if (this._invariantEngine == null)
        this._invariantEngine = this.GetEngine((LanguageContext) this._invariantContext);
      return this._invariantEngine;
    }
  }
}
