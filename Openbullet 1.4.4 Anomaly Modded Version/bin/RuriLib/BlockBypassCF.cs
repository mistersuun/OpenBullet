// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockBypassCF
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Leaf.xNet;
using Leaf.xNet.Services.Captcha;
using Leaf.xNet.Services.Cloudflare;
using RuriLib.LS;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class BlockBypassCF : BlockBase
{
  private string url = "";
  private string userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
  private bool printResponseInfo = true;
  private bool errorOn302 = true;

  public string Url
  {
    get => this.url;
    set
    {
      this.url = value;
      this.OnPropertyChanged(nameof (Url));
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

  public bool PrintResponseInfo
  {
    get => this.printResponseInfo;
    set
    {
      this.printResponseInfo = value;
      this.OnPropertyChanged(nameof (PrintResponseInfo));
    }
  }

  public bool ErrorOn302
  {
    get => this.errorOn302;
    set
    {
      this.errorOn302 = value;
      this.OnPropertyChanged(nameof (ErrorOn302));
    }
  }

  public BlockBypassCF() => this.Label = "BYPASS CF";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    this.Url = LineParser.ParseLiteral(ref input, "URL");
    if (input != "" && LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Literal)
      this.UserAgent = LineParser.ParseLiteral(ref input, "UA");
    while (input != "")
      LineParser.SetBool(ref input, (object) this);
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "BYPASSCF").Literal(this.Url).Literal(this.UserAgent, "UserAgent").Boolean(this.PrintResponseInfo, "PrintResponseInfo").Boolean(this.ErrorOn302, "ErrorOn302");
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    base.Process(data);
    if (data.UseProxies && data.Proxy.Clearance != "" && !data.GlobalSettings.Proxies.AlwaysGetClearance)
    {
      data.Log(new LogEntry("Skipping CF Bypass because there is already a valid cookie", Colors.White));
      data.Cookies["cf_clearance"] = data.Proxy.Clearance;
    }
    else
    {
      string str1 = BlockBase.ReplaceValues(this.url, data);
      Uri uri = new Uri(str1);
      int num = data.GlobalSettings.General.RequestTimeout * 1000;
      Leaf.xNet.HttpRequest request = new Leaf.xNet.HttpRequest();
      request.IgnoreProtocolErrors = true;
      request.ConnectTimeout = num;
      request.ReadWriteTimeout = num;
      request.Cookies = new CookieStorage();
      foreach (KeyValuePair<string, string> cookie in data.Cookies)
        request.Cookies.Add(new Cookie(cookie.Key, cookie.Value, "/", uri.Host));
      if (data.UseProxies)
      {
        switch (data.Proxy.Type)
        {
          case Extreme.Net.ProxyType.Http:
            request.Proxy = (Leaf.xNet.ProxyClient) Leaf.xNet.HttpProxyClient.Parse(data.Proxy.Proxy);
            break;
          case Extreme.Net.ProxyType.Socks4:
            request.Proxy = (Leaf.xNet.ProxyClient) Leaf.xNet.Socks4ProxyClient.Parse(data.Proxy.Proxy);
            break;
          case Extreme.Net.ProxyType.Socks4a:
            request.Proxy = (Leaf.xNet.ProxyClient) Socks4AProxyClient.Parse(data.Proxy.Proxy);
            break;
          case Extreme.Net.ProxyType.Socks5:
            request.Proxy = (Leaf.xNet.ProxyClient) Leaf.xNet.Socks5ProxyClient.Parse(data.Proxy.Proxy);
            break;
          case Extreme.Net.ProxyType.Chain:
            throw new Exception("The Chain Proxy Type is not supported in Leaf.xNet (used for CF Bypass).");
        }
        request.Proxy.ReadWriteTimeout = num;
        request.Proxy.ConnectTimeout = num;
        request.Proxy.Username = data.Proxy.Username;
        request.Proxy.Password = data.Proxy.Password;
      }
      request.UserAgent = BlockBase.ReplaceValues(this.userAgent, data);
      SettingsCaptchas captchas = data.GlobalSettings.Captchas;
      if (captchas.TwoCapToken != "")
      {
        Leaf.xNet.HttpRequest httpRequest = request;
        TwoCaptchaSolver twoCaptchaSolver = new TwoCaptchaSolver();
        twoCaptchaSolver.ApiKey = captchas.TwoCapToken;
        httpRequest.CaptchaSolver = (ICaptchaSolver) twoCaptchaSolver;
      }
      Leaf.xNet.HttpResponse throughCloudflare = request.GetThroughCloudflare(new Uri(str1));
      string str2 = throughCloudflare.ToString();
      CookieCollection cookies = throughCloudflare.Cookies.GetCookies(str1);
      string str3 = "";
      string str4 = "";
      try
      {
        str3 = cookies["cf_clearance"].Value;
        str4 = cookies["__cfduid"].Value;
      }
      catch
      {
      }
      if (data.UseProxies)
      {
        data.Proxy.Clearance = str3;
        data.Proxy.Cfduid = str4;
      }
      if (str3 != "")
      {
        data.Log(new LogEntry("Got Cloudflare clearance!", Colors.GreenYellow));
        data.Log(new LogEntry(str3 + Environment.NewLine + str4 + Environment.NewLine, Colors.White));
      }
      data.ResponseCode = ((int) throughCloudflare.StatusCode).ToString();
      if (this.PrintResponseInfo)
        data.Log(new LogEntry("Response code: " + data.ResponseCode, Colors.Cyan));
      if (this.PrintResponseInfo)
        data.Log(new LogEntry("Received headers:", Colors.DeepPink));
      Dictionary<string, string>.Enumerator enumerator = throughCloudflare.EnumerateHeaders();
      data.ResponseHeaders.Clear();
      while (enumerator.MoveNext())
      {
        KeyValuePair<string, string> current = enumerator.Current;
        data.ResponseHeaders.Add(current.Key, current.Value);
        if (this.PrintResponseInfo)
          data.Log(new LogEntry($"{current.Key}: {current.Value}", Colors.LightPink));
      }
      if (!throughCloudflare.ContainsHeader(Leaf.xNet.HttpHeader.ContentLength))
      {
        data.ResponseHeaders["Content-Length"] = !data.ResponseHeaders.ContainsKey("Content-Encoding") || !data.ResponseHeaders["Content-Encoding"].Contains("gzip") ? str2.Length.ToString() : GZip.Zip(str2).Length.ToString();
        if (this.PrintResponseInfo)
          data.Log(new LogEntry("Content-Length: " + data.ResponseHeaders["Content-Length"], Colors.LightPink));
      }
      if (this.PrintResponseInfo)
        data.Log(new LogEntry("Received cookies:", Colors.Goldenrod));
      foreach (Cookie cookie in throughCloudflare.Cookies.GetCookies(str1))
      {
        if (data.Cookies.ContainsKey(cookie.Name))
          data.Cookies[cookie.Name] = cookie.Value;
        else
          data.Cookies.Add(cookie.Name, cookie.Value);
        if (this.PrintResponseInfo)
          data.Log(new LogEntry($"{cookie.Name}: {cookie.Value}", Colors.LightGoldenrodYellow));
      }
      data.ResponseSource = str2;
      if (this.PrintResponseInfo)
      {
        data.Log(new LogEntry("Response Source:", Colors.Green));
        data.Log(new LogEntry(data.ResponseSource, Colors.GreenYellow));
      }
      if (!this.ErrorOn302 || !data.ResponseCode.Contains("302"))
        return;
      data.Status = BotStatus.ERROR;
    }
  }
}
