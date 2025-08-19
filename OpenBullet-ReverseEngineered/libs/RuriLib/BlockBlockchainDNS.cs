// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockBlockchainDNS
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using RuriLib.LS;
using RuriLib.Models;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class BlockBlockchainDNS : BlockBase
{
  private string variableName = "";
  private string url = "https://google.com";

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

  public BlockBlockchainDNS() => this.Label = "BLOCKCHAINDNS";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    this.Url = LineParser.ParseLiteral(ref input, "URL");
    if (LineParser.ParseToken(ref input, TokenType.Arrow, false) == "")
      return (BlockBase) this;
    LineParser.EnsureIdentifier(ref input, "VAR");
    this.VariableName = LineParser.ParseLiteral(ref input, "VARIABLE NAME");
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "BLOCKCHAINDNS").Literal(this.Url).Arrow().Token((object) "VAR").Literal(this.VariableName);
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    if (!data.GlobalSettings.Captchas.BypassBalanceCheck)
      base.Process(data);
    data.Log(new LogEntry("Resolving DNS...", Colors.White));
    HttpRequest httpRequest = new HttpRequest();
    HttpResponse httpResponse1 = httpRequest.Raw(HttpMethod.GET, "https://bdns.co/r/" + BlockBase.ReplaceValues(this.url, data));
    string address = httpResponse1.ToString().Trim();
    data.Log(httpResponse1.HasError ? new LogEntry("Failed to resolve " + this.url, Colors.Tomato) : new LogEntry("Succesfully resolved: " + this.url, Colors.GreenYellow));
    int num = data.GlobalSettings.General.RequestTimeout * 1000;
    httpRequest.IgnoreProtocolErrors = true;
    httpRequest.AllowAutoRedirect = false;
    httpRequest.EnableEncodingContent = true;
    httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
    httpRequest.ReadWriteTimeout = num;
    httpRequest.ConnectTimeout = num;
    httpRequest.KeepAlive = true;
    httpRequest.MaximumAutomaticRedirections = data.ConfigSettings.MaxRedirects;
    HttpResponse httpResponse2 = httpRequest.Raw(HttpMethod.GET, address);
    data.Log(new LogEntry("Response Source: ", Colors.Green));
    data.ResponseSource = httpResponse2.ToString();
    data.Log(new LogEntry(data.ResponseSource, Colors.GreenYellow));
    data.Log(new LogEntry("Resolved URL stored in variable: " + this.variableName, Colors.White));
    data.Variables.Set(new CVar(this.variableName, address));
  }
}
