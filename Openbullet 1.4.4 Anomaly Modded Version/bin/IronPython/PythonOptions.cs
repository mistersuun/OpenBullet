// Decompiled with JetBrains decompiler
// Type: IronPython.PythonOptions
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using Microsoft.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

#nullable disable
namespace IronPython;

[CLSCompliant(true)]
[Serializable]
public sealed class PythonOptions : LanguageOptions
{
  private readonly ReadOnlyCollection<string> _arguments;
  private readonly ReadOnlyCollection<string> _warningFilters;
  private readonly bool _warnPy3k;
  private readonly bool _python30;
  private readonly bool _bytesWarning;
  private readonly bool _debug;
  private readonly int _recursionLimit;
  private readonly Severity _indentationInconsistencySeverity;
  private readonly PythonDivisionOptions _division;
  private readonly bool _stripDocStrings;
  private readonly bool _optimize;
  private readonly bool _inspect;
  private readonly bool _noUserSite;
  private readonly bool _noSite;
  private readonly bool _ignoreEnvironment;
  private readonly bool _verbose;
  private readonly bool _frames;
  private readonly bool _fullFrames;
  private readonly bool _tracing;
  private readonly Version _version;
  private readonly Regex _noDebug;
  private readonly int? _gcStress;
  private bool _enableProfiler;
  private readonly bool _lightweightScopes;

  public ReadOnlyCollection<string> Arguments => this._arguments;

  public bool Optimize => this._optimize;

  public bool StripDocStrings => this._stripDocStrings;

  public ReadOnlyCollection<string> WarningFilters => this._warningFilters;

  public bool WarnPython30 => this._warnPy3k;

  public bool Python30 => this._python30;

  public bool BytesWarning => this._bytesWarning;

  public bool Debug => this._debug;

  public bool Inspect => this._inspect;

  public bool NoUserSite => this._noUserSite;

  public bool NoSite => this._noSite;

  public bool IgnoreEnvironment => this._ignoreEnvironment;

  public bool Verbose => this._verbose;

  public int RecursionLimit => this._recursionLimit;

  public bool Frames => this._frames;

  public bool FullFrames => this._fullFrames;

  public bool Tracing => this._tracing;

  public Severity IndentationInconsistencySeverity => this._indentationInconsistencySeverity;

  [CLSCompliant(false)]
  public PythonDivisionOptions DivisionOptions => this._division;

  public bool LightweightScopes => this._lightweightScopes;

  public bool EnableProfiler
  {
    get => this._enableProfiler;
    set => this._enableProfiler = value;
  }

  public int? GCStress => this._gcStress;

  public Regex NoDebug => this._noDebug;

  public PythonOptions()
    : this((IDictionary<string, object>) null)
  {
  }

  public Version PythonVersion => this._version;

  public PythonOptions(IDictionary<string, object> options)
    : base(PythonOptions.EnsureSearchPaths(options))
  {
    this._arguments = LanguageOptions.GetStringCollectionOption(options, nameof (Arguments)) ?? LanguageOptions.EmptyStringCollection;
    this._warningFilters = LanguageOptions.GetStringCollectionOption(options, nameof (WarningFilters), ';', ',') ?? LanguageOptions.EmptyStringCollection;
    this._warnPy3k = LanguageOptions.GetOption<bool>(options, "WarnPy3k", false);
    this._bytesWarning = LanguageOptions.GetOption<bool>(options, nameof (BytesWarning), false);
    this._debug = LanguageOptions.GetOption<bool>(options, nameof (Debug), false);
    this._inspect = LanguageOptions.GetOption<bool>(options, nameof (Inspect), false);
    this._noUserSite = LanguageOptions.GetOption<bool>(options, nameof (NoUserSite), false);
    this._noSite = LanguageOptions.GetOption<bool>(options, nameof (NoSite), false);
    this._ignoreEnvironment = LanguageOptions.GetOption<bool>(options, nameof (IgnoreEnvironment), false);
    this._verbose = LanguageOptions.GetOption<bool>(options, nameof (Verbose), false);
    this._optimize = LanguageOptions.GetOption<bool>(options, nameof (Optimize), false);
    this._stripDocStrings = LanguageOptions.GetOption<bool>(options, nameof (StripDocStrings), false);
    this._division = LanguageOptions.GetOption<PythonDivisionOptions>(options, nameof (DivisionOptions), PythonDivisionOptions.Old);
    this._recursionLimit = LanguageOptions.GetOption<int>(options, nameof (RecursionLimit), int.MaxValue);
    this._indentationInconsistencySeverity = LanguageOptions.GetOption<Severity>(options, nameof (IndentationInconsistencySeverity), Severity.Ignore);
    this._enableProfiler = LanguageOptions.GetOption<bool>(options, nameof (EnableProfiler), false);
    this._lightweightScopes = LanguageOptions.GetOption<bool>(options, nameof (LightweightScopes), false);
    this._fullFrames = LanguageOptions.GetOption<bool>(options, nameof (FullFrames), false);
    this._frames = this._fullFrames || LanguageOptions.GetOption<bool>(options, nameof (Frames), true);
    this._gcStress = LanguageOptions.GetOption<int?>(options, nameof (GCStress), new int?());
    this._tracing = LanguageOptions.GetOption<bool>(options, nameof (Tracing), false);
    this._noDebug = LanguageOptions.GetOption<Regex>(options, nameof (NoDebug), (Regex) null);
    object version;
    if (options != null && options.TryGetValue(nameof (PythonVersion), out version))
    {
      if ((object) (version as Version) != null)
        this._version = (Version) version;
      else
        this._version = version is string ? new Version((string) version) : throw new ValueErrorException("Expected string or Version for PythonVersion");
      if (this._version != new Version(2, 7) && this._version != new Version(3, 0))
        throw new ValueErrorException("Expected Version to be 2.7 or 3.0");
    }
    else
      this._version = new Version(2, 7);
    this._python30 = this._version == new Version(3, 0);
  }

  private static IDictionary<string, object> EnsureSearchPaths(IDictionary<string, object> options)
  {
    if (options == null)
      return (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "SearchPaths",
          (object) new string[1]{ "." }
        }
      };
    if (!options.ContainsKey("SearchPaths"))
      options["SearchPaths"] = (object) new string[1]{ "." };
    return options;
  }
}
