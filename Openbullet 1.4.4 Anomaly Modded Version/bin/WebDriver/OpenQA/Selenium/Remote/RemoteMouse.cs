// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteMouse
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;
using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class RemoteMouse : IMouse
{
  private RemoteWebDriver driver;

  public RemoteMouse(RemoteWebDriver driver) => this.driver = driver;

  public void Click(ICoordinates where)
  {
    this.MoveIfNeeded(where);
    this.driver.InternalExecute(DriverCommand.MouseClick, new Dictionary<string, object>()
    {
      {
        "button",
        (object) 0
      }
    });
  }

  public void DoubleClick(ICoordinates where)
  {
    this.driver.InternalExecute(DriverCommand.MouseDoubleClick, (Dictionary<string, object>) null);
  }

  public void MouseDown(ICoordinates where)
  {
    this.driver.InternalExecute(DriverCommand.MouseDown, (Dictionary<string, object>) null);
  }

  public void MouseUp(ICoordinates where)
  {
    this.driver.InternalExecute(DriverCommand.MouseUp, (Dictionary<string, object>) null);
  }

  public void MouseMove(ICoordinates where)
  {
    string str = where != null ? where.AuxiliaryLocator.ToString() : throw new ArgumentNullException(nameof (where), "where coordinates cannot be null");
    this.driver.InternalExecute(DriverCommand.MouseMoveTo, new Dictionary<string, object>()
    {
      {
        "element",
        (object) str
      }
    });
  }

  public void MouseMove(ICoordinates where, int offsetX, int offsetY)
  {
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    if (where != null)
    {
      string str = where.AuxiliaryLocator.ToString();
      parameters.Add("element", (object) str);
    }
    parameters.Add("xoffset", (object) offsetX);
    parameters.Add("yoffset", (object) offsetY);
    this.driver.InternalExecute(DriverCommand.MouseMoveTo, parameters);
  }

  public void ContextClick(ICoordinates where)
  {
    this.MoveIfNeeded(where);
    this.driver.InternalExecute(DriverCommand.MouseClick, new Dictionary<string, object>()
    {
      {
        "button",
        (object) 2
      }
    });
  }

  private void MoveIfNeeded(ICoordinates where)
  {
    if (where == null)
      return;
    this.MouseMove(where);
  }
}
