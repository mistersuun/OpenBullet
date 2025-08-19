// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.LanguageOptions
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

#nullable disable
namespace Microsoft.Scripting;

[Serializable]
public class LanguageOptions
{
  private bool _exceptionDetail;
  private bool _showClrExceptions;
  private bool _interpretedMode;
  private readonly bool _perfStats;
  private readonly bool _noAdaptiveCompilation;
  private readonly int _compilationThreshold;
  private readonly ReadOnlyCollection<string> _searchPaths;
  protected static readonly ReadOnlyCollection<string> EmptyStringCollection = new ReadOnlyCollection<string>((IList<string>) ArrayUtils.EmptyStrings);

  public bool NoAdaptiveCompilation => this._noAdaptiveCompilation;

  public int CompilationThreshold => this._compilationThreshold;

  [Obsolete("No longer used.")]
  public bool InterpretedMode
  {
    get => this._interpretedMode;
    set => this._interpretedMode = value;
  }

  public bool ExceptionDetail
  {
    get => this._exceptionDetail;
    set => this._exceptionDetail = value;
  }

  public bool ShowClrExceptions
  {
    get => this._showClrExceptions;
    set => this._showClrExceptions = value;
  }

  public bool PerfStats => this._perfStats;

  public ReadOnlyCollection<string> SearchPaths => this._searchPaths;

  public LanguageOptions()
    : this((IDictionary<string, object>) null)
  {
  }

  public LanguageOptions(IDictionary<string, object> options)
  {
    this._interpretedMode = LanguageOptions.GetOption<bool>(options, nameof (InterpretedMode), false);
    this._exceptionDetail = LanguageOptions.GetOption<bool>(options, nameof (ExceptionDetail), false);
    this._showClrExceptions = LanguageOptions.GetOption<bool>(options, nameof (ShowClrExceptions), false);
    this._perfStats = LanguageOptions.GetOption<bool>(options, nameof (PerfStats), false);
    this._noAdaptiveCompilation = LanguageOptions.GetOption<bool>(options, nameof (NoAdaptiveCompilation), false);
    this._compilationThreshold = LanguageOptions.GetOption<int>(options, nameof (CompilationThreshold), -1);
    this._searchPaths = LanguageOptions.GetSearchPathsOption(options) ?? new ReadOnlyCollection<string>((IList<string>) new string[0]);
  }

  public static T GetOption<T>(IDictionary<string, object> options, string name, T defaultValue)
  {
    object obj1;
    if (options == null || !options.TryGetValue(name, out obj1))
      return defaultValue;
    return obj1 is T obj2 ? obj2 : (T) Convert.ChangeType(obj1, typeof (T), (IFormatProvider) CultureInfo.CurrentCulture);
  }

  public static ReadOnlyCollection<string> GetStringCollectionOption(
    IDictionary<string, object> options,
    string name,
    params char[] separators)
  {
    object obj;
    if (options == null || !options.TryGetValue(name, out obj))
      return (ReadOnlyCollection<string>) null;
    switch (obj)
    {
      case ICollection<string> list:
        foreach (string str in (IEnumerable<string>) list)
        {
          if (str == null)
            throw new ArgumentException($"Invalid value for option {name}: collection shouldn't containt null items");
        }
        return new ReadOnlyCollection<string>((IList<string>) ArrayUtils.MakeArray<string>(list));
      case string str1:
        if (separators != null && separators.Length != 0)
          return new ReadOnlyCollection<string>((IList<string>) StringUtils.Split(str1, separators, int.MaxValue, StringSplitOptions.RemoveEmptyEntries));
        break;
    }
    throw new ArgumentException("Invalid value for option " + name);
  }

  public static ReadOnlyCollection<string> GetSearchPathsOption(IDictionary<string, object> options)
  {
    return LanguageOptions.GetStringCollectionOption(options, "SearchPaths", Path.PathSeparator);
  }
}
