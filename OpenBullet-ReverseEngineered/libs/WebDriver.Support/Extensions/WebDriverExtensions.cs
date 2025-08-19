// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.Extensions.WebDriverExtensions
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using OpenQA.Selenium.Remote;
using System;
using System.Reflection;

#nullable disable
namespace OpenQA.Selenium.Support.Extensions;

public static class WebDriverExtensions
{
  public static Screenshot TakeScreenshot(this IWebDriver driver)
  {
    switch (driver)
    {
      case ITakesScreenshot takesScreenshot:
        return takesScreenshot.GetScreenshot();
      case IHasCapabilities hasCapabilities:
        if (!hasCapabilities.Capabilities.HasCapability(CapabilityType.TakesScreenshot) || !(bool) hasCapabilities.Capabilities.GetCapability(CapabilityType.TakesScreenshot))
          throw new WebDriverException("Driver capabilities do not support taking screenshots");
        if (!(driver.GetType().GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object) driver, new object[2]
        {
          (object) DriverCommand.Screenshot,
          null
        }) is Response response))
          throw new WebDriverException("Unexpected failure getting screenshot; response was not in the proper format.");
        return new Screenshot(response.Value.ToString());
      default:
        throw new WebDriverException("Driver does not implement ITakesScreenshot or IHasCapabilities");
    }
  }

  public static void ExecuteJavaScript(this IWebDriver driver, string script, params object[] args)
  {
    WebDriverExtensions.ExecuteJavaScriptInternal(driver, script, args);
  }

  public static T ExecuteJavaScript<T>(this IWebDriver driver, string script, params object[] args)
  {
    object o = WebDriverExtensions.ExecuteJavaScriptInternal(driver, script, args);
    T obj = default (T);
    Type nullableType = typeof (T);
    if (o == null)
    {
      if (nullableType.IsValueType && Nullable.GetUnderlyingType(nullableType) == (Type) null)
        throw new WebDriverException("Script returned null, but desired type is a value type");
    }
    else
      obj = nullableType.IsInstanceOfType(o) ? (T) o : throw new WebDriverException("Script returned a value, but the result could not be cast to the desired type");
    return obj;
  }

  private static object ExecuteJavaScriptInternal(IWebDriver driver, string script, object[] args)
  {
    if (!(driver is IJavaScriptExecutor javaScriptExecutor))
      throw new WebDriverException("Driver does not implement IJavaScriptExecutor");
    return javaScriptExecutor.ExecuteScript(script, args);
  }
}
