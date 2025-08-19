// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Chrome.ChromeOptions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

#nullable disable
namespace OpenQA.Selenium.Chrome;

public class ChromeOptions : DriverOptions
{
  public static readonly string Capability = "goog:chromeOptions";
  internal static readonly string ForceAlwaysMatchCapabilityName = "se:forceAlwaysMatch";
  private const string BrowserNameValue = "chrome";
  private const string ArgumentsChromeOption = "args";
  private const string BinaryChromeOption = "binary";
  private const string ExtensionsChromeOption = "extensions";
  private const string LocalStateChromeOption = "localState";
  private const string PreferencesChromeOption = "prefs";
  private const string DetachChromeOption = "detach";
  private const string DebuggerAddressChromeOption = "debuggerAddress";
  private const string ExcludeSwitchesChromeOption = "excludeSwitches";
  private const string MinidumpPathChromeOption = "minidumpPath";
  private const string MobileEmulationChromeOption = "mobileEmulation";
  private const string PerformanceLoggingPreferencesChromeOption = "perfLoggingPrefs";
  private const string WindowTypesChromeOption = "windowTypes";
  private const string UseSpecCompliantProtocolOption = "w3c";
  private bool leaveBrowserRunning;
  private bool useSpecCompliantProtocol;
  private string binaryLocation;
  private string debuggerAddress;
  private string minidumpPath;
  private List<string> arguments = new List<string>();
  private List<string> extensionFiles = new List<string>();
  private List<string> encodedExtensions = new List<string>();
  private List<string> excludedSwitches = new List<string>();
  private List<string> windowTypes = new List<string>();
  private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
  private Dictionary<string, object> additionalChromeOptions = new Dictionary<string, object>();
  private Dictionary<string, object> userProfilePreferences;
  private Dictionary<string, object> localStatePreferences;
  private string mobileEmulationDeviceName;
  private ChromeMobileEmulationDeviceSettings mobileEmulationDeviceSettings;
  private ChromePerformanceLoggingPreferences perfLoggingPreferences;

  public ChromeOptions()
  {
    this.BrowserName = "chrome";
    this.AddKnownCapabilityName(ChromeOptions.Capability, "current ChromeOptions class instance");
    this.AddKnownCapabilityName(CapabilityType.LoggingPreferences, "SetLoggingPreference method");
    this.AddKnownCapabilityName("args", "AddArguments method");
    this.AddKnownCapabilityName("binary", "BinaryLocation property");
    this.AddKnownCapabilityName("extensions", "AddExtensions method");
    this.AddKnownCapabilityName("localState", "AddLocalStatePreference method");
    this.AddKnownCapabilityName("prefs", "AddUserProfilePreference method");
    this.AddKnownCapabilityName("detach", "LeaveBrowserRunning property");
    this.AddKnownCapabilityName(nameof (debuggerAddress), "DebuggerAddress property");
    this.AddKnownCapabilityName("excludeSwitches", "AddExcludedArgument property");
    this.AddKnownCapabilityName(nameof (minidumpPath), "MinidumpPath property");
    this.AddKnownCapabilityName("mobileEmulation", "EnableMobileEmulation method");
    this.AddKnownCapabilityName("perfLoggingPrefs", "PerformanceLoggingPreferences property");
    this.AddKnownCapabilityName(nameof (windowTypes), "AddWindowTypes method");
    this.AddKnownCapabilityName("w3c", "UseSpecCompliantProtocol property");
    this.AddKnownCapabilityName(ChromeOptions.ForceAlwaysMatchCapabilityName, "");
  }

  public string BinaryLocation
  {
    get => this.binaryLocation;
    set => this.binaryLocation = value;
  }

  public bool LeaveBrowserRunning
  {
    get => this.leaveBrowserRunning;
    set => this.leaveBrowserRunning = value;
  }

  public ReadOnlyCollection<string> Arguments => this.arguments.AsReadOnly();

  public ReadOnlyCollection<string> Extensions
  {
    get
    {
      List<string> stringList = new List<string>((IEnumerable<string>) this.encodedExtensions);
      foreach (string extensionFile in this.extensionFiles)
      {
        string base64String = Convert.ToBase64String(File.ReadAllBytes(extensionFile));
        stringList.Add(base64String);
      }
      return stringList.AsReadOnly();
    }
  }

  public string DebuggerAddress
  {
    get => this.debuggerAddress;
    set => this.debuggerAddress = value;
  }

  public string MinidumpPath
  {
    get => this.minidumpPath;
    set => this.minidumpPath = value;
  }

  public ChromePerformanceLoggingPreferences PerformanceLoggingPreferences
  {
    get => this.perfLoggingPreferences;
    set => this.perfLoggingPreferences = value;
  }

  public bool UseSpecCompliantProtocol
  {
    get => this.useSpecCompliantProtocol;
    set => this.useSpecCompliantProtocol = value;
  }

  public void AddArgument(string argument)
  {
    if (string.IsNullOrEmpty(argument))
      throw new ArgumentException("argument must not be null or empty", nameof (argument));
    this.AddArguments(argument);
  }

  public void AddArguments(params string[] argumentsToAdd)
  {
    this.AddArguments((IEnumerable<string>) new List<string>((IEnumerable<string>) argumentsToAdd));
  }

  public void AddArguments(IEnumerable<string> argumentsToAdd)
  {
    if (argumentsToAdd == null)
      throw new ArgumentNullException(nameof (argumentsToAdd), "argumentsToAdd must not be null");
    this.arguments.AddRange(argumentsToAdd);
  }

  public void AddExcludedArgument(string argument)
  {
    if (string.IsNullOrEmpty(argument))
      throw new ArgumentException("argument must not be null or empty", nameof (argument));
    this.AddExcludedArguments(argument);
  }

  public void AddExcludedArguments(params string[] argumentsToExclude)
  {
    this.AddExcludedArguments((IEnumerable<string>) new List<string>((IEnumerable<string>) argumentsToExclude));
  }

  public void AddExcludedArguments(IEnumerable<string> argumentsToExclude)
  {
    if (argumentsToExclude == null)
      throw new ArgumentNullException(nameof (argumentsToExclude), "argumentsToExclude must not be null");
    this.excludedSwitches.AddRange(argumentsToExclude);
  }

  public void AddExtension(string pathToExtension)
  {
    if (string.IsNullOrEmpty(pathToExtension))
      throw new ArgumentException("pathToExtension must not be null or empty", nameof (pathToExtension));
    this.AddExtensions(pathToExtension);
  }

  public void AddExtensions(params string[] extensions)
  {
    this.AddExtensions((IEnumerable<string>) new List<string>((IEnumerable<string>) extensions));
  }

  public void AddExtensions(IEnumerable<string> extensions)
  {
    if (extensions == null)
      throw new ArgumentNullException(nameof (extensions), "extensions must not be null");
    foreach (string extension in extensions)
    {
      if (!File.Exists(extension))
        throw new FileNotFoundException("No extension found at the specified path", extension);
      this.extensionFiles.Add(extension);
    }
  }

  public void AddEncodedExtension(string extension)
  {
    if (string.IsNullOrEmpty(extension))
      throw new ArgumentException("extension must not be null or empty", nameof (extension));
    this.AddEncodedExtensions(extension);
  }

  public void AddEncodedExtensions(params string[] extensions)
  {
    this.AddEncodedExtensions((IEnumerable<string>) new List<string>((IEnumerable<string>) extensions));
  }

  public void AddEncodedExtensions(IEnumerable<string> extensions)
  {
    if (extensions == null)
      throw new ArgumentNullException(nameof (extensions), "extensions must not be null");
    foreach (string extension in extensions)
    {
      try
      {
        Convert.FromBase64String(extension);
      }
      catch (FormatException ex)
      {
        throw new WebDriverException("Could not properly decode the base64 string", (Exception) ex);
      }
      this.encodedExtensions.Add(extension);
    }
  }

  public void AddUserProfilePreference(string preferenceName, object preferenceValue)
  {
    if (this.userProfilePreferences == null)
      this.userProfilePreferences = new Dictionary<string, object>();
    this.userProfilePreferences[preferenceName] = preferenceValue;
  }

  public void AddLocalStatePreference(string preferenceName, object preferenceValue)
  {
    if (this.localStatePreferences == null)
      this.localStatePreferences = new Dictionary<string, object>();
    this.localStatePreferences[preferenceName] = preferenceValue;
  }

  public void EnableMobileEmulation(string deviceName)
  {
    this.mobileEmulationDeviceSettings = (ChromeMobileEmulationDeviceSettings) null;
    this.mobileEmulationDeviceName = deviceName;
  }

  public void EnableMobileEmulation(ChromeMobileEmulationDeviceSettings deviceSettings)
  {
    this.mobileEmulationDeviceName = (string) null;
    this.mobileEmulationDeviceSettings = deviceSettings == null || !string.IsNullOrEmpty(deviceSettings.UserAgent) ? deviceSettings : throw new ArgumentException("Device settings must include a user agent string.", nameof (deviceSettings));
  }

  public void AddWindowType(string windowType)
  {
    if (string.IsNullOrEmpty(windowType))
      throw new ArgumentException("windowType must not be null or empty", nameof (windowType));
    this.AddWindowTypes(windowType);
  }

  public void AddWindowTypes(params string[] windowTypesToAdd)
  {
    this.AddWindowTypes((IEnumerable<string>) new List<string>((IEnumerable<string>) windowTypesToAdd));
  }

  public void AddWindowTypes(IEnumerable<string> windowTypesToAdd)
  {
    if (windowTypesToAdd == null)
      throw new ArgumentNullException(nameof (windowTypesToAdd), "windowTypesToAdd must not be null");
    this.windowTypes.AddRange(windowTypesToAdd);
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
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "There is already an option for the {0} capability. Please use the {1} instead.", (object) capabilityName, (object) typeSafeOptionName);
      if (capabilityName == ChromeOptions.ForceAlwaysMatchCapabilityName)
        message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The {0} capability is internal to the driver, and not intended to be set from users' code. Do not attempt to set this capability.", (object) capabilityName);
      throw new ArgumentException(message, nameof (capabilityName));
    }
    if (string.IsNullOrEmpty(capabilityName))
      throw new ArgumentException("Capability name may not be null an empty string.", nameof (capabilityName));
    if (isGlobalCapability)
      this.additionalCapabilities[capabilityName] = capabilityValue;
    else
      this.additionalChromeOptions[capabilityName] = capabilityValue;
  }

  public override ICapabilities ToCapabilities()
  {
    Dictionary<string, object> capabilityValue = this.BuildChromeOptionsDictionary();
    DesiredCapabilities desiredCapabilities = this.GenerateDesiredCapabilities(false);
    desiredCapabilities.SetCapability(ChromeOptions.Capability, (object) capabilityValue);
    Dictionary<string, object> preferencesDictionary = this.GenerateLoggingPreferencesDictionary();
    if (preferencesDictionary != null)
      desiredCapabilities.SetCapability(CapabilityType.LoggingPreferences, (object) preferencesDictionary);
    foreach (KeyValuePair<string, object> additionalCapability in this.additionalCapabilities)
      desiredCapabilities.SetCapability(additionalCapability.Key, additionalCapability.Value);
    return (ICapabilities) desiredCapabilities.AsReadOnly();
  }

  private Dictionary<string, object> BuildChromeOptionsDictionary()
  {
    Dictionary<string, object> dictionary = new Dictionary<string, object>();
    if (this.Arguments.Count > 0)
      dictionary["args"] = (object) this.Arguments;
    if (!string.IsNullOrEmpty(this.binaryLocation))
      dictionary["binary"] = (object) this.binaryLocation;
    ReadOnlyCollection<string> extensions = this.Extensions;
    if (extensions.Count > 0)
      dictionary["extensions"] = (object) extensions;
    if (this.localStatePreferences != null && this.localStatePreferences.Count > 0)
      dictionary["localState"] = (object) this.localStatePreferences;
    if (this.userProfilePreferences != null && this.userProfilePreferences.Count > 0)
      dictionary["prefs"] = (object) this.userProfilePreferences;
    if (this.leaveBrowserRunning)
      dictionary["detach"] = (object) this.leaveBrowserRunning;
    if (this.useSpecCompliantProtocol)
      dictionary["w3c"] = (object) this.useSpecCompliantProtocol;
    if (!string.IsNullOrEmpty(this.debuggerAddress))
      dictionary["debuggerAddress"] = (object) this.debuggerAddress;
    if (this.excludedSwitches.Count > 0)
      dictionary["excludeSwitches"] = (object) this.excludedSwitches;
    if (!string.IsNullOrEmpty(this.minidumpPath))
      dictionary["minidumpPath"] = (object) this.minidumpPath;
    if (!string.IsNullOrEmpty(this.mobileEmulationDeviceName) || this.mobileEmulationDeviceSettings != null)
      dictionary["mobileEmulation"] = (object) this.GenerateMobileEmulationSettingsDictionary();
    if (this.perfLoggingPreferences != null)
      dictionary["perfLoggingPrefs"] = (object) this.GeneratePerformanceLoggingPreferencesDictionary();
    if (this.windowTypes.Count > 0)
      dictionary["windowTypes"] = (object) this.windowTypes;
    foreach (KeyValuePair<string, object> additionalChromeOption in this.additionalChromeOptions)
      dictionary.Add(additionalChromeOption.Key, additionalChromeOption.Value);
    return dictionary;
  }

  private Dictionary<string, object> GeneratePerformanceLoggingPreferencesDictionary()
  {
    Dictionary<string, object> preferencesDictionary = new Dictionary<string, object>();
    preferencesDictionary["enableNetwork"] = (object) this.perfLoggingPreferences.IsCollectingNetworkEvents;
    preferencesDictionary["enablePage"] = (object) this.perfLoggingPreferences.IsCollectingPageEvents;
    string tracingCategories = this.perfLoggingPreferences.TracingCategories;
    if (!string.IsNullOrEmpty(tracingCategories))
      preferencesDictionary["traceCategories"] = (object) tracingCategories;
    preferencesDictionary["bufferUsageReportingInterval"] = (object) Convert.ToInt64(this.perfLoggingPreferences.BufferUsageReportingInterval.TotalMilliseconds);
    return preferencesDictionary;
  }

  private Dictionary<string, object> GenerateMobileEmulationSettingsDictionary()
  {
    Dictionary<string, object> settingsDictionary = new Dictionary<string, object>();
    if (!string.IsNullOrEmpty(this.mobileEmulationDeviceName))
      settingsDictionary["deviceName"] = (object) this.mobileEmulationDeviceName;
    else if (this.mobileEmulationDeviceSettings != null)
    {
      settingsDictionary["userAgent"] = (object) this.mobileEmulationDeviceSettings.UserAgent;
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary["width"] = (object) this.mobileEmulationDeviceSettings.Width;
      dictionary["height"] = (object) this.mobileEmulationDeviceSettings.Height;
      dictionary["pixelRatio"] = (object) this.mobileEmulationDeviceSettings.PixelRatio;
      if (!this.mobileEmulationDeviceSettings.EnableTouchEvents)
        dictionary["touch"] = (object) this.mobileEmulationDeviceSettings.EnableTouchEvents;
      settingsDictionary["deviceMetrics"] = (object) dictionary;
    }
    return settingsDictionary;
  }
}
