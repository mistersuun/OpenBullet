// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.FirefoxProfile
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;

#nullable disable
namespace OpenQA.Selenium.Firefox;

public class FirefoxProfile
{
  private const string ExtensionFileName = "webdriver.xpi";
  private const string ExtensionResourceId = "WebDriver.FirefoxExt.zip";
  private const string UserPreferencesFileName = "user.js";
  private const string WebDriverPortPreferenceName = "webdriver_firefox_port";
  private const string EnableNativeEventsPreferenceName = "webdriver_enable_native_events";
  private const string AcceptUntrustedCertificatesPreferenceName = "webdriver_accept_untrusted_certs";
  private const string AssumeUntrustedCertificateIssuerPreferenceName = "webdriver_assume_untrusted_issuer";
  private int profilePort;
  private string profileDir;
  private string sourceProfileDir;
  private bool enableNativeEvents;
  private bool loadNoFocusLibrary;
  private bool acceptUntrustedCerts;
  private bool assumeUntrustedIssuer;
  private bool deleteSource;
  private bool deleteOnClean = true;
  private Preferences profilePreferences;
  private Dictionary<string, FirefoxExtension> extensions = new Dictionary<string, FirefoxExtension>();

  public FirefoxProfile()
    : this((string) null)
  {
  }

  public FirefoxProfile(string profileDirectory)
    : this(profileDirectory, false)
  {
  }

  public FirefoxProfile(string profileDirectory, bool deleteSourceOnClean)
  {
    this.sourceProfileDir = profileDirectory;
    this.profilePort = FirefoxDriver.DefaultPort;
    this.enableNativeEvents = FirefoxDriver.DefaultEnableNativeEvents;
    this.acceptUntrustedCerts = FirefoxDriver.AcceptUntrustedCertificates;
    this.assumeUntrustedIssuer = FirefoxDriver.AssumeUntrustedCertificateIssuer;
    this.deleteSource = deleteSourceOnClean;
    this.ReadDefaultPreferences();
    this.profilePreferences.AppendPreferences(this.ReadExistingPreferences());
    this.AddWebDriverExtension();
  }

  public int Port
  {
    get => this.profilePort;
    set => this.profilePort = value;
  }

  public string ProfileDirectory => this.profileDir;

  public bool DeleteAfterUse
  {
    get => this.deleteOnClean;
    set => this.deleteOnClean = value;
  }

  public bool EnableNativeEvents
  {
    get => this.enableNativeEvents;
    set => this.enableNativeEvents = value;
  }

  public bool AlwaysLoadNoFocusLibrary
  {
    get => this.loadNoFocusLibrary;
    set => this.loadNoFocusLibrary = value;
  }

  public bool AcceptUntrustedCertificates
  {
    get => this.acceptUntrustedCerts;
    set => this.acceptUntrustedCerts = value;
  }

  public bool AssumeUntrustedCertificateIssuer
  {
    get => this.assumeUntrustedIssuer;
    set => this.assumeUntrustedIssuer = value;
  }

  public static FirefoxProfile FromBase64String(string base64)
  {
    string tempDirectoryName = FileUtilities.GenerateRandomTempDirectoryName("webdriver.{0}.duplicated");
    using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(base64)))
    {
      using (ZipStorer zipStorer = ZipStorer.Open((Stream) memoryStream, FileAccess.Read))
      {
        foreach (ZipStorer.ZipFileEntry zipFileEntry in zipStorer.ReadCentralDirectory())
        {
          string path2 = zipFileEntry.FilenameInZip.Replace('/', Path.DirectorySeparatorChar);
          string destinationFileName = Path.Combine(tempDirectoryName, path2);
          zipStorer.ExtractFile(zipFileEntry, destinationFileName);
        }
      }
    }
    return new FirefoxProfile(tempDirectoryName, true);
  }

  public void AddExtension(string extensionToInstall)
  {
    this.extensions.Add(Path.GetFileNameWithoutExtension(extensionToInstall), new FirefoxExtension(extensionToInstall));
  }

  public void SetPreference(string name, string value)
  {
    this.profilePreferences.SetPreference(name, value);
  }

  public void SetPreference(string name, int value)
  {
    this.profilePreferences.SetPreference(name, value);
  }

  public void SetPreference(string name, bool value)
  {
    this.profilePreferences.SetPreference(name, value);
  }

  [Obsolete("Use the FirefoxOptions class to set a proxy for Firefox.")]
  public void SetProxyPreferences(Proxy proxy) => this.InternalSetProxyPreferences(proxy);

  public void WriteToDisk()
  {
    this.profileDir = FirefoxProfile.GenerateProfileDirectoryName();
    if (!string.IsNullOrEmpty(this.sourceProfileDir))
      FileUtilities.CopyDirectory(this.sourceProfileDir, this.profileDir);
    else
      Directory.CreateDirectory(this.profileDir);
    this.InstallExtensions();
    this.DeleteLockFiles();
    this.DeleteExtensionsCache();
    this.UpdateUserPreferences();
  }

  public void Clean()
  {
    if (this.deleteOnClean && !string.IsNullOrEmpty(this.profileDir) && Directory.Exists(this.profileDir))
      FileUtilities.DeleteDirectory(this.profileDir);
    if (!this.deleteSource || string.IsNullOrEmpty(this.sourceProfileDir) || !Directory.Exists(this.sourceProfileDir))
      return;
    FileUtilities.DeleteDirectory(this.sourceProfileDir);
  }

  public string ToBase64String()
  {
    string base64String = string.Empty;
    this.WriteToDisk();
    using (MemoryStream zipStream = new MemoryStream())
    {
      using (ZipStorer zipStorer = ZipStorer.Create((Stream) zipStream, string.Empty))
      {
        foreach (string file in Directory.GetFiles(this.profileDir, "*.*", SearchOption.AllDirectories))
        {
          string fileNameInZip = file.Substring(this.profileDir.Length).Replace(Path.DirectorySeparatorChar, '/');
          zipStorer.AddFile(ZipStorer.CompressionMethod.Deflate, file, fileNameInZip, string.Empty);
        }
      }
      base64String = Convert.ToBase64String(zipStream.ToArray());
      this.Clean();
    }
    return base64String;
  }

  internal void AddWebDriverExtension()
  {
    if (this.extensions.ContainsKey("webdriver"))
      return;
    this.extensions.Add("webdriver", new FirefoxExtension("webdriver.xpi", "WebDriver.FirefoxExt.zip"));
  }

  internal void RemoveWebDriverExtension()
  {
    if (!this.extensions.ContainsKey("webdriver"))
      return;
    this.extensions.Remove("webdriver");
  }

  internal void InternalSetProxyPreferences(Proxy proxy)
  {
    if (proxy == null)
      throw new ArgumentNullException(nameof (proxy), "proxy must not be null");
    if (proxy.Kind == ProxyKind.Unspecified)
      return;
    this.SetPreference("network.proxy.type", (int) proxy.Kind);
    switch (proxy.Kind)
    {
      case ProxyKind.Manual:
        this.SetPreference("network.proxy.no_proxies_on", string.Empty);
        this.SetManualProxyPreference("ftp", proxy.FtpProxy);
        this.SetManualProxyPreference("http", proxy.HttpProxy);
        this.SetManualProxyPreference("ssl", proxy.SslProxy);
        this.SetManualProxyPreference("socks", proxy.SocksProxy);
        if (proxy.BypassProxyAddresses == null)
          break;
        this.SetPreference("network.proxy.no_proxies_on", proxy.BypassProxyAddresses);
        break;
      case ProxyKind.ProxyAutoConfigure:
        this.SetPreference("network.proxy.autoconfig_url", proxy.ProxyAutoConfigUrl);
        break;
    }
  }

  private static string GenerateProfileDirectoryName()
  {
    return FileUtilities.GenerateRandomTempDirectoryName("anonymous.{0}.webdriver-profile");
  }

  private void DeleteLockFiles()
  {
    File.Delete(Path.Combine(this.profileDir, ".parentlock"));
    File.Delete(Path.Combine(this.profileDir, "parent.lock"));
  }

  private void InstallExtensions()
  {
    foreach (string key in this.extensions.Keys)
      this.extensions[key].Install(this.profileDir);
  }

  private void DeleteExtensionsCache()
  {
    string path = Path.Combine(new DirectoryInfo(Path.Combine(this.profileDir, "extensions")).Parent.FullName, "extensions.cache");
    if (!File.Exists(path))
      return;
    File.Delete(path);
  }

  private void UpdateUserPreferences()
  {
    if (this.profilePort == 0)
      throw new WebDriverException("You must set the port to listen on before updating user preferences file");
    string str = Path.Combine(this.profileDir, "user.js");
    if (File.Exists(str))
    {
      try
      {
        File.Delete(str);
      }
      catch (Exception ex)
      {
        throw new WebDriverException("Cannot delete existing user preferences", ex);
      }
    }
    this.profilePreferences.SetPreference("webdriver_firefox_port", this.profilePort);
    this.profilePreferences.SetPreference("webdriver_enable_native_events", this.enableNativeEvents);
    this.profilePreferences.SetPreference("webdriver_accept_untrusted_certs", this.acceptUntrustedCerts);
    this.profilePreferences.SetPreference("webdriver_assume_untrusted_issuer", this.assumeUntrustedIssuer);
    string preference = this.profilePreferences.GetPreference("browser.startup.homepage");
    if (!string.IsNullOrEmpty(preference))
    {
      this.profilePreferences.SetPreference("startup.homepage_welcome_url", string.Empty);
      if (preference != "about:blank")
        this.profilePreferences.SetPreference("browser.startup.page", 1);
    }
    this.profilePreferences.WriteToFile(str);
  }

  private void ReadDefaultPreferences()
  {
    using (Stream resourceStream = ResourceUtilities.GetResourceStream("webdriver.json", "WebDriver.FirefoxPreferences"))
    {
      using (StreamReader streamReader = new StreamReader(resourceStream))
      {
        Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd(), (JsonConverter) new ResponseValueJsonConverter());
        this.profilePreferences = new Preferences(dictionary["frozen"] as Dictionary<string, object>, dictionary["mutable"] as Dictionary<string, object>);
      }
    }
  }

  private Dictionary<string, string> ReadExistingPreferences()
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    try
    {
      if (!string.IsNullOrEmpty(this.sourceProfileDir))
      {
        string path = Path.Combine(this.sourceProfileDir, "user.js");
        if (File.Exists(path))
        {
          foreach (string readAllLine in File.ReadAllLines(path))
          {
            if (readAllLine.StartsWith("user_pref(\"", StringComparison.OrdinalIgnoreCase))
            {
              string str = readAllLine.Substring("user_pref(\"".Length);
              str.Substring(0, str.Length - ");".Length);
              string[] strArray = readAllLine.Split(new string[1]
              {
                ","
              }, StringSplitOptions.None);
              strArray[0] = strArray[0].Substring(0, strArray[0].Length - 1);
              dictionary.Add(strArray[0].Trim(), strArray[1].Trim());
            }
          }
        }
      }
    }
    catch (IOException ex)
    {
      throw new WebDriverException(string.Empty, (Exception) ex);
    }
    return dictionary;
  }

  private void SetManualProxyPreference(string key, string settingString)
  {
    if (settingString == null)
      return;
    string[] strArray = settingString.Split(':');
    this.SetPreference("network.proxy." + key, strArray[0]);
    if (strArray.Length <= 1)
      return;
    this.SetPreference($"network.proxy.{key}_port", int.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture));
  }
}
