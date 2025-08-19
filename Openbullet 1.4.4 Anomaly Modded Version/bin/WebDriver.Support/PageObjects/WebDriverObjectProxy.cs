// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.WebDriverObjectProxy
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

internal abstract class WebDriverObjectProxy : RealProxy
{
  private readonly IElementLocator locator;
  private readonly IEnumerable<By> bys;
  private readonly bool cache;

  protected WebDriverObjectProxy(
    Type classToProxy,
    IElementLocator locator,
    IEnumerable<By> bys,
    bool cache)
    : base(classToProxy)
  {
    this.locator = locator;
    this.bys = bys;
    this.cache = cache;
  }

  protected IElementLocator Locator => this.locator;

  protected IEnumerable<By> Bys => this.bys;

  protected bool Cache => this.cache;

  protected static ReturnMessage InvokeMethod(IMethodCallMessage msg, object representedValue)
  {
    if (msg == null)
      throw new ArgumentNullException(nameof (msg), "The message containing invocation information cannot be null");
    return new ReturnMessage((msg.MethodBase as MethodInfo).Invoke(representedValue, msg.Args), (object[]) null, 0, msg.LogicalCallContext, msg);
  }
}
