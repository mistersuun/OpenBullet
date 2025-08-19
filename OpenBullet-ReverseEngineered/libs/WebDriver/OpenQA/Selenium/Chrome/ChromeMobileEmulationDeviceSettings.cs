// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Chrome.ChromeMobileEmulationDeviceSettings
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium.Chrome;

public class ChromeMobileEmulationDeviceSettings
{
  private string userAgent = string.Empty;
  private long width;
  private long height;
  private double pixelRatio;
  private bool enableTouchEvents = true;

  public ChromeMobileEmulationDeviceSettings()
  {
  }

  public ChromeMobileEmulationDeviceSettings(string userAgent) => this.userAgent = userAgent;

  public string UserAgent
  {
    get => this.userAgent;
    set => this.userAgent = value;
  }

  public long Width
  {
    get => this.width;
    set => this.width = value;
  }

  public long Height
  {
    get => this.height;
    set => this.height = value;
  }

  public double PixelRatio
  {
    get => this.pixelRatio;
    set => this.pixelRatio = value;
  }

  public bool EnableTouchEvents
  {
    get => this.enableTouchEvents;
    set => this.enableTouchEvents = value;
  }
}
