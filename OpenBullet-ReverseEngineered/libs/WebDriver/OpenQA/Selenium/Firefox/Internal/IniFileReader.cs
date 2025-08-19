// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.Internal.IniFileReader
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#nullable disable
namespace OpenQA.Selenium.Firefox.Internal;

internal class IniFileReader
{
  private Dictionary<string, Dictionary<string, string>> iniFileStore = new Dictionary<string, Dictionary<string, string>>();

  public IniFileReader(string fileName)
  {
    if (string.IsNullOrEmpty(fileName))
      throw new ArgumentNullException(nameof (fileName), "File name must not be null or empty");
    if (!File.Exists(fileName))
      throw new FileNotFoundException("INI file not found", fileName);
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    string key = string.Empty;
    foreach (string readAllLine in File.ReadAllLines(fileName))
    {
      if (!string.IsNullOrEmpty(readAllLine.Trim()) && !readAllLine.StartsWith(";", StringComparison.OrdinalIgnoreCase))
      {
        if (readAllLine.StartsWith("[", StringComparison.OrdinalIgnoreCase) && readAllLine.EndsWith("]", StringComparison.OrdinalIgnoreCase))
        {
          if (!string.IsNullOrEmpty(key))
            this.iniFileStore.Add(key, dictionary);
          key = readAllLine.Substring(1, readAllLine.Length - 2).ToUpperInvariant();
          dictionary = new Dictionary<string, string>();
        }
        else
        {
          string[] strArray = readAllLine.Split(new char[1]
          {
            '='
          }, 2);
          string upperInvariant = strArray[0].ToUpperInvariant();
          string empty = string.Empty;
          if (strArray.Length > 1)
            empty = strArray[1];
          dictionary.Add(upperInvariant, empty);
        }
      }
    }
    this.iniFileStore.Add(key, dictionary);
  }

  public ReadOnlyCollection<string> SectionNames
  {
    get
    {
      return new ReadOnlyCollection<string>((IList<string>) new List<string>((IEnumerable<string>) this.iniFileStore.Keys));
    }
  }

  public string GetValue(string sectionName, string valueName)
  {
    string key1 = !string.IsNullOrEmpty(sectionName) ? sectionName.ToUpperInvariant() : throw new ArgumentNullException(nameof (sectionName), "Section name cannot be null or empty");
    string key2 = !string.IsNullOrEmpty(valueName) ? valueName.ToUpperInvariant() : throw new ArgumentNullException(nameof (valueName), "Value name cannot be null or empty");
    Dictionary<string, string> dictionary = this.iniFileStore.ContainsKey(key1) ? this.iniFileStore[key1] : throw new ArgumentException("Section does not exist: " + sectionName, nameof (sectionName));
    return dictionary.ContainsKey(key2) ? dictionary[key2] : throw new ArgumentException("Value does not exist: " + valueName, nameof (valueName));
  }
}
