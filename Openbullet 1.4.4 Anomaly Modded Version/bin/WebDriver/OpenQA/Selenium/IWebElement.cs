// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.IWebElement
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Drawing;

#nullable disable
namespace OpenQA.Selenium;

public interface IWebElement : ISearchContext
{
  string TagName { get; }

  string Text { get; }

  bool Enabled { get; }

  bool Selected { get; }

  Point Location { get; }

  Size Size { get; }

  bool Displayed { get; }

  void Clear();

  void SendKeys(string text);

  void Submit();

  void Click();

  string GetAttribute(string attributeName);

  string GetProperty(string propertyName);

  string GetCssValue(string propertyName);
}
