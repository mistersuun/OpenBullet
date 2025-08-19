// Decompiled with JetBrains decompiler
// Type: RuriLib.SBlockNavigate
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using OpenQA.Selenium;
using RuriLib.LS;
using System;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class SBlockNavigate : BlockBase
{
  private string url = "https://example.com";
  private int timeout = 60;
  private bool banOnTimeout = true;

  public string Url
  {
    get => this.url;
    set
    {
      this.url = value;
      this.OnPropertyChanged(nameof (Url));
    }
  }

  public int Timeout
  {
    get => this.timeout;
    set
    {
      this.timeout = value;
      this.OnPropertyChanged(nameof (Timeout));
    }
  }

  public bool BanOnTimeout
  {
    get => this.banOnTimeout;
    set
    {
      this.banOnTimeout = value;
      this.OnPropertyChanged(nameof (BanOnTimeout));
    }
  }

  public SBlockNavigate() => this.Label = "NAVIGATE";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    this.Url = LineParser.ParseLiteral(ref input, "URL");
    if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Integer)
      this.Timeout = LineParser.ParseInt(ref input, "TIMEOUT");
    if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
      LineParser.SetBool(ref input, (object) this);
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "NAVIGATE").Literal(this.Url).Integer(this.Timeout, "Timeout").Boolean(this.BanOnTimeout, "BanOnTimeout");
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    base.Process(data);
    if (data.Driver == null)
    {
      data.Log(new LogEntry("Open a browser first!", Colors.White));
      throw new Exception("Browser not open");
    }
    data.Log(new LogEntry("Navigating to " + BlockBase.ReplaceValues(this.url, data), Colors.White));
    data.Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds((double) this.timeout);
    try
    {
      data.Driver.Navigate().GoToUrl(BlockBase.ReplaceValues(this.url, data));
      data.Log(new LogEntry("Navigated!", Colors.White));
    }
    catch (WebDriverTimeoutException ex)
    {
      data.Log(new LogEntry("Timeout on Page Load", Colors.Tomato));
      if (this.BanOnTimeout)
        data.Status = BotStatus.BAN;
    }
    data.Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds((double) data.GlobalSettings.Selenium.PageLoadTimeout);
    BlockBase.UpdateSeleniumData(data);
  }
}
