// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.WebDriverResult
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium;

public enum WebDriverResult
{
  Success = 0,
  IndexOutOfBounds = 1,
  NoCollection = 2,
  NoString = 3,
  NoStringLength = 4,
  NoStringWrapper = 5,
  NoSuchDriver = 6,
  NoSuchElement = 7,
  NoSuchFrame = 8,
  UnknownCommand = 9,
  ObsoleteElement = 10, // 0x0000000A
  ElementNotDisplayed = 11, // 0x0000000B
  InvalidElementState = 12, // 0x0000000C
  UnhandledError = 13, // 0x0000000D
  ExpectedError = 14, // 0x0000000E
  ElementNotSelectable = 15, // 0x0000000F
  NoSuchDocument = 16, // 0x00000010
  UnexpectedJavaScriptError = 17, // 0x00000011
  NoScriptResult = 18, // 0x00000012
  XPathLookupError = 19, // 0x00000013
  NoSuchCollection = 20, // 0x00000014
  Timeout = 21, // 0x00000015
  NullPointer = 22, // 0x00000016
  NoSuchWindow = 23, // 0x00000017
  InvalidCookieDomain = 24, // 0x00000018
  UnableToSetCookie = 25, // 0x00000019
  UnexpectedAlertOpen = 26, // 0x0000001A
  NoAlertPresent = 27, // 0x0000001B
  AsyncScriptTimeout = 28, // 0x0000001C
  InvalidElementCoordinates = 29, // 0x0000001D
  InvalidSelector = 32, // 0x00000020
  SessionNotCreated = 33, // 0x00000021
  MoveTargetOutOfBounds = 34, // 0x00000022
  InvalidXPathSelector = 51, // 0x00000033
  InsecureCertificate = 59, // 0x0000003B
  ElementNotInteractable = 60, // 0x0000003C
  InvalidArgument = 61, // 0x0000003D
  NoSuchCookie = 62, // 0x0000003E
  UnableToCaptureScreen = 63, // 0x0000003F
  ElementClickIntercepted = 64, // 0x00000040
}
