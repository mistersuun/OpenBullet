// Decompiled with JetBrains decompiler
// Type: RuriLib.ViewModels.SettingsGeneral
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace RuriLib.ViewModels;

public class SettingsGeneral : ViewModelBase
{
  private int _waitTime = 100;
  private int _requestsTimeout = 10;
  private int _maxHits;
  private BotsDisplayMode _botsDisplayMode = BotsDisplayMode.Everything;
  private bool _enableBotLog;
  private bool saveLastSource;
  private bool webhookEnabled;
  private string webhookURL = "";
  private string webhookUser = "Undefined";

  public int WaitTime
  {
    get => this._waitTime;
    set
    {
      this._waitTime = value;
      this.OnPropertyChanged(nameof (WaitTime));
    }
  }

  public int RequestTimeout
  {
    get => this._requestsTimeout;
    set
    {
      this._requestsTimeout = value;
      this.OnPropertyChanged(nameof (RequestTimeout));
    }
  }

  public int MaxHits
  {
    get => this._maxHits;
    set
    {
      this._maxHits = value;
      this.OnPropertyChanged(nameof (MaxHits));
    }
  }

  public BotsDisplayMode BotsDisplayMode
  {
    get => this._botsDisplayMode;
    set
    {
      this._botsDisplayMode = value;
      this.OnPropertyChanged(nameof (BotsDisplayMode));
    }
  }

  public bool EnableBotLog
  {
    get => this._enableBotLog;
    set
    {
      this._enableBotLog = value;
      this.OnPropertyChanged(nameof (EnableBotLog));
    }
  }

  public bool SaveLastSource
  {
    get => this.saveLastSource;
    set
    {
      this.saveLastSource = value;
      this.OnPropertyChanged(nameof (SaveLastSource));
    }
  }

  public bool WebhookEnabled
  {
    get => this.webhookEnabled;
    set
    {
      this.webhookEnabled = value;
      this.OnPropertyChanged(nameof (WebhookEnabled));
    }
  }

  public string WebhookURL
  {
    get => this.webhookURL;
    set
    {
      this.webhookURL = value;
      this.OnPropertyChanged(nameof (WebhookURL));
    }
  }

  public string WebhookUser
  {
    get => this.webhookUser;
    set
    {
      this.webhookUser = value;
      this.OnPropertyChanged(nameof (WebhookUser));
    }
  }

  public void Reset()
  {
    SettingsGeneral settingsGeneral = new SettingsGeneral();
    foreach (PropertyInfo propertyInfo in (IEnumerable<PropertyInfo>) new List<PropertyInfo>((IEnumerable<PropertyInfo>) typeof (SettingsGeneral).GetProperties()))
      propertyInfo.SetValue((object) this, propertyInfo.GetValue((object) settingsGeneral, (object[]) null));
  }
}
