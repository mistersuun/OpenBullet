// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Safari.SafariOptions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Safari;

public class SafariOptions : DriverOptions
{
  private const string BrowserNameValue = "safari";
  private const string TechPreviewBrowserNameValue = "safari technology preview";
  private const string EnableAutomaticInspectionSafariOption = "safari:automaticInspection";
  private const string EnableAutomticProfilingSafariOption = "safari:automaticProfiling";
  private bool enableAutomaticInspection;
  private bool enableAutomaticProfiling;
  private bool isTechnologyPreview;
  private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();

  public SafariOptions()
  {
    this.BrowserName = "safari";
    this.AddKnownCapabilityName("safari:automaticInspection", "EnableAutomaticInspection property");
    this.AddKnownCapabilityName("safari:automaticProfiling", "EnableAutomaticProfiling property");
  }

  public bool EnableAutomaticInspection
  {
    get => this.enableAutomaticInspection;
    set => this.enableAutomaticInspection = value;
  }

  public bool EnableAutomaticProfiling
  {
    get => this.enableAutomaticProfiling;
    set => this.enableAutomaticProfiling = value;
  }

  [Obsolete("This property will be removed once the driver for the Safari Technology Preview properly supports the browser name of 'safari'.")]
  public bool IsTechnologyPreview
  {
    get => this.isTechnologyPreview;
    set => this.isTechnologyPreview = value;
  }

  public override void AddAdditionalCapability(string capabilityName, object capabilityValue)
  {
    if (string.IsNullOrEmpty(capabilityName))
      throw new ArgumentException("Capability name may not be null an empty string.", nameof (capabilityName));
    this.additionalCapabilities[capabilityName] = capabilityValue;
  }

  public override ICapabilities ToCapabilities()
  {
    if (this.isTechnologyPreview)
      this.BrowserName = "safari technology preview";
    DesiredCapabilities desiredCapabilities = this.GenerateDesiredCapabilities(true);
    if (this.enableAutomaticInspection)
      desiredCapabilities.SetCapability("safari:automaticInspection", (object) true);
    if (this.enableAutomaticProfiling)
      desiredCapabilities.SetCapability("safari:automaticProfiling", (object) true);
    foreach (KeyValuePair<string, object> additionalCapability in this.additionalCapabilities)
      desiredCapabilities.SetCapability(additionalCapability.Key, additionalCapability.Value);
    return (ICapabilities) desiredCapabilities;
  }
}
