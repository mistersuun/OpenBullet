// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Platform
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace OpenQA.Selenium;

public class Platform
{
  private static Platform current;
  private PlatformType platformTypeValue;
  private int major;
  private int minor;

  public Platform(PlatformType typeValue) => this.platformTypeValue = typeValue;

  private Platform()
  {
    this.major = Environment.OSVersion.Version.Major;
    this.minor = Environment.OSVersion.Version.Minor;
    switch (Environment.OSVersion.Platform)
    {
      case PlatformID.Win32NT:
        if (this.major == 5)
        {
          this.platformTypeValue = PlatformType.XP;
          break;
        }
        if (this.major == 6)
        {
          this.platformTypeValue = PlatformType.Vista;
          break;
        }
        this.platformTypeValue = PlatformType.Windows;
        break;
      case PlatformID.Unix:
        this.platformTypeValue = PlatformType.Unix;
        break;
      case PlatformID.MacOSX:
        this.platformTypeValue = PlatformType.Mac;
        break;
    }
  }

  public static Platform CurrentPlatform
  {
    get
    {
      if (Platform.current == null)
        Platform.current = new Platform();
      return Platform.current;
    }
  }

  public int MajorVersion => this.major;

  public int MinorVersion => this.minor;

  public PlatformType PlatformType => this.platformTypeValue;

  public string ProtocolPlatformType => this.platformTypeValue.ToString("G").ToUpperInvariant();

  public bool IsPlatformType(PlatformType compareTo)
  {
    bool flag;
    switch (compareTo)
    {
      case PlatformType.Any:
        flag = true;
        break;
      case PlatformType.Windows:
        flag = this.platformTypeValue == PlatformType.Windows || this.platformTypeValue == PlatformType.XP || this.platformTypeValue == PlatformType.Vista;
        break;
      case PlatformType.XP:
        flag = this.platformTypeValue == PlatformType.Windows || this.platformTypeValue == PlatformType.XP;
        break;
      case PlatformType.Vista:
        flag = this.platformTypeValue == PlatformType.Windows || this.platformTypeValue == PlatformType.Vista;
        break;
      case PlatformType.Linux:
        flag = this.platformTypeValue == PlatformType.Linux || this.platformTypeValue == PlatformType.Unix;
        break;
      default:
        flag = this.platformTypeValue == compareTo;
        break;
    }
    return flag;
  }

  public override string ToString() => this.platformTypeValue.ToString();

  internal static Platform FromString(string platformName)
  {
    PlatformType typeValue = PlatformType.Any;
    try
    {
      typeValue = (PlatformType) Enum.Parse(typeof (PlatformType), platformName, true);
    }
    catch (ArgumentException ex)
    {
    }
    return new Platform(typeValue);
  }
}
