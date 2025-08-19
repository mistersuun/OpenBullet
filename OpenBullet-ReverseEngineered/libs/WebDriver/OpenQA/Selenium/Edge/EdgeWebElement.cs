// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Edge.EdgeWebElement
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;

#nullable disable
namespace OpenQA.Selenium.Edge;

public class EdgeWebElement(EdgeDriver parent, string elementId) : RemoteWebElement((RemoteWebDriver) parent, elementId)
{
}
