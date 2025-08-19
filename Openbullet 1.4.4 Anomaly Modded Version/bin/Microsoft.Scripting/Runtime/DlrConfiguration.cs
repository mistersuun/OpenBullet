// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.DlrConfiguration
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class DlrConfiguration
{
  private bool _frozen;
  public static readonly StringComparer FileExtensionComparer = StringComparer.OrdinalIgnoreCase;
  public static readonly StringComparer LanguageNameComparer = StringComparer.OrdinalIgnoreCase;
  public static readonly StringComparer OptionNameComparer = StringComparer.Ordinal;
  private readonly Dictionary<string, LanguageConfiguration> _languageNames;
  private readonly Dictionary<string, LanguageConfiguration> _languageExtensions;
  private readonly Dictionary<AssemblyQualifiedTypeName, LanguageConfiguration> _languageConfigurations;
  private readonly Dictionary<Type, LanguageConfiguration> _loadedProviderTypes;

  public DlrConfiguration(bool debugMode, bool privateBinding, IDictionary<string, object> options)
  {
    ContractUtils.RequiresNotNull((object) options, nameof (options));
    this.DebugMode = debugMode;
    this.PrivateBinding = privateBinding;
    this.Options = options;
    this._languageNames = new Dictionary<string, LanguageConfiguration>((IEqualityComparer<string>) DlrConfiguration.LanguageNameComparer);
    this._languageExtensions = new Dictionary<string, LanguageConfiguration>((IEqualityComparer<string>) DlrConfiguration.FileExtensionComparer);
    this._languageConfigurations = new Dictionary<AssemblyQualifiedTypeName, LanguageConfiguration>();
    this._loadedProviderTypes = new Dictionary<Type, LanguageConfiguration>();
  }

  public bool DebugMode { get; }

  public bool PrivateBinding { get; }

  internal IDictionary<string, object> Options { get; }

  internal IDictionary<AssemblyQualifiedTypeName, LanguageConfiguration> Languages
  {
    get
    {
      return (IDictionary<AssemblyQualifiedTypeName, LanguageConfiguration>) this._languageConfigurations;
    }
  }

  public void AddLanguage(
    string languageTypeName,
    string displayName,
    IList<string> names,
    IList<string> fileExtensions,
    IDictionary<string, object> options)
  {
    this.AddLanguage(languageTypeName, displayName, names, fileExtensions, options, (string) null);
  }

  internal void AddLanguage(
    string languageTypeName,
    string displayName,
    IList<string> names,
    IList<string> fileExtensions,
    IDictionary<string, object> options,
    string paramName)
  {
    ContractUtils.Requires(!this._frozen, "Configuration cannot be modified once the runtime is initialized");
    ContractUtils.Requires(names.TrueForAll<string>((Predicate<string>) (id => !string.IsNullOrEmpty(id) && !this._languageNames.ContainsKey(id))), paramName ?? nameof (names), "Language name should not be null, empty or duplicated between languages");
    ContractUtils.Requires(fileExtensions.TrueForAll<string>((Predicate<string>) (ext => !string.IsNullOrEmpty(ext) && !this._languageExtensions.ContainsKey(ext))), paramName ?? nameof (fileExtensions), "File extension should not be null, empty or duplicated between languages");
    ContractUtils.RequiresNotNull((object) displayName, paramName ?? nameof (displayName));
    if (string.IsNullOrEmpty(displayName))
    {
      ContractUtils.Requires(names.Count > 0, paramName ?? nameof (displayName), "Must have a non-empty display name or a a non-empty list of language names");
      displayName = names[0];
    }
    AssemblyQualifiedTypeName qualifiedTypeName = AssemblyQualifiedTypeName.ParseArgument(languageTypeName, paramName ?? nameof (languageTypeName));
    if (this._languageConfigurations.ContainsKey(qualifiedTypeName))
      throw new ArgumentException($"Duplicate language with type name '{qualifiedTypeName}'", nameof (languageTypeName));
    Dictionary<string, object> options1 = new Dictionary<string, object>(this.Options);
    foreach (KeyValuePair<string, object> option in (IEnumerable<KeyValuePair<string, object>>) options)
      options1[option.Key] = option.Value;
    LanguageConfiguration languageConfiguration = new LanguageConfiguration(qualifiedTypeName, displayName, (IDictionary<string, object>) options1);
    this._languageConfigurations.Add(qualifiedTypeName, languageConfiguration);
    foreach (string name in (IEnumerable<string>) names)
      this._languageNames[name] = languageConfiguration;
    foreach (string fileExtension in (IEnumerable<string>) fileExtensions)
      this._languageExtensions[DlrConfiguration.NormalizeExtension(fileExtension)] = languageConfiguration;
  }

  internal static string NormalizeExtension(string extension)
  {
    return extension[0] != '.' ? "." + extension : extension;
  }

  internal void Freeze() => this._frozen = true;

  internal bool TryLoadLanguage(
    ScriptDomainManager manager,
    AssemblyQualifiedTypeName providerName,
    out LanguageContext language)
  {
    LanguageConfiguration config;
    if (this._languageConfigurations.TryGetValue(providerName, out config))
    {
      language = this.LoadLanguageContext(manager, config);
      return true;
    }
    language = (LanguageContext) null;
    return false;
  }

  internal bool TryLoadLanguage(
    ScriptDomainManager manager,
    string str,
    bool isExtension,
    out LanguageContext language)
  {
    LanguageConfiguration config;
    if ((isExtension ? this._languageExtensions : this._languageNames).TryGetValue(str, out config))
    {
      language = this.LoadLanguageContext(manager, config);
      return true;
    }
    language = (LanguageContext) null;
    return false;
  }

  private LanguageContext LoadLanguageContext(
    ScriptDomainManager manager,
    LanguageConfiguration config)
  {
    bool alreadyLoaded;
    LanguageContext languageContext = config.LoadLanguageContext(manager, out alreadyLoaded);
    if (!alreadyLoaded)
    {
      lock (this._loadedProviderTypes)
      {
        Type type = languageContext.GetType();
        LanguageConfiguration languageConfiguration;
        if (this._loadedProviderTypes.TryGetValue(type, out languageConfiguration))
          throw new InvalidOperationException($"Language implemented by type '{config.ProviderName}' has already been loaded using name '{languageConfiguration.ProviderName}'");
        this._loadedProviderTypes.Add(type, config);
      }
    }
    return languageContext;
  }

  public string[] GetLanguageNames(LanguageContext context)
  {
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, LanguageConfiguration> languageName in this._languageNames)
    {
      if (languageName.Value.LanguageContext == context)
        stringList.Add(languageName.Key);
    }
    return stringList.ToArray();
  }

  internal string[] GetLanguageNames(LanguageConfiguration config)
  {
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, LanguageConfiguration> languageName in this._languageNames)
    {
      if (languageName.Value == config)
        stringList.Add(languageName.Key);
    }
    return stringList.ToArray();
  }

  public string[] GetLanguageNames()
  {
    return ArrayUtils.MakeArray<string>((ICollection<string>) this._languageNames.Keys);
  }

  public string[] GetFileExtensions(LanguageContext context)
  {
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, LanguageConfiguration> languageExtension in this._languageExtensions)
    {
      if (languageExtension.Value.LanguageContext == context)
        stringList.Add(languageExtension.Key);
    }
    return stringList.ToArray();
  }

  internal string[] GetFileExtensions(LanguageConfiguration config)
  {
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, LanguageConfiguration> languageExtension in this._languageExtensions)
    {
      if (languageExtension.Value == config)
        stringList.Add(languageExtension.Key);
    }
    return stringList.ToArray();
  }

  public string[] GetFileExtensions()
  {
    return ArrayUtils.MakeArray<string>((ICollection<string>) this._languageExtensions.Keys);
  }

  internal LanguageConfiguration GetLanguageConfig(LanguageContext context)
  {
    foreach (LanguageConfiguration languageConfig in this._languageConfigurations.Values)
    {
      if (languageConfig.LanguageContext == context)
        return languageConfig;
    }
    return (LanguageConfiguration) null;
  }
}
