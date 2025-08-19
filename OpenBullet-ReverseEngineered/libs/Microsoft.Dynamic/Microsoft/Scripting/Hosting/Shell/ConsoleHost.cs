// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.ConsoleHost
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell;

public abstract class ConsoleHost
{
  private int _exitCode;
  private ConsoleHostOptionsParser _optionsParser;
  private ScriptRuntime _runtime;
  private ScriptEngine _engine;
  private ConsoleOptions _consoleOptions;
  private IConsole _console;
  private CommandLine _commandLine;

  public ConsoleHostOptions Options => this._optionsParser.Options;

  public ScriptRuntimeSetup RuntimeSetup => this._optionsParser.RuntimeSetup;

  public ScriptEngine Engine
  {
    get => this._engine;
    protected set => this._engine = value;
  }

  public ScriptRuntime Runtime
  {
    get => this._runtime;
    protected set => this._runtime = value;
  }

  protected int ExitCode
  {
    get => this._exitCode;
    set => this._exitCode = value;
  }

  protected ConsoleHostOptionsParser ConsoleHostOptionsParser
  {
    get => this._optionsParser;
    set => this._optionsParser = value;
  }

  protected IConsole ConsoleIO
  {
    get => this._console;
    set => this._console = value;
  }

  protected CommandLine CommandLine => this._commandLine;

  protected virtual string ExeName
  {
    get
    {
      Assembly entryAssembly = Assembly.GetEntryAssembly();
      return !(entryAssembly != (Assembly) null) ? nameof (ConsoleHost) : entryAssembly.GetName().Name;
    }
  }

  protected virtual void ParseHostOptions(string[] args) => this._optionsParser.Parse(args);

  protected virtual ScriptRuntimeSetup CreateRuntimeSetup()
  {
    ScriptRuntimeSetup runtimeSetup = ScriptRuntimeSetup.ReadConfiguration();
    string provider = this.Provider.AssemblyQualifiedName;
    if (!runtimeSetup.LanguageSetups.Any<LanguageSetup>((Func<LanguageSetup, bool>) (s => s.TypeName == provider)))
    {
      LanguageSetup languageSetup = this.CreateLanguageSetup();
      if (languageSetup != null)
        runtimeSetup.LanguageSetups.Add(languageSetup);
    }
    return runtimeSetup;
  }

  protected virtual LanguageSetup CreateLanguageSetup() => (LanguageSetup) null;

  protected virtual PlatformAdaptationLayer PlatformAdaptationLayer
  {
    get => PlatformAdaptationLayer.Default;
  }

  protected virtual Type Provider => (Type) null;

  private string GetLanguageProvider(ScriptRuntimeSetup setup)
  {
    Type provider = this.Provider;
    if (provider != (Type) null)
      return provider.AssemblyQualifiedName;
    if (this.Options.HasLanguageProvider)
      return this.Options.LanguageProvider;
    if (this.Options.RunFile != null)
    {
      string ext = Path.GetExtension(this.Options.RunFile);
      foreach (LanguageSetup languageSetup in (IEnumerable<LanguageSetup>) setup.LanguageSetups)
      {
        if (languageSetup.FileExtensions.Any<string>((Func<string, bool>) (e => DlrConfiguration.FileExtensionComparer.Equals(e, ext))))
          return languageSetup.TypeName;
      }
    }
    throw new InvalidOptionException("No language specified.");
  }

  protected virtual CommandLine CreateCommandLine() => new CommandLine();

  protected virtual OptionsParser CreateOptionsParser()
  {
    return (OptionsParser) new OptionsParser<ConsoleOptions>();
  }

  protected virtual IConsole CreateConsole(
    ScriptEngine engine,
    CommandLine commandLine,
    ConsoleOptions options)
  {
    ContractUtils.RequiresNotNull((object) options, nameof (options));
    return options.TabCompletion ? ConsoleHost.CreateSuperConsole(commandLine, options.ColorfulConsole) : (IConsole) new BasicConsole(options.ColorfulConsole);
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static IConsole CreateSuperConsole(CommandLine commandLine, bool isColorful)
  {
    return (IConsole) new SuperConsole(commandLine, isColorful);
  }

  public virtual void Terminate(int exitCode) => this._commandLine.Terminate(exitCode);

  public virtual int Run(string[] args)
  {
    ScriptRuntimeSetup runtimeSetup = this.CreateRuntimeSetup();
    this._optionsParser = new ConsoleHostOptionsParser(new ConsoleHostOptions(), runtimeSetup);
    try
    {
      this.ParseHostOptions(args);
    }
    catch (InvalidOptionException ex)
    {
      Console.Error.WriteLine("Invalid argument: " + ex.Message);
      return this._exitCode = 1;
    }
    this.SetEnvironment();
    string languageProvider = this.GetLanguageProvider(runtimeSetup);
    LanguageSetup languageSetup1 = (LanguageSetup) null;
    foreach (LanguageSetup languageSetup2 in (IEnumerable<LanguageSetup>) runtimeSetup.LanguageSetups)
    {
      if (languageSetup2.TypeName == languageProvider)
        languageSetup1 = languageSetup2;
    }
    if (languageSetup1 == null)
    {
      languageSetup1 = new LanguageSetup(this.Provider.AssemblyQualifiedName, this.Provider.Name);
      runtimeSetup.LanguageSetups.Add(languageSetup1);
    }
    ConsoleHost.InsertSearchPaths(runtimeSetup.Options, (ICollection<string>) this.Options.SourceUnitSearchPaths);
    this._consoleOptions = this.ParseOptions(this.Options.IgnoredArgs.ToArray(), runtimeSetup, languageSetup1);
    if (this._consoleOptions == null)
      return this._exitCode = 1;
    this._runtime = new ScriptRuntime(runtimeSetup);
    try
    {
      this._engine = this._runtime.GetEngineByTypeName(languageProvider);
    }
    catch (Exception ex)
    {
      Console.Error.WriteLine(ex.Message);
      return this._exitCode = 1;
    }
    this.Execute();
    return this._exitCode;
  }

  protected virtual ConsoleOptions ParseOptions(
    string[] args,
    ScriptRuntimeSetup runtimeSetup,
    LanguageSetup languageSetup)
  {
    OptionsParser optionsParser = this.CreateOptionsParser();
    try
    {
      optionsParser.Parse(args, runtimeSetup, languageSetup, this.PlatformAdaptationLayer);
    }
    catch (InvalidOptionException ex)
    {
      this.ReportInvalidOption(ex);
      return (ConsoleOptions) null;
    }
    return optionsParser.CommonConsoleOptions;
  }

  protected virtual void ReportInvalidOption(InvalidOptionException e)
  {
    Console.Error.WriteLine(e.Message);
  }

  private static void InsertSearchPaths(
    IDictionary<string, object> options,
    ICollection<string> paths)
  {
    if (options == null || paths == null || paths.Count <= 0)
      return;
    List<string> stringList = new List<string>((IEnumerable<string>) ((object) LanguageOptions.GetSearchPathsOption(options) ?? (object) ArrayUtils.EmptyStrings));
    stringList.InsertRange(0, (IEnumerable<string>) paths);
    options["SearchPaths"] = (object) stringList;
  }

  protected virtual void PrintHelp() => Console.WriteLine(this.GetHelp());

  protected virtual string GetHelp()
  {
    StringBuilder output = new StringBuilder();
    string[,] help = this.Options.GetHelp();
    output.AppendLine($"Usage: {this.ExeName}.exe [<dlr-options>] [--] [<language-specific-command-line>]");
    output.AppendLine();
    output.AppendLine("DLR options (both slash or dash could be used to prefix options):");
    ArrayUtils.PrintTable(output, help);
    output.AppendLine();
    output.AppendLine("Language specific command line:");
    this.PrintLanguageHelp(output);
    output.AppendLine();
    return output.ToString();
  }

  public void PrintLanguageHelp(StringBuilder output)
  {
    ContractUtils.RequiresNotNull((object) output, nameof (output));
    string commandLine;
    string[,] options;
    string[,] environmentVariables;
    string comments;
    this.CreateOptionsParser().GetHelp(out commandLine, out options, out environmentVariables, out comments);
    if (commandLine == null && options == null && environmentVariables == null && comments == null)
      return;
    if (commandLine != null)
    {
      output.AppendLine(commandLine);
      output.AppendLine();
    }
    if (options != null)
    {
      output.AppendLine("Options:");
      ArrayUtils.PrintTable(output, options);
      output.AppendLine();
    }
    if (environmentVariables != null)
    {
      output.AppendLine("Environment variables:");
      ArrayUtils.PrintTable(output, environmentVariables);
      output.AppendLine();
    }
    if (comments != null)
    {
      output.Append(comments);
      output.AppendLine();
    }
    output.AppendLine();
  }

  private void Execute()
  {
    if (this._consoleOptions.IsMta)
    {
      Thread thread = new Thread(new ThreadStart(this.ExecuteInternal));
      thread.SetApartmentState(ApartmentState.MTA);
      thread.Start();
      thread.Join();
    }
    else
      this.ExecuteInternal();
  }

  protected virtual void ExecuteInternal()
  {
    if (this._consoleOptions.PrintVersion)
      this.PrintVersion();
    if (this._consoleOptions.PrintUsage)
      this.PrintUsage();
    if (this._consoleOptions.Exit)
    {
      this._exitCode = 0;
    }
    else
    {
      switch (this.Options.RunAction)
      {
        case ConsoleHostOptions.Action.None:
        case ConsoleHostOptions.Action.RunConsole:
          this._exitCode = this.RunCommandLine();
          break;
        case ConsoleHostOptions.Action.RunFile:
          this._exitCode = this.RunFile();
          break;
        default:
          throw Assert.Unreachable;
      }
    }
  }

  private void SetEnvironment()
  {
    foreach (string environmentVar in this.Options.EnvironmentVars)
    {
      if (!string.IsNullOrEmpty(environmentVar))
      {
        string[] strArray = environmentVar.Split('=');
        Environment.SetEnvironmentVariable(strArray[0], strArray.Length > 1 ? strArray[1] : "");
      }
    }
  }

  private int RunFile()
  {
    int num = 0;
    try
    {
      return this._engine.CreateScriptSourceFromFile(this.Options.RunFile).ExecuteProgram();
    }
    catch (Exception ex)
    {
      this.UnhandledException(this.Engine, ex);
      return 1;
    }
    finally
    {
      try
      {
        Snippets.SaveAndVerifyAssemblies();
      }
      catch (Exception ex)
      {
        num = 1;
      }
    }
  }

  private int RunCommandLine()
  {
    this._commandLine = this.CreateCommandLine();
    if (this._console == null)
      this._console = this.CreateConsole(this.Engine, this._commandLine, this._consoleOptions);
    int? nullable = new int?();
    try
    {
      if (this._consoleOptions.HandleExceptions)
      {
        try
        {
          this._commandLine.Run(this.Engine, this._console, this._consoleOptions);
        }
        catch (Exception ex)
        {
          if (CommandLine.IsFatalException(ex))
            throw;
          this.UnhandledException(this.Engine, ex);
        }
      }
      else
        this._commandLine.Run(this.Engine, this._console, this._consoleOptions);
    }
    finally
    {
      try
      {
        Snippets.SaveAndVerifyAssemblies();
      }
      catch (Exception ex)
      {
        nullable = new int?(1);
      }
    }
    return !nullable.HasValue ? this._commandLine.ExitCode : nullable.Value;
  }

  private void PrintUsage()
  {
    StringBuilder output = new StringBuilder();
    output.AppendFormat("Usage: {0}.exe ", (object) this.ExeName);
    this.PrintLanguageHelp(output);
    Console.Write(output.ToString());
  }

  protected void PrintVersion()
  {
    Console.WriteLine("{0} {1} on {2}", (object) this.Engine.Setup.DisplayName, (object) this.Engine.LanguageVersion, (object) ConsoleHost.GetRuntime());
  }

  private static string GetRuntime()
  {
    Type type = typeof (object).Assembly.GetType("Mono.Runtime");
    if (type != (Type) null)
      return (string) type.GetMethod("GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic).Invoke((object) null, (object[]) null);
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".NET {0}", (object) Environment.Version);
  }

  protected virtual void UnhandledException(ScriptEngine engine, Exception e)
  {
    Console.Error.Write("Unhandled exception");
    Console.Error.WriteLine(':');
    Console.Error.WriteLine(engine.GetService<ExceptionOperations>().FormatException(e));
  }

  protected static void PrintException(TextWriter output, Exception e)
  {
    ContractUtils.RequiresNotNull((object) e, nameof (e));
    for (; e != null; e = e.InnerException)
      output.WriteLine((object) e);
  }
}
