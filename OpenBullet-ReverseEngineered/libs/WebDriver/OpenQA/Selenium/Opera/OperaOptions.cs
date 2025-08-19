// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Opera.OperaOptions
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
namespace OpenQA.Selenium.Opera;

public class OperaOptions : DriverOptions
{
  public static readonly string Capability = "operaOptions";
  private const string BrowserNameValue = "opera";
  private const string ArgumentsOperaOption = "args";
  private const string BinaryOperaOption = "binary";
  private const string ExtensionsOperaOption = "extensions";
  private const string LocalStateOperaOption = "localState";
  private const string PreferencesOperaOption = "prefs";
  private const string DetachOperaOption = "detach";
  private const string DebuggerAddressOperaOption = "debuggerAddress";
  private const string ExcludeSwitchesOperaOption = "excludeSwitches";
  private const string MinidumpPathOperaOption = "minidumpPath";
  private bool leaveBrowserRunning;
  private string binaryLocation;
  private string debuggerAddress;
  private string minidumpPath;
  private List<string> arguments = new List<string>();
  private List<string> extensionFiles = new List<string>();
  private List<string> encodedExtensions = new List<string>();
  private List<string> excludedSwitches = new List<string>();
  private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
  private Dictionary<string, object> additionalOperaOptions = new Dictionary<string, object>();
  private Dictionary<string, object> userProfilePreferences;
  private Dictionary<string, object> localStatePreferences;
  private Proxy proxy;

  public OperaOptions() => this.BrowserName = "opera";

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

  public override void AddAdditionalCapability(string capabilityName, object capabilityValue)
  {
    this.AddAdditionalCapability(capabilityName, capabilityValue, false);
  }

  public void AddAdditionalCapability(
    string capabilityName,
    object capabilityValue,
    bool isGlobalCapability)
  {
    if (capabilityName == OperaOptions.Capability || capabilityName == CapabilityType.Proxy || capabilityName == "args" || capabilityName == "binary" || capabilityName == "extensions" || capabilityName == "localState" || capabilityName == "prefs" || capabilityName == "detach" || capabilityName == "debuggerAddress" || capabilityName == "extensions" || capabilityName == "excludeSwitches" || capabilityName == "minidumpPath")
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "There is already an option for the {0} capability. Please use that instead.", (object) capabilityName), nameof (capabilityName));
    if (string.IsNullOrEmpty(capabilityName))
      throw new ArgumentException("Capability name may not be null an empty string.", nameof (capabilityName));
    if (isGlobalCapability)
      this.additionalCapabilities[capabilityName] = capabilityValue;
    else
      this.additionalOperaOptions[capabilityName] = capabilityValue;
  }

  public override ICapabilities ToCapabilities()
  {
    Dictionary<string, object> capabilityValue = this.BuildOperaOptionsDictionary();
    DesiredCapabilities desiredCapabilities = this.GenerateDesiredCapabilities(false);
    desiredCapabilities.SetCapability(OperaOptions.Capability, (object) capabilityValue);
    foreach (KeyValuePair<string, object> additionalCapability in this.additionalCapabilities)
      desiredCapabilities.SetCapability(additionalCapability.Key, additionalCapability.Value);
    return (ICapabilities) desiredCapabilities;
  }

  private Dictionary<string, object> BuildOperaOptionsDictionary()
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
    if (!string.IsNullOrEmpty(this.debuggerAddress))
      dictionary["debuggerAddress"] = (object) this.debuggerAddress;
    if (this.excludedSwitches.Count > 0)
      dictionary["excludeSwitches"] = (object) this.excludedSwitches;
    if (!string.IsNullOrEmpty(this.minidumpPath))
      dictionary["minidumpPath"] = (object) this.minidumpPath;
    foreach (KeyValuePair<string, object> additionalOperaOption in this.additionalOperaOptions)
      dictionary.Add(additionalOperaOption.Key, additionalOperaOption.Value);
    return dictionary;
  }
}
