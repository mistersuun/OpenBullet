// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteWebElement
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class RemoteWebElement : 
  IWebElement,
  ISearchContext,
  IFindsByLinkText,
  IFindsById,
  IFindsByName,
  IFindsByTagName,
  IFindsByClassName,
  IFindsByXPath,
  IFindsByPartialLinkText,
  IFindsByCssSelector,
  IWrapsDriver,
  ILocatable,
  ITakesScreenshot,
  IWebElementReference
{
  public const string ElementReferencePropertyName = "element-6066-11e4-a52e-4f735466cecf";
  public const string LegacyElementReferencePropertyName = "ELEMENT";
  private RemoteWebDriver driver;
  private string elementId;

  public RemoteWebElement(RemoteWebDriver parentDriver, string id)
  {
    this.driver = parentDriver;
    this.elementId = id;
  }

  public IWebDriver WrappedDriver => (IWebDriver) this.driver;

  public virtual string TagName
  {
    get
    {
      return this.Execute(DriverCommand.GetElementTagName, new Dictionary<string, object>()
      {
        {
          "id",
          (object) this.elementId
        }
      }).Value.ToString();
    }
  }

  public virtual string Text
  {
    get
    {
      return this.Execute(DriverCommand.GetElementText, new Dictionary<string, object>()
      {
        {
          "id",
          (object) this.elementId
        }
      }).Value.ToString();
    }
  }

  public virtual bool Enabled
  {
    get
    {
      return (bool) this.Execute(DriverCommand.IsElementEnabled, new Dictionary<string, object>()
      {
        {
          "id",
          (object) this.elementId
        }
      }).Value;
    }
  }

  public virtual bool Selected
  {
    get
    {
      return (bool) this.Execute(DriverCommand.IsElementSelected, new Dictionary<string, object>()
      {
        {
          "id",
          (object) this.elementId
        }
      }).Value;
    }
  }

  public virtual Point Location
  {
    get
    {
      string commandToExecute = DriverCommand.GetElementLocation;
      if (this.driver.IsSpecificationCompliant)
        commandToExecute = DriverCommand.GetElementRect;
      Dictionary<string, object> dictionary = (Dictionary<string, object>) this.Execute(commandToExecute, new Dictionary<string, object>()
      {
        {
          "id",
          (object) this.Id
        }
      }).Value;
      return new Point(Convert.ToInt32(dictionary["x"], (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(dictionary["y"], (IFormatProvider) CultureInfo.InvariantCulture));
    }
  }

  public virtual Size Size
  {
    get
    {
      string commandToExecute = DriverCommand.GetElementSize;
      if (this.driver.IsSpecificationCompliant)
        commandToExecute = DriverCommand.GetElementRect;
      Dictionary<string, object> dictionary = (Dictionary<string, object>) this.Execute(commandToExecute, new Dictionary<string, object>()
      {
        {
          "id",
          (object) this.Id
        }
      }).Value;
      return new Size(Convert.ToInt32(dictionary["width"], (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(dictionary["height"], (IFormatProvider) CultureInfo.InvariantCulture));
    }
  }

  public virtual bool Displayed
  {
    get
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      Response response;
      if (this.driver.IsSpecificationCompliant)
      {
        string atom = RemoteWebElement.GetAtom("isDisplayed.js");
        parameters.Add("script", (object) atom);
        parameters.Add("args", (object) new object[1]
        {
          (object) this.ToElementReference().ToDictionary()
        });
        response = this.Execute(DriverCommand.ExecuteScript, parameters);
      }
      else
      {
        parameters.Add("id", (object) this.Id);
        response = this.Execute(DriverCommand.IsElementDisplayed, parameters);
      }
      return (bool) response.Value;
    }
  }

  public virtual Point LocationOnScreenOnceScrolledIntoView
  {
    get
    {
      Dictionary<string, object> dictionary;
      if (this.driver.IsSpecificationCompliant)
        dictionary = this.driver.ExecuteScript("var rect = arguments[0].getBoundingClientRect(); return {'x': rect.left, 'y': rect.top};", new object[1]
        {
          (object) this
        }) as Dictionary<string, object>;
      else
        dictionary = (Dictionary<string, object>) this.Execute(DriverCommand.GetElementLocationOnceScrolledIntoView, new Dictionary<string, object>()
        {
          {
            "id",
            (object) this.Id
          }
        }).Value;
      return new Point(Convert.ToInt32(dictionary["x"], (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(dictionary["y"], (IFormatProvider) CultureInfo.InvariantCulture));
    }
  }

  public virtual ICoordinates Coordinates => (ICoordinates) new RemoteCoordinates(this);

  string IWebElementReference.ElementReferenceId => this.elementId;

  protected string Id => this.elementId;

  public virtual void Clear()
  {
    this.Execute(DriverCommand.ClearElement, new Dictionary<string, object>()
    {
      {
        "id",
        (object) this.elementId
      }
    });
  }

  public virtual void SendKeys(string text)
  {
    if (text == null)
      throw new ArgumentNullException(nameof (text), "text cannot be null");
    if (this.driver.FileDetector.IsFile(text))
      text = this.UploadFile(text);
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    parameters.Add("id", (object) this.elementId);
    if (this.driver.IsSpecificationCompliant)
    {
      parameters.Add(nameof (text), (object) text);
      parameters.Add("value", (object) text.ToCharArray());
    }
    else
      parameters.Add("value", (object) new object[1]
      {
        (object) text
      });
    this.Execute(DriverCommand.SendKeysToElement, parameters);
  }

  public virtual void Submit()
  {
    if (this.driver.IsSpecificationCompliant)
    {
      switch (this.GetAttribute("type"))
      {
        case "submit":
          this.Click();
          break;
        default:
          this.driver.ExecuteScript("var e = arguments[0].ownerDocument.createEvent('Event');e.initEvent('submit', true, true);if (arguments[0].dispatchEvent(e)) { arguments[0].submit(); }", new object[1]
          {
            (object) this.FindElement(By.XPath("./ancestor-or-self::form"))
          });
          break;
      }
    }
    else
      this.Execute(DriverCommand.SubmitElement, new Dictionary<string, object>()
      {
        {
          "id",
          (object) this.elementId
        }
      });
  }

  public virtual void Click()
  {
    this.Execute(DriverCommand.ClickElement, new Dictionary<string, object>()
    {
      {
        "id",
        (object) this.elementId
      }
    });
  }

  public virtual string GetAttribute(string attributeName)
  {
    string empty = string.Empty;
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    Response response;
    if (this.driver.IsSpecificationCompliant)
    {
      string atom = RemoteWebElement.GetAtom("getAttribute.js");
      parameters.Add("script", (object) atom);
      parameters.Add("args", (object) new object[2]
      {
        (object) this.ToElementReference().ToDictionary(),
        (object) attributeName
      });
      response = this.Execute(DriverCommand.ExecuteScript, parameters);
    }
    else
    {
      parameters.Add("id", (object) this.elementId);
      parameters.Add("name", (object) attributeName);
      response = this.Execute(DriverCommand.GetElementAttribute, parameters);
    }
    string attribute;
    if (response.Value == null)
    {
      attribute = (string) null;
    }
    else
    {
      attribute = response.Value.ToString();
      if (response.Value is bool)
        attribute = attribute.ToLowerInvariant();
    }
    return attribute;
  }

  public virtual string GetProperty(string propertyName)
  {
    string empty = string.Empty;
    Response response = this.Execute(DriverCommand.GetElementProperty, new Dictionary<string, object>()
    {
      {
        "id",
        (object) this.Id
      },
      {
        "name",
        (object) propertyName
      }
    });
    return response.Value != null ? response.Value.ToString() : (string) null;
  }

  public virtual string GetCssValue(string propertyName)
  {
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    parameters.Add("id", (object) this.Id);
    if (this.driver.IsSpecificationCompliant)
      parameters.Add("name", (object) propertyName);
    else
      parameters.Add(nameof (propertyName), (object) propertyName);
    return this.Execute(DriverCommand.GetElementValueOfCssProperty, parameters).Value.ToString();
  }

  public virtual ReadOnlyCollection<IWebElement> FindElements(By by)
  {
    return !(by == (By) null) ? by.FindElements((ISearchContext) this) : throw new ArgumentNullException(nameof (by), "by cannot be null");
  }

  public virtual IWebElement FindElement(By by)
  {
    return !(by == (By) null) ? by.FindElement((ISearchContext) this) : throw new ArgumentNullException(nameof (by), "by cannot be null");
  }

  public virtual IWebElement FindElementByLinkText(string linkText)
  {
    return this.FindElement("link text", linkText);
  }

  public virtual ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText)
  {
    return this.FindElements("link text", linkText);
  }

  public virtual IWebElement FindElementById(string id)
  {
    return this.driver.IsSpecificationCompliant ? this.FindElement("css selector", "#" + RemoteWebDriver.EscapeCssSelector(id)) : this.FindElement(nameof (id), id);
  }

  public virtual ReadOnlyCollection<IWebElement> FindElementsById(string id)
  {
    return this.driver.IsSpecificationCompliant ? this.FindElements("css selector", "#" + RemoteWebDriver.EscapeCssSelector(id)) : this.FindElements(nameof (id), id);
  }

  public virtual IWebElement FindElementByName(string name)
  {
    return this.driver.IsSpecificationCompliant ? this.FindElement("css selector", $"*[name=\"{name}\"]") : this.FindElement(nameof (name), name);
  }

  public virtual ReadOnlyCollection<IWebElement> FindElementsByName(string name)
  {
    return this.driver.IsSpecificationCompliant ? this.FindElements("css selector", $"*[name=\"{name}\"]") : this.FindElements(nameof (name), name);
  }

  public virtual IWebElement FindElementByTagName(string tagName)
  {
    return this.driver.IsSpecificationCompliant ? this.FindElement("css selector", tagName) : this.FindElement("tag name", tagName);
  }

  public virtual ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
  {
    return this.driver.IsSpecificationCompliant ? this.FindElements("css selector", tagName) : this.FindElements("tag name", tagName);
  }

  public virtual IWebElement FindElementByClassName(string className)
  {
    return this.driver.IsSpecificationCompliant ? this.FindElement("css selector", "." + RemoteWebDriver.EscapeCssSelector(className)) : this.FindElement("class name", className);
  }

  public virtual ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
  {
    return this.driver.IsSpecificationCompliant ? this.FindElements("css selector", "." + RemoteWebDriver.EscapeCssSelector(className)) : this.FindElements("class name", className);
  }

  public virtual IWebElement FindElementByXPath(string xpath)
  {
    return this.FindElement(nameof (xpath), xpath);
  }

  public virtual ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
  {
    return this.FindElements(nameof (xpath), xpath);
  }

  public virtual IWebElement FindElementByPartialLinkText(string partialLinkText)
  {
    return this.FindElement("partial link text", partialLinkText);
  }

  public virtual ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(
    string partialLinkText)
  {
    return this.FindElements("partial link text", partialLinkText);
  }

  public virtual IWebElement FindElementByCssSelector(string cssSelector)
  {
    return this.FindElement("css selector", cssSelector);
  }

  public virtual ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
  {
    return this.FindElements("css selector", cssSelector);
  }

  public virtual Screenshot GetScreenshot()
  {
    return new Screenshot(this.Execute(DriverCommand.ElementScreenshot, new Dictionary<string, object>()
    {
      {
        "id",
        (object) this.elementId
      }
    }).Value.ToString());
  }

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Element (id = {0})", (object) this.elementId);
  }

  public override int GetHashCode() => this.elementId.GetHashCode();

  public override bool Equals(object obj)
  {
    if (!(obj is IWebElement webElement))
      return false;
    if (obj is IWrapsElement wrapsElement)
      webElement = wrapsElement.WrappedElement;
    if (!(webElement is RemoteWebElement remoteWebElement))
      return false;
    if (this.elementId == remoteWebElement.Id)
      return true;
    if (!this.driver.IsSpecificationCompliant)
    {
      try
      {
        object obj1 = this.Execute(DriverCommand.ElementEquals, new Dictionary<string, object>()
        {
          {
            "id",
            (object) this.Id
          },
          {
            "other",
            (object) remoteWebElement.Id
          }
        }).Value;
        return obj1 != null && obj1 is bool flag && flag;
      }
      catch (NotImplementedException ex)
      {
      }
    }
    return false;
  }

  Dictionary<string, object> IWebElementReference.ToDictionary()
  {
    return new Dictionary<string, object>()
    {
      {
        "element-6066-11e4-a52e-4f735466cecf",
        (object) this.elementId
      }
    };
  }

  protected virtual IWebElement FindElement(string mechanism, string value)
  {
    return this.driver.GetElementFromResponse(this.Execute(DriverCommand.FindChildElement, new Dictionary<string, object>()
    {
      {
        "id",
        (object) this.elementId
      },
      {
        "using",
        (object) mechanism
      },
      {
        nameof (value),
        (object) value
      }
    }));
  }

  protected virtual ReadOnlyCollection<IWebElement> FindElements(string mechanism, string value)
  {
    return this.driver.GetElementsFromResponse(this.Execute(DriverCommand.FindChildElements, new Dictionary<string, object>()
    {
      {
        "id",
        (object) this.elementId
      },
      {
        "using",
        (object) mechanism
      },
      {
        nameof (value),
        (object) value
      }
    }));
  }

  protected virtual Response Execute(string commandToExecute, Dictionary<string, object> parameters)
  {
    return this.driver.InternalExecute(commandToExecute, parameters);
  }

  private static string GetAtom(string atomResourceName)
  {
    string str = string.Empty;
    using (Stream resourceStream = ResourceUtilities.GetResourceStream(atomResourceName, atomResourceName))
    {
      using (StreamReader streamReader = new StreamReader(resourceStream))
        str = streamReader.ReadToEnd();
    }
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "return ({0}).apply(null, arguments);", (object) str);
  }

  private string UploadFile(string localFile)
  {
    string str = string.Empty;
    try
    {
      using (MemoryStream zipStream = new MemoryStream())
      {
        using (ZipStorer zipStorer = ZipStorer.Create((Stream) zipStream, string.Empty))
        {
          string fileName = Path.GetFileName(localFile);
          zipStorer.AddFile(ZipStorer.CompressionMethod.Deflate, localFile, fileName, string.Empty);
          str = Convert.ToBase64String(zipStream.ToArray());
        }
      }
      return this.Execute(DriverCommand.UploadFile, new Dictionary<string, object>()
      {
        {
          "file",
          (object) str
        }
      }).Value.ToString();
    }
    catch (IOException ex)
    {
      throw new WebDriverException("Cannot upload " + localFile, (Exception) ex);
    }
  }

  private IWebElementReference ToElementReference() => (IWebElementReference) this;
}
