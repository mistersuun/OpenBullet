// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.LanguageSetup
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

#nullable disable
namespace Microsoft.Scripting.Hosting;

[Serializable]
public sealed class LanguageSetup
{
  private string _typeName;
  private string _displayName;
  private IList<string> _names;
  private IList<string> _fileExtensions;
  private IDictionary<string, object> _options;
  private bool _frozen;
  private bool? _interpretedMode;
  private bool? _exceptionDetail;
  private bool? _perfStats;
  private bool? _noAdaptiveCompilation;

  public LanguageSetup(string typeName)
    : this(typeName, "", (IEnumerable<string>) ArrayUtils.EmptyStrings, (IEnumerable<string>) ArrayUtils.EmptyStrings)
  {
  }

  public LanguageSetup(string typeName, string displayName)
    : this(typeName, displayName, (IEnumerable<string>) ArrayUtils.EmptyStrings, (IEnumerable<string>) ArrayUtils.EmptyStrings)
  {
  }

  public LanguageSetup(
    string typeName,
    string displayName,
    IEnumerable<string> names,
    IEnumerable<string> fileExtensions)
  {
    ContractUtils.RequiresNotEmpty(typeName, nameof (typeName));
    ContractUtils.RequiresNotNull((object) displayName, nameof (displayName));
    ContractUtils.RequiresNotNull((object) names, nameof (names));
    ContractUtils.RequiresNotNull((object) fileExtensions, nameof (fileExtensions));
    this._typeName = typeName;
    this._displayName = displayName;
    this._names = (IList<string>) new List<string>(names);
    this._fileExtensions = (IList<string>) new List<string>(fileExtensions);
    this._options = (IDictionary<string, object>) new Dictionary<string, object>();
  }

  public T GetOption<T>(string name, T defaultValue)
  {
    object obj1;
    if (this._options == null || !this._options.TryGetValue(name, out obj1))
      return defaultValue;
    return obj1 is T obj2 ? obj2 : (T) Convert.ChangeType(obj1, typeof (T), (IFormatProvider) CultureInfo.CurrentCulture);
  }

  public string TypeName
  {
    get => this._typeName;
    set
    {
      ContractUtils.RequiresNotEmpty(value, nameof (value));
      this.CheckFrozen();
      this._typeName = value;
    }
  }

  public string DisplayName
  {
    get => this._displayName;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this.CheckFrozen();
      this._displayName = value;
    }
  }

  public IList<string> Names => this._names;

  public IList<string> FileExtensions => this._fileExtensions;

  public IDictionary<string, object> Options => this._options;

  [Obsolete("This option is ignored")]
  public bool InterpretedMode
  {
    get => this.GetCachedOption(nameof (InterpretedMode), ref this._interpretedMode);
    set
    {
      this.CheckFrozen();
      this.Options[nameof (InterpretedMode)] = (object) value;
    }
  }

  [Obsolete("Use Options[\"NoAdaptiveCompilation\"] instead.")]
  public bool NoAdaptiveCompilation
  {
    get => this.GetCachedOption(nameof (NoAdaptiveCompilation), ref this._noAdaptiveCompilation);
    set
    {
      this.CheckFrozen();
      this.Options[nameof (NoAdaptiveCompilation)] = (object) value;
    }
  }

  public bool ExceptionDetail
  {
    get => this.GetCachedOption(nameof (ExceptionDetail), ref this._exceptionDetail);
    set
    {
      this.CheckFrozen();
      this.Options[nameof (ExceptionDetail)] = (object) value;
    }
  }

  [Obsolete("Use Options[\"PerfStats\"] instead.")]
  public bool PerfStats
  {
    get => this.GetCachedOption(nameof (PerfStats), ref this._perfStats);
    set
    {
      this.CheckFrozen();
      this.Options[nameof (PerfStats)] = (object) value;
    }
  }

  private bool GetCachedOption(string name, ref bool? storage)
  {
    if (storage.HasValue)
      return storage.Value;
    if (!this._frozen)
      return this.GetOption<bool>(name, false);
    storage = new bool?(this.GetOption<bool>(name, false));
    return storage.Value;
  }

  internal void Freeze()
  {
    this._frozen = true;
    this._names = (IList<string>) new ReadOnlyCollection<string>((IList<string>) ArrayUtils.MakeArray<string>((ICollection<string>) this._names));
    this._fileExtensions = (IList<string>) new ReadOnlyCollection<string>((IList<string>) ArrayUtils.MakeArray<string>((ICollection<string>) this._fileExtensions));
    this._options = (IDictionary<string, object>) new ReadOnlyDictionary<string, object>((IDictionary<string, object>) new Dictionary<string, object>(this._options));
  }

  private void CheckFrozen()
  {
    if (this._frozen)
      throw new InvalidOperationException("Cannot modify LanguageSetup after it has been used to create a ScriptRuntime");
  }
}
