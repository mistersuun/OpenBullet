// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Edge.EdgeOptions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Edge;

public class EdgeOptions : DriverOptions
{
  private const string BrowserNameValue = "MicrosoftEdge";
  private const string UseInPrivateBrowsingCapability = "ms:inPrivate";
  private const string ExtensionPathsCapability = "ms:extensionPaths";
  private const string StartPageCapability = "ms:startPage";
  private EdgePageLoadStrategy pageLoadStrategy;
  private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();
  private bool useInPrivateBrowsing;
  private string startPage;
  private List<string> extensionPaths = new List<string>();

  public EdgeOptions() => this.BrowserName = "MicrosoftEdge";

  public bool UseInPrivateBrowsing
  {
    get => this.useInPrivateBrowsing;
    set => this.useInPrivateBrowsing = value;
  }

  public string StartPage
  {
    get => this.startPage;
    set => this.startPage = value;
  }

  public void AddExtensionPath(string extensionPath)
  {
    if (string.IsNullOrEmpty(extensionPath))
      throw new ArgumentException("extensionPath must not be null or empty", nameof (extensionPath));
    this.AddExtensionPaths(extensionPath);
  }

  public void AddExtensionPaths(params string[] extensionPathsToAdd)
  {
    this.AddExtensionPaths((IEnumerable<string>) new List<string>((IEnumerable<string>) extensionPathsToAdd));
  }

  public void AddExtensionPaths(IEnumerable<string> extensionPathsToAdd)
  {
    if (extensionPathsToAdd == null)
      throw new ArgumentNullException(nameof (extensionPathsToAdd), "extensionPathsToAdd must not be null");
    this.extensionPaths.AddRange(extensionPathsToAdd);
  }

  public override void AddAdditionalCapability(string capabilityName, object capabilityValue)
  {
    if (string.IsNullOrEmpty(capabilityName))
      throw new ArgumentException("Capability name may not be null an empty string.", nameof (capabilityName));
    this.additionalCapabilities[capabilityName] = capabilityValue;
  }

  public override ICapabilities ToCapabilities()
  {
    DesiredCapabilities desiredCapabilities = this.GenerateDesiredCapabilities(false);
    if (this.useInPrivateBrowsing)
      desiredCapabilities.SetCapability("ms:inPrivate", (object) true);
    if (!string.IsNullOrEmpty(this.startPage))
      desiredCapabilities.SetCapability("ms:startPage", (object) this.startPage);
    if (this.extensionPaths.Count > 0)
      desiredCapabilities.SetCapability("ms:extensionPaths", (object) this.extensionPaths);
    foreach (KeyValuePair<string, object> additionalCapability in this.additionalCapabilities)
      desiredCapabilities.SetCapability(additionalCapability.Key, additionalCapability.Value);
    return (ICapabilities) desiredCapabilities;
  }
}
