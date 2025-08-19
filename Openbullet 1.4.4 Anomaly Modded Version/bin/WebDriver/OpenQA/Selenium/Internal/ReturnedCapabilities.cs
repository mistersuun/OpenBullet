// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Internal.ReturnedCapabilities
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Internal;

internal class ReturnedCapabilities : ICapabilities, IHasCapabilitiesDictionary
{
  private readonly Dictionary<string, object> capabilities = new Dictionary<string, object>();

  public ReturnedCapabilities()
  {
  }

  public ReturnedCapabilities(Dictionary<string, object> rawMap)
  {
    if (rawMap == null)
      return;
    foreach (string key in rawMap.Keys)
      this.capabilities[key] = rawMap[key];
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

  public object this[string capabilityName]
  {
    get
    {
      return this.capabilities.ContainsKey(capabilityName) ? this.capabilities[capabilityName] : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The capability {0} is not present in this set of capabilities", (object) capabilityName));
    }
  }

  Dictionary<string, object> IHasCapabilitiesDictionary.CapabilitiesDictionary
  {
    get => this.CapabilitiesDictionary;
  }

  internal Dictionary<string, object> CapabilitiesDictionary => this.capabilities;

  public bool HasCapability(string capability) => this.capabilities.ContainsKey(capability);

  public object GetCapability(string capability)
  {
    object capability1 = (object) null;
    if (this.capabilities.ContainsKey(capability))
      capability1 = this.capabilities[capability];
    return capability1;
  }

  public Dictionary<string, object> ToDictionary() => this.capabilities;

  public override string ToString()
  {
    return JsonConvert.SerializeObject((object) this.capabilities, Formatting.Indented);
  }
}
