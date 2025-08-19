// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.DriverOptions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium;

public abstract class DriverOptions
{
  private string browserName;
  private string browserVersion;
  private string platformName;
  private Proxy proxy;
  private bool? acceptInsecureCertificates;
  private UnhandledPromptBehavior unhandledPromptBehavior;
  private PageLoadStrategy pageLoadStrategy;
  private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
  private Dictionary<string, LogLevel> loggingPreferences = new Dictionary<string, LogLevel>();
  private Dictionary<string, string> knownCapabilityNames = new Dictionary<string, string>();

  protected DriverOptions()
  {
    this.AddKnownCapabilityName(CapabilityType.BrowserName, "BrowserName property");
    this.AddKnownCapabilityName(CapabilityType.BrowserVersion, "BrowserVersion property");
    this.AddKnownCapabilityName(CapabilityType.PlatformName, "PlatformName property");
    this.AddKnownCapabilityName(CapabilityType.Proxy, "Proxy property");
    this.AddKnownCapabilityName(CapabilityType.UnhandledPromptBehavior, "UnhandledPromptBehavior property");
    this.AddKnownCapabilityName(CapabilityType.PageLoadStrategy, "PageLoadStrategy property");
  }

  public string BrowserName
  {
    get => this.browserName;
    protected set => this.browserName = value;
  }

  public string BrowserVersion
  {
    get => this.browserVersion;
    set => this.browserVersion = value;
  }

  public string PlatformName
  {
    get => this.platformName;
    set => this.platformName = value;
  }

  public bool? AcceptInsecureCertificates
  {
    get => this.acceptInsecureCertificates;
    set => this.acceptInsecureCertificates = value;
  }

  public UnhandledPromptBehavior UnhandledPromptBehavior
  {
    get => this.unhandledPromptBehavior;
    set => this.unhandledPromptBehavior = value;
  }

  public PageLoadStrategy PageLoadStrategy
  {
    get => this.pageLoadStrategy;
    set => this.pageLoadStrategy = value;
  }

  public Proxy Proxy
  {
    get => this.proxy;
    set => this.proxy = value;
  }

  public abstract void AddAdditionalCapability(string capabilityName, object capabilityValue);

  public abstract ICapabilities ToCapabilities();

  public virtual DriverOptionsMergeResult GetMergeResult(DriverOptions other)
  {
    DriverOptionsMergeResult mergeResult = new DriverOptionsMergeResult();
    if (this.browserName != null && other.BrowserName != null)
    {
      mergeResult.IsMergeConflict = true;
      mergeResult.MergeConflictOptionName = "BrowserName";
      return mergeResult;
    }
    if (this.browserVersion != null && other.BrowserVersion != null)
    {
      mergeResult.IsMergeConflict = true;
      mergeResult.MergeConflictOptionName = "BrowserVersion";
      return mergeResult;
    }
    if (this.platformName != null && other.PlatformName != null)
    {
      mergeResult.IsMergeConflict = true;
      mergeResult.MergeConflictOptionName = "PlatformName";
      return mergeResult;
    }
    if (this.proxy != null && other.Proxy != null)
    {
      mergeResult.IsMergeConflict = true;
      mergeResult.MergeConflictOptionName = "Proxy";
      return mergeResult;
    }
    if (this.unhandledPromptBehavior != UnhandledPromptBehavior.Default && other.UnhandledPromptBehavior != UnhandledPromptBehavior.Default)
    {
      mergeResult.IsMergeConflict = true;
      mergeResult.MergeConflictOptionName = "UnhandledPromptBehavior";
      return mergeResult;
    }
    if (this.pageLoadStrategy == PageLoadStrategy.Default || other.PageLoadStrategy == PageLoadStrategy.Default)
      return mergeResult;
    mergeResult.IsMergeConflict = true;
    mergeResult.MergeConflictOptionName = "PageLoadStrategy";
    return mergeResult;
  }

  public void SetLoggingPreference(string logType, LogLevel logLevel)
  {
    this.loggingPreferences[logType] = logLevel;
  }

  public override string ToString()
  {
    return JsonConvert.SerializeObject((object) this.ToDictionary(), Formatting.Indented);
  }

  internal Dictionary<string, object> ToDictionary()
  {
    return (this.ToCapabilities() as IHasCapabilitiesDictionary).CapabilitiesDictionary;
  }

  protected void AddKnownCapabilityName(string capabilityName, string typeSafeOptionName)
  {
    this.knownCapabilityNames[capabilityName] = typeSafeOptionName;
  }

  protected bool IsKnownCapabilityName(string capabilityName)
  {
    return this.knownCapabilityNames.ContainsKey(capabilityName);
  }

  protected string GetTypeSafeOptionName(string capabilityName)
  {
    return this.IsKnownCapabilityName(capabilityName) ? string.Empty : this.knownCapabilityNames[capabilityName];
  }

  protected Dictionary<string, object> GenerateLoggingPreferencesDictionary()
  {
    if (this.loggingPreferences.Count == 0)
      return (Dictionary<string, object>) null;
    Dictionary<string, object> preferencesDictionary = new Dictionary<string, object>();
    foreach (string key in this.loggingPreferences.Keys)
      preferencesDictionary[key] = (object) this.loggingPreferences[key].ToString().ToUpperInvariant();
    return preferencesDictionary;
  }

  protected DesiredCapabilities GenerateDesiredCapabilities(bool isSpecificationCompliant)
  {
    DesiredCapabilities desiredCapabilities = new DesiredCapabilities();
    if (!string.IsNullOrEmpty(this.browserName))
      desiredCapabilities.SetCapability(CapabilityType.BrowserName, (object) this.browserName);
    if (!string.IsNullOrEmpty(this.browserVersion))
      desiredCapabilities.SetCapability(CapabilityType.BrowserVersion, (object) this.browserVersion);
    if (!string.IsNullOrEmpty(this.platformName))
      desiredCapabilities.SetCapability(CapabilityType.PlatformName, (object) this.platformName);
    if (this.acceptInsecureCertificates.HasValue)
      desiredCapabilities.SetCapability(CapabilityType.AcceptInsecureCertificates, (object) this.acceptInsecureCertificates);
    if (this.pageLoadStrategy != PageLoadStrategy.Default)
    {
      string capabilityValue = "normal";
      switch (this.pageLoadStrategy)
      {
        case PageLoadStrategy.Eager:
          capabilityValue = "eager";
          break;
        case PageLoadStrategy.None:
          capabilityValue = "none";
          break;
      }
      desiredCapabilities.SetCapability(CapabilityType.PageLoadStrategy, (object) capabilityValue);
    }
    if (this.UnhandledPromptBehavior != UnhandledPromptBehavior.Default)
    {
      string capabilityValue = "ignore";
      switch (this.UnhandledPromptBehavior)
      {
        case UnhandledPromptBehavior.Accept:
          capabilityValue = "accept";
          break;
        case UnhandledPromptBehavior.Dismiss:
          capabilityValue = "dismiss";
          break;
        case UnhandledPromptBehavior.AcceptAndNotify:
          capabilityValue = "accept and notify";
          break;
        case UnhandledPromptBehavior.DismissAndNotify:
          capabilityValue = "dismiss and notify";
          break;
      }
      desiredCapabilities.SetCapability(CapabilityType.UnhandledPromptBehavior, (object) capabilityValue);
    }
    if (this.Proxy != null)
    {
      Dictionary<string, object> capabilityValue = this.Proxy.ToCapability();
      if (!isSpecificationCompliant)
        capabilityValue = this.Proxy.ToLegacyCapability();
      if (capabilityValue != null)
        desiredCapabilities.SetCapability(CapabilityType.Proxy, (object) capabilityValue);
    }
    return desiredCapabilities;
  }
}
