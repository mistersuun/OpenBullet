// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockRecaptcha
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.CaptchaServices;
using RuriLib.LS;
using RuriLib.Models;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class BlockRecaptcha : BlockCaptcha
{
  private string variableName = "";
  private string url = "https://google.com";
  private string siteKey = "";

  public string VariableName
  {
    get => this.variableName;
    set
    {
      this.variableName = value;
      this.OnPropertyChanged(nameof (VariableName));
    }
  }

  public string Url
  {
    get => this.url;
    set
    {
      this.url = value;
      this.OnPropertyChanged(nameof (Url));
    }
  }

  public string SiteKey
  {
    get => this.siteKey;
    set
    {
      this.siteKey = value;
      this.OnPropertyChanged(nameof (SiteKey));
    }
  }

  public BlockRecaptcha() => this.Label = "RECAPTCHA";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    this.Url = LineParser.ParseLiteral(ref input, "URL");
    this.SiteKey = LineParser.ParseLiteral(ref input, "SITEKEY");
    if (LineParser.ParseToken(ref input, TokenType.Arrow, false) == "")
      return (BlockBase) this;
    LineParser.EnsureIdentifier(ref input, "VAR");
    this.VariableName = LineParser.ParseLiteral(ref input, "VARIABLE NAME");
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "RECAPTCHA").Literal(this.Url).Literal(this.SiteKey).Arrow().Token((object) "VAR").Literal(this.VariableName);
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    if (!data.GlobalSettings.Captchas.BypassBalanceCheck)
      base.Process(data);
    data.Log(new LogEntry("Solving reCaptcha...", Colors.White));
    string str = Service.Initialize(data.GlobalSettings.Captchas).SolveRecaptcha(this.siteKey, BlockBase.ReplaceValues(this.url, data));
    data.Log(str == "" ? new LogEntry("Couldn't get a reCaptcha response from the service", Colors.Tomato) : new LogEntry("Succesfully got the response: " + str, Colors.GreenYellow));
    if (!(this.VariableName != ""))
      return;
    data.Log(new LogEntry("Response stored in variable: " + this.variableName, Colors.White));
    data.Variables.Set(new CVar(this.variableName, str));
  }
}
