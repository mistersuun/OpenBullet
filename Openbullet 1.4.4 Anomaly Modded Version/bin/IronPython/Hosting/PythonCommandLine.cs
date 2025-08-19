// Decompiled with JetBrains decompiler
// Type: IronPython.Hosting.PythonCommandLine
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using IronPython.Modules;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

#nullable disable
namespace IronPython.Hosting;

public sealed class PythonCommandLine : CommandLine
{
  private PythonContext PythonContext => this.Language;

  private PythonConsoleOptions Options => (PythonConsoleOptions) base.Options;

  protected override string Logo => PythonCommandLine.GetLogoDisplay();

  public static string GetLogoDisplay()
  {
    return PythonContext.GetVersionString() + "\nType \"help\", \"copyright\", \"credits\" or \"license\" for more information.\n";
  }

  private int GetEffectiveExitCode(SystemExitException e)
  {
    object otherCode;
    int exitCode = e.GetExitCode(out otherCode);
    if (otherCode == null)
      return exitCode;
    this.Console.WriteLine(otherCode.ToString(), Style.Error);
    return exitCode;
  }

  protected override void Shutdown()
  {
    try
    {
      this.Language.Shutdown();
    }
    catch (Exception ex)
    {
      this.Console.WriteLine("", Style.Error);
      this.Console.WriteLine("Error in sys.exitfunc:", Style.Error);
      this.Console.Write(this.Language.FormatException(ex), Style.Error);
    }
  }

  protected override int Run()
  {
    if (this.Options.ModuleToRun != null)
    {
      object o;
      try
      {
        o = Importer.Import(this.PythonContext.SharedContext, "runpy", PythonTuple.EMPTY, 0);
      }
      catch (Exception ex)
      {
        this.Console.WriteLine("Could not import runpy module", Style.Error);
        return -1;
      }
      object boundAttr;
      try
      {
        boundAttr = PythonOps.GetBoundAttr(this.PythonContext.SharedContext, o, "run_module");
      }
      catch (Exception ex)
      {
        this.Console.WriteLine("Could not access runpy.run_module", Style.Error);
        return -1;
      }
      try
      {
        PythonOps.CallWithKeywordArgs(this.PythonContext.SharedContext, boundAttr, new object[3]
        {
          (object) this.Options.ModuleToRun,
          (object) "__main__",
          ScriptingRuntimeHelpers.True
        }, new string[2]{ "run_name", "alter_sys" });
      }
      catch (SystemExitException ex)
      {
        return this.GetEffectiveExitCode(ex);
      }
      return 0;
    }
    int num = base.Run();
    if (Environment.GetEnvironmentVariable("IRONPYTHONINSPECT") != null && !this.Options.Introspection)
      num = this.RunInteractiveLoop();
    return num;
  }

  protected override int RunInteractiveLoop()
  {
    ScriptScope sysModule = this.Engine.GetSysModule();
    sysModule.SetVariable("ps1", (object) ">>> ");
    sysModule.SetVariable("ps2", (object) "... ");
    return base.RunInteractiveLoop();
  }

  protected override void Initialize()
  {
    base.Initialize();
    this.Console.Output = (TextWriter) new OutputWriter(this.PythonContext, false);
    this.Console.ErrorOutput = (TextWriter) new OutputWriter(this.PythonContext, true);
    int count = this.PythonContext.PythonOptions.SearchPaths.Count;
    this.Language.DomainManager.LoadAssembly(typeof (string).Assembly);
    this.Language.DomainManager.LoadAssembly(typeof (Debug).Assembly);
    this.InitializePath(ref count);
    this.InitializeEnvironmentVariables();
    this.InitializeModules();
    this.InitializeExtensionDLLs();
    this.ImportSite();
    if (Environment.GetEnvironmentVariable("IRONPYTHONINSPECT") != null)
      this.Options.Introspection = true;
    string directory = ".";
    if (this.Options.Command == null && this.Options.FileName != null)
    {
      if (this.Options.FileName == "-")
      {
        this.Options.FileName = "<stdin>";
      }
      else
      {
        if (Directory.Exists(this.Options.FileName))
          this.Options.FileName = Path.Combine(this.Options.FileName, "__main__.py");
        if (!File.Exists(this.Options.FileName))
        {
          this.Console.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "File {0} does not exist.", (object) this.Options.FileName), Style.Error);
          Environment.Exit(1);
        }
        directory = Path.GetDirectoryName(this.Language.DomainManager.Platform.GetFullPath(this.Options.FileName));
      }
    }
    this.PythonContext.InsertIntoPath(0, directory);
    this.PythonContext.MainThread = Thread.CurrentThread;
  }

  protected override Scope CreateScope()
  {
    ModuleOptions moduleOptions = this.PythonContext.PythonOptions.DivisionOptions == PythonDivisionOptions.New ? ModuleOptions.TrueDivision : ModuleOptions.None;
    ModuleContext moduleContext = new ModuleContext(new PythonDictionary(), this.PythonContext);
    moduleContext.Features = moduleOptions;
    moduleContext.InitializeBuiltins(true);
    this.PythonContext.PublishModule("__main__", moduleContext.Module);
    moduleContext.Globals[(object) "__doc__"] = (object) null;
    moduleContext.Globals[(object) "__name__"] = (object) "__main__";
    return moduleContext.GlobalScope;
  }

  private void InitializePath(ref int pathIndex)
  {
    if (this.Options.IgnoreEnvironmentVariables)
      return;
    string environmentVariable = Environment.GetEnvironmentVariable("IRONPYTHONPATH");
    if (environmentVariable == null || environmentVariable.Length <= 0)
      return;
    string str = environmentVariable;
    char[] chArray = new char[1]{ Path.PathSeparator };
    foreach (string directory in str.Split(chArray))
      this.PythonContext.InsertIntoPath(pathIndex++, directory);
  }

  private void InitializeEnvironmentVariables()
  {
    if (this.Options.IgnoreEnvironmentVariables)
      return;
    string environmentVariable = Environment.GetEnvironmentVariable("IRONPYTHONWARNINGS");
    object obj = this.PythonContext.GetSystemStateValue("warnoptions");
    if (obj == null)
    {
      obj = (object) new IronPython.Runtime.List();
      this.PythonContext.SetSystemStateValue("warnoptions", obj);
    }
    if (!(obj is IronPython.Runtime.List list) || string.IsNullOrEmpty(environmentVariable))
      return;
    string str1 = environmentVariable;
    char[] chArray = new char[1]{ ',' };
    foreach (string str2 in str1.Split(chArray))
      list.Add((object) str2);
  }

  private void InitializeModules()
  {
    string str1 = "";
    string str2 = (string) null;
    Assembly entryAssembly = Assembly.GetEntryAssembly();
    if (entryAssembly != (Assembly) null)
    {
      str1 = entryAssembly.Location;
      str2 = Path.GetDirectoryName(str1);
    }
    while (str2 != null && !File.Exists(Path.Combine(str2, "Lib/os.py")))
      str2 = Path.GetDirectoryName(str2);
    this.PythonContext.SetHostVariables(str2 ?? "", str1, (string) null);
  }

  private void InitializeExtensionDLLs()
  {
    string path = Path.Combine(this.PythonContext.InitialPrefix, "DLLs");
    if (!Directory.Exists(path))
      return;
    foreach (string enumerateFile in Directory.EnumerateFiles(path, "*.dll"))
    {
      if (enumerateFile.ToLower().EndsWith(".dll"))
      {
        try
        {
          ClrModule.AddReferenceToFile(this.PythonContext.SharedContext, new FileInfo(enumerateFile).Name);
        }
        catch
        {
        }
      }
    }
  }

  private void ImportSite()
  {
    if (this.Options.SkipImportSite)
      return;
    try
    {
      Importer.ImportModule(this.PythonContext.SharedContext, (object) null, "site", false, -1);
    }
    catch (Exception ex)
    {
      this.Console.Write(this.Language.FormatException(ex), Style.Error);
    }
  }

  protected override int RunInteractive()
  {
    this.PrintLogo();
    if (this.Scope == null)
      this.Scope = this.CreateScope();
    try
    {
      this.RunStartup();
    }
    catch (SystemExitException ex)
    {
      return this.GetEffectiveExitCode(ex);
    }
    catch (Exception ex)
    {
    }
    ScriptScope sysModule = this.Engine.GetSysModule();
    sysModule.SetVariable("ps1", (object) ">>> ");
    sysModule.SetVariable("ps2", (object) "... ");
    return this.RunInteractiveLoop();
  }

  protected override string Prompt
  {
    get
    {
      object o;
      return this.Engine.GetSysModule().TryGetVariable("ps1", out o) ? PythonOps.ToString(((PythonScopeExtension) this.Scope.GetExtension(this.Language.ContextId)).ModuleContext.GlobalContext, o) : ">>> ";
    }
  }

  public override string PromptContinuation
  {
    get
    {
      object o;
      return this.Engine.GetSysModule().TryGetVariable("ps2", out o) ? PythonOps.ToString(((PythonScopeExtension) this.Scope.GetExtension(this.Language.ContextId)).ModuleContext.GlobalContext, o) : "... ";
    }
  }

  private void RunStartup()
  {
    if (this.Options.IgnoreEnvironmentVariables)
      return;
    string environmentVariable = Environment.GetEnvironmentVariable("IRONPYTHONSTARTUP");
    if (environmentVariable == null || environmentVariable.Length <= 0)
      return;
    if (this.Options.HandleExceptions)
    {
      try
      {
        this.ExecuteCommand(this.Engine.CreateScriptSourceFromFile(environmentVariable));
      }
      catch (Exception ex)
      {
        if (ex is SystemExitException)
          throw;
        this.Console.Write(this.Language.FormatException(ex), Style.Error);
      }
    }
    else
      this.ExecuteCommand(this.Engine.CreateScriptSourceFromFile(environmentVariable));
  }

  protected override int? TryInteractiveAction()
  {
    try
    {
      try
      {
        return this.TryInteractiveActionWorker();
      }
      finally
      {
        PythonOps.ClearCurrentException();
      }
    }
    catch (SystemExitException ex)
    {
      return new int?(this.GetEffectiveExitCode(ex));
    }
  }

  private int? TryInteractiveActionWorker()
  {
    int? nullable = new int?();
    try
    {
      nullable = this.RunOneInteraction();
    }
    catch (ThreadAbortException ex)
    {
      if (ex.ExceptionState is KeyboardInterruptException)
        Thread.ResetAbort();
    }
    return nullable;
  }

  private int? RunOneInteraction()
  {
    bool continueInteraction;
    string code = this.ReadStatement(out continueInteraction);
    if (!continueInteraction)
    {
      this.PythonContext.DispatchCommand((System.Action) null);
      return new int?(0);
    }
    if (string.IsNullOrEmpty(code))
    {
      this.Console.Write(string.Empty, Style.Out);
      return new int?();
    }
    SourceUnit su = this.Language.CreateSnippet(code, "<stdin>", SourceCodeKind.InteractiveCode);
    PythonCompilerOptions pco = (PythonCompilerOptions) this.Language.GetCompilerOptions(this.Scope);
    pco.Module |= ModuleOptions.ExecOrEvalCode;
    System.Action command = (System.Action) (() =>
    {
      try
      {
        su.Compile((CompilerOptions) pco, this.ErrorSink).Run(this.Scope);
      }
      catch (Exception ex)
      {
        if (ex is SystemExitException)
          throw;
        this.UnhandledException(ex);
      }
    });
    try
    {
      this.PythonContext.DispatchCommand(command);
    }
    catch (SystemExitException ex)
    {
      return new int?(this.GetEffectiveExitCode(ex));
    }
    return new int?();
  }

  protected override ErrorSink ErrorSink => (ErrorSink) IronPython.Runtime.ThrowingErrorSink.Default;

  protected override int GetNextAutoIndentSize(string text)
  {
    return Parser.GetNextAutoIndentSize(text, this.Options.AutoIndentSize);
  }

  protected override int RunCommand(string command)
  {
    if (!this.Options.HandleExceptions)
      return this.RunCommandWorker(command);
    try
    {
      return this.RunCommandWorker(command);
    }
    catch (Exception ex)
    {
      this.Console.WriteLine(this.Language.FormatException(ex), Style.Error);
      return 1;
    }
  }

  private int RunCommandWorker(string command)
  {
    ModuleOptions options = (ModuleOptions) (132 | (this.PythonContext.PythonOptions.DivisionOptions == PythonDivisionOptions.New ? 1 : 0));
    if (this.Options.SkipFirstSourceLine)
      options |= ModuleOptions.SkipFirstLine;
    ScriptCode scriptCode;
    PythonModule module = this.PythonContext.CompileModule("", "__main__", this.PythonContext.CreateSnippet(command, "-c", SourceCodeKind.File), options, out scriptCode);
    this.PythonContext.PublishModule("__main__", module);
    this.Scope = module.Scope;
    try
    {
      scriptCode.Run(this.Scope);
    }
    catch (SystemExitException ex)
    {
      this.Options.Introspection = false;
      return this.GetEffectiveExitCode(ex);
    }
    return 0;
  }

  protected override int RunFile(string fileName)
  {
    int num = 1;
    if (this.Options.HandleExceptions)
    {
      try
      {
        num = this.RunFileWorker(fileName);
      }
      catch (Exception ex)
      {
        this.Console.WriteLine(this.Language.FormatException(ex), Style.Error);
      }
    }
    else
      num = this.RunFileWorker(fileName);
    return num;
  }

  private int RunFileWorker(string fileName)
  {
    try
    {
      object importer;
      if (Importer.TryImportMainFromZip(DefaultContext.Default, fileName, out importer))
        return 0;
      if (importer != null)
      {
        if (importer.GetType() != typeof (PythonImport.NullImporter))
        {
          this.Console.WriteLine($"can't find '__main__' module in '{fileName}'", Style.Error);
          return 0;
        }
      }
    }
    catch (SystemExitException ex)
    {
      this.Options.Introspection = false;
      return this.GetEffectiveExitCode(ex);
    }
    ModuleOptions options = ModuleOptions.Optimized | ModuleOptions.ModuleBuiltins;
    if (this.Options.SkipFirstSourceLine)
      options |= ModuleOptions.SkipFirstLine;
    ScriptCode scriptCode;
    PythonModule module = this.PythonContext.CompileModule(fileName, "__main__", this.PythonContext.CreateFileUnit(string.IsNullOrEmpty(fileName) ? (string) null : fileName, this.PythonContext.DefaultEncoding), options, out scriptCode);
    this.PythonContext.PublishModule("__main__", module);
    this.Scope = module.Scope;
    try
    {
      scriptCode.Run(this.Scope);
    }
    catch (SystemExitException ex)
    {
      this.Options.Introspection = false;
      return this.GetEffectiveExitCode(ex);
    }
    return 0;
  }

  public override IList<string> GetGlobals(string name)
  {
    IList<string> globals = base.GetGlobals(name);
    foreach (object key in (IEnumerable<object>) this.PythonContext.BuiltinModuleInstance.__dict__.Keys)
    {
      if (key is string str && str.StartsWith(name))
        globals.Add(str);
    }
    return globals;
  }

  protected override void UnhandledException(Exception e)
  {
    PythonOps.PrintException(this.PythonContext.SharedContext, e, this.Console);
  }

  private PythonContext Language => (PythonContext) base.Language;
}
