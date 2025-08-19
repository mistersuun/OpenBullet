// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockImageCaptcha
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.CaptchaServices;
using RuriLib.LS;
using RuriLib.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class BlockImageCaptcha : BlockCaptcha
{
  private string url = "";
  private string variableName = "";
  private bool base64;
  private bool sendScreenshot;
  private string userAgent = "";

  public string Url
  {
    get => this.url;
    set
    {
      this.url = value;
      this.OnPropertyChanged(nameof (Url));
    }
  }

  public string VariableName
  {
    get => this.variableName;
    set
    {
      this.variableName = value;
      this.OnPropertyChanged(nameof (VariableName));
    }
  }

  public bool Base64
  {
    get => this.base64;
    set
    {
      this.base64 = value;
      this.OnPropertyChanged(nameof (Base64));
    }
  }

  public bool SendScreenshot
  {
    get => this.sendScreenshot;
    set
    {
      this.sendScreenshot = value;
      this.OnPropertyChanged(nameof (SendScreenshot));
    }
  }

  public string UserAgent
  {
    get => this.userAgent;
    set
    {
      this.userAgent = value;
      this.OnPropertyChanged(nameof (UserAgent));
    }
  }

  public BlockImageCaptcha() => this.Label = "CAPTCHA";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    this.Url = LineParser.ParseLiteral(ref input, "URL");
    if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Literal)
      this.UserAgent = LineParser.ParseLiteral(ref input, "UserAgent");
    while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
      LineParser.SetBool(ref input, (object) this);
    LineParser.EnsureIdentifier(ref input, "->");
    LineParser.EnsureIdentifier(ref input, "VAR");
    this.VariableName = LineParser.ParseLiteral(ref input, "VARIABLE NAME");
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "CAPTCHA").Literal(this.Url);
    if (this.UserAgent != "")
      blockWriter.Literal(this.UserAgent);
    blockWriter.Boolean(this.Base64, "Base64").Boolean(this.SendScreenshot, "SendScreenshot").Arrow().Token((object) "VAR").Literal(this.VariableName);
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    if (!data.GlobalSettings.Captchas.BypassBalanceCheck)
      base.Process(data);
    string str1 = BlockBase.ReplaceValues(this.url, data);
    data.Log(new LogEntry("Downloading image...", Colors.White));
    string str2 = $"Captchas/captcha{data.BotNumber}.jpg";
    if (this.base64)
    {
      byte[] buffer = Convert.FromBase64String(str1);
      using (FileStream fileStream = new FileStream(str2, FileMode.Create))
      {
        fileStream.Write(buffer, 0, buffer.Length);
        fileStream.Flush();
      }
    }
    else if (this.sendScreenshot && data.Screenshots.Count > 0)
    {
      new Bitmap(data.Screenshots.Last<string>()).Save(str2);
    }
    else
    {
      try
      {
        Dictionary<string, string> newCookies;
        RuriLib.Functions.Download.Download.RemoteFile(str2, str1, data.UseProxies, data.Proxy, data.Cookies, out newCookies, data.GlobalSettings.General.RequestTimeout * 1000, BlockBase.ReplaceValues(this.UserAgent, data));
        data.Cookies = newCookies;
      }
      catch (Exception ex)
      {
        data.Log(new LogEntry(ex.Message, Colors.Tomato));
        throw;
      }
    }
    string str3 = "";
    Bitmap image = new Bitmap(str2);
    try
    {
      str3 = Service.Initialize(data.GlobalSettings.Captchas).SolveCaptcha(image);
    }
    catch (Exception ex)
    {
      data.Log(new LogEntry(ex.Message, Colors.Tomato));
      throw;
    }
    finally
    {
      image.Dispose();
    }
    data.Log(str3 == "" ? new LogEntry("Couldn't get a response from the service", Colors.Tomato) : new LogEntry("Succesfully got the response: " + str3, Colors.GreenYellow));
    if (!(this.VariableName != ""))
      return;
    data.Log(new LogEntry("Response stored in variable: " + this.variableName, Colors.White));
    data.Variables.Set(new CVar(this.variableName, str3));
  }
}
