// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.IE.InternetExplorerOptions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.IE;

public class InternetExplorerOptions : DriverOptions
{
  public static readonly string Capability = "se:ieOptions";
  private const string BrowserNameValue = "internet explorer";
  private const string IgnoreProtectedModeSettingsCapability = "ignoreProtectedModeSettings";
  private const string IgnoreZoomSettingCapability = "ignoreZoomSetting";
  private const string InitialBrowserUrlCapability = "initialBrowserUrl";
  private const string EnablePersistentHoverCapability = "enablePersistentHover";
  private const string ElementScrollBehaviorCapability = "elementScrollBehavior";
  private const string RequireWindowFocusCapability = "requireWindowFocus";
  private const string BrowserAttachTimeoutCapability = "browserAttachTimeout";
  private const string BrowserCommandLineSwitchesCapability = "ie.browserCommandLineSwitches";
  private const string ForceCreateProcessApiCapability = "ie.forceCreateProcessApi";
  private const string UsePerProcessProxyCapability = "ie.usePerProcessProxy";
  private const string EnsureCleanSessionCapability = "ie.ensureCleanSession";
  private const string ForceShellWindowsApiCapability = "ie.forceShellWindowsApi";
  private const string FileUploadDialogTimeoutCapability = "ie.fileUploadDialogTimeout";
  private const string EnableFullPageScreenshotCapability = "ie.enableFullPageScreenshot";
  private bool ignoreProtectedModeSettings;
  private bool ignoreZoomLevel;
  private bool enableNativeEvents = true;
  private bool requireWindowFocus;
  private bool enablePersistentHover = true;
  private bool forceCreateProcessApi;
  private bool forceShellWindowsApi;
  private bool usePerProcessProxy;
  private bool ensureCleanSession;
  private bool validateCookieDocumentType = true;
  private bool enableFullPageScreenshot = true;
  private TimeSpan browserAttachTimeout = TimeSpan.MinValue;
  private TimeSpan fileUploadDialogTimeout = TimeSpan.MinValue;
  private string initialBrowserUrl = string.Empty;
  private string browserCommandLineArguments = string.Empty;
  private InternetExplorerElementScrollBehavior elementScrollBehavior;
  private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
  private Dictionary<string, object> additionalInternetExplorerOptions = new Dictionary<string, object>();

  public InternetExplorerOptions()
  {
    this.BrowserName = "internet explorer";
    this.PlatformName = "windows";
    this.AddKnownCapabilityName(InternetExplorerOptions.Capability, "current InterentExplorerOptions class instance");
    this.AddKnownCapabilityName(nameof (ignoreProtectedModeSettings), "IntroduceInstabilityByIgnoringProtectedModeSettings property");
    this.AddKnownCapabilityName("ignoreZoomSetting", "IgnoreZoomLevel property");
    this.AddKnownCapabilityName(CapabilityType.HasNativeEvents, "EnableNativeEvents property");
    this.AddKnownCapabilityName(nameof (initialBrowserUrl), "InitialBrowserUrl property");
    this.AddKnownCapabilityName(nameof (elementScrollBehavior), "ElementScrollBehavior property");
    this.AddKnownCapabilityName(CapabilityType.UnexpectedAlertBehavior, "UnhandledPromptBehavior property");
    this.AddKnownCapabilityName(nameof (enablePersistentHover), "EnablePersistentHover property");
    this.AddKnownCapabilityName(nameof (requireWindowFocus), "RequireWindowFocus property");
    this.AddKnownCapabilityName(nameof (browserAttachTimeout), "BrowserAttachTimeout property");
    this.AddKnownCapabilityName("ie.forceCreateProcessApi", "ForceCreateProcessApi property");
    this.AddKnownCapabilityName("ie.forceShellWindowsApi", "ForceShellWindowsApi property");
    this.AddKnownCapabilityName("ie.browserCommandLineSwitches", "BrowserComaandLineArguments property");
    this.AddKnownCapabilityName("ie.usePerProcessProxy", "UsePerProcessProxy property");
    this.AddKnownCapabilityName("ie.ensureCleanSession", "EnsureCleanSession property");
    this.AddKnownCapabilityName("ie.fileUploadDialogTimeout", "FileUploadDialogTimeout property");
    this.AddKnownCapabilityName("ie.enableFullPageScreenshot", "EnableFullPageScreenshot property");
  }

  public bool IntroduceInstabilityByIgnoringProtectedModeSettings
  {
    get => this.ignoreProtectedModeSettings;
    set => this.ignoreProtectedModeSettings = value;
  }

  public bool IgnoreZoomLevel
  {
    get => this.ignoreZoomLevel;
    set => this.ignoreZoomLevel = value;
  }

  public bool EnableNativeEvents
  {
    get => this.enableNativeEvents;
    set => this.enableNativeEvents = value;
  }

  public bool RequireWindowFocus
  {
    get => this.requireWindowFocus;
    set => this.requireWindowFocus = value;
  }

  public string InitialBrowserUrl
  {
    get => this.initialBrowserUrl;
    set => this.initialBrowserUrl = value;
  }

  public InternetExplorerElementScrollBehavior ElementScrollBehavior
  {
    get => this.elementScrollBehavior;
    set => this.elementScrollBehavior = value;
  }

  public bool EnablePersistentHover
  {
    get => this.enablePersistentHover;
    set => this.enablePersistentHover = value;
  }

  public TimeSpan BrowserAttachTimeout
  {
    get => this.browserAttachTimeout;
    set => this.browserAttachTimeout = value;
  }

  public TimeSpan FileUploadDialogTimeout
  {
    get => this.fileUploadDialogTimeout;
    set => this.fileUploadDialogTimeout = value;
  }

  public bool ForceCreateProcessApi
  {
    get => this.forceCreateProcessApi;
    set => this.forceCreateProcessApi = value;
  }

  public bool ForceShellWindowsApi
  {
    get => this.forceShellWindowsApi;
    set => this.forceShellWindowsApi = value;
  }

  public string BrowserCommandLineArguments
  {
    get => this.browserCommandLineArguments;
    set => this.browserCommandLineArguments = value;
  }

  public bool UsePerProcessProxy
  {
    get => this.usePerProcessProxy;
    set => this.usePerProcessProxy = value;
  }

  public bool EnsureCleanSession
  {
    get => this.ensureCleanSession;
    set => this.ensureCleanSession = value;
  }

  public override void AddAdditionalCapability(string capabilityName, object capabilityValue)
  {
    this.AddAdditionalCapability(capabilityName, capabilityValue, false);
  }

  public void AddAdditionalCapability(
    string capabilityName,
    object capabilityValue,
    bool isGlobalCapability)
  {
    if (this.IsKnownCapabilityName(capabilityName))
    {
      string typeSafeOptionName = this.GetTypeSafeOptionName(capabilityName);
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "There is already an option for the {0} capability. Please use the {1} instead.", (object) capabilityName, (object) typeSafeOptionName), nameof (capabilityName));
    }
    if (string.IsNullOrEmpty(capabilityName))
      throw new ArgumentException("Capability name may not be null an empty string.", nameof (capabilityName));
    if (isGlobalCapability)
      this.additionalCapabilities[capabilityName] = capabilityValue;
    else
      this.additionalInternetExplorerOptions[capabilityName] = capabilityValue;
  }

  public override ICapabilities ToCapabilities()
  {
    DesiredCapabilities desiredCapabilities = this.GenerateDesiredCapabilities(true);
    Dictionary<string, object> capabilityValue = this.BuildInternetExplorerOptionsDictionary();
    desiredCapabilities.SetCapability(InternetExplorerOptions.Capability, (object) capabilityValue);
    foreach (KeyValuePair<string, object> additionalCapability in this.additionalCapabilities)
      desiredCapabilities.SetCapability(additionalCapability.Key, additionalCapability.Value);
    return (ICapabilities) desiredCapabilities;
  }

  private Dictionary<string, object> BuildInternetExplorerOptionsDictionary()
  {
    Dictionary<string, object> dictionary = new Dictionary<string, object>();
    dictionary[CapabilityType.HasNativeEvents] = (object) this.enableNativeEvents;
    dictionary["enablePersistentHover"] = (object) this.enablePersistentHover;
    if (this.requireWindowFocus)
      dictionary["requireWindowFocus"] = (object) true;
    if (this.ignoreProtectedModeSettings)
      dictionary["ignoreProtectedModeSettings"] = (object) true;
    if (this.ignoreZoomLevel)
      dictionary["ignoreZoomSetting"] = (object) true;
    if (!string.IsNullOrEmpty(this.initialBrowserUrl))
      dictionary["initialBrowserUrl"] = (object) this.initialBrowserUrl;
    if (this.elementScrollBehavior != InternetExplorerElementScrollBehavior.Default)
      dictionary["elementScrollBehavior"] = this.elementScrollBehavior != InternetExplorerElementScrollBehavior.Bottom ? (object) 0 : (object) 1;
    if (this.browserAttachTimeout != TimeSpan.MinValue)
      dictionary["browserAttachTimeout"] = (object) Convert.ToInt32(this.browserAttachTimeout.TotalMilliseconds);
    if (this.fileUploadDialogTimeout != TimeSpan.MinValue)
      dictionary["ie.fileUploadDialogTimeout"] = (object) Convert.ToInt32(this.fileUploadDialogTimeout.TotalMilliseconds);
    if (this.forceCreateProcessApi)
    {
      dictionary["ie.forceCreateProcessApi"] = (object) true;
      if (!string.IsNullOrEmpty(this.browserCommandLineArguments))
        dictionary["ie.browserCommandLineSwitches"] = (object) this.browserCommandLineArguments;
    }
    if (this.forceShellWindowsApi)
      dictionary["ie.forceShellWindowsApi"] = (object) true;
    if (this.Proxy != null)
      dictionary["ie.usePerProcessProxy"] = (object) this.usePerProcessProxy;
    if (this.ensureCleanSession)
      dictionary["ie.ensureCleanSession"] = (object) true;
    if (!this.enableFullPageScreenshot)
      dictionary["ie.enableFullPageScreenshot"] = (object) false;
    foreach (KeyValuePair<string, object> internetExplorerOption in this.additionalInternetExplorerOptions)
      dictionary[internetExplorerOption.Key] = internetExplorerOption.Value;
    return dictionary;
  }
}
