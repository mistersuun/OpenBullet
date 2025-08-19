// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.ByFactory
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

internal static class ByFactory
{
  public static By From(FindsByAttribute attribute)
  {
    How how = attribute.How;
    string str = attribute.Using;
    switch (how)
    {
      case How.Id:
        return By.Id(str);
      case How.Name:
        return By.Name(str);
      case How.TagName:
        return By.TagName(str);
      case How.ClassName:
        return By.ClassName(str);
      case How.CssSelector:
        return By.CssSelector(str);
      case How.LinkText:
        return By.LinkText(str);
      case How.PartialLinkText:
        return By.PartialLinkText(str);
      case How.XPath:
        return By.XPath(str);
      case How.Custom:
        if (attribute.CustomFinderType == (Type) null)
          throw new ArgumentException("Cannot use How.Custom without supplying a custom finder type");
        ConstructorInfo constructorInfo = attribute.CustomFinderType.IsSubclassOf(typeof (By)) ? attribute.CustomFinderType.GetConstructor(new Type[1]
        {
          typeof (string)
        }) : throw new ArgumentException("Custom finder type must be a descendent of the By class");
        return !(constructorInfo == (ConstructorInfo) null) ? constructorInfo.Invoke(new object[1]
        {
          (object) str
        }) as By : throw new ArgumentException("Custom finder type must expose a public constructor with a string argument");
      default:
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Did not know how to construct How from how {0}, using {1}", (object) how, (object) str));
    }
  }
}
