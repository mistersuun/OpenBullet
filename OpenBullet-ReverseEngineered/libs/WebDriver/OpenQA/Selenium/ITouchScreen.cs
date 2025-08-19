// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.ITouchScreen
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;
using System;

#nullable disable
namespace OpenQA.Selenium;

[Obsolete("Use the TouchActions or ActionBuilder class to simulate touch input.")]
public interface ITouchScreen
{
  void SingleTap(ICoordinates where);

  void Down(int locationX, int locationY);

  void Up(int locationX, int locationY);

  void Move(int locationX, int locationY);

  void Scroll(ICoordinates where, int offsetX, int offsetY);

  void Scroll(int offsetX, int offsetY);

  void DoubleTap(ICoordinates where);

  void LongPress(ICoordinates where);

  void Flick(int speedX, int speedY);

  void Flick(ICoordinates where, int offsetX, int offsetY, int speed);
}
