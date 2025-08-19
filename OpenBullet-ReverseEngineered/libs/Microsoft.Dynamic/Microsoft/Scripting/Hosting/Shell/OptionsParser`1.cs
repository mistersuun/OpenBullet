// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.OptionsParser`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell;

public class OptionsParser<TConsoleOptions> : OptionsParser where TConsoleOptions : Microsoft.Scripting.Hosting.Shell.ConsoleOptions, new()
{
  private TConsoleOptions _consoleOptions;
  private bool _saveAssemblies;
  private string _assembliesDir;

  public TConsoleOptions ConsoleOptions
  {
    get
    {
      if ((object) this._consoleOptions == null)
        this._consoleOptions = new TConsoleOptions();
      return this._consoleOptions;
    }
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this._consoleOptions = value;
    }
  }

  public sealed override Microsoft.Scripting.Hosting.Shell.ConsoleOptions CommonConsoleOptions
  {
    get => (Microsoft.Scripting.Hosting.Shell.ConsoleOptions) this.ConsoleOptions;
  }

  protected override void ParseArgument(string arg)
  {
    ContractUtils.RequiresNotNull((object) arg, nameof (arg));
    switch (arg)
    {
      case "-?":
      case "-h":
      case "-help":
      case "/?":
        this.ConsoleOptions.PrintUsage = true;
        this.ConsoleOptions.Exit = true;
        this.IgnoreRemainingArgs();
        break;
      case "-D":
        this.RuntimeSetup.DebugMode = true;
        break;
      case "-X:AutoIndent":
        this.ConsoleOptions.AutoIndent = true;
        break;
      case "-X:ColorfulConsole":
        this.ConsoleOptions.ColorfulConsole = true;
        break;
      case "-X:CompilationThreshold":
        this.LanguageSetup.Options["CompilationThreshold"] = (object) int.Parse(this.PopNextArg());
        break;
      case "-X:ExceptionDetail":
      case "-X:ShowClrExceptions":
        this.LanguageSetup.Options[arg.Substring(3)] = ScriptingRuntimeHelpers.True;
        break;
      case "-X:Interpret":
        this.LanguageSetup.Options["InterpretedMode"] = ScriptingRuntimeHelpers.True;
        break;
      case "-X:NoAdaptiveCompilation":
        this.LanguageSetup.Options["NoAdaptiveCompilation"] = (object) true;
        break;
      case "-X:PassExceptions":
        this.ConsoleOptions.HandleExceptions = false;
        break;
      case "-X:PrivateBinding":
        this.RuntimeSetup.PrivateBinding = true;
        break;
      case "-X:RemoteRuntimeChannel":
        this.ConsoleOptions.RemoteRuntimeChannel = this.PopNextArg();
        break;
      case "-X:TabCompletion":
        this.ConsoleOptions.TabCompletion = true;
        break;
      default:
        this.ConsoleOptions.FileName = arg.Trim();
        break;
    }
    if (!this._saveAssemblies)
      return;
    Snippets.SetSaveAssemblies(true, this._assembliesDir);
  }

  internal static void SetDlrOption(string option)
  {
    OptionsParser<TConsoleOptions>.SetDlrOption(option, "true");
  }

  internal static void SetDlrOption(string option, string value)
  {
    Environment.SetEnvironmentVariable("DLR_" + option, value);
  }

  public override void GetHelp(
    out string commandLine,
    out string[,] options,
    out string[,] environmentVariables,
    out string comments)
  {
    commandLine = "[options] [file|- [arguments]]";
    options = new string[14, 2]
    {
      {
        "-c cmd",
        "Program passed in as string (terminates option list)"
      },
      {
        "-h",
        "Display usage"
      },
      {
        "-i",
        "Inspect interactively after running script"
      },
      {
        "-V",
        "Print the version number and exit"
      },
      {
        "-D",
        "Enable application debugging"
      },
      {
        "-X:AutoIndent",
        "Enable auto-indenting in the REPL loop"
      },
      {
        "-X:ExceptionDetail",
        "Enable ExceptionDetail mode"
      },
      {
        "-X:NoAdaptiveCompilation",
        "Disable adaptive compilation"
      },
      {
        "-X:CompilationThreshold",
        "The number of iterations before the interpreter starts compiling"
      },
      {
        "-X:PassExceptions",
        "Do not catch exceptions that are unhandled by script code"
      },
      {
        "-X:PrivateBinding",
        "Enable binding to private members"
      },
      {
        "-X:ShowClrExceptions",
        "Display CLS Exception information"
      },
      {
        "-X:TabCompletion",
        "Enable TabCompletion mode"
      },
      {
        "-X:ColorfulConsole",
        "Enable ColorfulConsole"
      }
    };
    environmentVariables = new string[0, 0];
    comments = (string) null;
  }
}
