// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.CommandLine
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Hosting.Shell.Remote;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell;

public class CommandLine
{
  private LanguageContext _language;
  private IConsole _console;
  private ConsoleOptions _options;
  private ScriptScope _scope;
  private ScriptEngine _engine;
  private ICommandDispatcher _commandDispatcher;
  private int? _terminatingExitCode;
  private int _exitCode = 1;

  protected IConsole Console => this._console;

  protected ConsoleOptions Options => this._options;

  protected ScriptEngine Engine => this._engine;

  public ScriptScope ScriptScope
  {
    get => this._scope;
    protected set => this._scope = value;
  }

  public int ExitCode
  {
    get => this._exitCode;
    protected set => this._exitCode = value;
  }

  protected Scope Scope
  {
    get => this._scope == null ? (Scope) null : HostingHelpers.GetScope(this._scope);
    set => this._scope = HostingHelpers.CreateScriptScope(this._engine, value);
  }

  protected LanguageContext Language
  {
    get
    {
      if (this._language == null)
        this._language = HostingHelpers.GetLanguageContext(this._engine);
      return this._language;
    }
  }

  protected virtual string Prompt => ">>> ";

  public virtual string PromptContinuation => "... ";

  protected virtual string Logo => (string) null;

  protected virtual void Initialize()
  {
    if (this._commandDispatcher != null)
      return;
    this._commandDispatcher = this.CreateCommandDispatcher();
  }

  protected virtual Scope CreateScope() => new Scope();

  protected virtual ICommandDispatcher CreateCommandDispatcher()
  {
    return (ICommandDispatcher) new CommandLine.SimpleCommandDispatcher();
  }

  public virtual void Terminate(int exitCode) => this._terminatingExitCode = new int?(exitCode);

  public void Run(ScriptEngine engine, IConsole console, ConsoleOptions options)
  {
    ContractUtils.RequiresNotNull((object) engine, nameof (engine));
    ContractUtils.RequiresNotNull((object) console, nameof (console));
    ContractUtils.RequiresNotNull((object) options, nameof (options));
    this._engine = engine;
    this._options = options;
    this._console = console;
    this.Initialize();
    try
    {
      this._exitCode = this.Run();
    }
    catch (ThreadAbortException ex)
    {
      if (ex.ExceptionState is KeyboardInterruptException)
      {
        Thread.ResetAbort();
        this._exitCode = -1;
      }
      else
        throw;
    }
    finally
    {
      this.Shutdown();
      this._engine = (ScriptEngine) null;
      this._options = (ConsoleOptions) null;
      this._console = (IConsole) null;
    }
  }

  protected virtual int Run()
  {
    int num;
    if (this._options.Command != null)
    {
      num = this.RunCommand(this._options.Command);
    }
    else
    {
      if (this._options.FileName == null)
        return this.RunInteractive();
      num = this.RunFile(this._options.FileName);
    }
    return this._options.Introspection ? this.RunInteractiveLoop() : num;
  }

  protected virtual void Shutdown()
  {
    try
    {
      this._engine.Runtime.Shutdown();
    }
    catch (Exception ex)
    {
      this.UnhandledException(ex);
    }
  }

  protected virtual int RunFile(string fileName)
  {
    return this.RunFile(this._engine.CreateScriptSourceFromFile(fileName));
  }

  protected virtual int RunCommand(string command)
  {
    return this.RunFile(this._engine.CreateScriptSourceFromString(command, SourceCodeKind.Statements));
  }

  protected virtual int RunFile(ScriptSource source)
  {
    int num = 1;
    if (this.Options.HandleExceptions)
    {
      try
      {
        num = source.ExecuteProgram();
      }
      catch (Exception ex)
      {
        this.UnhandledException(ex);
      }
    }
    else
      num = source.ExecuteProgram();
    return num;
  }

  protected void PrintLogo()
  {
    if (this.Logo == null)
      return;
    this._console.Write(this.Logo, Style.Out);
  }

  protected virtual int RunInteractive()
  {
    this.PrintLogo();
    return this.RunInteractiveLoop();
  }

  protected virtual int RunInteractiveLoop()
  {
    if (this._scope == null)
      this._scope = this._engine.CreateScope();
    string remoteRuntimeChannel = this._options.RemoteRuntimeChannel;
    if (remoteRuntimeChannel != null)
    {
      RemoteRuntimeServer.StartServer(remoteRuntimeChannel, this._scope);
      return 0;
    }
    int? nullable = new int?();
    do
    {
      if (this.Options.HandleExceptions)
      {
        try
        {
          nullable = this.TryInteractiveAction();
        }
        catch (Exception ex)
        {
          if (CommandLine.IsFatalException(ex))
            throw;
          this.UnhandledException(ex);
        }
      }
      else
        nullable = this.TryInteractiveAction();
    }
    while (!nullable.HasValue);
    return nullable.Value;
  }

  internal static bool IsFatalException(Exception e)
  {
    return e is ThreadAbortException threadAbortException && !(threadAbortException.ExceptionState is KeyboardInterruptException);
  }

  protected virtual void UnhandledException(Exception e)
  {
    this._console.WriteLine(this._engine.GetService<ExceptionOperations>().FormatException(e), Style.Error);
  }

  protected virtual int? TryInteractiveAction()
  {
    int? nullable = new int?();
    try
    {
      nullable = this.RunOneInteraction();
    }
    catch (ThreadAbortException ex)
    {
      if (ex.ExceptionState is KeyboardInterruptException)
      {
        this.UnhandledException((Exception) ex);
        Thread.ResetAbort();
      }
      else
        throw;
    }
    return nullable;
  }

  private int? RunOneInteraction()
  {
    bool continueInteraction;
    string command = this.ReadStatement(out continueInteraction);
    if (!continueInteraction)
      return new int?(this._terminatingExitCode ?? 0);
    if (string.IsNullOrEmpty(command))
    {
      this._console.Write(string.Empty, Style.Out);
      return new int?();
    }
    this.ExecuteCommand(command);
    return new int?();
  }

  protected virtual void ExecuteCommand(string command)
  {
    this.ExecuteCommand(this._engine.CreateScriptSourceFromString(command, SourceCodeKind.InteractiveCode));
  }

  protected object ExecuteCommand(ScriptSource source)
  {
    ErrorListener errorListener = (ErrorListener) new ErrorSinkProxyListener(this.ErrorSink);
    return this._commandDispatcher.Execute(source.Compile(this._engine.GetCompilerOptions(this._scope), errorListener), this._scope);
  }

  protected virtual ErrorSink ErrorSink => ErrorSink.Default;

  private static bool TreatAsBlankLine(string line, int autoIndentSize)
  {
    return line.Length == 0 || autoIndentSize != 0 && line.Trim().Length == 0 && line.Length == autoIndentSize;
  }

  protected string ReadStatement(out bool continueInteraction)
  {
    StringBuilder stringBuilder = new StringBuilder();
    int autoIndentSize = 0;
    this._console.Write(this.Prompt, Style.Prompt);
    string str;
    ScriptCodeParseResult commandProperties;
    while (true)
    {
      string line = this.ReadLine(autoIndentSize);
      continueInteraction = true;
      if (line != null && !this._terminatingExitCode.HasValue)
      {
        bool allowIncompleteStatement = CommandLine.TreatAsBlankLine(line, autoIndentSize);
        stringBuilder.Append(line);
        stringBuilder.Append("\n");
        str = stringBuilder.ToString();
        commandProperties = this.GetCommandProperties(str);
        if (!SourceCodePropertiesUtils.IsCompleteOrInvalid(commandProperties, allowIncompleteStatement))
        {
          if (this._options.AutoIndent && this._options.AutoIndentSize != 0)
            autoIndentSize = this.GetNextAutoIndentSize(str);
          this._console.Write(this.PromptContinuation, Style.Prompt);
        }
        else
          goto label_4;
      }
      else
        break;
    }
    continueInteraction = false;
    return (string) null;
label_4:
    return commandProperties == ScriptCodeParseResult.Empty ? (string) null : str;
  }

  protected virtual ScriptCodeParseResult GetCommandProperties(string code)
  {
    return this._engine.CreateScriptSourceFromString(code, SourceCodeKind.InteractiveCode).GetCodeProperties(this._engine.GetCompilerOptions(this._scope));
  }

  protected virtual int GetNextAutoIndentSize(string text) => 0;

  protected virtual string ReadLine(int autoIndentSize) => this._console.ReadLine(autoIndentSize);

  protected internal virtual TextWriter GetOutputWriter(bool isErrorOutput)
  {
    return !isErrorOutput ? System.Console.Out : System.Console.Error;
  }

  public IList<string> GetMemberNames(string code)
  {
    return this._engine.Operations.GetMemberNames(this._engine.CreateScriptSourceFromString(code, SourceCodeKind.Expression).Execute(this._scope));
  }

  public virtual IList<string> GetGlobals(string name)
  {
    List<string> globals = new List<string>();
    foreach (string variableName in this._scope.GetVariableNames())
    {
      if (variableName.StartsWith(name))
        globals.Add(variableName);
    }
    return (IList<string>) globals;
  }

  private class SimpleCommandDispatcher : ICommandDispatcher
  {
    public object Execute(CompiledCode compiledCode, ScriptScope scope)
    {
      return compiledCode.Execute(scope);
    }
  }
}
