// Decompiled with JetBrains decompiler
// Type: RuriLib.BotData
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using Microsoft.CSharp.RuntimeBinder;
using OpenQA.Selenium.Remote;
using RuriLib.CaptchaServices;
using RuriLib.Models;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class BotData
{
  public Random Random = new Random();
  public string CustomStatus = "CUSTOM";

  public BotStatus Status { get; set; }

  public string StatusString
  {
    get => this.Status != BotStatus.CUSTOM ? this.Status.ToString() : this.CustomStatus;
  }

  public int BotNumber { get; set; }

  public CaptchaService CaptchaService { get; set; }

  public RemoteWebDriver Driver { get; set; }

  public bool BrowserOpen { get; set; }

  public TcpClient TCPClient { get; set; }

  public NetworkStream NETStream { get; set; }

  public SslStream SSLStream { get; set; }

  public bool TCPSSL { get; set; } = true;

  public CData Data { get; set; }

  public CProxy Proxy { get; set; }

  public bool UseProxies { get; set; }

  public RLSettingsViewModel GlobalSettings { get; set; }

  public ConfigSettings ConfigSettings { get; set; }

  public double Balance { get; set; }

  public List<string> Screenshots { get; set; }

  public string Address
  {
    get
    {
      if (BotData.\u003C\u003Eo__69.\u003C\u003Ep__0 == null)
        BotData.\u003C\u003Eo__69.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (BotData)));
      return BotData.\u003C\u003Eo__69.\u003C\u003Ep__0.Target((CallSite) BotData.\u003C\u003Eo__69.\u003C\u003Ep__0, this.Variables.Get("ADDRESS").Value);
    }
    set => this.Variables.SetHidden("ADDRESS", (object) value);
  }

  public string ResponseCode
  {
    get
    {
      if (BotData.\u003C\u003Eo__72.\u003C\u003Ep__0 == null)
        BotData.\u003C\u003Eo__72.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (BotData)));
      return BotData.\u003C\u003Eo__72.\u003C\u003Ep__0.Target((CallSite) BotData.\u003C\u003Eo__72.\u003C\u003Ep__0, this.Variables.Get("RESPONSECODE").Value);
    }
    set => this.Variables.SetHidden("RESPONSECODE", (object) value);
  }

  public Dictionary<string, string> ResponseHeaders
  {
    get
    {
      if (BotData.\u003C\u003Eo__75.\u003C\u003Ep__0 == null)
        BotData.\u003C\u003Eo__75.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Dictionary<string, string>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Dictionary<string, string>), typeof (BotData)));
      return BotData.\u003C\u003Eo__75.\u003C\u003Ep__0.Target((CallSite) BotData.\u003C\u003Eo__75.\u003C\u003Ep__0, this.Variables.Get("HEADERS").Value);
    }
    set => this.Variables.SetHidden("HEADERS", (object) value);
  }

  public Dictionary<string, string> Cookies
  {
    get
    {
      if (BotData.\u003C\u003Eo__78.\u003C\u003Ep__0 == null)
        BotData.\u003C\u003Eo__78.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Dictionary<string, string>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Dictionary<string, string>), typeof (BotData)));
      return BotData.\u003C\u003Eo__78.\u003C\u003Ep__0.Target((CallSite) BotData.\u003C\u003Eo__78.\u003C\u003Ep__0, this.Variables.Get("COOKIES").Value);
    }
    set => this.Variables.SetHidden("COOKIES", (object) value);
  }

  public string ResponseSource
  {
    get
    {
      if (BotData.\u003C\u003Eo__81.\u003C\u003Ep__0 == null)
        BotData.\u003C\u003Eo__81.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (BotData)));
      return BotData.\u003C\u003Eo__81.\u003C\u003Ep__0.Target((CallSite) BotData.\u003C\u003Eo__81.\u003C\u003Ep__0, this.Variables.Get("SOURCE").Value);
    }
    set => this.Variables.SetHidden("SOURCE", (object) value);
  }

  public VariableList Variables { get; set; }

  public VariableList GlobalVariables { get; set; }

  public Dictionary<string, string> GlobalCookies { get; set; }

  public List<LogEntry> LogBuffer { get; set; }

  public bool IsDebug { get; set; }

  public BotData(
    RLSettingsViewModel globalSettings,
    ConfigSettings configSettings,
    CData data,
    CProxy proxy,
    bool useProxies,
    Random random,
    int botNumber = 0,
    bool isDebug = true)
  {
    this.Data = data;
    this.Proxy = proxy;
    this.UseProxies = useProxies;
    this.Random = random;
    this.Status = BotStatus.NONE;
    this.BotNumber = this.BotNumber;
    this.GlobalSettings = globalSettings;
    this.ConfigSettings = configSettings;
    this.Balance = 0.0;
    this.Screenshots = new List<string>();
    this.Variables = new VariableList();
    this.Address = "";
    this.ResponseCode = "0";
    this.ResponseSource = "";
    this.Cookies = new Dictionary<string, string>();
    this.ResponseHeaders = new Dictionary<string, string>();
    try
    {
      foreach (CVar variable in this.Data.GetVariables(this.ConfigSettings.EncodeData))
        this.Variables.Set(variable);
    }
    catch
    {
    }
    this.GlobalVariables = new VariableList();
    this.GlobalCookies = (Dictionary<string, string>) new CookieDictionary();
    this.LogBuffer = new List<LogEntry>();
    this.Driver = (RemoteWebDriver) null;
    this.BrowserOpen = false;
    this.IsDebug = isDebug;
    this.BotNumber = botNumber;
  }

  public void Log(LogEntry entry)
  {
    if (!this.GlobalSettings.General.EnableBotLog && !this.IsDebug)
      return;
    this.LogBuffer.Add(entry);
  }

  public void LogRange(List<LogEntry> list)
  {
    if (!this.GlobalSettings.General.EnableBotLog && !this.IsDebug)
      return;
    this.LogBuffer.AddRange((IEnumerable<LogEntry>) list);
  }

  public void LogNewLine() => this.LogBuffer.Add(new LogEntry("", Colors.White));
}
