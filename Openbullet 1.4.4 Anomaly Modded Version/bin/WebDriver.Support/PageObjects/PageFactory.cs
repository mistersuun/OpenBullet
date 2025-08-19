// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.PageFactory
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

[Obsolete("The PageFactory implementation in the .NET bindings is deprecated and will be removed in a future release. This portion of the code has been migrated to the DotNetSeleniumExtras repository on GitHub (https://github.com/DotNetSeleniumTools/DotNetSeleniumExtras)")]
public sealed class PageFactory
{
  private PageFactory()
  {
  }

  public static T InitElements<T>(IWebDriver driver)
  {
    return PageFactory.InitElements<T>((IElementLocator) new DefaultElementLocator((ISearchContext) driver));
  }

  public static T InitElements<T>(IElementLocator locator)
  {
    ConstructorInfo constructor = typeof (T).GetConstructor(new Type[1]
    {
      typeof (IWebDriver)
    });
    if (constructor == (ConstructorInfo) null)
      throw new ArgumentException("No constructor for the specified class containing a single argument of type IWebDriver can be found");
    if (locator == null)
      throw new ArgumentNullException(nameof (locator), "locator cannot be null");
    if (!(locator.SearchContext is IWebDriver))
      throw new ArgumentException("The search context of the element locator must implement IWebDriver", nameof (locator));
    T page = (T) constructor.Invoke(new object[1]
    {
      (object) (locator.SearchContext as IWebDriver)
    });
    PageFactory.InitElements((object) page, locator);
    return page;
  }

  public static void InitElements(ISearchContext driver, object page)
  {
    PageFactory.InitElements(page, (IElementLocator) new DefaultElementLocator(driver));
  }

  public static void InitElements(
    ISearchContext driver,
    object page,
    IPageObjectMemberDecorator decorator)
  {
    PageFactory.InitElements(page, (IElementLocator) new DefaultElementLocator(driver), decorator);
  }

  public static void InitElements(object page, IElementLocator locator)
  {
    PageFactory.InitElements(page, locator, (IPageObjectMemberDecorator) new DefaultPageObjectMemberDecorator());
  }

  public static void InitElements(
    object page,
    IElementLocator locator,
    IPageObjectMemberDecorator decorator)
  {
    if (page == null)
      throw new ArgumentNullException(nameof (page), "page cannot be null");
    if (locator == null)
      throw new ArgumentNullException(nameof (locator), "locator cannot be null");
    if (decorator == null)
      throw new ArgumentNullException(nameof (locator), "decorator cannot be null");
    if (locator.SearchContext == null)
      throw new ArgumentException("The SearchContext of the locator object cannot be null", nameof (locator));
    Type type = page.GetType();
    List<MemberInfo> memberInfoList = new List<MemberInfo>();
    memberInfoList.AddRange((IEnumerable<MemberInfo>) type.GetFields(BindingFlags.Instance | BindingFlags.Public));
    memberInfoList.AddRange((IEnumerable<MemberInfo>) type.GetProperties(BindingFlags.Instance | BindingFlags.Public));
    for (; type != (Type) null; type = type.BaseType)
    {
      memberInfoList.AddRange((IEnumerable<MemberInfo>) type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
      memberInfoList.AddRange((IEnumerable<MemberInfo>) type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic));
    }
    foreach (MemberInfo member in memberInfoList)
    {
      object obj = decorator.Decorate(member, locator);
      if (obj != null)
      {
        FieldInfo fieldInfo = member as FieldInfo;
        PropertyInfo propertyInfo = member as PropertyInfo;
        if (fieldInfo != (FieldInfo) null)
          fieldInfo.SetValue(page, obj);
        else if (propertyInfo != (PropertyInfo) null)
          propertyInfo.SetValue(page, obj, (object[]) null);
      }
    }
  }
}
