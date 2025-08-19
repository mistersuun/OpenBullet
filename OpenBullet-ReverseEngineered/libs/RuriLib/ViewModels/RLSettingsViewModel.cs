// Decompiled with JetBrains decompiler
// Type: RuriLib.ViewModels.RLSettingsViewModel
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

#nullable disable
namespace RuriLib.ViewModels;

public class RLSettingsViewModel
{
  public SettingsGeneral General { get; set; } = new SettingsGeneral();

  public SettingsProxies Proxies { get; set; } = new SettingsProxies();

  public SettingsCaptchas Captchas { get; set; } = new SettingsCaptchas();

  public SettingsSelenium Selenium { get; set; } = new SettingsSelenium();

  public void Reset()
  {
    this.General.Reset();
    this.Proxies.Reset();
    this.Captchas.Reset();
    this.Selenium.Reset();
  }
}
