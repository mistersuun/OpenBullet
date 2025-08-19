// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ScriptRuntimeSetup
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Hosting.Configuration;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#nullable disable
namespace Microsoft.Scripting.Hosting;

[Serializable]
public sealed class ScriptRuntimeSetup
{
  private Type _hostType;
  private IList<object> _hostArguments;
  private IList<LanguageSetup> _languageSetups;
  private bool _debugMode;
  private bool _privateBinding;
  private IDictionary<string, object> _options;
  private bool _frozen;

  public ScriptRuntimeSetup()
  {
    this._languageSetups = (IList<LanguageSetup>) new List<LanguageSetup>();
    this._options = (IDictionary<string, object>) new Dictionary<string, object>();
    this._hostType = typeof (ScriptHost);
    this._hostArguments = (IList<object>) ArrayUtils.EmptyObjects;
  }

  public IList<LanguageSetup> LanguageSetups => this._languageSetups;

  public bool DebugMode
  {
    get => this._debugMode;
    set
    {
      this.CheckFrozen();
      this._debugMode = value;
    }
  }

  public bool PrivateBinding
  {
    get => this._privateBinding;
    set
    {
      this.CheckFrozen();
      this._privateBinding = value;
    }
  }

  public Type HostType
  {
    get => this._hostType;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      ContractUtils.Requires(typeof (ScriptHost).IsAssignableFrom(value), nameof (value), "Must be ScriptHost or a derived type of ScriptHost");
      this.CheckFrozen();
      this._hostType = value;
    }
  }

  public IDictionary<string, object> Options => this._options;

  public IList<object> HostArguments
  {
    get => this._hostArguments;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this.CheckFrozen();
      this._hostArguments = value;
    }
  }

  internal DlrConfiguration ToConfiguration()
  {
    ContractUtils.Requires(this._languageSetups.Count > 0, "ScriptRuntimeSetup must have at least one LanguageSetup");
    ReadOnlyCollection<LanguageSetup> setups = new ReadOnlyCollection<LanguageSetup>((IList<LanguageSetup>) ArrayUtils.MakeArray<LanguageSetup>((ICollection<LanguageSetup>) this._languageSetups));
    ReadOnlyCollection<object> readOnlyCollection = new ReadOnlyCollection<object>((IList<object>) ArrayUtils.MakeArray<object>((ICollection<object>) this._hostArguments));
    ReadOnlyDictionary<string, object> options = new ReadOnlyDictionary<string, object>((IDictionary<string, object>) new Dictionary<string, object>(this._options));
    DlrConfiguration configuration = new DlrConfiguration(this._debugMode, this._privateBinding, (IDictionary<string, object>) options);
    foreach (LanguageSetup languageSetup in setups)
      configuration.AddLanguage(languageSetup.TypeName, languageSetup.DisplayName, languageSetup.Names, languageSetup.FileExtensions, languageSetup.Options);
    this._languageSetups = (IList<LanguageSetup>) setups;
    this._options = (IDictionary<string, object>) options;
    this._hostArguments = (IList<object>) readOnlyCollection;
    this.Freeze(setups);
    return configuration;
  }

  private void Freeze(ReadOnlyCollection<LanguageSetup> setups)
  {
    foreach (LanguageSetup setup in setups)
      setup.Freeze();
    this._frozen = true;
  }

  private void CheckFrozen()
  {
    if (this._frozen)
      throw new InvalidOperationException("Cannot modify ScriptRuntimeSetup after it has been used to create a ScriptRuntime");
  }

  public static ScriptRuntimeSetup ReadConfiguration()
  {
    ScriptRuntimeSetup setup = new ScriptRuntimeSetup();
    Section.LoadRuntimeSetup(setup, (Stream) null);
    return setup;
  }

  public static ScriptRuntimeSetup ReadConfiguration(Stream configFileStream)
  {
    ContractUtils.RequiresNotNull((object) configFileStream, nameof (configFileStream));
    ScriptRuntimeSetup setup = new ScriptRuntimeSetup();
    Section.LoadRuntimeSetup(setup, configFileStream);
    return setup;
  }

  public static ScriptRuntimeSetup ReadConfiguration(string configFilePath)
  {
    ContractUtils.RequiresNotNull((object) configFilePath, nameof (configFilePath));
    using (FileStream configFileStream = File.OpenRead(configFilePath))
      return ScriptRuntimeSetup.ReadConfiguration((Stream) configFileStream);
  }
}
