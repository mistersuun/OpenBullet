// Decompiled with JetBrains decompiler
// Type: RuriLib.Config
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.LS;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace RuriLib;

public class Config : ViewModelBase
{
  public Config(ConfigSettings settings, string script)
  {
    this.Settings = settings;
    this.Script = script;
  }

  public ConfigSettings Settings { get; set; }

  public string Script { get; set; }

  public bool SeleniumPresent
  {
    get
    {
      return this.Script.Contains("NAVIGATE") || this.Script.Contains("BROWSERACTION") || this.Script.Contains("ELEMENTACTION") || this.Script.Contains("EXECUTEJS") || this.Script.Contains("MOUSEACTION");
    }
  }

  public bool DangerousScriptPresent => this.Script.ToUpper().Contains("BEGIN SCRIPT");

  public bool CaptchasNeeded => this.Script.Contains("CAPTCHA");

  public bool HasCFBypass => this.Script.Contains("BYPASSCF");

  public int BlocksAmount
  {
    get
    {
      return ((IEnumerable<string>) this.Script.Split(new string[1]
      {
        Environment.NewLine
      }, StringSplitOptions.RemoveEmptyEntries)).Where<string>((Func<string, bool>) (l => BlockParser.IsBlock(l))).Count<string>();
    }
  }

  public string AllowedWordlists
  {
    get => $"{this.Settings.AllowedWordlist1} | {this.Settings.AllowedWordlist2}";
  }

  public string LastModifiedString
  {
    get
    {
      DateTime lastModified = this.Settings.LastModified;
      string shortDateString = lastModified.ToShortDateString();
      lastModified = this.Settings.LastModified;
      string shortTimeString = lastModified.ToShortTimeString();
      return $"{shortDateString} {shortTimeString}";
    }
  }
}
