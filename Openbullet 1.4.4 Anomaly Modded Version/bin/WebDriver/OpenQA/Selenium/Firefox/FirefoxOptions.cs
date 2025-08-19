// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.FirefoxOptions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Firefox;

public class FirefoxOptions : DriverOptions
{
  private const string BrowserNameValue = "firefox";
  private const string IsMarionetteCapability = "marionette";
  private const string FirefoxLegacyProfileCapability = "firefox_profile";
  private const string FirefoxLegacyBinaryCapability = "firefox_binary";
  private const string FirefoxProfileCapability = "profile";
  private const string FirefoxBinaryCapability = "binary";
  private const string FirefoxArgumentsCapability = "args";
  private const string FirefoxLogCapability = "log";
  private const string FirefoxPrefsCapability = "prefs";
  private const string FirefoxOptionsCapability = "moz:firefoxOptions";
  private bool isMarionette = true;
  private string browserBinaryLocation;
  private FirefoxDriverLogLevel logLevel = FirefoxDriverLogLevel.Default;
  private FirefoxProfile profile;
  private List<string> firefoxArguments = new List<string>();
  private Dictionary<string, object> profilePreferences = new Dictionary<string, object>();
  private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
  private Dictionary<string, object> additionalFirefoxOptions = new Dictionary<string, object>();

  public FirefoxOptions()
  {
    this.BrowserName = "firefox";
    this.AddKnownCapabilityName("moz:firefoxOptions", "current FirefoxOptions class instance");
    this.AddKnownCapabilityName("marionette", "UseLegacyImplementation property");
    this.AddKnownCapabilityName(nameof (profile), "Profile property");
    this.AddKnownCapabilityName("binary", "BrowserExecutableLocation property");
    this.AddKnownCapabilityName("args", "AddArguments method");
    this.AddKnownCapabilityName("prefs", "SetPreference method");
    this.AddKnownCapabilityName("log", "LogLevel property");
    this.AddKnownCapabilityName("firefox_profile", "Profile property");
    this.AddKnownCapabilityName("firefox_binary", "BrowserExecutableLocation property");
  }

  internal FirefoxOptions(
    FirefoxProfile profile,
    FirefoxBinary binary,
    DesiredCapabilities capabilities)
  {
    this.BrowserName = "firefox";
    if (profile != null)
      this.profile = profile;
    if (binary != null)
      this.browserBinaryLocation = binary.BinaryExecutable.ExecutablePath;
    if (capabilities == null)
      return;
    this.ImportCapabilities(capabilities);
  }

  public bool UseLegacyImplementation
  {
    get => !this.isMarionette;
    set => this.isMarionette = !value;
  }

  public FirefoxProfile Profile
  {
    get => this.profile;
    set => this.profile = value;
  }

  public string BrowserExecutableLocation
  {
    get => this.browserBinaryLocation;
    set => this.browserBinaryLocation = value;
  }

  public FirefoxDriverLogLevel LogLevel
  {
    get => this.logLevel;
    set => this.logLevel = value;
  }

  public void AddArgument(string argumentName)
  {
    if (string.IsNullOrEmpty(argumentName))
      throw new ArgumentException("argumentName must not be null or empty", nameof (argumentName));
    this.AddArguments(argumentName);
  }

  public void AddArguments(params string[] argumentsToAdd)
  {
    this.AddArguments((IEnumerable<string>) new List<string>((IEnumerable<string>) argumentsToAdd));
  }

  public void AddArguments(IEnumerable<string> argumentsToAdd)
  {
    if (argumentsToAdd == null)
      throw new ArgumentNullException(nameof (argumentsToAdd), "argumentsToAdd must not be null");
    this.firefoxArguments.AddRange(argumentsToAdd);
  }

  public void SetPreference(string preferenceName, bool preferenceValue)
  {
    this.SetPreferenceValue(preferenceName, (object) preferenceValue);
  }

  public void SetPreference(string preferenceName, int preferenceValue)
  {
    this.SetPreferenceValue(preferenceName, (object) preferenceValue);
  }

  public void SetPreference(string preferenceName, long preferenceValue)
  {
    this.SetPreferenceValue(preferenceName, (object) preferenceValue);
  }

  public void SetPreference(string preferenceName, double preferenceValue)
  {
    this.SetPreferenceValue(preferenceName, (object) preferenceValue);
  }

  public void SetPreference(string preferenceName, string preferenceValue)
  {
    this.SetPreferenceValue(preferenceName, (object) preferenceValue);
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
      this.additionalFirefoxOptions[capabilityName] = capabilityValue;
  }

  public override ICapabilities ToCapabilities()
  {
    DesiredCapabilities desiredCapabilities = this.GenerateDesiredCapabilities(this.isMarionette);
    if (this.isMarionette)
    {
      Dictionary<string, object> optionsDictionary = this.GenerateFirefoxOptionsDictionary();
      desiredCapabilities.SetCapability("moz:firefoxOptions", (object) optionsDictionary);
    }
    else
    {
      if (this.profile != null)
      {
        if (this.Proxy != null)
          this.profile.InternalSetProxyPreferences(this.Proxy);
        desiredCapabilities.SetCapability("profile", (object) this.profile.ToBase64String());
      }
      if (!string.IsNullOrEmpty(this.browserBinaryLocation))
      {
        desiredCapabilities.SetCapability("binary", (object) this.browserBinaryLocation);
      }
      else
      {
        using (FirefoxBinary firefoxBinary = new FirefoxBinary())
        {
          string executablePath = firefoxBinary.BinaryExecutable.ExecutablePath;
          desiredCapabilities.SetCapability("binary", (object) executablePath);
        }
      }
    }
    foreach (KeyValuePair<string, object> additionalCapability in this.additionalCapabilities)
      desiredCapabilities.SetCapability(additionalCapability.Key, additionalCapability.Value);
    return (ICapabilities) desiredCapabilities;
  }

  private Dictionary<string, object> GenerateFirefoxOptionsDictionary()
  {
    Dictionary<string, object> optionsDictionary = new Dictionary<string, object>();
    if (this.profile != null)
    {
      this.profile.RemoveWebDriverExtension();
      optionsDictionary["profile"] = (object) this.profile.ToBase64String();
    }
    if (!string.IsNullOrEmpty(this.browserBinaryLocation))
      optionsDictionary["binary"] = (object) this.browserBinaryLocation;
    else if (!this.isMarionette)
    {
      using (FirefoxBinary firefoxBinary = new FirefoxBinary())
      {
        string executablePath = firefoxBinary.BinaryExecutable.ExecutablePath;
        if (!string.IsNullOrEmpty(executablePath))
          optionsDictionary["binary"] = (object) executablePath;
      }
    }
    if (this.logLevel != FirefoxDriverLogLevel.Default)
      optionsDictionary["log"] = (object) new Dictionary<string, object>()
      {
        ["level"] = (object) this.logLevel.ToString().ToLowerInvariant()
      };
    if (this.firefoxArguments.Count > 0)
    {
      List<object> objectList = new List<object>();
      foreach (string firefoxArgument in this.firefoxArguments)
        objectList.Add((object) firefoxArgument);
      optionsDictionary["args"] = (object) objectList;
    }
    if (this.profilePreferences.Count > 0)
      optionsDictionary["prefs"] = (object) this.profilePreferences;
    foreach (KeyValuePair<string, object> additionalFirefoxOption in this.additionalFirefoxOptions)
      optionsDictionary.Add(additionalFirefoxOption.Key, additionalFirefoxOption.Value);
    return optionsDictionary;
  }

  private void SetPreferenceValue(string preferenceName, object preferenceValue)
  {
    if (string.IsNullOrEmpty(preferenceName))
      throw new ArgumentException("Preference name may not be null an empty string.", nameof (preferenceName));
    if (!this.isMarionette)
      throw new ArgumentException("Preferences cannot be set directly when using the legacy FirefoxDriver implementation. Set them in the profile.");
    this.profilePreferences[preferenceName] = preferenceValue;
  }

  private void ImportCapabilities(DesiredCapabilities capabilities)
  {
    foreach (KeyValuePair<string, object> capabilities1 in capabilities.CapabilitiesDictionary)
    {
      if (!(capabilities1.Key == CapabilityType.BrowserName))
      {
        if (capabilities1.Key == CapabilityType.BrowserVersion)
          this.BrowserVersion = capabilities1.Value.ToString();
        else if (capabilities1.Key == CapabilityType.PlatformName)
          this.PlatformName = capabilities1.Value.ToString();
        else if (capabilities1.Key == CapabilityType.Proxy)
          this.Proxy = new Proxy(capabilities1.Value as Dictionary<string, object>);
        else if (capabilities1.Key == CapabilityType.UnhandledPromptBehavior)
          this.UnhandledPromptBehavior = (UnhandledPromptBehavior) Enum.Parse(typeof (UnhandledPromptBehavior), capabilities1.Value.ToString(), true);
        else if (capabilities1.Key == CapabilityType.PageLoadStrategy)
          this.PageLoadStrategy = (PageLoadStrategy) Enum.Parse(typeof (PageLoadStrategy), capabilities1.Value.ToString(), true);
        else if (capabilities1.Key == "moz:firefoxOptions")
        {
          foreach (KeyValuePair<string, object> keyValuePair in capabilities1.Value as Dictionary<string, object>)
          {
            if (keyValuePair.Key == "args")
            {
              foreach (object obj in keyValuePair.Value as object[])
                this.firefoxArguments.Add(obj.ToString());
            }
            else if (keyValuePair.Key == "prefs")
              this.profilePreferences = keyValuePair.Value as Dictionary<string, object>;
            else if (keyValuePair.Key == "log")
            {
              Dictionary<string, object> dictionary = keyValuePair.Value as Dictionary<string, object>;
              if (dictionary.ContainsKey("level"))
                this.logLevel = (FirefoxDriverLogLevel) Enum.Parse(typeof (FirefoxDriverLogLevel), dictionary["level"].ToString(), true);
            }
            else if (keyValuePair.Key == "binary")
              this.browserBinaryLocation = keyValuePair.Value.ToString();
            else if (keyValuePair.Key == "profile")
              this.profile = FirefoxProfile.FromBase64String(keyValuePair.Value.ToString());
            else
              this.AddAdditionalCapability(keyValuePair.Key, keyValuePair.Value);
          }
        }
        else
          this.AddAdditionalCapability(capabilities1.Key, capabilities1.Value, true);
      }
    }
  }
}
