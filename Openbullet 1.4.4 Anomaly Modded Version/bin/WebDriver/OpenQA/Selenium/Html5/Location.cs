// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Html5.Location
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Html5;

public class Location
{
  private readonly double latitude;
  private readonly double longitude;
  private readonly double altitude;

  public Location(double latitude, double longitude, double altitude)
  {
    this.latitude = latitude;
    this.longitude = longitude;
    this.altitude = altitude;
  }

  public double Latitude => this.latitude;

  public double Longitude => this.longitude;

  public double Altitude => this.altitude;

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Latitude: {0}, Longitude: {1}, Altitude: {2}", (object) this.latitude, (object) this.longitude, (object) this.altitude);
  }
}
