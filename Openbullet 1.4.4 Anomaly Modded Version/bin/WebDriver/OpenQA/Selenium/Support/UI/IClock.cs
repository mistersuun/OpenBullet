// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.UI.IClock
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace OpenQA.Selenium.Support.UI;

public interface IClock
{
  DateTime Now { get; }

  DateTime LaterBy(TimeSpan delay);

  bool IsNowBefore(DateTime otherDateTime);
}
