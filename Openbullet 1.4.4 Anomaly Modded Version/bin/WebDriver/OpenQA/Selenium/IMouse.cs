// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.IMouse
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;
using System;

#nullable disable
namespace OpenQA.Selenium;

[Obsolete("Use the Actions or ActionBuilder class to simulate mouse input.")]
public interface IMouse
{
  void Click(ICoordinates where);

  void DoubleClick(ICoordinates where);

  void MouseDown(ICoordinates where);

  void MouseUp(ICoordinates where);

  void MouseMove(ICoordinates where);

  void MouseMove(ICoordinates where, int offsetX, int offsetY);

  void ContextClick(ICoordinates where);
}
