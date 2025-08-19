// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.By
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Collections.ObjectModel;

#nullable disable
namespace OpenQA.Selenium;

[Serializable]
public class By
{
  private string description = "OpenQA.Selenium.By";
  private Func<ISearchContext, IWebElement> findElementMethod;
  private Func<ISearchContext, ReadOnlyCollection<IWebElement>> findElementsMethod;

  protected By()
  {
  }

  protected By(
    Func<ISearchContext, IWebElement> findElementMethod,
    Func<ISearchContext, ReadOnlyCollection<IWebElement>> findElementsMethod)
  {
    this.findElementMethod = findElementMethod;
    this.findElementsMethod = findElementsMethod;
  }

  protected string Description
  {
    get => this.description;
    set => this.description = value;
  }

  protected Func<ISearchContext, IWebElement> FindElementMethod
  {
    get => this.findElementMethod;
    set => this.findElementMethod = value;
  }

  protected Func<ISearchContext, ReadOnlyCollection<IWebElement>> FindElementsMethod
  {
    get => this.findElementsMethod;
    set => this.findElementsMethod = value;
  }

  public static bool operator ==(By one, By two)
  {
    if ((object) one == (object) two)
      return true;
    return (object) one != null && (object) two != null && one.Equals((object) two);
  }

  public static bool operator !=(By one, By two) => !(one == two);

  public static By Id(string idToFind)
  {
    if (idToFind == null)
      throw new ArgumentNullException(nameof (idToFind), "Cannot find elements with a null id attribute.");
    return new By()
    {
      findElementMethod = (Func<ISearchContext, IWebElement>) (context => ((IFindsById) context).FindElementById(idToFind)),
      findElementsMethod = (Func<ISearchContext, ReadOnlyCollection<IWebElement>>) (context => ((IFindsById) context).FindElementsById(idToFind)),
      description = "By.Id: " + idToFind
    };
  }

  public static By LinkText(string linkTextToFind)
  {
    if (linkTextToFind == null)
      throw new ArgumentNullException(nameof (linkTextToFind), "Cannot find elements when link text is null.");
    return new By()
    {
      findElementMethod = (Func<ISearchContext, IWebElement>) (context => ((IFindsByLinkText) context).FindElementByLinkText(linkTextToFind)),
      findElementsMethod = (Func<ISearchContext, ReadOnlyCollection<IWebElement>>) (context => ((IFindsByLinkText) context).FindElementsByLinkText(linkTextToFind)),
      description = "By.LinkText: " + linkTextToFind
    };
  }

  public static By Name(string nameToFind)
  {
    if (nameToFind == null)
      throw new ArgumentNullException(nameof (nameToFind), "Cannot find elements when name text is null.");
    return new By()
    {
      findElementMethod = (Func<ISearchContext, IWebElement>) (context => ((IFindsByName) context).FindElementByName(nameToFind)),
      findElementsMethod = (Func<ISearchContext, ReadOnlyCollection<IWebElement>>) (context => ((IFindsByName) context).FindElementsByName(nameToFind)),
      description = "By.Name: " + nameToFind
    };
  }

  public static By XPath(string xpathToFind)
  {
    if (xpathToFind == null)
      throw new ArgumentNullException(nameof (xpathToFind), "Cannot find elements when the XPath expression is null.");
    return new By()
    {
      findElementMethod = (Func<ISearchContext, IWebElement>) (context => ((IFindsByXPath) context).FindElementByXPath(xpathToFind)),
      findElementsMethod = (Func<ISearchContext, ReadOnlyCollection<IWebElement>>) (context => ((IFindsByXPath) context).FindElementsByXPath(xpathToFind)),
      description = "By.XPath: " + xpathToFind
    };
  }

  public static By ClassName(string classNameToFind)
  {
    if (classNameToFind == null)
      throw new ArgumentNullException(nameof (classNameToFind), "Cannot find elements when the class name expression is null.");
    return new By()
    {
      findElementMethod = (Func<ISearchContext, IWebElement>) (context => ((IFindsByClassName) context).FindElementByClassName(classNameToFind)),
      findElementsMethod = (Func<ISearchContext, ReadOnlyCollection<IWebElement>>) (context => ((IFindsByClassName) context).FindElementsByClassName(classNameToFind)),
      description = "By.ClassName[Contains]: " + classNameToFind
    };
  }

  public static By PartialLinkText(string partialLinkTextToFind)
  {
    return new By()
    {
      findElementMethod = (Func<ISearchContext, IWebElement>) (context => ((IFindsByPartialLinkText) context).FindElementByPartialLinkText(partialLinkTextToFind)),
      findElementsMethod = (Func<ISearchContext, ReadOnlyCollection<IWebElement>>) (context => ((IFindsByPartialLinkText) context).FindElementsByPartialLinkText(partialLinkTextToFind)),
      description = "By.PartialLinkText: " + partialLinkTextToFind
    };
  }

  public static By TagName(string tagNameToFind)
  {
    if (tagNameToFind == null)
      throw new ArgumentNullException(nameof (tagNameToFind), "Cannot find elements when name tag name is null.");
    return new By()
    {
      findElementMethod = (Func<ISearchContext, IWebElement>) (context => ((IFindsByTagName) context).FindElementByTagName(tagNameToFind)),
      findElementsMethod = (Func<ISearchContext, ReadOnlyCollection<IWebElement>>) (context => ((IFindsByTagName) context).FindElementsByTagName(tagNameToFind)),
      description = "By.TagName: " + tagNameToFind
    };
  }

  public static By CssSelector(string cssSelectorToFind)
  {
    if (cssSelectorToFind == null)
      throw new ArgumentNullException(nameof (cssSelectorToFind), "Cannot find elements when name CSS selector is null.");
    return new By()
    {
      findElementMethod = (Func<ISearchContext, IWebElement>) (context => ((IFindsByCssSelector) context).FindElementByCssSelector(cssSelectorToFind)),
      findElementsMethod = (Func<ISearchContext, ReadOnlyCollection<IWebElement>>) (context => ((IFindsByCssSelector) context).FindElementsByCssSelector(cssSelectorToFind)),
      description = "By.CssSelector: " + cssSelectorToFind
    };
  }

  public virtual IWebElement FindElement(ISearchContext context) => this.findElementMethod(context);

  public virtual ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
  {
    return this.findElementsMethod(context);
  }

  public override string ToString() => this.description;

  public override bool Equals(object obj)
  {
    By by = obj as By;
    return by != (By) null && this.description.Equals(by.description);
  }

  public override int GetHashCode() => this.description.GetHashCode();
}
