// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteWebElementFactory
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class RemoteWebElementFactory
{
  private RemoteWebDriver driver;

  public RemoteWebElementFactory(RemoteWebDriver parentDriver) => this.driver = parentDriver;

  protected RemoteWebDriver ParentDriver => this.driver;

  public virtual RemoteWebElement CreateElement(Dictionary<string, object> elementDictionary)
  {
    return new RemoteWebElement(this.ParentDriver, this.GetElementId(elementDictionary));
  }

  public bool ContainsElementReference(Dictionary<string, object> elementDictionary)
  {
    string elementPropertyName = string.Empty;
    return this.TryGetElementPropertyName(elementDictionary, out elementPropertyName);
  }

  public string GetElementId(Dictionary<string, object> elementDictionary)
  {
    string elementPropertyName = string.Empty;
    string str = this.TryGetElementPropertyName(elementDictionary, out elementPropertyName) ? elementDictionary[elementPropertyName].ToString() : throw new ArgumentException(nameof (elementDictionary), "The specified dictionary does not contain an element reference");
    return !string.IsNullOrEmpty(str) ? str : throw new InvalidOperationException("The specified element ID is either null or the empty string.");
  }

  private bool TryGetElementPropertyName(
    Dictionary<string, object> elementDictionary,
    out string elementPropertyName)
  {
    if (elementDictionary == null)
      throw new ArgumentNullException(nameof (elementDictionary), "The dictionary containing the element reference cannot be null");
    if (elementDictionary.ContainsKey("element-6066-11e4-a52e-4f735466cecf"))
    {
      elementPropertyName = "element-6066-11e4-a52e-4f735466cecf";
      return true;
    }
    if (elementDictionary.ContainsKey("ELEMENT"))
    {
      elementPropertyName = "ELEMENT";
      return true;
    }
    elementPropertyName = string.Empty;
    return false;
  }
}
