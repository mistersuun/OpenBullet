// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteWindow
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class RemoteWindow : IWindow
{
  private RemoteWebDriver driver;

  public RemoteWindow(RemoteWebDriver driver) => this.driver = driver;

  public Point Position
  {
    get
    {
      Response response;
      if (this.driver.IsSpecificationCompliant)
        response = this.driver.InternalExecute(DriverCommand.GetWindowRect, (Dictionary<string, object>) null);
      else
        response = this.driver.InternalExecute(DriverCommand.GetWindowPosition, new Dictionary<string, object>()
        {
          {
            "windowHandle",
            (object) "current"
          }
        });
      Dictionary<string, object> dictionary = (Dictionary<string, object>) response.Value;
      return new Point(Convert.ToInt32(dictionary["x"], (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(dictionary["y"], (IFormatProvider) CultureInfo.InvariantCulture));
    }
    set
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      parameters.Add("x", (object) value.X);
      parameters.Add("y", (object) value.Y);
      if (this.driver.IsSpecificationCompliant)
      {
        this.driver.InternalExecute(DriverCommand.SetWindowRect, parameters);
      }
      else
      {
        parameters.Add("windowHandle", (object) "current");
        this.driver.InternalExecute(DriverCommand.SetWindowPosition, parameters);
      }
    }
  }

  public Size Size
  {
    get
    {
      Response response;
      if (this.driver.IsSpecificationCompliant)
        response = this.driver.InternalExecute(DriverCommand.GetWindowRect, (Dictionary<string, object>) null);
      else
        response = this.driver.InternalExecute(DriverCommand.GetWindowSize, new Dictionary<string, object>()
        {
          {
            "windowHandle",
            (object) "current"
          }
        });
      Dictionary<string, object> dictionary = (Dictionary<string, object>) response.Value;
      int int32 = Convert.ToInt32(dictionary["height"], (IFormatProvider) CultureInfo.InvariantCulture);
      return new Size(Convert.ToInt32(dictionary["width"], (IFormatProvider) CultureInfo.InvariantCulture), int32);
    }
    set
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      parameters.Add("width", (object) value.Width);
      parameters.Add("height", (object) value.Height);
      if (this.driver.IsSpecificationCompliant)
      {
        this.driver.InternalExecute(DriverCommand.SetWindowRect, parameters);
      }
      else
      {
        parameters.Add("windowHandle", (object) "current");
        this.driver.InternalExecute(DriverCommand.SetWindowSize, parameters);
      }
    }
  }

  public void Maximize()
  {
    Dictionary<string, object> parameters = (Dictionary<string, object>) null;
    if (!this.driver.IsSpecificationCompliant)
    {
      parameters = new Dictionary<string, object>();
      parameters.Add("windowHandle", (object) "current");
    }
    this.driver.InternalExecute(DriverCommand.MaximizeWindow, parameters);
  }

  public void Minimize()
  {
    Dictionary<string, object> parameters = (Dictionary<string, object>) null;
    this.driver.InternalExecute(DriverCommand.MinimizeWindow, parameters);
  }

  public void FullScreen()
  {
    Dictionary<string, object> parameters = (Dictionary<string, object>) null;
    this.driver.InternalExecute(DriverCommand.FullScreenWindow, parameters);
  }
}
