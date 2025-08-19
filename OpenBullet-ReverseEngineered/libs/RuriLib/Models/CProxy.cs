// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.CProxy
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace RuriLib.Models;

public class CProxy
{
  public Guid Id { get; set; }

  public string Proxy { get; set; } = "";

  public string Username { get; set; } = "";

  public string Password { get; set; } = "";

  public ProxyType Type { get; set; }

  public string Country { get; set; } = "";

  public int Ping { get; set; }

  public CProxy Next { get; set; }

  public DateTime LastUsed { get; set; } = new DateTime(1970, 1, 1);

  public DateTime LastChecked { get; set; } = new DateTime(1970, 1, 1);

  public ProxyWorking Working { get; set; } = ProxyWorking.UNTESTED;

  [BsonIgnore]
  public bool HasNext => this.Next != null;

  [BsonIgnore]
  public int Uses { get; set; }

  [BsonIgnore]
  public int Hooked { get; set; }

  [BsonIgnore]
  public string Clearance { get; set; } = "";

  [BsonIgnore]
  public string Cfduid { get; set; } = "";

  [BsonIgnore]
  public Status Status { get; set; }

  public CProxy()
  {
  }

  public CProxy(string proxy, ProxyType type)
  {
    this.Proxy = proxy;
    this.Type = type;
  }

  public CProxy Parse(
    string proxy,
    ProxyType defaultType = ProxyType.Http,
    string defaultUsername = "",
    string defaultPassword = "")
  {
    string[] source1 = proxy.Split(new string[1]{ "->" }, 2, StringSplitOptions.None);
    string input = source1[0];
    if (input.StartsWith("("))
    {
      GroupCollection groups = Regex.Match(input, "^\\((.*)\\)(.*)").Groups;
      ProxyType result;
      Enum.TryParse<ProxyType>(groups[1].Value, true, out result);
      this.Type = result;
      input = input.Replace($"({groups[1].Value})", "");
    }
    else
      this.Type = defaultType;
    string[] source2 = input.Split(':');
    this.Proxy = $"{source2[0]}:{source2[1]}";
    if (((IEnumerable<string>) source2).Count<string>() > 2)
    {
      this.Username = source2[2];
      this.Password = source2[3];
    }
    else
    {
      this.Username = defaultUsername;
      this.Password = defaultPassword;
    }
    if (((IEnumerable<string>) source1).Count<string>() > 1)
      this.Next = new CProxy().Parse(source1[1], defaultType, defaultUsername, defaultPassword);
    return this;
  }

  public ProxyClient GetClient()
  {
    if (this.HasNext)
    {
      ChainProxyClient client = new ChainProxyClient();
      for (CProxy cproxy = this; cproxy != null; cproxy = cproxy.Next)
      {
        switch (cproxy.Type)
        {
          case ProxyType.Http:
            client.AddHttpProxy(cproxy.Proxy);
            break;
          case ProxyType.Socks4:
            client.AddSocks4Proxy(cproxy.Proxy);
            break;
          case ProxyType.Socks4a:
            client.AddSocks4aProxy(cproxy.Proxy);
            break;
          case ProxyType.Socks5:
            client.AddSocks5Proxy(cproxy.Proxy);
            break;
        }
      }
      return (ProxyClient) client;
    }
    switch (this.Type)
    {
      case ProxyType.Http:
        return (ProxyClient) HttpProxyClient.Parse(this.Proxy);
      case ProxyType.Socks4:
        return (ProxyClient) Socks4ProxyClient.Parse(this.Proxy);
      case ProxyType.Socks4a:
        return (ProxyClient) Socks4aProxyClient.Parse(this.Proxy);
      case ProxyType.Socks5:
        return (ProxyClient) Socks5ProxyClient.Parse(this.Proxy);
      default:
        return (ProxyClient) null;
    }
  }

  [BsonIgnore]
  public string Host
  {
    get
    {
      try
      {
        return ((IEnumerable<string>) this.Proxy.Split(':')).First<string>();
      }
      catch
      {
        return "";
      }
    }
  }

  [BsonIgnore]
  public string Port
  {
    get
    {
      try
      {
        return ((IEnumerable<string>) this.Proxy.Split(':')).Last<string>();
      }
      catch
      {
        return "";
      }
    }
  }

  [BsonIgnore]
  public bool IsValidNumeric
  {
    get
    {
      if (!(this.Host == "") && !(this.Port == "") && !this.Port.Any<char>((Func<char, bool>) (c => !char.IsDigit(c))))
      {
        if (((IEnumerable<string>) this.Host.Split('.')).Count<string>() == 4)
          return this.IsNumeric;
      }
      return false;
    }
  }

  [BsonIgnore]
  public bool IsNumeric
  {
    get
    {
      return !((IEnumerable<string>) this.Host.Split('.')).Any<string>((Func<string, bool>) (x => x.Any<char>((Func<char, bool>) (c => !char.IsDigit(c)))));
    }
  }

  [BsonIgnore]
  public TimeSpan UsedAgo => DateTime.Now - this.LastUsed;
}
