// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.ValidData
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;

#nullable disable
namespace RuriLib.Models;

public class ValidData : ViewModelBase
{
  private string data;
  private string proxy;
  private ProxyType proxyType;
  private BotStatus result;
  private string type;
  private string capturedData;
  private int unixDate;
  private string source;
  private List<LogEntry> log;

  public string Data
  {
    get => this.data;
    set
    {
      this.data = value;
      this.OnPropertyChanged(nameof (Data));
    }
  }

  public string Proxy
  {
    get => this.proxy;
    set
    {
      this.proxy = value;
      this.OnPropertyChanged(nameof (Proxy));
    }
  }

  public ProxyType ProxyType
  {
    get => this.proxyType;
    set
    {
      this.proxyType = value;
      this.OnPropertyChanged(nameof (ProxyType));
    }
  }

  public BotStatus Result
  {
    get => this.result;
    set
    {
      this.result = value;
      this.OnPropertyChanged(nameof (Result));
    }
  }

  public string Type
  {
    get => this.type;
    set
    {
      this.type = value;
      this.OnPropertyChanged(nameof (Type));
    }
  }

  public string CapturedData
  {
    get => this.capturedData;
    set
    {
      this.capturedData = value;
      this.OnPropertyChanged(nameof (CapturedData));
    }
  }

  public int UnixDate
  {
    get => this.unixDate;
    set
    {
      this.unixDate = value;
      this.OnPropertyChanged(nameof (UnixDate));
      this.OnPropertyChanged(nameof (UnixDate));
    }
  }

  public string Timestamp
  {
    get
    {
      DateTime dateTime = new DateTime(1970, 1, 1);
      dateTime = dateTime.AddSeconds((double) this.UnixDate);
      return dateTime.ToShortDateString();
    }
  }

  public DateTime Time => new DateTime(1970, 1, 1).AddSeconds((double) this.UnixDate);

  public string Source
  {
    get => this.source;
    set
    {
      this.source = value;
      this.OnPropertyChanged(nameof (Source));
    }
  }

  public List<LogEntry> Log
  {
    get => this.log;
    set
    {
      this.log = value;
      this.OnPropertyChanged(nameof (Log));
    }
  }

  public ValidData(
    string data,
    string proxy,
    ProxyType proxyType,
    BotStatus result,
    string type,
    string capturedData,
    string source,
    List<LogEntry> log)
  {
    this.Data = data;
    this.Proxy = proxy;
    this.Result = result;
    this.Type = type;
    this.CapturedData = capturedData;
    this.UnixDate = (int) Math.Round((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
    this.Source = source;
    this.Log = log;
  }
}
