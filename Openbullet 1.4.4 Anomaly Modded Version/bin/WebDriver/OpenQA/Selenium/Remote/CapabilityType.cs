// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.CapabilityType
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

public static class CapabilityType
{
  public static readonly string BrowserName = "browserName";
  public static readonly string BrowserVersion = "browserVersion";
  public static readonly string PlatformName = "platformName";
  public static readonly string Platform = "platform";
  public static readonly string Version = "version";
  public static readonly string IsJavaScriptEnabled = "javascriptEnabled";
  public static readonly string TakesScreenshot = "takesScreenshot";
  public static readonly string HandlesAlerts = "handlesAlerts";
  public static readonly string SupportsFindingByCss = "cssSelectorsEnabled";
  public static readonly string Proxy = "proxy";
  public static readonly string Rotatable = "rotatable";
  public static readonly string AcceptSslCertificates = "acceptSslCerts";
  public static readonly string AcceptInsecureCertificates = "acceptInsecureCerts";
  public static readonly string HasNativeEvents = "nativeEvents";
  public static readonly string UnexpectedAlertBehavior = "unexpectedAlertBehaviour";
  public static readonly string UnhandledPromptBehavior = "unhandledPromptBehavior";
  public static readonly string PageLoadStrategy = "pageLoadStrategy";
  public static readonly string LoggingPreferences = "loggingPrefs";
  public static readonly string DisableOverlappedElementCheck = "overlappingCheckDisabled";
  public static readonly string EnableProfiling = "webdriver.logging.profiler.enabled";
  public static readonly string SupportsLocationContext = "locationContextEnabled";
  public static readonly string SupportsApplicationCache = "applicationCacheEnabled";
  public static readonly string SupportsWebStorage = "webStorageEnabled";
  public static readonly string SetWindowRect = "setWindowRect";
  public static readonly string Timeouts = "timeouts";
  private static readonly List<string> KnownSpecCompliantCapabilityNames = new List<string>()
  {
    CapabilityType.BrowserName,
    CapabilityType.BrowserVersion,
    CapabilityType.PlatformName,
    CapabilityType.AcceptInsecureCertificates,
    CapabilityType.PageLoadStrategy,
    CapabilityType.Proxy,
    CapabilityType.SetWindowRect,
    CapabilityType.Timeouts,
    CapabilityType.UnhandledPromptBehavior
  };

  public static bool IsSpecCompliantCapabilityName(string capabilityName)
  {
    return CapabilityType.KnownSpecCompliantCapabilityNames.Contains(capabilityName) || capabilityName.Contains(":");
  }
}
