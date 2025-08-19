// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteTouchScreen
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;
using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class RemoteTouchScreen : ITouchScreen
{
  private RemoteWebDriver driver;

  public RemoteTouchScreen(RemoteWebDriver driver) => this.driver = driver;

  public void SingleTap(ICoordinates where)
  {
    string str = where != null ? where.AuxiliaryLocator.ToString() : throw new ArgumentNullException(nameof (where), "where coordinates cannot be null");
    this.driver.InternalExecute(DriverCommand.TouchSingleTap, new Dictionary<string, object>()
    {
      {
        "element",
        (object) str
      }
    });
  }

  public void Down(int locationX, int locationY)
  {
    this.driver.InternalExecute(DriverCommand.TouchPress, new Dictionary<string, object>()
    {
      {
        "x",
        (object) locationX
      },
      {
        "y",
        (object) locationY
      }
    });
  }

  public void Up(int locationX, int locationY)
  {
    this.driver.InternalExecute(DriverCommand.TouchRelease, new Dictionary<string, object>()
    {
      {
        "x",
        (object) locationX
      },
      {
        "y",
        (object) locationY
      }
    });
  }

  public void Move(int locationX, int locationY)
  {
    this.driver.InternalExecute(DriverCommand.TouchMove, new Dictionary<string, object>()
    {
      {
        "x",
        (object) locationX
      },
      {
        "y",
        (object) locationY
      }
    });
  }

  public void Scroll(ICoordinates where, int offsetX, int offsetY)
  {
    string str = where != null ? where.AuxiliaryLocator.ToString() : throw new ArgumentNullException(nameof (where), "where coordinates cannot be null");
    this.driver.InternalExecute(DriverCommand.TouchScroll, new Dictionary<string, object>()
    {
      {
        "element",
        (object) str
      },
      {
        "xoffset",
        (object) offsetX
      },
      {
        "yoffset",
        (object) offsetY
      }
    });
  }

  public void Scroll(int offsetX, int offsetY)
  {
    this.driver.InternalExecute(DriverCommand.TouchScroll, new Dictionary<string, object>()
    {
      {
        "xoffset",
        (object) offsetX
      },
      {
        "yoffset",
        (object) offsetY
      }
    });
  }

  public void DoubleTap(ICoordinates where)
  {
    string str = where != null ? where.AuxiliaryLocator.ToString() : throw new ArgumentNullException(nameof (where), "where coordinates cannot be null");
    this.driver.InternalExecute(DriverCommand.TouchDoubleTap, new Dictionary<string, object>()
    {
      {
        "element",
        (object) str
      }
    });
  }

  public void LongPress(ICoordinates where)
  {
    string str = where != null ? where.AuxiliaryLocator.ToString() : throw new ArgumentNullException(nameof (where), "where coordinates cannot be null");
    this.driver.InternalExecute(DriverCommand.TouchLongPress, new Dictionary<string, object>()
    {
      {
        "element",
        (object) str
      }
    });
  }

  public void Flick(int speedX, int speedY)
  {
    this.driver.InternalExecute(DriverCommand.TouchFlick, new Dictionary<string, object>()
    {
      {
        "xspeed",
        (object) speedX
      },
      {
        "yspeed",
        (object) speedY
      }
    });
  }

  public void Flick(ICoordinates where, int offsetX, int offsetY, int speed)
  {
    string str = where != null ? where.AuxiliaryLocator.ToString() : throw new ArgumentNullException(nameof (where), "where coordinates cannot be null");
    this.driver.InternalExecute(DriverCommand.TouchFlick, new Dictionary<string, object>()
    {
      {
        "element",
        (object) str
      },
      {
        "xoffset",
        (object) offsetX
      },
      {
        "yoffset",
        (object) offsetY
      },
      {
        nameof (speed),
        (object) speed
      }
    });
  }
}
