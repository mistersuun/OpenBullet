// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.WebElementProxy
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

internal sealed class WebElementProxy : WebDriverObjectProxy, IWrapsElement
{
  private IWebElement cachedElement;

  private WebElementProxy(
    Type classToProxy,
    IElementLocator locator,
    IEnumerable<By> bys,
    bool cache)
    : base(classToProxy, locator, bys, cache)
  {
  }

  public IWebElement WrappedElement => this.Element;

  private IWebElement Element
  {
    get
    {
      if (!this.Cache || this.cachedElement == null)
        this.cachedElement = this.Locator.LocateElement(this.Bys);
      return this.cachedElement;
    }
  }

  public static object CreateProxy(
    Type classToProxy,
    IElementLocator locator,
    IEnumerable<By> bys,
    bool cacheLookups)
  {
    return new WebElementProxy(classToProxy, locator, bys, cacheLookups).GetTransparentProxy();
  }

  public override IMessage Invoke(IMessage msg)
  {
    IWebElement element = this.Element;
    IMethodCallMessage methodCallMessage = msg as IMethodCallMessage;
    return typeof (IWrapsElement).IsAssignableFrom((methodCallMessage.MethodBase as MethodInfo).DeclaringType) ? (IMessage) new ReturnMessage((object) element, (object[]) null, 0, methodCallMessage.LogicalCallContext, methodCallMessage) : (IMessage) WebDriverObjectProxy.InvokeMethod(methodCallMessage, (object) element);
  }
}
