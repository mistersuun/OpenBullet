// Decompiled with JetBrains decompiler
// Type: RuriLib.ViewModels.SettingsCaptchas
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.CaptchaServices;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace RuriLib.ViewModels;

public class SettingsCaptchas : ViewModelBase
{
  private ServiceType currentService;
  private string antiCapToken = "";
  private string imageTypToken = "";
  private string dbcUser = "";
  private string dbcPass = "";
  private string twoCapToken = "";
  private string ruCapToken = "";
  private string dcUser = "";
  private string dcPass = "";
  private string azCapToken = "";
  private string srUserId = "";
  private string srToken = "";
  private string cIOToken = "";
  private string cdToken = "";
  private string customTwoCapToken = "";
  private string customTwoCapDomain = "example.com";
  private int customTwoCapPort = 80 /*0x50*/;
  private bool bypassBalanceCheck;
  private int timeout = 120;

  public ServiceType CurrentService
  {
    get => this.currentService;
    set
    {
      this.currentService = value;
      this.OnPropertyChanged(nameof (CurrentService));
    }
  }

  public string AntiCapToken
  {
    get => this.antiCapToken;
    set
    {
      this.antiCapToken = value;
      this.OnPropertyChanged(nameof (AntiCapToken));
    }
  }

  public string ImageTypToken
  {
    get => this.imageTypToken;
    set
    {
      this.imageTypToken = value;
      this.OnPropertyChanged(nameof (ImageTypToken));
    }
  }

  public string DBCUser
  {
    get => this.dbcUser;
    set
    {
      this.dbcUser = value;
      this.OnPropertyChanged(nameof (DBCUser));
    }
  }

  public string DBCPass
  {
    get => this.dbcPass;
    set
    {
      this.dbcPass = value;
      this.OnPropertyChanged(nameof (DBCPass));
    }
  }

  public string TwoCapToken
  {
    get => this.twoCapToken;
    set
    {
      this.twoCapToken = value;
      this.OnPropertyChanged(nameof (TwoCapToken));
    }
  }

  public string RuCapToken
  {
    get => this.ruCapToken;
    set
    {
      this.ruCapToken = value;
      this.OnPropertyChanged(nameof (RuCapToken));
    }
  }

  public string DCUser
  {
    get => this.dcUser;
    set
    {
      this.dcUser = value;
      this.OnPropertyChanged(nameof (DCUser));
    }
  }

  public string DCPass
  {
    get => this.dcPass;
    set
    {
      this.dcPass = value;
      this.OnPropertyChanged(nameof (DCPass));
    }
  }

  public string AZCapToken
  {
    get => this.azCapToken;
    set
    {
      this.azCapToken = value;
      this.OnPropertyChanged(nameof (AZCapToken));
    }
  }

  public string SRUserId
  {
    get => this.srUserId;
    set
    {
      this.srUserId = value;
      this.OnPropertyChanged(nameof (SRUserId));
    }
  }

  public string SRToken
  {
    get => this.srToken;
    set
    {
      this.srToken = value;
      this.OnPropertyChanged(nameof (SRToken));
    }
  }

  public string CIOToken
  {
    get => this.cIOToken;
    set
    {
      this.cIOToken = value;
      this.OnPropertyChanged(nameof (CIOToken));
    }
  }

  public string CDToken
  {
    get => this.cdToken;
    set
    {
      this.cdToken = value;
      this.OnPropertyChanged(nameof (CDToken));
    }
  }

  public string CustomTwoCapToken
  {
    get => this.customTwoCapToken;
    set
    {
      this.customTwoCapToken = value;
      this.OnPropertyChanged(nameof (CustomTwoCapToken));
    }
  }

  public string CustomTwoCapDomain
  {
    get => this.customTwoCapDomain;
    set
    {
      this.customTwoCapDomain = value;
      this.OnPropertyChanged(nameof (CustomTwoCapDomain));
    }
  }

  public int CustomTwoCapPort
  {
    get => this.customTwoCapPort;
    set
    {
      this.customTwoCapPort = value;
      this.OnPropertyChanged(nameof (CustomTwoCapPort));
    }
  }

  public bool BypassBalanceCheck
  {
    get => this.bypassBalanceCheck;
    set
    {
      this.bypassBalanceCheck = value;
      this.OnPropertyChanged(nameof (BypassBalanceCheck));
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

  public void Reset()
  {
    SettingsCaptchas settingsCaptchas = new SettingsCaptchas();
    foreach (PropertyInfo propertyInfo in (IEnumerable<PropertyInfo>) new List<PropertyInfo>((IEnumerable<PropertyInfo>) typeof (SettingsCaptchas).GetProperties()))
      propertyInfo.SetValue((object) this, propertyInfo.GetValue((object) settingsCaptchas, (object[]) null));
  }
}
