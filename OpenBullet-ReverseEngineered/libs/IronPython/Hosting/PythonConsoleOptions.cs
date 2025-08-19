// Decompiled with JetBrains decompiler
// Type: IronPython.Hosting.PythonConsoleOptions
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Hosting.Shell;
using System;

#nullable disable
namespace IronPython.Hosting;

[CLSCompliant(true)]
public class PythonConsoleOptions : ConsoleOptions
{
  private bool _ignoreEnvironmentVariables;
  private bool _skipImportSite;
  private bool _skipFistSourceLine;
  private string _runAsModule;
  private bool _basicConsole;

  public bool IgnoreEnvironmentVariables
  {
    get => this._ignoreEnvironmentVariables;
    set => this._ignoreEnvironmentVariables = value;
  }

  public bool SkipImportSite
  {
    get => this._skipImportSite;
    set => this._skipImportSite = value;
  }

  public string ModuleToRun
  {
    get => this._runAsModule;
    set => this._runAsModule = value;
  }

  public bool SkipFirstSourceLine
  {
    get => this._skipFistSourceLine;
    set => this._skipFistSourceLine = value;
  }

  public bool BasicConsole
  {
    get => this._basicConsole;
    set => this._basicConsole = value;
  }
}
