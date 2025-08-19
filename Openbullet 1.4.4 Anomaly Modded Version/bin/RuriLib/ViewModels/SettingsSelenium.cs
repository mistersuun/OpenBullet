// Decompiled with JetBrains decompiler
// Type: RuriLib.ViewModels.SettingsSelenium
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace RuriLib.ViewModels;

public class SettingsSelenium : ViewModelBase
{
  private BrowserType browser;
  private bool headless;
  private string firefoxBinaryLocation = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
  private string chromeBinaryLocation = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
  private List<string> chromeExtensions = new List<string>();
  private bool drawMouseMovement = true;
  private int pageLoadTimeout = 60;

  public BrowserType Browser
  {
    get => this.browser;
    set
    {
      this.browser = value;
      this.OnPropertyChanged(nameof (Browser));
    }
  }

  public bool Headless
  {
    get => this.headless;
    set
    {
      this.headless = value;
      this.OnPropertyChanged(nameof (Headless));
    }
  }

  public string FirefoxBinaryLocation
  {
    get => this.firefoxBinaryLocation;
    set
    {
      this.firefoxBinaryLocation = value;
      this.OnPropertyChanged(nameof (FirefoxBinaryLocation));
    }
  }

  public string ChromeBinaryLocation
  {
    get => this.chromeBinaryLocation;
    set
    {
      this.chromeBinaryLocation = value;
      this.OnPropertyChanged(nameof (ChromeBinaryLocation));
    }
  }

  public List<string> ChromeExtensions
  {
    get => this.chromeExtensions;
    set
    {
      this.chromeExtensions = value;
      this.OnPropertyChanged(nameof (ChromeExtensions));
    }
  }

  public bool DrawMouseMovement
  {
    get => this.drawMouseMovement;
    set
    {
      this.drawMouseMovement = value;
      this.OnPropertyChanged(nameof (DrawMouseMovement));
    }
  }

  public int PageLoadTimeout
  {
    get => this.pageLoadTimeout;
    set
    {
      this.pageLoadTimeout = value;
      this.OnPropertyChanged(nameof (PageLoadTimeout));
    }
  }

  public void Reset()
  {
    SettingsSelenium settingsSelenium = new SettingsSelenium();
    foreach (PropertyInfo propertyInfo in (IEnumerable<PropertyInfo>) new List<PropertyInfo>((IEnumerable<PropertyInfo>) typeof (SettingsSelenium).GetProperties()))
      propertyInfo.SetValue((object) this, propertyInfo.GetValue((object) settingsSelenium, (object[]) null));
  }
}
