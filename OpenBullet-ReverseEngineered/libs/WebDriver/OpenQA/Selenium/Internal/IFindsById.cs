// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Internal.IFindsById
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.ObjectModel;

#nullable disable
namespace OpenQA.Selenium.Internal;

public interface IFindsById
{
  IWebElement FindElementById(string id);

  ReadOnlyCollection<IWebElement> FindElementsById(string id);
}
