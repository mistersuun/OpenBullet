// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.FirefoxDriver
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace OpenQA.Selenium.Firefox;

public class FirefoxDriver : RemoteWebDriver
{
  public static readonly string ProfileCapabilityName = "firefox_profile";
  public static readonly string BinaryCapabilityName = "firefox_binary";
  public static readonly int DefaultPort = 7055;
  public static readonly bool DefaultEnableNativeEvents = Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows);
  public static readonly bool AcceptUntrustedCertificates = true;
  public static readonly bool AssumeUntrustedCertificateIssuer = true;

  public FirefoxDriver()
    : this(new FirefoxOptions((FirefoxProfile) null, (FirefoxBinary) null, (DesiredCapabilities) null))
  {
  }

  public FirefoxDriver(FirefoxOptions options)
    : this(FirefoxDriver.CreateService(options), options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public FirefoxDriver(FirefoxDriverService service)
    : this(service, new FirefoxOptions(), RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public FirefoxDriver(string geckoDriverDirectory)
    : this(geckoDriverDirectory, new FirefoxOptions())
  {
  }

  public FirefoxDriver(string geckoDriverDirectory, FirefoxOptions options)
    : this(geckoDriverDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public FirefoxDriver(
    string geckoDriverDirectory,
    FirefoxOptions options,
    TimeSpan commandTimeout)
    : this(FirefoxDriverService.CreateDefaultService(geckoDriverDirectory), options, commandTimeout)
  {
  }

  public FirefoxDriver(FirefoxDriverService service, FirefoxOptions options)
    : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public FirefoxDriver(
    FirefoxDriverService service,
    FirefoxOptions options,
    TimeSpan commandTimeout)
    : base(FirefoxDriver.CreateExecutor(service, options, commandTimeout), FirefoxDriver.ConvertOptionsToCapabilities(options))
  {
  }

  public override IFileDetector FileDetector
  {
    get => base.FileDetector;
    set
    {
    }
  }

  public bool IsMarionette => this.IsSpecificationCompliant;

  protected virtual void PrepareEnvironment()
  {
  }

  private static ICommandExecutor CreateExecutor(
    FirefoxDriverService service,
    FirefoxOptions options,
    TimeSpan commandTimeout)
  {
    if (options.UseLegacyImplementation)
      return FirefoxDriver.CreateExtensionConnection(new FirefoxBinary(options.BrowserExecutableLocation), options.Profile ?? new FirefoxProfile(), commandTimeout);
    return service != null ? (ICommandExecutor) new DriverServiceCommandExecutor((DriverService) service, commandTimeout) : throw new ArgumentNullException(nameof (service), "You requested a service-based implementation, but passed in a null service object.");
  }

  private static ICommandExecutor CreateExtensionConnection(
    FirefoxBinary binary,
    FirefoxProfile profile,
    TimeSpan commandTimeout)
  {
    FirefoxProfile profile1 = profile;
    string environmentVariable = Environment.GetEnvironmentVariable("webdriver.firefox.profile");
    if (profile1 == null && environmentVariable != null)
      profile1 = new FirefoxProfileManager().GetProfile(environmentVariable);
    else if (profile1 == null)
      profile1 = new FirefoxProfile();
    return (ICommandExecutor) new FirefoxDriverCommandExecutor(binary, profile1, "localhost", commandTimeout);
  }

  private static ICapabilities ConvertOptionsToCapabilities(FirefoxOptions options)
  {
    ICapabilities capabilities = options != null ? options.ToCapabilities() : throw new ArgumentNullException(nameof (options), "options must not be null");
    if (options.UseLegacyImplementation)
      capabilities = FirefoxDriver.RemoveUnneededCapabilities(capabilities);
    return capabilities;
  }

  private static ICapabilities RemoveUnneededCapabilities(ICapabilities capabilities)
  {
    DesiredCapabilities desiredCapabilities = capabilities as DesiredCapabilities;
    desiredCapabilities.CapabilitiesDictionary.Remove(FirefoxDriver.ProfileCapabilityName);
    desiredCapabilities.CapabilitiesDictionary.Remove(FirefoxDriver.BinaryCapabilityName);
    return (ICapabilities) desiredCapabilities;
  }

  private static FirefoxOptions CreateOptionsFromCapabilities(ICapabilities capabilities)
  {
    FirefoxBinary binary1 = FirefoxDriver.ExtractBinary(capabilities);
    FirefoxProfile profile = FirefoxDriver.ExtractProfile(capabilities);
    DesiredCapabilities desiredCapabilities = FirefoxDriver.RemoveUnneededCapabilities(capabilities) as DesiredCapabilities;
    FirefoxBinary binary2 = binary1;
    DesiredCapabilities capabilities1 = desiredCapabilities;
    return new FirefoxOptions(profile, binary2, capabilities1);
  }

  private static FirefoxBinary ExtractBinary(ICapabilities capabilities)
  {
    return capabilities.GetCapability(FirefoxDriver.BinaryCapabilityName) != null ? new FirefoxBinary(capabilities.GetCapability(FirefoxDriver.BinaryCapabilityName).ToString()) : new FirefoxBinary();
  }

  private static FirefoxProfile ExtractProfile(ICapabilities capabilities)
  {
    FirefoxProfile profile = new FirefoxProfile();
    if (capabilities.GetCapability(FirefoxDriver.ProfileCapabilityName) != null)
    {
      object capability = capabilities.GetCapability(FirefoxDriver.ProfileCapabilityName);
      FirefoxProfile firefoxProfile = capability as FirefoxProfile;
      string base64 = capability as string;
      if (firefoxProfile != null)
        profile = firefoxProfile;
      else if (base64 != null)
      {
        try
        {
          profile = FirefoxProfile.FromBase64String(base64);
        }
        catch (IOException ex)
        {
          throw new WebDriverException("Unable to create profile from specified string", (Exception) ex);
        }
      }
    }
    if (capabilities.GetCapability(CapabilityType.Proxy) != null)
    {
      Proxy proxy1 = (Proxy) null;
      object capability = capabilities.GetCapability(CapabilityType.Proxy);
      Proxy proxy2 = capability as Proxy;
      Dictionary<string, object> settings = capability as Dictionary<string, object>;
      if (proxy2 != null)
        proxy1 = proxy2;
      else if (settings != null)
        proxy1 = new Proxy(settings);
      profile.SetProxyPreferences(proxy1);
    }
    if (capabilities.GetCapability(CapabilityType.AcceptSslCertificates) != null)
    {
      bool capability = (bool) capabilities.GetCapability(CapabilityType.AcceptSslCertificates);
      profile.AcceptUntrustedCertificates = capability;
    }
    return profile;
  }

  private static FirefoxDriverService CreateService(FirefoxOptions options)
  {
    return options != null && options.UseLegacyImplementation ? (FirefoxDriverService) null : FirefoxDriverService.CreateDefaultService();
  }
}
