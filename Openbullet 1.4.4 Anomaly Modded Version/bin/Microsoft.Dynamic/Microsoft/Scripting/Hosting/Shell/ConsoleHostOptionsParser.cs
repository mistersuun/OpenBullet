// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.ConsoleHostOptionsParser
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell;

public class ConsoleHostOptionsParser
{
  private readonly ConsoleHostOptions _options;
  private readonly ScriptRuntimeSetup _runtimeSetup;

  public ConsoleHostOptions Options => this._options;

  public ScriptRuntimeSetup RuntimeSetup => this._runtimeSetup;

  public ConsoleHostOptionsParser(ConsoleHostOptions options, ScriptRuntimeSetup runtimeSetup)
  {
    ContractUtils.RequiresNotNull((object) options, nameof (options));
    ContractUtils.RequiresNotNull((object) runtimeSetup, nameof (runtimeSetup));
    this._options = options;
    this._runtimeSetup = runtimeSetup;
  }

  public void Parse(string[] args)
  {
    ContractUtils.RequiresNotNull((object) args, nameof (args));
    int num = 0;
    while (num < args.Length)
    {
      string str1 = args[num++];
      string name;
      string value;
      this.ParseOption(str1, out name, out value);
      switch (name)
      {
        case "":
        case "/":
          while (num < args.Length)
            this._options.IgnoredArgs.Add(args[num++]);
          continue;
        case "console":
          this._options.RunAction = ConsoleHostOptions.Action.RunConsole;
          continue;
        case "lang":
          this.OptionValueRequired(name, value);
          string str2 = (string) null;
          foreach (LanguageSetup languageSetup in (IEnumerable<LanguageSetup>) this._runtimeSetup.LanguageSetups)
          {
            if (languageSetup.Names.Any<string>((Func<string, bool>) (n => DlrConfiguration.LanguageNameComparer.Equals(n, value))))
            {
              str2 = languageSetup.TypeName;
              break;
            }
          }
          this._options.LanguageProvider = str2 != null ? str2 : throw new InvalidOptionException($"Unknown language id '{value}'.");
          this._options.HasLanguageProvider = true;
          continue;
        case "path":
        case "paths":
          this.OptionValueRequired(name, value);
          this._options.SourceUnitSearchPaths = value.Split(';');
          continue;
        case "run":
          this.OptionValueRequired(name, value);
          this._options.RunAction = ConsoleHostOptions.Action.RunFile;
          this._options.RunFile = value;
          continue;
        case "setenv":
          this._options.EnvironmentVars.AddRange((IEnumerable<string>) value.Split(';'));
          continue;
        default:
          this._options.IgnoredArgs.Add(str1);
          goto case "";
      }
    }
  }

  private void ParseOption(string arg, out string name, out string value)
  {
    int length = arg.IndexOf(':');
    if (length >= 0)
    {
      name = arg.Substring(0, length);
      value = arg.Substring(length + 1);
    }
    else
    {
      name = arg;
      value = (string) null;
    }
    if (name.StartsWith("--"))
      name = name.Substring("--".Length);
    else if (name.StartsWith("-") && name.Length > 1)
      name = name.Substring("-".Length);
    else if (name.StartsWith("/") && name.Length > 1)
    {
      name = name.Substring("/".Length);
    }
    else
    {
      value = name;
      name = (string) null;
    }
    name = name?.ToLower(CultureInfo.InvariantCulture);
  }

  protected void OptionValueRequired(string optionName, string value)
  {
    if (value == null)
      throw new InvalidOptionException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Argument expected for the {0} option.", (object) optionName));
  }
}
