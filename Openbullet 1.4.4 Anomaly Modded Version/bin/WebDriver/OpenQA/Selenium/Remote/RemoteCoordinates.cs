// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteCoordinates
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Internal;
using System;
using System.Drawing;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class RemoteCoordinates : ICoordinates
{
  private RemoteWebElement element;

  public RemoteCoordinates(RemoteWebElement element) => this.element = element;

  public Point LocationOnScreen => throw new NotImplementedException();

  public Point LocationInViewport => this.element.LocationOnScreenOnceScrolledIntoView;

  public Point LocationInDom => this.element.Location;

  public object AuxiliaryLocator
  {
    get
    {
      IWebElementReference element = (IWebElementReference) this.element;
      return element == null ? (object) null : (object) element.ElementReferenceId;
    }
  }
}
