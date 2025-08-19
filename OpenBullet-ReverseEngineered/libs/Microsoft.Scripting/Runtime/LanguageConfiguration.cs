// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.LanguageConfiguration
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

internal sealed class LanguageConfiguration
{
  private readonly IDictionary<string, object> _options;
  private LanguageContext _context;

  public LanguageContext LanguageContext => this._context;

  public AssemblyQualifiedTypeName ProviderName { get; }

  public string DisplayName { get; }

  public LanguageConfiguration(
    AssemblyQualifiedTypeName providerName,
    string displayName,
    IDictionary<string, object> options)
  {
    this.ProviderName = providerName;
    this.DisplayName = displayName;
    this._options = options;
  }

  internal LanguageContext LoadLanguageContext(
    ScriptDomainManager domainManager,
    out bool alreadyLoaded)
  {
    if (this._context == null)
    {
      Assembly assembly = domainManager.Platform.LoadAssembly(this.ProviderName.AssemblyName.FullName);
      Type type = assembly.GetType(this.ProviderName.TypeName);
      if (type == (Type) null)
        throw new InvalidOperationException($"Failed to load language '{this.DisplayName}': assembly '{assembly.Location}' does not contain type '{this.ProviderName.TypeName}'");
      if (!type.IsSubclassOf(typeof (LanguageContext)))
        throw new InvalidOperationException($"Failed to load language '{this.DisplayName}': type '{type}' is not a valid language provider because it does not inherit from LanguageContext");
      LanguageContext instance;
      try
      {
        instance = (LanguageContext) Activator.CreateInstance(type, (object) domainManager, (object) this._options);
      }
      catch (TargetInvocationException ex)
      {
        throw new TargetInvocationException($"Failed to load language '{this.DisplayName}': {ex.InnerException.Message}", ex.InnerException);
      }
      catch (Exception ex)
      {
        throw new InvalidImplementationException(Strings.InvalidCtorImplementation((object) type, (object) ex.Message), ex);
      }
      alreadyLoaded = Interlocked.CompareExchange<LanguageContext>(ref this._context, instance, (LanguageContext) null) != null;
    }
    else
      alreadyLoaded = true;
    return this._context;
  }
}
