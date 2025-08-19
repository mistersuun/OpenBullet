// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.UI.ExpectedConditions
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace OpenQA.Selenium.Support.UI;

[Obsolete("The ExpectedConditions implementation in the .NET bindings is deprecated and will be removed in a future release. This portion of the code has been migrated to the DotNetSeleniumExtras repository on GitHub (https://github.com/DotNetSeleniumTools/DotNetSeleniumExtras)")]
public sealed class ExpectedConditions
{
  private ExpectedConditions()
  {
  }

  public static Func<IWebDriver, bool> TitleIs(string title)
  {
    return (Func<IWebDriver, bool>) (driver => title == driver.Title);
  }

  public static Func<IWebDriver, bool> TitleContains(string title)
  {
    return (Func<IWebDriver, bool>) (driver => driver.Title.Contains(title));
  }

  public static Func<IWebDriver, bool> UrlToBe(string url)
  {
    return (Func<IWebDriver, bool>) (driver => driver.Url.ToLowerInvariant().Equals(url.ToLowerInvariant()));
  }

  public static Func<IWebDriver, bool> UrlContains(string fraction)
  {
    return (Func<IWebDriver, bool>) (driver => driver.Url.ToLowerInvariant().Contains(fraction.ToLowerInvariant()));
  }

  public static Func<IWebDriver, bool> UrlMatches(string regex)
  {
    return (Func<IWebDriver, bool>) (driver => new Regex(regex, RegexOptions.IgnoreCase).Match(driver.Url).Success);
  }

  public static Func<IWebDriver, IWebElement> ElementExists(By locator)
  {
    return (Func<IWebDriver, IWebElement>) (driver => driver.FindElement(locator));
  }

  public static Func<IWebDriver, IWebElement> ElementIsVisible(By locator)
  {
    return (Func<IWebDriver, IWebElement>) (driver =>
    {
      try
      {
        return ExpectedConditions.ElementIfVisible(driver.FindElement(locator));
      }
      catch (StaleElementReferenceException ex)
      {
        return (IWebElement) null;
      }
    });
  }

  public static Func<IWebDriver, ReadOnlyCollection<IWebElement>> VisibilityOfAllElementsLocatedBy(
    By locator)
  {
    return (Func<IWebDriver, ReadOnlyCollection<IWebElement>>) (driver =>
    {
      try
      {
        ReadOnlyCollection<IWebElement> elements = driver.FindElements(locator);
        return elements.Any<IWebElement>((Func<IWebElement, bool>) (element => !element.Displayed)) ? (ReadOnlyCollection<IWebElement>) null : (elements.Any<IWebElement>() ? elements : (ReadOnlyCollection<IWebElement>) null);
      }
      catch (StaleElementReferenceException ex)
      {
        return (ReadOnlyCollection<IWebElement>) null;
      }
    });
  }

  public static Func<IWebDriver, ReadOnlyCollection<IWebElement>> VisibilityOfAllElementsLocatedBy(
    ReadOnlyCollection<IWebElement> elements)
  {
    return (Func<IWebDriver, ReadOnlyCollection<IWebElement>>) (driver =>
    {
      try
      {
        return elements.Any<IWebElement>((Func<IWebElement, bool>) (element => !element.Displayed)) ? (ReadOnlyCollection<IWebElement>) null : (elements.Any<IWebElement>() ? elements : (ReadOnlyCollection<IWebElement>) null);
      }
      catch (StaleElementReferenceException ex)
      {
        return (ReadOnlyCollection<IWebElement>) null;
      }
    });
  }

  public static Func<IWebDriver, ReadOnlyCollection<IWebElement>> PresenceOfAllElementsLocatedBy(
    By locator)
  {
    return (Func<IWebDriver, ReadOnlyCollection<IWebElement>>) (driver =>
    {
      try
      {
        ReadOnlyCollection<IWebElement> elements = driver.FindElements(locator);
        return elements.Any<IWebElement>() ? elements : (ReadOnlyCollection<IWebElement>) null;
      }
      catch (StaleElementReferenceException ex)
      {
        return (ReadOnlyCollection<IWebElement>) null;
      }
    });
  }

  public static Func<IWebDriver, bool> TextToBePresentInElement(IWebElement element, string text)
  {
    return (Func<IWebDriver, bool>) (driver =>
    {
      try
      {
        return element.Text.Contains(text);
      }
      catch (StaleElementReferenceException ex)
      {
        return false;
      }
    });
  }

  public static Func<IWebDriver, bool> TextToBePresentInElementLocated(By locator, string text)
  {
    return (Func<IWebDriver, bool>) (driver =>
    {
      try
      {
        return driver.FindElement(locator).Text.Contains(text);
      }
      catch (StaleElementReferenceException ex)
      {
        return false;
      }
    });
  }

  public static Func<IWebDriver, bool> TextToBePresentInElementValue(
    IWebElement element,
    string text)
  {
    return (Func<IWebDriver, bool>) (driver =>
    {
      try
      {
        string attribute = element.GetAttribute("value");
        return attribute != null && attribute.Contains(text);
      }
      catch (StaleElementReferenceException ex)
      {
        return false;
      }
    });
  }

  public static Func<IWebDriver, bool> TextToBePresentInElementValue(By locator, string text)
  {
    return (Func<IWebDriver, bool>) (driver =>
    {
      try
      {
        string attribute = driver.FindElement(locator).GetAttribute("value");
        return attribute != null && attribute.Contains(text);
      }
      catch (StaleElementReferenceException ex)
      {
        return false;
      }
    });
  }

  public static Func<IWebDriver, IWebDriver> FrameToBeAvailableAndSwitchToIt(string frameLocator)
  {
    return (Func<IWebDriver, IWebDriver>) (driver =>
    {
      try
      {
        return driver.SwitchTo().Frame(frameLocator);
      }
      catch (NoSuchFrameException ex)
      {
        return (IWebDriver) null;
      }
    });
  }

  public static Func<IWebDriver, IWebDriver> FrameToBeAvailableAndSwitchToIt(By locator)
  {
    return (Func<IWebDriver, IWebDriver>) (driver =>
    {
      try
      {
        IWebElement element = driver.FindElement(locator);
        return driver.SwitchTo().Frame(element);
      }
      catch (NoSuchFrameException ex)
      {
        return (IWebDriver) null;
      }
    });
  }

  public static Func<IWebDriver, bool> InvisibilityOfElementLocated(By locator)
  {
    return (Func<IWebDriver, bool>) (driver =>
    {
      try
      {
        return !driver.FindElement(locator).Displayed;
      }
      catch (NoSuchElementException ex)
      {
        return true;
      }
      catch (StaleElementReferenceException ex)
      {
        return true;
      }
    });
  }

  public static Func<IWebDriver, bool> InvisibilityOfElementWithText(By locator, string text)
  {
    return (Func<IWebDriver, bool>) (driver =>
    {
      try
      {
        string text1 = driver.FindElement(locator).Text;
        return string.IsNullOrEmpty(text1) || !text1.Equals(text);
      }
      catch (NoSuchElementException ex)
      {
        return true;
      }
      catch (StaleElementReferenceException ex)
      {
        return true;
      }
    });
  }

  public static Func<IWebDriver, IWebElement> ElementToBeClickable(By locator)
  {
    return (Func<IWebDriver, IWebElement>) (driver =>
    {
      IWebElement webElement = ExpectedConditions.ElementIfVisible(driver.FindElement(locator));
      try
      {
        return webElement != null && webElement.Enabled ? webElement : (IWebElement) null;
      }
      catch (StaleElementReferenceException ex)
      {
        return (IWebElement) null;
      }
    });
  }

  public static Func<IWebDriver, IWebElement> ElementToBeClickable(IWebElement element)
  {
    return (Func<IWebDriver, IWebElement>) (driver =>
    {
      try
      {
        return element != null && element.Displayed && element.Enabled ? element : (IWebElement) null;
      }
      catch (StaleElementReferenceException ex)
      {
        return (IWebElement) null;
      }
    });
  }

  public static Func<IWebDriver, bool> StalenessOf(IWebElement element)
  {
    return (Func<IWebDriver, bool>) (driver =>
    {
      try
      {
        return element == null || !element.Enabled;
      }
      catch (StaleElementReferenceException ex)
      {
        return true;
      }
    });
  }

  public static Func<IWebDriver, bool> ElementToBeSelected(IWebElement element)
  {
    return ExpectedConditions.ElementSelectionStateToBe(element, true);
  }

  public static Func<IWebDriver, bool> ElementToBeSelected(IWebElement element, bool selected)
  {
    return (Func<IWebDriver, bool>) (driver => element.Selected == selected);
  }

  public static Func<IWebDriver, bool> ElementSelectionStateToBe(IWebElement element, bool selected)
  {
    return (Func<IWebDriver, bool>) (driver => element.Selected == selected);
  }

  public static Func<IWebDriver, bool> ElementToBeSelected(By locator)
  {
    return ExpectedConditions.ElementSelectionStateToBe(locator, true);
  }

  public static Func<IWebDriver, bool> ElementSelectionStateToBe(By locator, bool selected)
  {
    return (Func<IWebDriver, bool>) (driver =>
    {
      try
      {
        return driver.FindElement(locator).Selected == selected;
      }
      catch (StaleElementReferenceException ex)
      {
        return false;
      }
    });
  }

  public static Func<IWebDriver, IAlert> AlertIsPresent()
  {
    return (Func<IWebDriver, IAlert>) (driver =>
    {
      try
      {
        return driver.SwitchTo().Alert();
      }
      catch (NoAlertPresentException ex)
      {
        return (IAlert) null;
      }
    });
  }

  public static Func<IWebDriver, bool> AlertState(bool state)
  {
    return (Func<IWebDriver, bool>) (driver =>
    {
      try
      {
        driver.SwitchTo().Alert();
        return state;
      }
      catch (NoAlertPresentException ex)
      {
        return !state;
      }
    });
  }

  private static IWebElement ElementIfVisible(IWebElement element)
  {
    return !element.Displayed ? (IWebElement) null : element;
  }
}
