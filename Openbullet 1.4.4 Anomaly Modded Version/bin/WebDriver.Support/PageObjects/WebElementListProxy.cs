// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.WebElementListProxy
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

internal class WebElementListProxy : WebDriverObjectProxy
{
  private List<IWebElement> collection;

  private WebElementListProxy(
    Type typeToBeProxied,
    IElementLocator locator,
    IEnumerable<By> bys,
    bool cache)
    : base(typeToBeProxied, locator, bys, cache)
  {
  }

  private List<IWebElement> ElementList
  {
    get
    {
      if (!this.Cache || this.collection == null)
      {
        this.collection = new List<IWebElement>();
        this.collection.AddRange((IEnumerable<IWebElement>) this.Locator.LocateElements(this.Bys));
      }
      return this.collection;
    }
  }

  public static object CreateProxy(
    Type classToProxy,
    IElementLocator locator,
    IEnumerable<By> bys,
    bool cacheLookups)
  {
    return new WebElementListProxy(classToProxy, locator, bys, cacheLookups).GetTransparentProxy();
  }

  public override IMessage Invoke(IMessage msg)
  {
    List<IWebElement> elementList = this.ElementList;
    return (IMessage) WebDriverObjectProxy.InvokeMethod(msg as IMethodCallMessage, (object) elementList);
  }
}
