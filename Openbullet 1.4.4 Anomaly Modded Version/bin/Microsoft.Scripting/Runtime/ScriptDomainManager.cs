// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ScriptDomainManager
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class ScriptDomainManager
{
  private List<Assembly> _loadedAssemblies = new List<Assembly>();
  private int _lastContextId;

  public PlatformAdaptationLayer Platform
  {
    get => this.Host.PlatformAdaptationLayer ?? throw new InvalidImplementationException();
  }

  public SharedIO SharedIO { get; }

  public DynamicRuntimeHostingProvider Host { get; }

  public DlrConfiguration Configuration { get; }

  public ScriptDomainManager(
    DynamicRuntimeHostingProvider hostingProvider,
    DlrConfiguration configuration)
  {
    ContractUtils.RequiresNotNull((object) hostingProvider, nameof (hostingProvider));
    ContractUtils.RequiresNotNull((object) configuration, nameof (configuration));
    configuration.Freeze();
    this.Host = hostingProvider;
    this.Configuration = configuration;
    this.SharedIO = new SharedIO();
    this.Globals = new Scope();
  }

  internal ContextId GenerateContextId()
  {
    return new ContextId(Interlocked.Increment(ref this._lastContextId));
  }

  public LanguageContext GetLanguage(Type providerType)
  {
    ContractUtils.RequiresNotNull((object) providerType, nameof (providerType));
    return this.GetLanguageByTypeName(providerType.AssemblyQualifiedName);
  }

  public LanguageContext GetLanguageByTypeName(string providerAssemblyQualifiedTypeName)
  {
    ContractUtils.RequiresNotNull((object) providerAssemblyQualifiedTypeName, nameof (providerAssemblyQualifiedTypeName));
    LanguageContext language;
    if (!this.Configuration.TryLoadLanguage(this, AssemblyQualifiedTypeName.ParseArgument(providerAssemblyQualifiedTypeName, nameof (providerAssemblyQualifiedTypeName)), out language))
      throw Error.UnknownLanguageProviderType();
    return language;
  }

  public bool TryGetLanguage(string languageName, out LanguageContext language)
  {
    ContractUtils.RequiresNotNull((object) languageName, nameof (languageName));
    return this.Configuration.TryLoadLanguage(this, languageName, false, out language);
  }

  public LanguageContext GetLanguageByName(string languageName)
  {
    LanguageContext language;
    if (!this.TryGetLanguage(languageName, out language))
      throw new ArgumentException($"Unknown language name: '{languageName}'");
    return language;
  }

  public bool TryGetLanguageByFileExtension(string fileExtension, out LanguageContext language)
  {
    ContractUtils.RequiresNotEmpty(fileExtension, nameof (fileExtension));
    return this.Configuration.TryLoadLanguage(this, DlrConfiguration.NormalizeExtension(fileExtension), true, out language);
  }

  public LanguageContext GetLanguageByExtension(string fileExtension)
  {
    LanguageContext language;
    if (!this.TryGetLanguageByFileExtension(fileExtension, out language))
      throw new ArgumentException($"Unknown file extension: '{fileExtension}'");
    return language;
  }

  public Scope Globals { get; set; }

  public event EventHandler<AssemblyLoadedEventArgs> AssemblyLoaded;

  public bool LoadAssembly(Assembly assembly)
  {
    ContractUtils.RequiresNotNull((object) assembly, nameof (assembly));
    lock (this._loadedAssemblies)
    {
      if (this._loadedAssemblies.Contains(assembly))
        return false;
      this._loadedAssemblies.Add(assembly);
    }
    EventHandler<AssemblyLoadedEventArgs> assemblyLoaded = this.AssemblyLoaded;
    if (assemblyLoaded != null)
      assemblyLoaded((object) this, new AssemblyLoadedEventArgs(assembly));
    return true;
  }

  public IList<Assembly> GetLoadedAssemblyList()
  {
    lock (this._loadedAssemblies)
      return (IList<Assembly>) this._loadedAssemblies.ToArray();
  }
}
