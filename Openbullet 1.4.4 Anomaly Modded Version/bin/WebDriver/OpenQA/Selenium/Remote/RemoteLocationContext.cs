// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteLocationContext
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Html5;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class RemoteLocationContext : ILocationContext
{
  private RemoteWebDriver driver;

  public RemoteLocationContext(RemoteWebDriver driver) => this.driver = driver;

  public Location PhysicalLocation
  {
    get
    {
      return this.driver.InternalExecute(DriverCommand.GetLocation, (Dictionary<string, object>) null).Value is Dictionary<string, object> dictionary ? new Location(double.Parse(dictionary["latitude"].ToString(), (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(dictionary["longitude"].ToString(), (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(dictionary["altitude"].ToString(), (IFormatProvider) CultureInfo.InvariantCulture)) : (Location) null;
    }
    set
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value), "value cannot be null");
      this.driver.InternalExecute(DriverCommand.SetLocation, new Dictionary<string, object>()
      {
        {
          "location",
          (object) new Dictionary<string, object>()
          {
            {
              "latitude",
              (object) value.Latitude
            },
            {
              "longitude",
              (object) value.Longitude
            },
            {
              "altitude",
              (object) value.Altitude
            }
          }
        }
      });
    }
  }
}
