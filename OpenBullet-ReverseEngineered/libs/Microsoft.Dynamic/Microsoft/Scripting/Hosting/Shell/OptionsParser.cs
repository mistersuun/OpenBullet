// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.OptionsParser
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell;

public abstract class OptionsParser
{
  private ScriptRuntimeSetup _runtimeSetup;
  private LanguageSetup _languageSetup;
  private PlatformAdaptationLayer _platform;
  private List<string> _ignoredArgs = new List<string>();
  private string[] _args;
  private int _current = -1;

  public ScriptRuntimeSetup RuntimeSetup => this._runtimeSetup;

  public LanguageSetup LanguageSetup => this._languageSetup;

  public PlatformAdaptationLayer Platform => this._platform;

  public abstract ConsoleOptions CommonConsoleOptions { get; }

  public IList<string> IgnoredArgs => (IList<string>) this._ignoredArgs;

  public void Parse(
    string[] args,
    ScriptRuntimeSetup setup,
    LanguageSetup languageSetup,
    PlatformAdaptationLayer platform)
  {
    ContractUtils.RequiresNotNull((object) args, nameof (args));
    ContractUtils.RequiresNotNull((object) setup, nameof (setup));
    ContractUtils.RequiresNotNull((object) languageSetup, nameof (languageSetup));
    ContractUtils.RequiresNotNull((object) platform, nameof (platform));
    this._args = args;
    this._runtimeSetup = setup;
    this._languageSetup = languageSetup;
    this._platform = platform;
    this._current = 0;
    try
    {
      this.BeforeParse();
      while (this._current < args.Length)
        this.ParseArgument(args[this._current++]);
      this.AfterParse();
    }
    finally
    {
      this._args = (string[]) null;
      this._runtimeSetup = (ScriptRuntimeSetup) null;
      this._languageSetup = (LanguageSetup) null;
      this._platform = (PlatformAdaptationLayer) null;
      this._current = -1;
    }
  }

  protected virtual void BeforeParse()
  {
  }

  protected virtual void AfterParse()
  {
  }

  protected abstract void ParseArgument(string arg);

  protected void IgnoreRemainingArgs()
  {
    while (this._current < this._args.Length)
      this._ignoredArgs.Add(this._args[this._current++]);
  }

  protected string[] PopRemainingArgs()
  {
    string[] strArray = ArrayUtils.ShiftLeft<string>(this._args, this._current);
    this._current = this._args.Length;
    return strArray;
  }

  protected string PeekNextArg()
  {
    if (this._current < this._args.Length)
      return this._args[this._current];
    throw new InvalidOptionException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Argument expected for the {0} option.", this._current > 0 ? (object) this._args[this._current - 1] : (object) string.Empty));
  }

  protected string PopNextArg()
  {
    string str = this.PeekNextArg();
    ++this._current;
    return str;
  }

  protected void PushArgBack() => --this._current;

  protected static Exception InvalidOptionValue(string option, string value)
  {
    return (Exception) new InvalidOptionException($"'{value}' is not a valid value for option '{option}'");
  }

  public abstract void GetHelp(
    out string commandLine,
    out string[,] options,
    out string[,] environmentVariables,
    out string comments);
}
