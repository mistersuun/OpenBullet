// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.ConsoleOptions
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell;

[Serializable]
public class ConsoleOptions
{
  private string _command;
  private string _filename;
  private bool _printVersion;
  private bool _exit;
  private int _autoIndentSize = 4;
  private string[] _remainingArgs;
  private bool _introspection;
  private bool _autoIndent;
  private bool _handleExceptions = true;
  private bool _tabCompletion;
  private bool _colorfulConsole;
  private bool _printUsage;
  private bool _isMta;
  private string _remoteRuntimeChannel;

  public bool AutoIndent
  {
    get => this._autoIndent;
    set => this._autoIndent = value;
  }

  public bool HandleExceptions
  {
    get => this._handleExceptions;
    set => this._handleExceptions = value;
  }

  public bool TabCompletion
  {
    get => this._tabCompletion;
    set => this._tabCompletion = value;
  }

  public bool ColorfulConsole
  {
    get => this._colorfulConsole;
    set => this._colorfulConsole = value;
  }

  public bool PrintUsage
  {
    get => this._printUsage;
    set => this._printUsage = value;
  }

  public string Command
  {
    get => this._command;
    set => this._command = value;
  }

  public string FileName
  {
    get => this._filename;
    set => this._filename = value;
  }

  public bool PrintVersion
  {
    get => this._printVersion;
    set => this._printVersion = value;
  }

  public bool Exit
  {
    get => this._exit;
    set => this._exit = value;
  }

  public int AutoIndentSize
  {
    get => this._autoIndentSize;
    set => this._autoIndentSize = value;
  }

  public string[] RemainingArgs
  {
    get => this._remainingArgs;
    set => this._remainingArgs = value;
  }

  public bool Introspection
  {
    get => this._introspection;
    set => this._introspection = value;
  }

  public bool IsMta
  {
    get => this._isMta;
    set => this._isMta = value;
  }

  public string RemoteRuntimeChannel
  {
    get => this._remoteRuntimeChannel;
    set => this._remoteRuntimeChannel = value;
  }

  public ConsoleOptions()
  {
  }

  protected ConsoleOptions(ConsoleOptions options)
  {
    ContractUtils.RequiresNotNull((object) options, nameof (options));
    this._command = options._command;
    this._filename = options._filename;
    this._printVersion = options._printVersion;
    this._exit = options._exit;
    this._autoIndentSize = options._autoIndentSize;
    this._remainingArgs = ArrayUtils.Copy<string>(options._remainingArgs);
    this._introspection = options._introspection;
    this._autoIndent = options._autoIndent;
    this._handleExceptions = options._handleExceptions;
    this._tabCompletion = options._tabCompletion;
    this._colorfulConsole = options._colorfulConsole;
    this._printUsage = options._printUsage;
    this._isMta = options._isMta;
    this._remoteRuntimeChannel = options._remoteRuntimeChannel;
  }
}
