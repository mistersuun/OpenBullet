// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteTargetLocator
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class RemoteTargetLocator : ITargetLocator
{
  private RemoteWebDriver driver;

  public RemoteTargetLocator(RemoteWebDriver driver) => this.driver = driver;

  public IWebDriver Frame(int frameIndex)
  {
    this.driver.InternalExecute(DriverCommand.SwitchToFrame, new Dictionary<string, object>()
    {
      {
        "id",
        (object) frameIndex
      }
    });
    return (IWebDriver) this.driver;
  }

  public IWebDriver Frame(string frameName)
  {
    string str = frameName != null ? Regex.Replace(frameName, "(['\"\\\\#.:;,!?+<>=~*^$|%&@`{}\\-/\\[\\]\\(\\)])", "\\$1") : throw new ArgumentNullException(nameof (frameName), "Frame name cannot be null");
    ReadOnlyCollection<IWebElement> elements = this.driver.FindElements(By.CssSelector($"frame[name='{str}'],iframe[name='{str}']"));
    if (elements.Count == 0)
    {
      elements = this.driver.FindElements(By.CssSelector($"frame#{str},iframe#{str}"));
      if (elements.Count == 0)
        throw new NoSuchFrameException("No frame element found with name or id " + frameName);
    }
    return this.Frame(elements[0]);
  }

  public IWebDriver Frame(IWebElement frameElement)
  {
    if (frameElement == null)
      throw new ArgumentNullException(nameof (frameElement), "Frame element cannot be null");
    if (!(frameElement is IWebElementReference elementReference) && frameElement is IWrapsElement wrapsElement)
      elementReference = wrapsElement.WrappedElement as IWebElementReference;
    Dictionary<string, object> dictionary = elementReference != null ? elementReference.ToDictionary() : throw new ArgumentException("frameElement cannot be converted to IWebElementReference", nameof (frameElement));
    dictionary.Add("ELEMENT", (object) elementReference.ElementReferenceId);
    this.driver.InternalExecute(DriverCommand.SwitchToFrame, new Dictionary<string, object>()
    {
      {
        "id",
        (object) dictionary
      }
    });
    return (IWebDriver) this.driver;
  }

  public IWebDriver ParentFrame()
  {
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    this.driver.InternalExecute(DriverCommand.SwitchToParentFrame, parameters);
    return (IWebDriver) this.driver;
  }

  public IWebDriver Window(string windowHandleOrName)
  {
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    if (this.driver.IsSpecificationCompliant)
    {
      parameters.Add("handle", (object) windowHandleOrName);
      try
      {
        this.driver.InternalExecute(DriverCommand.SwitchToWindow, parameters);
        return (IWebDriver) this.driver;
      }
      catch (NoSuchWindowException ex1)
      {
        string windowHandleOrName1 = (string) null;
        try
        {
          windowHandleOrName1 = this.driver.CurrentWindowHandle;
        }
        catch (NoSuchWindowException ex2)
        {
        }
        foreach (string windowHandle in this.driver.WindowHandles)
        {
          this.Window(windowHandle);
          if (windowHandleOrName == this.driver.ExecuteScript("return window.name", new object[0]).ToString())
            return (IWebDriver) this.driver;
        }
        if (windowHandleOrName1 != null)
          this.Window(windowHandleOrName1);
        throw;
      }
    }
    else
    {
      parameters.Add("name", (object) windowHandleOrName);
      this.driver.InternalExecute(DriverCommand.SwitchToWindow, parameters);
      return (IWebDriver) this.driver;
    }
  }

  public IWebDriver DefaultContent()
  {
    this.driver.InternalExecute(DriverCommand.SwitchToFrame, new Dictionary<string, object>()
    {
      {
        "id",
        (object) null
      }
    });
    return (IWebDriver) this.driver;
  }

  public IWebElement ActiveElement()
  {
    return this.driver.GetElementFromResponse(this.driver.InternalExecute(DriverCommand.GetActiveElement, (Dictionary<string, object>) null));
  }

  public IAlert Alert()
  {
    this.driver.InternalExecute(DriverCommand.GetAlertText, (Dictionary<string, object>) null);
    return (IAlert) new RemoteAlert(this.driver);
  }
}
