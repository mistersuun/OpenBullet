// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.ConsoleHostOptions
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell;

public class ConsoleHostOptions
{
  public List<string> IgnoredArgs { get; } = new List<string>();

  public string RunFile { get; set; }

  public string[] SourceUnitSearchPaths { get; set; }

  public ConsoleHostOptions.Action RunAction { get; set; }

  public List<string> EnvironmentVars { get; } = new List<string>();

  public string LanguageProvider { get; set; }

  public bool HasLanguageProvider { get; set; }

  public string[,] GetHelp()
  {
    return new string[4, 2]
    {
      {
        "/help",
        "Displays this help."
      },
      {
        "/lang:<extension>",
        "Specify language by the associated extension (py, js, vb, rb). Determined by an extension of the first file. Defaults to IronPython."
      },
      {
        "/paths:<file-path-list>",
        "Semicolon separated list of import paths (/run only)."
      },
      {
        "/setenv:<var1=value1;...>",
        "Sets specified environment variables for the console process. Not available on Silverlight."
      }
    };
  }

  public enum Action
  {
    None,
    RunConsole,
    RunFile,
    DisplayHelp,
  }
}
