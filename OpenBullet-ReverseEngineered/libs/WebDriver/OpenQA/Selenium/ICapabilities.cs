// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.ICapabilities
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium;

public interface ICapabilities
{
  object this[string capabilityName] { get; }

  bool HasCapability(string capability);

  object GetCapability(string capability);
}
