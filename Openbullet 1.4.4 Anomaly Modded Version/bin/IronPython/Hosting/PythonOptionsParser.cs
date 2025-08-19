// Decompiled with JetBrains decompiler
// Type: IronPython.Hosting.PythonOptionsParser
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting.Shell;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#nullable disable
namespace IronPython.Hosting;

public sealed class PythonOptionsParser : OptionsParser<PythonConsoleOptions>
{
  private List<string> _warningFilters;

  protected override void ParseArgument(string arg)
  {
    ContractUtils.RequiresNotNull((object) arg, nameof (arg));
    switch (arg)
    {
      case "-":
        this.PushArgBack();
        this.LanguageSetup.Options["Arguments"] = (object) this.PopRemainingArgs();
        break;
      case "-3":
        this.LanguageSetup.Options["WarnPy3k"] = ScriptingRuntimeHelpers.True;
        break;
      case "-?":
        this.ConsoleOptions.PrintUsage = true;
        this.ConsoleOptions.Exit = true;
        break;
      case "-B":
        break;
      case "-E":
        this.ConsoleOptions.IgnoreEnvironmentVariables = true;
        this.LanguageSetup.Options["IgnoreEnvironment"] = ScriptingRuntimeHelpers.True;
        break;
      case "-O":
        this.LanguageSetup.Options["Optimize"] = ScriptingRuntimeHelpers.True;
        break;
      case "-OO":
        this.LanguageSetup.Options["Optimize"] = ScriptingRuntimeHelpers.True;
        this.LanguageSetup.Options["StripDocStrings"] = ScriptingRuntimeHelpers.True;
        break;
      case "-Q":
        this.LanguageSetup.Options["DivisionOptions"] = (object) PythonOptionsParser.ToDivisionOptions(this.PopNextArg());
        break;
      case "-Qnew":
      case "-Qold":
      case "-Qwarn":
      case "-Qwarnall":
        this.LanguageSetup.Options["DivisionOptions"] = (object) PythonOptionsParser.ToDivisionOptions(arg.Substring(2));
        break;
      case "-S":
        this.ConsoleOptions.SkipImportSite = true;
        this.LanguageSetup.Options["NoSite"] = ScriptingRuntimeHelpers.True;
        break;
      case "-U":
        break;
      case "-V":
        this.ConsoleOptions.PrintVersion = true;
        this.ConsoleOptions.Exit = true;
        this.IgnoreRemainingArgs();
        break;
      case "-W":
        if (this._warningFilters == null)
          this._warningFilters = new List<string>();
        this._warningFilters.Add(this.PopNextArg());
        break;
      case "-X:BasicConsole":
        this.ConsoleOptions.BasicConsole = true;
        break;
      case "-X:Debug":
        this.RuntimeSetup.DebugMode = true;
        this.LanguageSetup.Options["Debug"] = ScriptingRuntimeHelpers.True;
        break;
      case "-X:EnableProfiler":
        this.LanguageSetup.Options["EnableProfiler"] = ScriptingRuntimeHelpers.True;
        break;
      case "-X:Frames":
        if (this.LanguageSetup.Options.ContainsKey("Frames") && this.LanguageSetup.Options["Frames"] != ScriptingRuntimeHelpers.True)
          throw new InvalidOptionException("Only one of -X:[Full]Frames/-X:NoFrames may be specified");
        this.LanguageSetup.Options["Frames"] = ScriptingRuntimeHelpers.True;
        break;
      case "-X:FullFrames":
        if (this.LanguageSetup.Options.ContainsKey("Frames") && this.LanguageSetup.Options["Frames"] != ScriptingRuntimeHelpers.True)
          throw new InvalidOptionException("Only one of -X:[Full]Frames/-X:NoFrames may be specified");
        this.LanguageSetup.Options["Frames"] = this.LanguageSetup.Options["FullFrames"] = ScriptingRuntimeHelpers.True;
        break;
      case "-X:GCStress":
        int result1;
        this.LanguageSetup.Options["GCStress"] = StringUtils.TryParseInt32(this.PopNextArg(), out result1) && result1 >= 0 && result1 <= GC.MaxGeneration ? (object) result1 : throw new InvalidOptionException($"The argument for the {arg} option must be between 0 and {GC.MaxGeneration}.");
        break;
      case "-X:LightweightScopes":
        this.LanguageSetup.Options["LightweightScopes"] = ScriptingRuntimeHelpers.True;
        break;
      case "-X:MTA":
        this.ConsoleOptions.IsMta = true;
        break;
      case "-X:MaxRecursion":
        int result2;
        this.LanguageSetup.Options["RecursionLimit"] = StringUtils.TryParseInt32(this.PopNextArg(), out result2) && result2 >= 10 ? (object) result2 : throw new InvalidOptionException($"The argument for the {arg} option must be an integer >= 10.");
        break;
      case "-X:NoDebug":
        string pattern = this.PopNextArg();
        try
        {
          this.LanguageSetup.Options["NoDebug"] = (object) new Regex(pattern);
          break;
        }
        catch
        {
          throw OptionsParser.InvalidOptionValue("-X:NoDebug", pattern);
        }
      case "-X:NoFrames":
        if (this.LanguageSetup.Options.ContainsKey("Frames") && this.LanguageSetup.Options["Frames"] != ScriptingRuntimeHelpers.False)
          throw new InvalidOptionException("Only one of -X:[Full]Frames/-X:NoFrames may be specified");
        this.LanguageSetup.Options["Frames"] = ScriptingRuntimeHelpers.False;
        break;
      case "-X:Python30":
        this.LanguageSetup.Options["PythonVersion"] = (object) new Version(3, 0);
        break;
      case "-X:Tracing":
        this.LanguageSetup.Options["Tracing"] = ScriptingRuntimeHelpers.True;
        break;
      case "-b":
        this.LanguageSetup.Options["BytesWarning"] = ScriptingRuntimeHelpers.True;
        break;
      case "-c":
        this.ConsoleOptions.Command = this.PeekNextArg();
        string[] strArray = this.PopRemainingArgs();
        strArray[0] = arg;
        this.LanguageSetup.Options["Arguments"] = (object) strArray;
        break;
      case "-d":
        break;
      case "-i":
        this.ConsoleOptions.Introspection = true;
        this.LanguageSetup.Options["Inspect"] = ScriptingRuntimeHelpers.True;
        break;
      case "-m":
        this.ConsoleOptions.ModuleToRun = this.PeekNextArg();
        this.LanguageSetup.Options["Arguments"] = (object) this.PopRemainingArgs();
        break;
      case "-s":
        this.LanguageSetup.Options["NoUserSite"] = ScriptingRuntimeHelpers.True;
        break;
      case "-t":
        this.LanguageSetup.Options["IndentationInconsistencySeverity"] = (object) Severity.Warning;
        break;
      case "-tt":
        this.LanguageSetup.Options["IndentationInconsistencySeverity"] = (object) Severity.Error;
        break;
      case "-u":
        break;
      case "-v":
        this.LanguageSetup.Options["Verbose"] = ScriptingRuntimeHelpers.True;
        break;
      case "-x":
        this.ConsoleOptions.SkipFirstSourceLine = true;
        break;
      default:
        if (arg.StartsWith("-W"))
        {
          if (this._warningFilters == null)
            this._warningFilters = new List<string>();
          this._warningFilters.Add(arg.Substring(2));
          break;
        }
        if (arg.StartsWith("-m"))
        {
          this.ConsoleOptions.ModuleToRun = arg.Substring(2);
          this.LanguageSetup.Options["Arguments"] = (object) this.PopRemainingArgs();
          break;
        }
        base.ParseArgument(arg);
        if (this.ConsoleOptions.FileName == null)
          break;
        this.PushArgBack();
        this.LanguageSetup.Options["Arguments"] = (object) this.PopRemainingArgs();
        break;
    }
  }

  protected override void AfterParse()
  {
    if (this._warningFilters == null)
      return;
    this.LanguageSetup.Options["WarningFilters"] = (object) this._warningFilters.ToArray();
  }

  private static PythonDivisionOptions ToDivisionOptions(string value)
  {
    switch (value)
    {
      case "old":
        return PythonDivisionOptions.Old;
      case "new":
        return PythonDivisionOptions.New;
      case "warn":
        return PythonDivisionOptions.Warn;
      case "warnall":
        return PythonDivisionOptions.WarnAll;
      default:
        throw OptionsParser.InvalidOptionValue("-Q", value);
    }
  }

  public override void GetHelp(
    out string commandLine,
    out string[,] options,
    out string[,] environmentVariables,
    out string comments)
  {
    string[,] options1;
    base.GetHelp(out commandLine, out options1, out environmentVariables, out comments);
    commandLine = "Usage: ipy [options] [file.py|- [arguments]]";
    string[,] strArray = ArrayUtils.Concatenate<string>(new string[27, 2]
    {
      {
        "-v",
        "Verbose (trace import statements) (also PYTHONVERBOSE=x)"
      },
      {
        "-m module",
        "run library module as a script"
      },
      {
        "-x",
        "Skip first line of the source"
      },
      {
        "-u",
        "Unbuffered stdout & stderr"
      },
      {
        "-O",
        "generate optimized code"
      },
      {
        "-OO",
        "remove doc strings and apply -O optimizations"
      },
      {
        "-E",
        "Ignore environment variables"
      },
      {
        "-Q arg",
        "Division options: -Qold (default), -Qwarn, -Qwarnall, -Qnew"
      },
      {
        "-S",
        "Don't imply 'import site' on initialization"
      },
      {
        "-s",
        "Don't add user site directory to sys.path"
      },
      {
        "-t",
        "Issue warnings about inconsistent tab usage"
      },
      {
        "-tt",
        "Issue errors for inconsistent tab usage"
      },
      {
        "-W arg",
        "Warning control (arg is action:message:category:module:lineno) also IRONPYTHONWARNINGS=arg"
      },
      {
        "-3",
        "Warn about Python 3.x incompatibilities"
      },
      {
        "-X:NoFrames",
        "Disable sys._getframe support, can improve execution speed"
      },
      {
        "-X:Frames",
        "Enable basic sys._getframe support"
      },
      {
        "-X:FullFrames",
        "Enable sys._getframe with access to locals"
      },
      {
        "-X:Tracing",
        "Enable support for tracing all methods even before sys.settrace is called"
      },
      {
        "-X:GCStress",
        "Specifies the GC stress level (the generation to collect each statement)"
      },
      {
        "-X:MaxRecursion",
        "Set the maximum recursion level"
      },
      {
        "-X:Debug",
        "Enable application debugging (preferred over -D)"
      },
      {
        "-X:NoDebug <regex>",
        "Provides a regular expression of files which should not be emitted in debug mode"
      },
      {
        "-X:MTA",
        "Run in multithreaded apartment"
      },
      {
        "-X:Python30",
        "Enable available Python 3.0 features"
      },
      {
        "-X:EnableProfiler",
        "Enables profiling support in the compiler"
      },
      {
        "-X:LightweightScopes",
        "Generate optimized scopes that can be garbage collected"
      },
      {
        "-X:BasicConsole",
        "Use only the basic console features"
      }
    }, options1);
    List<string> stringList = new List<string>();
    List<int> intList = new List<int>();
    for (int index = 0; index < strArray.Length / 2; ++index)
    {
      stringList.Add(strArray[index, 0]);
      intList.Add(index);
    }
    int[] array = intList.ToArray();
    Array.Sort<string, int>(stringList.ToArray(), array, (IComparer<string>) StringComparer.OrdinalIgnoreCase);
    options = new string[strArray.Length / 2, 2];
    for (int index = 0; index < array.Length; ++index)
    {
      options[index, 0] = strArray[array[index], 0];
      options[index, 1] = strArray[array[index], 1];
    }
    environmentVariables = new string[2, 2]
    {
      {
        "IRONPYTHONPATH",
        "Path to search for module"
      },
      {
        "IRONPYTHONSTARTUP",
        "Startup module"
      }
    };
  }
}
