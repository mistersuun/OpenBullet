// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.IElementLocator
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

public interface IElementLocator
{
  ISearchContext SearchContext { get; }

  IWebElement LocateElement(IEnumerable<By> bys);

  ReadOnlyCollection<IWebElement> LocateElements(IEnumerable<By> bys);
}
