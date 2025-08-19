// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.FirefoxProfileManager
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Firefox.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#nullable disable
namespace OpenQA.Selenium.Firefox;

public class FirefoxProfileManager
{
  private Dictionary<string, string> profiles = new Dictionary<string, string>();

  public FirefoxProfileManager()
  {
    this.ReadProfiles(FirefoxProfileManager.GetApplicationDataDirectory());
  }

  public ReadOnlyCollection<string> ExistingProfiles
  {
    get => new List<string>((IEnumerable<string>) this.profiles.Keys).AsReadOnly();
  }

  public FirefoxProfile GetProfile(string profileName)
  {
    FirefoxProfile profile = (FirefoxProfile) null;
    if (!string.IsNullOrEmpty(profileName) && this.profiles.ContainsKey(profileName))
    {
      profile = new FirefoxProfile(this.profiles[profileName]);
      if (profile.Port == 0)
        profile.Port = FirefoxDriver.DefaultPort;
    }
    return profile;
  }

  private static string GetApplicationDataDirectory()
  {
    string empty = string.Empty;
    string path2;
    switch (Environment.OSVersion.Platform)
    {
      case PlatformID.Unix:
        path2 = Path.Combine(".mozilla", "firefox");
        break;
      case PlatformID.MacOSX:
        path2 = Path.Combine("Library", Path.Combine("Application Support", "Firefox"));
        break;
      default:
        path2 = Path.Combine("Mozilla", "Firefox");
        break;
    }
    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path2);
  }

  private void ReadProfiles(string appDataDirectory)
  {
    string str1 = Path.Combine(appDataDirectory, "profiles.ini");
    if (!File.Exists(str1))
      return;
    IniFileReader iniFileReader = new IniFileReader(str1);
    foreach (string sectionName in iniFileReader.SectionNames)
    {
      if (sectionName.StartsWith("profile", StringComparison.OrdinalIgnoreCase))
      {
        string key = iniFileReader.GetValue(sectionName, "name");
        bool flag = iniFileReader.GetValue(sectionName, "isrelative") == "1";
        string path2 = iniFileReader.GetValue(sectionName, "path");
        string empty = string.Empty;
        string str2 = !flag ? path2 : Path.Combine(appDataDirectory, path2);
        this.profiles.Add(key, str2);
      }
    }
  }
}
