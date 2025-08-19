// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.IAlert
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium;

public interface IAlert
{
  string Text { get; }

  void Dismiss();

  void Accept();

  void SendKeys(string keysToSend);

  void SetAuthenticationCredentials(string userName, string password);
}
