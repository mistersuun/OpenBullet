// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.ReadOnlyDesiredCapabilities
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class ReadOnlyDesiredCapabilities : ICapabilities, IHasCapabilitiesDictionary
{
  private readonly Dictionary<string, object> capabilities = new Dictionary<string, object>();

  private ReadOnlyDesiredCapabilities()
  {
  }

  internal ReadOnlyDesiredCapabilities(DesiredCapabilities desiredCapabilities)
  {
    foreach (KeyValuePair<string, object> capabilities in desiredCapabilities.CapabilitiesDictionary)
      this.capabilities[capabilities.Key] = capabilities.Value;
  }

  public string BrowserName
  {
    get
    {
      string empty = string.Empty;
      object capability = this.GetCapability(CapabilityType.BrowserName);
      if (capability != null)
        empty = capability.ToString();
      return empty;
    }
  }

  public Platform Platform
  {
    get
    {
      return this.GetCapability(CapabilityType.Platform) is Platform capability ? capability : new Platform(PlatformType.Any);
    }
  }

  public string Version
  {
    get
    {
      string empty = string.Empty;
      object capability = this.GetCapability(CapabilityType.Version);
      if (capability != null)
        empty = capability.ToString();
      return empty;
    }
  }

  public bool AcceptInsecureCerts
  {
    get
    {
      bool acceptInsecureCerts = false;
      object capability = this.GetCapability(CapabilityType.AcceptInsecureCertificates);
      if (capability != null)
        acceptInsecureCerts = (bool) capability;
      return acceptInsecureCerts;
    }
  }

  Dictionary<string, object> IHasCapabilitiesDictionary.CapabilitiesDictionary
  {
    get => this.CapabilitiesDictionary;
  }

  internal Dictionary<string, object> CapabilitiesDictionary => this.capabilities;

  public object this[string capabilityName]
  {
    get
    {
      return this.capabilities.ContainsKey(capabilityName) ? this.capabilities[capabilityName] : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The capability {0} is not present in this set of capabilities", (object) capabilityName));
    }
  }

  public bool HasCapability(string capability) => this.capabilities.ContainsKey(capability);

  public object GetCapability(string capability)
  {
    object capability1 = (object) null;
    if (this.capabilities.ContainsKey(capability))
    {
      capability1 = this.capabilities[capability];
      string str = capability1 as string;
      if (capability == CapabilityType.Platform && str != null)
        capability1 = (object) Platform.FromString(capability1.ToString());
    }
    return capability1;
  }

  public Dictionary<string, object> ToDictionary() => this.capabilities;

  public override int GetHashCode()
  {
    return 31 /*0x1F*/ * (31 /*0x1F*/ * (this.BrowserName != null ? this.BrowserName.GetHashCode() : 0) + (this.Version != null ? this.Version.GetHashCode() : 0)) + (this.Platform != null ? this.Platform.GetHashCode() : 0);
  }

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Capabilities [BrowserName={0}, Platform={1}, Version={2}]", (object) this.BrowserName, (object) this.Platform.PlatformType.ToString(), (object) this.Version);
  }

  public override bool Equals(object obj)
  {
    return this == obj || obj is DesiredCapabilities desiredCapabilities && (this.BrowserName != null ? (this.BrowserName != desiredCapabilities.BrowserName ? 1 : 0) : (desiredCapabilities.BrowserName != null ? 1 : 0)) == 0 && this.Platform.IsPlatformType(desiredCapabilities.Platform.PlatformType) && (this.Version != null ? (this.Version != desiredCapabilities.Version ? 1 : 0) : (desiredCapabilities.Version != null ? 1 : 0)) == 0;
  }
}
