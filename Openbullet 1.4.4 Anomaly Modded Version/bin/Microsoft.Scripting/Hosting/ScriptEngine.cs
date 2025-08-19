// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ScriptEngine
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Hosting;

[DebuggerDisplay("{Setup.DisplayName}")]
public sealed class ScriptEngine : MarshalByRefObject
{
  private readonly LanguageContext _language;
  private readonly ScriptRuntime _runtime;
  private LanguageSetup _config;
  private ObjectOperations _operations;

  internal ScriptEngine(ScriptRuntime runtime, LanguageContext context)
  {
    this._runtime = runtime;
    this._language = context;
  }

  public ObjectOperations Operations
  {
    get
    {
      if (this._operations == null)
        Interlocked.CompareExchange<ObjectOperations>(ref this._operations, this.CreateOperations(), (ObjectOperations) null);
      return this._operations;
    }
  }

  public ObjectOperations CreateOperations()
  {
    return new ObjectOperations(new DynamicOperations(this._language), this);
  }

  public ObjectOperations CreateOperations(ScriptScope scope)
  {
    ContractUtils.RequiresNotNull((object) scope, nameof (scope));
    return new ObjectOperations(this._language.Operations, this);
  }

  public object Execute(string expression)
  {
    return this.CreateScriptSourceFromString(expression).Execute();
  }

  public object Execute(string expression, ScriptScope scope)
  {
    return this.CreateScriptSourceFromString(expression).Execute(scope);
  }

  public T Execute<T>(string expression) => this.Operations.ConvertTo<T>(this.Execute(expression));

  public T Execute<T>(string expression, ScriptScope scope)
  {
    return this.Operations.ConvertTo<T>(this.Execute(expression, scope));
  }

  public ScriptScope ExecuteFile(string path) => this.ExecuteFile(path, this.CreateScope());

  public ScriptScope ExecuteFile(string path, ScriptScope scope)
  {
    this.CreateScriptSourceFromFile(path).Execute(scope);
    return scope;
  }

  public ObjectHandle ExecuteAndWrap(string expression, ScriptScope scope)
  {
    return new ObjectHandle(this.Execute(expression, scope));
  }

  public ObjectHandle ExecuteAndWrap(string expression)
  {
    return new ObjectHandle(this.Execute(expression));
  }

  [Obsolete("Use ScriptSource.ExecuteAndWrap instead")]
  public ObjectHandle ExecuteAndWrap(
    string expression,
    ScriptScope scope,
    out ObjectHandle exception)
  {
    exception = (ObjectHandle) null;
    try
    {
      return new ObjectHandle(this.Execute(expression, scope));
    }
    catch (Exception ex)
    {
      exception = new ObjectHandle((object) ex);
      return (ObjectHandle) null;
    }
  }

  [Obsolete("Use ScriptSource.ExecuteAndWrap instead")]
  public ObjectHandle ExecuteAndWrap(string expression, out ObjectHandle exception)
  {
    exception = (ObjectHandle) null;
    try
    {
      return new ObjectHandle(this.Execute(expression));
    }
    catch (Exception ex)
    {
      exception = new ObjectHandle((object) ex);
      return (ObjectHandle) null;
    }
  }

  public ScriptScope CreateScope() => new ScriptScope(this, this._language.CreateScope());

  public ScriptScope CreateScope(IDictionary<string, object> dictionary)
  {
    ContractUtils.RequiresNotNull((object) dictionary, nameof (dictionary));
    return new ScriptScope(this, this._language.CreateScope(dictionary));
  }

  public ScriptScope CreateScope(IDynamicMetaObjectProvider storage)
  {
    ContractUtils.RequiresNotNull((object) storage, nameof (storage));
    return new ScriptScope(this, this._language.CreateScope(storage));
  }

  public ScriptScope GetScope(string path)
  {
    ContractUtils.RequiresNotNull((object) path, nameof (path));
    Scope scope = this._language.GetScope(path);
    return scope == null ? (ScriptScope) null : new ScriptScope(this, scope);
  }

  public ScriptSource CreateScriptSourceFromString(string expression)
  {
    ContractUtils.RequiresNotNull((object) expression, nameof (expression));
    return this.CreateScriptSource((TextContentProvider) new SourceStringContentProvider(expression), (string) null, SourceCodeKind.AutoDetect);
  }

  public ScriptSource CreateScriptSourceFromString(string code, SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) code, nameof (code));
    ContractUtils.Requires(kind.IsValid(), nameof (kind));
    return this.CreateScriptSource((TextContentProvider) new SourceStringContentProvider(code), (string) null, kind);
  }

  public ScriptSource CreateScriptSourceFromString(string expression, string path)
  {
    ContractUtils.RequiresNotNull((object) expression, nameof (expression));
    return this.CreateScriptSource((TextContentProvider) new SourceStringContentProvider(expression), path, SourceCodeKind.AutoDetect);
  }

  public ScriptSource CreateScriptSourceFromString(string code, string path, SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) code, nameof (code));
    ContractUtils.Requires(kind.IsValid(), nameof (kind));
    return this.CreateScriptSource((TextContentProvider) new SourceStringContentProvider(code), path, kind);
  }

  public ScriptSource CreateScriptSourceFromFile(string path)
  {
    return this.CreateScriptSourceFromFile(path, StringUtils.DefaultEncoding, SourceCodeKind.File);
  }

  public ScriptSource CreateScriptSourceFromFile(string path, Encoding encoding)
  {
    return this.CreateScriptSourceFromFile(path, encoding, SourceCodeKind.File);
  }

  public ScriptSource CreateScriptSourceFromFile(
    string path,
    Encoding encoding,
    SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) path, nameof (path));
    ContractUtils.RequiresNotNull((object) encoding, nameof (encoding));
    ContractUtils.Requires(kind.IsValid(), nameof (kind));
    if (!this._language.CanCreateSourceCode)
      throw new NotSupportedException("Invariant engine cannot create scripts");
    return new ScriptSource(this, this._language.CreateFileUnit(path, encoding, kind));
  }

  public ScriptSource CreateScriptSource(CodeObject content)
  {
    return this.CreateScriptSource(content, (string) null, SourceCodeKind.File);
  }

  public ScriptSource CreateScriptSource(CodeObject content, string path)
  {
    return this.CreateScriptSource(content, path, SourceCodeKind.File);
  }

  public ScriptSource CreateScriptSource(CodeObject content, SourceCodeKind kind)
  {
    return this.CreateScriptSource(content, (string) null, kind);
  }

  public ScriptSource CreateScriptSource(CodeObject content, string path, SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) content, nameof (content));
    if (!this._language.CanCreateSourceCode)
      throw new NotSupportedException("Invariant engine cannot create scripts");
    return new ScriptSource(this, this._language.GenerateSourceCode(content, path, kind));
  }

  public ScriptSource CreateScriptSource(StreamContentProvider content, string path)
  {
    ContractUtils.RequiresNotNull((object) content, nameof (content));
    return this.CreateScriptSource(content, path, StringUtils.DefaultEncoding, SourceCodeKind.File);
  }

  public ScriptSource CreateScriptSource(
    StreamContentProvider content,
    string path,
    Encoding encoding)
  {
    ContractUtils.RequiresNotNull((object) content, nameof (content));
    ContractUtils.RequiresNotNull((object) encoding, nameof (encoding));
    return this.CreateScriptSource(content, path, encoding, SourceCodeKind.File);
  }

  public ScriptSource CreateScriptSource(
    StreamContentProvider content,
    string path,
    Encoding encoding,
    SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) content, nameof (content));
    ContractUtils.RequiresNotNull((object) encoding, nameof (encoding));
    ContractUtils.Requires(kind.IsValid(), nameof (kind));
    return this.CreateScriptSource((TextContentProvider) new LanguageBoundTextContentProvider(this._language, content, encoding, path), path, kind);
  }

  public ScriptSource CreateScriptSource(
    TextContentProvider contentProvider,
    string path,
    SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) contentProvider, nameof (contentProvider));
    ContractUtils.Requires(kind.IsValid(), nameof (kind));
    if (!this._language.CanCreateSourceCode)
      throw new NotSupportedException("Invariant engine cannot create scripts");
    return new ScriptSource(this, this._language.CreateSourceUnit(contentProvider, path, kind));
  }

  public TService GetService<TService>(params object[] args) where TService : class
  {
    if (typeof (TService) == typeof (TokenCategorizer))
    {
      TokenizerService service = this._language.GetService<TokenizerService>(ArrayUtils.Insert<object>((object) this._language, args));
      return service == null ? default (TService) : (TService) new TokenCategorizer(service);
    }
    if (typeof (TService) == typeof (ExceptionOperations))
    {
      ExceptionOperations service = this._language.GetService<ExceptionOperations>();
      return service == null ? (TService) new ExceptionOperations(this._language) : (TService) service;
    }
    if (!(typeof (TService) == typeof (DocumentationOperations)))
      return this._language.GetService<TService>(args);
    DocumentationProvider service1 = this._language.GetService<DocumentationProvider>(args);
    return service1 == null ? default (TService) : (TService) new DocumentationOperations(service1);
  }

  public LanguageSetup Setup
  {
    get
    {
      if (this._config == null)
      {
        LanguageConfiguration languageConfig = this._runtime.Manager.Configuration.GetLanguageConfig(this._language);
        foreach (LanguageSetup languageSetup in (IEnumerable<LanguageSetup>) this._runtime.Setup.LanguageSetups)
        {
          if (languageConfig.ProviderName == new AssemblyQualifiedTypeName(languageSetup.TypeName))
            return this._config = languageSetup;
        }
      }
      return this._config;
    }
  }

  public ScriptRuntime Runtime => this._runtime;

  public Version LanguageVersion => this._language.LanguageVersion;

  public CompilerOptions GetCompilerOptions() => this._language.GetCompilerOptions();

  public CompilerOptions GetCompilerOptions(ScriptScope scope)
  {
    return this._language.GetCompilerOptions(scope.Scope);
  }

  public void SetSearchPaths(ICollection<string> paths)
  {
    ContractUtils.RequiresNotNull((object) paths, nameof (paths));
    ContractUtils.RequiresNotNullItems<string>((IEnumerable<string>) paths, nameof (paths));
    this._language.SetSearchPaths(paths);
  }

  public ICollection<string> GetSearchPaths() => this._language.GetSearchPaths();

  internal LanguageContext LanguageContext => this._language;

  internal TRet Call<T, TRet>(Func<LanguageContext, T, TRet> f, T arg) => f(this._language, arg);

  public override object InitializeLifetimeService() => (object) null;
}
